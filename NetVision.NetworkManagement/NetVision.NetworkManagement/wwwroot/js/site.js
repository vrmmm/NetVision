/**
 * ==============================================================================
 * NETVISION KURUMSAL AĞ YÖNETİMİ & İZLEME PLATFORMU (site.js)
 * Manavgat Belediyesi 8 Katlı Akıllı Bina Ağ Yönetimi İstemci Motoru
 * ==============================================================================
 * Bu dosya sitedeki tüm etkileşimleri yönetir:
 * 1. Canlı Ping ve ICMP teşhis aracı çalıştırma ve terminal çıktısı basma
 * 2. Topoloji haritasında düğümlere tıklandığında cihaz detay modalını açma
 * 3. Hata simülasyonu (SimulateFault) ve alarmları sıfırlama (ClearAlarms)
 * 4. Cisco CLI konfigürasyonlarını panoya kopyalama
 * 5. Cihaz envanteri ve tablolar için dinamik arama/filtreleme
 * ==============================================================================
 */

document.addEventListener('DOMContentLoaded', function () {
    console.log("NetVision Ağ Yönetim Platformu Başlatıldı - Manavgat Belediyesi");

    // --------------------------------------------------------------------------
    // 1. CANLI PING / ICMP TEŞHİS MOTORU
    // Sitede Ne İşe Yarar: PingTool sayfasında kullanıcının girdiği hedef IP'ye
    // AJAX ile istek atıp terminal penceresinde adım adım ping çıktılarını simüle eder.
    // --------------------------------------------------------------------------
    const pingBtn = document.getElementById('runPingBtn');
    const pingTargetInput = document.getElementById('pingTargetInput');
    const pingTerminalOutput = document.getElementById('pingTerminalOutput');
    const pingPresetSelect = document.getElementById('pingPresetSelect');

    if (pingPresetSelect && pingTargetInput) {
        // Hızlı hedef seçim kutusundan IP seçildiğinde input'a yaz
        pingPresetSelect.addEventListener('change', function () {
            if (this.value) {
                pingTargetInput.value = this.value;
            }
        });
    }

    if (pingBtn && pingTargetInput && pingTerminalOutput) {
        pingBtn.addEventListener('click', function () {
            const target = pingTargetInput.value.trim();
            if (!target) {
                alert("Lütfen geçerli bir hedef IP veya cihaz adı giriniz.");
                return;
            }

            // Butonu yükleniyor durumuna getir
            pingBtn.disabled = true;
            pingBtn.innerHTML = '<span>Paketler Gönderiliyor...</span>';
            pingTerminalOutput.innerHTML = `> netvision-diag --target ${target}\n> ICMP echo isteği başlatılıyor...\n`;

            fetch('/Home/ExecutePing', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ targetIp: target })
            })
            .then(res => res.json())
            .then(response => {
                pingBtn.disabled = false;
                pingBtn.innerHTML = '<span>Ping Testini Başlat</span>';

                if (response.success && response.data) {
                    const lines = response.data.terminalOutputLines;
                    let lineIndex = 0;
                    pingTerminalOutput.innerHTML = `> ping ${response.data.targetIp} (${response.data.targetName})\n\n`;

                    // Satırları daktilo gibi sırayla ekrana bas
                    const interval = setInterval(() => {
                        if (lineIndex < lines.length) {
                            pingTerminalOutput.innerHTML += lines[lineIndex] + '\n';
                            pingTerminalOutput.scrollTop = pingTerminalOutput.scrollHeight;
                            lineIndex++;
                        } else {
                            clearInterval(interval);
                        }
                    }, 200);
                } else {
                    pingTerminalOutput.innerHTML += `Hata: ${response.message || 'Ping testi başarısız oldu.'}\n`;
                }
            })
            .catch(err => {
                pingBtn.disabled = false;
                pingBtn.innerHTML = '<span>Ping Testini Başlat</span>';
                pingTerminalOutput.innerHTML += `Bağlantı Hatası: ${err.message}\n`;
            });
        });
    }

    // --------------------------------------------------------------------------
    // 2. HATA SİMÜLASYONU VE ALARM TETİKLEME (FAULT INJECTION)
    // Sitede Ne İşe Yarar: 'Hata Simüle Et' butonuna tıklandığında sisteme
    // Port Down, Yüksek CPU veya Güvenlik ihlali gibi alarmlar enjekte eder.
    // --------------------------------------------------------------------------
    const simulateFaultBtns = document.querySelectorAll('.simulate-fault-btn');
    const clearAlarmsBtn = document.getElementById('clearAlarmsBtn');

    simulateFaultBtns.forEach(btn => {
        btn.addEventListener('click', function () {
            const faultType = this.getAttribute('data-fault') || 'link_down';
            btn.disabled = true;

            fetch('/Home/SimulateFault', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ faultType: faultType })
            })
            .then(res => res.json())
            .then(data => {
                btn.disabled = false;
                // Sayfada alarm listesi varsa yenile veya bildirim ver
                alert("Simülasyon Uyarısı: Ağ olayı sisteme işlendi. Alarmlar sayfasında görüntülenebilir.");
                window.location.reload();
            })
            .catch(err => {
                btn.disabled = false;
                console.error("Hata simülasyon hatası:", err);
            });
        });
    });

    if (clearAlarmsBtn) {
        clearAlarmsBtn.addEventListener('click', function () {
            fetch('/Home/ClearAlarms', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            })
            .then(res => res.json())
            .then(() => {
                alert("Tüm alarmlar temizlendi.");
                window.location.reload();
            });
        });
    }

    // --------------------------------------------------------------------------
    // 3. TOPOLOJİ DÜĞÜM DETAYLARI VE MODAL PENCERESİ
    // Sitede Ne İşe Yarar: Topoloji sayfasında veya envanterde bir cihaza tıklandığında
    // modal pencerede o cihazın IP, MAC, Port, Departman ve durum bilgilerini açar.
    // --------------------------------------------------------------------------
    const deviceModal = document.getElementById('deviceDetailModal');
    const modalCloseBtn = document.getElementById('modalCloseBtn');
    const topologyNodes = document.querySelectorAll('.topology-node-box');

    function showDeviceModal(devId) {
        if (!deviceModal) return;

        fetch(`/Home/GetDeviceDetail?id=${encodeURIComponent(devId)}`)
            .then(res => res.json())
            .then(dev => {
                if (dev) {
                    document.getElementById('modalDevName').textContent = dev.name;
                    document.getElementById('modalDevId').textContent = dev.id;
                    document.getElementById('modalDevType').textContent = dev.type;
                    document.getElementById('modalDevIp').textContent = dev.ipAddress;
                    document.getElementById('modalDevMac').textContent = dev.macAddress;
                    document.getElementById('modalDevVlan').textContent = dev.vlan;
                    document.getElementById('modalDevFloor').textContent = dev.floor;
                    document.getElementById('modalDevPort').textContent = dev.port;
                    document.getElementById('modalDevDesc').textContent = dev.description || 'Aktif ağ bileşeni';

                    const statusBadge = document.getElementById('modalDevStatus');
                    statusBadge.textContent = dev.status;
                    statusBadge.className = `badge ${dev.status === 'Online' ? 'badge-online' : 'badge-offline'}`;

                    deviceModal.classList.add('show');
                }
            })
            .catch(err => console.error("Cihaz detayı alınamadı:", err));
    }

    topologyNodes.forEach(node => {
        node.addEventListener('click', function () {
            topologyNodes.forEach(n => n.classList.remove('selected'));
            this.classList.add('selected');
            const devId = this.getAttribute('data-id');
            if (devId) {
                showDeviceModal(devId);
            }
        });
    });

    if (modalCloseBtn && deviceModal) {
        modalCloseBtn.addEventListener('click', function () {
            deviceModal.classList.remove('show');
        });

        deviceModal.addEventListener('click', function (e) {
            if (e.target === deviceModal) {
                deviceModal.classList.remove('show');
            }
        });
    }

    // --------------------------------------------------------------------------
    // 4. CISCO KONFİGÜRASYONUNU PANOUYA KOPYALAMA
    // Sitede Ne İşe Yarar: Cisco CLI sayfasındaki 'Konfigürasyonu Kopyala' butonuna
    // basıldığında CLI kodunu kullanıcının panosuna (clipboard) aktarır.
    // --------------------------------------------------------------------------
    const copyConfigBtn = document.getElementById('copyConfigBtn');
    const configCodeBody = document.getElementById('ciscoConfigBody');

    if (copyConfigBtn && configCodeBody) {
        copyConfigBtn.addEventListener('click', function () {
            const codeText = configCodeBody.innerText || configCodeBody.textContent;
            navigator.clipboard.writeText(codeText).then(() => {
                const originalHtml = copyConfigBtn.innerHTML;
                copyConfigBtn.innerHTML = '<span>Kopyalandı!</span>';
                setTimeout(() => {
                    copyConfigBtn.innerHTML = originalHtml;
                }, 2000);
            });
        });
    }
});
