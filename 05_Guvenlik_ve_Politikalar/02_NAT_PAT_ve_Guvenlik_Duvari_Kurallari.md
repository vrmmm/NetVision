# Güvenlik Duvarı: NAT/PAT ve Cisco ASA 5506-X Politikaları

Bu dokümanda, **FW-01 (Cisco ASA 5506-X)** güvenlik duvarı cihazı üzerinde yapılandırılan Port Address Translation (PAT / Dynamic NAT), güvenlik bölgeleri (Security Zones), durum bilgili paket denetimi (Stateful Inspection) ve yönlendirme kuralları incelenmektedir.

---

## 1. Güvenlik Duvarı Bölgeleri ve Arayüz Yapısı

| Arayüz | Bölge Adı (`nameif`) | Güvenlik Seviyesi (`security-level`) | IP Adresi | Açıklama |
|---|---|:---:|---|---|
| **GigabitEthernet1/1** | `outside` | **0** | `203.0.113.2/30` | Dış dünya / ISP WAN çıkışı. Güvensiz bölge. |
| **GigabitEthernet1/2** | `inside` | **100** | `10.10.10.3/24` | Kurumsal iç ağ (CORE-01 omurga). Güvenli bölge. |

> **Cisco ASA Güvenlik Kuralı**: Yüksek güvenlik seviyesinden (100) düşük güvenlik seviyesine (0) giden trafik varsayılan olarak serbesttir ve ASA tarafından durum tablosunda (State Table / XLATE) tutulur. Düşük güvenlik seviyesinden (0) yüksek seviyeye (100) gelen trafik ise açıkça ACL ile izin verilmedikçe tamamen engellenir.

---

## 2. Dynamic NAT (PAT) Yapılandırması

Kurum içerisindeki tüm alt ağların (`10.10.0.0/16`) tek bir genel (public) IP adresi üzerinden internete çıkabilmesi için dinamik port adres çevirisi (PAT) tanımlanmıştır:

```cisco
object network OBJ_CORP_LAN
 subnet 10.10.0.0 255.255.0.0
 nat (inside,outside) dynamic interface
```

### PAT Çalışma Mantığı
1. Kat 2'deki bir kullanıcı (`10.10.20.105:54321`) internetteki Google DNS (`8.8.8.8:53`) sunucusuna istek gönderir.
2. Paket CORE-01 üzerinden `10.10.10.3` (FW-01 Inside) arayüzüne ulaşır.
3. FW-01 kaynak IP'yi dış arayüz IP'sine (`203.0.113.2:10542`) çevirir ve bunu NAT tablosuna yazar.
4. ISP Router'dan yanıt geldiğinde FW-01 NAT tablosundan bu portu eşleştirip paketi tekrar `10.10.20.105` adresine iletir.

---

## 3. ICMP ve Stateful Paket Denetimi (MPF)

Cisco ASA varsayılan olarak ICMP (Ping) trafiğini denetlemez (inspect etmez). Bu durum iç ağdaki istemcilerin dış ağa attığı ping isteklerinin yanıtlarının ASA tarafından engellenmesine yol açar. Bunu çözmek için Modüler Politika Çerçevesi (MPF) yapılandırılmıştır:

```cisco
policy-map global_policy
 class inspection_default
  inspect icmp
  inspect dns
  inspect http
!
service-policy global_policy global
```

Bu yapılandırma ile iç ağdan dışarıya atılan ping paketlerinin `Echo-Reply` yanıtları ASA tarafından tanınır ve içeriye sorunsuz biçimde aktarılır.
