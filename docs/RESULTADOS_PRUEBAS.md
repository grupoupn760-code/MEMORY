# Resultados de los casos de prueba (Tabla 14)

Fuente: capturas del informe `EDD_EF_Grupo07_1.pdf` (37 págs.), revisadas el 20/11/2026. Solo se marca **Validado** si la captura muestra el resultado esperado.

| ID | Resultado obtenido | Estado | Qué falta / nota |
|---|---|---|---|
| CP-01 | 2015373474714: “1 producto encontrado.”, POL MC FEPAPAYA INX, S/ 59.99 y T. Ripley S/ 39.99 (20 disponibles). 9999999999999: “Prenda no encontrada.” | Validado parcialmente | Falta la captura de Comprobaciones (2 comparaciones, altura 3), o quitar esa frase del esperado. |
| CP-02 | — | Pendiente | 20/11: las nuevas capturas siguen sin cuadrar (carrito B y un código de 13 dígitos con “Prenda no encontrada.”). Opción: reutilizar la captura del carrito A de CP-04 (FEPAPAYA, FALDA, ESTRE; S/ 199.97), poner la entrada en ese orden y capturar “20153734” con el aviso “Código inválido: debe tener exactamente 13 dígitos (0-9); tiene 8 caracteres.” |
| CP-03 | POL MC FEPAPAYA INX ×1 (S/ 59.99) y FALDA LARG FESTIDO I ×2 (S/ 179.98); total S/ 239.97 (con Tarjeta Ripley S/ 219.97, ahorro S/ 20.00). | Validado | — |
| CP-04 | Carrito A: total S/ 199.97 (con Tarjeta Ripley S/ 179.97). Carrito B: total S/ 149.96. | Validado | Usar como evidencia también la captura del carrito B que hoy está en CP-02. |
| CP-05 | Compra del carrito A en caja con “Tarjeta Ripley” seleccionada; precios aplicados S/ 39.99, S/ 89.99 y S/ 49.99. | Validado | — |
| CP-06 | Turno 2 (carrito B) con “Otra tarjeta”: total a pagar S/ 149.96, ahorro S/ 0.00. | Validado | — |
| CP-07 | Total regular S/ 199.97, ahorro S/ 20.00, total a pagar S/ 179.97 (op. gravada S/ 152.52, IGV S/ 27.45). | Validado | — |
| CP-08 | Falda con 2 unidades antes de deshacer. | Pendiente | 21/11: mismas capturas; las de los botones no muestran el carrito y “Deshacer” sigue activo en las tres. Opción: cambiar la entrada a su carrito (FALDA, JEANS, FEPAPAYA) y capturar el carrito completo tras “Deshacer” (falda ×1, total S/ 239.97) y el botón gris con la pila vacía. |
| CP-09 | 12 compras en la fila (“Clientes en fila: 12”); el botón muestra “Atender siguiente (turno 1)”; la bitácora registra las llegadas 1 a 12 en orden (22:43–23:08 del 20/11/2026). | Validado parcialmente | Falta atender hasta vaciar la fila y mostrar “Atender siguiente” deshabilitado con un cliente en caja. |
| CP-10 | — | Pendiente | 21/11: la captura es el panel Pago antes de confirmar (Otra tarjeta, S/ 239.97, op. gravada 203.36, IGV 36.61; cálculo correcto), no el carrito B ni la boleta. Opción: cambiar entrada y esperado a esta compra y capturar la boleta que aparece tras “Confirmar pago”. |
| CP-11 | Número 1: “1 boleta encontrada”, Boleta N° 000001 (06/11/2026, Tarjeta Ripley, S/ 179.98). Número 99: “No existe la boleta N° 99”. | Validado | — |
| CP-12 | La boleta N° 000001 (emitida el 06/11/2026) se consultó el 20/11/2026, después de cerrar y abrir la app; también se conservan la 000003 (07/11) y la 000004 (17/11). | Validado parcialmente | Persistencia comprobada con la captura de CP-11. La entrada dice “2 boletas” y hay al menos 4; falta mostrar el número de la siguiente boleta. |
| CP-13 | — | Pendiente | 21/11: misma captura (selector abierto). Falta el panel Pago del carrito A con “Otra tarjeta”: total regular y final S/ 199.97, ahorro S/ 0.00, op. gravada S/ 169.47, IGV S/ 30.50. |
| CP-14 | Compra sin prendas con precio especial (POL MC ESTRE INX COR ×2, HI ML CUERNO AZI COR, SWE ML DESTAR INX CO) con Tarjeta Ripley: total regular y a pagar S/ 139.96, ahorro S/ 0.00, op. gravada S/ 118.61, IGV S/ 21.35; con Otra tarjeta, los mismos importes. | Validado | 21/11: nuevas capturas. Cambiar la entrada a esta compra (no es el carrito B). |
| CP-15 | Boleta N° 000004 con Tarjeta Ripley: total regular S/ 199.97, ahorro S/ 20.00, op. gravada S/ 152.52, IGV S/ 27.45, total final S/ 179.97, 3 unidades. | Validado | Mover aquí la captura de la boleta 000004 (hoy en CP-14); la captura actual de CP-15 es la de CP-06. |
| CP-16 | En caja el DNI aparece oculto (primer dígito, “xxxxx” y los dos últimos); la boleta muestra el DNI y el correo registrados. | Validado | Corregir el esperado: la app oculta con “xxxxx” y deja visibles el primer dígito y los dos últimos. Usar datos inventados (por ejemplo 70000021 → 7xxxxx21) o difuminar DNI y correo reales. |
| CP-17 | Con los campos vacíos, la boleta N° 000003 sale como “Cliente anónimo”, con DNI y correo “no registrado”. | Validado parcialmente | Falta la captura de “DNI inválido” y “Correo inválido”. |
| CP-18 | Caja 511 → Central: ruta por Caja 512, Caja 513 y Servidor de tienda, 26 ms. Con Servidor–Central CAÍDO: ruta por el Almacén, 34 ms. Con Almacén–Central también CAÍDO: “No hay una ruta disponible entre Caja 511 y Central (simulada).” | Validado parcialmente | 21/11: 3 capturas nuevas. Falta el tiempo inválido (“abc” → “Tiempo: no es un número válido.”); o capturarlo, o quitar esa frase del esperado y marcar Validado. |
| CP-19 | Filtro “Venta - Pago”: 12 registros con fecha y hora, acción, turno y resultado (22:43–23:08 del 20/11/2026). | Validado | — |

