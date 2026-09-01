using System;
using System.Collections.Generic;
using System.Linq;
using NetVision.NetworkManagement.Models;

namespace NetVision.NetworkManagement.Services
{
    // ==============================================================================
    // AĞ VERİ SERVİSİ UYGULAMASI (NetworkDataService)
    // Manavgat Belediyesi 8 Katlı Akıllı Binası için oluşturulan 157 cihazlık
    // gerçekçi kurumsal ağ altyapısını, VLAN, Cisco CLI, ICMP teşhis ve alarm motorunu yönetir.
    // ==============================================================================
    public class NetworkDataService : INetworkDataService
    {
        // Bellekte tutulan dinamik cihaz ve alarm koleksiyonları
        private readonly List<NetworkDevice> _devices = new();
        private readonly List<VlanInfo> _vlans = new();
        private readonly List<FloorSpec> _floors = new();
        private readonly List<ValidationTest> _validationTests = new();
        private readonly List<AlarmLog> _alarms = new();
        private readonly List<CiscoConfigFile> _ciscoConfigs = new();

        public NetworkDataService()
        {
            // Servis ilk ayağa kalktığında kurumsal verileri yükle
            InitializeVlans();
            InitializeFloors();
            InitializeDeviceInventory();
            InitializeValidationTests();
            InitializeCiscoConfigs();
            InitializeDefaultAlarms();
        }

        // --------------------------------------------------------------------------
        // 1. DASHBOARD VERİ ÖZETİ
        // Ana dashboard ekranında bulunan sayaç kartları, grafik verileri ve metrikleri hazırlar.
        // --------------------------------------------------------------------------
        public DashboardViewModel GetDashboardSummary()
        {
            int onlineCount = _devices.Count(d => d.Status == "Online");
            int totalCount = _devices.Count;
            int activeVlanCount = _vlans.Count;

            return new DashboardViewModel
            {
                TotalDeviceCount = totalCount,
                OnlineDeviceCount = onlineCount,
                ActiveVlanCount = activeVlanCount,
                CpuLoadPercentage = 12.4,
                MemoryUsagePercentage = 28.6,
                AverageLatencyMs = 0.42,
                FirewallStatus = "FW-01 Aktif (PAT: 10.10.0.0/16 -> 203.0.113.2)",
                UptimeString = "99.98% (48 Gün 12 Saat Kesintisiz)",
                Vlans = _vlans,
                Floors = _floors,
                CriticalDevices = _devices.Where(d => d.Type == "Core Switch" || d.Type == "Firewall" || d.Type == "Server" || d.Type == "Router").ToList(),
                RecentAlarms = _alarms.OrderByDescending(a => a.Timestamp).Take(5).ToList()
            };
        }

        // --------------------------------------------------------------------------
        // 2. CİHAZ ENVANTERİ (CMDB) FİLTRELEME
        // Kullanıcının arama kutusuna yazdığı metne, seçtiği kata veya VLAN'a göre filtreleme yapar.
        // --------------------------------------------------------------------------
        public List<NetworkDevice> GetDevices(string? search = null, string? floor = null, string? vlan = null)
        {
            var query = _devices.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(d => d.Name.ToLower().Contains(search) ||
                                         d.IpAddress.ToLower().Contains(search) ||
                                         d.MacAddress.ToLower().Contains(search) ||
                                         d.Department.ToLower().Contains(search) ||
                                         d.Type.ToLower().Contains(search) ||
                                         d.Port.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(floor) && floor != "All")
            {
                query = query.Where(d => d.Floor == floor);
            }

            if (!string.IsNullOrWhiteSpace(vlan) && vlan != "All")
            {
                query = query.Where(d => d.Vlan.Contains(vlan));
            }

            return query.OrderBy(d => d.Id).ToList();
        }

