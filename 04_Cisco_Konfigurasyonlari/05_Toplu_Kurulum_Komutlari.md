# Cisco Cihazları Toplu Kurulum ve Konfigürasyon Kılavuzu

Bu doküman, NetVision projesinde yer alan tüm aktif ağ cihazlarının (Router, Firewall, Core Switch ve Erişim Switchleri) Packet Tracer ortamında hızlı ve hatasız yapılandırılmasını sağlayan toplu komut setlerini içerir.

---

## 1. Cihaz Rolleri ve IP/VLAN Özet Tablosu

| Cihaz Adı | Model | Konum / Kat | Yönetim IP (VLAN 10) | WAN / Outside IP | Görevi |
|---|---|---|---|---|---|
| **ISP-RTR** | Cisco 2911 | Dış Ağ / ISP | - | `203.0.113.1/30` | Dış İnternet & DNS Simülasyonu |
| **FW-01** | Cisco ASA 5506-X | Sistem Odası | `10.10.10.3/24` (Inside) | `203.0.113.2/30` (Outside) | Güvenlik Duvarı, Dynamic NAT/PAT, Tehdit İzolasyonu |
| **CORE-01** | Cisco 3560/3650 | Sistem Odası | `10.10.10.1/24` (Gateway) | - | L3 Routing, DHCP Sunucu, Omurga, Inter-VLAN ACL |
| **SW-M2-01** | Cisco 2960-24TT | Kat -2 (Teknik/Depo) | `10.10.10.4/24` | - | Kat -2 Erişim Switchi |
| **SW-M1-01** | Cisco 2960-24TT | Kat -1 (Arşiv/Lojistik)| `10.10.10.5/24` | - | Kat -1 Erişim Switchi |
| **SW-GF-01** | Cisco 2960-24TT | Zemin Kat (Danışma) | `10.10.10.6/24` | - | Zemin Kat Erişim Switchi |
| **SW-01-01** | Cisco 2960-24TT | 1. Kat (İK) | `10.10.10.7/24` | - | 1. Kat Erişim Switchi |
| **SW-02-01** | Cisco 2960-24TT | 2. Kat (Muhasebe) | `10.10.10.8/24` | - | 2. Kat Erişim Switchi |
| **SW-03-01** | Cisco 2960-24TT | 3. Kat (Yazılım A) | `10.10.10.9/24` | - | 3. Kat A Grubu Erişim Switchi |
| **SW-03-02** | Cisco 2960-24TT | 3. Kat (Ar-Ge B) | `10.10.10.10/24` | - | 3. Kat B Grubu Erişim Switchi |
| **SW-04-01** | Cisco 2960-24TT | 4. Kat (Yönetim) | `10.10.10.11/24` | - | 4. Kat Erişim Switchi |
| **SW-05-01** | Cisco 2960-24TT | 5. Kat (Sistem Odası)| `10.10.10.12/24` | - | Sunucu & IT Erişim Switchi |

---

## 2. Hızlı Kurulum Sıralaması

1. **Adım 1: ISP-RTR Yapılandırması** -> [03_ISP-RTR_Config.txt](file:///c:/Users/user/OneDrive/Desktop/NetVision/04_Cisco_Konfigurasyonlari/03_ISP-RTR_Config.txt)
2. **Adım 2: FW-01 (ASA Firewall) Yapılandırması** -> [02_FW-01_ASA_Config.txt](file:///c:/Users/user/OneDrive/Desktop/NetVision/04_Cisco_Konfigurasyonlari/02_FW-01_ASA_Config.txt)
3. **Adım 3: CORE-01 (Merkezi Switch) Yapılandırması** -> [01_CORE-01_Config.txt](file:///c:/Users/user/OneDrive/Desktop/NetVision/04_Cisco_Konfigurasyonlari/01_CORE-01_Config.txt)
4. **Adım 4: Erişim Switchlerinin Yapılandırılması** -> [04_Access_Switches_Configs.txt](file:///c:/Users/user/OneDrive/Desktop/NetVision/04_Cisco_Konfigurasyonlari/04_Access_Switches_Configs.txt)

---

## 3. Standart Güvenlik ve Giriş Bilgileri

Tüm ağ cihazları için merkezi olarak belirlenmiş varsayılan kimlik bilgileri:

- **Kullanıcı Adı**: `admin`
- **Kullanıcı Şifresi**: `Cisco@123`
- **Enable Secret / Yetkili Şifresi**: `Cisco@123`
- **Console / VTY Şifresi**: `Cisco@123`
- **Yönetim Alan Adı**: `netvision.local`
- **SSH Versiyonu**: Versiyon 2 (2048 bit RSA)
