# NetVision Gereksinimleri

## 1. Projenin Amacı

NetVision, kurumsal bir ağ altyapısının merkezi olarak
izlenmesini ve yönetilmesini amaçlayan yazılım tabanlı
bir ağ yönetim ve izleme platformudur.

Sistem, proje kapsamında tasarlanan kurumsal ağ
altyapısındaki cihazların ve bağlantıların kayıt altına
alınmasını, cihaz durumlarının izlenmesini ve meydana
gelen ağ olaylarının takip edilmesini sağlayacaktır.

---

## 2. Cihaz Yönetimi

Sistem aşağıdaki ağ cihazlarının sisteme eklenmesini
ve yönetilmesini destekleyecektir:

- Router
- Switch
- Firewall
- Server
- Access Point

Her cihaz için temel bilgiler tutulacaktır.

### Cihaz Bilgileri

- Cihaz adı
- Cihaz türü
- IP adresi
- MAC adresi
- Üretici
- Model
- Bulunduğu kat
- Bulunduğu bölüm
- Çalışma durumu

---

## 3. Ağ Topolojisi

Sistem, ağ cihazları arasındaki bağlantıların
görüntülenebilmesini sağlayacaktır.

Örneğin:

Internet
    ↓
Router
    ↓
Firewall
    ↓
Core Switch
    ↓
Access Switch
    ↓
Client Devices

şeklindeki bağlantı yapısı sistem içerisinde
görüntülenebilecektir.

---

## 4. Ağ İzleme

Sistem ağ cihazlarının erişilebilirlik durumunu
kontrol edebilecektir.

Cihazlar aşağıdaki durumlardan birinde
görüntülenebilecektir:

- Online
- Offline
- Warning
- Unknown

İlk aşamada cihaz erişilebilirliği ping yöntemi
kullanılarak kontrol edilecektir.

---

## 5. Olay ve Alarm Yönetimi

Sistem aşağıdaki olayları tespit edebilmelidir:

- Cihazın erişilemez hale gelmesi
- Cihazın tekrar erişilebilir hale gelmesi
- Yüksek gecikme
- Paket kaybı
- Ağ bağlantısının kesilmesi

Oluşan olaylar kayıt altına alınacaktır.

---

## 6. Veritabanı

Ağ altyapısına ait bilgiler ilişkisel veritabanında
saklanacaktır.

Veritabanında temel olarak;

- Kurum
- Bina
- Kat
- Bölüm
- Cihaz
- Ağ arayüzü
- Bağlantı
- VLAN
- Olay

gibi bilgiler tutulması planlanmaktadır.

---

## 7. Ağ Topolojisinin Görselleştirilmesi

Sistemin ilerleyen aşamalarında ağ altyapısının
grafiksel olarak görüntülenmesi sağlanacaktır.

Kullanıcı;

- cihazları,
- cihazlar arasındaki bağlantıları,
- cihazların durumlarını,
- ağın genel yapısını

tek bir arayüz üzerinden görebilecektir.

---

## 8. Raporlama

Sistem ilerleyen aşamalarda aşağıdaki bilgilerin
raporlanmasını destekleyebilecektir:

- Toplam cihaz sayısı
- Aktif cihaz sayısı
- Pasif cihaz sayısı
- Oluşan alarm sayısı
- Ağ olayları
- Cihazların çalışma geçmişi

---

## 9. Gelecekte Eklenebilecek Özellikler

Projenin temel sürümü tamamlandıktan sonra aşağıdaki
özellikler geliştirilebilir:

- SNMP desteği
- Trafik izleme
- Otomatik ağ keşfi
- Port durumlarının izlenmesi
- Daha gelişmiş ağ istatistikleri
- E-posta bildirimi
- 3D bina görünümü
- Gelişmiş raporlama