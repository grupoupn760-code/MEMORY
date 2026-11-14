# RipleyComprasMAUI

Prototipo académico para el curso **Estructuras de Datos (CIIN1190P, UPN 2026-2)**. Simula el proceso de compra
de prendas en una caja de Ripley en tres subprocesos:

- **Preventa:** selección de productos.
- **Venta:** fila FIFO de clientes, elección de tarjeta y pago **simulado**.
- **Postventa:** boleta **simulada** y consultas.

Está hecho en C#, .NET 10 y .NET MAUI (aplicación de escritorio Windows), con persistencia en JSON mediante `System.Text.Json`.
No procesa pagos reales, no pide datos de tarjeta y no emite comprobantes tributarios.

---

## 1. Arquitectura (sigue la imagen de referencia)

```
RipleyComprasMAUI/                      ← solución
├─ RipleyComprasMAUI.slnx
├─ README.md
├─ RipleyComprasMAUI/                   ← proyecto MAUI (distribución de la imagen)
│  ├─ Models/        Producto.cs, ItemCarrito.cs, Venta.cs, Boleta.cs
│  ├─ Structures/    NodoSimple.cs, ListaEnlazada.cs, PilaAcciones.cs, ColaCompras.cs, NodoArbol.cs, ArbolBusqueda.cs
│  │                 + GrafoCajas.cs (complemento exigido por el desafío; no figura en la imagen)
│  ├─ Services/      CatalogoService.cs, CarritoService.cs, CompraService.cs, BoletaService.cs
│  ├─ Data/          JsonDataManager.cs
│  ├─ Helpers/       InputHelper.cs
│  ├─ UI/            MainPage.xaml, MainPage.xaml.cs, EstilosNativos.cs (tema claro del calendario en Windows)
│  ├─ Resources/Raw/ productos.json      ← única semilla JSON empaquetada (MauiAsset)
│  ├─ App.xaml, App.xaml.cs, MauiProgram.cs, RipleyComprasMAUI.csproj
│  └─ Platforms/Windows, Properties, Resources/{Styles,Fonts,AppIcon,Splash}   ← archivos técnicos de la plantilla
├─ Comprobaciones/                      ← consola OPCIONAL de comprobaciones (fuera del proyecto MAUI)
└─ docs/verificacion_python/            ← réplica Python usada para verificar los cálculos (opcional)
```

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Modelos | `Producto` | Prenda: Codigo, Nombre, PrecioRegular, PrecioTarjetaRipley y Stock. |
| | `ItemCarrito` | Línea de compra. Copia los dos precios al agregarse y guarda `PrecioAplicado`, así conserva el valor histórico. |
| | `Venta` | Cliente (nombre, DNI y correo opcionales), turno, estado, artículos, tarjeta, fechas e importes. Su propiedad `Boleta` vale null hasta emitir el comprobante. |
| | `Boleta` | Datos propios del comprobante: Numero, FechaEmision, IdVenta y Leyenda. |
| Estructuras | `NodoSimple<T>` | Nodo con `Dato` y `Siguiente`. |
| | `ListaEnlazada<T>` | Carrito, historial de ventas, bitácoras y listas de adyacencia. |
| | `PilaAcciones<T>` | Deshacer los cambios del carrito (LIFO). |
| | `ColaCompras<T>` | Fila de clientes con sus compras (FIFO). |
| | `NodoArbol` / `ArbolBusqueda` | Árbol AVL: catálogo por `Codigo` (string) e índice de boletas por `Boleta.Numero` (int). |
| | `GrafoCajas` (complemento) | Red ficticia de cajas con pesos, enlaces activos o caídos y Dijkstra. |
| Servicios | `CatalogoService` | Carga y valida el catálogo, lo inserta en el árbol, busca por código o por nombre y lista. |
| | `CarritoService` | Único responsable del carrito: agregar, cambiar cantidad, quitar, recorrer, vaciar y deshacer. |
| | `CompraService` | Turnos, cola FIFO, compra en atención, tarjeta, importes y coordinación de la confirmación. |
| | `BoletaService` | Genera y registra la `Boleta`, mantiene historial e índice, y consulta por número, DNI y fecha. |
| Datos | `JsonDataManager` | Única clase de persistencia: rutas, `Cargar<T>`, `Guardar<T>` y escritura temporal + reemplazo. |
| Ayudas | `InputHelper` | Validación tipada, normalización para buscar por nombre y formato de importes y fechas. |
| Interfaz | `UI/MainPage` | Una sola página con las secciones Preventa, Venta, Boleta, Bitácora y Red de cajas. |

