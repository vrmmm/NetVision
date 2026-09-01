# Port Hesapları

Her katta bulunan Ethernet bağlantısı gerektiren
cihazlar dikkate alınarak erişim portu ihtiyacı
hesaplanmıştır.

| Kat   | PC | Printer | AP | Kamera | IP Phone | Toplam Port |
|-----  |---:|--------:|---:|-------:|---------:|------------:|
| -2    | 5  | 1       | 1  | 4      | 1        | 12          |
| -1    | 10 | 1       | 1  | 4      | 2        | 18          |
| Zemin | 15 | 2       | 2  | 4      | 4        | 27          |
| 1     | 20 | 2       | 2  | 4      | 5        | 33          |
| 2     | 25 | 2       | 2  | 4      | 7        | 40          |
| 3     | 35 | 2       | 2  | 4      | 8        | 51          |
| 4     | 20 | 1       | 3  | 4      | 7        | 35          |
| 5     | 15 | 1       | 3  | 4      | 6        | 29          |


## Sonuç

3. katta 51 adet Ethernet bağlantısı ihtiyacı
bulunduğundan tek bir 48 port switch yeterli
olmayacaktır.

Bu nedenle 3. katta iki adet 48 port yönetilebilir
switch kullanılacaktır.

Diğer katlarda mevcut bağlantı ihtiyacını
karşılayabilecek 48 port yönetilebilir switchler
kullanılması planlanmaktadır.

                CORE
                  │
             Fiber uplink
                  │
          ┌───────┴───────┐
          │               │
       SW-03A          SW-03B
          │               │
       Devices         Devices