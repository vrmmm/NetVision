# NetVision: Bilgisayar Ağı Tasarımı, Konfigürasyonu, Güvenliği ve İzleme Projesi
## Kapsamlı Final Proje Raporu ve Staj Defteri

---

## 1. Proje Özeti ve Amacı
Bu proje, 8 katlı kurumsal bir yönetim binasının (**Manavgat Belediyesi**) tüm yerel alan ağı (LAN), omurga yönlendirmesi (Core Switching), güvenlik duvarı (Firewall), internet servis sağlayıcı (ISP) bağlantıları ve yazılım tabanlı ağ izleme altyapısının (**NetVision Platformu**) sıfırdan tasarlanması, simüle edilmesi, konfigüre edilmesi ve doğrulanması amacıyla gerçekleştirilmiştir.

Projenin temel hedefleri:
- 145 kullanıcı PC'si, 12 Ağ Yazıcısı, 16 Kablosuz Erişim Noktası (AP), 32 IP Güvenlik Kamerası, 40 IP Telefon ve 6 Kurumsal Sunucudan oluşan geniş ölçekli altyapıyı tek bir broadcast alanında boğulmaktan kurtarıp **VLAN tabanlı hiyerarşik kurumsal ağ mimarisine** dönüştürmek.
- **Cisco Catalyst 3560/3650** omurga switch üzerinde Inter-VLAN Routing ve dinamik DHCP havuzları oluşturmak.
- **Cisco ASA 5506-X** güvenlik duvarı üzerinde Stateful Packet Inspection, Dynamic NAT/PAT ve erişim kontrol listeleri (ACL) ile dış dünyadan gelebilecek tehditlere karşı sıfır-güven (zero-trust) mimarisi kurmak.
- Misafir (Guest) ağını izole ederek kurum içi kaynaklara sızılmasını engellemek.
- Kurumsal ağ cihazlarının sağlık durumlarını, bağlantılarını ve alarmlarını anlık izleyen modern **NetVision Web Platformu**'nu geliştirmek.

---

## 2. Ağ Mimarisi ve Tasarım Gerekçeleri

### 2.1 Hiyerarşik Ağ Modeli (Core - Access Yapısı)
Ağ tasarımında Cisco'nun hiyerarşik model prensipleri benimsenmiştir:
1. **Core Layer (Merkezi Omurga)**: `CORE-01` Multilayer switch, yüksek hızlı paket yönlendirmesi, inter-VLAN trafiğinin taşınması ve DHCP dağıtımını merkezi olarak üstlenir.
2. **Access Layer (Erişim Katmanı)**: Her kata yerleştirilen 9 adet yönetilebilir Cisco Catalyst switch (`SW-M2-01`'den `SW-05-01`'e kadar), uç cihazların ağa güvenli bağlanmasını sağlar.
3. **Security Perimeter (Güvenlik Sınırı)**: `FW-01` (Cisco ASA) kurumsal iç ağ (Inside) ile dış internet (Outside) arasında güvenlik bariyeri oluşturur.
4. **WAN / ISP Layer**: `ISP-RTR` kuruma tahsis edilen fiber optik internet bağlantısını ve DNS/Web servislerini temsil eder.

---

## 3. Adresleme ve Segmentasyon Tabloları

### 3.1 VLAN ve IP Planı
| VLAN ID | VLAN Adı | Subnet | Gateway | DHCP Dağıtım Aralığı | Tahsis Edilen Birimler |
|---:|---|---|---|---|---|
| **10** | **MANAGEMENT** | `10.10.10.0/24` | `10.10.10.1` | Statik IP (`.2` - `.12`) | Ağ Yöneticileri, Switch ve Firewall Yönetim Arayüzleri |
| **20** | **USERS** | `10.10.20.0/24` | `10.10.20.1` | `10.10.20.100` - `10.10.20.200` | Personel Masaüstü/Laptop Bilgisayarları, Yazıcılar |
| **30** | **SERVERS** | `10.10.30.0/24` | `10.10.30.1` | Statik IP (`.10` - `.15`) | AD/DNS, Web, DB, Mail, File ve NetVision Sunucuları |
| **40** | **SECURITY** | `10.10.40.0/24` | `10.10.40.1` | `10.10.40.100` - `10.10.40.200` | 8 Katta Dağıtık 32 Adet IP CCTV Kamera ve NVR |
| **50** | **GUEST** | `10.10.50.0/24` | `10.10.50.1` | `10.10.50.100` - `10.10.50.200` | Misafir Kablosuz (Wi-Fi) Kullanıcıları |
| **60** | **VOICE** | `10.10.60.0/24` | `10.10.60.1` | `10.10.60.100` - `10.10.60.200` | Kurum İçi 40 Adet IP Telefon (VoIP) ve IP PBX |

---

## 4. Karşılaşılan Problemler ve Uygulanan Çözümler