**Separación CarritoService / CompraService.**
- `CarritoService` es el único que modifica artículos. Usa la `ListaEnlazada` y la `PilaAcciones` de cada `ContextoCarrito`.
- Cada cliente tiene su propio contexto (lista y pila), así no se mezclan historiales.
- `CompraService` nunca agrega, quita ni cambia cantidades por su cuenta:
  - al registrar la llegada pide a `CarritoService` una **copia independiente** de los artículos;
  - si hay que corregir la compra en atención (por ejemplo, por falta de stock), pide a `CarritoService` un contexto de corrección propio, con su propia pila, y vuelve a calcular los importes.
- La política de precios por tarjeta está solo en `CompraService.PrecioSegunTarjeta`. `CarritoService` solo suma el total regular.

**Relación Venta–Boleta sin duplicar datos.**
- `Venta` guarda los artículos, el cliente, la tarjeta y los importes. `Venta.Boleta` contiene la `Boleta`.
- `Boleta` guarda solo sus datos propios (número, fecha de emisión y leyenda) y `IdVenta`. No guarda otra copia de la venta ni una referencia de vuelta a ella, así no hay ciclos al serializar.
- Para mostrar el comprobante completo, `BoletaService.ObtenerComprobante` combina los dos modelos.

**Búsqueda por nombre sin mayúsculas ni tildes.**
- `InputHelper.NormalizarParaBusqueda` aplica, solo para comparar, estos pasos:
  1. descompone el texto (`FormD`);
  2. descarta las marcas diacríticas;
  3. pasa a mayúsculas;
  4. deja un solo espacio entre palabras.
- `CatalogoService.BuscarPorNombre` recorre el catálogo y usa `InputHelper.NombreContiene`.
- `"POLO ROJO"`, `"Polo Rojo"`, `"Poló rojo"` y `"  póló   RÓJO  "` dan la misma lista, tanto con acento compuesto como separado.
- El nombre guardado no cambia.

**Datos.**
- La semilla `Resources/Raw/productos.json` se empaqueta como MauiAsset con el nombre lógico `productos.json`.
- Los archivos que se modifican se crean en la carpeta privada `FileSystem.AppDataDirectory/data`: ver la sección 3.

**Complementos fuera de la imagen.**
- `Structures/GrafoCajas.cs`: el desafío pide que "un grafo debe modelar las rutas de sincronización entre agencias con conectividad intermitente".
- `Comprobaciones/`: consola opcional de comprobaciones, fuera del proyecto MAUI.
- No se agregaron servicios, modelos ni capas.

---

## 2. Requisitos y ejecución

1. Use Windows 10 (1809 o superior) o Windows 11 con **Visual Studio 2026**.
2. En *Visual Studio Installer → Modificar → Cargas de trabajo*, active **Desarrollo de interfaz de usuario de aplicaciones multiplataforma de .NET** (.NET MAUI). Incluye el SDK de .NET 10. Visual Studio 2022 no sirve para .NET 10: use Visual Studio 2026.
3. Verifique la instalación en una terminal:
   - `dotnet --list-sdks` debe mostrar una versión `10.0.x` (o superior);
   - `dotnet workload list` debe mostrar `maui` o `maui-windows`.
4. Abra `RipleyComprasMAUI.sln` (o `RipleyComprasMAUI.slnx` en Visual Studio 2026). Si `RipleyComprasMAUI` no aparece en negrita, haga clic derecho sobre él y elija **Establecer como proyecto de inicio**.
5. En la barra de depuración elija **Windows Machine** y pulse **F5**. La primera compilación descarga los paquetes NuGet, así que necesita internet.

El proyecto apunta solo a `net10.0-windows10.0.19041.0` porque se pidió una aplicación de escritorio Windows. La plantilla oficial
también incluye Android, iOS y Mac Catalyst. Para volver a multiplataforma, reemplace la línea `TargetFrameworks` del `.csproj` por la de la plantilla.

Cambios para .NET 10: el destino pasa a `net10.0-windows10.0.19041.0`, `Microsoft.Extensions.Logging.Debug` a la versión 10.0.0 y `Comprobaciones` a `net10.0`. En .NET 10 `DatePicker.Date` es `DateTime?`; la lectura de la fecha en Boleta ya contempla el valor nulo (usa la fecha de hoy).

---

## 3. Datos de trabajo (JSON)

