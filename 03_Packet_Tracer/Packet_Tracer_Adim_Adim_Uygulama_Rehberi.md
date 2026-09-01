# Cisco Packet Tracer Adım Adım Uygulama ve Sorun Giderme Rehberi

Bu rehber, **NetVision** projesinin Packet Tracer ortamında (`NetVision_Network_v1.pkt`) karşılaşılan kilit sorunların çözülmesini ve tüm konfigürasyonların sıfır hatayla uygulanmasını adım adım anlatır.

---

## 1. CORE-01 "User Access Verification" Kilitlenmesinin Çözümü

### Sorunun Tanımı
İzleme Raporunda belirtildiği üzere CORE-01 omurga switch'in CLI ekranına tıklandığında:
```text
User Access Verification
Username: 
```
şeklinde kimlik doğrulama ekranında kalması veya parolanın bilinmemesi durumunda yapılandırmaya erişilememektedir.

### Çözüm Yolu A: Standart Kimlik Bilgilerini Deneme
İlk olarak aşağıdaki standart kurumsal kimlik bilgilerini sırayla deneyiniz:
1. **Kullanıcı Adı**: `admin` | **Şifre**: `Cisco@123` (veya `cisco`, `class`, `admin123`)
2. **Kullanıcı Adı**: `cisco` | **Şifre**: `cisco`
3. Eğer `Password:` sorarsa: `cisco` veya `class`

---

### Çözüm Yolu B: Packet Tracer Şifre Sıfırlama / Password Recovery (Veri Kaybı Olmadan)
Eğer şifre bilinmiyorsa veya kilitlenme sürüyorsa, mevcut konfigürasyonu **silmeden** kurtarma prosedürü:

1. **Cihazı Yeniden Başlatma**:
   - Packet Tracer'da CORE-01 switch'in `Physical` sekmesine gidin.
   - Güç anahtarını kapatıp 2 saniye sonra tekrar açın (Power Cycle).
   - Veya CLI sekmesine gelip klavyeden `Ctrl + C` / `Ctrl + Break` kombinasyonuna art arda basın.
2. **Switch ROMMON / Bootloader Moduna Geçiş**:
   - Switch açılırken `switch:` istemine düşecektir:
   ```text
   switch: flash_init
   switch: load_helper
   switch: dir flash:
   switch: rename flash:config.text flash:config.old
   switch: boot
   ```
3. **Konfigürasyonu Geri Yükleyip Yeni Şifre Tanımlama**:
   - Switch açıldığında `Would you like to enter the initial configuration dialog? [yes/no]:` sorusuna `no` yazıp Enter'a basın.
   ```text
   Switch> enable
   Switch# rename flash:config.old flash:config.text
   Switch# copy flash:config.text system:running-config
   CORE-01# configure terminal
   CORE-01(config)# username admin privilege 15 secret Cisco@123
   CORE-01(config)# enable secret Cisco@123
   CORE-01(config)# line con 0
   CORE-01(config-line)# login local
   CORE-01(config-line)# exit
   CORE-01(config)# line vty 0 15
   CORE-01(config-line)# login local
   CORE-01(config-line)# transport input ssh telnet
   CORE-01(config-line)# exit
   CORE-01(config)# exit
   CORE-01# write memory
   ```
4. Artık `CORE-01` üzerindeki tüm VLAN, DHCP ve Routing yapılandırması korunmuş ve yönetici şifresi `admin` / `Cisco@123` olarak güncellenmiştir.

---

## 2. Cisco Cihazlarına Konfigürasyonların Yüklenmesi

