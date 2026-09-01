# VLAN - Switch Matrisi

| Switch   | VLAN 10 | VLAN 20 | VLAN 30 | VLAN 40 | VLAN 50 | VLAN 60 |
|----------|-------- |---------|---------|---------|---------|---------|
| SW-M2-01 |    ✓    |   ✓    |   -     |    ✓    |    ✓    |    ✓   |
| SW-M1-01 |    ✓    |   ✓    |   -     |    ✓    |    ✓    |    ✓   |
| SW-GF-01 |    ✓    |   ✓    |   -     |    ✓    |    ✓    |    ✓   |
| SW-01-01 |    ✓    |   ✓    |   -     |    ✓    |    ✓    |    ✓   |
| SW-02-01 |    ✓    |   ✓    |   -     |    ✓    |    ✓    |    ✓   |
| SW-03-01 |    ✓    |   ✓    |   -     |    ✓    |    ✓    |    ✓   |
| SW-03-02 |    ✓    |   ✓    |   -     |    ✓    |    ✓    |    ✓   |
| SW-04-01 |    ✓    |   ✓    |   -     |    ✓    |    ✓    |    ✓   |
| SW-05-01 |    ✓    |   ✓    |   ✓     |   ✓     |    ✓    |    ✓   |
 


# Bağlantı Görünümü


                         INTERNET
                            │
                          ROUTER
                            │
                         FIREWALL
                            │
                       CORE-01
                            │
                         TRUNK
                            │
                        SW-03-01
                     /      |       \
                    /       |        \
              VLAN 20    VLAN 40   VLAN 60
               PC        Camera    IP Phone