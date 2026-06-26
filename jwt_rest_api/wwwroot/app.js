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
            // Generate deterministic colors based on psycho profile string length/characters
            const pProfile = p.psychoProfile || 'Neutral';
            let badgeClass = 'bg-slate-100 text-slate-600';
            
            if (pProfile !== 'Neutral') {
                const charCodeSum = pProfile.split('').reduce((a, b) => a + b.charCodeAt(0), 0);
                const colorIndex = charCodeSum % 5;
                const colors = [
                    'bg-purple-100 text-purple-700',
                    'bg-blue-100 text-blue-700',
                    'bg-green-100 text-green-700',
                    'bg-orange-100 text-orange-700',
                    'bg-pink-100 text-pink-700'
                ];
                badgeClass = colors[colorIndex];
            }

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
    const points = data.trafficData && data.trafficData.length === 6 
        ? data.trafficData 
        : [0, 0, 0, 0, 0, 0]; // fallback

    for (let i = 5; i >= 0; i--) {
        const d = new Date(now.getTime() - i * 60000); // last 5 minutes
        labels.push(`${d.getHours().toString().padStart(2, '0')}:${d.getMinutes().toString().padStart(2, '0')}`);
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

    // ---- Psycho Profile Donut / Radar Chart ----
    const ctxPsycho = document.getElementById('psychoChart').getContext('2d');

    const profileCounts = {};
    let totalProfiles = 0;
    
    let hasBigFive = false;
    let bigFiveTotals = { "Neuroticism": 0, "Extraversion": 0, "Openness": 0, "Agreeableness": 0, "Conscientiousness": 0 };
    let bigFiveCount = 0;

    data.players.forEach(p => {
        let prof = p.psychoProfile || 'Neutral';
        if (prof.startsWith('{')) {
            try {
                let parsed = JSON.parse(prof);
                hasBigFive = true;
                bigFiveTotals["Neuroticism"] += parsed["Neuroticism"] || 0;
                bigFiveTotals["Extraversion"] += parsed["Extraversion"] || 0;
                bigFiveTotals["Openness"] += parsed["Openness"] || 0;
                bigFiveTotals["Agreeableness"] += parsed["Agreeableness"] || 0;
                bigFiveTotals["Conscientiousness"] += parsed["Conscientiousness"] || 0;
                bigFiveCount++;
            } catch(e) { 
                profileCounts['Invalid'] = (profileCounts['Invalid'] || 0) + 1;
            }
        } else {
            profileCounts[prof] = (profileCounts[prof] || 0) + 1;
        }
        totalProfiles++;
    });

    // Destroy existing chart to allow type changes
    if (psychoChartInstance) {
        psychoChartInstance.destroy();
        psychoChartInstance = null;
    }

    if (hasBigFive) {
        // Hide center text container
        const centerContainer = document.getElementById('chart-center-container');
        if (centerContainer) centerContainer.style.display = 'none';

        // Calculate averages
        const avgData = [
            (bigFiveTotals["Neuroticism"] / bigFiveCount).toFixed(1),
            (bigFiveTotals["Extraversion"] / bigFiveCount).toFixed(1),
            (bigFiveTotals["Openness"] / bigFiveCount).toFixed(1),
            (bigFiveTotals["Agreeableness"] / bigFiveCount).toFixed(1),
            (bigFiveTotals["Conscientiousness"] / bigFiveCount).toFixed(1)
        ];

        psychoChartInstance = new Chart(ctxPsycho, {
            type: 'radar',
            data: {
                labels: ['Neuroticism', 'Extraversion', 'Openness', 'Agreeableness', 'Conscientiousness'],
                datasets: [{
                    label: 'Avg Big Five Scores',
                    data: avgData,
                    backgroundColor: 'rgba(168, 85, 247, 0.2)',
                    borderColor: '#a855f7',
                    pointBackgroundColor: '#a855f7',
                    pointBorderColor: '#fff',
                    pointHoverBackgroundColor: '#fff',
                    pointHoverBorderColor: '#a855f7'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    r: {
                        beginAtZero: true
                    }
                },
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            usePointStyle: true,
                            padding: 20,
                            font: { family: "'Inter', sans-serif", size: 11, weight: '500' }
                        }
                    }
                }
            }
        });
    } else {
        // Show center text container
        const centerContainer = document.getElementById('chart-center-container');
        if (centerContainer) centerContainer.style.display = 'flex';
        document.getElementById('chart-center-val').innerText = totalProfiles;

        const labels = Object.keys(profileCounts);
        const chartData = Object.values(profileCounts);
        
        const baseColors = ['#a855f7', '#3b82f6', '#22c55e', '#f59e0b', '#ec4899', '#ef4444', '#06b6d4'];
        const bgColors = labels.map((label, index) => {
            if (label === 'Neutral') return '#cbd5e1';
            return baseColors[index % baseColors.length];
        });

        if (totalProfiles === 0) {
            labels.push('Neutral');
            chartData.push(1);
            bgColors.push('#cbd5e1');
        }

        psychoChartInstance = new Chart(ctxPsycho, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: chartData,
                    backgroundColor: bgColors,
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

// UI Navigation: Switch Sidebar Tabs
function switchTab(tabId) {
    // 1. Hide all sections
    document.getElementById('section-overview').classList.add('hidden');
    document.getElementById('section-users').classList.add('hidden');
    document.getElementById('section-analytics').classList.add('hidden');
    // We can add 'flex' back if it's the analytics tab, but mostly we toggle 'hidden'
    if (tabId === 'analytics') {
        document.getElementById('section-analytics').classList.replace('hidden', 'flex');
    } else {
        document.getElementById('section-analytics').classList.replace('flex', 'hidden');
        document.getElementById(`section-${tabId}`).classList.remove('hidden');
    }

    // 2. Reset all nav item styles to default
    const tabs = ['overview', 'users', 'analytics'];
    tabs.forEach(t => {
        const nav = document.getElementById(`nav-${t}`);
        if (nav) {
            nav.className = 'flex items-center gap-3 px-4 py-2.5 rounded-lg font-medium transition-colors cursor-pointer text-slate-500 hover:bg-slate-100 hover:text-purple-600';
        }
    });

    // 3. Highlight active tab
    const activeNav = document.getElementById(`nav-${tabId}`);
    if (activeNav) {
        activeNav.className = 'flex items-center gap-3 px-4 py-2.5 rounded-lg font-medium transition-colors cursor-pointer bg-slate-100 text-purple-600';
    }
}

// UI Navigation: Filter Player Table
function filterTable() {
    const input = document.getElementById("playerSearchInput");
    const filter = input.value.toLowerCase();
    const tbody = document.getElementById("player-table-body");
    const trs = tbody.getElementsByTagName("tr");

    for (let i = 0; i < trs.length; i++) {
        // Find the first td which contains name and email
        const td = trs[i].getElementsByTagName("td")[0];
        if (td) {
            const textValue = td.textContent || td.innerText;
            if (textValue.toLowerCase().indexOf(filter) > -1) {
                trs[i].style.display = "";
            } else {
                trs[i].style.display = "none";
            }
        }
    }
}