        // --------------------------------------------------------------------------
        // 3. TEKİL CİHAZ DETAYI
        // Topolojide veya envanterde bir cihaza tıklandığında detay popup'ı için veriyi çeker.
        // --------------------------------------------------------------------------
        public NetworkDevice? GetDeviceById(string id)
        {
            return _devices.FirstOrDefault(d => d.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        // --------------------------------------------------------------------------
        // 4. VLAN LİSTESİ
        // Sitedeki VLAN kartları, filtre açılır kutuları ve segmentasyon şemasında kullanılır.
        // --------------------------------------------------------------------------
        public List<VlanInfo> GetVlans() => _vlans;

        // --------------------------------------------------------------------------
        // 5. KAT BAZLI ALTYAPI LİSTESİ
        // 8 katlı binanın her katındaki port, PC, kamera, yazıcı ve switch dağılımını verir.
        // --------------------------------------------------------------------------
        public List<FloorSpec> GetFloorSpecs() => _floors;

        // --------------------------------------------------------------------------
        // 6. DOĞRULAMA TEST MATRİSİ
        // 14 maddelik uçtan uca ağ testlerini (DHCP, Inter-VLAN, ACL, NAT, ICMP) sunar.
        // --------------------------------------------------------------------------
        public List<ValidationTest> GetValidationTests(string? category = null)
        {
            if (string.IsNullOrWhiteSpace(category) || category == "All")
                return _validationTests;

            return _validationTests.Where(t => t.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // --------------------------------------------------------------------------
        // 7. CISCO KONFİGÜRASYONLARI
        // CORE-01 Switch, FW-01 ASA Firewall, ISP Router ve Access Switch CLI yapılandırmaları.
        // --------------------------------------------------------------------------
        public List<CiscoConfigFile> GetAllCiscoConfigs() => _ciscoConfigs;

        public CiscoConfigFile? GetCiscoConfig(string deviceKey)
        {
            return _ciscoConfigs.FirstOrDefault(c => c.DeviceKey.Equals(deviceKey, StringComparison.OrdinalIgnoreCase));
        }

        // --------------------------------------------------------------------------
        // 8. CANLI PING & ICMP TEŞHİS MOTORU
        // Kullanıcı arayüzünden girilen IP adresine yönelik gerçekçi ICMP paket simülasyonu çalıştırır.
        // --------------------------------------------------------------------------
        public PingTestResult ExecutePing(string targetIp)
        {
            var matchedDevice = _devices.FirstOrDefault(d => d.IpAddress.Contains(targetIp) || d.Id.Equals(targetIp, StringComparison.OrdinalIgnoreCase) || d.Name.Equals(targetIp, StringComparison.OrdinalIgnoreCase));
            string resolvedIp = matchedDevice != null ? matchedDevice.IpAddress.Split(' ')[0] : targetIp;
            string devName = matchedDevice != null ? matchedDevice.Name : "Bilinmeyen Hedef";

            // Engellenen ACL simülasyonu (VLAN 50'den VLAN 30'a misafir engeli veya geçersiz IP)
            bool isBlocked = targetIp.Contains("10.10.30.") && targetIp.EndsWith(".999");
            bool isSuccess = !isBlocked && !string.IsNullOrWhiteSpace(resolvedIp);

            var result = new PingTestResult
            {
                TargetIp = resolvedIp,
                TargetName = devName,
                IsSuccess = isSuccess,
                PacketsSent = 4,
                PacketsReceived = isSuccess ? 4 : 0,
                PacketLossPercentage = isSuccess ? 0 : 100,
                MinRttMs = isSuccess ? 0.28 : 0,
                MaxRttMs = isSuccess ? 0.62 : 0,
                AvgRttMs = isSuccess ? 0.41 : 0,
                Ttl = 128
            };

            result.TerminalOutputLines.Add($"Pinging {resolvedIp} with 32 bytes of data:");
            if (isSuccess)
            {
                result.TerminalOutputLines.Add($"Reply from {resolvedIp}: bytes=32 time=0.38ms TTL=128");
                result.TerminalOutputLines.Add($"Reply from {resolvedIp}: bytes=32 time=0.45ms TTL=128");
                result.TerminalOutputLines.Add($"Reply from {resolvedIp}: bytes=32 time=0.31ms TTL=128");
                result.TerminalOutputLines.Add($"Reply from {resolvedIp}: bytes=32 time=0.52ms TTL=128");
                result.TerminalOutputLines.Add("");
                result.TerminalOutputLines.Add($"Ping statistics for {resolvedIp}:");
                result.TerminalOutputLines.Add($"    Packets: Sent = 4, Received = 4, Lost = 0 (0% loss),");
                result.TerminalOutputLines.Add($"Approximate round trip times in milli-seconds:");
                result.TerminalOutputLines.Add($"    Minimum = 0.28ms, Maximum = 0.62ms, Average = 0.41ms");
            }
            else
            {
                result.TerminalOutputLines.Add($"Request timed out.");
                result.TerminalOutputLines.Add($"Request timed out.");
                result.TerminalOutputLines.Add($"Request timed out.");
                result.TerminalOutputLines.Add($"Request timed out.");
                result.TerminalOutputLines.Add("");
                result.TerminalOutputLines.Add($"Ping statistics for {resolvedIp}:");
                result.TerminalOutputLines.Add($"    Packets: Sent = 4, Received = 0, Lost = 4 (100% loss)");
            }

            return result;
        }

        // --------------------------------------------------------------------------
        // 9. ALARM LİSTESİ
        // Syslog ve hata olaylarını kronolojik olarak döndürür.
        // --------------------------------------------------------------------------
        public List<AlarmLog> GetAlarms()
        {
            return _alarms.OrderByDescending(a => a.Timestamp).ToList();
        }

        // --------------------------------------------------------------------------
        // 10. HATA SİMÜLASYONU VE ALARM TEMİZLEME
        // Canlı arayüzde bir arıza senaryosu oluşturur (Örn: Port Down, Yüksek CPU, Link Kopması).
        // --------------------------------------------------------------------------
        public void SimulateNetworkFault(string faultType)
        {
            string msg;
            string dev;
            string sev = "WARNING";

            switch (faultType?.ToLower())
            {
                case "link_down":
                    dev = "SW-03-01";
                    msg = "%LINK-3-UPDOWN: Interface GigabitEthernet0/1, changed state to down";
                    sev = "CRITICAL";
                    break;
                case "high_cpu":
                    dev = "CORE-01";
                    msg = "%SYS-4-CPURISINGTHRESHOLD: Threshold: Total CPU Utilization(92%) reached";
                    sev = "WARNING";
                    break;
                case "security_violation":
                    dev = "SW-01-01";
                    msg = "%PORT_SECURITY-2-PVIOLATION: Security violation occurred on port Fa0/5 (Unauthorized MAC)";
                    sev = "CRITICAL";
                    break;
                default:
                    dev = "FW-01";
                    msg = "%ASA-4-106023: Denied icmp src outside:198.51.100.55 dst inside:10.10.10.1 (type 8, code 0)";
                    sev = "INFO";
                    break;
            }

            _alarms.Insert(0, new AlarmLog
            {
                SourceDevice = dev,
                Message = msg,
                Severity = sev,
                Category = "Simulation",
                Timestamp = DateTime.Now,
                IsActive = true
            });
        }

        public void ClearAllAlarms()
        {
            _alarms.Clear();
            _alarms.Add(new AlarmLog
            {
                SourceDevice = "SYSTEM",
                Message = "Tüm alarmlar ağ yöneticisi tarafından temizlendi. Sistem nominal durumda.",
                Severity = "SUCCESS",
                Category = "System",
                Timestamp = DateTime.Now,
                IsActive = false
            });
        }

        // ==========================================================================
        // ÖZEL BAŞLANGIÇ VERİSİ OLUŞTURUCULARI (INITIALIZERS)
        // ==========================================================================

        private void InitializeVlans()
        {
            _vlans.Add(new VlanInfo { Id = 10, Name = "MANAGEMENT", Subnet = "10.10.10.0/24", Gateway = "10.10.10.1", DeviceCount = 12, Purpose = "Omurga switch, firewall ve erişim switchlerinin SSH/SNMP yönetim ağı.", StatusColor = "#8E8E8E" });
            _vlans.Add(new VlanInfo { Id = 20, Name = "USERS", Subnet = "10.10.20.0/24", Gateway = "10.10.20.1", DeviceCount = 145, Purpose = "8 kattaki tüm personel bilgisayarları ve uç istemciler.", StatusColor = "#E6E6E6" });
            _vlans.Add(new VlanInfo { Id = 30, Name = "SERVERS", Subnet = "10.10.30.0/24", Gateway = "10.10.30.1", DeviceCount = 6, Purpose = "Sistem odası sunucu çiftliği (AD, DNS, Web, Veritabanı, Yedekleme, NetVision).", StatusColor = "#8E8E8E" });
            _vlans.Add(new VlanInfo { Id = 40, Name = "SECURITY", Subnet = "10.10.40.0/24", Gateway = "10.10.40.1", DeviceCount = 32, Purpose = "Bina genelindeki IP CCTV güvenlik kameraları ve NVR kayıt sistemleri.", StatusColor = "#8E8E8E" });
            _vlans.Add(new VlanInfo { Id = 50, Name = "GUEST", Subnet = "10.10.50.0/24", Gateway = "10.10.50.1", DeviceCount = 16, Purpose = "Ziyaretçi kablosuz ağı (Zero-Trust ACL ile iç ağdan tamamen izole).", StatusColor = "#8E8E8E" });
            _vlans.Add(new VlanInfo { Id = 60, Name = "VOICE", Subnet = "10.10.60.0/24", Gateway = "10.10.60.1", DeviceCount = 40, Purpose = "IP Telefonlar (VoIP QoS öncelikli ses trafiği).", StatusColor = "#8E8E8E" });
        }

        private void InitializeFloors()
        {
            _floors.Add(new FloorSpec { Floor = "-2", Name = "Teknik Altyapı / Depo", PcCount = 5, PrinterCount = 1, ApCount = 1, CameraCount = 4, PhoneCount = 1, SwitchModel = "SW-M2-01 (Catalyst 2960-24TT)", SwitchIp = "10.10.10.4", RequiredPorts = 12 });
            _floors.Add(new FloorSpec { Floor = "-1", Name = "Arşiv / Lojistik", PcCount = 10, PrinterCount = 1, ApCount = 1, CameraCount = 4, PhoneCount = 2, SwitchModel = "SW-M1-01 (Catalyst 2960-24TT)", SwitchIp = "10.10.10.5", RequiredPorts = 18 });
            _floors.Add(new FloorSpec { Floor = "0", Name = "Zemin Kat (Resepsiyon / Danışma)", PcCount = 15, PrinterCount = 2, ApCount = 2, CameraCount = 4, PhoneCount = 4, SwitchModel = "SW-GF-01 (Catalyst 2960-48TT)", SwitchIp = "10.10.10.6", RequiredPorts = 27 });
            _floors.Add(new FloorSpec { Floor = "1", Name = "İnsan Kaynakları", PcCount = 20, PrinterCount = 2, ApCount = 2, CameraCount = 4, PhoneCount = 5, SwitchModel = "SW-01-01 (Catalyst 2960-48TT)", SwitchIp = "10.10.10.7", RequiredPorts = 33 });
            _floors.Add(new FloorSpec { Floor = "2", Name = "Muhasebe / Finans", PcCount = 25, PrinterCount = 2, ApCount = 2, CameraCount = 4, PhoneCount = 7, SwitchModel = "SW-02-01 (Catalyst 2960-48TT)", SwitchIp = "10.10.10.8", RequiredPorts = 40 });
            _floors.Add(new FloorSpec { Floor = "3", Name = "Yazılım / Ar-Ge (Çift Switch)", PcCount = 35, PrinterCount = 2, ApCount = 2, CameraCount = 4, PhoneCount = 8, SwitchModel = "SW-03-01 / SW-03-02 (Çift 2960)", SwitchIp = "10.10.10.9, 10.10.10.10", RequiredPorts = 51 });
            _floors.Add(new FloorSpec { Floor = "4", Name = "Yönetim / Toplantı", PcCount = 20, PrinterCount = 1, ApCount = 3, CameraCount = 4, PhoneCount = 7, SwitchModel = "SW-04-01 (Catalyst 2960-48TT)", SwitchIp = "10.10.10.11", RequiredPorts = 35 });
            _floors.Add(new FloorSpec { Floor = "5", Name = "Bilgi İşlem / Sistem Odası", PcCount = 15, PrinterCount = 1, ApCount = 3, CameraCount = 4, PhoneCount = 6, SwitchModel = "SW-05-01 (Catalyst 2960-48TT)", SwitchIp = "10.10.10.12", RequiredPorts = 29 });
        }

        private void InitializeDeviceInventory()
        {
            // 1. Omurga ve Dış Ağ Altyapı Cihazları
            _devices.Add(new NetworkDevice { Id = "ISP-RTR", Name = "ISP-RTR (Cisco 2911)", Type = "Router", IpAddress = "203.0.113.1", MacAddress = "0010.7B2A.1001", Vlan = "WAN", Floor = "Dış Ağ", Department = "ISP Omurga", Port = "Gig0/0", Status = "Online", Description = "İnternet Servis Sağlayıcı Çıkış Yönlendiricisi" });
            _devices.Add(new NetworkDevice { Id = "FW-01", Name = "FW-01 (Cisco ASA 5506-X)", Type = "Firewall", IpAddress = "10.10.10.3 / 203.0.113.2", MacAddress = "00A0.C984.1002", Vlan = "10 / WAN", Floor = "Kat 5", Department = "Sistem Odası", Port = "Gig1/1, Gig1/2", Status = "Online", Description = "Merkezi Güvenlik Duvarı & Dynamic PAT Ağ Geçidi" });
            _devices.Add(new NetworkDevice { Id = "CORE-01", Name = "CORE-01 (Cisco Catalyst 3560)", Type = "Core Switch", IpAddress = "10.10.10.1 (SVI)", MacAddress = "0001.4239.AB01", Vlan = "10,20,30,40,50,60", Floor = "Kat 5", Department = "Merkezi Omurga", Port = "Gig0/1-10", Status = "Online", Description = "Layer 3 Omurga Switch, Inter-VLAN Routing & DHCP Sunucusu" });

            // 2. Sistem Odası Sunucuları (VLAN 30)
            _devices.Add(new NetworkDevice { Id = "SRV-01", Name = "SRV-01 (Active Directory & DNS)", Type = "Server", IpAddress = "10.10.30.10", MacAddress = "0050.7966.3010", Vlan = "30 (SERVERS)", Floor = "Kat 5", Department = "Sistem Odası", Port = "SW-05-01:Fa0/1", Status = "Online", Description = "Etki Alanı ve Birincil DNS Sunucusu" });
            _devices.Add(new NetworkDevice { Id = "SRV-02", Name = "SRV-02 (Web & Intranet Portal)", Type = "Server", IpAddress = "10.10.30.11", MacAddress = "0050.7966.3011", Vlan = "30 (SERVERS)", Floor = "Kat 5", Department = "Sistem Odası", Port = "SW-05-01:Fa0/2", Status = "Online", Description = "Kurumsal İç Web Sunucusu (IIS/Nginx)" });
            _devices.Add(new NetworkDevice { Id = "SRV-03", Name = "SRV-03 (Veritabanı Sunucusu)", Type = "Server", IpAddress = "10.10.30.12", MacAddress = "0050.7966.3012", Vlan = "30 (SERVERS)", Floor = "Kat 5", Department = "Sistem Odası", Port = "SW-05-01:Fa0/3", Status = "Online", Description = "Merkezi Kurumsal Veritabanı (MSSQL/PostgreSQL)" });
            _devices.Add(new NetworkDevice { Id = "SRV-04", Name = "SRV-04 (Yedekleme & Dosya Deposu)", Type = "Server", IpAddress = "10.10.30.13", MacAddress = "0050.7966.3013", Vlan = "30 (SERVERS)", Floor = "Kat 5", Department = "Sistem Odası", Port = "SW-05-01:Fa0/4", Status = "Online", Description = "NAS & Merkezi Dosya Depolama" });
            _devices.Add(new NetworkDevice { Id = "SRV-05", Name = "SRV-05 (Mail & SMTP Gateway)", Type = "Server", IpAddress = "10.10.30.14", MacAddress = "0050.7966.3014", Vlan = "30 (SERVERS)", Floor = "Kat 5", Department = "Sistem Odası", Port = "SW-05-01:Fa0/5", Status = "Online", Description = "Kurumsal E-Posta Sunucusu" });
            _devices.Add(new NetworkDevice { Id = "SRV-06", Name = "SRV-06 (NetVision Monitor & Syslog)", Type = "Server", IpAddress = "10.10.30.15", MacAddress = "0050.7966.3015", Vlan = "30 (SERVERS)", Floor = "Kat 5", Department = "Sistem Odası", Port = "SW-05-01:Fa0/6", Status = "Online", Description = "NetVision Ağ Yönetimi & SNMP İzleme Motoru" });

            // 3. Kat Erişim Switchleri (VLAN 10)
            var switches = new[]
            {
                new { Id = "SW-M2-01", Floor = "-2", FloorName = "Kat -2", Ip = "10.10.10.4", Dept = "Teknik Altyapı / Depo" },
                new { Id = "SW-M1-01", Floor = "-1", FloorName = "Kat -1", Ip = "10.10.10.5", Dept = "Arşiv / Lojistik" },
                new { Id = "SW-GF-01", Floor = "0", FloorName = "Zemin Kat", Ip = "10.10.10.6", Dept = "Resepsiyon / Danışma" },
                new { Id = "SW-01-01", Floor = "1", FloorName = "Kat 1", Ip = "10.10.10.7", Dept = "İnsan Kaynakları" },
                new { Id = "SW-02-01", Floor = "2", FloorName = "Kat 2", Ip = "10.10.10.8", Dept = "Muhasebe / Finans" },
                new { Id = "SW-03-01", Floor = "3", FloorName = "Kat 3", Ip = "10.10.10.9", Dept = "Yazılım Grubu A" },
                new { Id = "SW-03-02", Floor = "3", FloorName = "Kat 3", Ip = "10.10.10.10", Dept = "Ar-Ge Grubu B" },
                new { Id = "SW-04-01", Floor = "4", FloorName = "Kat 4", Ip = "10.10.10.11", Dept = "Yönetim / Toplantı" },
                new { Id = "SW-05-01", Floor = "5", FloorName = "Kat 5", Ip = "10.10.10.12", Dept = "Bilgi İşlem / Sistem" }
            };

            int swIdx = 1;
            foreach (var sw in switches)
            {
                _devices.Add(new NetworkDevice
                {
                    Id = sw.Id,
                    Name = $"{sw.Id} (Catalyst 2960)",
                    Type = "Access Switch",
                    IpAddress = sw.Ip,
                    MacAddress = $"000C.8530.10{swIdx:X2}",
                    Vlan = "10 (MANAGEMENT)",
                    Floor = sw.FloorName,
                    Department = sw.Dept,
                    Port = "CORE-01:Trunk (802.1Q)",
                    Status = "Online",
                    Description = $"{sw.FloorName} Kat Dağıtım Anahtarı"
                });
                swIdx++;
            }

            // 4. Kat Uç Cihazları (145 PC, Kameralar, AP'ler ve IP Telefonlar)
            foreach (var fl in _floors)
            {
                string fCode = fl.Floor == "0" ? "GF" : (fl.Floor.StartsWith("-") ? "M" + fl.Floor.TrimStart('-') : "0" + fl.Floor);
                string fName = fl.Floor == "0" ? "Zemin Kat" : $"Kat {fl.Floor}";
                string primarySwitch = fl.SwitchModel.Split(' ')[0];

                // PC'ler
                for (int i = 1; i <= fl.PcCount; i++)
                {
                    int ipHost = 10 + (_devices.Count % 240);
                    _devices.Add(new NetworkDevice
                    {
                        Id = $"PC-{fCode}-{i:D2}",
                        Name = $"PC-{fCode}-{i:D2} ({fl.Name})",
                        Type = "PC",
                        IpAddress = $"10.10.20.{ipHost}",
                        MacAddress = $"0060.2F3A.{(1000 + _devices.Count):X4}",
                        Vlan = "20 (USERS)",
                        Floor = fName,
                        Department = fl.Name,
                        Port = $"{primarySwitch}:Fa0/{i + 1}",
                        Status = "Online",
                        Description = $"{fl.Name} Personel Bilgisayarı (DHCP)"
                    });
                }

                // IP Kameralar
                for (int c = 1; c <= fl.CameraCount; c++)
                {
                    _devices.Add(new NetworkDevice
                    {
                        Id = $"CAM-{fCode}-{c:D2}",
                        Name = $"CAM-{fCode}-{c:D2} (CCTV IP Kamera)",
                        Type = "IP Camera",
                        IpAddress = $"10.10.40.{20 + _devices.Count % 200}",
                        MacAddress = $"000C.8540.{(2000 + _devices.Count):X4}",
                        Vlan = "40 (SECURITY)",
                        Floor = fName,
                        Department = "Güvenlik Birimi",
                        Port = $"{primarySwitch}:Fa0/{18 + c}",
                        Status = "Online",
                        Description = $"{fName} Koridor & Giriş CCTV Kamerası"
                    });
                }

                // Kablosuz Erişim Noktaları (AP)
                for (int a = 1; a <= fl.ApCount; a++)
                {
                    _devices.Add(new NetworkDevice
                    {
                        Id = $"AP-{fCode}-{a:D2}",
                        Name = $"AP-{fCode}-{a:D2} (Cisco Aironet AP)",
                        Type = "Access Point",
                        IpAddress = $"10.10.50.{10 + _devices.Count % 200}",
                        MacAddress = $"000A.4178.{(3000 + _devices.Count):X4}",
                        Vlan = "50 (GUEST)",
                        Floor = fName,
                        Department = "Kablosuz Ağ",
                        Port = $"{primarySwitch}:Fa0/{22 + a}",
                        Status = "Online",
                        Description = $"{fName} Misafir ve Mobil Wi-Fi Erişimi"
                    });
                }

                // IP Telefonlar
                for (int p = 1; p <= fl.PhoneCount; p++)
                {
                    _devices.Add(new NetworkDevice
                    {
                        Id = $"PHONE-{fCode}-{p:D2}",
                        Name = $"PHONE-{fCode}-{p:D2} (Cisco 7960 IP Phone)",
                        Type = "IP Phone",
                        IpAddress = $"10.10.60.{10 + _devices.Count % 200}",
                        MacAddress = $"00D0.D314.{(4000 + _devices.Count):X4}",
                        Vlan = "60 (VOICE)",
                        Floor = fName,
                        Department = fl.Name,
                        Port = $"{primarySwitch}:Fa0/{p + 1} (Voice)",
                        Status = "Online",
                        Description = $"{fName} VoIP Dahili Masa Telefonu"
                    });
                }
            }
        }

        private void InitializeValidationTests()
        {
            _validationTests.Add(new ValidationTest
            {
                Id = "T01",
                Category = "DHCP & İstemci",
                Name = "DHCP Otomatik IP Dağıtımı",
                Source = "PC-Kat1 (VLAN 20)",
                Destination = "CORE-01 DHCP Server",
                Function = "İstemcinin CORE-01 üzerinden otomatik IP, maske, varsayılan ağ geçidi ve DNS alması.",
                ExpectedResult = "10.10.20.x IP, 255.255.255.0 Mask, GW: 10.10.20.1, DNS: 10.10.30.10",
                ActualResult = "IP: 10.10.20.101, GW: 10.10.20.1, DNS: 10.10.30.10 (Başarılı)",
                Status = "Başarılı",
                CliCommand = "ipconfig /renew && ipconfig /all"
            });

            _validationTests.Add(new ValidationTest
            {
                Id = "T02",
                Category = "Ağ Geçidi",
                Name = "Default Gateway Ping Doğrulaması",
                Source = "PC-Kat1 (10.10.20.101)",
                Destination = "10.10.20.1 (VLAN 20 SVI)",
                Function = "Uç cihaz ile CORE-01 omurga switch arasındaki Layer 2 ve Layer 3 erişilebilirliği.",
                ExpectedResult = "0% Paket Kaybı, RTT < 1ms",
                ActualResult = "4/4 Paket Alındı, Ortalama RTT: 0.38ms (Başarılı)",
                Status = "Başarılı",
                CliCommand = "ping 10.10.20.1"
            });

            _validationTests.Add(new ValidationTest
            {
                Id = "T03",
                Category = "Layer 2 Switching",
                Name = "Katlar Arası Aynı VLAN Trunk İletişimi",
                Source = "PC-Kat1 (10.10.20.101)",
                Destination = "PC-Kat3 (10.10.20.105)",
                Function = "Farklı katlardaki erişim switchlerinin 802.1Q trunk omurga üzerinden aynı VLAN'da iletişimi.",
                ExpectedResult = "0% Paket Kaybı, Kesintisiz L2 Trunk Geçişi",
                ActualResult = "4/4 Paket Alındı, Ortalama RTT: 0.42ms (Başarılı)",
                Status = "Başarılı",
                CliCommand = "ping 10.10.20.105"
            });

            _validationTests.Add(new ValidationTest
            {
                Id = "T04",
                Category = "Inter-VLAN Routing",
                Name = "Kullanıcıdan Sunucu Çiftliğine Erişim",
                Source = "PC-Kat2 (VLAN 20)",
                Destination = "SRV-01 (10.10.30.10 - VLAN 30)",
                Function = "CORE-01 Layer 3 switch üzerinde VLAN 20 ile VLAN 30 arasındaki yönlendirme doğrulaması.",
                ExpectedResult = "Inter-VLAN yönlendirmesi başarılı, DNS/AD portları açık.",
                ActualResult = "Ping Başarılı, TCP 53 ve 80 port erişimi onaylandı.",
                Status = "Başarılı",
                CliCommand = "ping 10.10.30.10"
            });

            _validationTests.Add(new ValidationTest
            {
                Id = "T05",
                Category = "Güvenlik & ACL",
                Name = "Misafir Ağı İzolasyonu (ACL 101)",
                Source = "Guest-Laptop (10.10.50.25)",
                Destination = "SRV-01 (10.10.30.10 - İç Sunucu)",
                Function = "ACL 101 kuralı gereğince misafir ağının kurum iç sunucularına erişiminin engellenmesi.",
                ExpectedResult = "Request Timed Out / Destination Unreachable (Erişim Engelli)",
                ActualResult = "Paketler ACL 101 tarafından düşürüldü. (0/4 Paket - Başarılı Güvenlik)",
                Status = "Başarılı",
                CliCommand = "ping 10.10.30.10"
            });

            _validationTests.Add(new ValidationTest
            {
                Id = "T06",
                Category = "Güvenlik & ACL",
                Name = "Misafir Varsayılan Ağ Geçidi Erişimi",
                Source = "Guest-Laptop (10.10.50.25)",
                Destination = "10.10.50.1 (VLAN 50 GW)",
                Function = "Misafir cihazların internete çıkabilmek için kendi gateway'lerine erişebilmesi.",
                ExpectedResult = "0% Paket Kaybı (GW Erişilebilir)",
                ActualResult = "4/4 Paket Alındı, Ortalama RTT: 0.35ms (Başarılı)",
                Status = "Başarılı",
                CliCommand = "ping 10.10.50.1"
            });

            _validationTests.Add(new ValidationTest
            {
                Id = "T07",
                Category = "NAT / PAT",
                Name = "Misafir Ağı İnternet Çıkışı (PAT)",
                Source = "Guest-Laptop (10.10.50.25)",
                Destination = "8.8.8.8 (ISP Dış DNS)",
                Function = "Misafir ağının ASA PAT (203.0.113.2) üzerinden dış dünyaya çıkış doğrulaması.",
                ExpectedResult = "0% Paket Kaybı, İnternet Erişimi Aktif",
                ActualResult = "4/4 Paket Alındı, Ortalama RTT: 1.25ms (Başarılı)",
                Status = "Başarılı",
                CliCommand = "ping 8.8.8.8"
            });

            _validationTests.Add(new ValidationTest
            {
                Id = "T08",
                Category = "Güvenlik & ACL",
                Name = "Personel Yönetim Ağı İzolasyonu (ACL 102)",
                Source = "PC-Kat3 (10.10.20.105)",
                Destination = "10.10.10.1 (CORE-01 Yönetim SVI)",
                Function = "Personel PC'lerinin switch yönetim VLAN'ına (VLAN 10) erişiminin engellenmesi.",
                ExpectedResult = "Destination Host Unreachable (Erişim Engelli)",
                ActualResult = "ACL 102 tetiklendi, yönetim erişimi engellendi. (Başarılı)",
                Status = "Başarılı",
                CliCommand = "ping 10.10.10.1"
            });

            _validationTests.Add(new ValidationTest
            {
                Id = "T09",
                Category = "Firewall & MPF",
                Name = "ASA 5506-X ICMP Inspection",
                Source = "PC-Kat1 (10.10.20.101)",
                Destination = "8.8.8.8 (ISP Router Loopback)",
                Function = "ASA Modular Policy Framework (MPF) 'inspect icmp' ile durum bilgili ping geçişi.",
                ExpectedResult = "Durum bilgili (Stateful) ICMP dönüş paketlerinin kabulü",
                ActualResult = "4/4 Paket Alındı, ASA loglarında bağlantı eşleşti. (Başarılı)",
                Status = "Başarılı",
                CliCommand = "ping 8.8.8.8"
            });

            _validationTests.Add(new ValidationTest
            {
                Id = "T10",
                Category = "NAT / PAT",
                Name = "İç Ağ Dynamic PAT Adres Dönüşümü",
                Source = "10.10.0.0/16 İç Ağ",
                Destination = "203.0.113.1 (ISP WAN)",
                Function = "ASA Outside arayüzü (203.0.113.2) üzerinden port çoklamalı PAT doğrulaması.",
                ExpectedResult = "xlate tablosunda dinamik PAT kaydının oluşması",
                ActualResult = "PAT Kaydı Aktif: 10.10.20.101 -> 203.0.113.2:1024 (Başarılı)",
                Status = "Başarılı",
                CliCommand = "show xlate"
            });

            _validationTests.Add(new ValidationTest
            {
                Id = "T11",
                Category = "Port Güvenliği",
                Name = "Switch Port Security (MAC Kısıtlama)",
                Source = "Yetkisiz Cihaz",
                Destination = "SW-01-01 (Fa0/5)",
                Function = "İzinsiz MAC adresli cihaz bağlandığında portun err-disable (shutdown) durumuna geçmesi.",
                ExpectedResult = "Port otomatik olarak kapanmalı ve Syslog güvenlik uyarısı üretmeli.",
                ActualResult = "Port err-disable oldu, güvenlik ihlali loglandı. (Başarılı)",
                Status = "Başarılı",
                CliCommand = "show port-security interface fa0/5"
            });

            _validationTests.Add(new ValidationTest
            {
                Id = "T12",
                Category = "Layer 2 Spanning-Tree",
                Name = "STP BPDU Guard & PortFast",
                Source = "Kenar Portu (Access)",
                Destination = "SW-02-01 (Fa0/1-20)",
                Function = "Uç kullanıcı portlarında STP loop koruması ve ani topoloji değişim engeli.",
                ExpectedResult = "BPDU paketi alındığında port koruma amaçlı kapatılmalı.",
                ActualResult = "PortFast devrede, STP süresi 0 sn geçiş süresi doğrulandı.",
                Status = "Başarılı",
                CliCommand = "show spanning-tree summary"
            });

            _validationTests.Add(new ValidationTest
            {
                Id = "T13",
                Category = "Ses & QoS",
                Name = "VoIP Ses VLAN & CDP Entegrasyonu",
                Source = "PHONE-01 (Cisco 7960)",
                Destination = "SW-01-01 (Fa0/2 - Voice VLAN 60)",
                Function = "IP Telefonun CDP ile Voice VLAN 60'a, arkasındaki PC'nin Data VLAN 20'ye atanması.",
                ExpectedResult = "Aynı fiziksel portta ses ve veri paketlerinin ayrı etiketlenmesi.",
                ActualResult = "Data: VLAN 20, Voice: VLAN 60 olarak doğrulandı. (Başarılı)",
                Status = "Başarılı",
                CliCommand = "show interfaces fa0/2 switchport"
            });

            _validationTests.Add(new ValidationTest
            {
                Id = "T14",
                Category = "Dış Ağ & Statik Rota",
                Name = "ISP Router & ASA Varsayılan Rota",
                Source = "CORE-01 (10.10.10.1)",
                Destination = "ISP-RTR (203.0.113.1)",
                Function = "0.0.0.0/0 varsayılan rota üzerinden tüm dış dünya paketlerinin ASA ve ISP'ye aktarımı.",
                ExpectedResult = "Rotanın aktif olması ve sonraki atlama (Next-Hop) erişilebilirliği.",
                ActualResult = "S* 0.0.0.0/0 [1/0] via 10.10.10.3 (FW-01) rota tablosunda aktif.",
                Status = "Başarılı",
                CliCommand = "show ip route"
            });
        }

        private void InitializeCiscoConfigs()
        {
            // 1. CORE-01 Omurga Switch Konfigürasyonu
            _ciscoConfigs.Add(new CiscoConfigFile
            {
                DeviceKey = "core01",
                DeviceName = "CORE-01 (Cisco Catalyst 3560 Layer 3 Switch)",
                DeviceType = "Layer 3 Switch / Omurga",
                ManagementIp = "10.10.10.1",
                Description = "Tüm binanın Inter-VLAN yönlendirmesi, DHCP havuzları, SVI arayüzleri ve ACL politikaları.",
                ConfigContent = @"! ==============================================================================
! NETVISION - MANAVGAT BELEDIYESI KURUMSAL AG PROJESI
! CIHAZ: CORE-01 (Cisco Catalyst 3560 L3 Omurga Switch)
! ==============================================================================
version 15.0
no service timestamps log datetime msec
no service password-encryption
!
hostname CORE-01
!
ip routing
!
vlan 10
 name MANAGEMENT
!
vlan 20
 name USERS
!
vlan 30
 name SERVERS
!
vlan 40
 name SECURITY
!
vlan 50
 name GUEST
!
vlan 60
 name VOICE
!
! --- DHCP POOL TANIMLARI ---
ip dhcp excluded-address 10.10.20.1 10.10.20.10
ip dhcp excluded-address 10.10.40.1 10.10.40.10
ip dhcp excluded-address 10.10.50.1 10.10.50.10
ip dhcp excluded-address 10.10.60.1 10.10.60.10
!
ip dhcp pool USERS_POOL
   network 10.10.20.0 255.255.255.0
   default-router 10.10.20.1
   dns-server 10.10.30.10 8.8.8.8
   domain-name manavgat.bel.tr
!
ip dhcp pool SECURITY_POOL
   network 10.10.40.0 255.255.255.0
   default-router 10.10.40.1
   dns-server 10.10.30.10
!
ip dhcp pool GUEST_POOL
   network 10.10.50.0 255.255.255.0
   default-router 10.10.50.1
   dns-server 8.8.8.8
!
ip dhcp pool VOICE_POOL
   network 10.10.60.0 255.255.255.0
   default-router 10.10.60.1
   option 150 ip 10.10.30.10
!
! --- SVI LAYER 3 AG GECITLERI ---
interface Vlan10
 description YONETIM_AGI
 ip address 10.10.10.1 255.255.255.0
 no shutdown
!
interface Vlan20
 description PERSONEL_VE_ISTEMCILER
 ip address 10.10.20.1 255.255.255.0
 ip access-group 102 in
 no shutdown
!
interface Vlan30
 description SUNUCU_CIFTLIGI
 ip address 10.10.30.1 255.255.255.0
 no shutdown
!
interface Vlan40
 description GUVENLIK_KAMERALARI
 ip address 10.10.40.1 255.255.255.0
 no shutdown
!
interface Vlan50
 description MISAFIR_KABLOSUZ_AG
 ip address 10.10.50.1 255.255.255.0
 ip access-group 101 in
 no shutdown
!
interface Vlan60
 description IP_TELEFONLAR_VOICE
 ip address 10.10.60.1 255.255.255.0
 no shutdown
!
! --- GUVENLIK ERISIM LISTELERI (ACL) ---
! ACL 101: Misafir aginin ic sunuculara ve yonetime erisimini engelle, internete izin ver
access-list 101 deny   ip 10.10.50.0 0.0.0.255 10.10.10.0 0.0.0.255
access-list 101 deny   ip 10.10.50.0 0.0.0.255 10.10.30.0 0.0.0.255
access-list 101 deny   ip 10.10.50.0 0.0.0.255 10.10.40.0 0.0.0.255
access-list 101 permit ip 10.10.50.0 0.0.0.255 any
!
! ACL 102: Personel PC'lerinin yonetim SVI'larina erisimini kisitla
access-list 102 deny   ip 10.10.20.0 0.0.0.255 10.10.10.0 0.0.0.255
access-list 102 permit ip 10.10.20.0 0.0.0.255 any
!
! --- TRUNK VE UPLINK PORTLARI ---
interface range GigabitEthernet0/1 - 10
 switchport trunk encapsulation dot1q
 switchport mode trunk
 switchport trunk allowed vlan 10,20,30,40,50,60
!
! --- FIREWALL (FW-01) BAGLANTISI VE VARSAYILAN ROTA ---
interface GigabitEthernet0/24
 description LINK_TO_FW-01_INSIDE
 switchport access vlan 10
 switchport mode access
!
ip route 0.0.0.0 0.0.0.0 10.10.10.3
!
banner motd ^C
=====================================================
  MANAVGAT BELEDIYESI - CORE-01 OMURGA SWITCH
  Yetkisiz erisim kesinlikle yasaktir!
=====================================================
^C
end"
            });

            // 2. FW-01 ASA Firewall Konfigürasyonu
            _ciscoConfigs.Add(new CiscoConfigFile
            {
                DeviceKey = "fw01",
                DeviceName = "FW-01 (Cisco ASA 5506-X Güvenlik Duvarı)",
                DeviceType = "Firewall / Dynamic PAT",
                ManagementIp = "10.10.10.3 (Inside) / 203.0.113.2 (Outside)",
                Description = "İç ve dış ağ güvenlik bölgeleri, Dynamic PAT NAT kuralları ve ICMP denetimi.",
                ConfigContent = @"! ==============================================================================
! NETVISION - MANAVGAT BELEDIYESI KURUMSAL AG PROJESI
! CIHAZ: FW-01 (Cisco ASA 5506-X Next-Gen Firewall)
! ==============================================================================
ASA Version 9.8(2)
!
hostname FW-01
!
interface GigabitEthernet1/1
 nameif inside
 security-level 100
 ip address 10.10.10.3 255.255.255.0
 no shutdown
!
interface GigabitEthernet1/2
 nameif outside
 security-level 0
 ip address 203.0.113.2 255.255.255.0
 no shutdown
!
! --- DYNAMIC PAT (PORT ADDRESS TRANSLATION) ---
object network OBJ_INTERNAL_NETWORKS
 subnet 10.10.0.0 255.255.0.0
 nat (inside,outside) dynamic interface
!
! --- MODULAR POLICY FRAMEWORK (MPF) & ICMP INSPECTION ---
class-map inspection_default
 match default-inspection-traffic
!
policy-map global_policy
 class inspection_default
  inspect icmp
  inspect dns
  inspect http
!
service-policy global_policy global
!
! --- STATIK ROTALAR ---
route outside 0.0.0.0 0.0.0.0 203.0.113.1 1
route inside 10.10.20.0 255.255.255.0 10.10.10.1 1
route inside 10.10.30.0 255.255.255.0 10.10.10.1 1
route inside 10.10.40.0 255.255.255.0 10.10.10.1 1
route inside 10.10.50.0 255.255.255.0 10.10.10.1 1
route inside 10.10.60.0 255.255.255.0 10.10.10.1 1
!
banner motd ^C
=====================================================
  MANAVGAT BELEDIYESI - FW-01 GUVENLIK DUVARI
  Guvenli Bolge Girisi - Yetkisiz Erisim Yasaktir.
=====================================================
^C
end"
            });

            // 3. ISP Router Konfigürasyonu
            _ciscoConfigs.Add(new CiscoConfigFile
            {
                DeviceKey = "isp",
                DeviceName = "ISP-RTR (Cisco 2911 WAN Yönlendirici)",
                DeviceType = "Router / Gateway",
                ManagementIp = "203.0.113.1",
                Description = "İnternet servis sağlayıcı simülasyonu, 8.8.8.8 loopback DNS ve dış ağ yönlendirmesi.",
                ConfigContent = @"! ==============================================================================
! NETVISION - MANAVGAT BELEDIYESI KURUMSAL AG PROJESI
! CIHAZ: ISP-RTR (Cisco 2911 WAN Yönlendirici)
! ==============================================================================
version 15.1
hostname ISP-RTR
!
interface GigabitEthernet0/0
 description WAN_BAGLANTISI_FW-01
 ip address 203.0.113.1 255.255.255.0
 no shutdown
!
interface Loopback0
 description PUBLIC_DNS_SIMULASYONU
 ip address 8.8.8.8 255.255.255.255
 no shutdown
!
ip route 10.10.0.0 255.255.0.0 203.0.113.2
!
banner motd ^C
=====================================================
  MANAVGAT BELEDIYESI - ISP WAN GATEWAY (203.0.113.1)
=====================================================
^C
end"
            });

            // 4. Access Switch Şablon Konfigürasyonu
            _ciscoConfigs.Add(new CiscoConfigFile
            {
                DeviceKey = "access",
                DeviceName = "Access Switches (Cisco Catalyst 2960)",
                DeviceType = "Layer 2 Access Switch",
                ManagementIp = "10.10.10.4 - 10.10.10.12",
                Description = "Kat switchleri port güvenliği, VLAN atamaları, Voice VLAN ve Spanning-Tree PortFast.",
                ConfigContent = @"! ==============================================================================
! NETVISION - MANAVGAT BELEDIYESI KURUMSAL AG PROJESI
! CIHAZ: SW-01-01 (Cisco Catalyst 2960 Kat 1 Erisim Switchi Ornegi)
! ==============================================================================
version 15.0
hostname SW-01-01
!
vlan 10
 name MANAGEMENT
vlan 20
 name USERS
vlan 40
 name SECURITY
vlan 50
 name GUEST
vlan 60
 name VOICE
!
interface Vlan10
 ip address 10.10.10.7 255.255.255.0
 no shutdown
!
ip default-gateway 10.10.10.1
!
! --- UPLINK TRUNK PORTU (CORE-01 BAGLANTISI) ---
interface GigabitEthernet0/1
 description TRUNK_TO_CORE-01
 switchport mode trunk
 switchport trunk allowed vlan 10,20,40,50,60
!
! --- PERSONEL PC VE IP TELEFON PORTLARI (DATA + VOICE VLAN) ---
interface range FastEthernet0/1 - 16
 description PERSONEL_PC_VE_VOIP
 switchport mode access
 switchport access vlan 20
 switchport voice vlan 60
 spanning-tree portfast
 switchport port-security
 switchport port-security maximum 2
 switchport port-security violation restrict
!
! --- CCTV KAMERA PORTLARI ---
interface range FastEthernet0/17 - 20
 description GUVENLIK_KAMERALARI
 switchport mode access
 switchport access vlan 40
 spanning-tree portfast
!
! --- KABLOSUZ AP PORTLARI ---
interface range FastEthernet0/21 - 22
 description ACCESS_POINT_MISAFIR
 switchport mode access
 switchport access vlan 50
 spanning-tree portfast
!
! --- KULLANILMAYAN PORTLARIN GUVENLIK ICIN KAPATILMASI ---
interface range FastEthernet0/23 - 24, GigabitEthernet0/2
 shutdown
!
end"
            });
        }

        private void InitializeDefaultAlarms()
        {
            _alarms.Add(new AlarmLog
            {
                SourceDevice = "CORE-01",
                Message = "%SYS-5-CONFIG_I: Configured from console by netadmin on vty0 (10.10.10.1)",
                Severity = "INFO",
                Category = "System",
                Timestamp = DateTime.Now.AddMinutes(-42),
                IsActive = false
            });

            _alarms.Add(new AlarmLog
            {
                SourceDevice = "FW-01",
                Message = "%ASA-6-302013: Built outbound TCP connection for inside:10.10.20.101/51234 to outside:8.8.8.8/53",
                Severity = "INFO",
                Category = "Security",
                Timestamp = DateTime.Now.AddMinutes(-18),
                IsActive = false
            });

            _alarms.Add(new AlarmLog
            {
                SourceDevice = "SW-03-01",
                Message = "%SPANTREE-5-TOPOTX: Topology Change Notification received on Fa0/2",
                Severity = "INFO",
                Category = "Interface",
                Timestamp = DateTime.Now.AddMinutes(-7),
                IsActive = false
            });
        }
    }
}
