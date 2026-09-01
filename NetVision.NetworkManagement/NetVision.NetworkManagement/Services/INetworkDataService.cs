using System.Collections.Generic;
using NetVision.NetworkManagement.Models;

namespace NetVision.NetworkManagement.Services
{
    // ==============================================================================
    // AĞ VERİ SERVİSİ ARAYÜZÜ (INetworkDataService)
    // Sitenin tüm sayfalarında (Dashboard, Topoloji, Envanter, Port Matrisi,
    // Ping Teşhis, Test Matrisi, Cisco CLI ve Alarmlar) ihtiyaç duyulan verileri sağlar.
    // ==============================================================================
    public interface INetworkDataService
    {
        // 1. Dashboard Özeti: Genel ağ metrikleri, CPU, bellek ve aktif durumları getirir
        DashboardViewModel GetDashboardSummary();

        // 2. Envanter (CMDB): 157 adet cihazın listesini; isteğe bağlı arama, kat veya VLAN filtresiyle getirir
        List<NetworkDevice> GetDevices(string? search = null, string? floor = null, string? vlan = null);

        // 3. Tekil Cihaz Bilgisi: Kimliğe (ID) göre cihaz detayını döndürür
        NetworkDevice? GetDeviceById(string id);

        // 4. VLAN Listesi: 6 adet kurumsal VLAN bilgisini (Ad, Blok, GW, Cihaz Sayısı) döndürür
        List<VlanInfo> GetVlans();

        // 5. Kat Dağılımı: 8 katlı akıllı binanın kat bazlı switch ve cihaz envanterini döndürür
        List<FloorSpec> GetFloorSpecs();

        // 6. Test Matrisi: 14 maddelik ağ ve güvenlik doğrulama testlerini kategorili olarak döndürür
        List<ValidationTest> GetValidationTests(string? category = null);

        // 7. Cisco Konfigürasyonları: Cihaz anahtarına (core01, fw01, isp, access) göre CLI scriptlerini getirir
        List<CiscoConfigFile> GetAllCiscoConfigs();
        CiscoConfigFile? GetCiscoConfig(string deviceKey);

        // 8. Canlı Ping & ICMP Teşhis: Belirtilen hedef IP veya cihaza ping gönderip sonuçları üretir
        PingTestResult ExecutePing(string targetIp);

        // 9. Alarm ve Syslog Olayları: Sistemdeki aktif ve geçmiş alarmları döndürür
        List<AlarmLog> GetAlarms();

        // 10. Alarm Simülasyonu: Test amaçlı ağ arızası oluşturur veya mevcut alarmları temizler
        void SimulateNetworkFault(string faultType);
        void ClearAllAlarms();
    }
}
