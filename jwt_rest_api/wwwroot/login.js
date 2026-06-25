document.addEventListener('DOMContentLoaded', () => {
    const loginForm = document.getElementById('loginForm');
    const togglePassword = document.getElementById('togglePassword');
    const passwordInput = document.getElementById('password');
    const eyeIcon = document.getElementById('eyeIcon');
    const errorMsg = document.getElementById('errorMsg');
    const errorText = document.getElementById('errorText');

    // Toggle password visibility
    togglePassword.addEventListener('click', () => {
        const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
        passwordInput.setAttribute('type', type);

        if (type === 'text') {
            eyeIcon.classList.remove('ph-eye');
            eyeIcon.classList.add('ph-eye-slash');
        } else {
            eyeIcon.classList.remove('ph-eye-slash');
            eyeIcon.classList.add('ph-eye');
        }
    });

    // Handle form submission
    loginForm.addEventListener('submit', async (e) => {
        e.preventDefault();

        const email = document.getElementById('email').value;
        const password = passwordInput.value;
        const submitBtn = loginForm.querySelector('button[type="submit"]');

        // Reset error state
        errorMsg.classList.add('hidden');

        // Show loading state on button
        const originalBtnContent = submitBtn.innerHTML;
        submitBtn.innerHTML = `<i class="ph ph-spinner animate-spin text-xl"></i><span>Signing in...</span>`;
        submitBtn.disabled = true;

        try {
            // Setup for your actual API endpoint based on your project structure
            // Example: /api/auth/login
            const response = await fetch('/api/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ email, password })
            });

            if (response.ok) {
                const data = await response.json();
                // Assuming the API returns a JWT token
                if (data.token) {
                    localStorage.setItem('token', data.token);
                    // Redirect to dashboard
                    window.location.href = '/index.html';
                } else {
                    throw new Error('Token not received from server');
                }
            } else {
                let errorData;
                try {
                    errorData = await response.json();
                } catch (jsonErr) {
                    throw new Error('Invalid credentials');
                }
                showError(errorData.message || errorData.title || 'Invalid email or password.');
            }
        } catch (error) {
            console.error('Login error:', error);
            showError(error.message || 'Gagal terhubung ke server. Coba lagi nanti.');
        } finally {
            // Restore button state
            setTimeout(() => {
                if (submitBtn) {
                    submitBtn.innerHTML = originalBtnContent;
                    submitBtn.disabled = false;
                }
            }, 1000);
        }
    });

    function showError(message) {
        errorText.textContent = message;
        errorMsg.classList.remove('hidden');
        // Shake animation for the card
        const container = document.querySelector('.glass-card');
        container.animate([
            { transform: 'translateX(0)' },
            { transform: 'translateX(-10px)' },
            { transform: 'translateX(10px)' },
            { transform: 'translateX(-10px)' },
            { transform: 'translateX(10px)' },
            { transform: 'translateX(0)' }
        ], { duration: 400, easing: 'ease-in-out' });
    }

    // --- FITUR LOGIN GOOGLE ---
    
    // 1. Fungsi yang berjalan saat Google sukses verifikasi
    async function handleGoogleLogin(googleResponse) {
        errorMsg.classList.add('hidden'); // Sembunyikan error lama
        
        try {
            // Kirim token Google ke pintu masuk Admin yang kita buat tadi
            const res = await fetch('/api/auth/login/admin', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ idToken: googleResponse.credential })
            });

            if (res.ok) {
                const data = await res.json();
                if (data.token) {
                    // Berhasil masuk Whitelist! Simpan tiketnya dan pindah ke dashboard
                    localStorage.setItem('token', data.token);
                    window.location.href = '/index.html';
                }
            } else {
                // Email ditolak / Tidak ada di Whitelist
                let errorData;
                try { errorData = await res.json(); } catch(e) {}
                showError(errorData?.error || 'Akses ditolak oleh server.');
            }
        } catch (error) {
            showError('Tidak dapat terhubung ke server.');
        }
    }

    // 2. Fungsi untuk memanggil tombol asli Google
    async function initGoogleLogin() {
        try {
            // Mengambil nomor identitas (Client ID) dari server Anda
            const response = await fetch('/api/auth/config');
            if (!response.ok) throw new Error('Gagal mengambil konfigurasi');
            const data = await response.json();
            
            // Menggambar tombol di kotak kosong yang kita siapkan di HTML
            google.accounts.id.initialize({
                client_id: data.googleClientId,
                callback: handleGoogleLogin
            });
            
            google.accounts.id.renderButton(
                document.getElementById("googleAuthContainer"),
                { theme: "outline", size: "large", width: "100%", text: "continue_with" }
            );
        } catch (error) {
            console.error('Error inisialisasi Google Login:', error);
        }
    }
    
    // 3. Jalankan pembuat tombol saat halaman dibuka
    initGoogleLogin();
});