| Archivo | Dónde está | Contenido |
|---|---|---|
| `Resources/Raw/productos.json` | En el proyecto (semilla empaquetada, solo lectura) | Las 7 prendas del caso. El stock es **ficticio**: 20 unidades por prenda. |
| `productos.json` (de trabajo) | `AppDataDirectory/data/` | Copia de la semilla; aquí se guardan los cambios de stock. |
| `ventas.json` (de trabajo) | `AppDataDirectory/data/` | Ventas finalizadas, cada una con su `Boleta`. |

- **Primer inicio:** se copia la semilla y se crea `ventas.json` con `[]`.
- **Inicios siguientes:** no se sobrescribe nada.
- **Ruta exacta:** aparece en el encabezado de la aplicación, en el campo *Carpeta de datos*, que se puede copiar. En aplicaciones MAUI sin empaquetar suele estar dentro de `%LOCALAPPDATA%`.
- **Qué no se guarda:** la fila, la pila de deshacer y la bitácora son estado de la sesión.

**Reiniciar los datos de prueba (de forma deliberada):** cierre la aplicación, borre `productos.json` y `ventas.json` (o la carpeta `data`)
de la ruta mostrada y vuelva a abrirla. La app nunca borra estos archivos por su cuenta.

**Archivos con problemas:**
- `[]` es una colección vacía válida.
- Un archivo vacío, con `null` o con JSON dañado **no se reemplaza**: la app muestra un mensaje rojo con el motivo.
  - Si falla `ventas.json`, no se registran ventas hasta corregirlo.
  - Si falla `productos.json`, el catálogo queda vacío.
- Los registros inválidos (código repetido, precio Tarjeta Ripley mayor que el regular, stock negativo, etc.) no se usan. Se muestran como advertencia y se conservan tal cual al volver a guardar.

**Control al confirmar un pago (dos archivos distintos).** `CompraService.ConfirmarPagoSimulado` sigue este orden:
1. Valida el stock actual.
2. Guarda `ventas.json` con la venta y su boleta. Si falla, no cambia nada y el cliente sigue en atención.
3. Guarda `productos.json` con el stock descontado. Si falla, **restaura** `ventas.json` sin la venta y el cliente sigue en atención: se puede reintentar sin duplicar.
4. Solo entonces cambia la memoria: historial, índice AVL, stock (una sola vez) y consecutivo.

Alcance de este control:
- Si fallan a la vez el guardado de productos y la restauración de ventas, la venta se mantiene (ya está en `ventas.json`) para no duplicarla.
- En ese caso el stock correcto queda en memoria y se escribe en el siguiente guardado exitoso.
- Si la app se cierra antes de ese guardado, ese descuento no llega a `productos.json`. Es un límite aceptado del prototipo, que no tiene transacciones.

**Ejemplo de `ventas.json`** (se crea en ejecución; no es otro asset del proyecto; el identificador y las fechas varían):

```json
[
  {
    "Id": "6f1c2a9e-3b4d-4c5e-8f70-1a2b3c4d5e6f",
    "Turno": 1,
    "NombreCliente": "Cliente Prueba",
    "Dni": "70000021",
    "Correo": "cliente.prueba@correo.com",
    "Estado": "Finalizada",
    "Articulos": [
      { "Codigo": "2015373474714", "Nombre": "POL MC FEPAPAYA INX", "Cantidad": 1, "PrecioRegular": 59.99, "PrecioTarjetaRipley": 39.99, "PrecioAplicado": 39.99 },
      { "Codigo": "2015373379330", "Nombre": "POL MC ESTRE INX COR", "Cantidad": 1, "PrecioRegular": 49.99, "PrecioTarjetaRipley": 49.99, "PrecioAplicado": 49.99 },
      { "Codigo": "2015372278467", "Nombre": "FALDA LARG FESTIDO I", "Cantidad": 1, "PrecioRegular": 89.99, "PrecioTarjetaRipley": 89.99, "PrecioAplicado": 89.99 }
    ],
    "Tarjeta": "TarjetaRipley",
    "FechaLlegada": "2026-11-06T19:45:02.1234567-05:00",
    "TotalRegular": 199.97,
    "Ahorro": 20.00,
    "TotalFinal": 179.97,
    "OperacionGravada": 152.52,
    "Igv": 27.45,
    "Boleta": {
      "Numero": 1,
      "FechaEmision": "2026-11-06T19:47:31.7654321-05:00",
      "IdVenta": "6f1c2a9e-3b4d-4c5e-8f70-1a2b3c4d5e6f",
      "Leyenda": "Boleta simulada - sin validez tributaria"
    }
  }
]
```