### Problem 1: CORE-01 CLI "User Access Verification" Kilitlenmesi
- **Gözlem**: Switch CLI arayüzü açıldığında kimlik doğrulama ekranında kalması.
- **Analiz**: `line con 0` veya `line vty` üzerinde `login local` tanımlanmış ancak kullanıcı adı/şifresi eşleşmemiştir.
- **Çözüm**: ROMMON / Password Recovery prosedürü uygulanarak mevcut çalışan konfigürasyon korunmuş; `username admin privilege 15 secret Cisco@123` ve `enable secret Cisco@123` merkezi kimlik bilgileri tanımlanarak kilitlenme giderilmiştir.

### Problem 2: ASA 5506-X Üzerinden Ping Paketlerinin Geçmemesi (ICMP Bloklanması)
- **Gözlem**: İç ağdaki istemcilerin ISP Router ve simüle internet IP'lerine ping atamaması.
- **Analiz**: Cisco ASA güvenlik duvarları varsayılan olarak durum bilgili ICMP denetimi (inspection) yapmaz; dolayısıyla dışarıdan dönen `Echo-Reply` paketlerini drop eder.
- **Çözüm**: Modüler Politika Çerçevesi (`policy-map global_policy`) altına `inspect icmp` eklenerek durum bilgili ICMP geçişi sağlanmıştır.

### Problem 3: ISP Router Tarafında Kurumsal Ağlara Dönüş Yolu Eksikliği
- **Gözlem**: PAT/NAT yapılmasına rağmen ISP Router'ın iç ağ bloklarını doğrudan tanıyamaması.
- **Çözüm**: ISP-RTR üzerinde `ip route 10.10.0.0 255.255.0.0 203.0.113.2` statik rotası eklenmiştir.

---

## 5. Günlük Staj Defteri Kayıtları (30 İş Günü)

