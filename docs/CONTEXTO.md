# RipleyComprasMAUI — contexto del proyecto

Repositorio de memoria: https://github.com/grupoupn760-code/MEMORY · Repositorio de entrega (Anexo F del informe): https://github.com/grupoupn760-code/RIPLEY_ (versión .NET 9, conectado con permiso de escritura desde el 25/11/2026) · Actualizado: 25/11/2026

## Objetivo
Prototipo académico (Estructuras de Datos, UPN 2026-2, Grupo 07) que simula la caja 511 de Ripley en tres subprocesos: **Preventa** (catálogo y carrito), **Venta** (fila FIFO, tarjeta, pago simulado) y **Postventa** (boleta simulada y consultas). Estructuras propias y persistencia en JSON.

## Estructura (rutas desde la raíz del repo)
- `CLAUDE.md` (reglas), `docs/CONTEXTO.md` (este archivo), `docs/HISTORIAL.md` (una línea por sesión).
- `RipleyComprasMAUI/`: solución .NET 10 (`.slnx`/`.sln`) con la app y la consola `Comprobaciones`; `README.md` y `CODIGO_FUENTE.md`. `variantes/net9/`: los dos `.csproj` para .NET 9.
- App: `Models/` (Producto, ItemCarrito, Venta con su Boleta, Boleta), `Structures/` (ListaEnlazada, PilaAcciones, ColaCompras, ArbolBusqueda AVL, GrafoCajas), `Services/` (Catalogo, Carrito, Compra, Boleta), `Data/JsonDataManager.cs`, `Helpers/InputHelper.cs`, `UI/MainPage.xaml(.cs)` (Preventa, Venta, Boleta, Bitácora, Red de cajas), `Platforms/Windows/App.xaml` (colores de los selectores).
- `docs/REVISION_INFORME.md` (revisión del 16/11) y `docs/RESULTADOS_PRUEBAS.md` (resultados de CP-01 a CP-19).
- `docs/informe/`: Word corregido del 17/11, `CAPITULO_VII_IA.md` y `DECLARACION_IA.md` (fuentes de los Anexos E y D del informe final).
- `docs/presentacion/`: `Sustentacion_RipleyComprasMAUI.pptx` (15 diap.) y `_v2.pptx` (15 diap. con la estructura del equipo: bitácora en 3 tablas, diagramas de negocio, pseudocódigo PC1–PC6, casos y conclusiones).

## Tecnologías
C#, .NET MAUI solo Windows (unpackaged), Visual Studio 2026, .NET 10 (variante .NET 9), System.Text.Json. Sin SQL, EF, Blazor, WPF ni WinForms.

## Decisiones clave
- Estructuras propias en la lógica (sin List, Queue, Stack); `List` solo para JSON y pantalla.
- `decimal`, redondeo `AwayFromZero`; op. gravada = redondear(total/1.18, 2) e IGV = total − op. gravada.
- `ventas.json` guarda cada Venta con su Boleta (la Boleta solo lleva IdVenta). DNI guardado completo para buscarlo; oculto como 6xxxxx00 en caja y consultas.
- Solo se guarda lo confirmado (escritura atómica al *Confirmar pago*); carrito, fila y bitácora viven en memoria.
- Stock validado al agregar y al confirmar. Code-behind sin lógica de negocio.
- Colores de los selectores en XAML: cambiarlos desde código daba `COMException`.

## Estado actual (25/11/2026)
- **Informe final entregado por el equipo** (`EDD_EF_Grupo07.pdf`, 43 págs.): título “el rediseño transaccional de caja 511 de Ripley”; Capítulos I–VI, pseudocódigo PC1–PC6 en 4.5, Anexo D (declaración de IA en formato UPN, herramienta Claude Opus 5.5), Anexo E (evaluación de impacto y uso responsable de IA) y Anexo F (repositorio `RIPLEY_`).
- **Casos de prueba:** el equipo marca los 19 como **Validado**. Nuevas boletas: 000005 (CP-10, Otra tarjeta, S/ 239.97). Detalle y observaciones en `docs/RESULTADOS_PRUEBAS.md`.
- **App:** corre en Windows con la interfaz nueva (.NET 10). La variante .NET 9 no se probó.
- **Presentación:** v2 lista; el pseudocódigo de las diapositivas se redactó desde el código y difiere en forma del PC1–PC6 del informe.

## Pendientes (por prioridad)
1. Sustentación: subir el PPT al aula virtual (no hay conector), confirmar expositores y, si se quiere, copiar el PC1–PC6 del informe a las diapositivas 11–12 de la v2.
2. Opcional en el informe (ver observaciones abajo).
3. Opcional en código: desplegable de tarjeta en morado (hoy oscuro, legible) y “1 xS/” en Venta.

## Problemas abiertos / observaciones del informe final
- Concordancia informe final ↔ código RIPLEY_ (25/11): estructuras, complejidades, PC3/PC4/PC6, cálculos, validaciones, persistencia, Tabla 13 y casos coinciden. No coinciden: plataforma (.NET 10 en el informe, RIPLEY_ es .NET 9); texto de 4.3 (dice que el cajero registra artículos); 5.2 con nombres traducidos (Ticket, Libro de registro, Red de registros, Enviar a registro); CP-16 “6******0”; Tabla 8 “No se encontró la boleta”; PC5 “No existe una ruta…”; PC1/PC2 “devolver nulo” (el código lanza excepción); ejemplo de código 2005320106904 que no está en productos.json.
- Bitácora y Tabla 1.4 mezclan datos: la fila 6 dice voucher de S/ 510.85 (la foto original era S/ 1080.13); la fila 7 dice 22/08 19:40 y la Tabla 1.4 dice boleta del 16/11/2026 con 8 prendas y S/ 510.85; “Operaciones con QR: 3 (S/ 131.18…)” sigue sin respaldo en la Imagen 6.
- 1.5: las filas 2 y 3 repiten el mismo texto observado; 4.2 repite el párrafo sobre acoplamiento; 4.3 dice que el cajero registra los artículos (en el código se registran en Preventa).
- CP-16 usa un DNI que parece real y dice “6******0” (la app muestra 6xxxxx00). CP-09: lo obtenido muestra la fila de 12, no la atención completa.
- El carrito A (ahorro S/ 20.00) es un caso construido, sin foto de campo.
- RIPLEY_ (25/11): Structures/Services/Models perdieron todos los comentarios respecto de MEMORY, y `InputHelper.NormalizarParaBusqueda` perdió un `continue;` (duplica espacios). La boleta imprime DNI y correo completos; el `catch` de `EstilosNativos` quedó vacío. Lista de correcciones C-01 a C-05 del equipo: todas confirmadas; C-01, C-04 y C-05 hechas y subidas a RIPLEY_ main (commits 5d86f55 y f35fb49); C-02 y C-03 por decidir. Falta compilar y repetir CP-01/CP-02 tras C-04.
