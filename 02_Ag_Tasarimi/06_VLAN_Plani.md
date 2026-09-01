# VLAN Planı

Kurumsal ağda farklı kullanım amaçlarına sahip
cihazların birbirinden mantıksal olarak ayrılması
amacıyla VLAN yapısı kullanılacaktır.

## VLAN Grupları

| VLAN ID | VLAN Adı | Kullanım |
|---:|---|---|
| 10 | MANAGEMENT | Ağ cihazlarının yönetimi |
| 20 | USERS | Personel bilgisayarları |
| 30 | SERVERS | Sunucular |
| 40 | SECURITY | IP kamera sistemleri |
| 50 | GUEST | Misafir kullanıcılar |
| 60 | VOICE | IP telefonlar |

## VLAN Açıklamaları

### VLAN 10 - MANAGEMENT

Router, switch ve diğer ağ cihazlarının yönetim
arayüzleri için kullanılacaktır.

### VLAN 20 - USERS

Kurum personelinin bilgisayarları ve standart
kullanıcı cihazları için kullanılacaktır.

### VLAN 30 - SERVERS

Kurum içerisinde bulunan sunucular bu VLAN
içerisinde bulunacaktır.

### VLAN 40 - SECURITY

IP kamera sistemleri bu VLAN içerisinde
bulunacaktır.

### VLAN 50 - GUEST

Misafir kullanıcıların internete erişimi için
kullanılacaktır.

Misafir ağının kurum içindeki sunuculara ve
yönetim ağlarına erişimi engellenecektir.

### VLAN 60 - VOICE

IP telefonlar için ayrılmış ağ olacaktır.