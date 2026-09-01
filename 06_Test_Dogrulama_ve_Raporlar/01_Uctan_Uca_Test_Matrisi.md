# Uçtan Uca Test ve Doğrulama Matrisi

Bu doküman, kurumsal ağ altyapısının tüm katmanlarında (L2 Switching, L3 Routing, DHCP, NAT/PAT, Güvenlik Duvarı ve ACL İzolasyonları) gerçekleştirilen doğrulama testlerini içerir.

---

## 1. Test Senaryoları ve Sonuç Matrisi

| # | Test Adı | Kaynak Cihaz / VLAN | Hedef Cihaz / IP | Test Edilen Fonksiyon | Beklenen Sonuç | Durum |
|:---:|---|---|---|---|---|:---:|
| **T01** | DHCP Adres Alma | PC-Floor1 (VLAN 20) | CORE-01 DHCP Server | Otomatik IP Ataması (`ipconfig /renew`)| `10.10.20.x` IP, Mask `/24`, GW `10.10.20.1`, DNS `10.10.30.10` alındı |  **BAŞARILI** |
| **T02** | Gateway Erişimi | PC-Floor1 (VLAN 20) | `10.10.20.1` (VLAN 20 SVI) | L2/L3 Gateway Ping Testi | %0 Paket Kaybı, RTT < 1ms |  **BAŞARILI** |
| **T03** | Katlar Arası İletişim | PC-Floor1 (VLAN 20) | PC-Floor3 (`10.10.20.105`) | Aynı VLAN Katlar Arası Trunk İletişimi | %0 Paket Kaybı, Ping Başarılı |  **BAŞARILI** |
| **T04** | Inter-VLAN Routing | PC-Floor2 (VLAN 20) | SRV-01 Web/AD (`10.10.30.10`) | VLAN 20 -> VLAN 30 L3 Yönlendirme | Ping ve HTTP/DNS Port Erişimi Başarılı |  **BAŞARILI** |
| **T05** | Misafir İzolasyon Testi | Guest-Laptop (VLAN 50) | SRV-01 (`10.10.30.10`) | ACL 101 GUEST İzolasyon Denetimi | **Request Timed Out** (Erişim Engellendi) |  **BAŞARILI** |
| **T06** | Misafir GW Ping | Guest-Laptop (VLAN 50) | `10.10.50.1` (GW) | GUEST Default Gateway Ping | Ping Başarılı (GW Erişilebilir) |  **BAŞARILI** |
| **T07** | Misafir İnternet Çıkışı | Guest-Laptop (VLAN 50) | ISP DNS (`8.8.8.8`) | GUEST -> Outside NAT/PAT İnternet Çıkışı | %0 Paket Kaybı, Ping Başarılı |  **BAŞARILI** |
| **T08** | Personel Mgmt Engeli | PC-Floor3 (VLAN 20) | CORE-01 Mgmt (`10.10.10.1`) | ACL 102 Yetkisiz Yönetim Engelleme | **Destination Host Unreachable** |  **BAŞARILI** |
| **T09** | Admin Mgmt Erişimi | Admin-PC (VLAN 10) | CORE-01 Mgmt (`10.10.10.1`) | Yönetim Ağından SSH/Telnet Erişimi | SSH Oturumu Açıldı, Login Başarılı |  **BAŞARILI** |
| **T10** | Dış Ağ İnternet Erişimi | PC-Floor2 (VLAN 20) | ISP Loopback (`8.8.8.8`) | Uçtan Uca ASA PAT & Routing | %0 Paket Kaybı, NAT Tablosunda XLATE Görüldü |  **BAŞARILI** |
| **T11** | Güvenlik Kamerası Erişimi| NVR Server (`10.10.30.10`) | IP Kamera (`10.10.40.101`) | VLAN 30 -> VLAN 40 CCTV İletişimi | Ping ve RTSP Akış İletişimi Başarılı |  **BAŞARILI** |
| **T12** | 802.1Q Trunk Doğrulama | SW-GF-01 (Gig0/1) | CORE-01 (Gig0/4) | Trunking & VLAN Tagging Denetimi | VLAN 10,20,40,50,60 Forwarding Durumunda |  **BAŞARILI** |
| **T13** | Port Güvenliği İhlali | Kat 1 Port Fa0/5 | Yetkisiz 3. MAC Adresi | Port Security Violation Denetimi | Port Kısıtlandı (Restrict), Paketler Düşürüldü |  **BAŞARILI** |
| **T14** | Dışarıdan İzinsiz Giriş | ISP-RTR (`203.0.113.1`) | SRV-01 (`10.10.30.10`) | Dış Dünyadan İç Ağa Sızma Girişimi | ASA Güvenlik Duvarı Tarafından Drop Edildi |  **BAŞARILI** |

---

## 2. Test Sonuç Özeti

- **Toplam Test Sayısı**: 14
- **Başarılı Test Sayısı**: 14 (%100 Başarı)
- **Başarısız Test Sayısı**: 0
- **Güvenlik ve İzolasyon Seviyesi**: Tam Uyumlu
