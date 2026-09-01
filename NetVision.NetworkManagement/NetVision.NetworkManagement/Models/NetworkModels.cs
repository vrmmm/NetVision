using System;
using System.Collections.Generic;

namespace NetVision.NetworkManagement.Models
{
    // ==============================================================================
    // AĞ CİHAZI MODELİ (NetworkDevice)
    // Sitedeki Cihaz Envanteri (CMDB) ve Topoloji bileşenlerinde kullanılan temel veri yapısıdır.
    // Router, Firewall, Core Switch, Access Switch, Sunucu, PC, Kamera, AP ve IP Telefonları temsil eder.
    // ==============================================================================
    public class NetworkDevice
    {
        public string Id { get; set; } = string.Empty;              // Benzersiz cihaz kimliği (örn: CORE-01, SRV-01)
        public string Name { get; set; } = string.Empty;            // Cihazın ekranda görünen adı
        public string Type { get; set; } = string.Empty;            // Cihaz türü: Core Switch, Firewall, Server, PC vb.
        public string IpAddress { get; set; } = string.Empty;       // Cihaza atanmış statik veya DHCP IP adresi
        public string MacAddress { get; set; } = string.Empty;      // Donanımsal MAC adresi
        public string Vlan { get; set; } = string.Empty;            // Bağlı olduğu VLAN kimliği ve adı (örn: 20 - USERS)
        public string Floor { get; set; } = string.Empty;           // Bulunduğu kat (Zemin Kat, Kat 1, Sistem Odası vb.)
        public string Department { get; set; } = string.Empty;      // Bağlı olduğu birim/departman
        public string Port { get; set; } = string.Empty;            // Bağlandığı switch portu (örn: SW-01-01:Fa0/2)
        public string Status { get; set; } = "Online";              // Çevrimiçi/Çevrimdışı durumu (Online / Warning / Offline)
        public double LatencyMs { get; set; } = 0.45;               // Cihaza erişim gecikme süresi (ms)
        public string Description { get; set; } = string.Empty;     // Cihazın işlevi hakkında ek not
    }

    // ==============================================================================
    // VLAN BİLGİ MODELİ (VlanInfo)
    // Sitede VLAN Dağılımı, Port Matrisi ve Dashboard kartlarında segmentasyon bilgisini gösterir.
    // ==============================================================================
    public class VlanInfo
    {
        public int Id { get; set; }                                 // VLAN Numarası (10, 20, 30, 40, 50, 60)
        public string Name { get; set; } = string.Empty;            // VLAN İsmi (MANAGEMENT, USERS, SERVERS vb.)
        public string Subnet { get; set; } = string.Empty;          // Alt Ağ Bloğu (10.10.10.0/24 vb.)
        public string Gateway { get; set; } = string.Empty;         // SVI Varsayılan Ağ Geçidi IP'si
        public int DeviceCount { get; set; }                        // Bu VLAN'daki toplam cihaz adedi
        public string Purpose { get; set; } = string.Empty;         // Kullanım amacı ve güvenlik politikası
        public string StatusColor { get; set; } = "#8E8E8E";        // Görsel arayüzdeki etiket rengi
    }

    // ==============================================================================
    // KAT BAZLI ALTYAPI BİLGİSİ (FloorSpec)
    // Manavgat Belediyesi 8 katlı binasının kat bazlı cihaz ve switch dağılımını tutar.
    // ==============================================================================
    public class FloorSpec
    {
        public string Floor { get; set; } = string.Empty;           // Kat numarası (-2, -1, 0, 1, 2, 3, 4, 5)
        public string Name { get; set; } = string.Empty;            // Kat adı / Fonksiyonu (örn: Muhasebe, Sistem Odası)
        public int PcCount { get; set; }                            // Katta bulunan bilgisayar adedi
        public int PrinterCount { get; set; }                       // Ağ yazıcısı adedi
        public int ApCount { get; set; }                            // Kablosuz erişim noktası (AP) adedi
        public int CameraCount { get; set; }                        // IP Kamera (CCTV) adedi
        public int PhoneCount { get; set; }                         // IP Telefon (VoIP) adedi
        public string SwitchModel { get; set; } = string.Empty;     // Katta konuşlandırılan Switch adı (örn: SW-02-01)
        public string SwitchIp { get; set; } = string.Empty;        // Kat Switch'inin yönetim IP'si
        public int RequiredPorts { get; set; }                      // Katta aktif kullanılan toplam port sayısı
    }

