# Revisión del informe EDD_EF_Grupo07.pdf frente al código

Revisión del 16/11/2026: se leyeron las 32 páginas, incluidas las figuras. Código comparado: commit `9a86ba4` (`RipleyComprasMAUI/`, .NET 10, con la corrección de los selectores).

**Veredicto.** Los cálculos del informe coinciden con el código: árbol, totales, ahorro, IGV, mensajes de validación y datos de `productos.json`. El problema está en tres frentes:
- Los diagramas de diseño describen una versión anterior: la fila va antes de las prendas, `boletas.json` y otros nombres de métodos.
- Quedan restos del proyecto de billetera móvil: el título, el Anexo B y el enlace al repositorio.
- Hay contradicciones internas de estado ("Por ejecutar" frente a "Probado ✅") y de numeración de imágenes y tablas.

## A. Informe frente al código

| # | Dónde | Qué dice el informe | Qué hace el código | Recomendación |
|---|---|---|---|---|
| A1 | Fig. 3 (TO-BE), Diag. 02, secuencia, estados, CP-05/06 | Primero la fila y luego se registran las prendas en caja | Las prendas se eligen en **Preventa** y la compra entra a la fila con *Enviar a caja*; en caja solo se corrige | Cambiar el informe (README §7.1) |
| A2 | 2.2 PostVenta, Fig. 3, CP-12, CP-16, clases | `boletas.json`; la Boleta copia importes y `DniOculto` | `ventas.json`: cada `Venta` contiene su `Boleta` (`Numero`, `FechaEmision`, `IdVenta`, `Leyenda`) | Cambiar el informe |
| A3 | CP-16 | El JSON guarda solo el DNI oculto | Guarda el DNI completo (ficticio) para buscarlo; lo oculta como `7xxxxx21` en mensajes | Cambiar el informe o decidir ocultarlo también en el archivo (se perdería la búsqueda por DNI) |
| A4 | CP-16, CP-17, Fig. 3 | Botones "Guardar datos" y "Omitir" | Campos *Cliente (opcional)* en Preventa; se omiten dejándolos vacíos | Cambiar el informe |
| A5 | Diagrama de clases | `CargarAsync`, `Limpiar`, `RegistrarDatosCliente`, `FinalizarCompra`, `GenerarBoleta`, `CargarBoletasAsync`, `ValidarCodigo`… | `Cargar`, `Vaciar`, datos en `RegistrarLlegada`, `ConfirmarPagoSimulado`, `PrepararBoleta`/`GuardarConNuevaVenta`, `CargarVentas`, `TryValidarCodigo`…; además `BuscarPorDni`/`BuscarPorFecha` | Rehacer el diagrama desde `CODIGO_FUENTE.md` |
| A6 | Cap. III, Cap. V, clases | No aparece el grafo | `GrafoCajas` (Dijkstra) existe y tiene su pantalla *Red de cajas* | Agregar su fila en la matriz (Cap. III) y en la tabla de capas (Cap. V) |
| A7 | — | No se menciona | Bitácora por subproceso (pantalla *Bitácora*) | Mencionarla en Cap. V |
| A8 | CP-08, Fig. pila | Con la pila vacía aparece "No hay acciones para deshacer" | El botón *Deshacer* se desactiva cuando no hay acciones (el mensaje solo existe en el servicio) | Cambiar el resultado esperado a "Deshacer queda deshabilitado", o reactivar el botón en el código |
| A9 | CP-11 | "No se encontró la boleta" | La pantalla muestra "No existe la boleta N° 99" | Cambiar una de las dos (1 línea de código o 1 celda del informe) |
| A10 | CP-01 | Muestra 2 comparaciones y altura 3 | La pantalla solo dice "1 producto encontrado"; las comparaciones se ven en `Comprobaciones` | Evidencia de CP-01: captura de la consola `Comprobaciones` |
| A11 | CP-02 | Códigos `201537347471` y `201537337933` | Tienen 12 dígitos; el código los rechazaría | Corregir a `2015373474714` y `2015373379330` |
| A12 | Fig. 2 y 3, 1.5 | "Atender siguiente", "Deshacer" y "Registrar boleta" figuran como *propuesto / por verificar* | Ya están implementados | Pasarlos a línea sólida y quitar "(propuesto)" y "(verificar)" en 2.2 y 3.1 |
| A13 | Anexo D, Cap. V | Repositorio `tfhsl02/CAJA_AREQUIPA.git` y "GitHub (Anexo E)" | Repositorio real: `github.com/grupoupn760-code/MEMORY` | Cambiar el enlace y la referencia a Anexo D |

**Coincide con el código (sin cambios):** las 7 prendas y sus precios; el árbol de la Fig. 4.4 (raíz JEANS, 2 comparaciones para FEPAPAYA, rotaciones en la 3.ª, 5.ª, 6.ª y 7.ª inserción, altura 3); los totales S/ 199.97 → 179.97 (ahorro 20.00), 149.96, 239.97, 289.96 y 149.98; IGV 27.45/22.88 y op. gravada 152.52/127.08; cantidad 1–99 sin superar el stock; los mensajes "Código inválido", "Prenda no encontrada", "El carrito está vacío", "Número inválido", "DNI inválido" y "Correo inválido"; la leyenda "Boleta simulada"; la siguiente boleta, N° 3; los estados En fila / En caja / Finalizada; .NET 10, MAUI y System.Text.Json.