`System.Text.Json` escribe cada propiedad en su propia línea. Aquí los artículos se compactaron para que el ejemplo se lea mejor.
Los valores calculados (`SubtotalRegular`, `SubtotalAplicado`, `ClienteTexto`, etc.) llevan `[JsonIgnore]`: no se guardan.

---

## 4. Uso rápido (flujo completo)

El menú superior tiene cinco secciones, en este orden: **Preventa, Venta, Boleta, Bitácora y Red de cajas**. La sección activa se ve en blanco con texto morado; el título de cada pantalla repite el nombre del menú.

1. **Preventa.**
   1. A la izquierda están juntas la búsqueda y el catálogo: busque por código (por ejemplo, `2015373474714`) o por nombre (por ejemplo, `pól mc`), o pulse *Ver catálogo*.
   2. Seleccione el producto, escriba la cantidad y pulse *Agregar al carrito*.
   3. El carrito está a la derecha, en un recuadro aparte: `−`, `+`, *Cambiar*, *Quitar*, *Deshacer* y *Vaciar*. Muestra el total y lo que se pagaría con Tarjeta Ripley.
   4. Si quiere, escriba nombre, DNI y correo en *Cliente (opcional)* (datos inventados) y pulse *Enviar a caja*. El botón se activa solo cuando el carrito tiene productos.
2. **Venta.**
   1. Pulse *Atender siguiente* (el botón indica el turno que entra y se desactiva mientras hay un cliente en caja).
   2. En el recuadro *Pago*, elija *Tarjeta Ripley* u *Otra tarjeta*; puede cambiarla antes de confirmar.
   3. Revise el ahorro y el total a pagar (la operación gravada y el IGV aparecen en letra más pequeña).
   4. Pulse *Confirmar pago*. Si hace falta corregir la compra (por ejemplo, por falta de stock), use *Corregir compra* o *Retirar compra*.
3. **Boleta.**
   - Al confirmar el pago se abre esta sección con la boleta emitida.
   - En *Buscar boleta* se busca por número, por DNI exacto o por fecha, o se pulsa *Ver todas*. La boleta elegida queda marcada en los resultados y su detalle aparece a la derecha.
4. **Bitácora.** El filtro y la lista de registros ocupan la pantalla: todos los subprocesos o solo *Preventa - Selección de productos*, *Venta - Pago* o *Postventa - Boleta*. Al pie se muestra la carpeta de datos.
5. **Red de cajas.** Primero se elige *Origen* y *Destino* y se pulsa *Calcular ruta*. En el recuadro *Enlaces* se activa o desactiva el enlace entre esas dos cajas, se cambia su tiempo o se pulsa *Restaurar red*.

### 4.1 Interfaz y colores

- Los textos de la interfaz no muestran nombres de estructuras ni complejidades; esa información queda en este README y en la bitácora de código.
- En Windows, `Platforms/Windows/App.xaml` define los colores de todos los selectores (ComboBox de WinUI): el de tarjeta (`pkTarjeta`), el filtro de la bitácora y Origen/Destino de la red. Los mismos colores se declaran para tema claro y oscuro, así el desplegable no queda oscuro aunque Windows use tema oscuro. `UI/EstilosNativos.cs` solo pone en tema claro el calendario de *Fecha*.
- Una primera versión cambiaba los colores de cada selector desde código al crearlo y en Windows lanzaba `COMException` (`ComboBoxBackgroundUnfocused`); por eso se pasó a recursos declarados en XAML.
- Contraste calculado según WCAG 2.1 (mínimo 4.5:1 para texto normal):

| Texto / fondo | Uso | Contraste |
|---|---|---|
| Blanco sobre `#4B2A7B` | opción seleccionada del desplegable, botones | 10.96:1 |
| Blanco sobre `#3A1F63` | opción seleccionada con el puntero encima | 13.50:1 |
| Blanco sobre `#6A2FA8` | *Enviar a caja*, *Atender siguiente*, *Confirmar pago*, *Calcular ruta* | 8.12:1 |
| Blanco sobre `#2E1A4D` | menú (secciones no activas) | 15.32:1 |
| `#4B2A7B` sobre blanco | menú (sección activa) | 10.96:1 |
| `#1F1B24` sobre blanco | opciones del desplegable y selector cerrado | 16.93:1 |
| `#1F1B24` sobre `#EEE8F6` | opción con el puntero encima, fila elegida | 14.11:1 |
| `#5C5666` sobre `#F7F3FC` | textos secundarios dentro del carrito y del pago | 6.44:1 |
| `#5C5666` sobre `#E2DFE6` | botones y selectores deshabilitados | 5.35:1 |
| `#1B6E33` sobre blanco | ahorro | 6.32:1 |
| `#B3261E` sobre blanco | mensajes de error | 6.54:1 |

