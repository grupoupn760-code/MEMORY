# CAPÍTULO VII

## Evaluación del impacto y uso responsable de herramientas digitales e IA

### 7.1. Herramientas utilizadas

Para el análisis, la programación y la documentación del prototipo usamos herramientas digitales y un asistente de inteligencia artificial. La IA se usó como apoyo. Las decisiones del diseño y la validación final quedaron a cargo del equipo.

**Tabla 18. Herramientas digitales e IA utilizadas**

| Herramienta | Para qué la usamos | Control que aplicamos |
|---|---|---|
| Claude (asistente de IA de Anthropic) | Proponer la estructura del código (modelos, estructuras, servicios y persistencia), rediseñar la interfaz, explicar y corregir errores de compilación y ejecución, revisar la coherencia del informe con el código, redactar el resultado obtenido de los casos de prueba y armar la presentación de la sustentación. | Cada propuesta se compiló y ejecutó en Visual Studio. Los resultados se contrastaron con las capturas de la aplicación y los cálculos se comprobaron a mano. |
| Visual Studio 2026 y .NET MAUI | Compilar, depurar y ejecutar el prototipo en Windows. | Ejecución real de los casos de prueba CP-01 a CP-19. |
| GitHub (repositorio MEMORY) | Guardar el código, la documentación y la memoria del proyecto (`CLAUDE.md`, `docs/CONTEXTO.md`, `docs/HISTORIAL.md`). | Historial de cambios con fecha; cada sesión de trabajo deja registrado qué se cambió y por qué. |
| Réplica de la lógica en Python | Comprobar los cálculos y las reglas del prototipo sin depender de la interfaz. | 63 de 63 comprobaciones superadas (07/11/2026). |
| Herramientas de diagramación y edición de imágenes | Rehacer los diagramas TO-BE, de estados, de clases y de secuencia, y difuminar los datos personales de las fotos. | Cada diagrama se revisó contra el código; cada foto, contra la bitácora. |

### 7.2. Uso estratégico de la IA por etapa

- **Análisis.** La IA ayudó a ordenar las observaciones de campo en entradas, procesos y salidas, y a revisar que cada tabla y figura del informe estuviera respaldada por una imagen o un dato registrado. Así detectamos incoherencias: restos de otro proyecto, imágenes mal numeradas y montos que no coincidían con las fotos.
- **Programación.** La usamos para generar una primera versión de las estructuras propias (lista enlazada, pila, cola, árbol AVL y grafo) y de los servicios. Siempre con estas restricciones del curso:
  - estructuras implementadas a mano, sin `List`, `Queue` ni `Stack` de .NET en la lógica;
  - `decimal` para los montos;
  - persistencia solo en JSON.

  El equipo revisó el código, lo compiló y lo probó.
- **Interfaz.** Le describimos a la IA cómo debía sentirse la aplicación ("como una caja de tienda, sin tener que explicar qué botón tocar"). Con eso propuso el orden de las pantallas, los textos y los colores, incluido el cálculo del contraste de los colores según WCAG 2.1.
- **Continuidad del trabajo.** Para no depender de la memoria de una sola conversación, guardamos el contexto del proyecto en archivos del repositorio y se los dimos a la IA al iniciar cada sesión. Así se mantienen las decisiones tomadas y el estado de las pruebas.
- **Pruebas y sustentación.** La IA revisó cada captura contra la entrada y el resultado esperado del caso. Así detectamos capturas cruzadas o incompletas antes de marcar un caso como "Validado". También ayudó a resumir el trabajo en la presentación: diagramas, pseudocódigo PC1 a PC6 y casos representativos. El equipo revisó y ajustó cada diapositiva.
- **Reflexión.** La IA ayudó a contrastar lo observado en tienda con lo que hace el prototipo y a identificar los riesgos de privacidad que se describen en 7.4.

### 7.3. Verificación y control de errores

La IA se equivocó varias veces. Por eso nada de lo que produjo se dio por correcto sin comprobarlo. La Tabla 19 resume los errores detectados y cómo se resolvieron.

**Tabla 19. Errores de la IA detectados por el equipo**

