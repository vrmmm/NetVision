# Manavgat Belediyesi


## Kurum Tanımı

Manavgat Belediyesi, farklı departmanlarda çalışan
personellere sahip, sekiz katlı bir yönetim ve teknoloji
merkezinde faaliyet gösteren farazi bir kuruluştur.

Kurumun bilgisayar ağı; personel bilgisayarları,
sunucular, güvenlik sistemleri, kablosuz erişim noktaları
ve diğer ağ cihazlarının birbirleriyle ve internet
altyapısıyla haberleşmesini sağlayacak şekilde
tasarlanacaktır.

Kurumun ağ altyapısı bu proje kapsamında sıfırdan
tasarlanacak ve herhangi bir gerçek kurumun ağ verileri
kullanılmayacaktır.



# Bina Tanımı



| Kat   |       Birimler                  | Kullanıcı |
| ----- | --------------------------------|-----------|
| -2    | Teknik altyapı / depo           |     5     |
| -1    | Arşiv / lojistik                |     10    |
| Zemin | Resepsiyon / güvenlik / danışma |     15    |
| 1     | İnsan Kaynakları                |     20    |
| 2     | Muhasebe / Finans               |     25    |
| 3     | Yazılım / Ar-Ge                 |     25    |
| 4     | Yönetim / Toplantı              |     15    |
| 5     | Bilgi İşlem / Sunucu odası      |     25    |



Toplam:

145 kullanıcı.

Bu sayı tamamen bizim varsayımımız.

Gerçek belediye/personel verisi değil.

Her kullanıcı için en az bir bilgisayar bulunmaktadır.

| Cihaz        | Yaklaşık sayı |
| ------------ | ------------: |
| PC           |           145 |
| Printer      |            12 |
| Access Point |            16 |
| IP Camera    |            32 |
| IP Phone     |            40 |
| Server       |             6 |
| Switch       |            10 |
| Router       |             1 |
| Firewall     |             1 |

# İnternet Gereksinimleri



ISP
 │
 │ Fiber Optic
 ▼
ONT / Fiber Termination
 │
 ▼
Router
 │
 ▼
Firewall
 │
 ▼
Core Switch



Burada:

ONT'nin kullanılacağına dair varsayım yapıyoruz.

Gerçek bir ISP'nin özel cihazını modellemiyoruz.