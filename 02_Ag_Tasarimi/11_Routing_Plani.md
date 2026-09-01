# Routing Planı

## Layer 3 Routing

Kurumsal ağ içerisindeki VLAN'lar arasında
iletişimin sağlanması için Core Switch üzerinde
Layer 3 routing kullanılacaktır.

Her VLAN için Core Switch üzerinde bir gateway
tanımlanacaktır.

## Gateway Yapısı

| VLAN | Network | Gateway |
|---:|---|---|
| 10 | 10.10.10.0/24 | 10.10.10.1 |
| 20 | 10.10.20.0/24 | 10.10.20.1 |
| 30 | 10.10.30.0/24 | 10.10.30.1 |
| 40 | 10.10.40.0/24 | 10.10.40.1 |
| 50 | 10.10.50.0/24 | 10.10.50.1 |
| 60 | 10.10.60.0/24 | 10.10.60.1 |

## Routing Mantığı

Örneğin VLAN 20 içerisindeki bir kullanıcı
VLAN 30 içerisindeki bir sunucuya erişmek
istediğinde trafik:

USER
↓
VLAN 20
↓
10.10.20.1
↓
CORE-01
↓
VLAN 30
↓
10.10.30.x