## B. Coherencia interna del documento

1. **Restos del otro proyecto.**
   - El título habla de "rediseño transaccional de una **billetera móvil**".
   - El Anexo B menciona ListaDoble/Circular, ColaOperaciones, PilaReversiones, ArbolOperaciones, GrafoAgencias, ProcesadorOperaciones, AlmacenDatos, "conectividad intermitente (Tabla 5)" y "Tabla 17".
   - La Tabla 1.5 (filas 2 y 3) habla de retener operaciones sin conectividad y de una pila de reversas, aunque el alcance excluye ambas cosas y la fila 5 dice que no se implementan.
2. **Estado contradictorio.** El alcance (1.2) y el Cap. VI dicen "Por ejecutar", pero el Anexo B marca todo como "Implementado ✅ Probado". Hay que alinearlos con la realidad: el código está implementado y las pruebas siguen pendientes.
3. **Numeración de imágenes.**
   - Las fotos no llevan número.
   - El texto usa "Imagen 2" para el carrito con precios, pero esa es la fila 3 de la bitácora.
   - La Tabla 1.5 y la Fig. 2 citan las Imágenes 3, 4, 5 y **8** con contenidos que no corresponden. No hay Imagen 8, ni una foto de "operación denegada, código 51".
4. **Cifras y horas de las fotos.**
   - La fila 5 dice S/ 131.18, pero la pantalla de la foto muestra S/ 510.85 y la hora 21:03.
   - La boleta (fila 7) es del 28/10/2026, no del 22/08/2026 19:40. La Fig. 2 lo reconoce, la bitácora no.
   - La visita figura de 18:00 a 19:40 en la bitácora y de 18:30 a 19:35 en el Anexo B.
5. **Tabla 1.4.**
   - "Clientes que llegaron a la zona de pago" figura en Preventa; corresponde a Venta.
   - "4 prendas" en la Imagen 2, pero 2.2 b) y la Fig. 4.4 dicen 3.
   - "Prendas en la boleta: 0", pero la boleta de la Imagen 7 tiene 4.
   - "3 operaciones con QR", pero la foto muestra una con QR, una con crédito y una con débito.
6. **Tablas sin número** citadas como "Tabla 2", "Tabla 3", "Tabla 4", "Tabla 5", "Tabla 7", "Tabla 14", "Tabla 2.1" y "Figura 1–6". Hay que numerarlas todas o quitar las referencias.
7. **Secciones mal rotuladas.**
   - 1.2 a) se titula "Fuera del alcance", pero contiene lo que está *dentro* del alcance; falta b).
   - El 2.º párrafo de 4.2 repite el texto de 1.6 sobre acoplamiento.
8. **Índice.** Los anexos van de A a D, pero el Cap. V cita "Anexo E".

## C. Forma y redacción

- Las tablas de las págs. 14 a 16 (matriz y traza) y 22 a 24 (casos de prueba) se salen del margen derecho y en el PDF se cortan columnas: "Eficiencia…" y "Resultado obtenido / Estado / Evidencia". Conviene ponerlas en horizontal o reducir columnas.
- Erratas: "comoras", "CarritoServicio", "Estructur as", "Fuera del Pre-venta", "(Imagen 6 y 7)" → "(Imágenes 6 y 7)"; el objetivo 6 está en pasado ("Implementamos").
- En el pie del Diag. 01 dice "Ripley Comas"; en el resto del texto, "caja 511" sin sede. Unificar.
- **Datos personales:** la foto de la boleta (fila 7) deja leer el nombre, el DNI y el correo de una persona real, la tarjeta parcial y el QR. (Corrección del 17/11: la pantalla de ingreso de datos de la fila 2 tiene los campos vacíos; no muestra datos.)

## Orden sugerido de corrección

1. Difuminar los datos personales de la foto de la boleta (fila 7).
2. Quitar los restos de la billetera móvil: título, Anexo B (tabla de módulos y responsabilidades), enlace del repositorio y filas 2 y 3 de la Tabla 1.5.
3. Alinear el diseño con el código: A1 a A6 y A12.
4. Corregir los casos de prueba: A3, A4, A8 a A11. Luego ejecutarlos y completar "Resultado obtenido".
5. Numerar las imágenes, tablas y figuras y corregir las cifras (B3 a B6).
6. Arreglar las tablas cortadas y las erratas.

## Estado (17/11/2026)

Aplicado en `docs/informe/EDD_EF_Grupo07_corregido.docx`. Además se encontró que **ninguna foto muestra el carrito de S/ 199.97 con "Usted ahorra S/ 20.00"**: la Imagen 3 real es un carrito de 8 prendas por S/ 510.85 sin ahorro, y el QR es por S/ 510.85 (no S/ 131.18). El Word usa el carrito A como caso de validación y deja una nota amarilla para que el equipo agregue la foto o lo declare construido.
