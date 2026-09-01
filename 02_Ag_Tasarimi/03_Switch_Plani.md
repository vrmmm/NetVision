# Switch Planı

## 1. Switch Mimarisi

Manavgat Belediyesi ağında merkezi bir Core
Switch ve katlara dağıtılmış Access Switchler
kullanılacaktır.

Core Switch, kurum ağının merkezi omurga cihazı
olarak görev yapacaktır.

Access Switchler ise katlardaki son kullanıcı
cihazlarının ağa bağlanmasını sağlayacaktır.

---

## 2. Core Switch

Cihaz Adı:

CORE-01

Görevi:

- Kat switchlerinin birbirleriyle haberleşmesini
  sağlamak
- VLAN'lar arasındaki ağ trafiğinin taşınmasını
  sağlamak
- Sunucu ağına bağlantı sağlamak
- Firewall üzerinden gelen trafiği kurum içine
  dağıtmak

---

## 3. Access Switchler

| Kat   |      Switch Adı     | Adet |
|-------|---------------------|-----:|
| -2    | SW-M2-01            |  1   |
| -1    | SW-M1-01            |  1   |
| Zemin | SW-GF-01            |  1   |
| 1     | SW-01-01            |  1   |
| 2     | SW-02-01            |  1   |
| 3     | SW-03-01 / SW-03-02 |  2   |
| 4     | SW-04-01            |  1   |
| 5     | SW-05-01            |  1   |

Toplam Access Switch: 9

---

## 4. Switch Özellikleri

Access Switchler için başlangıçta aşağıdaki
özelliklere sahip yönetilebilir switchler
varsayılmıştır:

- 48 Ethernet port
- Gigabit Ethernet
- VLAN desteği
- Trunk desteği
- STP desteği
- Port Security desteği
- QoS desteği
- PoE+ desteği
- Yönetim arayüzü

Kesin cihaz modeli daha sonraki aşamada
belirlenecektir.

---

## 5. 3. Kat Özel Durumu

3. katta toplam 51 adet Ethernet bağlantısı
ihtiyacı bulunduğundan tek bir 48 port switch
yeterli değildir.

Bu nedenle;

SW-03-01
SW-03-02

olmak üzere iki adet Access Switch kullanılacaktır.

---

## 6. Uplink

Access Switchlerin Core Switch ile bağlantılarının
yüksek hızlı uplink üzerinden gerçekleştirilmesi
planlanmaktadır.

Uplink bağlantılarının fiziksel ortamı ve port
yapısı ağ tasarımının sonraki aşamasında
kesinleştirilecektir.