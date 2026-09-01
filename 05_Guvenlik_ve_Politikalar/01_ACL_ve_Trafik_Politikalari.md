# Ağ Güvenliği: ACL ve Trafik İzolasyon Politikaları

Bu dokümanda, kurumsal ağda VLAN segmentasyonu ile birlikte uygulanan Access Control List (ACL) kuralları, trafik matrisi ve güvenlik gerekçeleri açıklanmaktadır.

---

## 1. VLAN Segmentasyonu ve Güvenlik Seviyeleri

| VLAN ID | VLAN Adı | IP Bloğu | Güvenlik / Yetki Düzeyi | Erişim İzinleri ve Kısıtlamaları |
|---:|---|---|---|---|
| **10** | **MANAGEMENT** | `10.10.10.0/24` | **En Yüksek (Kritik)** | Yalnızca yetkili IT Admin terminallerinden SSH/Konsol erişimine izin verilir. Diğer tüm VLAN'lardan gelen doğrudan erişim engellenir. |
| **20** | **USERS** | `10.10.20.0/24` | **Orta (Kurumsal)** | Kurumsal sunuculara (VLAN 30), IP kameralara (VLAN 40) ve İnternet çıkışına erişebilir. VLAN 10 (Management) arayüzlerine erişemez. |
| **30** | **SERVERS** | `10.10.30.0/24` | **Yüksek (Merkezi)** | Veritabanı, Web, Active Directory, DNS ve Dosya sunucularını barındırır. Yetkili kullanıcı ve cihazlardan gelen belirli port isteklerine yanıt verir. |
| **40** | **SECURITY** | `10.10.40.0/24` | **İzole (CCTV)** | IP Kameralar ve NVR kayıt cihazı. Yalnızca güvenlik departmanı ve yetkili izleme istasyonlarınca erişilebilir. |
| **50** | **GUEST** | `10.10.50.0/24` | **Sıfır Güven (Zero Trust)** | Misafir kullanıcılar için kablosuz ağ. Kurum içi hiçbir alt ağa (`10.10.0.0/16`) erişemez; yalnızca ISP üzerinden internete çıkar. |
| **60** | **VOICE** | `10.10.60.0/24` | **Özel (QoS Öncelikli)** | IP Telefonlar ve Santral (IP PBX). Ses trafiği için QoS öncelikli ve veri ağından ayrılmıştır. |

---

## 2. VLAN'lar Arası Trafik İletişim Matrisi

| Kaynak / Hedef | VLAN 10 (Mgmt) | VLAN 20 (Users) | VLAN 30 (Servers) | VLAN 40 (Security) | VLAN 50 (Guest) | VLAN 60 (Voice) | İnternet (WAN) |
|---|:---:|:---:|:---:|:---:|:---:|:---:|:---:|
| **VLAN 10 (Mgmt)** |  İZİNLİ |  İZİNLİ |  İZİNLİ |  İZİNLİ |  İZİNLİ |  İZİNLİ |  İZİNLİ |
| **VLAN 20 (Users)**|  **ENGELLİ** |  İZİNLİ |  İZİNLİ |  İZİNLİ |  ENGELLİ |  İZİNLİ |  İZİNLİ |
| **VLAN 30 (Servers)**|  ENGELLİ |  İZİNLİ |  İZİNLİ |  ENGELLİ |  ENGELLİ |  ENGELLİ |  İZİNLİ (Güncelleme) |
| **VLAN 40 (Security)**| ENGELLİ |  ENGELLİ |  İZİNLİ (NVR)|  İZİNLİ |  ENGELLİ |  ENGELLİ |  ENGELLİ |
| **VLAN 50 (Guest)**|  **ENGELLİ** |  **ENGELLİ** |  **ENGELLİ** |  **ENGELLİ** |  İZİNLİ |  **ENGELLİ** |  **İZİNLİ** |
| **VLAN 60 (Voice)**|  ENGELLİ |  ENGELLİ |  İZİNLİ (PBX) |  ENGELLİ |  ENGELLİ |  İZİNLİ |  ENGELLİ |

---

## 3. CORE-01 Üzerinde Uygulanan Genişletilmiş ACL Kuralları

### 3.1 Misafir Ağı İzolasyon Kuralı (`ACL_GUEST_ISOLATION`)
Misafirlerin kurum ağı kaynaklarına (sunucular, personel bilgisayarları, kameralar ve yönetim cihazları) sızmasını önlerken, yalnızca internette gezinmelerine ve DNS çözümlemelerine izin verir:

```cisco
ip access-list extended ACL_GUEST_ISOLATION
 remark *** 1. DNS Sorgulari (Port 53) Izinli ***
 permit udp 10.10.50.0 0.0.0.255 any eq domain
 permit tcp 10.10.50.0 0.0.0.255 any eq domain
 remark *** 2. Kendi Default Gateway IP'sine Ping Izinli ***
 permit icmp 10.10.50.0 0.0.0.255 host 10.10.50.1
 remark *** 3. Tum Kurumsal Ic Aglara (10.10.0.0/16) Erisimi Kesin Olarak Engelle ***
 deny ip 10.10.50.0 0.0.0.255 10.10.0.0 0.0.255.255
 remark *** 4. Dis Dunya / Internet Web Trafigine Izin Ver ***
 permit ip 10.10.50.0 0.0.0.255 any
```

### 3.2 Personel Ağı Kısıtlama Kuralı (`ACL_USERS_RESTRICTION`)
Personel bilgisayarlarından kurum omurgasına ve yönetim switch arayüzlerine doğrudan erişimi engelleyerek yönetici ayrıcalıklarını korur:

```cisco
ip access-list extended ACL_USERS_RESTRICTION
 remark *** Yonetim VLAN 10 Agina Erisimi Engelle ***
 deny ip 10.10.20.0 0.0.0.255 10.10.10.0 0.0.0.255
 remark *** Diger Tum Iletisimlere (Sunucu, Diger Katlar, Internet) Izin Ver ***
 permit ip 10.10.20.0 0.0.0.255 any
```
