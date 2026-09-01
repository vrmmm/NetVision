# Fiziksel Ağ Topolojisi

## Genel Yapı

Manavgat Belediyesi için tasarlanan ağ
altyapısında internet bağlantısı ISP üzerinden
sağlanacaktır.

İnternet bağlantısı sırasıyla router ve firewall
üzerinden kurumun Core Switch cihazına
ulaştırılacaktır.

Core Switch üzerinden katlardaki Access
Switchlere dağıtım gerçekleştirilecektir.

## Fiziksel Topoloji
 
                            
                              INTERNET
                                  │
                               FIBER
                                  │
                             ┌────▼────┐
                             │   ISP   │
                             └────┬────┘
                                  │
                             ┌────▼────┐
                             │ ROUTER  │
                             └────┬────┘
                                  │
                          ┌───────▼───────┐
                          │   FIREWALL    │
                          └───────┬───────┘
                                  │
                          ┌───────▼───────┐
                          │  CORE SWITCH  │
                          └───────┬───────┘
                                  │
       ┌────────┬────────┬───────┼───────┬────────┬─────────────┐
       │        │        │       │       │        │             │
      -2       -1      Zemin     1       2        3             4       5
       │        │        │       │       │        │             │       │
      SW       SW       SW      SW      SW        ├──SW-03A    SW      SW
                                                  └──SW-03B