Tüm konfigürasyon dosyaları [04_Cisco_Konfigurasyonlari/](file:///c:/Users/user/OneDrive/Desktop/NetVision/04_Cisco_Konfigurasyonlari/) dizininde modüler olarak hazırlanmıştır.

### 2.1 ISP-RTR Yapılandırması
1. Packet Tracer'da `ISP-RTR` (Cisco 2911) cihazına tıklayın -> `CLI` sekmesine geçin.
2. `04_Cisco_Konfigurasyonlari/03_ISP-RTR_Config.txt` içeriğini kopyalayıp doğrudan CLI ekranına sağ tıklayarak **Paste** yapın.
3. Çıktıda hata olmadığını ve `GigabitEthernet0/0` ile `Loopback0` arayüzlerinin UP olduğunu kontrol edin:
   ```text
   show ip interface brief
   ```

---

### 2.2 FW-01 (Cisco ASA 5506-X) Yapılandırması
1. `FW-01` cihazına tıklayın -> `CLI` sekmesine geçin.
2. `enable` yazın (Şifre sorarsa boş bırakıp Enter'a basın).
3. `04_Cisco_Konfigurasyonlari/02_FW-01_ASA_Config.txt` dosyasındaki komutları yapıştırın.
4. Doğrulama komutları:
   ```text
   show nameif
   show ip address
   show nat
   show route
   ```

---

### 2.3 CORE-01 (Merkezi Switch) Yapılandırması
1. `CORE-01` CLI ekranına geçip `enable` moduna girin.
2. `04_Cisco_Konfigurasyonlari/01_CORE-01_Config.txt` içeriğini yapıştırın.
3. Doğrulama komutları:
   ```text
   show ip route
   show ip dhcp pool
   show ip dhcp binding
   show vlan brief
   show interfaces trunk
   ```

---

### 2.4 Erişim Switchlerinin Yapılandırılması
Katlarda yer alan switchler için `04_Cisco_Konfigurasyonlari/04_Access_Switches_Configs.txt` içerisindeki ilgili bölümü sırayla ilgili switch'in CLI ekranına yapıştırın:
- `SW-M2-01` (Kat -2)
- `SW-M1-01` (Kat -1)
- `SW-GF-01` (Zemin Kat)
- `SW-01-01` (1. Kat)
- `SW-02-01` (2. Kat)
- `SW-03-01` (3. Kat - A)
- `SW-03-02` (3. Kat - B)
- `SW-04-01` (4. Kat)
- `SW-05-01` (5. Kat - Sunucular & Sistem Odası)

---

## 3. Uç Cihazların (PC, Laptop, Yazıcı, IP Phone) IP Testleri

1. **Kullanıcı PC'leri (VLAN 20)**:
   - PC'ye tıklayın -> `Desktop` -> `IP Configuration` -> `DHCP` seçeneğini işaretleyin.
   - PC'nin `10.10.20.100 - 10.10.20.200` aralığından IP aldığını, Gateway'in `10.10.20.1`, DNS'in `10.10.30.10` olduğunu doğrulayın.
2. **Misafir Cihazları (VLAN 50)**:
   - Misafir laptopuna tıklayın -> DHCP ile `10.10.50.x` IP'si aldığını doğrulayın.
   - Command Prompt açıp `ping 10.10.30.10` (Sunucu) deneyin -> **Request timed out / Destination host unreachable** (ACL Başarılı!).
   - `ping 8.8.8.8` (İnternet) deneyin -> **Reply from 8.8.8.8: bytes=32 time=... TTL=...** (NAT ve İnternet Başarılı!).
3. **Sunucular (VLAN 30 - Kat 5)**:
   - Statik IP'leri tanımlayın:
     - `SRV-01`: IP `10.10.30.10`, Mask `255.255.255.0`, Gateway `10.10.30.1`
     - `SRV-02`: IP `10.10.30.11`, Mask `255.255.255.0`, Gateway `10.10.30.1`
     - `SRV-03`: IP `10.10.30.12`, Mask `255.255.255.0`, Gateway `10.10.30.1`
     - `SRV-04`: IP `10.10.30.13`, Mask `255.255.255.0`, Gateway `10.10.30.1`
     - `SRV-05`: IP `10.10.30.14`, Mask `255.255.255.0`, Gateway `10.10.30.1`
     - `SRV-06`: IP `10.10.30.15`, Mask `255.255.255.0`, Gateway `10.10.30.1`
