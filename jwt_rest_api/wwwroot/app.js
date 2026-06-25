let trafficChartInstance = null;
let psychoChartInstance = null;

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    // Check for authentication token
    const token = localStorage.getItem('token');
    if (!token) {
        // Not logged in, redirect to login page
        window.location.href = '/login.html';
        return; // Stop further execution
    }

    fetchDashboardData();
    updateAdminProfile();
    
    // Auto refresh every 10 seconds
    setInterval(fetchDashboardData, 10000);
});

function updateAdminProfile() {
    const token = localStorage.getItem('token');
    if (!token) return;

    try {
        // Decode JWT Payload
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));

        const payload = JSON.parse(jsonPayload);
        
        // C# ClaimTypes.Name usually becomes 'unique_name' or 'name', but sometimes the full URL
        const name = payload.unique_name || payload.name || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || "Admin User";
        const email = payload.email || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || "admin@example.com";
        
        // Extract 2 letters for initials
        const initials = name.substring(0, 2).toUpperCase();

        document.getElementById('adminInitials').innerText = initials;
        document.getElementById('adminName').innerText = name;
        document.getElementById('adminEmail').innerText = email;
    } catch (e) {
        console.error("Failed to parse token for profile:", e);
    }
}

async function fetchDashboardData() {
    try {
        const token = localStorage.getItem('token');
        const response = await fetch('/api/admin/dashboard', {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (response.status === 401 || response.status === 403) {
            // Token expired or invalid
            localStorage.removeItem('token');
            window.location.href = '/login.html';
            return;
        }

        if (!response.ok) throw new Error('Failed to fetch dashboard data');

        const data = await response.json();
        updateUI(data);
    } catch (error) {
        console.error('Error fetching dashboard data:', error);
    }
}

function logout() {
    localStorage.removeItem('token');
    window.location.href = '/login.html';
}


function updateUI(data) {
    // 1. Update Top Metrics
    document.getElementById('metric-users').innerText = data.totalUsers;
    document.getElementById('metric-requests').innerText = data.totalRequests;

    // Calculate Avg Level
    let avgLevel = 0;
    if (data.players && data.players.length > 0) {
        const totalLvl = data.players.reduce((sum, p) => sum + p.level, 0);
        avgLevel = (totalLvl / data.players.length).toFixed(1);
    }
    document.getElementById('metric-avg-level').innerText = avgLevel;

    // 2. Update Table
    const tbody = document.getElementById('player-table-body');
    tbody.innerHTML = '';

    if (data.players && data.players.length > 0) {
        data.players.forEach(p => {
            let badgeClass = 'bg-slate-100 text-slate-600';
            if (p.psychoProfile === 'Empathetic') badgeClass = 'bg-purple-100 text-purple-700';
            else if (p.psychoProfile === 'Pragmatic') badgeClass = 'bg-green-100 text-green-800';

            const row = `
                <tr>
                    <td class="py-4 border-b border-slate-100 text-sm text-slate-700 font-medium">
                        <div class="flex flex-col">
                            <span class="font-bold text-slate-800">${escapeHtml(p.name)}</span>
                            <span class="text-xs text-slate-500">${escapeHtml(p.email)}</span>
                        </div>
                    </td>
                    <td class="py-4 border-b border-slate-100 text-sm text-slate-700 font-medium">
                        <span class="font-semibold text-slate-700">Lvl ${p.level}</span>
                        <span class="text-xs text-slate-400 ml-1">/ ${p.score} pts</span>
                    </td>
                    <td class="py-4 border-b border-slate-100 text-sm text-slate-700 font-medium">
                        <span class="font-semibold text-slate-700">Day ${p.dayReached}</span>
                        <span class="text-xs text-slate-400 ml-1">(Phase ${p.phaseReached})</span>
                    </td>
                    <td class="py-4 border-b border-slate-100 text-sm text-slate-700 font-medium">
                        <div class="flex items-center gap-1.5">
                            <i class="ph ph-chat-circle-text text-purple-500"></i>
                            <span class="font-semibold text-slate-700">${p.totalInteractions}</span>
                        </div>
                    </td>
                    <td class="py-4 border-b border-slate-100 text-sm text-slate-700 font-medium">
                        <span class="px-2.5 py-1 rounded-full text-xs font-semibold ${badgeClass}">${p.psychoProfile}</span>
                    </td>
                </tr>
            `;
            tbody.insertAdjacentHTML('beforeend', row);
        });
    } else {
        tbody.innerHTML = `<tr><td colspan="5" class="text-center py-8 text-slate-500">No players found yet.</td></tr>`;
    }

    // 3. Update Charts
    updateCharts(data);
}

function updateCharts(data) {
    // ---- Traffic Line Chart (Mocked time series based on total requests to simulate live data) ----
    const ctxTraffic = document.getElementById('trafficChart').getContext('2d');

    // Create some fake historical "requests per minute" points
    const now = new Date();
    const labels = [];
    const points = [];

    for (let i = 5; i >= 0; i--) {
        const d = new Date(now.getTime() - i * 60000); // last 5 minutes
        labels.push(`${d.getHours().toString().padStart(2, '0')}:${d.getMinutes().toString().padStart(2, '0')}`);

        // Mock fluctuating requests per minute (e.g. between 2 and 15)
        points.push(Math.floor(Math.random() * 14) + 2);
    }

    if (trafficChartInstance) {
        trafficChartInstance.data.labels = labels;
        trafficChartInstance.data.datasets[0].data = points;
        trafficChartInstance.data.datasets[0].label = 'Requests / Min';
        trafficChartInstance.update();
    } else {
        trafficChartInstance = new Chart(ctxTraffic, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Requests / Min',
                    data: points,
                    borderColor: '#7c3aed',
                    backgroundColor: 'rgba(124, 58, 237, 0.1)',
                    borderWidth: 2,
                    fill: true,
                    tension: 0.4,
                    pointBackgroundColor: '#ffffff',
                    pointBorderColor: '#7c3aed',
                    pointBorderWidth: 2,
                    pointRadius: 4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { display: false } },
                scales: {
                    y: {
                        beginAtZero: true,
                        suggestedMax: 20,
                        grid: { borderDash: [4, 4], color: '#e2e8f0', drawBorder: false }
                    },
                    x: {
                        grid: { display: false, drawBorder: false }
                    }
                }
            }
        });
    }

    // ---- Psycho Profile Donut Chart ----
    const ctxPsycho = document.getElementById('psychoChart').getContext('2d');

    // Count profiles
    let empathetic = 0, pragmatic = 0, neutral = 0;
    data.players.forEach(p => {
        if (p.psychoProfile === 'Empathetic') empathetic++;
        else if (p.psychoProfile === 'Pragmatic') pragmatic++;
        else neutral++;
    });

    const totalProfiles = empathetic + pragmatic + neutral;
    document.getElementById('chart-center-val').innerText = totalProfiles;

    const psychoData = [empathetic, pragmatic, neutral];
    // Don't show empty chart if 0
    if (totalProfiles === 0) {
        psychoData[2] = 1; // default neutral slice just to show something
    }

    if (psychoChartInstance) {
        psychoChartInstance.data.datasets[0].data = psychoData;
        psychoChartInstance.update();
    } else {
        psychoChartInstance = new Chart(ctxPsycho, {
            type: 'doughnut',
            data: {
                labels: ['Empathetic', 'Pragmatic', 'Neutral'],
                datasets: [{
                    data: psychoData,
                    backgroundColor: ['#a855f7', '#22c55e', '#cbd5e1'],
                    borderWidth: 0,
                    hoverOffset: 4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '75%',
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: { usePointStyle: true, boxWidth: 8, font: { family: "'Outfit', sans-serif" } }
                    }
                }
            }
        });
    }
}

// Utility to prevent XSS
function escapeHtml(unsafe) {
    if (!unsafe) return '';
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}