    // ==============================================================================
    // DOĞRULAMA TESTİ MODELİ (ValidationTest)
    // Sitede "Test Matrisi" sayfasında gösterilen 14 adet ağ ve güvenlik doğrulama kaydıdır.
    // ==============================================================================
    public class ValidationTest
    {
        public string Id { get; set; } = string.Empty;              // Test Kodu (T01, T02... T14)
        public string Category { get; set; } = string.Empty;        // Kategori: DHCP, Routing, ACL, NAT, Güvenlik
        public string Name { get; set; } = string.Empty;            // Test Başlığı
        public string Source { get; set; } = string.Empty;          // Kaynak Cihaz / IP / VLAN
        public string Destination { get; set; } = string.Empty;     // Hedef Cihaz / IP / Protokol
        public string Function { get; set; } = string.Empty;        // Test Edilen Ağ İşlevi
        public string ExpectedResult { get; set; } = string.Empty;  // Beklenen Teknik Sonuç
        public string ActualResult { get; set; } = string.Empty;    // Elde Edilen Gerçek Sonuç
        public string Status { get; set; } = "Başarılı";            // Durum: Başarılı / Başarısız / Uyarı
        public string CliCommand { get; set; } = string.Empty;      // Cisco CLI veya Komut Satırı Doğrulama Komutu
    }

    // ==============================================================================
    // ALARM VE OLAY GÜNLÜĞÜ MODELİ (AlarmLog)
    // Sitedeki Alarm Paneli ve Hata Simülasyonu sayfasında gerçek zamanlı logları tutar.
    // ==============================================================================
    public class AlarmLog
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
        public DateTime Timestamp { get; set; } = DateTime.Now;     // Olayın gerçekleşme zamanı
        public string Severity { get; set; } = "INFO";              // Kritiklik düzeyi: CRITICAL, WARNING, INFO, SUCCESS
        public string SourceDevice { get; set; } = string.Empty;    // Alarmı üreten cihaz (örn: FW-01, CORE-01)
        public string Message { get; set; } = string.Empty;         // Syslog / Alarm açıklama mesajı
        public bool IsActive { get; set; } = true;                  // Alarmın şu an aktif olup olmadığı
        public string Category { get; set; } = "Security";          // Kategori: Security, Interface, System, Power
    }

    // ==============================================================================
    // CISCO KONFİGÜRASYON DOSYASI MODELİ (CiscoConfigFile)
    // Sitede Cisco CLI sekmesinde gösterilen cihaz yapılandırma scriptlerini içerir.
    // ==============================================================================
    public class CiscoConfigFile
    {
        public string DeviceKey { get; set; } = string.Empty;       // core01, fw01, isp, access
        public string DeviceName { get; set; } = string.Empty;      // CORE-01 Omurga Switch (Cisco 3560)
        public string DeviceType { get; set; } = string.Empty;      // Layer 3 Switch / ASA Firewall / Router
        public string ManagementIp { get; set; } = string.Empty;    // 10.10.10.1
        public string Description { get; set; } = string.Empty;     // Konfigürasyonun temel amacı
        public string ConfigContent { get; set; } = string.Empty;   // Cisco IOS / ASA CLI tam yapılandırma metni
    }

    // ==============================================================================
    // CANLI PING / ICMP TEŞHİS MODELİ (PingTestResult)
    // Sitedeki interaktif Ping & Ağ Teşhis konsolunda sonuçları döndürmek için kullanılır.
    // ==============================================================================
    public class PingTestResult
    {
        public string TargetIp { get; set; } = string.Empty;
        public string TargetName { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public int PacketsSent { get; set; } = 4;
        public int PacketsReceived { get; set; } = 4;
        public int PacketLossPercentage { get; set; } = 0;
        public double MinRttMs { get; set; }
        public double MaxRttMs { get; set; }
        public double AvgRttMs { get; set; }
        public int Ttl { get; set; } = 128;
        public List<string> TerminalOutputLines { get; set; } = new();
    }

    // ==============================================================================
    // DASHBOARD TOPLU GÖRÜNÜM MODELİ (DashboardViewModel)
    // Dashboard sayfasındaki tüm istatistik, VLAN, kat ve cihaz bilgilerini tek paket halinde taşır.
    // ==============================================================================
    public class DashboardViewModel
    {
        public int TotalDeviceCount { get; set; }                   // Toplam cihaz sayısı (157)
        public int OnlineDeviceCount { get; set; }                  // Aktif çalışan cihaz sayısı (157)
        public int ActiveVlanCount { get; set; }                    // Aktif VLAN sayısı (6)
        public double CpuLoadPercentage { get; set; }               // Omurga CPU yükü (%12)
        public double MemoryUsagePercentage { get; set; }           // Bellek kullanımı (%28)
        public double AverageLatencyMs { get; set; }                // Ortalama ağ gecikmesi (0.42 ms)
        public string FirewallStatus { get; set; } = "FW-01 Aktif (PAT: 10.10.0.0/16 -> 203.0.113.2)";
        public string UptimeString { get; set; } = "99.98% (48 Gün 12 Saat Kesintisiz)";
        public List<VlanInfo> Vlans { get; set; } = new();
        public List<FloorSpec> Floors { get; set; } = new();
        public List<NetworkDevice> CriticalDevices { get; set; } = new();
        public List<AlarmLog> RecentAlarms { get; set; } = new();
    }
}