| Error | Cómo lo detectamos | Corrección |
|---|---|---|
| El código usaba un método `Redondear` que no existía (error CS0103). | Al compilar en Visual Studio. | Se agregó el método con una sola política de redondeo: 2 decimales, alejándose del cero. |
| El cambio de colores del selector de tarjeta lanzaba una `COMException` al abrir la aplicación. | Al ejecutar en Windows. | Los colores se pasaron a recursos declarados en XAML. Las capturas del 17 al 21/11/2026 muestran que la aplicación abre y el selector funciona. |
| En la revisión del informe, la IA afirmó que la foto del ingreso de datos mostraba datos personales. | Revisando la foto original: los campos estaban vacíos. | Se corrigió la revisión y solo se difuminó la foto de la boleta. |
| Una captura asignada a un caso de prueba no correspondía a la entrada del caso (por ejemplo, CP-02 mostraba otro carrito). | Comparando cada captura con la entrada y el resultado esperado. | Un caso solo se marca "Validado" si la captura muestra el resultado esperado; los demás quedan como parciales o pendientes. |

También comprobamos a mano los importes de cada caso. Por ejemplo, para un total de S/ 179.97: operación gravada = 179.97 / 1.18 = 152.52 e IGV = 179.97 − 152.52 = 27.45.

### 7.4. Uso ético: privacidad, integridad académica y transparencia

- **Datos personales.** Los clientes del prototipo son inventados, por ejemplo el DNI 70000021 y el correo cliente.prueba@correo.com. En la foto de la boleta real (Imagen 7) se difuminaron el nombre, el DNI, el correo, la tarjeta y el código QR. El prototipo oculta el DNI en caja y en las consultas (7xxxxx21). [Completar: confirmar que en las capturas de los casos de prueba se difuminaron los nombres, DNI y correos reales o que se repitieron con datos inventados.]
- **Integridad académica.** Declaramos aquí el uso de IA. Todos los integrantes revisamos el código y podemos explicar cómo funciona cada estructura, por qué se eligió y cuál es su complejidad.
- **Evidencia honesta.** No se presentan como ejecutadas pruebas que no se ejecutaron:
  - "Validado" solo aparece cuando hay una captura que lo respalda;
  - la réplica en Python se reporta como lo que es, una verificación de la lógica y no de la aplicación;
  - lo que no tiene evidencia de campo se declara como caso construido por el equipo.
- **Pago simulado.** El prototipo no se conecta con bancos, Niubiz ni SUNAT, y la boleta indica "Boleta simulada - sin validez tributaria". Así nadie puede confundirla con un comprobante real.

### 7.5. Impacto social y técnico de la solución

**Impacto técnico**
- **Fila:** la cola atiende las compras en orden de llegada en O(1). Con el pico observado de 12 clientes, un arreglo con desplazamientos haría 66 movimientos para atenderlos a todos; la cola, ninguno.
- **Búsqueda:** el árbol AVL encuentra una prenda entre 7 con a lo más 3 comparaciones y mantiene esa eficiencia, O(log n), aunque el catálogo crezca.
- **Carrito:** la lista enlazada y la pila de deshacer permiten corregir el carrito sin rehacer la compra.
- **Datos:** la escritura atómica de los archivos JSON evita perder ventas confirmadas si la aplicación se cierra.

**Impacto social**
- **Transparencia para el cliente:** el prototipo muestra el ahorro con Tarjeta Ripley antes de pagar y lo registra en la boleta, así el cliente sabe cuánto paga y por qué.
- **Atención justa:** la fila respeta estrictamente el orden de llegada.
- **Riesgos que reconocemos:**
  - el uso del DNI y del correo exige cuidar la privacidad del cliente;
  - una caja que depende de medios digitales (QR, tarjeta) puede dejar fuera a quienes no los usan;
  - en un sistema real, el pago necesitaría seguridad y certificación que este prototipo no tiene.
- **Lugar de la IA:** la IA aceleró el trabajo, pero no reemplaza la observación de campo ni el juicio del equipo. Los errores de la Tabla 19 muestran que sus resultados siempre deben verificarse.

### 7.6. Lecciones aprendidas

1. Pedir a la IA soluciones con restricciones claras (estructuras propias, JSON, `decimal`) da código más alineado con los objetivos del curso.
2. Compilar y probar cada cambio antes de aceptarlo evita acumular errores.
3. Guardar el contexto en archivos del proyecto permite trabajar en varias sesiones sin perder decisiones.
4. Las capturas y los cálculos manuales son la evidencia; lo que dice la IA no lo es.
5. La privacidad se cuida desde el diseño: datos inventados, DNI oculto y fotos difuminadas.
