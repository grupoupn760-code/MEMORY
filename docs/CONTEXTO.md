# RipleyComprasMAUI — contexto del proyecto

Repositorio de memoria: https://github.com/grupoupn760-code/MEMORY · Repositorio de entrega (Anexo F del informe): https://github.com/grupoupn760-code/RIPLEY_ (versión .NET 9, conectado con permiso de escritura desde el 25/11/2026) · Actualizado: 25/11/2026

## Objetivo
Prototipo académico (Estructuras de Datos, UPN 2026-2, Grupo 07) que simula la caja 511 de Ripley en tres subprocesos: **Preventa** (catálogo y carrito), **Venta** (fila FIFO, tarjeta, pago simulado) y **Postventa** (boleta simulada y consultas). Estructuras propias y persistencia en JSON.

## Estructura (rutas desde la raíz del repo)
- `CLAUDE.md` (reglas), `docs/CONTEXTO.md` (este archivo), `docs/HISTORIAL.md` (una línea por sesión).
- `RipleyComprasMAUI/`: solución .NET 10 (`.slnx`/`.sln`) con la app y la consola `Comprobaciones`; `README.md` y `CODIGO_FUENTE.md`. `variantes/net9/`: los dos `.csproj` para .NET 9.
- App: `Models/` (Producto, ItemCarrito, Venta con su Boleta, Boleta), `Structures/` (ListaEnlazada, PilaAcciones, ColaCompras, ArbolBusqueda AVL, GrafoCajas), `Services/` (Catalogo, Carrito, Compra, Boleta), `Data/JsonDataManager.cs`, `Helpers/InputHelper.cs`, `UI/MainPage.xaml(.cs)` (Preventa, Venta, Boleta, Bitácora, Red de cajas), `Platforms/Windows/App.xaml` (colores de los selectores).
- `docs/REVISION_INFORME.md` (revisión del 16/11) y `docs/RESULTADOS_PRUEBAS.md` (resultados de CP-01 a CP-19).
- `docs/informe/`: Word corregido del 17/11, `Capitulo_II_corregido.docx` (25/11, figuras en `cap2/`), `CAPITULO_VII_IA.md` y `DECLARACION_IA.md` (fuentes de los Anexos E y D del informe final).
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
1. Informe: Comprobaciones ya se ejecutó (25/11/2026 23:15:30): 100 correctas, 0 con falla. Falta que el equipo agregue la fila de la Tabla 13, el CP-20 en la Tabla 14 y cambie “CP-01 a CP-19” por “CP-01 a CP-20” (Tabla 15 y Anexo E).
2. Sustentación: subir el PPT al aula virtual (no hay conector), confirmar expositores y, si se quiere, copiar el PC1–PC6 del informe a las diapositivas 11–12 de la v2.
3. Opcional en el informe (ver observaciones abajo).
4. Opcional en código: desplegable de tarjeta en morado (hoy oscuro, legible) y “1 xS/” en Venta.

## Problemas abiertos / observaciones del informe final
- Concordancia informe ↔ código RIPLEY_ (versión _2 del 25/11, 42 págs.): **alineado**. Corregidos plataforma (.NET 9 STS), PC1, PC2, PC5, 4.3, 5.2, Tablas 1.4, 8 y 9, CP-16 y la bitácora fila 6 (REF 3135 a 3137). Solo queda, opcional, que toda la bitácora y la Figura 5 usan 22/08/2026 mientras la Tabla 1.4 y las fotos son del 16/11/2026. CP-15 muestra DNI y correo de otra persona (decisión del equipo).
- Ya resueltos en la versión _2: S/ 131.18 y QR quitados de la Tabla 1.4; REF 3135–3137 en Tabla 1.4, Tabla 9 y bitácora fila 6; CP-16 con 8xxxxx56. La Imagen 6 es la foto del 16/11/2026 (REF 3137 21:03 S/ 510.85 débito, 3136 S/ 650.86 crédito, 3135 débito).
- Capítulo II (25/11): debe describir el negocio, no la app, y cada tabla E-P-S debe calzar con su diagrama de flujo. En la versión _2 no calzaban (Tabla 6 con códigos y pila frente a la Figura 2; Tablas 7 y 8 y Figura 1 con AVL, cola y ventas.json; Figura 3 con S/ 520.85; Figura 4 con REF 2101–2103, S/ 1080.13 y 4 prendas). Se entregó `docs/informe/Capitulo_II_corregido.docx` (2.1, Figura 1 de negocio, Tablas 6–8, Figuras 3 y 4 con datos actuales; figuras en `docs/informe/cap2/`, incluida la Figura 2 redibujada en el mismo estilo). Falta que el equipo lo pase al informe. Tabla 1.4 rehecha en `docs/informe/Tabla_1_4_corregida.docx`: fila de Preventa (8 prendas elegidas en el piso, Imágenes 1 y 3), registro en caja en Venta, REF en Postventa, boleta con IGV S/ 77.93. Ojo: en la bitácora las filas 2 y 3 (DNI y carrito en el terminal) están bajo Pre-Venta, pero en la Figura 3 ocurren en caja (Venta).
- Menores que siguen: el párrafo sobre acoplamiento aparece dos veces (4.2 y junto a la Figura 5); CP-09 muestra la fila de 12, no la atención completa; el carrito A (ahorro S/ 20.00) es un caso construido, sin foto de campo.
- RIPLEY_ (25/11): Structures/Services/Models perdieron todos los comentarios respecto de MEMORY, y `InputHelper.NormalizarParaBusqueda` perdió un `continue;` (duplica espacios). La boleta imprime DNI y correo completos; el `catch` de `EstilosNativos` quedó vacío. Lista de correcciones C-01 a C-05 del equipo: todas confirmadas; C-01, C-04 y C-05 hechas y subidas a RIPLEY_ main (commits 5d86f55 y f35fb49); C-02 y C-03 resueltos en el informe (sin cambio de código). Falta compilar y repetir CP-01/CP-02 tras C-04.
