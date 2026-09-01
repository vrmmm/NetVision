# Cisco CLI Doğrulama ve Kanıt Çıktıları

Bu dokümanda, kurumsal ağın kurulum ve yapılandırma sonrası Cisco IOS ve Cisco ASA CLI arayüzlerinden toplanan doğrulama komut çıktıları yer almaktadır.

---

## 1. CORE-01 (Merkezi Switch) Çıktıları

### 1.1 Yönlendirme Tablosu (`show ip route`)
```text
CORE-01# show ip route
Codes: C - connected, S - static, R - RIP, M - mobile, B - BGP
       D - EIGRP, EX - EIGRP external, O - OSPF, IA - OSPF inter area 

Gateway of last resort is 10.10.10.3 to network 0.0.0.0

S*    0.0.0.0/0 [1/0] via 10.10.10.3
      10.0.0.0/8 is variably subnetted, 6 subnets, 1 masks
C        10.10.10.0/24 is directly connected, Vlan10
C        10.10.20.0/24 is directly connected, Vlan20
C        10.10.30.0/24 is directly connected, Vlan30
C        10.10.40.0/24 is directly connected, Vlan40
C        10.10.50.0/24 is directly connected, Vlan50
C        10.10.60.0/24 is directly connected, Vlan60
```

---

### 1.2 DHCP İstemci Dağıtım Tablosu (`show ip dhcp binding`)
```text
CORE-01# show ip dhcp binding
IP address       Client-ID/              Lease expiration        Type
                 Hardware address
10.10.20.21      0060.2F3A.44A1          Aug 24 2026 10:15 AM    Automatic
10.10.20.22      0001.9655.B802          Aug 24 2026 10:18 AM    Automatic
10.10.20.23      0090.21C4.128F          Aug 24 2026 10:22 AM    Automatic
10.10.40.21      000C.8540.A112          Sep 16 2026 09:00 AM    Automatic
10.10.50.21      000A.4178.63E9          Aug 18 2026 10:30 AM    Automatic
10.10.60.21      00D0.D314.598A          Aug 31 2026 11:00 AM    Automatic
```

---

### 1.3 VLAN Özeti (`show vlan brief`)
```text
CORE-01# show vlan brief

VLAN Name                             Status    Ports
---- -------------------------------- --------- -------------------------------
1    default                          active    
10   MANAGEMENT                       active    Gig0/1
20   USERS                            active    
30   SERVERS                          active    
40   SECURITY                         active    
50   GUEST                            active    
60   VOICE                            active    
```

---

### 1.4 Trunk Portları Durumu (`show interfaces trunk`)
```text
CORE-01# show interfaces trunk

Port        Mode             Encapsulation  Status        Native vlan
Gig0/2      on               802.1q         trunking      1
Gig0/3      on               802.1q         trunking      1
Gig0/4      on               802.1q         trunking      1
Gig0/5      on               802.1q         trunking      1
Gig0/6      on               802.1q         trunking      1
Gig0/7      on               802.1q         trunking      1
Gig0/8      on               802.1q         trunking      1
Gig0/9      on               802.1q         trunking      1
Gig0/10     on               802.1q         trunking      1

Port        Vlans allowed on trunk
Gig0/2-10   10,20,30,40,50,60
```

---

## 2. FW-01 (Cisco ASA 5506-X) Çıktıları

### 2.1 Güvenlik Bölgeleri ve IP Adresleri (`show nameif` & `show ip address`)
```text
FW-01# show nameif
Interface                Name                     Security
GigabitEthernet1/1       outside                    0
GigabitEthernet1/2       inside                   100

FW-01# show ip address
System IP Addresses:
Interface                Name                   IP address      Subnet mask     Method 
GigabitEthernet1/1       outside                203.0.113.2     255.255.255.252 manual
GigabitEthernet1/2       inside                 10.10.10.3      255.255.255.0   manual
```

---

### 2.2 NAT Çeviri Durumu (`show xlate` & `show nat`)
```text
FW-01# show nat
Manual NAT Policies (Section 1)
1 (inside) to (outside) source dynamic OBJ_CORP_LAN interface 
    translate_hits = 184, untranslate_hits = 184

FW-01# show xlate
1 in use, 1 most used
Flags: D - DNS, d - dynamic, G - group, I - identity, i - inside,
       r - portmap, s - static, T - transpose, v - VPN, x - extended
NAT from inside:10.10.20.21 to outside:203.0.113.2 flags d
```

---

## 3. Erişim Switchi (SW-GF-01) MAC Adres Tablosu

```text
SW-GF-01# show mac address-table dynamic
          Mac Address Table
-------------------------------------------

Vlan    Mac Address       Type        Ports
----    -----------       --------    -----
  10    0001.4239.ab01    DYNAMIC     Gig0/1
  20    0060.2f3a.44a1    DYNAMIC     Fa0/2
  20    0001.9655.b802    DYNAMIC     Fa0/3
  40    000c.8540.a112    DYNAMIC     Fa0/18
  50    000a.4178.63e9    DYNAMIC     Fa0/22
  60    00d0.d314.598a    DYNAMIC     Fa0/2
```
