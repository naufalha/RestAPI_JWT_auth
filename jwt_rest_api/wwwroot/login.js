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
            // --- Fallback for demo purposes ---
            // If the backend isn't ready or if there's a fetch error, we provide a mock login
            if (email === 'admin@ekatva.com' && password === 'admin') {
                setTimeout(() => {
                    localStorage.setItem('token', 'mock_jwt_token_12345');
                    window.location.href = '/index.html';
                }, 1000);
            } else {
                showError(error.message || 'Failed to connect to the server. Please try again later.');
            }
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
});