Resumen (21/11): 10 validados, 5 validados parcialmente, 4 pendientes (CP-02, 08, 10 y 13).

## Observaciones de las capturas
- **Datos personales:** las capturas muestran nombres, DNI y correos reales, aunque el informe dice que los datos son inventados. Difuminarlos o repetir con datos inventados.
- **Selector de tarjeta (P-19):** en la captura de CP-13 el desplegable sale oscuro con la marca rosada de Windows, no con el morado de `Platforms/Windows/App.xaml`. Se lee, pero el estilo no se aplicó.
- **Detalle visual:** en Venta los artículos se ven como “1 xS/ 39.99” (falta un espacio después de la “x”).

## Informe final (25/11/2026)
En `EDD_EF_Grupo07.pdf` final el equipo marca los 19 casos como **Validado**, con entradas y esperados ajustados a las capturas (CP-02 con el aviso de 8 dígitos; CP-08 con su carrito FALDA/JEANS/FEPAPAYA; CP-10 con la boleta 000005 de S/ 239.97; CP-12 con 5 boletas; CP-13 con el panel de Otra tarjeta; CP-14 con la compra de S/ 139.96; CP-18 con 26 ms, 34 ms, sin ruta y tiempo “abc”).
Observaciones que quedaron: CP-09 (lo obtenido muestra la fila de 12, no la atención hasta vaciarla) y CP-16 (esperado “6******0”; la app muestra el primer dígito, “xxxxx” y los dos últimos; el DNI usado no parece inventado).