Los valores se calcularon con la fórmula de luminancia relativa; **falta revisarlos a la vista en Windows con tema claro y con tema oscuro**.

---

## 5. Trazabilidad

`n` = prendas del catálogo, `m` = líneas del carrito, `k` = clientes en la fila, `v` = ventas registradas, `L` = largo de los textos comparados.

| Requisito funcional | Archivo / método | Estructura | Costo |
|---|---|---|---|
| Cargar y validar catálogo | `CatalogoService.Cargar`, `JsonDataManager.CargarProductos` | `ArbolBusqueda.Insertar` (AVL) | O(n log n) |
| Buscar prenda por código | `CatalogoService.BuscarPorCodigo` | `ArbolBusqueda.Buscar` | O(log n) |
| Buscar por nombre (sin mayúsculas ni tildes) | `CatalogoService.BuscarPorNombre`, `InputHelper.NombreContiene` | `RecorrerInOrden` + comparación | O(n) prendas × O(L) |
| Listar catálogo ordenado | `CatalogoService.Listar` | Recorrido inorden | O(n) |
| Agregar / aumentar prenda | `CarritoService.AgregarPrenda` | `ListaEnlazada.Buscar` + `AgregarAlFinal`; `PilaAcciones.Apilar` | O(m) buscar; O(1) agregar y apilar |
| Cambiar cantidad, `+` y `−` | `CarritoService.CambiarCantidad` | `ListaEnlazada.Buscar`; `PilaAcciones.Apilar` | O(m) |
| Quitar prenda | `CarritoService.QuitarPrenda` | `ListaEnlazada.Eliminar` (reenlaza un nodo) | O(m) localizar, O(1) reenlazar |
| Deshacer | `CarritoService.Deshacer` | `PilaAcciones.Desapilar`; `InsertarEn` restaura la posición | O(1) desapilar, O(m) revertir |
| Total regular | `CarritoService.CalcularTotalRegular` | Recorrido de la lista | O(m) |
| Enviar compra a la fila | `CompraService.RegistrarLlegada` | `CarritoService.CopiarArticulos`; `ColaCompras.Encolar` | O(m) copia; O(1) encolar |
| Atender siguiente | `CompraService.AtenderSiguiente` | `ColaCompras.Desencolar` | O(1) |
| Mostrar la fila | `CompraService.ObtenerFila` | Recorrido de la cola | O(k) |
| Tarjeta, total final y ahorro | `CompraService.SeleccionarTarjeta`, `CalcularImportes` | Recorrido de artículos | O(m) |
| Corregir compra en atención | `CompraService.CorregirCantidad` / `QuitarArticulo` / `DeshacerCorreccion` | `CarritoService` con contexto propio | O(m) |
| Confirmar pago simulado | `CompraService.ConfirmarPagoSimulado`, `BoletaService.GuardarConNuevaVenta` / `PublicarVenta`, `CatalogoService.HayStockSuficiente` / `GuardarConDescuento` | Árbol (stock), lista e índice | O(m log n + v) |
| Boleta por número | `BoletaService.BuscarPorNumero` | `ArbolBusqueda<int, Venta>` | O(log v) |
| Boletas por DNI / fecha | `BoletaService.BuscarPorDni` / `BuscarPorFecha` | Recorrido de `ListaEnlazada<Venta>` | O(v) |
| Ver todas (recientes primero) | `BoletaService.ListarRecientes` | `AgregarAlInicio` | O(v) |
| Persistencia | `JsonDataManager.Cargar<T>` / `Guardar<T>` | `List<T>` solo como transporte JSON | O(tamaño del archivo) |
| Bitácora por subproceso | `Bitacora` en `CarritoService`, `CompraService` y `BoletaService` | `ListaEnlazada<string>` | O(1) registrar |
| Red de cajas: ruta más corta | `GrafoCajas.BuscarRutaMinima` | Listas de adyacencia + Dijkstra con recorrido simple | O(V² + E) |

