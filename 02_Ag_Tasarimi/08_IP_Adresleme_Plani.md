# IP Adresleme Planı

Bu dokümanda Manavgat Belediyesi için tasarlanan kurumsal ağın IP adresleme yapısı belirlenecektir.

IP adresleri VLAN yapısı temel alınarak mantıksal olarak ayrılacaktır.

Her VLAN için;

- Ağ adresi
- Subnet maskesi
- Default gateway
- Kullanılabilir IP aralığı
- Broadcast adresi

belirlenecektir. 


VLAN 10 → MANAGEMENT ağı
VLAN 20 → USERS ağı
VLAN 30 → SERVER ağı
VLAN 40 → SECURITY ağı
VLAN 50 → GUEST ağı
VLAN 60 → VOICE ağı

Projemizde özel IPv4 adresleri kullanacağız.

10.10.0.0/16
       │
       ├── VLAN 10
       ├── VLAN 20
       ├── VLAN 30
       ├── VLAN 40
       ├── VLAN 50
       └── VLAN 60

# Neden Neden /16?
Subnet Mask:

255.255.0.0   demektir, dolayısıyla;  10.10.0.0 ile 10.10.255.255 arasında çok büyük bir adres alanımız var.

Biz bunun tamamını kullanmayacağız.

Bunun avantajı şu:

İleride kurum büyürse:

VLAN 70
VLAN 80
VLAN 90

gibi ağlar ekleyebiliriz.

# Gateway'i belirleme

Bir VLAN'ın dışındaki başka bir VLAN'a gitmek istediğinde cihazın bir default gateway'e ihtiyacı vardır.

Bu projede gateway standardımız:

Her VLAN'ın ilk kullanılabilir IP'si gateway olacak.


## VLAN ve IP Adresleme Tablosu

| VLAN ID | VLAN Adı | Network | Subnet Mask | Gateway | Host Aralığı | Broadcast |
|---:|---|---|---|---|---|---|
| 10 | MANAGEMENT | 10.10.10.0/24 | 255.255.255.0 | 10.10.10.1 | 10.10.10.2 - 10.10.10.254 | 10.10.10.255 |
| 20 | USERS | 10.10.20.0/24 | 255.255.255.0 | 10.10.20.1 | 10.10.20.2 - 10.10.20.254 | 10.10.20.255 |
| 30 | SERVERS | 10.10.30.0/24 | 255.255.255.0 | 10.10.30.1 | 10.10.30.2 - 10.10.30.254 | 10.10.30.255 |
| 40 | SECURITY | 10.10.40.0/24 | 255.255.255.0 | 10.10.40.1 | 10.10.40.2 - 10.10.40.254 | 10.10.40.255 |
| 50 | GUEST | 10.10.50.0/24 | 255.255.255.0 | 10.10.50.1 | 10.10.50.2 - 10.10.50.254 | 10.10.50.255 |
| 60 | VOICE | 10.10.60.0/24 | 255.255.255.0 | 10.10.60.1 | 10.10.60.2 - 10.10.60.254 | 10.10.60.255 |