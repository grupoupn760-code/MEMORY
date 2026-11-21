# RipleyComprasMAUI — contexto del proyecto

Repositorio: https://github.com/grupoupn760-code/MEMORY · Actualizado: 21/11/2026

## Objetivo
Prototipo académico (Estructuras de Datos CIIN1190P, UPN 2026-2) que simula una caja de Ripley en tres subprocesos: **Preventa** (carrito), **Venta** (fila FIFO, tarjeta, pago simulado) y **Postventa** (boleta simulada y consultas). Se usan estructuras de datos manuales y la persistencia es en JSON.

## Estructura (rutas desde la raíz del repo; la solución está en `RipleyComprasMAUI/`)
- `CLAUDE.md`: reglas de trabajo (leer este archivo al empezar y actualizarlo al terminar).
- `docs/CONTEXTO.md` (este archivo) y `docs/HISTORIAL.md` (una línea fechada por sesión).
- `docs/REVISION_INFORME.md`: revisión del informe `EDD_EF_Grupo07.pdf` frente al código (16/11/2026).
- `docs/informe/EDD_EF_Grupo07_corregido.docx`: informe corregido en Word (17/11/2026).
- `docs/RESULTADOS_PRUEBAS.md`: resultado obtenido y estado de CP-01 a CP-19 según las capturas del equipo (20/11/2026).
- `RipleyComprasMAUI/RipleyComprasMAUI.slnx` / `.sln`: la solución tiene 2 proyectos (versión .NET 10).
- `variantes/net9/`: los dos `.csproj` para .NET 9; para cambiar de versión, reemplaza los de la solución por estos. Todo lo demás es idéntico.
- `RipleyComprasMAUI/RipleyComprasMAUI/Models/`: `Producto`, `ItemCarrito`, `Venta` (contiene su `Boleta`) y `Boleta`.
- `RipleyComprasMAUI/RipleyComprasMAUI/Structures/`: `NodoSimple`, `ListaEnlazada`, `PilaAcciones` (deshacer), `ColaCompras` (fila), `NodoArbol`, `ArbolBusqueda` (AVL) y `GrafoCajas` (Dijkstra, complemento).
- `RipleyComprasMAUI/RipleyComprasMAUI/Services/`: `CatalogoService` (búsquedas), `CarritoService`, `CompraService` (fila, tarjeta, IGV, pago) y `BoletaService` (índice y consultas).
- `RipleyComprasMAUI/RipleyComprasMAUI/Data/JsonDataManager.cs`: lectura y escritura de `productos.json` y `ventas.json` en `AppDataDirectory/data`.
- `RipleyComprasMAUI/RipleyComprasMAUI/Helpers/InputHelper.cs`: validaciones y formato (S/, fechas, DNI oculto).
- `RipleyComprasMAUI/RipleyComprasMAUI/UI/MainPage.xaml(.cs)`: única pantalla, con las secciones Preventa, Venta, Boleta, Bitácora y Red de cajas.
- `RipleyComprasMAUI/RipleyComprasMAUI/UI/EstilosNativos.cs`: solo pone en tema claro el calendario del DatePicker en Windows.
- `RipleyComprasMAUI/RipleyComprasMAUI/Platforms/Windows/App.xaml`: colores de los ComboBox (selectores) en tema claro y oscuro.
- `RipleyComprasMAUI/RipleyComprasMAUI/Resources/Styles/Colors.xaml`, `Styles.xaml`: paleta morada y estilos (`BotonDestacado`, `BotonMenu`, `TarjetaResaltada`, `FilaLista`…).
- `RipleyComprasMAUI/RipleyComprasMAUI/Resources/Raw/productos.json`: semilla con 7 prendas y stock ficticio de 20.
- `RipleyComprasMAUI/Comprobaciones/Program.cs`: consola opcional que prueba la lógica sin la interfaz.
- `RipleyComprasMAUI/docs/verificacion_python/`: réplica de la lógica en Python (`pruebas_replica.py`).
- `RipleyComprasMAUI/README.md` (uso, trazabilidad, casos P-01 a P-19, contraste) y `CODIGO_FUENTE.md` (todo el código).

## Tecnologías
- C#, .NET MAUI, solo Windows (unpackaged), Visual Studio 2026.
- Dos variantes de destino: **.NET 10** (`net10.0-windows10.0.19041.0`, Logging.Debug 10.0.0) y **.NET 9** (`net9.0-…`, 9.0.5).
- `System.Text.Json`, sin SQL, EF, Blazor, WPF ni WinForms.