**Complejidades de las estructuras:**
- `ListaEnlazada`: agregar al final o al inicio cuesta O(1) gracias a la referencia al último nodo. Buscar, quitar y la búsqueda por nombre o por DNI son recorridos O(n).
- `PilaAcciones` (apilar, desapilar) y `ColaCompras` (encolar, desencolar): O(1).
- Árbol AVL: altura O(log n); insertar y buscar cuestan O(log n).
- Con las 7 prendas insertadas en orden ascendente, el árbol queda con altura 3 y raíz `2015372601050`, y encuentra `2015373474714` en 2 comparaciones (4 rotaciones). Esto se verificó con la réplica; ver la sección 6.

---

## 6. Comprobaciones

### 6.1 Ejecutado en este entorno (réplica en Python)

El entorno donde se escribió el código **no tiene .NET ni Windows**, así que el C# y la interfaz **no se compilaron ni se ejecutaron aquí**.
Para verificar los números se tradujo la lógica no visual a Python línea por línea (`docs/verificacion_python/replica.py`) y se ejecutó
`pruebas_replica.py`. Resultado: **63 correctas, 0 con falla**; la salida completa está en `salida_replica.txt`. La réplica cubre:

- **Árbol:** altura 3, raíz JEANS, 4 rotaciones y 2 comparaciones; la forma coincide con el diagrama del informe.
- **Escenarios A y B:** 199.97 / 179.97 / 20.00 / 152.52 / 27.45, el carrito modificado de 239.97, y 149.96 / 127.08 / 22.88.
- **Búsqueda por nombre:** equivalencias, incluidos los acentos compuestos y separados.
- **Carrito:** deshacer con el orden restaurado y la pila vacía al final.
- **Validaciones:** de enteros, decimales y textos.
- **Cola:** pico de 12 clientes en orden FIFO.
- **Dijkstra:** 26, 34 y sin ruta.

### 6.2 Programa `Comprobaciones` (C#) — por ejecutar en Windows

Compila los mismos archivos de la app (sin la interfaz) y usa carpetas temporales. Ejecútelo desde la carpeta `Comprobaciones` con
`dotnet run`, o márquelo como proyecto de inicio y pulse Ctrl+F5. Muestra `[OK]` o `[FALLA]` por caso y una línea final con la fecha de ejecución,
que sirve como registro fechado. Además de lo de la réplica, prueba:

- reabrir los archivos;
- la coherencia de `IdVenta` en el JSON;
- JSON inexistente, vacío y dañado;
- errores de escritura simulados con archivos de solo lectura, con la compensación;
- falta de stock;
- confirmar dos veces;
- búsqueda por DNI, número y fecha;
- la bitácora.

**Estado: propuesto, pendiente de ejecutar.**

### 6.3 Casos en la interfaz (propuestos, para capturas fechadas)

