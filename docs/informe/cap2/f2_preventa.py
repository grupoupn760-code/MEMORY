from kit import *
d=D(560,435); cx=280; W=270
d.t(20,24,"Preventa: el cliente elige sus prendas en el piso de ventas",14,True,anchor="start")
d.box(cx,44,240,44,"El cliente entra a la tienda","Inicio",term=True)
d.box(cx,112,W,44,"Recorre el piso de ventas","Cliente (Imagen 1)"); d.fl([(cx,88),(cx,111)])
d.box(cx,180,W,44,"Elige una prenda","Cliente"); d.fl([(cx,156),(cx,179)])
d.rombo(cx,284,160,58,["¿Elige otra","prenda?"]); d.fl([(cx,224),(cx,254)])
d.fl([(cx-80,284),(70,284),(70,202),(cx-W/2,202)],"Sí",82,277)
d.box(cx,370,260,48,"Va a la zona de pago","Fin - pasa a Venta con sus prendas",term=True)
d.fl([(cx,313),(cx,369)]); d.t(cx+14,346,"No",11.5,anchor="start")
d.png("f2_preventa.png")
