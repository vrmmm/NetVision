using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NetVision.NetworkManagement.Models;
using NetVision.NetworkManagement.Services;

namespace NetVision.NetworkManagement.Controllers
{
    // ==============================================================================
    // ANA KONTROLCÜ (HomeController)
    // Manavgat Belediyesi 8 Katlı Yönetim Binası Ağ Yönetim Platformunun tüm
    // web sayfalarını, veri akışını ve AJAX API uç noktalarını yöneten merkezi Controller sınıfıdır.
    // ==============================================================================
    public class HomeController : Controller
    {
        private readonly INetworkDataService _networkDataService;

        // Dependency Injection ile ağ veri servisinin controller'a enjekte edilmesi
        public HomeController(INetworkDataService networkDataService)
        {
            _networkDataService = networkDataService;
        }

        // --------------------------------------------------------------------------
        // 1. ANA SAYFA (LANDING PAGE)
        // Sayfada Ne İşe Yarar: Kullanıcıyı karşılayan ana kapak ekranıdır.
        // Sistemin genel hedeflerini, Manavgat Belediyesi kurumsal kimliğini ve hızlı modül butonlarını sunar.
        // --------------------------------------------------------------------------
        public IActionResult Index()
        {
            var summary = _networkDataService.GetDashboardSummary();
            return View(summary);
        }

        // --------------------------------------------------------------------------
        // 2. GENEL BAKIŞ PANELİ (DASHBOARD)
        // Sayfada Ne İşe Yarar: 157 cihazın online durumunu, VLAN kapasitelerini,
        // ortalama gecikme süresini (RTT), CPU yükünü, kat bazlı switch durumlarını ve son alarmları gösterir.
        // --------------------------------------------------------------------------
        public IActionResult Dashboard()
        {
            var summary = _networkDataService.GetDashboardSummary();
            return View(summary);
        }

        // --------------------------------------------------------------------------
        // 3. İNTERAKTİF TOPOLOJİ HARİTASI (TOPOLOGY)
        // Sayfada Ne İşe Yarar: ISP -> FW-01 (ASA) -> CORE-01 Omurga Switch -> Sunucu Çiftliği ve
        // 8 katın Catalyst 2960 switchlerini hiyerarşik ve interaktif bir şemada görselleştirir.
        // Düğümlere tıklandığında cihazın IP, MAC, port ve durum detaylarını açar.
        // --------------------------------------------------------------------------
        public IActionResult Topology()
        {
            var devices = _networkDataService.GetDevices();
            ViewBag.Vlans = _networkDataService.GetVlans();
            ViewBag.Floors = _networkDataService.GetFloorSpecs();
            return View(devices);
        }

        // --------------------------------------------------------------------------
        // 4. CİHAZ ENVANTERİ (CMDB INVENTORY)
        // Sayfada Ne İşe Yarar: Kurum bünyesindeki 157 adet cihazın (Router, Firewall, Core Switch,
        // Sunucular, Access Switchler, PC'ler, Kameralar, AP'ler ve IP Telefonlar) aranabilir,
        // kat ve VLAN bazında filtrelenebilir listesini sunar.
        // --------------------------------------------------------------------------
        public IActionResult Inventory(string? search, string? floor, string? vlan)
        {
            var devices = _networkDataService.GetDevices(search, floor, vlan);
            ViewBag.Search = search;
            ViewBag.SelectedFloor = floor ?? "All";
            ViewBag.SelectedVlan = vlan ?? "All";
            ViewBag.Floors = _networkDataService.GetFloorSpecs();
            ViewBag.Vlans = _networkDataService.GetVlans();
            return View(devices);
        }

        // --------------------------------------------------------------------------
        // 5. VLAN & PORT MATRİSİ (PORT MATRIX)
        // Sayfada Ne İşe Yarar: 8 katlı binanın her katındaki switchlerin port kullanım oranlarını,
        // Access/Trunk port dağılımını, PoE durumunu ve VLAN eşleşmelerini gösterir.
        // --------------------------------------------------------------------------
        public IActionResult PortMatrix()
        {
            var floors = _networkDataService.GetFloorSpecs();
            var vlans = _networkDataService.GetVlans();
            ViewBag.Vlans = vlans;
            return View(floors);
        }

        // --------------------------------------------------------------------------
        // 6. CANLI PING & ICMP TEŞHİS KONSOLU (PING TOOL)
        // Sayfada Ne İşe Yarar: Ağ yöneticisinin sistemdeki herhangi bir cihaza veya dış IP'ye
        // canlı ICMP ping isteği gönderip RTT gecikmesini ve paket kaybını test etmesini sağlar.
        // --------------------------------------------------------------------------
        public IActionResult PingTool()
        {
            var devices = _networkDataService.GetDevices();
            return View(devices);
        }