## Decisiones clave
- No se usan colecciones de .NET (List, Queue, Stack…) en la lógica: el curso exige estructuras propias. `List` solo se usa para JSON y para mostrar datos.
- `decimal` para dinero y redondeo `AwayFromZero`. IGV = Total − redondear(Total/1.18).
- El DNI se guarda como `string` completo, para poder buscarlo exacto, y se oculta en los mensajes. Los datos de clientes son inventados.
- `ventas.json` guarda cada `Venta` con su `Boleta`, y la `Boleta` lleva solo `IdVenta`, para no duplicar datos.
- El stock se valida al agregar y otra vez al confirmar. Si falla un archivo, se compensa restaurando `ventas.json`.
- Solo se guarda lo confirmado: `ventas.json` y `productos.json` se escriben al pulsar *Confirmar pago* (escritura atómica). Carrito, fila, compra en caja y bitácora viven en memoria y se pierden al cerrar, por diseño.
- Code-behind sin lógica de negocio; `App.CreateWindow` resuelve `MainPage` por DI (antes fallaba por recursos no cargados).
- Interfaz: textos sin jerga académica y menú sin números. Los colores de los ComboBox se definen en XAML porque cambiarlos desde código provocaba una `COMException`.

## Estado actual
- **Funciona (verificado):** la réplica en Python pasa 63/63, ejecutada el 07/11/2026. XAML bien formado, handlers y `x:Name` coinciden, llaves balanceadas.
- **.NET 10:** compila y arranca en la PC del usuario. La primera versión de `EstilosNativos` lanzaba una `COMException`; ya está corregida.
- **Repositorio:** el código y la documentación están subidos; desde el 16/11/2026 la memoria vive en `docs/` (antes `RipleyContext.md` en la raíz).
- **Sin confirmar:** que la corrección funcione en Windows, que ambas variantes compilen sin advertencias y cómo se ve la nueva interfaz.
- **Casos CP-01 a CP-19 (ejecutados por el equipo, capturas en `EDD_EF_Grupo07_1.pdf`):** 10 validados, 5 parciales (CP-01, 09, 12, 17, 18) y 4 pendientes de captura (CP-02, 08, 10, 13). `Comprobaciones` sin captura.
- **Informe:** corregido en Word (29 págs.) y alineado con el código: título, alcance, bitácora renumerada (Imágenes 1–7), tablas y figuras numeradas, diagramas TO-BE, estados, clases y secuencia rehechos, grafo agregado, CP-01 a CP-19 y Anexos B y D sin restos de la billetera. Foto de la boleta difuminada. Quedan notas amarillas para el equipo.

## Pendientes (por prioridad)
1. Compilar y ejecutar con la corrección (Limpiar → Recompilar → F5) en .NET 10 y luego en .NET 9.
2. P-19: revisar el contraste de `pkTarjeta` en tema claro y oscuro de Windows.
3. Completar las capturas faltantes según `docs/RESULTADOS_PRUEBAS.md` y mover las evidencias cruzadas (CP-02→CP-04, CP-14→CP-15).
4. Completar las notas amarillas del Word: foto del carrito con ahorro S/ 20.00 (o declararlo caso construido), horas de la bitácora frente al reloj del terminal, capturas de la app, fecha de ejecución de pruebas y enlace del repositorio que se entrega. Actualizar el índice en Word.

## Problemas abiertos
- `COMException` en `EstilosNativos.Pintar` (`ComboBoxBackgroundUnfocused`): corregida, falta confirmarla.
- El carrito A (S/ 199.97, ahorro S/ 20.00) no tiene foto de campo; las horas de la bitácora no coinciden con el reloj del terminal.
- Las capturas de pruebas usan nombres, DNI y correos reales: difuminar o repetir con datos inventados.
- En la app del usuario el desplegable de tarjeta sale oscuro (estilo de Windows): los colores de `Platforms/Windows/App.xaml` no se aplicaron. En Venta los artículos muestran “1 xS/ …” (falta un espacio).
- Ni el código C# ni la interfaz se compilaron fuera de la PC del usuario; puede haber advertencias de .NET 10 (por ejemplo, `DatePicker.Date` ahora es `DateTime?`, aunque el código ya lo contempla).
