# DHCP Planı

Kullanıcı cihazlarının IP adreslerinin otomatik
olarak atanması amacıyla DHCP kullanılacaktır.

## DHCP Kullanılacak VLAN'lar

- VLAN 20 - USERS
- VLAN 40 - SECURITY
- VLAN 50 - GUEST
- VLAN 60 - VOICE

## Statik IP Kullanacak Ağlar

- VLAN 10 - MANAGEMENT
- VLAN 30 - SERVERS

## DHCP Havuzları

### USERS

Network:
10.10.20.0/24

Gateway:
10.10.20.1

DHCP Range:
10.10.20.100 - 10.10.20.200

### SECURITY

Network:
10.10.40.0/24

Gateway:
10.10.40.1

DHCP Range:
10.10.40.100 - 10.10.40.200

### GUEST

Network:
10.10.50.0/24

Gateway:
10.10.50.1

DHCP Range:
10.10.50.100 - 10.10.50.200

### VOICE

Network:
10.10.60.0/24

Gateway:
10.10.60.1

DHCP Range:
10.10.60.100 - 10.10.60.200