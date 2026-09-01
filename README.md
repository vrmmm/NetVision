# 📡 NetVision - Kurumsal Ağ Yönetim Platformu

**NetVision**, Manavgat Belediyesi'nin merkezi ağ altyapısını izlemek, yönetmek ve görselleştirmek için tasarlanmış kapsamlı bir ağ yönetim ve izleme platformudur.

---

## 🎯 Proje Özeti

NetVision, kurumsal ağ ortamlarında:
- **Cihaz Yönetimi** - Router, Switch, Firewall, Server, Access Point gibi ağ cihazlarını merkezi olarak kaydetme ve yönetme
- **Ağ İzleme** - Cihazların çalışma durumunu (Online/Offline/Warning) gerçek zamanlı olarak kontrol etme
- **Olay Yönetimi** - Ağ olaylarını (cihaz erişim kaybı, bağlantı kesintisi, yüksek gecikme) takip ve alarm yönetimi
- **Topoloji Görselleştirmesi** - Ağ cihazları arasındaki bağlantıları görsel olarak sunma

işlevlerini yerine getirmektedir.

---

## 📁 Proje Yapısı

```
NetVision/
├── 01_Proje_Analizi/              # Proje tanımı ve gereksinimler
│   ├── 01_Kurum_Tanimi.md         # Manavgat Belediyesi tanımı ve yapısı
│   ├── 02_Gereksinimler.md        # Genel sistem gereksinimleri
│   ├── 03_NetVision_Gereksinimleri.md  # NetVision yazılım gereksinimleri
│   ├── 04_Proje_Sinirlari.md      # Proje kapsamı ve sınırlamaları
│   └── 05_Ilk_Ag_Mimarisi.png     # İlk ağ mimarisi diyagramı
│
├── 02_Ag_Tasarimi/                # Detaylı ağ tasarım dokümanları
│   ├── 01_Kat_Bazli_Cihaz_Plani.md
│   ├── 02_Port_Hesaplari.md
│   ├── 03_Switch_Plani.md
│   ├── 04_Fiziksel_Topoloji.md
│   ├── 05_Kablo_Plani.md
│   ├── 06_VLAN_Plani.md
│   ├── 07_VLAN_Switch_Matrisi.md
│   ├── 08_IP_Adresleme_Plani.md
│   ├── 08_Mantiksal_Topoloji.png
│   ├── 09_Cihaz_IP_Plani.md
│   ├── 10_DHCP_Plani.md
│   └── 11_Routing_Plani.md
│
├── 03_Packet_Tracer/              # Cisco Packet Tracer ağ simülasyonu
│   ├── NetVision_Network_v1.pkt   # Ağ simülasyon dosyası
│   └── Packet_Tracer_Adim_Adim_Uygulama_Rehberi.md
│
├── 04_Cisco_Konfigurasyonlari/    # Cisco cihaz konfigürasyonları
│   ├── 01_CORE-01_Config.txt      # Core Switch konfigürasyonu
│   ├── 02_FW-01_ASA_Config.txt    # Firewall (ASA) konfigürasyonu
│   ├── 03_ISP-RTR_Config.txt      # ISP Router konfigürasyonu
│   ├── 04_Access_Switches_Configs.txt  # Access Switch konfigürasyonları
│   └── 05_Toplu_Kurulum_Komutlari.md
│
├── 05_Guvenlik_ve_Politikalar/    # Güvenlik ve ağ politikaları
├── 06_Test_Dogrulama_ve_Raporlar/ # Test sonuçları ve doğrulama raporları
│
└── NetVision.NetworkManagement/   # ASP.NET Core MVC Uygulaması
    └── NetVision.NetworkManagement/
        ├── Controllers/           # HTTP isteklerini işleyen controller'lar
        ├── Models/               # Veri modelleri
        ├── Services/             # İş mantığı ve servisler
        ├── Views/                # Razor View şablonları (HTML/Razor)
        ├── wwwroot/              # CSS, JavaScript, görseller
        ├── Program.cs            # Uygulama başlangıç yapılandırması
        ├── appsettings.json      # Yapılandırma dosyası
        └── appsettings.Development.json
```

---

## 🛠️ Teknoloji Stack

| Katman | Teknoloji |
|--------|-----------|
| **Framework** | ASP.NET Core 10.0 MVC |
| **Dil** | C# |
| **Frontend** | Razor Views, HTML/CSS/JavaScript |
| **Veritabanı** | SQL (Design Phase) |
| **Ağ Protokolleri** | ICMP (Ping), SNMP (planned), TCP/IP |
| **Simülasyon** | Cisco Packet Tracer |

---

## 📋 Temel Özellikler

### 1️⃣ Cihaz Yönetimi
Sistem aşağıdaki ağ cihazlarını desteklemektedir:
- **Router** - İnternet bağlantısı ve yönlendirme
- **Switch** - Ağ anahtarlaması (Core ve Access)
- **Firewall** - Ağ güvenliği
- **Server** - Merkezi hizmetler
- **Access Point** - Kablosuz ağ erişimi

Her cihaz için aşağıdaki bilgiler tutulur:
- Cihaz adı, türü, IP adresi, MAC adresi
- Üretici, model, konumu (Kat/Bölüm)
- Gerçek zamanlı çalışma durumu

