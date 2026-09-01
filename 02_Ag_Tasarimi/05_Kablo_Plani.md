# Kablo Planı

## 1. ISP - Router

İnternet servis sağlayıcısından kuruma gelen
bağlantının fiber optik altyapı üzerinden
sağlandığı varsayılmıştır.

Bu bağlantı kurumun internet erişiminin başlangıç
noktasıdır.

---

## 2. Router - Firewall

Router ile Firewall arasında ağ bağlantısı
bulunacaktır.

Bu bağlantının kullanılacak fiziksel ortamı,
seçilecek cihazların teknik özellikleri
belirlendikten sonra kesinleştirilecektir.

---

## 3. Firewall - Core Switch

Firewall ile Core Switch arasında yüksek hızlı
ağ bağlantısı bulunacaktır.

Bu bağlantı üzerinden kurum içi ağlara giden ve
gelen trafik taşınacaktır.

---

## 4. Core Switch - Access Switchler

Core Switch ile katlardaki Access Switchler
arasındaki omurga bağlantılarında fiber optik
kablolama kullanılacaktır.

Bu yapı özellikle katlar arasındaki yüksek
bant genişliği ihtiyacını karşılamak amacıyla
tasarlanmıştır.

---

## 5. Access Switch - Son Kullanıcı Cihazları

Kat içerisindeki;

- PC
- Printer
- IP Phone
- IP Camera
- Access Point

cihazlarının Access Switchlere Ethernet
kabloları üzerinden bağlanması planlanmaktadır.

---

## 6. Kablo Yapısının Özeti

| Bağlantı                 | Fiziksel Ortam     |
|--------------------------|--------------------|
| ISP - Router             | Fiber              |
| Router - Firewall        | Henüz belirlenmedi |
| Firewall - Core          | Henüz belirlenmedi |
| Core - Access Switch     | Fiber              |
| Access Switch - PC       | Ethernet           |
| Access Switch - Printer  | Ethernet           |
| Access Switch - Camera   | Ethernet           |
| Access Switch - AP       | Ethernet           |
| Access Switch - IP Phone | Ethernet           |