        // --------------------------------------------------------------------------
        // 6.1 PING AJAX API UÇ NOKTASI (ExecutePing)
        // Sayfada Ne İşe Yarar: Ping butonuna basıldığında sayfayı yenilemeden arka planda
        // ICMP isteğini koşturur ve JSON olarak komut satırı terminal çıktısını döner.
        // --------------------------------------------------------------------------
        [HttpPost]
        public IActionResult ExecutePing([FromBody] PingRequestModel request)
        {
            if (string.IsNullOrWhiteSpace(request?.TargetIp))
            {
                return Json(new { success = false, message = "Lütfen geçerli bir IP adresi veya cihaz seçiniz." });
            }

            var result = _networkDataService.ExecutePing(request.TargetIp);
            return Json(new { success = true, data = result });
        }

        // --------------------------------------------------------------------------
        // 7. DOĞRULAMA TEST MATRİSİ (TEST MATRIX)
        // Sayfada Ne İşe Yarar: DHCP, Inter-VLAN routing, Misafir ACL 101 engeli, ASA PAT ve
        // Port Security gibi 14 adet kritik doğrulama testinin CLI kanıtlarını ve durumlarını listeler.
        // --------------------------------------------------------------------------
        public IActionResult TestMatrix(string? category)
        {
            var tests = _networkDataService.GetValidationTests(category);
            ViewBag.SelectedCategory = category ?? "All";
            return View(tests);
        }

        // --------------------------------------------------------------------------
        // 8. CISCO CLI KONFİGÜRASYONLARI (CISCO CONFIG)
        // Sayfada Ne İşe Yarar: Cisco Catalyst 3560 Omurga, Cisco ASA 5506-X Güvenlik Duvarı,
        // ISP Router ve Kat Switchlerinin çalışan konfigürasyon (running-config) dosyalarını görüntüler.
        // --------------------------------------------------------------------------
        public IActionResult CiscoConfig(string? device = "core01")
        {
            var configs = _networkDataService.GetAllCiscoConfigs();
            var activeConfig = _networkDataService.GetCiscoConfig(device ?? "core01") ?? configs.FirstOrDefault();
            ViewBag.ActiveKey = device ?? "core01";
            ViewBag.Configs = configs;
            return View(activeConfig);
        }

        // --------------------------------------------------------------------------
        // 9. ALARM VE SYSLOG GÜNLÜĞÜ (ALARMS)
        // Sayfada Ne İşe Yarar: Ağdaki olayları, güvenlik ihlallerini, port durumlarını ve
        // canlı simüle edilen arıza bildirimlerini kronolojik olarak sunar.
        // --------------------------------------------------------------------------
        public IActionResult Alarms()
        {
            var alarms = _networkDataService.GetAlarms();
            return View(alarms);
        }

        // --------------------------------------------------------------------------
        // 9.1 ARIZA SİMÜLASYONU AJAX UÇ NOKTASI (SimulateFault)
        // Sayfada Ne İşe Yarar: Arayüzdeki 'Hata Simüle Et' butonuna basıldığında
        // sisteme sahte arıza (Link Down, Yüksek CPU vb.) alarmı ekler.
        // --------------------------------------------------------------------------
        [HttpPost]
        public IActionResult SimulateFault([FromBody] FaultRequestModel request)
        {
            _networkDataService.SimulateNetworkFault(request?.FaultType ?? "link_down");
            return Json(new { success = true, alarms = _networkDataService.GetAlarms() });
        }

        // --------------------------------------------------------------------------
        // 9.2 ALARMLARI TEMİZLEME AJAX UÇ NOKTASI (ClearAlarms)
        // Sayfada Ne İşe Yarar: Ağ yöneticisinin tüm aktif arıza bildirimlerini sıfırlamasını sağlar.
        // --------------------------------------------------------------------------
        [HttpPost]
        public IActionResult ClearAlarms()
        {
            _networkDataService.ClearAllAlarms();
            return Json(new { success = true, alarms = _networkDataService.GetAlarms() });
        }

        // --------------------------------------------------------------------------
        // 10. CİHAZ DETAY AJAX API (GetDeviceDetail)
        // Sayfada Ne İşe Yarar: Topolojide tıklanan düğümün detaylarını JSON olarak döner.
        // --------------------------------------------------------------------------
        [HttpGet]
        public IActionResult GetDeviceDetail(string id)
        {
            var device = _networkDataService.GetDeviceById(id);
            if (device == null) return NotFound();
            return Json(device);
        }

        // --------------------------------------------------------------------------
        // 11. HATA VE GİZLİLİK SAYFALARI
        // --------------------------------------------------------------------------
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    // AJAX İstek Modelleri
    public class PingRequestModel
    {
        public string TargetIp { get; set; } = string.Empty;
    }

    public class FaultRequestModel
    {
        public string FaultType { get; set; } = string.Empty;
    }
}