### 2️⃣ Ağ İzleme
- **Ping-based Monitoring** - ICMP ping ile cihaz erişilebilirliği kontrol
- **Durum Seviyeleri:**
  - 🟢 Online - Cihaz erişilebilir
  - 🔴 Offline - Cihaz erişilemez
  - 🟡 Warning - Yüksek gecikme
  - ⚪ Unknown - Durum belirsiz

### 3️⃣ Olay & Alarm Yönetimi
Sistem otomatik olarak aşağıdaki olayları tespit eder:
- Cihazın erişilemez hale gelmesi
- Cihazın tekrar çevrimiçi olması
- Yüksek ağ gecikmesi (latency)
- Paket kaybı (packet loss)
- Ağ bağlantısı kesintisi

### 4️⃣ Topoloji Görselleştirmesi
- Ağ cihazları arasındaki bağlantıların grafiksel gösterimi
- Cihaz durumlarının renk kodları ile gösterimi
- VLAN yapısının görselleştirilmesi

---

## 🏗️ Ağ Mimarisi

### Kurumsal Yapı
- **Toplam Kullanıcı:** 145 kişi
- **Toplam Cihaz:** 263+
- **Binalar:** 8 kat (Zemin + 5 Normal + 2 Alt Kat)

### Cihaz Envanteri

| Cihaz Türü | Sayı |
|------------|------|
| Bilgisayar (PC) | 145 |
| Yazıcı (Printer) | 12 |
| Erişim Noktası (AP) | 16 |
| IP Kamera | 32 |
| IP Telefon | 40 |
| Server | 6 |
| Switch | 10 |
| Router | 1 |
| Firewall | 1 |

### İnternet Bağlantısı
```
ISP (Fiber Optic)
  ↓
ONT (Optical Network Terminal)
  ↓
Router
  ↓
Firewall (ASA)
  ↓
Core Switch
  ↓
Access Switches & Client Devices
```

---

## 📊 VLAN Yapısı

Ağ, aşağıdaki VLAN'lara bölünmüştür:

| VLAN ID | Ad | Amaç |
|---------|-----|------|
| VLAN 1 | Native | Yönetim |
| VLAN 10 | Management | Yönetim ağı |
| VLAN 20 | Users | Son kullanıcı cihazları |
| VLAN 30 | Printers | Yazıcılar |
| VLAN 40 | Servers | Server ve depolama |
| VLAN 50 | Phones | IP Telefonlar |
| VLAN 60 | Security | Güvenlik kameraları |

---

## 🔒 Güvenlik Özellikleri

- Firewall tabanlı ağ segmentasyonu
- VLAN isolation (Kat ve bölüm bazında)
- ACL (Access Control List) kuralları
- Port security
- Storm control

*(Detaylar: `05_Guvenlik_ve_Politikalar/` dizininde)*

---

## 📝 Dokümentasyon

| Belge | Açıklama |
|-------|----------|
| `01_Proje_Analizi/` | Proje tanımı, gereksinimler, kapsamı |
| `02_Ag_Tasarimi/` | Detaylı ağ tasarımı, IP adresleme, VLAN planı |
| `03_Packet_Tracer/` | Ağ simülasyonu ve uygulama rehberi |
| `04_Cisco_Konfigurasyonlari/` | Tüm Cisco cihaz konfigürasyonları |
| `05_Guvenlik_ve_Politikalar/` | Ağ güvenliği ve kurumsal politikalar |
| `06_Test_Dogrulama_ve_Raporlar/` | Test sonuçları ve validasyon raporları |

---

## 🔄 Proje Aşamaları

### ✅ Tamamlanan
- [x] Proje tanımı ve gereksinimler belirlenmesi
- [x] Detaylı ağ tasarımı (IP adresleme, VLAN, topoloji)
- [x] Cisco Packet Tracer simülasyonu
- [x] Cisco cihaz konfigürasyonları
- [x] ASP.NET Core MVC uygulaması (v1.0)

### 🔄 Devam Eden
- [ ] Veritabanı tasarımı ve implementasyonu
- [ ] Cihaz yönetimi (CRUD) sayfaları
- [ ] Ağ izleme (monitoring) servisi
- [ ] Olay/Alarm yönetimi

### 📅 Planlanmış (v2.0+)
- [ ] SNMP desteği
- [ ] Trafik analizi (Flow monitoring)
- [ ] Otomatik ağ keşfi (Network Discovery)
- [ ] Port durum izleme
- [ ] Gelişmiş raporlama ve istatistikler

---

## 📞 İletişim

- **Geliştirici:** [@vrmmm](https://github.com/vrmmm)
- **LinkedIn:** [Evrim Avcı](https://www.linkedin.com/in/evrim-avcı-08b020340?utm_source=share_via&utm_content=profile&utm_medium=member_android)
- **E-mail:** [evrimmavcii@gmail.com](mailto:evrimmavcii@gmail.com)
- **Repository:** [github.com/vrmmm/NetVision](https://github.com/vrmmm/NetVision)

---

## 🎓 Referanslar

- [Cisco IOS Komutları](https://www.cisco.com/)
- [ASP.NET Core Dokümentasyonu](https://docs.microsoft.com/aspnet/core)
- [VLAN Tasarım En İyi Uygulamalar](https://www.cisco.com/c/en/us/support/docs/switches/)
- [Ağ Mimarisi Standartları](https://www.ietf.org/)

---

**⭐ Bu projeyi faydalı bulduysanız, lütfen yıldız vermeyi unutmayın!**

**📌 Son Güncelleme:** Eylül 2026