| ID | Pasos | Resultado esperado |
|---|---|---|
| P-01 | **Escenario A.** Agregar FEPAPAYA, ESTRE y FALDA (1 c/u), *Enviar a caja*, *Atender siguiente*, elegir *Otra tarjeta* y luego cambiar a *Tarjeta Ripley*; confirmar. | Con Otra tarjeta: final S/ 199.97 y ahorro S/ 0.00. Con Tarjeta Ripley: regular S/ 199.97, final S/ 179.97, ahorro S/ 20.00, op. gravada S/ 152.52, IGV S/ 27.45. Boleta N° 000001 visible en la sección Boleta. |
| P-02 | Con FEPAPAYA, ESTRE y FALDA: `+` en FALDA (a 2) y *Quitar* ESTRE. | Total regular S/ 239.97; quedan FEPAPAYA (1) y FALDA (2). |
| P-03 | **Escenario B.** JEANS, HI ML CUERNO, SWE ML DESTAR y SWE ML DETEJ (1 c/u), con cada tarjeta. | Final S/ 149.96, ahorro S/ 0.00, op. gravada S/ 127.08, IGV S/ 22.88 con ambas tarjetas. |
| P-04 | Buscar código `2015373474714`, luego `9999999999999` y luego `20153734`. | Primero: "1 producto encontrado" (en `Comprobaciones` se verifica que son 2 comparaciones en el árbol). Segundo: "Prenda no encontrada". Tercero: "Código inválido… tiene 8 caracteres" y botón deshabilitado. |
| P-05 | Buscar nombre `pol mc`; luego `POL MC FEPAPAYA INX`, `Pol Mc Fepapaya Inx` y `Pól MC Fepápaya Inx`; luego solo espacios; luego `zapatilla`. | `pol mc`: 2 prendas para elegir. Las tres variantes: la misma prenda. Solo espacios: botón deshabilitado, sin listar todo. `zapatilla`: "Prenda no encontrada.". |
| P-06 | Equivalencia "Polo Rojo" con un catálogo ficticio (no se agrega al base): se ejecuta en `Comprobaciones`. | La misma lista y el mismo orden para todas las variantes; el nombre guardado no cambia. |
| P-07 | Agregar la misma prenda dos veces; cambiar cantidad; quitar; *Deshacer* hasta vaciar la pila. | Una sola línea con cantidad 2. Cada *Deshacer* revierte el último cambio y la prenda quitada vuelve a su posición. Con la pila vacía, *Deshacer* queda deshabilitado. |
| P-08 | Cantidad `0`, `-1`, `1.5`, `100`, `abc` y `25` (mayor que el stock de 20). | Mensaje bajo el campo y botón deshabilitado; con 25, "Stock insuficiente". El carrito y la pila no cambian. |
| P-09 | Registrar 12 compras y atenderlas. | Turnos 1 a 12 en orden. Cada carrito nuevo empieza con la pila en 0. |
| P-10 | Con un turno en atención, intentar *Atender siguiente*; doble clic rápido en *Confirmar pago*. | *Atender siguiente* deshabilitado mientras hay un cliente en caja (el servicio además rechaza con "Ya se atiende el turno…"). Se emite una sola boleta. |
| P-11 | DNI `1234`; correo `abc`; DNI `70000021` con correo `cliente.prueba@correo.com`; todo vacío. | "DNI inválido"; "Correo inválido"; envío habilitado; compra anónima con la boleta sin DNI. |
| P-12 | Dos compras con DNI `70000021` y una con `70000099`; buscar ambos DNI. | `70000021` devuelve 2 boletas; `70000099` devuelve 1; ninguna ajena. |
| P-13 | Buscar boleta `1` y `99`; filtrar por la fecha de hoy y por otra fecha. | Boleta 1 encontrada; para la 99, "No existe la boleta N° 99"; hoy lista las boletas; otra fecha: "No hay boletas emitidas…". |
| P-14 | Cerrar y abrir la app. | Mismas boletas, fechas, tarjetas, precios aplicados y ahorros; stock descontado; siguiente número = máximo + 1. |
| P-15 | Con la app cerrada, dejar `ventas.json` vacío o dañado y abrir; luego restaurar el archivo. | Mensaje rojo y el archivo sin modificar; no se registran ventas hasta corregirlo. |
| P-16 | Compra 1: JEANS x15; compra 2: JEANS x10; confirmar la 1 y luego la 2. | La 2 muestra "Falta stock… pide 10 y hay 5". Al corregir a 5 se confirma y el stock queda en 0. |
| P-17 | Abrir *Bitácora* y cambiar el filtro. | Registros con fecha y hora, acción, turno o carrito y resultado, separados por los tres subprocesos. |
| P-18 | *Red de cajas*: ruta Caja 511 → Central; desactivar Servidor–Central; desactivar Almacén–Central; tiempo `abc` o `0`. | "Tiempo total: 26 ms" por 512, 513 y Servidor; luego 34 ms por el Almacén; luego "No hay una ruta disponible…"; el tiempo se rechaza con mensaje. |
| P-19 | Con Windows en tema claro y luego en tema oscuro: en *Venta*, atender un turno, abrir *Selecciona una tarjeta* y elegir cada opción. | *Tarjeta Ripley* y *Otra tarjeta* se leen en el selector cerrado y en el desplegable; la opción elegida aparece en morado con texto blanco. |

Estado de P-01 a P-19: **propuestos**. No hay capturas ni registros de ejecución en Windows; deben generarse al probar.

---

## 7. Decisiones y contradicciones resueltas

