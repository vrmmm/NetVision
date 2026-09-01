# Switch Güvenliği ve Katman-2 Sıkılaştırma (Hardening)

Bu dokümanda, kurumsal ağda yer alan 9 adet erişim switchi (SW-M2-01'den SW-05-01'e) ve CORE-01 omurga switch üzerinde uygulanan Katman-2 güvenlik önlemleri, port güvenliği ve Spanning Tree koruma teknikleri açıklanmaktadır.

---

## 1. Uygulanan Katman-2 Güvenlik Mekanizmaları

### 1.1 Port Güvenliği (Port Security)
Her bir erişim portuna bağlanabilecek cihaz sayısı sınırlandırılmış ve MAC adresleri dinamik olarak öğrenilip kalıcı hale getirilmiştir (Sticky MAC):

```cisco
switchport port-security
switchport port-security maximum 2
switchport port-security violation restrict
switchport port-security mac-address sticky
```
- **Maximum 2**: Bir porta hem PC hem de IP Telefon (VoIP) bağlanabileceği senaryolar için en fazla 2 MAC adresine izin verilir.
- **Violation Restrict**: Yetkisiz 3. bir cihaz veya ağ kartı takıldığında port kapatılmaz; ancak yabancı paketi düşürür (drop) ve log kaydı oluşturur.

---

### 1.2 Spanning Tree PortFast ve BPDU Guard
Uç cihazların (PC, Kamera, Yazıcı) bağlı olduğu portların STP dinleme/öğrenme adımlarını atlayarak anında forwarding durumuna geçmesini sağlar; ancak bu porta yetkisiz bir switch veya sahte BPDU paketi takılırsa portu anında `err-disable` moduna alarak döngüleri (loop) engeller:

```cisco
spanning-tree portfast
spanning-tree bpduguard enable
```

---

### 1.3 Kullanılmayan Portların Kapatılması (Unused Port Lockdown)
Açık ve sahipsiz kalan prizlerden ağa yetkisiz sızmaları önlemek için her switchteki boş Ethernet portları idari olarak kapatılmıştır:

```cisco
interface range FastEthernet0/18 - 24, GigabitEthernet0/2
 description UNUSED_PORTS
 shutdown
```

---

### 1.4 Trunk Port Güvenliği ve VLAN Budama (VLAN Pruning)
Omurga ile erişim switchleri arasındaki trunk hatlarında yalnızca tanımlı kurumsal VLAN'ların taşınmasına izin verilmiş, VLAN 1 kullanım dışı bırakılmıştır:

```cisco
switchport mode trunk
switchport trunk allowed vlan 10,20,30,40,50,60
switchport nonegotiate
```
`switchport nonegotiate` komutu ile DTP (Dynamic Trunking Protocol) devre dışı bırakılarak olası VLAN zıplama (VLAN Hopping) saldırıları engellenmiştir.
