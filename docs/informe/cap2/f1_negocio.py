from comun import *
def caja(id, titulo, lineas, fill="#FFFFFF"):
    filas=''.join(f'<TR><TD ALIGN="LEFT"><FONT POINT-SIZE="9.5" COLOR="#3A3640">{l}</FONT></TD></TR>' for l in lineas)
    return f'{id} [shape=box, style="rounded,filled", fillcolor="{fill}", label=<<TABLE BORDER="0" CELLSPACING="1" CELLPADDING="1"><TR><TD ALIGN="LEFT"><B>{titulo}</B></TD></TR>{filas}</TABLE>>, width=2.9];'
c='rankdir=LR;\n'
filas=[
 ("pre","1. Preventa - selección de productos (piso de ventas)",
  ["Cliente que entra a la tienda","Prendas exhibidas con su etiqueta","(código de 13 dígitos y precio)"],
  ["Recorrer el piso de ventas","Elegir una prenda","Repetir mientras elija otra","Ir a la zona de pago"],
  ["Prendas elegidas por el cliente","Cliente con sus prendas","en la zona de pago"]),
 ("ven","2. Venta - pago (caja 511)",
  ["Cliente con sus prendas","Código y cantidad de cada prenda","DNI y correo, u omitir","Medio de pago: Tarjeta Ripley,","otra tarjeta o QR"],
  ["Atender en orden de llegada (fila)","Registrar y validar cada código","Ajustar cantidades (+ / −)","Mostrar total Tarjeta Ripley y normal","Cobrar y esperar la confirmación"],
  ["Turno del cliente en la fila","Aviso: código inválido o sin stock","Total a pagar según la tarjeta","Pago confirmado o","pedido de otro medio de pago"]),
 ("pos","3. Postventa - boleta y consulta",
  ["Pago confirmado","Prendas, total y datos del cliente","REF de la operación a consultar"],
  ["Registrar la operación con su REF","Imprimir el voucher Niubiz","Emitir y entregar la boleta (IGV 18 %)","Buscar la operación por REF"],
  ["Voucher con REF, monto y medio","Boleta impresa con IGV","Monto, medio y hora,","o aviso de que no se encontró"]),
]
for k,(id,t,e,p,s) in enumerate(reversed(filas)):
    c+=f'subgraph cluster_{id} {{ label=<<B>{t}</B>>; labeljust=l; style="rounded,filled"; fillcolor="#F4F2F7"; color="#D9D3E3";\n'
    c+=caja(id+'E','Entradas',e)+'\n'+caja(id+'P','Procesos',p,"#E9E6EE")+'\n'+caja(id+'S','Salidas',s)+'\n'
    c+=f'{id}E -> {id}P -> {id}S; }}\n'
c+='{rank=same; preE; venE; posE}\n{rank=same; preP; venP; posP}\n{rank=same; preS; venS; posS}\n'
render('f1_negocio',c)