1. **Orden fila / prendas.** El TO-BE del informe registra las prendas después de la fila. Aquí se siguió la instrucción vigente: el cliente elige en Preventa y la compra completa entra a la fila.
2. **DNI oculto.** El informe (CP-16) guarda solo el DNI oculto. Para permitir la búsqueda exacta, aquí se guarda completo como `string` (con datos ficticios) y se oculta solo en mensajes de consulta (`7xxxxx21`).
3. **Archivo de boletas.** El informe usa `boletas.json` con una Boleta que copia los importes. Aquí se usa `ventas.json`, donde cada `Venta` contiene su `Boleta` y la `Boleta` guarda solo sus datos propios más `IdVenta`.
4. **Estados.** "En fila / En caja / Finalizada" pasan a `EnFila`, `EnAtencion` y `Finalizada`.
5. **IGV.** Se usa `OperacionGravada = redondear(TotalFinal / 1.18, 2)` e `IGV = TotalFinal − OperacionGravada` (redondeo `AwayFromZero`). Da los mismos 27.45 y 22.88 del informe.
6. **Datos del cliente.** El informe los pide en Venta (`RegistrarDatosCliente`). Aquí se piden al enviar la compra a la fila, como indica la instrucción.
7. **Precio por tarjeta.** El diagrama de clases pone `ItemCarrito.PrecioAplicable(tarjeta)`. La política queda solo en `CompraService`; `ItemCarrito` guarda `PrecioAplicado` como valor histórico.
8. **Restos del proyecto bancario.** La Tabla 12 y el Anexo D del informe (ListaDoble, ListaCircular, ColaOperaciones, PilaReversiones, GrafoAgencias, ProcesadorOperaciones/AlmacenDatos, repositorio CAJA_AREQUIPA) no corresponden a este código. No se afirman pruebas de esos módulos.
9. **Listas dobles y circulares.** El desafío las menciona solo como temas de las sesiones (II.C), no como parte obligatoria del código, así que no se incluyeron.
10. **Grafo.** El desafío sí lo exige; por eso se agregó `Structures/GrafoCajas.cs`, solo en memoria y sin intervenir en el pago.
11. **Archivos de plantilla.** Se quitó `AppShell` (la imagen no lo incluye): `App.CreateWindow` abre la única `MainPage`, que está en `UI/`. Se omitió `Categoria` del informe porque el modelo pedido tiene cinco campos.
12. **Lectura y escritura.** La lectura y escritura de archivos es sincrónica porque los archivos son pequeños; `async` se usa para leer el asset empaquetado.

---

## 8. Limitaciones

- **Compilación:** falta verificar la compilación y la interfaz en Windows con Visual Studio 2026 (ver la sección 6).
- **Estado de la sesión:** los turnos se reinician en cada sesión. La fila, la pila de deshacer y la bitácora no se guardan (por diseño).
- **Stock:** no se reserva al encolar; se valida al agregar y otra vez al confirmar. Si otra venta lo consumió, la compra en atención se corrige o se retira (*Retirar compra*).
- **Dos archivos:** la confirmación usa la compensación simple descrita en la sección 3, no transacciones.
- **Búsquedas lineales:** por nombre, DNI y fecha son recorridos O(n), suficientes para el volumen del caso.
- **Alcance:** una sola caja y una sola ventana, sin concurrencia.
- **Red de cajas:** usa cajas y tiempos ficticios, solo en memoria.

---

## 9. Archivos propios y archivos de la plantilla

| Tipo | Archivos |
|---|---|
| Propios del proyecto | `Models/*`, `Structures/*`, `Services/*`, `Data/JsonDataManager.cs`, `Helpers/InputHelper.cs`, `UI/MainPage.xaml(.cs)`, `Resources/Raw/productos.json`, `Comprobaciones/*` |
| De la plantilla, modificados | `RipleyComprasMAUI.csproj` (solo Windows, sin imágenes de ejemplo, color del ícono), `App.xaml` (sin AppShell), `App.xaml.cs` (abre `MainPage` desde el contenedor), `MauiProgram.cs` (servicios y JSON), `Resources/Styles/Colors.xaml` y `Styles.xaml` (paleta propia, más breves) |
| De la plantilla, sin cambios de contenido | `Platforms/Windows/*` (solo el espacio de nombres), `Properties/launchSettings.json`, `Resources/Fonts/OpenSans-*.ttf`, `Resources/AppIcon/*.svg`, `Resources/Splash/splash.svg` |
| De la plantilla, retirados | `AppShell.xaml(.cs)`, `MainPage.xaml(.cs)` de la raíz, `Resources/Images/dotnet_bot.png`, `Resources/Raw/AboutAssets.txt` y las carpetas de Android, iOS, Mac Catalyst y Tizen |

Antes de entregar al aula virtual puede borrar `docs/verificacion_python/`, que es solo el respaldo de la verificación hecha sin .NET.
`Comprobaciones/` es opcional. Al comprimir, excluya `bin/`, `obj/` y `.vs/`.