| Gün | Faaliyet Konusu | Gerçekleştirilen Teknik Çalışmalar |
|:---:|---|---|
| **Gün 1** | Proje Analizi ve Kapsam Belirleme | Manavgat Belediyesi 8 katlı bina ihtiyaçları incelendi, kullanıcı sayıları (145 PC, 12 Yazıcı, 16 AP, 32 Kamera, 40 IP Phone, 6 Sunucu) belirlendi. |
| **Gün 2** | Kat Bazlı Port Hesaplamaları | Her kat için gerekli Ethernet port sayıları hesaplandı. 3. katta 51 port gerektiğinden çift switch (`SW-03-01` ve `SW-03-02`) kullanılmasına karar verildi. |
| **Gün 3** | Switch ve Omurga Mimarisi | Core switch ve kat erişim switchlerinin bağlantı yapısı belirlendi. Fiber omurga hatları planlandı. |
| **Gün 4** | Fiziksel ve Mantıksal Topoloji Tasarımı | Topoloji şeması çizildi; ISP -> FW -> Core Switch -> Access Switches hiyerarşisi oluşturuldu. |
| **Gün 5** | VLAN ve Subnetting Planlaması | Özel IP bloğu `10.10.0.0/16` seçilerek VLAN 10, 20, 30, 40, 50, 60 için `/24` alt ağları ve gateway standartları belirlendi. |
| **Gün 6** | Packet Tracer Laboratuvarının Kurulumu | Cihazlar (Cisco 2911, ASA 5506-X, Catalyst 3560, Catalyst 2960) çalışma alanına yerleştirildi ve kablolandı. |
| **Gün 7** | Erişim Switchleri Temel Yapılandırması | 9 adet erişim switchi üzerinde VLAN tanımları yapıldı, yönetim SVI IP'leri atandı ve default gateway girildi. |
| **Gün 8** | 802.1Q Trunk Portlarının Kurulumu | Omurga ile kat switchleri arasındaki uplink portları trunk moduna alındı, izinli VLAN listeleri (`allowed vlan`) kısıtlandı. |
| **Gün 9** | CORE-01 Inter-VLAN Routing | Core switch üzerinde `ip routing` aktif edildi, tüm VLAN'lar için SVI arayüzleri oluşturuldu. |
| **Gün 10** | Dinamik DHCP Havuzlarının Yapılandırılması | CORE-01 üzerinde `USERS`, `SECURITY`, `GUEST` ve `VOICE` DHCP havuzları kuruldu; gateway ve DNS opsiyonları girildi. |
| **Gün 11** | Uç Cihazların Ağa Bağlanması ve Test | PC ve laptopların DHCP üzerinden IP aldığı doğrulandı. Katlar arası ping testleri yapıldı. |
| **Gün 12** | Güvenlik Duvarı (Cisco ASA) Kurulumu | FW-01 üzerinde `inside` ve `outside` güvenlik bölgeleri tanımlandı, IP adresleri atandı. |
| **Gün 13** | Dynamic PAT (NAT) Yapılandırması | Kurum içi `10.10.0.0/16` ağının dış arayüz IP'si üzerinden internete çıkması için PAT kuralı yazıldı. |
| **Gün 14** | ISP-RTR ve Dış Ağ Entegrasyonu | ISP Router WAN arayüzü ve Loopback DNS (`8.8.8.8`) yapılandırıldı; ASA ve ISP arasında statik rotalar kuruldu. |
| **Gün 15** | ASA ICMP Denetimi ve Tehdit Engelleme | MPF `inspect icmp` aktif edilerek iç ağdan dış ağa ping geçişi sağlandı, dışarıdan gelen yetkisiz istekler engellendi. |
| **Gün 16** | Inter-VLAN ACL Güvenlik Kuralları | CORE-01 üzerinde `ACL_GUEST_ISOLATION` ve `ACL_USERS_RESTRICTION` kuralları yazılarak misafir izolasyonu sağlandı. |
| **Gün 17** | Switch Hardening ve Port Güvenliği | Kat switchlerinde `port-security`, `spanning-tree portfast`, `bpduguard` ve boş portların kapatılması uygulandı. |
| **Gün 18** | Uçtan Uca Sistem ve Güvenlik Doğrulama | 14 maddelik kapsamlı test matrisi icra edildi, CLI kanıt çıktıları kaydedildi. |
| **Gün 19** | NetVision ASP.NET Core Projesi Temeli | Proje iskelet yapısı oluşturuldu; C# MVC mimarisi, Controller ve View katmanları tasarlandı. |
| **Gün 20** | Veri Modelleri ve Servis Mimarisi | `NetworkDevice`, `VlanInfo`, `FloorSpec`, `ValidationTest`, `AlarmLog` ve `CiscoConfigFile` C# model sınıfları geliştirildi. |
| **Gün 21** | NetworkDataService Uygulaması | 157 cihazlık kurumsal envanter, VLAN tanımları ve Cisco CLI konfig verileri C# Singleton servis üzerinde üretildi. |
| **Gün 22** | Dashboard ve Ana Sayfa Geliştirmesi | Kurumsal karşılama (Index) sayfası ve genel ağ durum paneli (Dashboard) tüm metriklerle oluşturuldu. |
| **Gün 23** | İnteraktif Topoloji Haritası | ISP -> FW-01 ASA -> CORE-01 Omurga -> Sunucu Çiftliği -> Kat Switchleri hiyerarşik topoloji sayfası hazırlandı. |
| **Gün 24** | Cihaz Envanteri (CMDB) Modülü | Arama, kat ve VLAN filtreleme desteğiyle 157 cihazın listelendiği CMDB envanteri geliştirildi. |
| **Gün 25** | VLAN & Port Matrisi Sayfası | 8 katlı binanın switch port tahsisi, PoE dağılımı ve kapasite görselleştirme sayfası tamamlandı. |
| **Gün 26** | Canlı Ping & ICMP Teşhis Konsolu | AJAX tabanlı canlı ICMP ping teşhis aracı, terminal çıktısı ve gerçek zamanlı RTT hesaplama desteğiyle geliştirildi. |
| **Gün 27** | Doğrulama Test Matrisi Modülü | 14 maddelik ağ ve güvenlik doğrulama testleri kategori filtresi ve kanıt görünümüyle sayfaya eklendi. |
| **Gün 28** | Cisco CLI Konfigürasyon Yöneticisi | CORE-01, FW-01 ASA, ISP Router ve Access Switch CLI konfigürasyonları terminal görünümlü sekmeli yapıda sunuldu. |
| **Gün 29** | Alarm ve Syslog Olay Modülü | Gerçek zamanlı alarm takibi, arıza simülasyon senaryoları ve olay günlüğü sayfası tamamlandı. |
| **Gün 30** | Final Proje Raporu ve Teslim Hazırlığı | Proje dokümantasyonu tamamlandı, konfigürasyon yedekleri arşivlendi ve NetVision platformu son haline getirildi. |

---

## 6. NetVision Ağ Yönetim Platformu Yazılım Mimarisi
Projenin yazılım bileşeni olan NetVision platformu:
1. **İnteraktif Topoloji**: Ağdaki tüm cihazların hiyerarşik fiziksel/mantıksal haritasını ve hat durumlarını canlı görselleştirir.
2. **CMDB Cihaz Envanteri**: 8 kattaki tüm cihazların IP, MAC, VLAN ve durum kayıtlarını tutar.
3. **Simüle Ping ve Teşhis Motoru**: Cihazlar arası gecikme, paket kaybı ve ACL durumunu gerçek zamanlı hesaplayarak test eder.
4. **Cisco CLI Konfigürasyon Merkezi**: Tüm switch ve güvenlik duvarlarının hazır `.cfg` dosyalarını görüntüleme, kopyalama ve indirme imkanı sunar.
5. **Olay ve Alarm Yönetimi**: Ağ kesintileri ve güvenlik ihlallerini anlık loglar.

---

## 7. Sonuç ve Teknik Değerlendirme
Proje başarıyla tamamlanmış olup, Manavgat Belediyesi için tasarlanan ağ altyapısı:
- Yüksek performanslı ve düşük gecikmeli omurga iletişimi,
- Katı güvenlik ve misafir izolasyon politikaları,
- Katman-2 ve Katman-3 seviyesinde tam sıkılaştırma,
- Paket Tracer simülasyonu ve NetVision yönetim yazılımı ile doğrulanmış %100 çalışır vaziyete getirilmiştir.
