# RipleyComprasMAUI (.NET 10) — código fuente completo, archivo por archivo
Rutas relativas a la carpeta de la solución. El mismo código está en el ZIP del proyecto. No se listan los archivos binarios o gráficos que vienen sin cambios de la plantilla MAUI (`Resources/Fonts/OpenSans-*.ttf`, `Resources/AppIcon/*.svg`, `Resources/Splash/splash.svg`).

## Índice
1. `RipleyComprasMAUI.slnx`
2. `RipleyComprasMAUI/RipleyComprasMAUI.csproj`
3. `RipleyComprasMAUI/MauiProgram.cs`
4. `RipleyComprasMAUI/App.xaml`
5. `RipleyComprasMAUI/App.xaml.cs`
6. `RipleyComprasMAUI/Models/Producto.cs`
7. `RipleyComprasMAUI/Models/ItemCarrito.cs`
8. `RipleyComprasMAUI/Models/Venta.cs`
9. `RipleyComprasMAUI/Models/Boleta.cs`
10. `RipleyComprasMAUI/Structures/NodoSimple.cs`
11. `RipleyComprasMAUI/Structures/ListaEnlazada.cs`
12. `RipleyComprasMAUI/Structures/PilaAcciones.cs`
13. `RipleyComprasMAUI/Structures/ColaCompras.cs`
14. `RipleyComprasMAUI/Structures/NodoArbol.cs`
15. `RipleyComprasMAUI/Structures/ArbolBusqueda.cs`
16. `RipleyComprasMAUI/Structures/GrafoCajas.cs`
17. `RipleyComprasMAUI/Services/CatalogoService.cs`
18. `RipleyComprasMAUI/Services/CarritoService.cs`
19. `RipleyComprasMAUI/Services/CompraService.cs`
20. `RipleyComprasMAUI/Services/BoletaService.cs`
21. `RipleyComprasMAUI/Data/JsonDataManager.cs`
22. `RipleyComprasMAUI/Helpers/InputHelper.cs`
23. `RipleyComprasMAUI/UI/MainPage.xaml`
24. `RipleyComprasMAUI/UI/MainPage.xaml.cs`
25. `RipleyComprasMAUI/UI/EstilosNativos.cs`
26. `RipleyComprasMAUI/Resources/Raw/productos.json`
27. `RipleyComprasMAUI/Resources/Styles/Colors.xaml`
28. `RipleyComprasMAUI/Resources/Styles/Styles.xaml`
29. `RipleyComprasMAUI/Platforms/Windows/App.xaml`
30. `RipleyComprasMAUI/Platforms/Windows/App.xaml.cs`
31. `RipleyComprasMAUI/Platforms/Windows/app.manifest`
32. `RipleyComprasMAUI/Platforms/Windows/Package.appxmanifest`
33. `RipleyComprasMAUI/Properties/launchSettings.json`
34. `Comprobaciones/Comprobaciones.csproj`
35. `Comprobaciones/Program.cs`

---

## 1. `RipleyComprasMAUI.slnx`

```xml
<Solution>
  <Project Path="RipleyComprasMAUI/RipleyComprasMAUI.csproj" />
  <Project Path="Comprobaciones/Comprobaciones.csproj" />
</Solution>
```

---

## 2. `RipleyComprasMAUI/RipleyComprasMAUI.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">

	<PropertyGroup>
		<!-- Aplicación de escritorio Windows con .NET 10 y .NET MAUI (plantilla oficial "Aplicación .NET MAUI").
		     La plantilla también incluye net10.0-android;net10.0-ios;net10.0-maccatalyst; aquí se deja solo Windows,
		     que es la plataforma pedida. Para volver a multiplataforma, reemplace esta línea por las de la plantilla. -->
		<TargetFrameworks>net10.0-windows10.0.19041.0</TargetFrameworks>

		<OutputType>Exe</OutputType>
		<RootNamespace>RipleyComprasMAUI</RootNamespace>
		<UseMaui>true</UseMaui>
		<SingleProject>true</SingleProject>
		<ImplicitUsings>enable</ImplicitUsings>
		<Nullable>enable</Nullable>

		<!-- Display name -->
		<ApplicationTitle>RipleyComprasMAUI</ApplicationTitle>

		<!-- App Identifier -->
		<ApplicationId>pe.edu.upn.ripleycomprasmaui</ApplicationId>

		<!-- Versions -->
		<ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>
		<ApplicationVersion>1</ApplicationVersion>

		<!-- Aplicación sin empaquetar (unpackaged): se ejecuta con F5 en "Windows Machine". -->
		<WindowsPackageType>None</WindowsPackageType>

		<SupportedOSPlatformVersion>10.0.17763.0</SupportedOSPlatformVersion>
		<TargetPlatformMinVersion>10.0.17763.0</TargetPlatformMinVersion>
	</PropertyGroup>

	<ItemGroup>
		<!-- App Icon (archivos de la plantilla) -->
		<MauiIcon Include="Resources\AppIcon\appicon.svg" ForegroundFile="Resources\AppIcon\appiconfg.svg" Color="#4B2A7B" />

		<!-- Splash Screen (archivo de la plantilla; Windows no lo muestra) -->
		<MauiSplashScreen Include="Resources\Splash\splash.svg" Color="#4B2A7B" BaseSize="128,128" />

		<!-- Custom Fonts (archivos de la plantilla) -->
		<MauiFont Include="Resources\Fonts\*" />

		<!-- Raw Assets: Resources\Raw\productos.json se empaqueta con el nombre lógico "productos.json" -->
		<MauiAsset Include="Resources\Raw\**" LogicalName="%(RecursiveDir)%(Filename)%(Extension)" />
	</ItemGroup>

	<ItemGroup>
		<PackageReference Include="Microsoft.Maui.Controls" Version="$(MauiVersion)" />
		<PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="10.0.0" />
	</ItemGroup>

</Project>
```

---

## 3. `RipleyComprasMAUI/MauiProgram.cs`

```csharp
using System.Globalization;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Storage;
using RipleyComprasMAUI.Data;
using RipleyComprasMAUI.Services;
using RipleyComprasMAUI.UI;

namespace RipleyComprasMAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // Formato uniforme en toda la interfaz: punto decimal (S/ 199.97); las fechas se formatean como dd/MM/yyyy.
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Los JSON de trabajo viven en la carpeta privada de la app; la semilla es Resources/Raw/productos.json.
        string carpetaDatos = Path.Combine(FileSystem.AppDataDirectory, "data");
        builder.Services.AddSingleton(new JsonDataManager(carpetaDatos, () => FileSystem.OpenAppPackageFileAsync("productos.json")));

        // Una sola instancia de cada servicio por sesión: la fila no se pierde al cambiar de sección.
        builder.Services.AddSingleton<CatalogoService>();
        builder.Services.AddSingleton<CarritoService>();
        builder.Services.AddSingleton<BoletaService>();
        builder.Services.AddSingleton<CompraService>();
        builder.Services.AddSingleton<MainPage>();

        // Colores legibles para los selectores y el calendario de Windows (tema claro u oscuro del sistema).
        EstilosNativos.Aplicar();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
```

---

## 4. `RipleyComprasMAUI/App.xaml`

```xml
<?xml version="1.0" encoding="UTF-8" ?>
<Application xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="RipleyComprasMAUI.App">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="Resources/Styles/Colors.xaml" />
                <ResourceDictionary Source="Resources/Styles/Styles.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

---

## 5. `RipleyComprasMAUI/App.xaml.cs`

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using RipleyComprasMAUI.UI;

namespace RipleyComprasMAUI;

public partial class App : Application
{
    private readonly IServiceProvider servicios;

    public App(IServiceProvider servicios)
    {
        InitializeComponent();          // primero se cargan los recursos (colores y estilos)
        UserAppTheme = AppTheme.Light;
        this.servicios = servicios;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // MainPage se pide al contenedor DESPUÉS de cargar los recursos de App: sus StaticResource ya existen.
        // Es una única instancia (AddSingleton), con los mismos servicios durante toda la sesión.
        MainPage paginaPrincipal = servicios.GetRequiredService<MainPage>();
        return new Window(paginaPrincipal)
        {
            Title = "Ripley - Caja",
            Width = 1320,
            Height = 880,
            MinimumWidth = 1000,
            MinimumHeight = 680
        };
    }
}
```

---

## 6. `RipleyComprasMAUI/Models/Producto.cs`

```csharp
namespace RipleyComprasMAUI.Models;

/// <summary>
/// Prenda del catálogo (Preventa). Se guarda en productos.json y en el árbol por código.
/// </summary>
public class Producto
{
    public string Codigo { get; set; } = "";             // 13 dígitos, se guarda como texto (no como número)
    public string Nombre { get; set; } = "";
    public decimal PrecioRegular { get; set; }
    public decimal PrecioTarjetaRipley { get; set; }      // precio promocional: mayor que 0 y no mayor que el regular
    public int Stock { get; set; }                        // se descuenta solo al finalizar una venta
}
```

---

## 7. `RipleyComprasMAUI/Models/ItemCarrito.cs`

```csharp
using System.Text.Json.Serialization;

namespace RipleyComprasMAUI.Models;

/// <summary>
/// Línea de compra. Copia los dos precios al agregarse, así la venta conserva sus valores
/// históricos aunque el catálogo cambie después.
/// </summary>
public class ItemCarrito
{
    public string Codigo { get; set; } = "";
    public string Nombre { get; set; } = "";
    public int Cantidad { get; set; }
    public decimal PrecioRegular { get; set; }
    public decimal PrecioTarjetaRipley { get; set; }
    public decimal PrecioAplicado { get; set; }           // lo fija CompraService según la tarjeta; queda en la boleta

    // Cálculos derivados: no se guardan en el JSON porque se obtienen de los datos anteriores.
    [JsonIgnore]
    public decimal SubtotalRegular => PrecioRegular * Cantidad;

    [JsonIgnore]
    public decimal SubtotalAplicado => PrecioAplicado * Cantidad;

    /// <summary>Copia independiente: cambiar la copia no altera el original.</summary>
    public ItemCarrito Copiar()
    {
        return new ItemCarrito
        {
            Codigo = Codigo,
            Nombre = Nombre,
            Cantidad = Cantidad,
            PrecioRegular = PrecioRegular,
            PrecioTarjetaRipley = PrecioTarjetaRipley,
            PrecioAplicado = PrecioAplicado
        };
    }
}
```

---

## 8. `RipleyComprasMAUI/Models/Venta.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RipleyComprasMAUI.Models;

/// <summary>Estados de una compra en el prototipo (sin estados bancarios).</summary>
public enum EstadoVenta
{
    EnFila,
    EnAtencion,
    Finalizada
}

/// <summary>Medio de pago simulado: lista cerrada, nunca texto libre.</summary>
public enum TipoTarjeta
{
    SinSeleccionar,
    TarjetaRipley,
    OtraTarjeta
}

/// <summary>
/// Compra de un cliente: datos del cliente, copia de sus artículos, tarjeta e importes.
/// Puede estar pendiente (en la fila o en caja) o finalizada. La Boleta queda en null
/// hasta que se emite el comprobante; Boleta no guarda una referencia de vuelta a la Venta.
/// </summary>
public class Venta
{
    public Guid Id { get; set; }                               // identificador único de la venta
    public int Turno { get; set; }                             // número de llegada en la sesión
    public string? NombreCliente { get; set; }                 // null = compra anónima
    public string? Dni { get; set; }                           // string de 8 dígitos (conserva ceros iniciales)
    public string? Correo { get; set; }
    public EstadoVenta Estado { get; set; } = EstadoVenta.EnFila;
    public List<ItemCarrito> Articulos { get; set; } = new();  // copia independiente del carrito (colección de transporte JSON)
    public TipoTarjeta Tarjeta { get; set; } = TipoTarjeta.SinSeleccionar;
    public DateTime FechaLlegada { get; set; }
    public decimal TotalRegular { get; set; }
    public decimal Ahorro { get; set; }
    public decimal TotalFinal { get; set; }
    public decimal OperacionGravada { get; set; }
    public decimal Igv { get; set; }
    public Boleta? Boleta { get; set; }                        // comprobante asociado (null mientras no se emite)

    // Textos para mostrar en la interfaz; no se guardan en el JSON.
    [JsonIgnore]
    public string ClienteTexto => string.IsNullOrWhiteSpace(NombreCliente) ? "Cliente anónimo" : NombreCliente;

    [JsonIgnore]
    public string TarjetaTexto => NombreTarjeta(Tarjeta);

    [JsonIgnore]
    public int Unidades
    {
        get
        {
            int total = 0;
            foreach (ItemCarrito articulo in Articulos)
            {
                total += articulo.Cantidad;
            }
            return total;
        }
    }

    public static string NombreTarjeta(TipoTarjeta tarjeta)
    {
        return tarjeta switch
        {
            TipoTarjeta.TarjetaRipley => "Tarjeta Ripley",
            TipoTarjeta.OtraTarjeta => "Otra tarjeta",
            _ => "Sin tarjeta elegida"
        };
    }
}
```

---

## 9. `RipleyComprasMAUI/Models/Boleta.cs`

```csharp
using System;

namespace RipleyComprasMAUI.Models;

/// <summary>
/// Comprobante simulado. Guarda solo sus datos propios (número, fecha de emisión, leyenda)
/// y el identificador de la venta a la que pertenece. Los artículos, el cliente, la tarjeta
/// y los importes se leen de la Venta asociada: no se copian aquí.
/// </summary>
public class Boleta
{
    public const string LeyendaSimulada = "Boleta simulada - sin validez tributaria";

    public int Numero { get; set; }                 // consecutivo único, empieza en 1
    public DateTime FechaEmision { get; set; }      // fecha y hora reales del equipo al emitir
    public Guid IdVenta { get; set; }               // debe coincidir con Venta.Id
    public string Leyenda { get; set; } = LeyendaSimulada;
}
```

---

## 10. `RipleyComprasMAUI/Structures/NodoSimple.cs`

```csharp
namespace RipleyComprasMAUI.Structures;

/// <summary>
/// Nodo de enlace simple: guarda un dato y la referencia al siguiente nodo.
/// Lo reutilizan la lista enlazada, la pila y la cola.
/// </summary>
public class NodoSimple<T>
{
    public T Dato { get; set; }
    public NodoSimple<T>? Siguiente { get; set; }   // null cuando es el último

    public NodoSimple(T dato)
    {
        Dato = dato;
        Siguiente = null;
    }
}
```

---

## 11. `RipleyComprasMAUI/Structures/ListaEnlazada.cs`

```csharp
using System;
using System.Collections;
using System.Collections.Generic;

namespace RipleyComprasMAUI.Structures;

/// <summary>
/// Lista enlazada simple implementada a mano. Guarda la cabeza y el último nodo
/// para agregar al final en O(1). Se usa para el carrito, el historial de ventas,
/// las bitácoras y las listas de adyacencia del grafo.
/// </summary>
public class ListaEnlazada<T> : IEnumerable<T>
{
    private NodoSimple<T>? cabeza;
    private NodoSimple<T>? ultimo;

    public int Cantidad { get; private set; }
    public bool EstaVacia => cabeza == null;

    /// <summary>O(1): el nuevo nodo se enlaza después del último.</summary>
    public void AgregarAlFinal(T dato)
    {
        NodoSimple<T> nuevo = new NodoSimple<T>(dato);
        if (ultimo == null)
        {
            cabeza = nuevo;              // lista vacía: el nuevo es cabeza y último
            ultimo = nuevo;
        }
        else
        {
            ultimo.Siguiente = nuevo;    // el último anterior apunta al nuevo
            ultimo = nuevo;
        }
        Cantidad++;
    }

    /// <summary>O(1): el nuevo nodo apunta a la cabeza anterior y pasa a ser la cabeza.</summary>
    public void AgregarAlInicio(T dato)
    {
        NodoSimple<T> nuevo = new NodoSimple<T>(dato);
        nuevo.Siguiente = cabeza;
        cabeza = nuevo;
        if (ultimo == null)
        {
            ultimo = nuevo;
        }
        Cantidad++;
    }

    /// <summary>
    /// O(n): inserta en una posición (0 = inicio). Deshacer la usa para devolver
    /// una prenda quitada a su lugar original.
    /// </summary>
    public void InsertarEn(int posicion, T dato)
    {
        if (posicion <= 0 || cabeza == null)
        {
            AgregarAlInicio(dato);
            return;
        }
        if (posicion >= Cantidad)
        {
            AgregarAlFinal(dato);
            return;
        }

        NodoSimple<T> anterior = cabeza;
        for (int i = 0; i < posicion - 1; i++)
        {
            anterior = anterior.Siguiente!;            // avanza hasta el nodo previo a la posición
        }
        NodoSimple<T> nuevo = new NodoSimple<T>(dato);
        nuevo.Siguiente = anterior.Siguiente;          // el nuevo apunta al que estaba en esa posición
        anterior.Siguiente = nuevo;                    // el anterior apunta al nuevo
        Cantidad++;
    }

    /// <summary>O(n): devuelve el primer dato que cumple la condición, o default si no existe.</summary>
    public T? Buscar(Func<T, bool> condicion)
    {
        NodoSimple<T>? actual = cabeza;
        while (actual != null)
        {
            if (condicion(actual.Dato))
            {
                return actual.Dato;
            }
            actual = actual.Siguiente;
        }
        return default;
    }

    /// <summary>
    /// O(n): quita el primer dato que cumple la condición. Localizarlo cuesta O(n);
    /// después solo se reenlaza un nodo, sin desplazar elementos. Devuelve la posición que ocupaba.
    /// </summary>
    public bool Eliminar(Func<T, bool> condicion, out T? eliminado, out int posicion)
    {
        NodoSimple<T>? anterior = null;
        NodoSimple<T>? actual = cabeza;
        posicion = 0;

        while (actual != null)
        {
            if (condicion(actual.Dato))
            {
                if (anterior == null)
                {
                    cabeza = actual.Siguiente;               // se quitó la cabeza
                }
                else
                {
                    anterior.Siguiente = actual.Siguiente;   // el anterior "salta" el nodo quitado
                }
                if (actual == ultimo)
                {
                    ultimo = anterior;                       // se quitó el último
                }
                Cantidad--;
                eliminado = actual.Dato;
                return true;
            }
            anterior = actual;
            actual = actual.Siguiente;
            posicion++;
        }

        eliminado = default;
        posicion = -1;
        return false;
    }

    public void Limpiar()
    {
        cabeza = null;
        ultimo = null;
        Cantidad = 0;
    }

    /// <summary>Recorrido O(n) desde la cabeza hasta el último nodo (permite usar foreach).</summary>
    public IEnumerator<T> GetEnumerator()
    {
        NodoSimple<T>? actual = cabeza;
        while (actual != null)
        {
            yield return actual.Dato;
            actual = actual.Siguiente;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
```

---

## 12. `RipleyComprasMAUI/Structures/PilaAcciones.cs`

```csharp
using System;

namespace RipleyComprasMAUI.Structures;

/// <summary>
/// Pila LIFO enlazada: la última acción apilada es la primera que se deshace.
/// CarritoService guarda en ella las acciones del carrito (AccionCarrito).
/// </summary>
public class PilaAcciones<T>
{
    private NodoSimple<T>? cima;

    public int Cantidad { get; private set; }
    public bool EstaVacia => cima == null;

    /// <summary>O(1): el nuevo nodo apunta a la cima anterior y pasa a ser la cima.</summary>
    public void Apilar(T dato)
    {
        NodoSimple<T> nuevo = new NodoSimple<T>(dato);
        nuevo.Siguiente = cima;
        cima = nuevo;
        Cantidad++;
    }

    /// <summary>O(1): retira y devuelve la cima.</summary>
    public T Desapilar()
    {
        if (cima == null)
        {
            throw new InvalidOperationException("La pila está vacía.");
        }
        T dato = cima.Dato;
        cima = cima.Siguiente;          // la nueva cima es el nodo de abajo
        Cantidad--;
        return dato;
    }

    /// <summary>O(1): consulta la cima sin retirarla.</summary>
    public T? VerCima()
    {
        return cima == null ? default : cima.Dato;
    }

    public void Limpiar()
    {
        cima = null;
        Cantidad = 0;
    }
}
```

---

## 13. `RipleyComprasMAUI/Structures/ColaCompras.cs`

```csharp
using System;
using System.Collections;
using System.Collections.Generic;

namespace RipleyComprasMAUI.Structures;

/// <summary>
/// Cola FIFO enlazada con referencias al frente y al final: la primera compra
/// que llega es la primera que se atiende. Encolar y desencolar cuestan O(1).
/// </summary>
public class ColaCompras<T> : IEnumerable<T>
{
    private NodoSimple<T>? frente;
    private NodoSimple<T>? final;

    public int Cantidad { get; private set; }
    public bool EstaVacia => frente == null;

    /// <summary>O(1): entra por el final.</summary>
    public void Encolar(T dato)
    {
        NodoSimple<T> nuevo = new NodoSimple<T>(dato);
        if (final == null)
        {
            frente = nuevo;              // cola vacía: frente y final son el mismo nodo
            final = nuevo;
        }
        else
        {
            final.Siguiente = nuevo;     // el final anterior apunta al nuevo
            final = nuevo;
        }
        Cantidad++;
    }

    /// <summary>O(1): sale el frente (el que llegó primero).</summary>
    public T Desencolar()
    {
        if (frente == null)
        {
            throw new InvalidOperationException("La cola está vacía.");
        }
        T dato = frente.Dato;
        frente = frente.Siguiente;
        if (frente == null)
        {
            final = null;                // la cola quedó vacía
        }
        Cantidad--;
        return dato;
    }

    /// <summary>O(1): consulta el frente sin retirarlo.</summary>
    public T? VerFrente()
    {
        return frente == null ? default : frente.Dato;
    }

    /// <summary>Recorrido O(n) del frente al final, solo para mostrar la fila.</summary>
    public IEnumerator<T> GetEnumerator()
    {
        NodoSimple<T>? actual = frente;
        while (actual != null)
        {
            yield return actual.Dato;
            actual = actual.Siguiente;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
```

---

## 14. `RipleyComprasMAUI/Structures/NodoArbol.cs`

```csharp
namespace RipleyComprasMAUI.Structures;

/// <summary>
/// Nodo del árbol de búsqueda: clave, valor, hijos y altura (necesaria para el balanceo AVL).
/// </summary>
public class NodoArbol<TClave, TValor>
{
    public TClave Clave { get; }
    public TValor Valor { get; set; }
    public NodoArbol<TClave, TValor>? Izquierdo { get; set; }   // claves menores
    public NodoArbol<TClave, TValor>? Derecho { get; set; }     // claves mayores
    public int Altura { get; set; }                             // una hoja tiene altura 1

    public NodoArbol(TClave clave, TValor valor)
    {
        Clave = clave;
        Valor = valor;
        Altura = 1;
    }
}
```

---

## 15. `RipleyComprasMAUI/Structures/ArbolBusqueda.cs`

```csharp
using System;
using System.Collections.Generic;

namespace RipleyComprasMAUI.Structures;

/// <summary>
/// Árbol binario de búsqueda con balanceo AVL. Las claves menores van a la izquierda y las
/// mayores a la derecha; tras cada inserción se corrige el desequilibrio con rotaciones, así la
/// altura se mantiene cerca de log2(n) y buscar cuesta O(log n).
/// CatalogoService lo usa con clave Codigo (string) y BoletaService con clave Boleta.Numero (int).
/// Solo incluye inserción, búsqueda, recorrido inorden, altura y rotaciones (no se necesita eliminar).
/// </summary>
public class ArbolBusqueda<TClave, TValor>
{
    private readonly IComparer<TClave> comparador;
    private NodoArbol<TClave, TValor>? raiz;

    public ArbolBusqueda(IComparer<TClave>? comparador = null)
    {
        // Para códigos se usa comparación ordinal: compara carácter por carácter, sin reglas de idioma.
        this.comparador = comparador ?? Comparer<TClave>.Default;
    }

    public int Cantidad { get; private set; }
    public int Altura => AlturaDe(raiz);
    public int Rotaciones { get; private set; }               // rotaciones realizadas desde que se creó
    public int UltimasComparaciones { get; private set; }     // nodos visitados en la última búsqueda
    public bool EstaVacio => raiz == null;
    public TClave? ClaveRaiz => raiz == null ? default : raiz.Clave;

    /// <summary>Inserta la clave; devuelve false si ya existía (no se duplican claves).</summary>
    public bool Insertar(TClave clave, TValor valor)
    {
        bool insertado = false;
        raiz = Insertar(raiz, clave, valor, ref insertado);
        if (insertado)
        {
            Cantidad++;
        }
        return insertado;
    }

    private NodoArbol<TClave, TValor> Insertar(NodoArbol<TClave, TValor>? nodo, TClave clave, TValor valor, ref bool insertado)
    {
        if (nodo == null)
        {
            insertado = true;
            return new NodoArbol<TClave, TValor>(clave, valor);       // lugar vacío: nueva hoja
        }

        int comparacion = comparador.Compare(clave, nodo.Clave);
        if (comparacion < 0)
        {
            nodo.Izquierdo = Insertar(nodo.Izquierdo, clave, valor, ref insertado);
        }
        else if (comparacion > 0)
        {
            nodo.Derecho = Insertar(nodo.Derecho, clave, valor, ref insertado);
        }
        else
        {
            return nodo;                                               // clave repetida: no se inserta
        }

        return Balancear(nodo);                                        // al volver de la recursión se reequilibra
    }

    /// <summary>
    /// O(log n): baja desde la raíz comparando la clave; cuenta cada nodo visitado.
    /// </summary>
    public bool Buscar(TClave clave, out TValor? valor)
    {
        UltimasComparaciones = 0;
        NodoArbol<TClave, TValor>? actual = raiz;

        while (actual != null)
        {
            UltimasComparaciones++;
            int comparacion = comparador.Compare(clave, actual.Clave);
            if (comparacion == 0)
            {
                valor = actual.Valor;
                return true;
            }
            actual = comparacion < 0 ? actual.Izquierdo : actual.Derecho;
        }

        valor = default;
        return false;
    }

    /// <summary>O(n): izquierdo, nodo, derecho; devuelve los valores ordenados por clave.</summary>
    public ListaEnlazada<TValor> RecorrerInOrden()
    {
        ListaEnlazada<TValor> resultado = new ListaEnlazada<TValor>();
        RecorrerInOrden(raiz, resultado);
        return resultado;
    }

    private static void RecorrerInOrden(NodoArbol<TClave, TValor>? nodo, ListaEnlazada<TValor> resultado)
    {
        if (nodo == null)
        {
            return;
        }
        RecorrerInOrden(nodo.Izquierdo, resultado);
        resultado.AgregarAlFinal(nodo.Valor);
        RecorrerInOrden(nodo.Derecho, resultado);
    }

    // ===================== Balanceo AVL =====================

    private static int AlturaDe(NodoArbol<TClave, TValor>? nodo)
    {
        return nodo == null ? 0 : nodo.Altura;
    }

    private static void ActualizarAltura(NodoArbol<TClave, TValor> nodo)
    {
        nodo.Altura = 1 + Math.Max(AlturaDe(nodo.Izquierdo), AlturaDe(nodo.Derecho));
    }

    /// <summary>Factor de equilibrio = altura izquierda − altura derecha (debe quedar entre −1 y 1).</summary>
    private static int FactorEquilibrio(NodoArbol<TClave, TValor> nodo)
    {
        return AlturaDe(nodo.Izquierdo) - AlturaDe(nodo.Derecho);
    }

    private NodoArbol<TClave, TValor> Balancear(NodoArbol<TClave, TValor> nodo)
    {
        ActualizarAltura(nodo);
        int factor = FactorEquilibrio(nodo);

        if (factor > 1)                                    // cargado a la izquierda
        {
            if (FactorEquilibrio(nodo.Izquierdo!) < 0)
            {
                nodo.Izquierdo = RotarIzquierda(nodo.Izquierdo!);   // caso izquierda-derecha: rotación doble
            }
            return RotarDerecha(nodo);
        }

        if (factor < -1)                                   // cargado a la derecha
        {
            if (FactorEquilibrio(nodo.Derecho!) > 0)
            {
                nodo.Derecho = RotarDerecha(nodo.Derecho!);         // caso derecha-izquierda: rotación doble
            }
            return RotarIzquierda(nodo);
        }

        return nodo;                                       // ya está equilibrado
    }

    /// <summary>
    /// Rotación a la derecha: el hijo izquierdo sube y el nodo baja a su derecha.
    /// El subárbol derecho del hijo pasa a ser el izquierdo del nodo.
    /// </summary>
    private NodoArbol<TClave, TValor> RotarDerecha(NodoArbol<TClave, TValor> nodo)
    {
        NodoArbol<TClave, TValor> hijo = nodo.Izquierdo!;
        nodo.Izquierdo = hijo.Derecho;
        hijo.Derecho = nodo;
        ActualizarAltura(nodo);          // primero el que quedó abajo
        ActualizarAltura(hijo);
        Rotaciones++;
        return hijo;                     // nueva raíz de este subárbol
    }

    /// <summary>
    /// Rotación a la izquierda: el hijo derecho sube y el nodo baja a su izquierda.
    /// El subárbol izquierdo del hijo pasa a ser el derecho del nodo.
    /// </summary>
    private NodoArbol<TClave, TValor> RotarIzquierda(NodoArbol<TClave, TValor> nodo)
    {
        NodoArbol<TClave, TValor> hijo = nodo.Derecho!;
        nodo.Derecho = hijo.Izquierdo;
        hijo.Izquierdo = nodo;
        ActualizarAltura(nodo);
        ActualizarAltura(hijo);
        Rotaciones++;
        return hijo;
    }
}
```

---

## 16. `RipleyComprasMAUI/Structures/GrafoCajas.cs`

```csharp
using System;
using System.Globalization;

namespace RipleyComprasMAUI.Structures;

/// <summary>
/// COMPLEMENTO exigido por el desafío (no aparece en la imagen de arquitectura).
/// Grafo ponderado no dirigido de cajas y servidores ficticios de una tienda Ripley.
/// Cada caja tiene su lista de adyacencia (ListaEnlazada de enlaces); cada enlace tiene
/// un peso (tiempo de sincronización simulado, en ms) y puede estar activo o caído.
/// La ruta mínima se calcula con Dijkstra eligiendo el menor por recorrido simple, sin PriorityQueue.
/// Es una demostración en memoria: no envía datos ni interviene en el pago.
/// </summary>
public class GrafoCajas
{
    /// <summary>Arista de una lista de adyacencia.</summary>
    public class Enlace
    {
        public int Destino { get; }          // índice de la caja destino
        public double Peso { get; set; }     // tiempo de sincronización simulado (ms)
        public bool Activo { get; set; }     // false = conexión caída

        public Enlace(int destino, double peso)
        {
            Destino = destino;
            Peso = peso;
            Activo = true;
        }
    }

    private readonly string[] nombres;                       // nombre de cada caja (índice = vértice)
    private readonly ListaEnlazada<Enlace>[] adyacencia;     // una lista enlazada propia por caja
    private int cantidad;

    public GrafoCajas(int capacidad)
    {
        nombres = new string[capacidad];
        adyacencia = new ListaEnlazada<Enlace>[capacidad];
    }

    public int CantidadCajas => cantidad;

    public string NombreCaja(int indice)
    {
        return EsIndiceValido(indice) ? nombres[indice] : "(caja inexistente)";
    }

    public int AgregarCaja(string nombre)
    {
        if (cantidad == nombres.Length)
        {
            throw new InvalidOperationException("Se alcanzó la capacidad del grafo de ejemplo.");
        }
        nombres[cantidad] = nombre;
        adyacencia[cantidad] = new ListaEnlazada<Enlace>();
        cantidad++;
        return cantidad - 1;
    }

    /// <summary>Crea la conexión en ambos sentidos (grafo no dirigido).</summary>
    public bool Conectar(int a, int b, double peso)
    {
        if (!EsIndiceValido(a) || !EsIndiceValido(b) || a == b || peso <= 0 || BuscarEnlace(a, b) != null)
        {
            return false;
        }
        adyacencia[a].AgregarAlFinal(new Enlace(b, peso));
        adyacencia[b].AgregarAlFinal(new Enlace(a, peso));
        return true;
    }

    public bool CambiarEstadoEnlace(int a, int b, bool activo, out string mensaje)
    {
        Enlace? ida = BuscarEnlace(a, b);
        Enlace? vuelta = BuscarEnlace(b, a);
        if (ida == null || vuelta == null)
        {
            mensaje = $"No hay enlace directo entre {NombreCaja(a)} y {NombreCaja(b)}.";
            return false;
        }
        ida.Activo = activo;
        vuelta.Activo = activo;
        mensaje = $"Enlace {NombreCaja(a)} - {NombreCaja(b)}: {(activo ? "activo" : "caído")}.";
        return true;
    }

    public bool CambiarPeso(int a, int b, double peso, out string mensaje)
    {
        Enlace? ida = BuscarEnlace(a, b);
        Enlace? vuelta = BuscarEnlace(b, a);
        if (ida == null || vuelta == null)
        {
            mensaje = $"No hay enlace directo entre {NombreCaja(a)} y {NombreCaja(b)}.";
            return false;
        }
        if (!double.IsFinite(peso) || peso <= 0)
        {
            mensaje = "El peso debe ser un número positivo.";
            return false;
        }
        ida.Peso = peso;
        vuelta.Peso = peso;
        mensaje = $"Nuevo peso del enlace {NombreCaja(a)} - {NombreCaja(b)}: {Formatear(peso)} ms.";
        return true;
    }

    /// <summary>
    /// Dijkstra: distancias iniciales infinitas; en cada paso se elige, recorriendo el arreglo,
    /// la caja no visitada con menor distancia y se relajan sus enlaces activos. O(V² + E).
    /// </summary>
    public bool BuscarRutaMinima(int origen, int destino, out ListaEnlazada<int> ruta, out double distanciaTotal)
    {
        ruta = new ListaEnlazada<int>();
        distanciaTotal = 0;
        if (!EsIndiceValido(origen) || !EsIndiceValido(destino))
        {
            return false;
        }

        double[] distancia = new double[cantidad];
        int[] previo = new int[cantidad];
        bool[] visitado = new bool[cantidad];
        for (int i = 0; i < cantidad; i++)
        {
            distancia[i] = double.PositiveInfinity;
            previo[i] = -1;
        }
        distancia[origen] = 0;

        for (int paso = 0; paso < cantidad; paso++)
        {
            int actual = -1;
            for (int i = 0; i < cantidad; i++)       // recorrido simple: menor distancia no visitada
            {
                if (!visitado[i] && (actual == -1 || distancia[i] < distancia[actual]))
                {
                    actual = i;
                }
            }
            if (actual == -1 || double.IsPositiveInfinity(distancia[actual]))
            {
                break;                               // lo que queda es inalcanzable
            }
            visitado[actual] = true;
            if (actual == destino)
            {
                break;                               // la distancia del destino ya es definitiva
            }

            foreach (Enlace enlace in adyacencia[actual])
            {
                if (!enlace.Activo || visitado[enlace.Destino])
                {
                    continue;                        // los enlaces caídos no se usan
                }
                double candidata = distancia[actual] + enlace.Peso;
                if (candidata < distancia[enlace.Destino])
                {
                    distancia[enlace.Destino] = candidata;   // relajación: se encontró un camino más corto
                    previo[enlace.Destino] = actual;
                }
            }
        }

        if (double.IsPositiveInfinity(distancia[destino]))
        {
            return false;                            // no existe ruta con enlaces activos
        }

        for (int caja = destino; caja != -1; caja = previo[caja])
        {
            ruta.AgregarAlInicio(caja);              // se reconstruye desde el destino hacia el origen
        }
        distanciaTotal = distancia[destino];
        return true;
    }

    public string DescribirRuta(ListaEnlazada<int> ruta)
    {
        string texto = "";
        foreach (int caja in ruta)
        {
            texto += (texto.Length == 0 ? "" : " → ") + NombreCaja(caja);
        }
        return texto;
    }

    /// <summary>Una línea por enlace (cada par se muestra una sola vez).</summary>
    public ListaEnlazada<string> DescribirEnlaces()
    {
        ListaEnlazada<string> lineas = new ListaEnlazada<string>();
        for (int a = 0; a < cantidad; a++)
        {
            foreach (Enlace enlace in adyacencia[a])
            {
                if (a < enlace.Destino)
                {
                    lineas.AgregarAlFinal($"{nombres[a]} ↔ {nombres[enlace.Destino]} · {Formatear(enlace.Peso)} ms · {(enlace.Activo ? "activo" : "CAÍDO")}");
                }
            }
        }
        return lineas;
    }

    /// <summary>Red de ejemplo con nombres ficticios (pocos nodos, solo en memoria).</summary>
    public static GrafoCajas CrearEjemplo()
    {
        GrafoCajas grafo = new GrafoCajas(6);
        int caja511 = grafo.AgregarCaja("Caja 511");
        int caja512 = grafo.AgregarCaja("Caja 512");
        int caja513 = grafo.AgregarCaja("Caja 513");
        int servidor = grafo.AgregarCaja("Servidor de tienda");
        int almacen = grafo.AgregarCaja("Almacén");
        int central = grafo.AgregarCaja("Central (simulada)");

        grafo.Conectar(caja511, caja512, 4);
        grafo.Conectar(caja511, caja513, 9);
        grafo.Conectar(caja512, caja513, 3);
        grafo.Conectar(caja512, servidor, 10);
        grafo.Conectar(caja513, servidor, 4);
        grafo.Conectar(caja513, almacen, 7);
        grafo.Conectar(servidor, central, 15);
        grafo.Conectar(almacen, central, 20);
        return grafo;
    }

    private Enlace? BuscarEnlace(int a, int b)
    {
        if (!EsIndiceValido(a) || !EsIndiceValido(b))
        {
            return null;
        }
        return adyacencia[a].Buscar(enlace => enlace.Destino == b);   // recorrido de la lista de adyacencia
    }

    private bool EsIndiceValido(int indice)
    {
        return indice >= 0 && indice < cantidad;
    }

    private static string Formatear(double valor)
    {
        return valor.ToString("0.##", CultureInfo.InvariantCulture);
    }
}
```

---

## 17. `RipleyComprasMAUI/Services/CatalogoService.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Text;
using RipleyComprasMAUI.Data;
using RipleyComprasMAUI.Helpers;
using RipleyComprasMAUI.Models;
using RipleyComprasMAUI.Structures;

namespace RipleyComprasMAUI.Services;

/// <summary>
/// Preventa: administra y valida el catálogo. Las prendas viven en un árbol AVL con clave Codigo;
/// buscar por código recorre el árbol real y buscar por nombre recorre el catálogo completo.
/// </summary>
public class CatalogoService
{
    private readonly JsonDataManager datos;
    private ArbolBusqueda<string, Producto> arbol = new ArbolBusqueda<string, Producto>(StringComparer.Ordinal);
    private readonly List<Producto> rechazados = new List<Producto>();   // registros inválidos: se conservan tal cual al guardar

    public CatalogoService(JsonDataManager datos)
    {
        this.datos = datos;
    }

    public bool Cargado { get; private set; }
    public ListaEnlazada<string> Advertencias { get; } = new ListaEnlazada<string>();
    public bool StockPendienteDeGuardar { get; private set; }

    public int Cantidad => arbol.Cantidad;
    public int Altura => arbol.Altura;
    public int Rotaciones => arbol.Rotaciones;
    public int UltimasComparaciones => arbol.UltimasComparaciones;
    public string? CodigoRaiz => arbol.ClaveRaiz;

    /// <summary>Lee productos.json, valida cada prenda y la inserta en el árbol (O(n log n)).</summary>
    public bool Cargar(out string mensaje)
    {
        arbol = new ArbolBusqueda<string, Producto>(StringComparer.Ordinal);
        rechazados.Clear();
        Advertencias.Limpiar();
        Cargado = false;

        if (!datos.CargarProductos(out List<Producto> productos, out string error))
        {
            mensaje = "Catálogo no disponible: " + error;
            return false;
        }

        foreach (Producto producto in productos)
        {
            if (producto is null)
            {
                Advertencias.AgregarAlFinal("Se ignoró un elemento null en productos.json.");
                continue;
            }
            if (!EsProductoValido(producto, out string motivo))
            {
                rechazados.Add(producto);
                Advertencias.AgregarAlFinal($"Prenda rechazada ({producto.Codigo}): {motivo}.");
                continue;
            }
            if (!arbol.Insertar(producto.Codigo, producto))
            {
                rechazados.Add(producto);
                Advertencias.AgregarAlFinal($"Código repetido {producto.Codigo}: se conservó la primera prenda.");
            }
        }

        Cargado = true;
        mensaje = $"Catálogo: {arbol.Cantidad} prendas.";
        if (!Advertencias.EstaVacia)
        {
            mensaje += $" {Advertencias.Cantidad} registro(s) rechazado(s).";
        }
        return true;
    }

    /// <summary>Reglas del JSON: código de 13 dígitos, 0 &lt; precio Ripley ≤ precio regular, stock ≥ 0.</summary>
    public static bool EsProductoValido(Producto producto, out string motivo)
    {
        if (!InputHelper.EsCodigoValido(producto.Codigo))
        {
            motivo = "el código debe tener exactamente 13 dígitos";
            return false;
        }
        if (!InputHelper.TryValidarNombreProducto(producto.Nombre, out _, out motivo))
        {
            return false;
        }
        if (!InputHelper.EsImporteValido(producto.PrecioRegular))
        {
            motivo = "el precio regular debe ser mayor que 0 y tener como máximo 2 decimales";
            return false;
        }
        if (!InputHelper.EsImporteValido(producto.PrecioTarjetaRipley) || producto.PrecioTarjetaRipley > producto.PrecioRegular)
        {
            motivo = "el precio Tarjeta Ripley debe ser mayor que 0 y no mayor que el precio regular";
            return false;
        }
        if (producto.Stock < 0)
        {
            motivo = "el stock no puede ser negativo";
            return false;
        }
        motivo = "";
        return true;
    }

    /// <summary>O(log n): valida el formato y busca en el árbol.</summary>
    public bool BuscarPorCodigo(string? texto, out Producto? producto, out string mensaje)
    {
        producto = null;
        if (!InputHelper.TryValidarCodigo(texto, out string codigo, out mensaje))
        {
            return false;
        }
        if (!arbol.Buscar(codigo, out producto) || producto == null)
        {
            mensaje = "Prenda no encontrada.";
            return false;
        }
        mensaje = $"Prenda encontrada: {producto.Nombre}.";
        return true;
    }

    /// <summary>
    /// Recorre el catálogo en orden de código (O(n) prendas, más el costo de comparar cada texto)
    /// y devuelve todas las prendas cuyo nombre contiene la consulta, sin distinguir mayúsculas ni tildes.
    /// </summary>
    public ListaEnlazada<Producto> BuscarPorNombre(string? texto, out string mensaje)
    {
        ListaEnlazada<Producto> encontrados = new ListaEnlazada<Producto>();
        if (!InputHelper.TryValidarTextoBusqueda(texto, out string consulta, out mensaje))
        {
            return encontrados;
        }

        foreach (Producto producto in arbol.RecorrerInOrden())
        {
            if (InputHelper.NombreContiene(producto.Nombre, consulta))
            {
                encontrados.AgregarAlFinal(producto);
            }
        }

        mensaje = encontrados.EstaVacia
            ? "Prenda no encontrada."
            : $"{encontrados.Cantidad} producto(s) encontrado(s) para \"{consulta}\".";
        return encontrados;
    }

    /// <summary>Catálogo ordenado por código (recorrido inorden del árbol).</summary>
    public ListaEnlazada<Producto> Listar()
    {
        return arbol.RecorrerInOrden();
    }

    /// <summary>Usado por CarritoService para comprobar existencia y stock.</summary>
    public bool ObtenerProducto(string codigo, out Producto? producto)
    {
        return arbol.Buscar(codigo, out producto) && producto != null;
    }

    public int StockDisponible(string codigo)
    {
        return ObtenerProducto(codigo, out Producto? producto) ? producto!.Stock : 0;
    }

    /// <summary>Se vuelve a validar al finalizar: otra venta pudo consumir unidades mientras el cliente esperaba.</summary>
    public bool HayStockSuficiente(IEnumerable<ItemCarrito> articulos, out string conflictos)
    {
        StringBuilder detalle = new StringBuilder();
        foreach (ItemCarrito articulo in articulos)
        {
            int disponible = StockDisponible(articulo.Codigo);
            if (articulo.Cantidad > disponible)
            {
                detalle.AppendLine($"- {articulo.Nombre}: pide {articulo.Cantidad} y hay {disponible}.");
            }
        }
        conflictos = detalle.ToString().TrimEnd();
        return conflictos.Length == 0;
    }

    /// <summary>
    /// Guarda productos.json con el stock ya descontado, SIN tocar todavía el árbol:
    /// si el guardado falla, la memoria queda igual que antes.
    /// </summary>
    public bool GuardarConDescuento(IEnumerable<ItemCarrito> articulos, out string error)
    {
        if (!datos.GuardarProductos(CrearListaParaGuardar(articulos), out error))
        {
            return false;
        }
        StockPendienteDeGuardar = false;    // el archivo quedó al día con la memoria
        return true;
    }

    /// <summary>Descuenta el stock en memoria una sola vez, cuando la venta ya quedó registrada.</summary>
    public void AplicarDescuento(IEnumerable<ItemCarrito> articulos)
    {
        foreach (ItemCarrito articulo in articulos)
        {
            if (ObtenerProducto(articulo.Codigo, out Producto? producto))
            {
                producto!.Stock -= articulo.Cantidad;
            }
        }
    }

    /// <summary>
    /// Caso excepcional: la venta quedó en ventas.json pero productos.json no se pudo guardar.
    /// El stock correcto queda en memoria; el siguiente guardado exitoso escribe el catálogo completo.
    /// </summary>
    public void MarcarStockPendiente()
    {
        StockPendienteDeGuardar = true;
    }

    /// <summary>Copias de las prendas (lista de transporte para JSON) con el descuento indicado.</summary>
    private List<Producto> CrearListaParaGuardar(IEnumerable<ItemCarrito>? descontar)
    {
        List<Producto> lista = new List<Producto>();
        foreach (Producto producto in arbol.RecorrerInOrden())
        {
            int vendido = 0;
            if (descontar != null)
            {
                foreach (ItemCarrito articulo in descontar)
                {
                    if (articulo.Codigo == producto.Codigo)
                    {
                        vendido += articulo.Cantidad;
                    }
                }
            }
            lista.Add(new Producto
            {
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                PrecioRegular = producto.PrecioRegular,
                PrecioTarjetaRipley = producto.PrecioTarjetaRipley,
                Stock = producto.Stock - vendido
            });
        }
        lista.AddRange(rechazados);     // lo inválido no se usa, pero tampoco se borra del archivo
        return lista;
    }
}
```

---

## 18. `RipleyComprasMAUI/Services/CarritoService.cs`

```csharp
using System.Collections.Generic;
using RipleyComprasMAUI.Helpers;
using RipleyComprasMAUI.Models;
using RipleyComprasMAUI.Structures;

namespace RipleyComprasMAUI.Services;

/// <summary>Tipos de cambio que se pueden deshacer.</summary>
public enum TipoAccionCarrito
{
    AgregarLinea,       // se agregó una prenda nueva al carrito
    CambiarCantidad,    // cambió la cantidad de una línea existente
    QuitarLinea         // se quitó una línea
}

/// <summary>
/// Lo necesario para revertir un cambio. Guarda valores y copias, no referencias que puedan cambiar después.
/// </summary>
public class AccionCarrito
{
    public TipoAccionCarrito Tipo { get; init; }
    public string Codigo { get; init; } = "";
    public int CantidadAnterior { get; init; }          // para CambiarCantidad
    public int Posicion { get; init; }                  // para QuitarLinea: lugar que ocupaba en la lista
    public ItemCarrito? ArticuloQuitado { get; init; }  // para QuitarLinea: copia del artículo
    public string Descripcion { get; init; } = "";
}

/// <summary>
/// Carrito de UNA compra: su lista enlazada y su propia pila de deshacer.
/// Cada cliente tiene un contexto distinto, así no se mezclan artículos ni historiales.
/// </summary>
public class ContextoCarrito
{
    public ContextoCarrito(string referencia)
    {
        Referencia = referencia;
    }

    public string Referencia { get; }                                                   // "Carrito 3" o "Turno 2 (corrección en caja)"
    internal ListaEnlazada<ItemCarrito> Lista { get; } = new ListaEnlazada<ItemCarrito>();      // solo CarritoService la modifica
    internal PilaAcciones<AccionCarrito> Pila { get; } = new PilaAcciones<AccionCarrito>();

    public IEnumerable<ItemCarrito> Articulos => Lista;      // lectura para la interfaz y CompraService
    public int CantidadLineas => Lista.Cantidad;
    public bool EstaVacio => Lista.EstaVacia;
    public int AccionesParaDeshacer => Pila.Cantidad;
}

/// <summary>
/// Responsable exclusivo del carrito: agregar, cambiar cantidad, quitar, recorrer, vaciar y deshacer.
/// Usa ListaEnlazada (artículos) y PilaAcciones (deshacer). Consulta CatalogoService para existencia y stock.
/// No conoce la política de precios por tarjeta (eso es de CompraService).
/// </summary>
public class CarritoService
{
    public const string Subproceso = "Preventa - Selección de productos";

    private readonly CatalogoService catalogo;
    private int numeroCarrito;

    public CarritoService(CatalogoService catalogo)
    {
        this.catalogo = catalogo;
        numeroCarrito = 1;
        Actual = new ContextoCarrito("Carrito 1");
    }

    /// <summary>Carrito que se prepara en Preventa para el siguiente cliente.</summary>
    public ContextoCarrito Actual { get; private set; }

    /// <summary>Registros de la sesión de este subproceso.</summary>
    public ListaEnlazada<string> Bitacora { get; } = new ListaEnlazada<string>();

    /// <summary>Inicia un carrito y una pila nuevos: el cliente anterior ya no comparte nada con el siguiente.</summary>
    public void NuevoCarrito()
    {
        numeroCarrito++;
        Actual = new ContextoCarrito($"Carrito {numeroCarrito}");
    }

    /// <summary>
    /// Agrega una prenda. Si ya está en el carrito, aumenta su cantidad (no crea otra línea).
    /// Valida cantidad 1..99 y stock antes de modificar; si algo falla, ni la lista ni la pila cambian.
    /// </summary>
    public bool AgregarPrenda(ContextoCarrito carrito, string codigo, int cantidad, out string mensaje)
    {
        if (!InputHelper.EsCodigoValido(codigo))
        {
            mensaje = "Código inválido: debe tener exactamente 13 dígitos.";
            Registrar(carrito, "Agregar prenda", "Rechazado: " + mensaje);
            return false;
        }
        if (cantidad < InputHelper.CantidadMinima || cantidad > InputHelper.CantidadMaxima)
        {
            mensaje = $"La cantidad debe estar entre {InputHelper.CantidadMinima} y {InputHelper.CantidadMaxima}.";
            Registrar(carrito, "Agregar prenda", "Rechazado: " + mensaje);
            return false;
        }
        if (!catalogo.ObtenerProducto(codigo, out Producto? producto) || producto == null)
        {
            mensaje = "Prenda no encontrada.";
            Registrar(carrito, "Agregar prenda", "Rechazado: " + mensaje);
            return false;
        }

        ItemCarrito? existente = carrito.Lista.Buscar(articulo => articulo.Codigo == codigo);   // O(n)
        int cantidadAnterior = existente == null ? 0 : existente.Cantidad;
        int nuevaCantidad = cantidadAnterior + cantidad;

        if (nuevaCantidad > InputHelper.CantidadMaxima)
        {
            mensaje = $"Máximo {InputHelper.CantidadMaxima} unidades por prenda (ya hay {cantidadAnterior}).";
            Registrar(carrito, "Agregar prenda", "Rechazado: " + mensaje);
            return false;
        }
        if (nuevaCantidad > producto.Stock)
        {
            mensaje = $"Stock insuficiente de {producto.Nombre}: hay {producto.Stock} y el carrito tendría {nuevaCantidad}.";
            Registrar(carrito, "Agregar prenda", "Rechazado: " + mensaje);
            return false;
        }

        if (existente == null)
        {
            carrito.Lista.AgregarAlFinal(new ItemCarrito
            {
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Cantidad = cantidad,
                PrecioRegular = producto.PrecioRegular,
                PrecioTarjetaRipley = producto.PrecioTarjetaRipley
            });
            carrito.Pila.Apilar(new AccionCarrito
            {
                Tipo = TipoAccionCarrito.AgregarLinea,
                Codigo = codigo,
                Descripcion = $"agregar {producto.Nombre} x {cantidad}"
            });
        }
        else
        {
            carrito.Pila.Apilar(new AccionCarrito
            {
                Tipo = TipoAccionCarrito.CambiarCantidad,
                Codigo = codigo,
                CantidadAnterior = cantidadAnterior,
                Descripcion = $"{producto.Nombre} de {cantidadAnterior} a {nuevaCantidad}"
            });
            existente.Cantidad = nuevaCantidad;      // misma prenda: aumenta la cantidad de la línea existente
        }

        mensaje = $"Se agregó {producto.Nombre} x {cantidad}. Cantidad en el carrito: {nuevaCantidad}.";
        Registrar(carrito, "Agregar prenda", mensaje);
        return true;
    }

    /// <summary>
    /// Cambia la cantidad de una línea (también lo usan los botones + y −). Una cantidad igual
    /// a la actual no se considera cambio y no se apila. Para sacar la prenda se usa QuitarPrenda.
    /// </summary>
    public bool CambiarCantidad(ContextoCarrito carrito, string codigo, int nuevaCantidad, out string mensaje)
    {
        ItemCarrito? articulo = carrito.Lista.Buscar(item => item.Codigo == codigo);
        if (articulo == null)
        {
            mensaje = "El artículo no está en el carrito.";
            Registrar(carrito, "Cambiar cantidad", "Rechazado: " + mensaje);
            return false;
        }
        if (nuevaCantidad < InputHelper.CantidadMinima || nuevaCantidad > InputHelper.CantidadMaxima)
        {
            mensaje = $"La cantidad debe estar entre {InputHelper.CantidadMinima} y {InputHelper.CantidadMaxima}. Para sacar la prenda usa Quitar.";
            Registrar(carrito, "Cambiar cantidad", "Rechazado: " + mensaje);
            return false;
        }
        int stock = catalogo.StockDisponible(codigo);
        if (nuevaCantidad > stock)
        {
            mensaje = $"Stock insuficiente de {articulo.Nombre}: hay {stock}.";
            Registrar(carrito, "Cambiar cantidad", "Rechazado: " + mensaje);
            return false;
        }
        if (nuevaCantidad == articulo.Cantidad)
        {
            mensaje = "La cantidad no cambió.";
            return false;
        }

        carrito.Pila.Apilar(new AccionCarrito
        {
            Tipo = TipoAccionCarrito.CambiarCantidad,
            Codigo = codigo,
            CantidadAnterior = articulo.Cantidad,
            Descripcion = $"{articulo.Nombre} de {articulo.Cantidad} a {nuevaCantidad}"
        });
        articulo.Cantidad = nuevaCantidad;

        mensaje = $"{articulo.Nombre}: cantidad {nuevaCantidad}.";
        Registrar(carrito, "Cambiar cantidad", mensaje);
        return true;
    }

    /// <summary>Quita una línea: localizarla cuesta O(n) y luego solo se reenlaza un nodo.</summary>
    public bool QuitarPrenda(ContextoCarrito carrito, string codigo, out string mensaje)
    {
        if (!carrito.Lista.Eliminar(item => item.Codigo == codigo, out ItemCarrito? quitado, out int posicion) || quitado == null)
        {
            mensaje = "El artículo no está en el carrito.";
            Registrar(carrito, "Quitar prenda", "Rechazado: " + mensaje);
            return false;
        }

        carrito.Pila.Apilar(new AccionCarrito
        {
            Tipo = TipoAccionCarrito.QuitarLinea,
            Codigo = codigo,
            Posicion = posicion,
            ArticuloQuitado = quitado.Copiar(),
            Descripcion = $"quitar {quitado.Nombre}"
        });

        mensaje = $"Se quitó {quitado.Nombre} del carrito.";
        Registrar(carrito, "Quitar prenda", mensaje);
        return true;
    }

    /// <summary>
    /// Desapila la última acción y la revierte. Deshacer no apila una nueva acción.
    /// Al revertir una eliminación, la prenda vuelve a su posición original.
    /// </summary>
    public bool Deshacer(ContextoCarrito carrito, out string mensaje)
    {
        if (carrito.Pila.EstaVacia)
        {
            mensaje = "No hay acciones para deshacer.";
            Registrar(carrito, "Deshacer", mensaje);
            return false;
        }

        AccionCarrito accion = carrito.Pila.Desapilar();     // O(1): sale la última acción
        switch (accion.Tipo)
        {
            case TipoAccionCarrito.AgregarLinea:
                carrito.Lista.Eliminar(item => item.Codigo == accion.Codigo, out _, out _);
                break;

            case TipoAccionCarrito.CambiarCantidad:
                ItemCarrito? articulo = carrito.Lista.Buscar(item => item.Codigo == accion.Codigo);
                if (articulo != null)
                {
                    articulo.Cantidad = accion.CantidadAnterior;
                }
                break;

            case TipoAccionCarrito.QuitarLinea:
                if (accion.ArticuloQuitado != null)
                {
                    carrito.Lista.InsertarEn(accion.Posicion, accion.ArticuloQuitado.Copiar());
                }
                break;
        }

        mensaje = $"Se deshizo: {accion.Descripcion}.";
        Registrar(carrito, "Deshacer", mensaje);
        return true;
    }

    /// <summary>Vacía el carrito y su pila (el cliente empieza de nuevo).</summary>
    public void Vaciar(ContextoCarrito carrito, out string mensaje)
    {
        int lineas = carrito.Lista.Cantidad;
        carrito.Lista.Limpiar();
        carrito.Pila.Limpiar();
        mensaje = $"Carrito vaciado ({lineas} línea(s) retiradas).";
        Registrar(carrito, "Vaciar carrito", mensaje);
    }

    /// <summary>Recorre la lista O(m) y suma precio regular × cantidad.</summary>
    public decimal CalcularTotalRegular(ContextoCarrito carrito)
    {
        decimal total = 0;
        foreach (ItemCarrito articulo in carrito.Lista)
        {
            total += articulo.SubtotalRegular;
        }
        return total;
    }

    public int CantidadEnCarrito(ContextoCarrito carrito, string codigo)
    {
        ItemCarrito? articulo = carrito.Lista.Buscar(item => item.Codigo == codigo);
        return articulo == null ? 0 : articulo.Cantidad;
    }

    /// <summary>
    /// Copia independiente de los artículos para la Venta (lista de transporte JSON):
    /// preparar otra compra después no modifica la compra ya enviada a la fila.
    /// </summary>
    public List<ItemCarrito> CopiarArticulos(ContextoCarrito carrito)
    {
        List<ItemCarrito> copia = new List<ItemCarrito>();
        foreach (ItemCarrito articulo in carrito.Lista)
        {
            copia.Add(articulo.Copiar());
        }
        return copia;
    }

    /// <summary>
    /// Contexto separado para corregir la compra en atención (por ejemplo, por falta de stock),
    /// con su propia lista y su propia pila. No toca el carrito que se prepara en Preventa.
    /// </summary>
    public ContextoCarrito CrearContextoDeCorreccion(IEnumerable<ItemCarrito> articulos, string referencia)
    {
        ContextoCarrito contexto = new ContextoCarrito(referencia);
        foreach (ItemCarrito articulo in articulos)
        {
            contexto.Lista.AgregarAlFinal(articulo.Copiar());
        }
        return contexto;
    }

    private void Registrar(ContextoCarrito carrito, string accion, string resultado)
    {
        Bitacora.AgregarAlFinal(InputHelper.CrearRegistro(accion, carrito.Referencia, resultado));
    }
}
```

---

## 19. `RipleyComprasMAUI/Services/CompraService.cs`

```csharp
using System;
using System.Collections.Generic;
using RipleyComprasMAUI.Helpers;
using RipleyComprasMAUI.Models;
using RipleyComprasMAUI.Structures;

namespace RipleyComprasMAUI.Services;

/// <summary>
/// Venta: registra al cliente con una copia de su carrito, asigna turnos, encola y atiende
/// en orden de llegada (ColaCompras FIFO), mantiene una sola compra en atención, aplica la
/// tarjeta y calcula importes. Las modificaciones de artículos se piden a CarritoService;
/// la emisión de la boleta, a BoletaService; el stock, a CatalogoService.
/// </summary>
public class CompraService
{
    public const string Subproceso = "Venta - Pago";
    private const decimal FactorIgv = 1.18m;          // los precios ya incluyen IGV 18 %

    private readonly CarritoService carritos;
    private readonly CatalogoService catalogo;
    private readonly BoletaService boletas;
    private readonly ColaCompras<Venta> fila = new ColaCompras<Venta>();
    private int siguienteTurno = 1;
    private ContextoCarrito? correccion;              // carrito propio de la compra en atención (solo para corregirla)
    private bool confirmando;                         // evita confirmar dos veces la misma compra

    public CompraService(CarritoService carritos, CatalogoService catalogo, BoletaService boletas)
    {
        this.carritos = carritos;
        this.catalogo = catalogo;
        this.boletas = boletas;
    }

    public Venta? CompraEnAtencion { get; private set; }
    public ListaEnlazada<string> Bitacora { get; } = new ListaEnlazada<string>();
    public int ClientesEnFila => fila.Cantidad;
    public int AccionesDeCorreccion => correccion == null ? 0 : correccion.AccionesParaDeshacer;

    /// <summary>Copia de la fila solo para mostrarla (el orden es el de llegada).</summary>
    public List<Venta> ObtenerFila()
    {
        return new List<Venta>(fila);
    }

    /// <summary>
    /// El cliente llega con su carrito: se valida, se copia el carrito, se asigna turno y se encola (O(1)).
    /// Después, CarritoService inicia un carrito y una pila nuevos para el siguiente cliente.
    /// </summary>
    public bool RegistrarLlegada(string? nombre, string? dni, string? correo, out Venta? venta, out string mensaje)
    {
        venta = null;
        if (!InputHelper.TryValidarNombrePersona(nombre, out string? nombreValido, out mensaje)
            || !InputHelper.TryValidarDniOpcional(dni, out string? dniValido, out mensaje)
            || !InputHelper.TryValidarCorreo(correo, out string? correoValido, out mensaje))
        {
            Registrar("Registrar llegada", carritos.Actual.Referencia, "Rechazado: " + mensaje);
            return false;
        }

        ContextoCarrito carrito = carritos.Actual;
        if (carrito.EstaVacio)
        {
            mensaje = "El carrito está vacío: agrega al menos un producto antes de enviarlo a caja.";
            Registrar("Registrar llegada", carrito.Referencia, "Rechazado: " + mensaje);
            return false;
        }

        venta = new Venta
        {
            Id = Guid.NewGuid(),
            Turno = siguienteTurno,
            NombreCliente = nombreValido,
            Dni = dniValido,
            Correo = correoValido,
            Estado = EstadoVenta.EnFila,
            FechaLlegada = DateTime.Now,
            Articulos = carritos.CopiarArticulos(carrito)     // copia independiente del carrito
        };
        siguienteTurno++;
        CalcularImportes(venta);                              // sin tarjeta elegida: a precio regular
        fila.Encolar(venta);                                  // O(1): entra al final de la fila
        carritos.NuevoCarrito();                              // carrito y pila nuevos para el siguiente cliente

        mensaje = $"Turno {venta.Turno} ({venta.ClienteTexto}) enviado a caja con {venta.Articulos.Count} producto(s). Clientes en fila: {fila.Cantidad}.";
        Registrar("Registrar llegada", $"Turno {venta.Turno}", mensaje);
        return true;
    }

    /// <summary>Atiende al frente de la fila (O(1)). Solo puede haber una compra en atención.</summary>
    public bool AtenderSiguiente(out string mensaje)
    {
        if (CompraEnAtencion != null)
        {
            mensaje = $"Ya se atiende el turno {CompraEnAtencion.Turno}: confirma el pago o retira la compra antes de atender al siguiente.";
            Registrar("Atender siguiente", $"Turno {CompraEnAtencion.Turno}", "Rechazado: " + mensaje);
            return false;
        }
        if (fila.EstaVacia)
        {
            mensaje = "No hay clientes en la fila.";
            Registrar("Atender siguiente", "-", "Rechazado: " + mensaje);
            return false;
        }

        Venta venta = fila.Desencolar();                       // sale el que llegó primero
        venta.Estado = EstadoVenta.EnAtencion;
        CompraEnAtencion = venta;
        correccion = carritos.CrearContextoDeCorreccion(venta.Articulos, $"Turno {venta.Turno} (corrección en caja)");
        CalcularImportes(venta);

        mensaje = $"Atendiendo el turno {venta.Turno} ({venta.ClienteTexto}). Quedan {fila.Cantidad} en la fila. Selecciona una tarjeta.";
        Registrar("Atender siguiente", $"Turno {venta.Turno}", mensaje);
        return true;
    }

    /// <summary>Elige Tarjeta Ripley u Otra tarjeta y recalcula. Se puede cambiar antes de confirmar.</summary>
    public bool SeleccionarTarjeta(TipoTarjeta tarjeta, out string mensaje)
    {
        Venta? venta = CompraEnAtencion;
        if (venta == null)
        {
            mensaje = "No hay una compra en atención.";
            return false;
        }
        if (!InputHelper.EsTarjetaPermitida(tarjeta))
        {
            mensaje = "Selecciona una tarjeta: Tarjeta Ripley u Otra tarjeta.";
            Registrar("Elegir tarjeta", $"Turno {venta.Turno}", "Rechazado: " + mensaje);
            return false;
        }

        venta.Tarjeta = tarjeta;
        CalcularImportes(venta);
        mensaje = $"{venta.TarjetaTexto}: total final {InputHelper.FormatearSoles(venta.TotalFinal)}, ahorro {InputHelper.FormatearSoles(venta.Ahorro)}.";
        Registrar("Elegir tarjeta", $"Turno {venta.Turno}", mensaje);
        return true;
    }

    /// <summary>Única política de precios: con Tarjeta Ripley se usa el precio promocional; si no, el regular.</summary>
    public static decimal PrecioSegunTarjeta(ItemCarrito articulo, TipoTarjeta tarjeta)
    {
        return tarjeta == TipoTarjeta.TarjetaRipley ? articulo.PrecioTarjetaRipley : articulo.PrecioRegular;
    }

    /// <summary>
    /// Recorre los artículos (O(m)) y calcula los importes. Los precios incluyen IGV:
    /// operación gravada = redondear(total final / 1.18, 2) e IGV = total final − operación gravada.
    /// </summary>
    public static void CalcularImportes(Venta venta)
    {
        decimal totalRegular = 0;
        decimal totalFinal = 0;
        foreach (ItemCarrito articulo in venta.Articulos)
        {
            articulo.PrecioAplicado = PrecioSegunTarjeta(articulo, venta.Tarjeta);   // queda guardado en la boleta
            totalRegular += articulo.SubtotalRegular;
            totalFinal += articulo.SubtotalAplicado;
        }

        venta.TotalRegular = totalRegular;
        venta.TotalFinal = totalFinal;
        venta.Ahorro = totalRegular - totalFinal;                 // nunca negativo: precio Ripley ≤ precio regular
        venta.OperacionGravada = Redondear(totalFinal / FactorIgv);
        venta.Igv = totalFinal - venta.OperacionGravada;          // no se vuelve a sumar IGV al total
    }

    /// <summary>Política de redondeo única: 2 decimales, el punto medio se aleja del cero (152.515 → 152.52).</summary>
    private static decimal Redondear(decimal monto)
    {
        return Math.Round(monto, 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>Estimación para Preventa: cuánto pagaría con Tarjeta Ripley (no modifica ninguna compra).</summary>
    public decimal EstimarTotalTarjetaRipley(IEnumerable<ItemCarrito> articulos)
    {
        decimal total = 0;
        foreach (ItemCarrito articulo in articulos)
        {
            total += PrecioSegunTarjeta(articulo, TipoTarjeta.TarjetaRipley) * articulo.Cantidad;
        }
        return total;
    }

    // ============== Corrección de la compra en atención (la hace CarritoService) ==============

    public bool CorregirCantidad(string codigo, int nuevaCantidad, out string mensaje)
    {
        if (CompraEnAtencion == null || correccion == null)
        {
            mensaje = "No hay una compra en atención.";
            return false;
        }
        if (!carritos.CambiarCantidad(correccion, codigo, nuevaCantidad, out mensaje))
        {
            return false;
        }
        SincronizarCorreccion("Corregir cantidad", mensaje);
        return true;
    }

    public bool QuitarArticulo(string codigo, out string mensaje)
    {
        if (CompraEnAtencion == null || correccion == null)
        {
            mensaje = "No hay una compra en atención.";
            return false;
        }
        if (!carritos.QuitarPrenda(correccion, codigo, out mensaje))
        {
            return false;
        }
        SincronizarCorreccion("Quitar artículo", mensaje);
        return true;
    }

    public bool DeshacerCorreccion(out string mensaje)
    {
        if (CompraEnAtencion == null || correccion == null)
        {
            mensaje = "No hay una compra en atención.";
            return false;
        }
        if (!carritos.Deshacer(correccion, out mensaje))
        {
            return false;
        }
        SincronizarCorreccion("Deshacer corrección", mensaje);
        return true;
    }

    /// <summary>La compra toma una copia nueva del carrito corregido y se recalcula.</summary>
    private void SincronizarCorreccion(string accion, string detalle)
    {
        Venta venta = CompraEnAtencion!;
        venta.Articulos = carritos.CopiarArticulos(correccion!);
        CalcularImportes(venta);
        Registrar(accion, $"Turno {venta.Turno}", detalle + $" Nuevo total final: {InputHelper.FormatearSoles(venta.TotalFinal)}.");
    }

    /// <summary>El cliente se va sin pagar (por ejemplo, ya no hay stock). No se registra venta ni boleta.</summary>
    public bool RetirarCompraEnAtencion(out string mensaje)
    {
        Venta? venta = CompraEnAtencion;
        if (venta == null)
        {
            mensaje = "No hay una compra en atención.";
            return false;
        }
        CompraEnAtencion = null;
        correccion = null;
        mensaje = $"El turno {venta.Turno} se retiró sin pagar. No se emitió boleta ni se descontó stock.";
        Registrar("Retirar compra", $"Turno {venta.Turno}", mensaje);
        return true;
    }

    // ============================ Confirmación ============================

    /// <summary>
    /// Confirma el pago SIMULADO (sin datos de tarjeta ni bancos). Orden:
    /// 1) validar estado, tarjeta, artículos y stock actual;
    /// 2) BoletaService prepara la Boleta y guarda ventas.json con la venta;
    /// 3) CatalogoService guarda productos.json con el stock descontado;
    ///    si esto falla, se restaura ventas.json (compensación) y la compra sigue en atención;
    /// 4) recién entonces cambian las estructuras en memoria: historial, índice, stock y consecutivo.
    /// Así un reintento no descuenta el stock dos veces ni duplica la boleta.
    /// </summary>
    public bool ConfirmarPagoSimulado(out Venta? ventaFinalizada, out string mensaje)
    {
        ventaFinalizada = null;
        Venta? venta = CompraEnAtencion;

        if (venta == null)
        {
            mensaje = "No hay una compra en atención para confirmar.";
            Registrar("Confirmar pago simulado", "-", "Rechazado: " + mensaje);
            return false;
        }
        string referencia = $"Turno {venta.Turno}";
        if (confirmando)
        {
            mensaje = "La confirmación ya se está procesando.";
            return false;
        }
        if (!InputHelper.EsTarjetaPermitida(venta.Tarjeta))
        {
            mensaje = "Selecciona una tarjeta antes de confirmar el pago.";
            Registrar("Confirmar pago simulado", referencia, "Rechazado: " + mensaje);
            return false;
        }
        if (venta.Articulos.Count == 0)
        {
            mensaje = "La compra no tiene productos: corrígela o retírala.";
            Registrar("Confirmar pago simulado", referencia, "Rechazado: " + mensaje);
            return false;
        }

        confirmando = true;
        try
        {
            CalcularImportes(venta);                                   // importes definitivos

            if (!catalogo.HayStockSuficiente(venta.Articulos, out string conflictos))
            {
                mensaje = "Falta stock (otra venta usó unidades mientras el cliente esperaba):\n" + conflictos
                          + "\nCorrija la compra en atención y vuelva a confirmar.";
                Registrar("Confirmar pago simulado", referencia, "Rechazado por stock: " + conflictos.Replace('\n', ' '));
                return false;
            }

            Boleta boleta = boletas.PrepararBoleta(venta);
            if (!boletas.GuardarConNuevaVenta(venta, boleta, out string errorVentas))
            {
                mensaje = "No se registró la venta: " + errorVentas + " La compra sigue en atención; puedes reintentar.";
                Registrar("Confirmar pago simulado", referencia, "Error al guardar ventas.json");
                return false;
            }

            bool stockGuardado = catalogo.GuardarConDescuento(venta.Articulos, out string errorStock);
            if (!stockGuardado)
            {
                if (boletas.RestaurarArchivoVentas(venta, out _))
                {
                    mensaje = "No se pudo guardar el stock (" + errorStock + "). Se restauró ventas.json: la venta no quedó registrada y sigue en atención; puedes reintentar.";
                    Registrar("Confirmar pago simulado", referencia, "Error al guardar productos.json; ventas.json restaurado");
                    return false;
                }
                // ventas.json ya contiene la venta y no se pudo restaurar: se confirma para no duplicarla.
                catalogo.MarcarStockPendiente();
            }

            boletas.PublicarVenta(venta);                 // historial, índice AVL y siguiente número
            catalogo.AplicarDescuento(venta.Articulos);   // descuento de stock en memoria, una sola vez
            CompraEnAtencion = null;
            correccion = null;
            ventaFinalizada = venta;

            mensaje = $"Pago simulado confirmado. Boleta N° {boleta.Numero:D6} emitida para el turno {venta.Turno}.";
            if (!stockGuardado)
            {
                mensaje += " ADVERTENCIA: la venta quedó registrada, pero productos.json no se pudo guardar (" + errorStock
                           + "). El stock correcto está en memoria y se volverá a guardar en la siguiente venta.";
            }
            Registrar("Confirmar pago simulado", referencia, mensaje);
            return true;
        }
        finally
        {
            confirmando = false;
        }
    }

    private void Registrar(string accion, string referencia, string resultado)
    {
        Bitacora.AgregarAlFinal(InputHelper.CrearRegistro(accion, referencia, resultado));
    }
}
```

---

## 20. `RipleyComprasMAUI/Services/BoletaService.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Text;
using RipleyComprasMAUI.Data;
using RipleyComprasMAUI.Helpers;
using RipleyComprasMAUI.Models;
using RipleyComprasMAUI.Structures;

namespace RipleyComprasMAUI.Services;

/// <summary>
/// Postventa: genera la Boleta de cada venta finalizada, registra la venta (con su Boleta) en
/// ventas.json y permite consultar por número (árbol AVL), por DNI y por fecha (recorridos de la lista).
/// No implementa el carrito ni el cálculo comercial: usa los importes que dejó CompraService.
/// </summary>
public class BoletaService
{
    public const string Subproceso = "Postventa - Boleta";
    private const int AnchoBoleta = 46;

    private readonly JsonDataManager datos;
    private readonly ListaEnlazada<Venta> historial = new ListaEnlazada<Venta>();          // ventas finalizadas, en orden de emisión
    private ArbolBusqueda<int, Venta> indice = new ArbolBusqueda<int, Venta>();             // clave: Boleta.Numero
    private readonly List<Venta> rechazadas = new List<Venta>();                           // registros inválidos: se conservan al guardar
    private int siguienteNumero = 1;

    public BoletaService(JsonDataManager datos)
    {
        this.datos = datos;
    }

    public bool ArchivoDisponible { get; private set; }     // false si ventas.json no se pudo leer: no se sobrescribe
    public ListaEnlazada<string> Advertencias { get; } = new ListaEnlazada<string>();
    public ListaEnlazada<string> Bitacora { get; } = new ListaEnlazada<string>();
    public int CantidadVentas => historial.Cantidad;
    public int SiguienteNumero => siguienteNumero;
    public int AlturaIndice => indice.Altura;

    /// <summary>
    /// Carga ventas.json y reconstruye el historial y el índice. El siguiente número es
    /// el máximo Boleta.Numero guardado + 1 (no la cantidad de registros + 1).
    /// </summary>
    public bool Cargar(out string mensaje)
    {
        historial.Limpiar();
        indice = new ArbolBusqueda<int, Venta>();
        rechazadas.Clear();
        Advertencias.Limpiar();
        siguienteNumero = 1;
        ArchivoDisponible = false;

        if (!datos.CargarVentas(out List<Venta> ventas, out string error))
        {
            mensaje = "Ventas no disponibles: " + error + " No se registrarán ventas hasta revisar el archivo.";
            return false;
        }

        int maximo = 0;
        foreach (Venta venta in ventas)
        {
            if (venta is null)
            {
                Advertencias.AgregarAlFinal("Se ignoró un elemento null en ventas.json.");
                continue;
            }
            if (venta.Boleta != null && venta.Boleta.Numero > maximo)
            {
                maximo = venta.Boleta.Numero;      // también se cuentan los rechazados, para no reutilizar números
            }
            if (!EsVentaValida(venta, out string motivo))
            {
                rechazadas.Add(venta);
                Advertencias.AgregarAlFinal($"Venta rechazada ({venta.Id}): {motivo}.");
                continue;
            }
            if (!indice.Insertar(venta.Boleta!.Numero, venta))
            {
                rechazadas.Add(venta);
                Advertencias.AgregarAlFinal($"Boleta N° {venta.Boleta.Numero} repetida: se conservó la primera.");
                continue;
            }
            historial.AgregarAlFinal(venta);
        }

        siguienteNumero = maximo + 1;
        ArchivoDisponible = true;
        mensaje = $"Ventas: {historial.Cantidad} boleta(s) cargadas; siguiente número: {siguienteNumero}.";
        if (!Advertencias.EstaVacia)
        {
            mensaje += $" {Advertencias.Cantidad} registro(s) rechazado(s).";
        }
        return true;
    }

    /// <summary>Coherencia Venta–Boleta al leer el JSON.</summary>
    private static bool EsVentaValida(Venta venta, out string motivo)
    {
        if (venta.Boleta == null)
        {
            motivo = "no tiene Boleta";
            return false;
        }
        if (venta.Boleta.Numero < 1)
        {
            motivo = "número de boleta inválido";
            return false;
        }
        if (venta.Id == Guid.Empty || venta.Boleta.IdVenta != venta.Id)
        {
            motivo = "Boleta.IdVenta no coincide con el Id de la venta";
            return false;
        }
        if (venta.Estado != EstadoVenta.Finalizada)
        {
            motivo = "la venta no está finalizada";
            return false;
        }
        if (venta.Articulos == null || venta.Articulos.Count == 0)
        {
            motivo = "la venta no tiene artículos";
            return false;
        }
        if (!InputHelper.EsTarjetaPermitida(venta.Tarjeta))
        {
            motivo = "tarjeta inválida";
            return false;
        }
        if (venta.Dni != null && !InputHelper.EsDniValido(venta.Dni))
        {
            motivo = "DNI inválido";
            return false;
        }
        motivo = "";
        return true;
    }

    // ============================ Emisión (la coordina CompraService) ============================

    /// <summary>Crea el comprobante con el siguiente número y la fecha real del equipo.</summary>
    public Boleta PrepararBoleta(Venta venta)
    {
        return new Boleta
        {
            Numero = siguienteNumero,
            FechaEmision = DateTime.Now,
            IdVenta = venta.Id,
            Leyenda = Boleta.LeyendaSimulada
        };
    }

    /// <summary>
    /// Guarda ventas.json con todas las ventas más la nueva (ya con su Boleta).
    /// Si falla, la venta vuelve a quedar sin Boleta y en atención.
    /// </summary>
    public bool GuardarConNuevaVenta(Venta venta, Boleta boleta, out string error)
    {
        if (!ArchivoDisponible)
        {
            error = "ventas.json no se pudo leer al iniciar y no se sobrescribirá.";
            return false;
        }

        venta.Boleta = boleta;
        venta.Estado = EstadoVenta.Finalizada;
        List<Venta> lista = CrearListaParaGuardar();
        lista.Add(venta);

        if (datos.GuardarVentas(lista, out error))
        {
            return true;
        }
        venta.Boleta = null;
        venta.Estado = EstadoVenta.EnAtencion;
        return false;
    }

    /// <summary>Compensación: vuelve a escribir ventas.json solo con las ventas ya publicadas.</summary>
    public bool RestaurarArchivoVentas(Venta ventaNoRegistrada, out string error)
    {
        if (!datos.GuardarVentas(CrearListaParaGuardar(), out error))
        {
            return false;
        }
        ventaNoRegistrada.Boleta = null;
        ventaNoRegistrada.Estado = EstadoVenta.EnAtencion;
        return true;
    }

    /// <summary>La venta ya está en el archivo: se agrega al historial y al índice, y avanza el consecutivo.</summary>
    public void PublicarVenta(Venta venta)
    {
        Boleta boleta = venta.Boleta!;
        historial.AgregarAlFinal(venta);                 // O(1)
        indice.Insertar(boleta.Numero, venta);           // O(log n)
        siguienteNumero = boleta.Numero + 1;
        Registrar("Emitir y guardar boleta", $"Boleta {boleta.Numero:D6}",
                  $"Turno {venta.Turno}, {venta.TarjetaTexto}, total final {InputHelper.FormatearSoles(venta.TotalFinal)}, ahorro {InputHelper.FormatearSoles(venta.Ahorro)}.");
    }

    private List<Venta> CrearListaParaGuardar()
    {
        List<Venta> lista = new List<Venta>(historial);  // lista de transporte para JSON
        lista.AddRange(rechazadas);                      // los registros inválidos no se pierden
        return lista;
    }

    // ============================ Consultas ============================

    /// <summary>O(log n): búsqueda en el árbol por número de boleta.</summary>
    public bool BuscarPorNumero(int numero, out Venta? venta, out string mensaje)
    {
        venta = null;
        if (numero < 1)
        {
            mensaje = "Número inválido: debe ser mayor que 0.";
            Registrar("Consultar por número", numero.ToString(), "Rechazado: " + mensaje);
            return false;
        }
        if (!indice.Buscar(numero, out venta) || venta == null)
        {
            mensaje = $"No se encontró la boleta N° {numero}.";
            Registrar("Consultar por número", numero.ToString(), mensaje);
            return false;
        }
        mensaje = $"Boleta N° {numero:D6} encontrada.";
        Registrar("Consultar por número", numero.ToString(), mensaje);
        return true;
    }

    /// <summary>O(n): recorre el historial y devuelve TODAS las ventas con ese DNI exacto.</summary>
    public ListaEnlazada<Venta> BuscarPorDni(string dni, out string mensaje)
    {
        ListaEnlazada<Venta> encontradas = new ListaEnlazada<Venta>();
        if (!InputHelper.EsDniValido(dni))
        {
            mensaje = "DNI inválido: debe tener exactamente 8 dígitos (0-9).";
            Registrar("Consultar por DNI", "-", "Rechazado: " + mensaje);
            return encontradas;
        }

        foreach (Venta venta in historial)
        {
            if (venta.Dni == dni)                         // comparación exacta: no devuelve ventas ajenas
            {
                encontradas.AgregarAlFinal(venta);
            }
        }

        mensaje = encontradas.EstaVacia
            ? $"No hay boletas con el DNI {InputHelper.OcultarDni(dni)}."
            : $"{encontradas.Cantidad} boleta(s) con el DNI {InputHelper.OcultarDni(dni)}.";
        Registrar("Consultar por DNI", InputHelper.OcultarDni(dni), mensaje);
        return encontradas;
    }

    /// <summary>O(n): ventas cuya Boleta.FechaEmision cae en el día indicado.</summary>
    public ListaEnlazada<Venta> BuscarPorFecha(DateTime fecha, out string mensaje)
    {
        ListaEnlazada<Venta> encontradas = new ListaEnlazada<Venta>();
        foreach (Venta venta in historial)
        {
            if (venta.Boleta != null && venta.Boleta.FechaEmision.Date == fecha.Date)
            {
                encontradas.AgregarAlFinal(venta);
            }
        }

        mensaje = encontradas.EstaVacia
            ? $"No hay boletas emitidas el {InputHelper.FormatearFecha(fecha)}."
            : $"{encontradas.Cantidad} boleta(s) emitida(s) el {InputHelper.FormatearFecha(fecha)}.";
        Registrar("Consultar por fecha", InputHelper.FormatearFecha(fecha), mensaje);
        return encontradas;
    }

    /// <summary>O(n): todas las ventas, de la más reciente a la más antigua.</summary>
    public ListaEnlazada<Venta> ListarRecientes()
    {
        ListaEnlazada<Venta> recientes = new ListaEnlazada<Venta>();
        foreach (Venta venta in historial)
        {
            recientes.AgregarAlInicio(venta);            // agregar al inicio invierte el orden
        }
        return recientes;
    }

    /// <summary>
    /// Arma el comprobante completo combinando los dos modelos relacionados: Boleta (número,
    /// fecha, leyenda) y Venta (cliente, artículos con su precio aplicado, tarjeta e importes guardados).
    /// No recalcula nada con los precios actuales del catálogo.
    /// </summary>
    public string ObtenerComprobante(Venta venta)
    {
        Boleta? boleta = venta.Boleta;
        if (boleta == null)
        {
            return "Esta venta todavía no tiene boleta.";
        }

        StringBuilder texto = new StringBuilder();
        string doble = new string('=', AnchoBoleta);
        string simple = new string('-', AnchoBoleta);

        texto.AppendLine(doble);
        texto.AppendLine(Centrar("RIPLEY"));
        texto.AppendLine(Centrar(boleta.Leyenda));
        texto.AppendLine(doble);
        texto.AppendLine(Linea("Boleta N°", boleta.Numero.ToString("D6")));
        texto.AppendLine(Linea("Fecha de emisión", InputHelper.FormatearFechaHora(boleta.FechaEmision)));
        texto.AppendLine(Linea("Venta", $"{venta.Id.ToString().Substring(0, 8)} (turno {venta.Turno})"));
        texto.AppendLine(Linea("Cliente", venta.ClienteTexto));
        texto.AppendLine(Linea("DNI", venta.Dni ?? "no registrado"));
        texto.AppendLine(Linea("Correo", venta.Correo ?? "no registrado"));
        texto.AppendLine(simple);

        foreach (ItemCarrito articulo in venta.Articulos)
        {
            texto.AppendLine($"{articulo.Codigo}  {articulo.Nombre}");
            string detalle = $"  {articulo.Cantidad} x {InputHelper.FormatearSoles(articulo.PrecioAplicado)}";
            string subtotal = InputHelper.FormatearSoles(articulo.SubtotalAplicado);
            texto.AppendLine(detalle.PadRight(AnchoBoleta - subtotal.Length) + subtotal);
        }

        texto.AppendLine(simple);
        texto.AppendLine(Linea("Tarjeta", venta.TarjetaTexto));
        texto.AppendLine(Linea("Total regular", InputHelper.FormatearSoles(venta.TotalRegular)));
        texto.AppendLine(Linea("Ahorro", InputHelper.FormatearSoles(venta.Ahorro)));
        texto.AppendLine(Linea("Op. gravada", InputHelper.FormatearSoles(venta.OperacionGravada)));
        texto.AppendLine(Linea("IGV (18 %)", InputHelper.FormatearSoles(venta.Igv)));
        texto.AppendLine(Linea("TOTAL FINAL", InputHelper.FormatearSoles(venta.TotalFinal)));
        texto.AppendLine(doble);
        texto.Append("Pago simulado: no se realizó ningún cobro real.");

        Registrar("Ver boleta", $"Boleta {boleta.Numero:D6}", "Detalle consultado.");
        return texto.ToString();
    }

    private static string Linea(string etiqueta, string valor)
    {
        string izquierda = etiqueta.PadRight(17) + ": ";
        return izquierda + valor.PadLeft(Math.Max(0, AnchoBoleta - izquierda.Length));
    }

    private static string Centrar(string texto)
    {
        int espacios = Math.Max(0, (AnchoBoleta - texto.Length) / 2);
        return new string(' ', espacios) + texto;
    }

    private void Registrar(string accion, string referencia, string resultado)
    {
        Bitacora.AgregarAlFinal(InputHelper.CrearRegistro(accion, referencia, resultado));
    }
}
```

---

## 21. `RipleyComprasMAUI/Data/JsonDataManager.cs`

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using RipleyComprasMAUI.Models;

namespace RipleyComprasMAUI.Data;

/// <summary>
/// Única clase de persistencia (adaptada de AlmacenDatos.cs): rutas y lectura/escritura JSON
/// de productos.json y ventas.json en la carpeta privada de la aplicación. Solo guarda modelos
/// simples; los nodos de las estructuras nunca se serializan.
/// No depende de MAUI: recibe la carpeta y una función que abre la semilla empaquetada.
/// </summary>
public class JsonDataManager
{
    private const string ArchivoProductos = "productos.json";
    private const string ArchivoVentas = "ventas.json";

    private static readonly JsonSerializerOptions Opciones = new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),   // las tildes y la ñ se guardan legibles
        Converters = { new JsonStringEnumConverter() }            // guarda "TarjetaRipley" en lugar de 1
    };

    private readonly Func<Task<Stream>> abrirSemillaProductos;

    public JsonDataManager(string carpetaDatos, Func<Task<Stream>> abrirSemillaProductos)
    {
        CarpetaDatos = carpetaDatos;
        this.abrirSemillaProductos = abrirSemillaProductos;
    }

    public string CarpetaDatos { get; }
    public string RutaProductos => Path.Combine(CarpetaDatos, ArchivoProductos);
    public string RutaVentas => Path.Combine(CarpetaDatos, ArchivoVentas);

    /// <summary>
    /// Inicialización controlada: solo crea lo que falta. En el primer inicio copia la semilla
    /// Resources/Raw/productos.json y crea ventas.json con []. Nunca sobrescribe archivos existentes.
    /// </summary>
    public async Task<(bool Correcto, string Mensaje)> PrepararArchivosAsync()
    {
        try
        {
            Directory.CreateDirectory(CarpetaDatos);
            string creados = "";

            if (!File.Exists(RutaProductos))
            {
                using Stream semilla = await abrirSemillaProductos();
                using StreamReader lector = new StreamReader(semilla);
                string json = await lector.ReadToEndAsync();
                EscribirConReemplazo(RutaProductos, json);
                creados += " productos.json (copiado de la semilla)";
            }
            if (!File.Exists(RutaVentas))
            {
                EscribirConReemplazo(RutaVentas, "[]");
                creados += " ventas.json (vacío)";
            }

            string mensaje = creados.Length == 0
                ? "Se usaron los archivos de datos existentes."
                : "Primer inicio: se crearon" + creados + ".";
            return (true, mensaje);
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
        {
            return (false, "No se pudo preparar la carpeta de datos: " + ex.Message);
        }
    }

    public bool CargarProductos(out List<Producto> productos, out string error)
    {
        return Cargar(RutaProductos, out productos, out error);
    }

    public bool GuardarProductos(List<Producto> productos, out string error)
    {
        return Guardar(RutaProductos, productos, out error);
    }

    public bool CargarVentas(out List<Venta> ventas, out string error)
    {
        return Cargar(RutaVentas, out ventas, out error);
    }

    public bool GuardarVentas(List<Venta> ventas, out string error)
    {
        return Guardar(RutaVentas, ventas, out error);
    }

    /// <summary>
    /// Lee una lista JSON. Un archivo con [] es una colección vacía válida; un archivo vacío,
    /// con null o dañado se informa como error y NO se reemplaza por [].
    /// </summary>
    private static bool Cargar<T>(string ruta, out List<T> datos, out string error)
    {
        datos = new List<T>();
        string nombre = Path.GetFileName(ruta);

        try
        {
            if (!File.Exists(ruta))
            {
                error = $"No existe {nombre} en la carpeta de datos.";
                return false;
            }

            string json = File.ReadAllText(ruta);
            if (string.IsNullOrWhiteSpace(json))
            {
                error = $"{nombre} está vacío (no contiene JSON). Se conservó sin cambios; revíselo.";
                return false;
            }

            List<T>? leidos = JsonSerializer.Deserialize<List<T>>(json, Opciones);
            if (leidos == null)
            {
                error = $"{nombre} contiene null en lugar de una lista. Se conservó sin cambios.";
                return false;
            }

            datos = leidos;
            error = "";
            return true;
        }
        catch (JsonException ex)
        {
            error = $"{nombre} está dañado y no se modificó ({ex.Message}).";
            return false;
        }
        catch (IOException ex)
        {
            error = $"No se pudo leer {nombre}: {ex.Message}";
            return false;
        }
        catch (UnauthorizedAccessException ex)
        {
            error = $"Sin permiso para leer {nombre}: {ex.Message}";
            return false;
        }
    }

    private static bool Guardar<T>(string ruta, List<T> datos, out string error)
    {
        try
        {
            string json = JsonSerializer.Serialize(datos, Opciones);
            EscribirConReemplazo(ruta, json);
            error = "";
            return true;
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
        {
            error = $"No se pudo guardar {Path.GetFileName(ruta)}: {ex.Message}";
            return false;
        }
    }

    /// <summary>
    /// Escribe primero un archivo temporal y luego reemplaza el original: si algo falla
    /// a mitad de la escritura, el archivo anterior no queda incompleto.
    /// </summary>
    private static void EscribirConReemplazo(string ruta, string contenido)
    {
        string temporal = ruta + ".tmp";
        File.WriteAllText(temporal, contenido);
        File.Move(temporal, ruta, true);
    }
}
```

---

## 22. `RipleyComprasMAUI/Helpers/InputHelper.cs`

```csharp
using System;
using System.Globalization;
using System.Text;
using RipleyComprasMAUI.Models;

namespace RipleyComprasMAUI.Helpers;

/// <summary>
/// Validación tipada de entradas con mensajes en español, normalización para buscar por nombre
/// y formato de importes y fechas. Ningún método modifica datos ni "corrige" en silencio lo que
/// el usuario escribió: si el texto no es válido, devuelve false y un mensaje.
/// </summary>
public static class InputHelper
{
    private static readonly CultureInfo Cultura = CultureInfo.InvariantCulture;

    public const int LongitudCodigo = 13;
    public const int LongitudDni = 8;
    public const int CantidadMinima = 1;
    public const int CantidadMaxima = 99;
    public const decimal PrecioMaximo = 99999.99m;

    // ============================ Enteros ============================

    /// <summary>
    /// Lee un entero con int.TryParse. Rechaza decimales ("1.5"), letras y valores fuera de rango;
    /// "-1" no se convierte en "1": se informa que está fuera de rango.
    /// </summary>
    public static bool TryLeerEntero(string? texto, string campo, int minimo, int maximo, out int valor, out string error)
    {
        valor = 0;
        string limpio = (texto ?? "").Trim();

        if (limpio.Length == 0)
        {
            error = $"{campo}: escriba un número entero.";
            return false;
        }
        if (!EsEnteroConSigno(limpio))
        {
            error = $"{campo}: debe ser un número entero (sin decimales, espacios ni letras).";
            return false;
        }
        if (!int.TryParse(limpio, NumberStyles.AllowLeadingSign, Cultura, out valor))
        {
            error = $"{campo}: el número es demasiado grande.";      // desbordamiento de int
            return false;
        }
        if (valor < minimo || valor > maximo)
        {
            error = $"{campo}: debe estar entre {minimo} y {maximo}.";
            return false;
        }

        error = "";
        return true;
    }

    // ============================ Decimales y double ============================

    /// <summary>Importe positivo con máximo dos decimales; acepta coma o punto como separador.</summary>
    public static bool TryLeerDecimal(string? texto, string campo, decimal minimo, decimal maximo, out decimal valor, out string error)
    {
        valor = 0;
        if (!TryPrepararNumero(texto, campo, 2, out string normalizado, out error))
        {
            return false;
        }
        if (!decimal.TryParse(normalizado, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, Cultura, out valor))
        {
            error = $"{campo}: el número es demasiado grande.";
            return false;
        }
        if (valor < minimo || valor > maximo)
        {
            error = $"{campo}: debe estar entre {minimo.ToString("0.00", Cultura)} y {maximo.ToString("0.00", Cultura)}.";
            return false;
        }

        error = "";
        return true;
    }

    /// <summary>Valor no monetario (por ejemplo, el peso de un enlace del grafo). Rechaza NaN e infinito.</summary>
    public static bool TryLeerDouble(string? texto, string campo, double minimo, double maximo, int maxDecimales, out double valor, out string error)
    {
        valor = 0;
        if (!TryPrepararNumero(texto, campo, maxDecimales, out string normalizado, out error))
        {
            return false;
        }
        if (!double.TryParse(normalizado, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, Cultura, out valor)
            || !double.IsFinite(valor))
        {
            error = $"{campo}: no es un número válido.";
            return false;
        }
        if (valor < minimo || valor > maximo)
        {
            error = $"{campo}: debe estar entre {minimo.ToString("0.##", Cultura)} y {maximo.ToString("0.##", Cultura)}.";
            return false;
        }

        error = "";
        return true;
    }

    /// <summary>
    /// Acepta un signo inicial opcional, dígitos y como máximo un separador decimal (coma o punto,
    /// que se normaliza a punto). No interpreta separadores de miles: "1,234" tiene 3 decimales y se rechaza.
    /// </summary>
    private static bool TryPrepararNumero(string? texto, string campo, int maxDecimales, out string normalizado, out string error)
    {
        normalizado = "";
        string limpio = (texto ?? "").Trim();
        if (limpio.Length == 0)
        {
            error = $"{campo}: escriba un número.";
            return false;
        }

        StringBuilder resultado = new StringBuilder();
        int separadores = 0;
        int digitosEnteros = 0;
        int decimales = 0;

        for (int i = 0; i < limpio.Length; i++)
        {
            char c = limpio[i];
            if ((c == '-' || c == '+') && i == 0)
            {
                resultado.Append(c);
                continue;
            }
            if (c == '.' || c == ',')
            {
                separadores++;
                if (separadores > 1)
                {
                    error = $"{campo}: use un solo separador decimal y no use separador de miles.";
                    return false;
                }
                resultado.Append('.');
                continue;
            }
            if (!EsDigitoAscii(c))
            {
                error = $"{campo}: solo se admiten dígitos y un separador decimal (punto o coma).";
                return false;
            }
            if (separadores == 0)
            {
                digitosEnteros++;
            }
            else
            {
                decimales++;
            }
            resultado.Append(c);
        }

        if (digitosEnteros == 0)
        {
            error = $"{campo}: escriba al menos un dígito antes del separador (por ejemplo, 0.5).";
            return false;
        }
        if (separadores == 1 && decimales == 0)
        {
            error = $"{campo}: complete los decimales después del separador.";
            return false;
        }
        if (decimales > maxDecimales)
        {
            error = $"{campo}: admite como máximo {maxDecimales} decimales.";
            return false;
        }

        normalizado = resultado.ToString();
        error = "";
        return true;
    }

    /// <summary>Precio leído del JSON: mayor que 0, con máximo dos decimales.</summary>
    public static bool EsImporteValido(decimal valor)
    {
        return valor > 0 && valor <= PrecioMaximo && decimal.Round(valor, 2) == valor;
    }

    // ============================ Código, DNI, correo y nombres ============================

    /// <summary>Código de prenda: exactamente 13 dígitos ASCII (se maneja como texto, no como int).</summary>
    public static bool EsCodigoValido(string? codigo)
    {
        return codigo != null && codigo.Length == LongitudCodigo && SoloDigitosAscii(codigo);
    }

    public static bool TryValidarCodigo(string? texto, out string codigo, out string error)
    {
        codigo = (texto ?? "").Trim();
        if (codigo.Length == 0)
        {
            error = "Escriba el código de la prenda (13 dígitos).";
            return false;
        }
        if (!EsCodigoValido(codigo))
        {
            error = $"Código inválido: debe tener exactamente 13 dígitos (0-9); tiene {codigo.Length} caracteres.";
            return false;
        }

        error = "";
        return true;
    }

    /// <summary>DNI: exactamente 8 dígitos ASCII, guardado como string para no perder ceros iniciales.</summary>
    public static bool EsDniValido(string? dni)
    {
        return dni != null && dni.Length == LongitudDni && SoloDigitosAscii(dni);
    }

    public static bool TryValidarDni(string? texto, out string dni, out string error)
    {
        dni = (texto ?? "").Trim();
        if (dni.Length == 0)
        {
            error = "Escriba el DNI de 8 dígitos.";
            return false;
        }
        if (!EsDniValido(dni))
        {
            error = "DNI inválido: debe tener exactamente 8 dígitos (0-9).";
            return false;
        }

        error = "";
        return true;
    }

    /// <summary>DNI opcional: vacío significa que el cliente no lo registró.</summary>
    public static bool TryValidarDniOpcional(string? texto, out string? dni, out string error)
    {
        dni = null;
        if (string.IsNullOrWhiteSpace(texto))
        {
            error = "";
            return true;
        }
        if (!TryValidarDni(texto, out string valido, out error))
        {
            return false;
        }
        dni = valido;
        return true;
    }

    /// <summary>Correo opcional con una validación básica: una @, dominio con punto y sin espacios.</summary>
    public static bool TryValidarCorreo(string? texto, out string? correo, out string error)
    {
        correo = null;
        string limpio = (texto ?? "").Trim();
        if (limpio.Length == 0)
        {
            error = "";
            return true;
        }
        if (limpio.Length > 80)
        {
            error = "Correo inválido: admite como máximo 80 caracteres.";
            return false;
        }

        int arroba = limpio.IndexOf('@');
        bool unaSolaArroba = arroba > 0 && arroba == limpio.LastIndexOf('@');
        string dominio = unaSolaArroba ? limpio.Substring(arroba + 1) : "";
        bool dominioValido = dominio.Contains('.') && !dominio.StartsWith('.') && !dominio.EndsWith('.') && !dominio.Contains("..");

        if (!unaSolaArroba || !dominioValido || !SoloCaracteresDeCorreo(limpio))
        {
            error = "Correo inválido: use el formato nombre@dominio.com, sin espacios ni tildes.";
            return false;
        }

        correo = limpio;
        error = "";
        return true;
    }

    /// <summary>
    /// Nombre de persona opcional: letras (con tildes y ñ), espacios, guion y apóstrofo.
    /// Vacío significa compra anónima.
    /// </summary>
    public static bool TryValidarNombrePersona(string? texto, out string? nombre, out string error)
    {
        nombre = null;
        string limpio = UnirEspacios(texto);
        if (limpio.Length == 0)
        {
            error = "";
            return true;
        }
        if (limpio.Length < 2 || limpio.Length > 60)
        {
            error = "Nombre: debe tener entre 2 y 60 caracteres.";
            return false;
        }
        if (!char.IsLetter(limpio[0]))
        {
            error = "Nombre: debe empezar con una letra.";
            return false;
        }
        foreach (char c in limpio)
        {
            bool permitido = char.IsLetter(c) || c == ' ' || c == '-' || c == '\'' || c == '’' || EsMarcaDiacritica(c);
            if (!permitido)
            {
                error = $"Nombre: el carácter '{c}' no está permitido (use letras, espacios, guion o apóstrofo).";
                return false;
            }
        }

        nombre = limpio;
        error = "";
        return true;
    }

    /// <summary>Nombre de prenda (al cargar el JSON): puede tener números; sin caracteres de control.</summary>
    public static bool TryValidarNombreProducto(string? texto, out string nombre, out string error)
    {
        nombre = UnirEspacios(texto);
        if (nombre.Length == 0 || nombre.Length > 60)
        {
            error = "el nombre está vacío o supera 60 caracteres";
            return false;
        }
        foreach (char c in nombre)
        {
            if (char.IsControl(c))
            {
                error = "el nombre tiene caracteres de control";
                return false;
            }
        }

        error = "";
        return true;
    }

    /// <summary>
    /// Texto para buscar prendas por nombre. Admite letras con tilde, dígitos y algunos signos.
    /// Una entrada vacía o solo con espacios no devuelve todo el catálogo: devuelve un mensaje.
    /// </summary>
    public static bool TryValidarTextoBusqueda(string? texto, out string consulta, out string error)
    {
        consulta = UnirEspacios(texto);
        if (consulta.Length == 0)
        {
            error = "Escriba un nombre o parte del nombre para buscar.";
            return false;
        }
        if (consulta.Length > 40)
        {
            error = "La búsqueda admite como máximo 40 caracteres.";
            return false;
        }
        foreach (char c in consulta)
        {
            bool permitido = char.IsLetterOrDigit(c) || c == ' ' || EsMarcaDiacritica(c) || "-./&'()#".IndexOf(c) >= 0;
            if (!permitido)
            {
                error = $"La búsqueda no admite el carácter '{c}'.";
                return false;
            }
        }

        error = "";
        return true;
    }

    // ============================ Tarjeta ============================

    /// <summary>La tarjeta sale de una selección (Picker), nunca de texto libre.</summary>
    public static bool TryLeerTarjeta(int indiceSeleccionado, out TipoTarjeta tarjeta, out string error)
    {
        switch (indiceSeleccionado)
        {
            case 0:
                tarjeta = TipoTarjeta.TarjetaRipley;
                error = "";
                return true;
            case 1:
                tarjeta = TipoTarjeta.OtraTarjeta;
                error = "";
                return true;
            default:
                tarjeta = TipoTarjeta.SinSeleccionar;
                error = "Seleccione Tarjeta Ripley u Otra tarjeta.";
                return false;
        }
    }

    public static bool EsTarjetaPermitida(TipoTarjeta tarjeta)
    {
        return tarjeta == TipoTarjeta.TarjetaRipley || tarjeta == TipoTarjeta.OtraTarjeta;
    }

    // ============================ Búsqueda sin mayúsculas ni tildes ============================

    /// <summary>
    /// Prepara un texto SOLO para comparar: descompone los caracteres (FormD, "ó" = "o" + tilde),
    /// descarta las marcas diacríticas, pasa a mayúsculas y deja un solo espacio entre palabras.
    /// Así "Poló rojo", "  póló   RÓJO  " y "POLO ROJO" quedan iguales. El nombre guardado no cambia.
    /// </summary>
    public static string NormalizarParaBusqueda(string? texto)
    {
        string descompuesto = (texto ?? "").Normalize(NormalizationForm.FormD);
        StringBuilder resultado = new StringBuilder(descompuesto.Length);
        bool espacioPendiente = false;

        foreach (char c in descompuesto)
        {
            if (EsMarcaDiacritica(c))
            {
                continue;                                    // se descarta la tilde o diéresis
            }
            if (char.IsWhiteSpace(c))
            {
                espacioPendiente = resultado.Length > 0;     // los espacios iniciales se ignoran
                continue;
            }
            if (espacioPendiente)
            {
                resultado.Append(' ');                       // varios espacios seguidos cuentan como uno
                espacioPendiente = false;
            }
            resultado.Append(char.ToUpperInvariant(c));
        }

        return resultado.ToString();
    }

    /// <summary>¿El nombre contiene la consulta como palabras consecutivas, sin distinguir mayúsculas ni tildes?</summary>
    public static bool NombreContiene(string? nombre, string? consulta)
    {
        string consultaNormal = NormalizarParaBusqueda(consulta);
        if (consultaNormal.Length == 0)
        {
            return false;
        }
        return NormalizarParaBusqueda(nombre).Contains(consultaNormal, StringComparison.Ordinal);
    }

    // ============================ Formato ============================

    public static string FormatearSoles(decimal monto)
    {
        return "S/ " + monto.ToString("0.00", Cultura);
    }

    public static string FormatearFechaHora(DateTime fecha)
    {
        return fecha.ToString("dd/MM/yyyy HH:mm:ss", Cultura);
    }

    public static string FormatearFecha(DateTime fecha)
    {
        return fecha.ToString("dd/MM/yyyy", Cultura);
    }

    /// <summary>Para listados: 70000021 → 7xxxxx21. El detalle de la boleta muestra el DNI completo.</summary>
    public static string OcultarDni(string? dni)
    {
        if (dni == null || !EsDniValido(dni))
        {
            return "sin DNI";
        }
        return dni[0] + new string('x', 5) + dni.Substring(6);
    }

    /// <summary>Línea de bitácora: fecha y hora | acción | turno o carrito | resultado.</summary>
    public static string CrearRegistro(string accion, string referencia, string resultado)
    {
        return $"{FormatearFechaHora(DateTime.Now)} | {accion} | {referencia} | {resultado}";
    }

    // ============================ Auxiliares ============================

    private static bool EsDigitoAscii(char c)
    {
        return c >= '0' && c <= '9';
    }

    private static bool SoloDigitosAscii(string texto)
    {
        foreach (char c in texto)
        {
            if (!EsDigitoAscii(c))
            {
                return false;
            }
        }
        return true;
    }

    private static bool EsEnteroConSigno(string texto)
    {
        int inicio = (texto[0] == '-' || texto[0] == '+') ? 1 : 0;
        if (inicio == texto.Length)
        {
            return false;                                    // solo un signo
        }
        for (int i = inicio; i < texto.Length; i++)
        {
            if (!EsDigitoAscii(texto[i]))
            {
                return false;
            }
        }
        return true;
    }

    private static bool SoloCaracteresDeCorreo(string texto)
    {
        foreach (char c in texto)
        {
            bool permitido = char.IsAsciiLetterOrDigit(c) || c == '@' || c == '.' || c == '_' || c == '-' || c == '+';
            if (!permitido)
            {
                return false;
            }
        }
        return true;
    }

    private static bool EsMarcaDiacritica(char c)
    {
        return CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark;
    }

    /// <summary>Quita espacios de los extremos y deja un solo espacio entre palabras (conserva tildes y mayúsculas).</summary>
    private static string UnirEspacios(string? texto)
    {
        StringBuilder resultado = new StringBuilder();
        bool espacioPendiente = false;
        foreach (char c in texto ?? "")
        {
            if (char.IsWhiteSpace(c))
            {
                espacioPendiente = resultado.Length > 0;
                continue;
            }
            if (espacioPendiente)
            {
                resultado.Append(' ');
                espacioPendiente = false;
            }
            resultado.Append(c);
        }
        return resultado.ToString();
    }
}
```

---

## 23. `RipleyComprasMAUI/UI/MainPage.xaml`

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:modelos="clr-namespace:RipleyComprasMAUI.Models"
             x:Class="RipleyComprasMAUI.UI.MainPage"
             Title="Ripley">

    <Grid RowDefinitions="Auto,Auto,*">

        <!-- ===================== Encabezado y menú ===================== -->
        <Grid Grid.Row="0" Padding="20,10" ColumnDefinitions="Auto,*" ColumnSpacing="28"
              BackgroundColor="{StaticResource ColorPrimarioOscuro}">
            <Label Text="Ripley" FontSize="30" FontFamily="OpenSansSemibold" TextColor="White" VerticalOptions="Center" />
            <HorizontalStackLayout Grid.Column="1" Spacing="6" VerticalOptions="Center">
                <Button x:Name="btnNavPreventa" Text="Preventa" Style="{StaticResource BotonMenu}" Clicked="OnNavegarClicked" />
                <Button x:Name="btnNavVenta" Text="Venta" Style="{StaticResource BotonMenu}" Clicked="OnNavegarClicked" />
                <Button x:Name="btnNavBoleta" Text="Boleta" Style="{StaticResource BotonMenu}" Clicked="OnNavegarClicked" />
                <Button x:Name="btnNavBitacora" Text="Bitácora" Style="{StaticResource BotonMenu}" Clicked="OnNavegarClicked" />
                <Button x:Name="btnNavRed" Text="Red de cajas" Style="{StaticResource BotonMenu}" Clicked="OnNavegarClicked" />
            </HorizontalStackLayout>
        </Grid>

        <!-- ===================== Mensaje de la última acción ===================== -->
        <Border x:Name="brdMensaje" Grid.Row="1" Margin="20,10,20,0" Padding="14,8" Style="{StaticResource Tarjeta}">
            <Label x:Name="lblMensaje" Text="Cargando..." />
        </Border>

        <Grid Grid.Row="2">

            <!-- ===================================================================== -->
            <!-- PREVENTA: búsqueda y catálogo a la izquierda, carrito separado a la derecha -->
            <!-- ===================================================================== -->
            <ScrollView x:Name="seccionPreventa">
                <Grid Padding="20,14" ColumnDefinitions="3*,2*" RowDefinitions="Auto,*" ColumnSpacing="18" RowSpacing="10">
                    <Label Grid.ColumnSpan="2" Text="Preventa" Style="{StaticResource EtiquetaTitulo}" />

                    <Border Grid.Row="1" Style="{StaticResource Tarjeta}" VerticalOptions="Start">
                        <VerticalStackLayout Spacing="8">
                            <Label Text="Buscar por código" Style="{StaticResource EtiquetaSubtitulo}" />
                            <Grid ColumnDefinitions="*,Auto" ColumnSpacing="8">
                                <Entry x:Name="txtCodigo" Placeholder="Código de 13 dígitos" Keyboard="Numeric"
                                       TextChanged="OnCodigoCambiado" Completed="OnBuscarCodigoClicked" />
                                <Button x:Name="btnBuscarCodigo" Grid.Column="1" Text="Buscar" WidthRequest="120" IsEnabled="False" Clicked="OnBuscarCodigoClicked" />
                            </Grid>
                            <Label x:Name="lblErrorCodigo" Style="{StaticResource EtiquetaError}" IsVisible="False" />

                            <Label Text="Buscar por nombre" Style="{StaticResource EtiquetaSubtitulo}" />
                            <Grid ColumnDefinitions="*,Auto" ColumnSpacing="8">
                                <Entry x:Name="txtNombreBuscar" Placeholder="Nombre o parte del nombre"
                                       TextChanged="OnNombreBuscarCambiado" Completed="OnBuscarNombreClicked" />
                                <Button x:Name="btnBuscarNombre" Grid.Column="1" Text="Buscar" WidthRequest="120" IsEnabled="False" Clicked="OnBuscarNombreClicked" />
                            </Grid>
                            <Label x:Name="lblErrorNombreBuscar" Style="{StaticResource EtiquetaError}" IsVisible="False" />

                            <Grid ColumnDefinitions="*,Auto" ColumnSpacing="8">
                                <Label x:Name="lblResultadoBusqueda" VerticalOptions="Center" Style="{StaticResource EtiquetaSuave}" />
                                <Button Grid.Column="1" Text="Ver catálogo" Style="{StaticResource BotonSecundario}" Clicked="OnListarCatalogoClicked" />
                            </Grid>

                            <Border Style="{StaticResource Tarjeta}" Padding="2">
                                <CollectionView x:Name="cvProductos" HeightRequest="300" SelectionMode="Single"
                                                SelectionChanged="OnProductoSeleccionado" EmptyView="Busca un producto o abre el catálogo.">
                                    <CollectionView.ItemTemplate>
                                        <DataTemplate x:DataType="modelos:Producto">
                                            <Grid Style="{StaticResource FilaLista}" ColumnDefinitions="*,Auto" RowDefinitions="Auto,Auto">
                                                <Label Text="{Binding Nombre}" FontFamily="OpenSansSemibold" />
                                                <Label Grid.Row="1" Text="{Binding Codigo}" Style="{StaticResource EtiquetaSuave}" />
                                                <Label Grid.Column="1" HorizontalTextAlignment="End" Text="{Binding PrecioRegular, StringFormat='S/ {0:0.00}'}" />
                                                <Label Grid.Row="1" Grid.Column="1" HorizontalTextAlignment="End" Style="{StaticResource EtiquetaSuave}">
                                                    <Label.FormattedText>
                                                        <FormattedString>
                                                            <Span Text="{Binding PrecioTarjetaRipley, StringFormat='T. Ripley S/ {0:0.00}'}" />
                                                            <Span Text="{Binding Stock, StringFormat=' · {0} disponibles'}" />
                                                        </FormattedString>
                                                    </Label.FormattedText>
                                                </Label>
                                            </Grid>
                                        </DataTemplate>
                                    </CollectionView.ItemTemplate>
                                </CollectionView>
                            </Border>

                            <Label x:Name="lblProductoSeleccionado" FontFamily="OpenSansSemibold" />
                            <Grid ColumnDefinitions="Auto,90,*" ColumnSpacing="10">
                                <Label Text="Cantidad" VerticalOptions="Center" />
                                <Entry x:Name="txtCantidadAgregar" Grid.Column="1" Keyboard="Numeric" TextChanged="OnCantidadAgregarCambiada" />
                                <Button x:Name="btnAgregar" Grid.Column="2" Text="Agregar al carrito" IsEnabled="False" Clicked="OnAgregarClicked" />
                            </Grid>
                            <Label x:Name="lblErrorCantidadAgregar" Style="{StaticResource EtiquetaError}" IsVisible="False" />
                        </VerticalStackLayout>
                    </Border>

                    <Border Grid.Row="1" Grid.Column="1" Style="{StaticResource TarjetaResaltada}" VerticalOptions="Start">
                        <VerticalStackLayout Spacing="8">
                            <Grid ColumnDefinitions="*,Auto">
                                <Label Text="Carrito" Style="{StaticResource EtiquetaTitulo}" />
                                <Label x:Name="lblTituloCarrito" Grid.Column="1" VerticalOptions="Center" Style="{StaticResource EtiquetaSuave}" />
                            </Grid>
                            <Border Style="{StaticResource Tarjeta}" Padding="2">
                                <CollectionView x:Name="cvCarrito" HeightRequest="230" SelectionMode="Single"
                                                SelectionChanged="OnArticuloCarritoSeleccionado" EmptyView="El carrito está vacío.">
                                    <CollectionView.ItemTemplate>
                                        <DataTemplate x:DataType="modelos:ItemCarrito">
                                            <Grid Style="{StaticResource FilaLista}" ColumnDefinitions="*,Auto" RowDefinitions="Auto,Auto">
                                                <Label Text="{Binding Nombre}" FontFamily="OpenSansSemibold" />
                                                <Label Grid.Row="1" Text="{Binding PrecioRegular, StringFormat='S/ {0:0.00} c/u'}" Style="{StaticResource EtiquetaSuave}" />
                                                <Label Grid.Column="1" HorizontalTextAlignment="End" FontFamily="OpenSansSemibold"
                                                       Text="{Binding Cantidad, StringFormat='x {0}'}" />
                                                <Label Grid.Row="1" Grid.Column="1" HorizontalTextAlignment="End"
                                                       Text="{Binding SubtotalRegular, StringFormat='S/ {0:0.00}'}" />
                                            </Grid>
                                        </DataTemplate>
                                    </CollectionView.ItemTemplate>
                                </CollectionView>
                            </Border>

                            <FlexLayout Wrap="Wrap" AlignItems="Center">
                                <Button x:Name="btnMenos" Text="−" WidthRequest="46" Margin="0,0,6,6" IsEnabled="False" Clicked="OnMenosClicked" />
                                <Button x:Name="btnMas" Text="+" WidthRequest="46" Margin="0,0,6,6" IsEnabled="False" Clicked="OnMasClicked" />
                                <Entry x:Name="txtNuevaCantidad" WidthRequest="70" Placeholder="Cant." Keyboard="Numeric" Margin="0,0,6,6"
                                       TextChanged="OnNuevaCantidadCambiada" />
                                <Button x:Name="btnCambiarCantidad" Text="Cambiar" Margin="0,0,6,6" Style="{StaticResource BotonSecundario}" IsEnabled="False" Clicked="OnCambiarCantidadClicked" />
                                <Button x:Name="btnQuitar" Text="Quitar" Margin="0,0,6,6" Style="{StaticResource BotonPeligro}" IsEnabled="False" Clicked="OnQuitarClicked" />
                                <Button x:Name="btnDeshacer" Text="Deshacer" Margin="0,0,6,6" Style="{StaticResource BotonSecundario}" Clicked="OnDeshacerClicked" />
                                <Button x:Name="btnVaciar" Text="Vaciar" Margin="0,0,6,6" Style="{StaticResource BotonSecundario}" IsEnabled="False" Clicked="OnVaciarClicked" />
                            </FlexLayout>
                            <Label x:Name="lblErrorNuevaCantidad" Style="{StaticResource EtiquetaError}" IsVisible="False" />

                            <Grid ColumnDefinitions="*,Auto">
                                <Label Text="Total" VerticalOptions="Center" Style="{StaticResource EtiquetaSubtitulo}" />
                                <Label x:Name="lblTotalRegular" Grid.Column="1" Style="{StaticResource EtiquetaTotal}" />
                            </Grid>
                            <Label x:Name="lblEstimadoRipley" Style="{StaticResource EtiquetaSuave}" HorizontalTextAlignment="End" />

                            <Label Text="Cliente (opcional)" Style="{StaticResource EtiquetaSubtitulo}" Margin="0,6,0,0" />
                            <Entry x:Name="txtClienteNombre" Placeholder="Nombre" TextChanged="OnDatosClienteCambiados" />
                            <Grid ColumnDefinitions="*,2*" ColumnSpacing="8">
                                <Entry x:Name="txtClienteDni" Placeholder="DNI (8 dígitos)" Keyboard="Numeric" TextChanged="OnDatosClienteCambiados" />
                                <Entry x:Name="txtClienteCorreo" Grid.Column="1" Placeholder="Correo" Keyboard="Email" TextChanged="OnDatosClienteCambiados" />
                            </Grid>
                            <Label x:Name="lblErrorCliente" Style="{StaticResource EtiquetaError}" IsVisible="False" />

                            <Button x:Name="btnEnviarCaja" Text="Enviar a caja" Style="{StaticResource BotonDestacado}" IsEnabled="False" Clicked="OnEnviarCajaClicked" />
                        </VerticalStackLayout>
                    </Border>
                </Grid>
            </ScrollView>

            <!-- ===================================================================== -->
            <!-- VENTA: fila a la izquierda; cliente en caja y pago a la derecha        -->
            <!-- ===================================================================== -->
            <ScrollView x:Name="seccionVenta" IsVisible="False">
                <Grid Padding="20,14" ColumnDefinitions="2*,3*" RowDefinitions="Auto,*" ColumnSpacing="18" RowSpacing="10">
                    <Label Grid.ColumnSpan="2" Text="Venta" Style="{StaticResource EtiquetaTitulo}" />

                    <Border Grid.Row="1" Style="{StaticResource Tarjeta}" VerticalOptions="Start">
                        <VerticalStackLayout Spacing="10">
                            <Grid ColumnDefinitions="*,Auto">
                                <Label Text="Fila" Style="{StaticResource EtiquetaSubtitulo}" VerticalOptions="Center" />
                                <Label x:Name="lblInfoFila" Grid.Column="1" Style="{StaticResource EtiquetaSuave}" VerticalOptions="Center" />
                            </Grid>
                            <Button x:Name="btnAtenderSiguiente" Text="Atender siguiente" Style="{StaticResource BotonDestacado}" Clicked="OnAtenderSiguienteClicked" />
                            <Border Style="{StaticResource Tarjeta}" Padding="2">
                                <CollectionView x:Name="cvFila" HeightRequest="420" SelectionMode="None" EmptyView="No hay clientes en espera.">
                                    <CollectionView.ItemTemplate>
                                        <DataTemplate x:DataType="modelos:Venta">
                                            <Grid Style="{StaticResource FilaLista}" ColumnDefinitions="*,Auto" RowDefinitions="Auto,Auto">
                                                <Label FontFamily="OpenSansSemibold">
                                                    <Label.FormattedText>
                                                        <FormattedString>
                                                            <Span Text="{Binding Turno, StringFormat='Turno {0}'}" />
                                                            <Span Text="{Binding ClienteTexto, StringFormat=' · {0}'}" />
                                                        </FormattedString>
                                                    </Label.FormattedText>
                                                </Label>
                                                <Label Grid.Row="1" Text="{Binding Unidades, StringFormat='{0} producto(s)'}" Style="{StaticResource EtiquetaSuave}" />
                                                <Label Grid.Column="1" Grid.RowSpan="2" VerticalOptions="Center" HorizontalTextAlignment="End"
                                                       Text="{Binding TotalRegular, StringFormat='S/ {0:0.00}'}" />
                                            </Grid>
                                        </DataTemplate>
                                    </CollectionView.ItemTemplate>
                                </CollectionView>
                            </Border>
                        </VerticalStackLayout>
                    </Border>

                    <VerticalStackLayout Grid.Row="1" Grid.Column="1" Spacing="14">
                        <Border x:Name="brdSinCompra" Style="{StaticResource Tarjeta}" Padding="24">
                            <Label Text="No hay un cliente en caja. Pulsa Atender siguiente." HorizontalTextAlignment="Center" Style="{StaticResource EtiquetaSubtitulo}" />
                        </Border>

                        <VerticalStackLayout x:Name="panelCompra" Spacing="14" IsVisible="False">
                            <Border Style="{StaticResource Tarjeta}">
                                <VerticalStackLayout Spacing="8">
                                    <Label Text="En caja" Style="{StaticResource EtiquetaSubtitulo}" />
                                    <Label x:Name="lblCompraActual" />
                                    <Border Style="{StaticResource Tarjeta}" Padding="2">
                                        <CollectionView x:Name="cvArticulosCaja" HeightRequest="190" SelectionMode="Single"
                                                        SelectionChanged="OnArticuloCajaSeleccionado" EmptyView="La compra no tiene productos.">
                                            <CollectionView.ItemTemplate>
                                                <DataTemplate x:DataType="modelos:ItemCarrito">
                                                    <Grid Style="{StaticResource FilaLista}" ColumnDefinitions="*,Auto" RowDefinitions="Auto,Auto">
                                                        <Label Text="{Binding Nombre}" FontFamily="OpenSansSemibold" />
                                                        <Label Grid.Row="1" Style="{StaticResource EtiquetaSuave}">
                                                            <Label.FormattedText>
                                                                <FormattedString>
                                                                    <Span Text="{Binding Cantidad, StringFormat='{0} x '}" />
                                                                    <Span Text="{Binding PrecioAplicado, StringFormat='S/ {0:0.00}'}" />
                                                                </FormattedString>
                                                            </Label.FormattedText>
                                                        </Label>
                                                        <Label Grid.Column="1" Grid.RowSpan="2" VerticalOptions="Center" HorizontalTextAlignment="End"
                                                               FontFamily="OpenSansSemibold" Text="{Binding SubtotalAplicado, StringFormat='S/ {0:0.00}'}" />
                                                    </Grid>
                                                </DataTemplate>
                                            </CollectionView.ItemTemplate>
                                        </CollectionView>
                                    </Border>
                                    <Label Text="Corregir compra" Style="{StaticResource EtiquetaSuave}" />
                                    <FlexLayout Wrap="Wrap" AlignItems="Center">
                                        <Entry x:Name="txtCantidadCaja" WidthRequest="70" Placeholder="Cant." Keyboard="Numeric" Margin="0,0,6,6"
                                               TextChanged="OnCantidadCajaCambiada" />
                                        <Button x:Name="btnCorregirCantidad" Text="Cambiar cantidad" Margin="0,0,6,6" Style="{StaticResource BotonSecundario}" IsEnabled="False" Clicked="OnCorregirCantidadClicked" />
                                        <Button x:Name="btnQuitarCaja" Text="Quitar" Margin="0,0,6,6" Style="{StaticResource BotonPeligro}" IsEnabled="False" Clicked="OnQuitarCajaClicked" />
                                        <Button x:Name="btnDeshacerCaja" Text="Deshacer" Margin="0,0,6,6" Style="{StaticResource BotonSecundario}" IsEnabled="False" Clicked="OnDeshacerCajaClicked" />
                                    </FlexLayout>
                                    <Label x:Name="lblErrorCantidadCaja" Style="{StaticResource EtiquetaError}" IsVisible="False" />
                                </VerticalStackLayout>
                            </Border>

                            <Border Style="{StaticResource TarjetaResaltada}">
                                <VerticalStackLayout Spacing="10">
                                    <Label Text="Pago" Style="{StaticResource EtiquetaTitulo}" />
                                    <Label Text="Medio de pago" Style="{StaticResource EtiquetaSubtitulo}" />
                                    <Picker x:Name="pkTarjeta" Title="Selecciona una tarjeta" IsEnabled="False" SelectedIndexChanged="OnTarjetaCambiada">
                                        <Picker.Items>
                                            <x:String>Tarjeta Ripley</x:String>
                                            <x:String>Otra tarjeta</x:String>
                                        </Picker.Items>
                                    </Picker>
                                    <Label x:Name="lblAvisoTarjeta" Style="{StaticResource EtiquetaError}" />

                                    <Grid ColumnDefinitions="*,Auto" RowDefinitions="Auto,Auto,Auto,Auto" RowSpacing="4">
                                        <Label Text="Total regular" />
                                        <Label x:Name="lblCajaRegular" Grid.Column="1" HorizontalTextAlignment="End" />
                                        <Label Grid.Row="1" Text="Ahorro" FontFamily="OpenSansSemibold" TextColor="{StaticResource ColorExito}" />
                                        <Label x:Name="lblCajaAhorro" Grid.Row="1" Grid.Column="1" HorizontalTextAlignment="End"
                                               FontFamily="OpenSansSemibold" TextColor="{StaticResource ColorExito}" />
                                        <Label Grid.Row="2" Text="Operación gravada" Style="{StaticResource EtiquetaSuave}" />
                                        <Label x:Name="lblCajaGravada" Grid.Row="2" Grid.Column="1" HorizontalTextAlignment="End" Style="{StaticResource EtiquetaSuave}" />
                                        <Label Grid.Row="3" Text="IGV (18 %)" Style="{StaticResource EtiquetaSuave}" />
                                        <Label x:Name="lblCajaIgv" Grid.Row="3" Grid.Column="1" HorizontalTextAlignment="End" Style="{StaticResource EtiquetaSuave}" />
                                    </Grid>
                                    <BoxView HeightRequest="1" Color="{StaticResource ColorBorde}" />
                                    <Grid ColumnDefinitions="*,Auto">
                                        <Label Text="Total a pagar" VerticalOptions="Center" Style="{StaticResource EtiquetaSubtitulo}" />
                                        <Label x:Name="lblCajaFinal" Grid.Column="1" Style="{StaticResource EtiquetaTotal}" />
                                    </Grid>
                                    <Button x:Name="btnConfirmarPago" Text="Confirmar pago" Style="{StaticResource BotonDestacado}" IsEnabled="False" Clicked="OnConfirmarPagoClicked" />
                                    <Button x:Name="btnRetirarCompra" Text="Retirar compra" Style="{StaticResource BotonSecundario}" IsEnabled="False" Clicked="OnRetirarCompraClicked" />
                                </VerticalStackLayout>
                            </Border>
                        </VerticalStackLayout>
                    </VerticalStackLayout>
                </Grid>
            </ScrollView>

            <!-- ===================================================================== -->
            <!-- BOLETA: búsquedas y resultados a la izquierda; boleta elegida a la derecha -->
            <!-- ===================================================================== -->
            <ScrollView x:Name="seccionBoleta" IsVisible="False">
                <Grid Padding="20,14" ColumnDefinitions="*,*" RowDefinitions="Auto,*" ColumnSpacing="18" RowSpacing="10">
                    <Label Grid.ColumnSpan="2" Text="Boleta" Style="{StaticResource EtiquetaTitulo}" />

                    <VerticalStackLayout Grid.Row="1" Spacing="12">
                        <Border Style="{StaticResource Tarjeta}">
                            <VerticalStackLayout Spacing="8">
                                <Label Text="Buscar boleta" Style="{StaticResource EtiquetaSubtitulo}" />
                                <Grid ColumnDefinitions="70,*,Auto" RowDefinitions="Auto,Auto,Auto,Auto,Auto" ColumnSpacing="8" RowSpacing="6">
                                    <Label Text="Número" VerticalOptions="Center" />
                                    <Entry x:Name="txtNumeroBoleta" Grid.Column="1" Placeholder="Ej. 1" Keyboard="Numeric"
                                           TextChanged="OnNumeroBoletaCambiado" Completed="OnBuscarNumeroClicked" />
                                    <Button x:Name="btnBuscarNumero" Grid.Column="2" Text="Buscar" WidthRequest="110" IsEnabled="False" Clicked="OnBuscarNumeroClicked" />
                                    <Label x:Name="lblErrorNumero" Grid.Row="1" Grid.Column="1" Grid.ColumnSpan="2" Style="{StaticResource EtiquetaError}" IsVisible="False" />

                                    <Label Grid.Row="2" Text="DNI" VerticalOptions="Center" />
                                    <Entry x:Name="txtDniBuscar" Grid.Row="2" Grid.Column="1" Placeholder="8 dígitos" Keyboard="Numeric"
                                           TextChanged="OnDniBuscarCambiado" Completed="OnBuscarDniClicked" />
                                    <Button x:Name="btnBuscarDni" Grid.Row="2" Grid.Column="2" Text="Buscar" WidthRequest="110" IsEnabled="False" Clicked="OnBuscarDniClicked" />
                                    <Label x:Name="lblErrorDniBuscar" Grid.Row="3" Grid.Column="1" Grid.ColumnSpan="2" Style="{StaticResource EtiquetaError}" IsVisible="False" />

                                    <Label Grid.Row="4" Text="Fecha" VerticalOptions="Center" />
                                    <DatePicker x:Name="dpFecha" Grid.Row="4" Grid.Column="1" Format="dd/MM/yyyy" />
                                    <Button Grid.Row="4" Grid.Column="2" Text="Buscar" WidthRequest="110" Clicked="OnBuscarFechaClicked" />
                                </Grid>
                                <Button Text="Ver todas" Style="{StaticResource BotonSecundario}" HorizontalOptions="Start" Clicked="OnVerTodasClicked" />
                            </VerticalStackLayout>
                        </Border>

                        <Border Style="{StaticResource Tarjeta}">
                            <VerticalStackLayout Spacing="8">
                                <Grid ColumnDefinitions="*,Auto">
                                    <Label Text="Resultados" Style="{StaticResource EtiquetaSubtitulo}" />
                                    <Label x:Name="lblResultadoVentas" Grid.Column="1" Style="{StaticResource EtiquetaSuave}" />
                                </Grid>
                                <CollectionView x:Name="cvVentas" HeightRequest="280" SelectionMode="Single"
                                                SelectionChanged="OnVentaSeleccionada" EmptyView="Sin boletas para mostrar.">
                                    <CollectionView.ItemTemplate>
                                        <DataTemplate x:DataType="modelos:Venta">
                                            <Grid Style="{StaticResource FilaLista}" ColumnDefinitions="*,Auto" RowDefinitions="Auto,Auto">
                                                <Label FontFamily="OpenSansSemibold">
                                                    <Label.FormattedText>
                                                        <FormattedString>
                                                            <Span Text="{Binding Boleta.Numero, StringFormat='N° {0:D6}'}" />
                                                            <Span Text="{Binding ClienteTexto, StringFormat=' · {0}'}" />
                                                        </FormattedString>
                                                    </Label.FormattedText>
                                                </Label>
                                                <Label Grid.Row="1" Style="{StaticResource EtiquetaSuave}">
                                                    <Label.FormattedText>
                                                        <FormattedString>
                                                            <Span Text="{Binding Boleta.FechaEmision, StringFormat='{0:dd/MM/yyyy HH:mm}'}" />
                                                            <Span Text="{Binding TarjetaTexto, StringFormat=' · {0}'}" />
                                                        </FormattedString>
                                                    </Label.FormattedText>
                                                </Label>
                                                <Label Grid.Column="1" Grid.RowSpan="2" VerticalOptions="Center" FontFamily="OpenSansSemibold"
                                                       Text="{Binding TotalFinal, StringFormat='S/ {0:0.00}'}" />
                                            </Grid>
                                        </DataTemplate>
                                    </CollectionView.ItemTemplate>
                                </CollectionView>
                                <Label x:Name="lblInfoBoletas" Style="{StaticResource EtiquetaSuave}" />
                            </VerticalStackLayout>
                        </Border>
                    </VerticalStackLayout>

                    <Border Grid.Row="1" Grid.Column="1" Style="{StaticResource TarjetaResaltada}" VerticalOptions="Start">
                        <VerticalStackLayout Spacing="10">
                            <Label x:Name="lblBoletaSeleccionada" Text="Ninguna boleta seleccionada" Style="{StaticResource EtiquetaTitulo}" />
                            <Border Style="{StaticResource Tarjeta}">
                                <Label x:Name="lblDetalleBoleta" FontFamily="Consolas" FontSize="13" LineBreakMode="NoWrap"
                                       Text="Elige una boleta de los resultados para ver su detalle." />
                            </Border>
                        </VerticalStackLayout>
                    </Border>
                </Grid>
            </ScrollView>

            <!-- ===================================================================== -->
            <!-- BITÁCORA: filtro y registros                                           -->
            <!-- ===================================================================== -->
            <Grid x:Name="seccionBitacora" IsVisible="False" Padding="20,14" RowDefinitions="Auto,Auto,*,Auto" RowSpacing="10">
                <Label Text="Bitácora" Style="{StaticResource EtiquetaTitulo}" />
                <Grid Grid.Row="1" ColumnDefinitions="Auto,380,Auto" ColumnSpacing="10">
                    <Label Text="Mostrar" VerticalOptions="Center" Style="{StaticResource EtiquetaSubtitulo}" />
                    <Picker x:Name="pkBitacora" Grid.Column="1" SelectedIndexChanged="OnFiltroBitacoraCambiado">
                        <Picker.Items>
                            <x:String>Todos los subprocesos</x:String>
                            <x:String>Preventa - Selección de productos</x:String>
                            <x:String>Venta - Pago</x:String>
                            <x:String>Postventa - Boleta</x:String>
                        </Picker.Items>
                    </Picker>
                    <Button Grid.Column="2" Text="Actualizar" Style="{StaticResource BotonSecundario}" Clicked="OnActualizarBitacoraClicked" />
                </Grid>
                <Border Grid.Row="2" Style="{StaticResource Tarjeta}" Padding="4">
                    <CollectionView x:Name="cvBitacora" SelectionMode="None" EmptyView="Sin registros.">
                        <CollectionView.ItemTemplate>
                            <DataTemplate x:DataType="x:String">
                                <Label Text="{Binding .}" Padding="10,4" FontSize="13" />
                            </DataTemplate>
                        </CollectionView.ItemTemplate>
                    </CollectionView>
                </Border>
                <Grid Grid.Row="3" ColumnDefinitions="Auto,*" ColumnSpacing="8">
                    <Label Text="Carpeta de datos" VerticalOptions="Center" Style="{StaticResource EtiquetaSuave}" />
                    <Entry x:Name="txtCarpetaDatos" Grid.Column="1" IsReadOnly="True" FontSize="12" />
                </Grid>
            </Grid>

            <!-- ===================================================================== -->
            <!-- RED DE CAJAS: primero la ruta; los enlaces van en una sección aparte   -->
            <!-- ===================================================================== -->
            <ScrollView x:Name="seccionRed" IsVisible="False">
                <Grid Padding="20,14" ColumnDefinitions="*,*" RowDefinitions="Auto,*" ColumnSpacing="18" RowSpacing="10">
                    <Label Grid.ColumnSpan="2" Text="Red de cajas" Style="{StaticResource EtiquetaTitulo}" />

                    <Border Grid.Row="1" Style="{StaticResource TarjetaResaltada}" VerticalOptions="Start">
                        <VerticalStackLayout Spacing="10">
                            <Label Text="Ruta más corta" Style="{StaticResource EtiquetaSubtitulo}" />
                            <Grid ColumnDefinitions="*,*" RowDefinitions="Auto,Auto" ColumnSpacing="10" RowSpacing="4">
                                <Label Text="Origen" />
                                <Label Grid.Column="1" Text="Destino" />
                                <Picker x:Name="pkOrigen" Grid.Row="1" Title="Elige el origen" />
                                <Picker x:Name="pkDestino" Grid.Row="1" Grid.Column="1" Title="Elige el destino" />
                            </Grid>
                            <Button Text="Calcular ruta" Style="{StaticResource BotonDestacado}" Clicked="OnRutaMinimaClicked" />
                            <Label x:Name="lblRuta" FontSize="15" />
                        </VerticalStackLayout>
                    </Border>

                    <Border Grid.Row="1" Grid.Column="1" Style="{StaticResource Tarjeta}" VerticalOptions="Start">
                        <VerticalStackLayout Spacing="8">
                            <Label Text="Enlaces" Style="{StaticResource EtiquetaSubtitulo}" />
                            <Label Text="Se aplica al enlace entre el origen y el destino elegidos." Style="{StaticResource EtiquetaSuave}" />
                            <FlexLayout Wrap="Wrap" AlignItems="Center">
                                <Button Text="Activar" Margin="0,0,6,6" Style="{StaticResource BotonSecundario}" Clicked="OnActivarEnlaceClicked" />
                                <Button Text="Desactivar" Margin="0,0,6,6" Style="{StaticResource BotonPeligro}" Clicked="OnDesactivarEnlaceClicked" />
                                <Entry x:Name="txtPeso" WidthRequest="110" Placeholder="Tiempo (ms)" Keyboard="Numeric" Margin="0,0,6,6"
                                       TextChanged="OnPesoCambiado" />
                                <Button x:Name="btnCambiarPeso" Text="Cambiar tiempo" Margin="0,0,6,6" Style="{StaticResource BotonSecundario}" IsEnabled="False" Clicked="OnCambiarPesoClicked" />
                            </FlexLayout>
                            <Label x:Name="lblErrorPeso" Style="{StaticResource EtiquetaError}" IsVisible="False" />
                            <Border Style="{StaticResource Tarjeta}" Padding="2">
                                <CollectionView x:Name="cvEnlaces" HeightRequest="260" SelectionMode="None">
                                    <CollectionView.ItemTemplate>
                                        <DataTemplate x:DataType="x:String">
                                            <Label Text="{Binding .}" Padding="10,5" />
                                        </DataTemplate>
                                    </CollectionView.ItemTemplate>
                                </CollectionView>
                            </Border>
                            <Button Text="Restaurar red" Style="{StaticResource BotonSecundario}" HorizontalOptions="Start" Clicked="OnRestablecerGrafoClicked" />
                        </VerticalStackLayout>
                    </Border>
                </Grid>
            </ScrollView>
        </Grid>
    </Grid>
</ContentPage>
```

---

## 24. `RipleyComprasMAUI/UI/MainPage.xaml.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using RipleyComprasMAUI.Data;
using RipleyComprasMAUI.Helpers;
using RipleyComprasMAUI.Models;
using RipleyComprasMAUI.Services;
using RipleyComprasMAUI.Structures;

namespace RipleyComprasMAUI.UI;

/// <summary>
/// Pantalla de caja con cinco secciones: Preventa, Venta, Boleta, Bitácora y Red de cajas.
/// Cada evento lee y valida las entradas con InputHelper, llama al servicio que corresponde y
/// refresca la vista. Aquí no hay estructuras de datos, cálculos comerciales ni acceso a archivos.
/// </summary>
public partial class MainPage : ContentPage
{
    private readonly JsonDataManager datos;
    private readonly CatalogoService catalogo;
    private readonly CarritoService carritos;
    private readonly CompraService compras;
    private readonly BoletaService boletas;
    private GrafoCajas grafo = GrafoCajas.CrearEjemplo();

    private bool datosCargados;                       // la carga inicial se hace una sola vez
    private bool procesandoPago;                      // bloquea el doble clic en "Confirmar pago"
    private bool actualizandoVista;                   // ignora eventos provocados por el propio código
    private Producto? productoSeleccionado;
    private ItemCarrito? articuloSeleccionado;        // línea elegida en el carrito de Preventa
    private ItemCarrito? articuloEnCajaSeleccionado;  // línea elegida en la compra en atención

    public MainPage(JsonDataManager datos, CatalogoService catalogo, CarritoService carritos,
                    CompraService compras, BoletaService boletas)
    {
        InitializeComponent();
        this.datos = datos;
        this.catalogo = catalogo;
        this.carritos = carritos;
        this.compras = compras;
        this.boletas = boletas;

        txtCarpetaDatos.Text = datos.CarpetaDatos;
        txtCantidadAgregar.Text = "1";
        dpFecha.Date = DateTime.Today;
        pkBitacora.SelectedIndex = 0;
        PrepararGrafo();
        MostrarSeccion(seccionPreventa, btnNavPreventa);
        RefrescarTodo();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (datosCargados)
        {
            return;
        }
        datosCargados = true;
        await CargarDatosAsync();
    }

    /// <summary>Primer inicio: crea los JSON de trabajo si faltan; luego carga catálogo y ventas.</summary>
    private async Task CargarDatosAsync()
    {
        (bool preparado, string mensajePreparacion) = await datos.PrepararArchivosAsync();
        if (!preparado)
        {
            MostrarMensaje(mensajePreparacion, true);
            return;
        }

        bool catalogoCorrecto = catalogo.Cargar(out string mensajeCatalogo);
        bool ventasCorrectas = boletas.Cargar(out string mensajeVentas);

        bool hayProblemas = !catalogoCorrecto || !ventasCorrectas
                            || !catalogo.Advertencias.EstaVacia || !boletas.Advertencias.EstaVacia;
        if (hayProblemas)
        {
            // Solo se detalla la carga cuando algo falló; si todo está bien basta con "Caja lista".
            List<string> lineas = new List<string> { mensajePreparacion, mensajeCatalogo, mensajeVentas };
            foreach (string aviso in catalogo.Advertencias)
            {
                lineas.Add("Advertencia: " + aviso);
            }
            foreach (string aviso in boletas.Advertencias)
            {
                lineas.Add("Advertencia: " + aviso);
            }
            MostrarMensaje(string.Join(Environment.NewLine, lineas), true);
        }
        else
        {
            MostrarMensaje($"Caja lista. {catalogo.Cantidad} productos en el catálogo.");
        }
        OnListarCatalogoClicked(this, EventArgs.Empty);   // el catálogo queda a la vista desde el inicio
        RefrescarTodo();
    }

    // ============================ Navegación y mensajes ============================

    private void OnNavegarClicked(object? sender, EventArgs e)
    {
        if (sender == btnNavVenta)
        {
            MostrarSeccion(seccionVenta, btnNavVenta);
        }
        else if (sender == btnNavBoleta)
        {
            MostrarSeccion(seccionBoleta, btnNavBoleta);
        }
        else if (sender == btnNavBitacora)
        {
            RefrescarBitacora();
            MostrarSeccion(seccionBitacora, btnNavBitacora);
        }
        else if (sender == btnNavRed)
        {
            MostrarSeccion(seccionRed, btnNavRed);
        }
        else
        {
            MostrarSeccion(seccionPreventa, btnNavPreventa);
        }
    }

    private void MostrarSeccion(View seccion, Button botonActivo)
    {
        seccionPreventa.IsVisible = seccion == seccionPreventa;
        seccionVenta.IsVisible = seccion == seccionVenta;
        seccionBoleta.IsVisible = seccion == seccionBoleta;
        seccionBitacora.IsVisible = seccion == seccionBitacora;
        seccionRed.IsVisible = seccion == seccionRed;

        // La sección activa se pinta en blanco con texto morado; las demás quedan sobre el morado oscuro.
        Button[] botones = { btnNavPreventa, btnNavVenta, btnNavBoleta, btnNavBitacora, btnNavRed };
        foreach (Button boton in botones)
        {
            bool activo = boton == botonActivo;
            boton.BackgroundColor = activo ? Colors.White : ColorDe("ColorPrimarioOscuro", Colors.Indigo);
            boton.TextColor = activo ? ColorDe("ColorPrimario", Colors.Purple) : Colors.White;
            boton.FontAttributes = activo ? FontAttributes.Bold : FontAttributes.None;
        }
    }

    private void MostrarMensaje(string texto, bool esError = false)
    {
        lblMensaje.Text = texto;
        lblMensaje.TextColor = esError ? ColorDe("ColorError", Colors.DarkRed) : ColorDe("ColorTexto", Colors.Black);
        brdMensaje.BackgroundColor = esError ? ColorDe("ColorErrorFondo", Colors.MistyRose) : ColorDe("ColorSuperficie", Colors.White);
    }

    private static void MostrarError(Label etiqueta, string error)
    {
        etiqueta.Text = error;
        etiqueta.IsVisible = error.Length > 0;
    }

    private static Color ColorDe(string clave, Color respaldo)
    {
        if (Application.Current != null
            && Application.Current.Resources.TryGetValue(clave, out object? valor)
            && valor is Color color)
        {
            return color;
        }
        return respaldo;
    }

    private void RefrescarTodo()
    {
        RefrescarListaProductos();
        ActualizarProductoSeleccionado();
        RefrescarCarrito();
        RefrescarFila();
        RefrescarCompraEnAtencion();
        RefrescarInfoBoletas();
        RefrescarBitacora();
    }

    // ============================ PREVENTA: búsqueda ============================

    private void OnCodigoCambiado(object? sender, TextChangedEventArgs e)
    {
        bool valido = InputHelper.TryValidarCodigo(e.NewTextValue, out _, out string error);
        MostrarError(lblErrorCodigo, string.IsNullOrWhiteSpace(e.NewTextValue) ? "" : error);
        btnBuscarCodigo.IsEnabled = valido;
    }

    private void OnBuscarCodigoClicked(object? sender, EventArgs e)
    {
        if (!InputHelper.TryValidarCodigo(txtCodigo.Text, out string codigo, out string error))
        {
            MostrarError(lblErrorCodigo, error);
            return;
        }

        if (catalogo.BuscarPorCodigo(codigo, out Producto? producto, out _) && producto != null)
        {
            MostrarResultados(new List<Producto> { producto }, "1 producto encontrado.");
            cvProductos.SelectedItem = producto;
        }
        else
        {
            MostrarResultados(new List<Producto>(), "Prenda no encontrada.");
        }
    }

    private void OnNombreBuscarCambiado(object? sender, TextChangedEventArgs e)
    {
        bool valido = InputHelper.TryValidarTextoBusqueda(e.NewTextValue, out _, out string error);
        MostrarError(lblErrorNombreBuscar, string.IsNullOrWhiteSpace(e.NewTextValue) ? "" : error);
        btnBuscarNombre.IsEnabled = valido;
    }

    private void OnBuscarNombreClicked(object? sender, EventArgs e)
    {
        if (!InputHelper.TryValidarTextoBusqueda(txtNombreBuscar.Text, out _, out string error))
        {
            MostrarError(lblErrorNombreBuscar, error);
            return;
        }
        ListaEnlazada<Producto> encontrados = catalogo.BuscarPorNombre(txtNombreBuscar.Text, out _);
        MostrarResultados(new List<Producto>(encontrados), encontrados.EstaVacia
            ? "Prenda no encontrada."
            : $"{encontrados.Cantidad} producto(s) encontrado(s).");
    }

    private void OnListarCatalogoClicked(object? sender, EventArgs e)
    {
        ListaEnlazada<Producto> todos = catalogo.Listar();
        MostrarResultados(new List<Producto>(todos), $"Catálogo completo: {todos.Cantidad} producto(s).");
    }

    private void MostrarResultados(List<Producto> productos, string mensaje)
    {
        productoSeleccionado = null;
        cvProductos.SelectedItem = null;
        cvProductos.ItemsSource = productos;
        lblResultadoBusqueda.Text = mensaje;
        ActualizarProductoSeleccionado();
    }

    /// <summary>Vuelve a dibujar los resultados mostrados (por ejemplo, para ver el stock después de una venta).</summary>
    private void RefrescarListaProductos()
    {
        if (cvProductos.ItemsSource is not List<Producto> mostrados)
        {
            return;
        }
        Producto? seleccionado = productoSeleccionado;
        cvProductos.ItemsSource = new List<Producto>(mostrados);
        cvProductos.SelectedItem = seleccionado;
        productoSeleccionado = seleccionado;
    }

    private void OnProductoSeleccionado(object? sender, SelectionChangedEventArgs e)
    {
        productoSeleccionado = e.CurrentSelection.Count > 0 ? e.CurrentSelection[0] as Producto : null;
        ActualizarProductoSeleccionado();
    }

    private void ActualizarProductoSeleccionado()
    {
        if (productoSeleccionado == null)
        {
            lblProductoSeleccionado.Text = "Selecciona un producto de la lista.";
            btnAgregar.IsEnabled = false;
            return;
        }
        int enCarrito = carritos.CantidadEnCarrito(carritos.Actual, productoSeleccionado.Codigo);
        lblProductoSeleccionado.Text =
            $"{productoSeleccionado.Nombre} · {productoSeleccionado.Stock} disponibles" +
            (enCarrito > 0 ? $" · {enCarrito} en el carrito" : "");
        btnAgregar.IsEnabled = InputHelper.TryLeerEntero(txtCantidadAgregar.Text, "Cantidad",
            InputHelper.CantidadMinima, InputHelper.CantidadMaxima, out _, out _);
    }

    private void OnCantidadAgregarCambiada(object? sender, TextChangedEventArgs e)
    {
        bool valida = InputHelper.TryLeerEntero(e.NewTextValue, "Cantidad",
            InputHelper.CantidadMinima, InputHelper.CantidadMaxima, out _, out string error);
        MostrarError(lblErrorCantidadAgregar, valida ? "" : error);
        btnAgregar.IsEnabled = valida && productoSeleccionado != null;
    }

    // ============================ PREVENTA: carrito (CarritoService) ============================

    private void OnAgregarClicked(object? sender, EventArgs e)
    {
        if (productoSeleccionado == null)
        {
            MostrarMensaje("Selecciona un producto antes de agregarlo.", true);
            return;
        }
        if (!InputHelper.TryLeerEntero(txtCantidadAgregar.Text, "Cantidad",
                InputHelper.CantidadMinima, InputHelper.CantidadMaxima, out int cantidad, out string error))
        {
            MostrarError(lblErrorCantidadAgregar, error);
            return;
        }

        bool correcto = carritos.AgregarPrenda(carritos.Actual, productoSeleccionado.Codigo, cantidad, out string mensaje);
        MostrarMensaje(mensaje, !correcto);
        RefrescarCarrito();
        ActualizarProductoSeleccionado();
    }

    private void OnArticuloCarritoSeleccionado(object? sender, SelectionChangedEventArgs e)
    {
        if (actualizandoVista)
        {
            return;
        }
        articuloSeleccionado = e.CurrentSelection.Count > 0 ? e.CurrentSelection[0] as ItemCarrito : null;
        if (articuloSeleccionado != null)
        {
            txtNuevaCantidad.Text = articuloSeleccionado.Cantidad.ToString();
        }
        ActualizarBotonesCarrito();
    }

    private void OnMasClicked(object? sender, EventArgs e)
    {
        CambiarCantidadSeleccionada(1);
    }

    private void OnMenosClicked(object? sender, EventArgs e)
    {
        CambiarCantidadSeleccionada(-1);
    }

    private void CambiarCantidadSeleccionada(int diferencia)
    {
        if (articuloSeleccionado == null)
        {
            MostrarMensaje("Selecciona un producto del carrito.", true);
            return;
        }
        int nueva = articuloSeleccionado.Cantidad + diferencia;
        if (nueva < InputHelper.CantidadMinima)
        {
            MostrarMensaje("La cantidad mínima es 1. Para sacar el producto usa Quitar.", true);
            return;
        }
        bool correcto = carritos.CambiarCantidad(carritos.Actual, articuloSeleccionado.Codigo, nueva, out string mensaje);
        MostrarMensaje(mensaje, !correcto);
        RefrescarCarrito();
    }

    private void OnNuevaCantidadCambiada(object? sender, TextChangedEventArgs e)
    {
        bool valida = InputHelper.TryLeerEntero(e.NewTextValue, "Nueva cantidad",
            InputHelper.CantidadMinima, InputHelper.CantidadMaxima, out _, out string error);
        MostrarError(lblErrorNuevaCantidad, valida || string.IsNullOrWhiteSpace(e.NewTextValue) ? "" : error);
        ActualizarBotonesCarrito();
    }

    private void OnCambiarCantidadClicked(object? sender, EventArgs e)
    {
        if (articuloSeleccionado == null)
        {
            MostrarMensaje("Selecciona un producto del carrito.", true);
            return;
        }
        if (!InputHelper.TryLeerEntero(txtNuevaCantidad.Text, "Nueva cantidad",
                InputHelper.CantidadMinima, InputHelper.CantidadMaxima, out int nueva, out string error))
        {
            MostrarError(lblErrorNuevaCantidad, error);
            return;
        }
        bool correcto = carritos.CambiarCantidad(carritos.Actual, articuloSeleccionado.Codigo, nueva, out string mensaje);
        MostrarMensaje(mensaje, !correcto);
        RefrescarCarrito();
    }

    private void OnQuitarClicked(object? sender, EventArgs e)
    {
        if (articuloSeleccionado == null)
        {
            MostrarMensaje("Selecciona un producto del carrito.", true);
            return;
        }
        bool correcto = carritos.QuitarPrenda(carritos.Actual, articuloSeleccionado.Codigo, out string mensaje);
        MostrarMensaje(mensaje, !correcto);
        articuloSeleccionado = null;
        RefrescarCarrito();
        ActualizarProductoSeleccionado();
    }

    private void OnDeshacerClicked(object? sender, EventArgs e)
    {
        bool correcto = carritos.Deshacer(carritos.Actual, out string mensaje);
        MostrarMensaje(mensaje, !correcto);
        RefrescarCarrito();
        ActualizarProductoSeleccionado();
    }

    private void OnVaciarClicked(object? sender, EventArgs e)
    {
        carritos.Vaciar(carritos.Actual, out string mensaje);
        MostrarMensaje(mensaje);
        articuloSeleccionado = null;
        RefrescarCarrito();
        ActualizarProductoSeleccionado();
    }

    private void RefrescarCarrito()
    {
        ContextoCarrito actual = carritos.Actual;
        string? codigoSeleccionado = articuloSeleccionado?.Codigo;
        List<ItemCarrito> lineas = new List<ItemCarrito>(actual.Articulos);   // copia solo para presentar

        actualizandoVista = true;
        articuloSeleccionado = null;
        foreach (ItemCarrito linea in lineas)
        {
            if (linea.Codigo == codigoSeleccionado)
            {
                articuloSeleccionado = linea;            // se conserva la selección tras refrescar
            }
        }
        cvCarrito.ItemsSource = lineas;
        cvCarrito.SelectedItem = articuloSeleccionado;
        actualizandoVista = false;

        if (articuloSeleccionado != null)
        {
            txtNuevaCantidad.Text = articuloSeleccionado.Cantidad.ToString();
        }

        decimal totalRegular = carritos.CalcularTotalRegular(actual);
        decimal estimadoRipley = compras.EstimarTotalTarjetaRipley(actual.Articulos);
        lblTituloCarrito.Text = actual.EstaVacio ? "Sin productos" : $"{actual.CantidadLineas} producto(s)";
        lblTotalRegular.Text = InputHelper.FormatearSoles(totalRegular);
        lblEstimadoRipley.Text = actual.EstaVacio || estimadoRipley >= totalRegular
            ? ""
            : $"Con Tarjeta Ripley: {InputHelper.FormatearSoles(estimadoRipley)} (ahorras {InputHelper.FormatearSoles(totalRegular - estimadoRipley)})";

        ActualizarBotonesCarrito();
        ActualizarBotonEnviar();
    }

    private void ActualizarBotonesCarrito()
    {
        bool haySeleccion = articuloSeleccionado != null;
        btnMas.IsEnabled = haySeleccion;
        btnMenos.IsEnabled = haySeleccion && articuloSeleccionado!.Cantidad > InputHelper.CantidadMinima;
        btnQuitar.IsEnabled = haySeleccion;
        btnCambiarCantidad.IsEnabled = haySeleccion && InputHelper.TryLeerEntero(txtNuevaCantidad.Text, "Nueva cantidad",
            InputHelper.CantidadMinima, InputHelper.CantidadMaxima, out _, out _);
        btnVaciar.IsEnabled = !carritos.Actual.EstaVacio;
        btnDeshacer.IsEnabled = carritos.Actual.AccionesParaDeshacer > 0;
    }

    // ============================ PREVENTA: cliente y envío a caja ============================

    private void OnDatosClienteCambiados(object? sender, TextChangedEventArgs e)
    {
        ActualizarBotonEnviar();
    }

    private void ActualizarBotonEnviar()
    {
        List<string> errores = new List<string>();
        if (!InputHelper.TryValidarNombrePersona(txtClienteNombre.Text, out _, out string errorNombre))
        {
            errores.Add(errorNombre);
        }
        if (!InputHelper.TryValidarDniOpcional(txtClienteDni.Text, out _, out string errorDni))
        {
            errores.Add(errorDni);
        }
        if (!InputHelper.TryValidarCorreo(txtClienteCorreo.Text, out _, out string errorCorreo))
        {
            errores.Add(errorCorreo);
        }
        MostrarError(lblErrorCliente, string.Join(Environment.NewLine, errores));
        btnEnviarCaja.IsEnabled = errores.Count == 0 && !carritos.Actual.EstaVacio;
    }

    private void OnEnviarCajaClicked(object? sender, EventArgs e)
    {
        bool correcto = compras.RegistrarLlegada(txtClienteNombre.Text, txtClienteDni.Text, txtClienteCorreo.Text,
                                                 out _, out string mensaje);
        MostrarMensaje(mensaje, !correcto);
        if (!correcto)
        {
            return;
        }

        txtClienteNombre.Text = "";
        txtClienteDni.Text = "";
        txtClienteCorreo.Text = "";
        articuloSeleccionado = null;
        RefrescarCarrito();
        RefrescarFila();
        ActualizarProductoSeleccionado();
    }

    // ============================ VENTA (CompraService) ============================

    private void RefrescarFila()
    {
        List<Venta> enEspera = compras.ObtenerFila();
        cvFila.ItemsSource = enEspera;
        lblInfoFila.Text = enEspera.Count == 0
            ? "Sin clientes en espera"
            : $"{enEspera.Count} en espera";
        btnAtenderSiguiente.Text = enEspera.Count == 0
            ? "Atender siguiente"
            : $"Atender siguiente (turno {enEspera[0].Turno})";
        btnAtenderSiguiente.IsEnabled = enEspera.Count > 0 && compras.CompraEnAtencion == null;
    }

    private void OnAtenderSiguienteClicked(object? sender, EventArgs e)
    {
        bool correcto = compras.AtenderSiguiente(out string mensaje);
        MostrarMensaje(mensaje, !correcto);
        if (correcto)
        {
            articuloEnCajaSeleccionado = null;
            txtCantidadCaja.Text = "";
        }
        RefrescarFila();
        RefrescarCompraEnAtencion();
    }

    private void OnTarjetaCambiada(object? sender, EventArgs e)
    {
        if (actualizandoVista || compras.CompraEnAtencion == null)
        {
            return;
        }
        if (!InputHelper.TryLeerTarjeta(pkTarjeta.SelectedIndex, out TipoTarjeta tarjeta, out string error))
        {
            MostrarMensaje(error, true);
            return;
        }
        bool correcto = compras.SeleccionarTarjeta(tarjeta, out string mensaje);
        MostrarMensaje(mensaje, !correcto);
        RefrescarCompraEnAtencion();
    }

    private void OnArticuloCajaSeleccionado(object? sender, SelectionChangedEventArgs e)
    {
        if (actualizandoVista)
        {
            return;
        }
        articuloEnCajaSeleccionado = e.CurrentSelection.Count > 0 ? e.CurrentSelection[0] as ItemCarrito : null;
        if (articuloEnCajaSeleccionado != null)
        {
            txtCantidadCaja.Text = articuloEnCajaSeleccionado.Cantidad.ToString();
        }
        ActualizarBotonesVenta();
    }

    private void OnCantidadCajaCambiada(object? sender, TextChangedEventArgs e)
    {
        bool valida = InputHelper.TryLeerEntero(e.NewTextValue, "Cantidad",
            InputHelper.CantidadMinima, InputHelper.CantidadMaxima, out _, out string error);
        MostrarError(lblErrorCantidadCaja, valida || string.IsNullOrWhiteSpace(e.NewTextValue) ? "" : error);
        ActualizarBotonesVenta();
    }

    private void OnCorregirCantidadClicked(object? sender, EventArgs e)
    {
        if (articuloEnCajaSeleccionado == null)
        {
            MostrarMensaje("Selecciona un producto de la compra en caja.", true);
            return;
        }
        if (!InputHelper.TryLeerEntero(txtCantidadCaja.Text, "Cantidad",
                InputHelper.CantidadMinima, InputHelper.CantidadMaxima, out int cantidad, out string error))
        {
            MostrarError(lblErrorCantidadCaja, error);
            return;
        }
        bool correcto = compras.CorregirCantidad(articuloEnCajaSeleccionado.Codigo, cantidad, out string mensaje);
        MostrarMensaje(mensaje, !correcto);
        RefrescarCompraEnAtencion();
    }

    private void OnQuitarCajaClicked(object? sender, EventArgs e)
    {
        if (articuloEnCajaSeleccionado == null)
        {
            MostrarMensaje("Selecciona un producto de la compra en caja.", true);
            return;
        }
        bool correcto = compras.QuitarArticulo(articuloEnCajaSeleccionado.Codigo, out string mensaje);
        MostrarMensaje(mensaje, !correcto);
        articuloEnCajaSeleccionado = null;
        RefrescarCompraEnAtencion();
    }

    private void OnDeshacerCajaClicked(object? sender, EventArgs e)
    {
        bool correcto = compras.DeshacerCorreccion(out string mensaje);
        MostrarMensaje(mensaje, !correcto);
        RefrescarCompraEnAtencion();
    }

    private void OnRetirarCompraClicked(object? sender, EventArgs e)
    {
        bool correcto = compras.RetirarCompraEnAtencion(out string mensaje);
        MostrarMensaje(mensaje, !correcto);
        articuloEnCajaSeleccionado = null;
        RefrescarCompraEnAtencion();
        RefrescarFila();
    }

    private async void OnConfirmarPagoClicked(object? sender, EventArgs e)
    {
        if (procesandoPago)
        {
            return;                                    // un segundo clic mientras se procesa no hace nada
        }
        procesandoPago = true;
        ActualizarBotonesVenta();                      // deshabilita Confirmar, Retirar y el selector de tarjeta
        MostrarMensaje("Procesando pago...");

        try
        {
            await Task.Delay(400);                     // pausa breve para que se vea el estado "procesando"
            bool correcto = compras.ConfirmarPagoSimulado(out Venta? venta, out string mensaje);
            MostrarMensaje(mensaje, !correcto);

            if (correcto && venta != null)
            {
                lblDetalleBoleta.Text = boletas.ObtenerComprobante(venta);
                MostrarVentas(new List<Venta> { venta }, "Boleta emitida");
                MostrarBoletaElegida(venta);
                MostrarSeccion(seccionBoleta, btnNavBoleta);         // la boleta recién emitida queda a la vista
            }
        }
        finally
        {
            procesandoPago = false;
            RefrescarTodo();
        }
    }

    private void RefrescarCompraEnAtencion()
    {
        Venta? venta = compras.CompraEnAtencion;
        actualizandoVista = true;

        brdSinCompra.IsVisible = venta == null;
        panelCompra.IsVisible = venta != null;

        if (venta == null)
        {
            lblCompraActual.Text = "";
            articuloEnCajaSeleccionado = null;
            cvArticulosCaja.ItemsSource = new List<ItemCarrito>();
            pkTarjeta.SelectedIndex = -1;
            lblCajaRegular.Text = "-";
            lblCajaAhorro.Text = "-";
            lblCajaGravada.Text = "-";
            lblCajaIgv.Text = "-";
            lblCajaFinal.Text = "-";
            lblAvisoTarjeta.Text = "";
        }
        else
        {
            string datosCliente = venta.Dni == null ? "" : $" · DNI {InputHelper.OcultarDni(venta.Dni)}";
            lblCompraActual.Text = $"Turno {venta.Turno} · {venta.ClienteTexto}{datosCliente}";

            string? codigoSeleccionado = articuloEnCajaSeleccionado?.Codigo;
            List<ItemCarrito> lineas = new List<ItemCarrito>(venta.Articulos);
            articuloEnCajaSeleccionado = null;
            foreach (ItemCarrito linea in lineas)
            {
                if (linea.Codigo == codigoSeleccionado)
                {
                    articuloEnCajaSeleccionado = linea;
                }
            }
            cvArticulosCaja.ItemsSource = lineas;
            cvArticulosCaja.SelectedItem = articuloEnCajaSeleccionado;

            pkTarjeta.SelectedIndex = venta.Tarjeta switch
            {
                TipoTarjeta.TarjetaRipley => 0,
                TipoTarjeta.OtraTarjeta => 1,
                _ => -1
            };
            lblCajaRegular.Text = InputHelper.FormatearSoles(venta.TotalRegular);
            lblCajaAhorro.Text = InputHelper.FormatearSoles(venta.Ahorro);
            lblCajaGravada.Text = InputHelper.FormatearSoles(venta.OperacionGravada);
            lblCajaIgv.Text = InputHelper.FormatearSoles(venta.Igv);
            lblCajaFinal.Text = InputHelper.FormatearSoles(venta.TotalFinal);
            lblAvisoTarjeta.Text = InputHelper.EsTarjetaPermitida(venta.Tarjeta)
                ? ""
                : "Selecciona una tarjeta para confirmar el pago.";
            lblAvisoTarjeta.IsVisible = lblAvisoTarjeta.Text.Length > 0;
        }

        actualizandoVista = false;
        ActualizarBotonesVenta();
    }

    private void ActualizarBotonesVenta()
    {
        Venta? venta = compras.CompraEnAtencion;
        bool puedeOperar = venta != null && !procesandoPago;
        bool puedeConfirmar = false;
        if (venta != null && puedeOperar)
        {
            puedeConfirmar = InputHelper.EsTarjetaPermitida(venta.Tarjeta) && venta.Articulos.Count > 0;
        }

        btnConfirmarPago.IsEnabled = puedeConfirmar;
        btnRetirarCompra.IsEnabled = puedeOperar;
        pkTarjeta.IsEnabled = puedeOperar;
        btnDeshacerCaja.IsEnabled = puedeOperar && compras.AccionesDeCorreccion > 0;
        btnAtenderSiguiente.IsEnabled = venta == null && !procesandoPago && compras.ClientesEnFila > 0;
        btnQuitarCaja.IsEnabled = puedeOperar && articuloEnCajaSeleccionado != null;
        btnCorregirCantidad.IsEnabled = puedeOperar && articuloEnCajaSeleccionado != null
            && InputHelper.TryLeerEntero(txtCantidadCaja.Text, "Cantidad", InputHelper.CantidadMinima, InputHelper.CantidadMaxima, out _, out _);
    }

    // ============================ BOLETA (BoletaService) ============================

    private void RefrescarInfoBoletas()
    {
        lblInfoBoletas.Text = boletas.ArchivoDisponible
            ? $"{boletas.CantidadVentas} boleta(s) emitidas"
            : "Las ventas no están disponibles: revisa el mensaje superior.";
        if (catalogo.StockPendienteDeGuardar)
        {
            lblInfoBoletas.Text += " · Atención: el stock no se pudo guardar.";
        }
    }

    private void OnNumeroBoletaCambiado(object? sender, TextChangedEventArgs e)
    {
        bool valido = InputHelper.TryLeerEntero(e.NewTextValue, "Número de boleta", 1, int.MaxValue, out _, out string error);
        MostrarError(lblErrorNumero, valido || string.IsNullOrWhiteSpace(e.NewTextValue) ? "" : error);
        btnBuscarNumero.IsEnabled = valido;
    }

    private void OnBuscarNumeroClicked(object? sender, EventArgs e)
    {
        if (!InputHelper.TryLeerEntero(txtNumeroBoleta.Text, "Número de boleta", 1, int.MaxValue, out int numero, out string error))
        {
            MostrarError(lblErrorNumero, error);
            return;
        }
        if (boletas.BuscarPorNumero(numero, out Venta? venta, out _) && venta != null)
        {
            MostrarVentas(new List<Venta> { venta }, "1 boleta encontrada");
            MostrarBoletaElegida(venta);
        }
        else
        {
            MostrarVentas(new List<Venta>(), $"No existe la boleta N° {numero}");
        }
    }

    private void OnDniBuscarCambiado(object? sender, TextChangedEventArgs e)
    {
        bool valido = InputHelper.TryValidarDni(e.NewTextValue, out _, out string error);
        MostrarError(lblErrorDniBuscar, valido || string.IsNullOrWhiteSpace(e.NewTextValue) ? "" : error);
        btnBuscarDni.IsEnabled = valido;
    }

    private void OnBuscarDniClicked(object? sender, EventArgs e)
    {
        if (!InputHelper.TryValidarDni(txtDniBuscar.Text, out string dni, out string error))
        {
            MostrarError(lblErrorDniBuscar, error);
            return;
        }
        ListaEnlazada<Venta> encontradas = boletas.BuscarPorDni(dni, out string mensaje);
        MostrarVentas(new List<Venta>(encontradas), mensaje);
    }

    private void OnBuscarFechaClicked(object? sender, EventArgs e)
    {
        DateTime fecha = dpFecha.Date is DateTime elegida ? elegida : DateTime.Today;
        ListaEnlazada<Venta> encontradas = boletas.BuscarPorFecha(fecha, out string mensaje);
        MostrarVentas(new List<Venta>(encontradas), mensaje);
    }

    private void OnVerTodasClicked(object? sender, EventArgs e)
    {
        ListaEnlazada<Venta> recientes = boletas.ListarRecientes();
        MostrarVentas(new List<Venta>(recientes), $"{recientes.Cantidad} boleta(s)");
    }

    private void OnVentaSeleccionada(object? sender, SelectionChangedEventArgs e)
    {
        if (actualizandoVista)
        {
            return;
        }
        if (e.CurrentSelection.Count > 0 && e.CurrentSelection[0] is Venta venta)
        {
            MostrarBoletaElegida(venta);
        }
    }

    /// <summary>Muestra en el panel derecho la boleta elegida y la marca en la lista de resultados.</summary>
    private void MostrarBoletaElegida(Venta venta)
    {
        lblBoletaSeleccionada.Text = venta.Boleta != null
            ? $"Boleta N° {venta.Boleta.Numero:D6} · {venta.ClienteTexto}"
            : venta.ClienteTexto;
        lblDetalleBoleta.Text = boletas.ObtenerComprobante(venta);
        actualizandoVista = true;
        cvVentas.SelectedItem = venta;
        actualizandoVista = false;
    }

    private void MostrarVentas(List<Venta> ventas, string mensaje)
    {
        actualizandoVista = true;
        cvVentas.SelectedItem = null;
        cvVentas.ItemsSource = ventas;
        actualizandoVista = false;
        lblResultadoVentas.Text = mensaje;
        lblBoletaSeleccionada.Text = "Ninguna boleta seleccionada";
        lblDetalleBoleta.Text = ventas.Count == 0
            ? "No hay boletas para mostrar."
            : "Elige una boleta de los resultados para ver su detalle.";
        RefrescarInfoBoletas();
    }

    // ============================ BITÁCORA ============================

    private void OnFiltroBitacoraCambiado(object? sender, EventArgs e)
    {
        RefrescarBitacora();
    }

    private void OnActualizarBitacoraClicked(object? sender, EventArgs e)
    {
        RefrescarBitacora();
    }

    /// <summary>0 = todos agrupados; 1, 2 y 3 = un solo subproceso.</summary>
    private void RefrescarBitacora()
    {
        int filtro = pkBitacora.SelectedIndex;
        List<string> lineas = new List<string>();
        if (filtro <= 0 || filtro == 1)
        {
            AgregarGrupo(lineas, CarritoService.Subproceso, carritos.Bitacora);
        }
        if (filtro <= 0 || filtro == 2)
        {
            AgregarGrupo(lineas, CompraService.Subproceso, compras.Bitacora);
        }
        if (filtro <= 0 || filtro == 3)
        {
            AgregarGrupo(lineas, BoletaService.Subproceso, boletas.Bitacora);
        }
        cvBitacora.ItemsSource = lineas;
    }

    private static void AgregarGrupo(List<string> lineas, string subproceso, ListaEnlazada<string> registros)
    {
        lineas.Add($"■ {subproceso} ({registros.Cantidad} registro(s))");
        if (registros.EstaVacia)
        {
            lineas.Add("     Sin registros todavía.");
        }
        foreach (string registro in registros)
        {
            lineas.Add("     " + registro);
        }
    }

    // ============================ RED DE CAJAS ============================

    private void PrepararGrafo()
    {
        List<string> nombres = new List<string>();
        for (int i = 0; i < grafo.CantidadCajas; i++)
        {
            nombres.Add(grafo.NombreCaja(i));
        }
        pkOrigen.ItemsSource = nombres;
        pkDestino.ItemsSource = nombres;
        pkOrigen.SelectedIndex = 0;
        pkDestino.SelectedIndex = grafo.CantidadCajas - 1;
        lblRuta.Text = "";
        RefrescarEnlaces();
    }

    private void RefrescarEnlaces()
    {
        cvEnlaces.ItemsSource = new List<string>(grafo.DescribirEnlaces());
    }

    private bool LeerCajasElegidas(out int origen, out int destino)
    {
        origen = pkOrigen.SelectedIndex;
        destino = pkDestino.SelectedIndex;
        if (origen < 0 || destino < 0)
        {
            lblRuta.Text = "Elige el origen y el destino.";
            return false;
        }
        if (origen == destino)
        {
            lblRuta.Text = "El origen y el destino deben ser distintos.";
            return false;
        }
        return true;
    }

    private void OnRutaMinimaClicked(object? sender, EventArgs e)
    {
        if (!LeerCajasElegidas(out int origen, out int destino))
        {
            return;
        }
        if (grafo.BuscarRutaMinima(origen, destino, out ListaEnlazada<int> ruta, out double distancia))
        {
            lblRuta.Text = $"Ruta: {grafo.DescribirRuta(ruta)}{Environment.NewLine}" +
                           $"Tiempo total: {distancia:0.##} ms";
        }
        else
        {
            lblRuta.Text = $"No hay una ruta disponible entre {grafo.NombreCaja(origen)} y {grafo.NombreCaja(destino)}.";
        }
    }

    private void OnActivarEnlaceClicked(object? sender, EventArgs e)
    {
        CambiarEstadoEnlace(true);
    }

    private void OnDesactivarEnlaceClicked(object? sender, EventArgs e)
    {
        CambiarEstadoEnlace(false);
    }

    private void CambiarEstadoEnlace(bool activo)
    {
        if (!LeerCajasElegidas(out int origen, out int destino))
        {
            return;
        }
        grafo.CambiarEstadoEnlace(origen, destino, activo, out string mensaje);
        lblRuta.Text = mensaje;
        RefrescarEnlaces();
    }

    private void OnPesoCambiado(object? sender, TextChangedEventArgs e)
    {
        bool valido = InputHelper.TryLeerDouble(e.NewTextValue, "Tiempo", 0.1, 10000, 2, out _, out string error);
        MostrarError(lblErrorPeso, valido || string.IsNullOrWhiteSpace(e.NewTextValue) ? "" : error);
        btnCambiarPeso.IsEnabled = valido;
    }

    private void OnCambiarPesoClicked(object? sender, EventArgs e)
    {
        if (!InputHelper.TryLeerDouble(txtPeso.Text, "Tiempo", 0.1, 10000, 2, out double peso, out string error))
        {
            MostrarError(lblErrorPeso, error);
            return;
        }
        if (!LeerCajasElegidas(out int origen, out int destino))
        {
            return;
        }
        grafo.CambiarPeso(origen, destino, peso, out string mensaje);
        lblRuta.Text = mensaje;
        RefrescarEnlaces();
    }

    private void OnRestablecerGrafoClicked(object? sender, EventArgs e)
    {
        grafo = GrafoCajas.CrearEjemplo();
        PrepararGrafo();
        lblRuta.Text = "Se restauró la red inicial.";
    }
}
```

---

## 25. `RipleyComprasMAUI/UI/EstilosNativos.cs`

```csharp
namespace RipleyComprasMAUI.UI;

/// <summary>
/// Ajuste visual del calendario del DatePicker en Windows (CalendarDatePicker de WinUI): se fuerza el tema
/// claro para que el calendario no se abra oscuro cuando Windows usa tema oscuro.
/// Los colores de los selectores (Picker → ComboBox) se definen una sola vez en Platforms/Windows/App.xaml.
/// Solo cambia la apariencia; no toca ninguna regla de negocio.
/// </summary>
public static class EstilosNativos
{
    public static void Aplicar()
    {
#if WINDOWS
        Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping("TemaClaroRipley", (handler, vista) =>
        {
            try
            {
                handler.PlatformView.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Light;
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                // Si WinUI no permite el cambio, el calendario se queda con el tema del sistema.
            }
        });
#endif
    }
}
```

---

## 26. `RipleyComprasMAUI/Resources/Raw/productos.json`

```json
[
  {
    "Codigo": "2015371440865",
    "Nombre": "SWE ML DESTAR INX CO",
    "PrecioRegular": 19.99,
    "PrecioTarjetaRipley": 19.99,
    "Stock": 20
  },
  {
    "Codigo": "2015371441190",
    "Nombre": "SWE ML DETEJ INX CO",
    "PrecioRegular": 19.99,
    "PrecioTarjetaRipley": 19.99,
    "Stock": 20
  },
  {
    "Codigo": "2015372278467",
    "Nombre": "FALDA LARG FESTIDO I",
    "PrecioRegular": 89.99,
    "PrecioTarjetaRipley": 89.99,
    "Stock": 20
  },
  {
    "Codigo": "2015372601050",
    "Nombre": "JEANS FITS ME WIDE L",
    "PrecioRegular": 89.99,
    "PrecioTarjetaRipley": 89.99,
    "Stock": 20
  },
  {
    "Codigo": "2015373379330",
    "Nombre": "POL MC ESTRE INX COR",
    "PrecioRegular": 49.99,
    "PrecioTarjetaRipley": 49.99,
    "Stock": 20
  },
  {
    "Codigo": "2015373474714",
    "Nombre": "POL MC FEPAPAYA INX",
    "PrecioRegular": 59.99,
    "PrecioTarjetaRipley": 39.99,
    "Stock": 20
  },
  {
    "Codigo": "2072369066599",
    "Nombre": "HI ML CUERNO AZI COR",
    "PrecioRegular": 19.99,
    "PrecioTarjetaRipley": 19.99,
    "Stock": 20
  }
]
```

---

## 27. `RipleyComprasMAUI/Resources/Styles/Colors.xaml`

```xml
<?xml version="1.0" encoding="UTF-8" ?>
<?xaml-comp compile="true" ?>
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    <!-- Paleta sobria de tienda: morado oscuro para encabezado y acciones, fondo claro y texto con buen contraste. -->
    <Color x:Key="ColorPrimario">#4B2A7B</Color>
    <Color x:Key="ColorPrimarioOscuro">#2E1A4D</Color>
    <Color x:Key="ColorPrimarioClaro">#EEE8F6</Color>
    <Color x:Key="ColorFondo">#F5F4F7</Color>
    <Color x:Key="ColorSuperficie">#FFFFFF</Color>
    <Color x:Key="ColorBorde">#D9D3E3</Color>
    <Color x:Key="ColorTexto">#1F1B24</Color>
    <Color x:Key="ColorTextoSuave">#5C5666</Color>
    <Color x:Key="ColorError">#B3261E</Color>
    <Color x:Key="ColorErrorFondo">#FBEAEA</Color>
    <Color x:Key="ColorExito">#1B6E33</Color>
    <Color x:Key="ColorDeshabilitado">#E2DFE6</Color>
    <Color x:Key="ColorAcento">#6A2FA8</Color>
    <Color x:Key="ColorSeleccionOscuro">#3A1F63</Color>
    <Color x:Key="ColorFondoCarrito">#F7F3FC</Color>
    <Color x:Key="White">White</Color>
    <Color x:Key="Black">Black</Color>

</ResourceDictionary>
```

---

## 28. `RipleyComprasMAUI/Resources/Styles/Styles.xaml`

```xml
<?xml version="1.0" encoding="UTF-8" ?>
<?xaml-comp compile="true" ?>
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    <!-- ========== Página ========== -->
    <Style TargetType="Page" ApplyToDerivedTypes="True">
        <Setter Property="Padding" Value="0" />
        <Setter Property="BackgroundColor" Value="{StaticResource ColorFondo}" />
    </Style>

    <!-- ========== Etiquetas ========== -->
    <Style x:Key="EtiquetaBase" TargetType="Label">
        <Setter Property="TextColor" Value="{StaticResource ColorTexto}" />
        <Setter Property="FontFamily" Value="OpenSansRegular" />
        <Setter Property="FontSize" Value="14" />
        <Setter Property="LineBreakMode" Value="WordWrap" />
    </Style>
    <Style TargetType="Label" BasedOn="{StaticResource EtiquetaBase}" />

    <Style x:Key="EtiquetaTitulo" TargetType="Label" BasedOn="{StaticResource EtiquetaBase}">
        <Setter Property="FontFamily" Value="OpenSansSemibold" />
        <Setter Property="FontSize" Value="20" />
        <Setter Property="TextColor" Value="{StaticResource ColorPrimario}" />
    </Style>

    <Style x:Key="EtiquetaSubtitulo" TargetType="Label" BasedOn="{StaticResource EtiquetaBase}">
        <Setter Property="FontFamily" Value="OpenSansSemibold" />
    </Style>

    <Style x:Key="EtiquetaSuave" TargetType="Label" BasedOn="{StaticResource EtiquetaBase}">
        <Setter Property="TextColor" Value="{StaticResource ColorTextoSuave}" />
        <Setter Property="FontSize" Value="13" />
    </Style>

    <Style x:Key="EtiquetaError" TargetType="Label" BasedOn="{StaticResource EtiquetaBase}">
        <Setter Property="TextColor" Value="{StaticResource ColorError}" />
        <Setter Property="FontSize" Value="13" />
    </Style>

    <Style x:Key="EtiquetaMonto" TargetType="Label" BasedOn="{StaticResource EtiquetaBase}">
        <Setter Property="FontFamily" Value="OpenSansSemibold" />
        <Setter Property="FontSize" Value="16" />
    </Style>

    <!-- ========== Botones ========== -->
    <Style x:Key="BotonBase" TargetType="Button">
        <Setter Property="TextColor" Value="{StaticResource White}" />
        <Setter Property="BackgroundColor" Value="{StaticResource ColorPrimario}" />
        <Setter Property="FontFamily" Value="OpenSansSemibold" />
        <Setter Property="FontSize" Value="14" />
        <Setter Property="CornerRadius" Value="6" />
        <Setter Property="Padding" Value="14,8" />
        <Setter Property="MinimumHeightRequest" Value="40" />
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal" />
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="TextColor" Value="{StaticResource ColorTextoSuave}" />
                            <Setter Property="BackgroundColor" Value="{StaticResource ColorDeshabilitado}" />
                        </VisualState.Setters>
                    </VisualState>
                    <VisualState x:Name="PointerOver" />
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>
    <Style TargetType="Button" BasedOn="{StaticResource BotonBase}" />

    <Style x:Key="BotonSecundario" TargetType="Button" BasedOn="{StaticResource BotonBase}">
        <Setter Property="BackgroundColor" Value="{StaticResource ColorPrimarioClaro}" />
        <Setter Property="TextColor" Value="{StaticResource ColorPrimario}" />
    </Style>

    <Style x:Key="BotonPeligro" TargetType="Button" BasedOn="{StaticResource BotonBase}">
        <Setter Property="BackgroundColor" Value="{StaticResource ColorError}" />
    </Style>

    <!-- Acción principal de cada pantalla: Agregar, Enviar a caja, Atender siguiente, Confirmar pago, Calcular ruta -->
    <Style x:Key="BotonDestacado" TargetType="Button" BasedOn="{StaticResource BotonBase}">
        <Setter Property="BackgroundColor" Value="{StaticResource ColorAcento}" />
        <Setter Property="FontSize" Value="16" />
        <Setter Property="MinimumHeightRequest" Value="50" />
        <Setter Property="CornerRadius" Value="8" />
    </Style>

    <!-- Menú superior: el código pinta la sección activa (fondo blanco, texto morado) -->
    <Style x:Key="BotonMenu" TargetType="Button" BasedOn="{StaticResource BotonBase}">
        <Setter Property="BackgroundColor" Value="{StaticResource ColorPrimarioOscuro}" />
        <Setter Property="TextColor" Value="{StaticResource White}" />
        <Setter Property="FontSize" Value="15" />
        <Setter Property="Padding" Value="18,8" />
    </Style>

    <!-- ========== Entradas ========== -->
    <Style TargetType="Entry">
        <Setter Property="TextColor" Value="{StaticResource ColorTexto}" />
        <Setter Property="BackgroundColor" Value="{StaticResource ColorSuperficie}" />
        <Setter Property="PlaceholderColor" Value="{StaticResource ColorTextoSuave}" />
        <Setter Property="FontFamily" Value="OpenSansRegular" />
        <Setter Property="FontSize" Value="14" />
        <Setter Property="MinimumHeightRequest" Value="40" />
    </Style>

    <Style TargetType="Picker">
        <Setter Property="TextColor" Value="{StaticResource ColorTexto}" />
        <Setter Property="TitleColor" Value="{StaticResource ColorTextoSuave}" />
        <Setter Property="BackgroundColor" Value="{StaticResource ColorSuperficie}" />
        <Setter Property="FontFamily" Value="OpenSansRegular" />
        <Setter Property="FontSize" Value="14" />
        <Setter Property="MinimumHeightRequest" Value="40" />
    </Style>

    <Style TargetType="DatePicker">
        <Setter Property="TextColor" Value="{StaticResource ColorTexto}" />
        <Setter Property="BackgroundColor" Value="{StaticResource ColorSuperficie}" />
        <Setter Property="FontFamily" Value="OpenSansRegular" />
        <Setter Property="FontSize" Value="14" />
        <Setter Property="MinimumHeightRequest" Value="40" />
    </Style>

    <!-- ========== Contenedores ========== -->
    <Style x:Key="Tarjeta" TargetType="Border">
        <Setter Property="Stroke" Value="{StaticResource ColorBorde}" />
        <Setter Property="StrokeThickness" Value="1" />
        <Setter Property="StrokeShape" Value="RoundRectangle 8" />
        <Setter Property="BackgroundColor" Value="{StaticResource ColorSuperficie}" />
        <Setter Property="Padding" Value="12" />
    </Style>

    <!-- Carrito y pago: se separan del resto con fondo lila y borde morado -->
    <Style x:Key="TarjetaResaltada" TargetType="Border" BasedOn="{StaticResource Tarjeta}">
        <Setter Property="Stroke" Value="{StaticResource ColorPrimario}" />
        <Setter Property="StrokeThickness" Value="2" />
        <Setter Property="BackgroundColor" Value="{StaticResource ColorFondoCarrito}" />
        <Setter Property="Padding" Value="16" />
    </Style>

    <Style x:Key="EtiquetaTotal" TargetType="Label" BasedOn="{StaticResource EtiquetaBase}">
        <Setter Property="FontFamily" Value="OpenSansSemibold" />
        <Setter Property="FontSize" Value="24" />
        <Setter Property="TextColor" Value="{StaticResource ColorPrimario}" />
    </Style>

    <!-- Fila de una lista: la elegida se pinta de lila para que se vea cuál está seleccionada -->
    <Style x:Key="FilaLista" TargetType="Grid">
        <Setter Property="Padding" Value="10,8" />
        <Setter Property="ColumnSpacing" Value="12" />
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal">
                        <VisualState.Setters>
                            <Setter Property="BackgroundColor" Value="Transparent" />
                        </VisualState.Setters>
                    </VisualState>
                    <VisualState x:Name="Selected">
                        <VisualState.Setters>
                            <Setter Property="BackgroundColor" Value="{StaticResource ColorPrimarioClaro}" />
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>

</ResourceDictionary>
```

---

## 29. `RipleyComprasMAUI/Platforms/Windows/App.xaml`

```xml
<maui:MauiWinUIApplication
    x:Class="RipleyComprasMAUI.WinUI.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="using:RipleyComprasMAUI.WinUI"
    xmlns:maui="using:Microsoft.Maui">

    <!-- Colores de los selectores (ComboBox de WinUI) para toda la aplicación: pkTarjeta, el filtro de la
         Bitácora y Origen/Destino de la Red de cajas. Se definen iguales para tema claro ("Light") y oscuro
         ("Default"), así el desplegable se lee bien aunque Windows esté en tema oscuro. La opción elegida
         usa el morado de la aplicación (#4B2A7B) con texto blanco. -->
    <maui:MauiWinUIApplication.Resources>
        <ResourceDictionary>
            <ResourceDictionary.ThemeDictionaries>
                <ResourceDictionary x:Key="Light">
                    <SolidColorBrush x:Key="ComboBoxBackground" Color="#FFFFFFFF" />
                    <SolidColorBrush x:Key="ComboBoxBackgroundPointerOver" Color="#FFF7F3FC" />
                    <SolidColorBrush x:Key="ComboBoxBackgroundPressed" Color="#FFEEE8F6" />
                    <SolidColorBrush x:Key="ComboBoxBackgroundFocused" Color="#FFFFFFFF" />
                    <SolidColorBrush x:Key="ComboBoxBackgroundUnfocused" Color="#FFFFFFFF" />
                    <SolidColorBrush x:Key="ComboBoxBackgroundDisabled" Color="#FFE2DFE6" />
                    <SolidColorBrush x:Key="ComboBoxForeground" Color="#FF1F1B24" />
                    <SolidColorBrush x:Key="ComboBoxForegroundFocused" Color="#FF1F1B24" />
                    <SolidColorBrush x:Key="ComboBoxForegroundFocusedPressed" Color="#FF1F1B24" />
                    <SolidColorBrush x:Key="ComboBoxForegroundDisabled" Color="#FF5C5666" />
                    <SolidColorBrush x:Key="ComboBoxPlaceHolderForeground" Color="#FF5C5666" />
                    <SolidColorBrush x:Key="ComboBoxPlaceHolderForegroundFocusedPressed" Color="#FF5C5666" />
                    <SolidColorBrush x:Key="ComboBoxBorderBrush" Color="#FFD9D3E3" />
                    <SolidColorBrush x:Key="ComboBoxBorderBrushPointerOver" Color="#FF4B2A7B" />
                    <SolidColorBrush x:Key="ComboBoxBorderBrushFocused" Color="#FF4B2A7B" />
                    <SolidColorBrush x:Key="ComboBoxDropDownGlyphForeground" Color="#FF4B2A7B" />
                    <SolidColorBrush x:Key="ComboBoxDropDownBackground" Color="#FFFFFFFF" />
                    <SolidColorBrush x:Key="ComboBoxDropDownBorderBrush" Color="#FFD9D3E3" />
                    <SolidColorBrush x:Key="ComboBoxItemForeground" Color="#FF1F1B24" />
                    <SolidColorBrush x:Key="ComboBoxItemForegroundPointerOver" Color="#FF1F1B24" />
                    <SolidColorBrush x:Key="ComboBoxItemForegroundPressed" Color="#FF1F1B24" />
                    <SolidColorBrush x:Key="ComboBoxItemBackgroundPointerOver" Color="#FFEEE8F6" />
                    <SolidColorBrush x:Key="ComboBoxItemBackgroundPressed" Color="#FFEEE8F6" />
                    <SolidColorBrush x:Key="ComboBoxItemBackgroundSelected" Color="#FF4B2A7B" />
                    <SolidColorBrush x:Key="ComboBoxItemBackgroundSelectedUnfocused" Color="#FF4B2A7B" />
                    <SolidColorBrush x:Key="ComboBoxItemBackgroundSelectedPointerOver" Color="#FF3A1F63" />
                    <SolidColorBrush x:Key="ComboBoxItemBackgroundSelectedPressed" Color="#FF3A1F63" />
                    <SolidColorBrush x:Key="ComboBoxItemForegroundSelected" Color="#FFFFFFFF" />
                    <SolidColorBrush x:Key="ComboBoxItemForegroundSelectedUnfocused" Color="#FFFFFFFF" />
                    <SolidColorBrush x:Key="ComboBoxItemForegroundSelectedPointerOver" Color="#FFFFFFFF" />
                    <SolidColorBrush x:Key="ComboBoxItemForegroundSelectedPressed" Color="#FFFFFFFF" />
                    <SolidColorBrush x:Key="ComboBoxItemPillFillBrush" Color="#FFFFFFFF" />
                </ResourceDictionary>
                <ResourceDictionary x:Key="Default">
                    <SolidColorBrush x:Key="ComboBoxBackground" Color="#FFFFFFFF" />
                    <SolidColorBrush x:Key="ComboBoxBackgroundPointerOver" Color="#FFF7F3FC" />
                    <SolidColorBrush x:Key="ComboBoxBackgroundPressed" Color="#FFEEE8F6" />
                    <SolidColorBrush x:Key="ComboBoxBackgroundFocused" Color="#FFFFFFFF" />
                    <SolidColorBrush x:Key="ComboBoxBackgroundUnfocused" Color="#FFFFFFFF" />
                    <SolidColorBrush x:Key="ComboBoxBackgroundDisabled" Color="#FFE2DFE6" />
                    <SolidColorBrush x:Key="ComboBoxForeground" Color="#FF1F1B24" />
                    <SolidColorBrush x:Key="ComboBoxForegroundFocused" Color="#FF1F1B24" />
                    <SolidColorBrush x:Key="ComboBoxForegroundFocusedPressed" Color="#FF1F1B24" />
                    <SolidColorBrush x:Key="ComboBoxForegroundDisabled" Color="#FF5C5666" />
                    <SolidColorBrush x:Key="ComboBoxPlaceHolderForeground" Color="#FF5C5666" />
                    <SolidColorBrush x:Key="ComboBoxPlaceHolderForegroundFocusedPressed" Color="#FF5C5666" />
                    <SolidColorBrush x:Key="ComboBoxBorderBrush" Color="#FFD9D3E3" />
                    <SolidColorBrush x:Key="ComboBoxBorderBrushPointerOver" Color="#FF4B2A7B" />
                    <SolidColorBrush x:Key="ComboBoxBorderBrushFocused" Color="#FF4B2A7B" />
                    <SolidColorBrush x:Key="ComboBoxDropDownGlyphForeground" Color="#FF4B2A7B" />
                    <SolidColorBrush x:Key="ComboBoxDropDownBackground" Color="#FFFFFFFF" />
                    <SolidColorBrush x:Key="ComboBoxDropDownBorderBrush" Color="#FFD9D3E3" />
                    <SolidColorBrush x:Key="ComboBoxItemForeground" Color="#FF1F1B24" />
                    <SolidColorBrush x:Key="ComboBoxItemForegroundPointerOver" Color="#FF1F1B24" />
                    <SolidColorBrush x:Key="ComboBoxItemForegroundPressed" Color="#FF1F1B24" />
                    <SolidColorBrush x:Key="ComboBoxItemBackgroundPointerOver" Color="#FFEEE8F6" />
                    <SolidColorBrush x:Key="ComboBoxItemBackgroundPressed" Color="#FFEEE8F6" />
                    <SolidColorBrush x:Key="ComboBoxItemBackgroundSelected" Color="#FF4B2A7B" />
                    <SolidColorBrush x:Key="ComboBoxItemBackgroundSelectedUnfocused" Color="#FF4B2A7B" />
                    <SolidColorBrush x:Key="ComboBoxItemBackgroundSelectedPointerOver" Color="#FF3A1F63" />
                    <SolidColorBrush x:Key="ComboBoxItemBackgroundSelectedPressed" Color="#FF3A1F63" />
                    <SolidColorBrush x:Key="ComboBoxItemForegroundSelected" Color="#FFFFFFFF" />
                    <SolidColorBrush x:Key="ComboBoxItemForegroundSelectedUnfocused" Color="#FFFFFFFF" />
                    <SolidColorBrush x:Key="ComboBoxItemForegroundSelectedPointerOver" Color="#FFFFFFFF" />
                    <SolidColorBrush x:Key="ComboBoxItemForegroundSelectedPressed" Color="#FFFFFFFF" />
                    <SolidColorBrush x:Key="ComboBoxItemPillFillBrush" Color="#FFFFFFFF" />
                </ResourceDictionary>
            </ResourceDictionary.ThemeDictionaries>
        </ResourceDictionary>
    </maui:MauiWinUIApplication.Resources>

</maui:MauiWinUIApplication>
```

---

## 30. `RipleyComprasMAUI/Platforms/Windows/App.xaml.cs`

```csharp
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RipleyComprasMAUI.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }

}
```

---

## 31. `RipleyComprasMAUI/Platforms/Windows/app.manifest`

```xml
<?xml version="1.0" encoding="utf-8"?>
<assembly manifestVersion="1.0" xmlns="urn:schemas-microsoft-com:asm.v1">
  <assemblyIdentity version="1.0.0.0" name="RipleyComprasMAUI.WinUI.app"/>

  <application xmlns="urn:schemas-microsoft-com:asm.v3">
    <windowsSettings>
      <!-- The combination of below two tags have the following effect:
           1) Per-Monitor for >= Windows 10 Anniversary Update
           2) System < Windows 10 Anniversary Update
      -->
      <dpiAware xmlns="http://schemas.microsoft.com/SMI/2005/WindowsSettings">true/PM</dpiAware>
      <dpiAwareness xmlns="http://schemas.microsoft.com/SMI/2016/WindowsSettings">PerMonitorV2, PerMonitor</dpiAwareness>
    </windowsSettings>
  </application>
</assembly>
```

---

## 32. `RipleyComprasMAUI/Platforms/Windows/Package.appxmanifest`

```xml
<?xml version="1.0" encoding="utf-8"?>
<Package
  xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
  xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10"
  xmlns:mp="http://schemas.microsoft.com/appx/2014/phone/manifest"
  xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities"
  IgnorableNamespaces="uap rescap">

  <Identity Name="maui-package-name-placeholder" Publisher="CN=User Name" Version="0.0.0.0" />

  <mp:PhoneIdentity PhoneProductId="50A918C5-2264-425C-BE19-77DBF67497F0" PhonePublisherId="00000000-0000-0000-0000-000000000000"/>

  <Properties>
    <DisplayName>$placeholder$</DisplayName>
    <PublisherDisplayName>User Name</PublisherDisplayName>
    <Logo>$placeholder$.png</Logo>
  </Properties>

  <Dependencies>
    <TargetDeviceFamily Name="Windows.Universal" MinVersion="10.0.17763.0" MaxVersionTested="10.0.19041.0" />
    <TargetDeviceFamily Name="Windows.Desktop" MinVersion="10.0.17763.0" MaxVersionTested="10.0.19041.0" />
  </Dependencies>

  <Resources>
    <Resource Language="x-generate" />
  </Resources>

  <Applications>
    <Application Id="App" Executable="$targetnametoken$.exe" EntryPoint="$targetentrypoint$">
      <uap:VisualElements
        DisplayName="$placeholder$"
        Description="$placeholder$"
        Square150x150Logo="$placeholder$.png"
        Square44x44Logo="$placeholder$.png"
        BackgroundColor="transparent">
        <uap:DefaultTile Square71x71Logo="$placeholder$.png" Wide310x150Logo="$placeholder$.png" Square310x310Logo="$placeholder$.png" />
        <uap:SplashScreen Image="$placeholder$.png" />
      </uap:VisualElements>
    </Application>
  </Applications>

  <Capabilities>
    <rescap:Capability Name="runFullTrust" />
  </Capabilities>

</Package>
```

---

## 33. `RipleyComprasMAUI/Properties/launchSettings.json`

```json
{
  "profiles": {
    "Windows Machine": {
      "commandName": "Project",
      "nativeDebugging": false
    }
  }
}
```

---

## 34. `Comprobaciones/Comprobaciones.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <!-- Programa de consola OPCIONAL para comprobar la lógica sin abrir la interfaz.
       Compila los mismos archivos de la app (Models, Structures, Services, Data, Helpers);
       no incluye UI ni MAUI. Ejecutar: dotnet run (desde esta carpeta). -->
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <RootNamespace>Comprobaciones</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <Compile Include="..\RipleyComprasMAUI\Models\*.cs" Link="App\Models\%(Filename)%(Extension)" />
    <Compile Include="..\RipleyComprasMAUI\Structures\*.cs" Link="App\Structures\%(Filename)%(Extension)" />
    <Compile Include="..\RipleyComprasMAUI\Services\*.cs" Link="App\Services\%(Filename)%(Extension)" />
    <Compile Include="..\RipleyComprasMAUI\Data\*.cs" Link="App\Data\%(Filename)%(Extension)" />
    <Compile Include="..\RipleyComprasMAUI\Helpers\*.cs" Link="App\Helpers\%(Filename)%(Extension)" />
  </ItemGroup>

</Project>
```

---

## 35. `Comprobaciones/Program.cs`

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RipleyComprasMAUI.Data;
using RipleyComprasMAUI.Helpers;
using RipleyComprasMAUI.Models;
using RipleyComprasMAUI.Services;
using RipleyComprasMAUI.Structures;

// Comprobaciones de la lógica de RipleyComprasMAUI, sin interfaz ni librerías de pruebas.
// Cada grupo usa su propia carpeta temporal: no se tocan los datos reales de la aplicación.

int correctas = 0;
int fallidas = 0;
string carpetaBase = Path.Combine(Path.GetTempPath(), "RipleyComprobaciones_" + Guid.NewGuid().ToString("N"));

const string Fepapaya = "2015373474714";
const string Estre = "2015373379330";
const string Falda = "2015372278467";
const string Jeans = "2015372601050";
const string Cuerno = "2072369066599";
const string Destar = "2015371440865";
const string Detej = "2015371441190";

const string SemillaRipley = """
[
  { "Codigo": "2015371440865", "Nombre": "SWE ML DESTAR INX CO", "PrecioRegular": 19.99, "PrecioTarjetaRipley": 19.99, "Stock": 20 },
  { "Codigo": "2015371441190", "Nombre": "SWE ML DETEJ INX CO",  "PrecioRegular": 19.99, "PrecioTarjetaRipley": 19.99, "Stock": 20 },
  { "Codigo": "2015372278467", "Nombre": "FALDA LARG FESTIDO I", "PrecioRegular": 89.99, "PrecioTarjetaRipley": 89.99, "Stock": 20 },
  { "Codigo": "2015372601050", "Nombre": "JEANS FITS ME WIDE L", "PrecioRegular": 89.99, "PrecioTarjetaRipley": 89.99, "Stock": 20 },
  { "Codigo": "2015373379330", "Nombre": "POL MC ESTRE INX COR", "PrecioRegular": 49.99, "PrecioTarjetaRipley": 49.99, "Stock": 20 },
  { "Codigo": "2015373474714", "Nombre": "POL MC FEPAPAYA INX",  "PrecioRegular": 59.99, "PrecioTarjetaRipley": 39.99, "Stock": 20 },
  { "Codigo": "2072369066599", "Nombre": "HI ML CUERNO AZI COR", "PrecioRegular": 19.99, "PrecioTarjetaRipley": 19.99, "Stock": 20 }
]
""";

// Catálogo FICTICIO solo para probar la búsqueda por nombre (no se agrega al catálogo base).
const string SemillaPoloRojo = """
[
  { "Codigo": "2000000000001", "Nombre": "Polo Rojo",            "PrecioRegular": 30.00, "PrecioTarjetaRipley": 25.00, "Stock": 5 },
  { "Codigo": "2000000000002", "Nombre": "POLO ROJO MANGA LARGA", "PrecioRegular": 35.00, "PrecioTarjetaRipley": 35.00, "Stock": 5 },
  { "Codigo": "2000000000003", "Nombre": "Polo Azul",            "PrecioRegular": 30.00, "PrecioTarjetaRipley": 30.00, "Stock": 5 },
  { "Codigo": "2000000000004", "Nombre": "Pantalón rojo",        "PrecioRegular": 70.00, "PrecioTarjetaRipley": 60.00, "Stock": 5 }
]
""";

void Comprobar(string nombre, bool condicion, string detalle = "")
{
    if (condicion)
    {
        correctas++;
        Console.WriteLine("[OK]    " + nombre);
    }
    else
    {
        fallidas++;
        Console.WriteLine("[FALLA] " + nombre + (detalle.Length > 0 ? "  -> " + detalle : ""));
    }
}

void Titulo(string texto)
{
    Console.WriteLine();
    Console.WriteLine("== " + texto + " ==");
}

async Task<Entorno> CrearEntornoAsync(string nombreCarpeta, string semilla)
{
    string carpeta = Path.Combine(carpetaBase, nombreCarpeta);
    JsonDataManager datos = new JsonDataManager(carpeta,
        () => Task.FromResult<Stream>(new MemoryStream(Encoding.UTF8.GetBytes(semilla))));
    await datos.PrepararArchivosAsync();
    return Armar(datos);
}

Entorno Armar(JsonDataManager datos)
{
    CatalogoService catalogo = new CatalogoService(datos);
    catalogo.Cargar(out _);
    CarritoService carritos = new CarritoService(catalogo);
    BoletaService boletas = new BoletaService(datos);
    boletas.Cargar(out _);
    CompraService compras = new CompraService(carritos, catalogo, boletas);
    return new Entorno(datos, catalogo, carritos, boletas, compras);
}

bool RegistrarCompra(Entorno e, (string Codigo, int Cantidad)[] lineas, string? nombre = null, string? dni = null, string? correo = null)
{
    foreach ((string codigo, int cantidad) in lineas)
    {
        if (!e.Carritos.AgregarPrenda(e.Carritos.Actual, codigo, cantidad, out string mensaje))
        {
            Console.WriteLine("        (no se pudo agregar: " + mensaje + ")");
            return false;
        }
    }
    return e.Compras.RegistrarLlegada(nombre, dni, correo, out _, out _);
}

string Orden(ContextoCarrito carrito)
{
    List<string> codigos = new List<string>();
    foreach (ItemCarrito articulo in carrito.Articulos)
    {
        codigos.Add(articulo.Codigo);
    }
    return string.Join(",", codigos);
}

string CodigosDe(ListaEnlazada<Producto> productos)
{
    List<string> codigos = new List<string>();
    foreach (Producto producto in productos)
    {
        codigos.Add(producto.Codigo);
    }
    return string.Join(",", codigos);
}

try
{
    // ------------------------------------------------------------------
    Titulo("Estructuras básicas");
    ColaCompras<int> cola = new ColaCompras<int>();
    cola.Encolar(1); cola.Encolar(2); cola.Encolar(3);
    Comprobar("Cola FIFO: sale primero el 1", cola.Desencolar() == 1 && cola.VerFrente() == 2 && cola.Cantidad == 2);
    PilaAcciones<string> pila = new PilaAcciones<string>();
    pila.Apilar("a"); pila.Apilar("b");
    Comprobar("Pila LIFO: sale primero la última", pila.Desapilar() == "b" && pila.Cantidad == 1);
    ListaEnlazada<int> lista = new ListaEnlazada<int>();
    lista.AgregarAlFinal(10); lista.AgregarAlFinal(20); lista.AgregarAlFinal(30);
    lista.Eliminar(x => x == 30, out _, out int posicionQuitada);
    lista.AgregarAlFinal(40);                       // "ultimo" debe haberse actualizado al quitar el 30
    lista.InsertarEn(1, 15);
    Comprobar("Lista: quitar el último y reinsertar en posición", posicionQuitada == 2 && string.Join(",", lista) == "10,15,20,40");

    ArbolBusqueda<int, int> arbolNumeros = new ArbolBusqueda<int, int>();
    for (int i = 1; i <= 100; i++)
    {
        arbolNumeros.Insertar(i, i);
    }
    Comprobar("AVL con 100 claves ascendentes: altura 7", arbolNumeros.Altura == 7, $"altura {arbolNumeros.Altura}");
    Comprobar("AVL: clave repetida no se inserta", !arbolNumeros.Insertar(50, 0) && arbolNumeros.Cantidad == 100);

    // ------------------------------------------------------------------
    Titulo("Preventa: árbol y búsquedas sobre el catálogo real");
    Entorno a = await CrearEntornoAsync("arbol", SemillaRipley);
    Comprobar("Catálogo con 7 prendas", a.Catalogo.Cantidad == 7, $"{a.Catalogo.Cantidad}");
    Comprobar("AVL: altura 3", a.Catalogo.Altura == 3, $"altura {a.Catalogo.Altura}");
    Comprobar("AVL: raíz 2015372601050 (JEANS)", a.Catalogo.CodigoRaiz == Jeans, $"raíz {a.Catalogo.CodigoRaiz}");
    Comprobar("AVL: 4 rotaciones al insertar en orden ascendente", a.Catalogo.Rotaciones == 4, $"{a.Catalogo.Rotaciones}");

    bool encontrada = a.Catalogo.BuscarPorCodigo(Fepapaya, out Producto? fepapaya, out _);
    Comprobar("2015373474714 encontrada en 2 comparaciones con S/ 59.99 y S/ 39.99",
        encontrada && fepapaya != null && fepapaya.PrecioRegular == 59.99m && fepapaya.PrecioTarjetaRipley == 39.99m
        && a.Catalogo.UltimasComparaciones == 2, $"comparaciones {a.Catalogo.UltimasComparaciones}");
    Comprobar("9999999999999: Prenda no encontrada",
        !a.Catalogo.BuscarPorCodigo("9999999999999", out _, out string mensajeInexistente) && mensajeInexistente.StartsWith("Prenda no encontrada"));
    Comprobar("20153734 (8 dígitos): Código inválido",
        !a.Catalogo.BuscarPorCodigo("20153734", out _, out string mensajeCorto) && mensajeCorto.StartsWith("Código inválido"));

    foreach (string variante in new[] { "POL MC FEPAPAYA INX", "Pol Mc Fepapaya Inx", "Pól MC Fepápaya Inx" })
    {
        ListaEnlazada<Producto> resultado = a.Catalogo.BuscarPorNombre(variante, out _);
        Comprobar($"Nombre \"{variante}\" encuentra solo 2015373474714", CodigosDe(resultado) == Fepapaya, CodigosDe(resultado));
    }
    ListaEnlazada<Producto> parcial = a.Catalogo.BuscarPorNombre("pol mc", out _);
    Comprobar("Búsqueda parcial \"pol mc\": 2 prendas en orden de código", CodigosDe(parcial) == Estre + "," + Fepapaya, CodigosDe(parcial));
    ListaEnlazada<Producto> vacia = a.Catalogo.BuscarPorNombre("   ", out string mensajeVacio);
    Comprobar("Consulta vacía: mensaje y ningún resultado", vacia.EstaVacia && mensajeVacio.StartsWith("Escriba"));
    ListaEnlazada<Producto> sinCoincidencias = a.Catalogo.BuscarPorNombre("zapatilla", out string mensajeSin);
    Comprobar("Sin coincidencias: Prenda no encontrada", sinCoincidencias.EstaVacia && mensajeSin == "Prenda no encontrada.");

    // ------------------------------------------------------------------
    Titulo("Búsqueda por nombre sin mayúsculas ni tildes (catálogo ficticio \"Polo Rojo\")");
    Entorno polo = await CrearEntornoAsync("polo", SemillaPoloRojo);
    string esperado = CodigosDe(polo.Catalogo.BuscarPorNombre("POLO ROJO", out _));
    string[] consultas = { "POLO ROJO", "Polo Rojo", "polo rojo", "Poló rojo", "  póló   RÓJO  ", "Poló rojo", "Poló rojo" };
    foreach (string consulta in consultas)
    {
        string obtenido = CodigosDe(polo.Catalogo.BuscarPorNombre(consulta, out _));
        Comprobar($"\"{consulta.Replace("́", "(tilde separada)")}\" → mismos códigos y mismo orden", obtenido == esperado, obtenido);
    }
    Comprobar("La lista esperada es 2000000000001,2000000000002", esperado == "2000000000001,2000000000002", esperado);
    Comprobar("Normalización equivalente para acento compuesto y separado",
        InputHelper.NormalizarParaBusqueda("Poló") == InputHelper.NormalizarParaBusqueda("Poló"));
    Producto? primero = polo.Catalogo.Listar().Buscar(prenda => prenda.Codigo == "2000000000001");
    Comprobar("El nombre guardado no cambia (\"Polo Rojo\")", primero != null && primero.Nombre == "Polo Rojo");

    // ------------------------------------------------------------------
    Titulo("Carrito: lista enlazada y pila de deshacer");
    Entorno c = await CrearEntornoAsync("carrito", SemillaRipley);
    ContextoCarrito k = c.Carritos.Actual;
    c.Carritos.AgregarPrenda(k, Fepapaya, 1, out _);
    c.Carritos.AgregarPrenda(k, Estre, 1, out _);
    c.Carritos.AgregarPrenda(k, Falda, 1, out _);
    c.Carritos.AgregarPrenda(k, Falda, 1, out _);
    Comprobar("Misma prenda dos veces: 3 líneas y FALDA x 2", k.CantidadLineas == 3 && c.Carritos.CantidadEnCarrito(k, Falda) == 2);
    Comprobar("Total regular 289.96", c.Carritos.CalcularTotalRegular(k) == 289.96m, $"{c.Carritos.CalcularTotalRegular(k)}");
    c.Carritos.Deshacer(k, out _);
    Comprobar("Deshacer el aumento: FALDA vuelve a 1 y el total a 199.97",
        c.Carritos.CantidadEnCarrito(k, Falda) == 1 && c.Carritos.CalcularTotalRegular(k) == 199.97m);
    c.Carritos.CambiarCantidad(k, Falda, 2, out _);
    c.Carritos.QuitarPrenda(k, Estre, out _);
    Comprobar("FEPAPAYA x 1 y FALDA x 2: total regular 239.97",
        c.Carritos.CalcularTotalRegular(k) == 239.97m && Orden(k) == Fepapaya + "," + Falda, Orden(k));
    c.Carritos.Deshacer(k, out _);
    Comprobar("Deshacer quitar: ESTRE vuelve a su posición original", Orden(k) == Fepapaya + "," + Estre + "," + Falda, Orden(k));
    c.Carritos.Deshacer(k, out _);
    Comprobar("Deshacer cambio de cantidad: FALDA vuelve a 1", c.Carritos.CantidadEnCarrito(k, Falda) == 1);
    c.Carritos.Deshacer(k, out _);
    c.Carritos.Deshacer(k, out _);
    c.Carritos.Deshacer(k, out _);
    Comprobar("Tras deshacer todo: carrito y pila vacíos", k.EstaVacio && k.AccionesParaDeshacer == 0);
    Comprobar("Con la pila vacía: \"No hay acciones para deshacer.\"",
        !c.Carritos.Deshacer(k, out string mensajeDeshacer) && mensajeDeshacer == "No hay acciones para deshacer.");

    // ------------------------------------------------------------------
    Titulo("Entradas inválidas");
    foreach (string texto in new[] { "0", "-1", "1.5", "100", "abc", "", "99999999999", "1 2" })
    {
        Comprobar($"Cantidad \"{texto}\" rechazada", !InputHelper.TryLeerEntero(texto, "Cantidad", 1, 99, out _, out _));
    }
    Comprobar("Cantidad \"7\" aceptada", InputHelper.TryLeerEntero(" 7 ", "Cantidad", 1, 99, out int siete, out _) && siete == 7);
    Comprobar("Decimal \"12,50\" se lee como 12.50", InputHelper.TryLeerDecimal("12,50", "Monto", 0.01m, 1000m, out decimal monto, out _) && monto == 12.50m);
    Comprobar("Decimal \"1,234\" (miles ambiguos) rechazado", !InputHelper.TryLeerDecimal("1,234", "Monto", 0.01m, 100000m, out _, out _));
    Comprobar("Decimal \"1.234,56\" rechazado", !InputHelper.TryLeerDecimal("1.234,56", "Monto", 0.01m, 100000m, out _, out _));
    Comprobar("Double \"NaN\" rechazado", !InputHelper.TryLeerDouble("NaN", "Peso", 0.1, 1000, 2, out _, out _));
    Comprobar("Double \"4.5\" aceptado", InputHelper.TryLeerDouble("4.5", "Peso", 0.1, 1000, 2, out double peso, out _) && peso == 4.5);
    Comprobar("DNI \"01234567\" válido y conserva el cero", InputHelper.TryValidarDni("01234567", out string dniCero, out _) && dniCero == "01234567");
    Comprobar("DNI \"1234\" inválido", !InputHelper.TryValidarDni("1234", out _, out _));
    Comprobar("Correo \"abc\" inválido", !InputHelper.TryValidarCorreo("abc", out _, out _));
    Comprobar("Correo vacío permitido (opcional)", InputHelper.TryValidarCorreo("  ", out string? sinCorreo, out _) && sinCorreo == null);
    Comprobar("Nombre con tilde, ñ, guion y apóstrofo aceptado",
        InputHelper.TryValidarNombrePersona("  María-José   O'Higgins Ñuñez ", out string? nombreLimpio, out _) && nombreLimpio == "María-José O'Higgins Ñuñez");

    ContextoCarrito k2 = c.Carritos.Actual;
    c.Carritos.AgregarPrenda(k2, Cuerno, 1, out _);
    int accionesAntes = k2.AccionesParaDeshacer;
    int filaAntes = c.Compras.ClientesEnFila;
    bool aceptoExceso = c.Carritos.AgregarPrenda(k2, Cuerno, 20, out _);        // 1 + 20 = 21 > stock 20
    bool aceptoCero = c.Carritos.AgregarPrenda(k2, Cuerno, 0, out _);
    bool aceptoCambioCero = c.Carritos.CambiarCantidad(k2, Cuerno, 0, out _);
    bool aceptoMismaCantidad = c.Carritos.CambiarCantidad(k2, Cuerno, 1, out _);
    Comprobar("Mayor al stock, 0 y cantidad sin cambios: rechazados sin tocar lista ni pila",
        !aceptoExceso && !aceptoCero && !aceptoCambioCero && !aceptoMismaCantidad
        && k2.AccionesParaDeshacer == accionesAntes && c.Carritos.CantidadEnCarrito(k2, Cuerno) == 1);
    Comprobar("DNI inválido: no se encola",
        !c.Compras.RegistrarLlegada("Ana Prueba", "1234", null, out _, out string mensajeDni) && mensajeDni.StartsWith("DNI inválido") && c.Compras.ClientesEnFila == filaAntes);
    Comprobar("Correo inválido: no se encola",
        !c.Compras.RegistrarLlegada("Ana Prueba", "70000021", "abc", out _, out string mensajeCorreo) && mensajeCorreo.StartsWith("Correo inválido") && c.Compras.ClientesEnFila == filaAntes);
    c.Carritos.Vaciar(k2, out _);
    Comprobar("Carrito vacío: no se envía a la fila", !c.Compras.RegistrarLlegada(null, null, null, out _, out _) && c.Compras.ClientesEnFila == filaAntes);

    // ------------------------------------------------------------------
    Titulo("Venta y Postventa: escenarios A y B, DNI, número y fecha");
    Entorno v = await CrearEntornoAsync("ventas", SemillaRipley);
    RegistrarCompra(v, new[] { (Fepapaya, 1), (Estre, 1), (Falda, 1) }, "Cliente Prueba", "70000021", "cliente.prueba@correo.com");
    v.Compras.AtenderSiguiente(out _);
    Venta compraA = v.Compras.CompraEnAtencion!;
    v.Compras.SeleccionarTarjeta(TipoTarjeta.OtraTarjeta, out _);
    Comprobar("A con Otra tarjeta: final 199.97 y ahorro 0.00", compraA.TotalFinal == 199.97m && compraA.Ahorro == 0.00m);
    v.Compras.SeleccionarTarjeta(TipoTarjeta.TarjetaRipley, out _);      // cambio de tarjeta antes de confirmar
    Comprobar("A con Tarjeta Ripley: regular 199.97, final 179.97, ahorro 20.00",
        compraA.TotalRegular == 199.97m && compraA.TotalFinal == 179.97m && compraA.Ahorro == 20.00m);
    Comprobar("A: operación gravada 152.52 e IGV 27.45", compraA.OperacionGravada == 152.52m && compraA.Igv == 27.45m,
        $"{compraA.OperacionGravada} / {compraA.Igv}");
    bool confirmadaA = v.Compras.ConfirmarPagoSimulado(out Venta? ventaA, out _);
    Comprobar("A confirmada: boleta N° 1 asociada a la venta", confirmadaA && ventaA?.Boleta?.Numero == 1 && ventaA.Boleta.IdVenta == ventaA.Id);
    Comprobar("Confirmar de nuevo no duplica la boleta", !v.Compras.ConfirmarPagoSimulado(out _, out _) && v.Boletas.CantidadVentas == 1);
    Comprobar("Stock descontado una sola vez (FEPAPAYA 19)", v.Catalogo.StockDisponible(Fepapaya) == 19);

    RegistrarCompra(v, new[] { (Jeans, 1), (Cuerno, 1), (Destar, 1), (Detej, 1) }, "Cliente Prueba", "70000021");
    v.Compras.AtenderSiguiente(out _);
    Venta compraB = v.Compras.CompraEnAtencion!;
    v.Compras.SeleccionarTarjeta(TipoTarjeta.TarjetaRipley, out _);
    Comprobar("B con Tarjeta Ripley: 149.96 y ahorro 0.00", compraB.TotalFinal == 149.96m && compraB.Ahorro == 0.00m);
    v.Compras.SeleccionarTarjeta(TipoTarjeta.OtraTarjeta, out _);
    Comprobar("B con Otra tarjeta: 149.96, op. gravada 127.08, IGV 22.88",
        compraB.TotalFinal == 149.96m && compraB.Ahorro == 0.00m && compraB.OperacionGravada == 127.08m && compraB.Igv == 22.88m,
        $"{compraB.OperacionGravada} / {compraB.Igv}");
    v.Compras.ConfirmarPagoSimulado(out _, out _);

    RegistrarCompra(v, new[] { (Cuerno, 1) });                                    // compra anónima
    RegistrarCompra(v, new[] { (Detej, 2) }, "Otra Clienta", "70000099");
    v.Compras.AtenderSiguiente(out _);
    Comprobar("No se atiende otra compra mientras hay una activa",
        !v.Compras.AtenderSiguiente(out _) && v.Compras.CompraEnAtencion?.Turno == 3);
    v.Compras.SeleccionarTarjeta(TipoTarjeta.OtraTarjeta, out _);
    v.Compras.ConfirmarPagoSimulado(out Venta? anonima, out _);
    Comprobar("Compra anónima: boleta sin nombre ni DNI", anonima?.Boleta?.Numero == 3 && anonima.Dni == null && anonima.NombreCliente == null);
    v.Compras.AtenderSiguiente(out _);
    v.Compras.SeleccionarTarjeta(TipoTarjeta.TarjetaRipley, out _);
    v.Compras.ConfirmarPagoSimulado(out _, out _);

    ListaEnlazada<Venta> delDni = v.Boletas.BuscarPorDni("70000021", out _);
    List<int> numerosDelDni = new List<int>();
    foreach (Venta venta in delDni)
    {
        numerosDelDni.Add(venta.Boleta!.Numero);
    }
    Comprobar("DNI 70000021: devuelve sus dos boletas (1 y 2)", string.Join(",", numerosDelDni) == "1,2", string.Join(",", numerosDelDni));
    Comprobar("DNI 70000099: solo su boleta", v.Boletas.BuscarPorDni("70000099", out _).Cantidad == 1);
    Comprobar("DNI sin compras: ninguna boleta", v.Boletas.BuscarPorDni("12345678", out _).Cantidad == 0);
    Comprobar("Boleta N° 2 encontrada en el árbol", v.Boletas.BuscarPorNumero(2, out Venta? b2, out _) && b2 != null && b2.TotalFinal == 149.96m);
    Comprobar("Boleta N° 99: no se encontró", !v.Boletas.BuscarPorNumero(99, out _, out string mensaje99) && mensaje99.StartsWith("No se encontró"));
    Comprobar("Fecha de hoy: 4 boletas", v.Boletas.BuscarPorFecha(DateTime.Today, out _).Cantidad == 4);
    Comprobar("Fecha de ayer: ninguna boleta", v.Boletas.BuscarPorFecha(DateTime.Today.AddDays(-1), out _).Cantidad == 0);
    string comprobante = v.Boletas.ObtenerComprobante(ventaA!);
    Comprobar("Comprobante con leyenda, tarjeta, ahorro, IGV y total", comprobante.Contains(Boleta.LeyendaSimulada)
        && comprobante.Contains("Tarjeta Ripley") && comprobante.Contains("S/ 20.00") && comprobante.Contains("S/ 27.45") && comprobante.Contains("S/ 179.97"));

    Comprobar("Bitácoras con registros de los tres subprocesos",
        !v.Carritos.Bitacora.EstaVacia && !v.Compras.Bitacora.EstaVacia && !v.Boletas.Bitacora.EstaVacia);

    // ------------------------------------------------------------------
    Titulo("Cerrar y reabrir (persistencia JSON)");
    Entorno r = Armar(v.Datos);           // nuevas instancias que leen los mismos archivos
    Comprobar("Se recargan las 4 ventas", r.Boletas.CantidadVentas == 4, $"{r.Boletas.CantidadVentas}");
    Comprobar("Siguiente número = máximo + 1 = 5", r.Boletas.SiguienteNumero == 5, $"{r.Boletas.SiguienteNumero}");
    r.Boletas.BuscarPorNumero(1, out Venta? recargada, out _);
    Comprobar("Boleta 1 conserva tarjeta, ahorro, precio aplicado y fecha de emisión",
        recargada != null && recargada.Tarjeta == TipoTarjeta.TarjetaRipley && recargada.Ahorro == 20.00m
        && recargada.Articulos[0].PrecioAplicado == 39.99m && recargada.Boleta!.FechaEmision == ventaA!.Boleta!.FechaEmision);
    Comprobar("Stock guardado (FEPAPAYA 19)", r.Catalogo.StockDisponible(Fepapaya) == 19);

    using (JsonDocument documento = JsonDocument.Parse(File.ReadAllText(v.Datos.RutaVentas)))
    {
        bool coherente = true;
        foreach (JsonElement elemento in documento.RootElement.EnumerateArray())
        {
            JsonElement boleta = elemento.GetProperty("Boleta");
            bool idCoincide = boleta.GetProperty("IdVenta").GetString() == elemento.GetProperty("Id").GetString();
            bool sinDuplicar = !boleta.TryGetProperty("Articulos", out _) && !boleta.TryGetProperty("TotalFinal", out _)
                               && !boleta.TryGetProperty("Venta", out _);
            coherente = coherente && idCoincide && sinDuplicar;
        }
        Comprobar("ventas.json: Boleta.IdVenta = Venta.Id, sin ciclos ni importes duplicados en Boleta", coherente);
    }

    // ------------------------------------------------------------------
    Titulo("Falta de stock al finalizar una compra que esperaba");
    Entorno s = await CrearEntornoAsync("stock", SemillaRipley);
    RegistrarCompra(s, new[] { (Jeans, 15) });
    RegistrarCompra(s, new[] { (Jeans, 10) });
    s.Compras.AtenderSiguiente(out _);
    s.Compras.SeleccionarTarjeta(TipoTarjeta.TarjetaRipley, out _);
    s.Compras.ConfirmarPagoSimulado(out _, out _);
    s.Compras.AtenderSiguiente(out _);
    s.Compras.SeleccionarTarjeta(TipoTarjeta.OtraTarjeta, out _);
    bool sinStock = s.Compras.ConfirmarPagoSimulado(out _, out string mensajeStock);
    Comprobar("Conflicto detectado y la compra sigue en atención",
        !sinStock && mensajeStock.StartsWith("Falta stock") && s.Compras.CompraEnAtencion?.Turno == 2 && s.Boletas.CantidadVentas == 1);
    s.Compras.CorregirCantidad(Jeans, 5, out _);
    Comprobar("Tras corregir a 5 unidades se confirma y el stock queda en 0",
        s.Compras.ConfirmarPagoSimulado(out _, out _) && s.Catalogo.StockDisponible(Jeans) == 0);

    // ------------------------------------------------------------------
    Titulo("Pico de 12 clientes (FIFO)");
    Entorno p = await CrearEntornoAsync("pico", SemillaRipley);
    string[] codigos = { Fepapaya, Estre, Falda, Jeans, Cuerno, Destar, Detej };
    bool pilasNuevas = true;
    for (int i = 1; i <= 12; i++)
    {
        RegistrarCompra(p, new[] { (codigos[(i - 1) % 7], 1) });
        pilasNuevas = pilasNuevas && p.Carritos.Actual.AccionesParaDeshacer == 0 && p.Carritos.Actual.EstaVacio;
    }
    Comprobar("12 clientes en la fila", p.Compras.ClientesEnFila == 12);
    Comprobar("Cada cliente nuevo empieza con carrito y pila vacíos", pilasNuevas);
    bool enOrden = true;
    bool sinMezcla = true;
    for (int i = 1; i <= 12; i++)
    {
        p.Compras.AtenderSiguiente(out _);
        Venta? atendida = p.Compras.CompraEnAtencion;
        enOrden = enOrden && atendida != null && atendida.Turno == i;
        sinMezcla = sinMezcla && atendida != null && atendida.Articulos.Count == 1
                    && atendida.Articulos[0].Codigo == codigos[(i - 1) % 7] && atendida.Articulos[0].Cantidad == 1;
        p.Compras.SeleccionarTarjeta(TipoTarjeta.OtraTarjeta, out _);
        p.Compras.ConfirmarPagoSimulado(out _, out _);
    }
    Comprobar("Turnos 1 a 12 atendidos en orden de llegada", enOrden);
    Comprobar("Ninguna compra mezcla artículos de otra", sinMezcla);
    Comprobar("Fila vacía y 12 boletas emitidas", p.Compras.ClientesEnFila == 0 && p.Boletas.CantidadVentas == 12);

    // ------------------------------------------------------------------
    Titulo("Archivos: inexistente, vacío válido, dañado y error de escritura");
    Entorno nuevo = await CrearEntornoAsync("archivos", SemillaRipley);
    Comprobar("JSON inexistente: se crean productos.json y ventas.json = []",
        File.Exists(nuevo.Datos.RutaProductos) && File.ReadAllText(nuevo.Datos.RutaVentas).Trim() == "[]");
    Comprobar("[] es una colección vacía válida", nuevo.Boletas.ArchivoDisponible && nuevo.Boletas.CantidadVentas == 0);

    File.WriteAllText(nuevo.Datos.RutaVentas, "{ esto no es JSON");
    BoletaService danada = new BoletaService(nuevo.Datos);
    bool cargoDanada = danada.Cargar(out _);
    Comprobar("JSON dañado: se informa el error y el archivo no se modifica",
        !cargoDanada && !danada.ArchivoDisponible && File.ReadAllText(nuevo.Datos.RutaVentas) == "{ esto no es JSON");
    File.WriteAllText(nuevo.Datos.RutaVentas, "");
    Comprobar("Archivo vacío (sin JSON) no se toma como []", !new BoletaService(nuevo.Datos).Cargar(out _));

    if (OperatingSystem.IsWindows())
    {
        Entorno w = await CrearEntornoAsync("escritura", SemillaRipley);
        RegistrarCompra(w, new[] { (Fepapaya, 1) }, "Cliente Prueba");
        w.Compras.AtenderSiguiente(out _);
        w.Compras.SeleccionarTarjeta(TipoTarjeta.TarjetaRipley, out _);

        File.SetAttributes(w.Datos.RutaVentas, FileAttributes.ReadOnly);       // simula que ventas.json no se puede escribir
        bool conVentasBloqueado = w.Compras.ConfirmarPagoSimulado(out _, out _);
        File.SetAttributes(w.Datos.RutaVentas, FileAttributes.Normal);
        Comprobar("Error al guardar ventas.json: no se anuncia venta y el cliente sigue en atención",
            !conVentasBloqueado && w.Compras.CompraEnAtencion != null && w.Boletas.CantidadVentas == 0 && w.Boletas.SiguienteNumero == 1);

        File.SetAttributes(w.Datos.RutaProductos, FileAttributes.ReadOnly);    // ventas se guarda, productos falla
        bool conProductosBloqueado = w.Compras.ConfirmarPagoSimulado(out _, out _);
        File.SetAttributes(w.Datos.RutaProductos, FileAttributes.Normal);
        BoletaService enDisco = new BoletaService(w.Datos);
        enDisco.Cargar(out _);
        Comprobar("Error al guardar productos.json: se restaura ventas.json y no se descuenta stock",
            !conProductosBloqueado && enDisco.CantidadVentas == 0 && w.Catalogo.StockDisponible(Fepapaya) == 20
            && w.Compras.CompraEnAtencion?.Boleta == null);

        bool reintento = w.Compras.ConfirmarPagoSimulado(out Venta? reintentada, out _);
        Comprobar("Reintento: una sola boleta (N° 1) y stock descontado una vez",
            reintento && reintentada?.Boleta?.Numero == 1 && w.Boletas.CantidadVentas == 1 && w.Catalogo.StockDisponible(Fepapaya) == 19);
    }
    else
    {
        Console.WriteLine("[OMITIDA] Error de escritura: la simulación con archivos de solo lectura es para Windows.");
    }

    // ------------------------------------------------------------------
    Titulo("Grafo (complemento del desafío)");
    GrafoCajas grafo = GrafoCajas.CrearEjemplo();
    bool hayRuta = grafo.BuscarRutaMinima(0, 5, out ListaEnlazada<int> ruta, out double distancia);
    Comprobar("Ruta mínima Caja 511 → Central: 26 ms por 512, 513 y servidor",
        hayRuta && distancia == 26 && grafo.DescribirRuta(ruta) == "Caja 511 → Caja 512 → Caja 513 → Servidor de tienda → Central (simulada)",
        $"{distancia} {grafo.DescribirRuta(ruta)}");
    grafo.CambiarEstadoEnlace(3, 5, false, out _);
    hayRuta = grafo.BuscarRutaMinima(0, 5, out ruta, out distancia);
    Comprobar("Enlace Servidor–Central caído: ruta por el Almacén, 34 ms", hayRuta && distancia == 34, $"{distancia}");
    grafo.CambiarEstadoEnlace(4, 5, false, out _);
    Comprobar("Sin enlaces activos hacia Central: no existe ruta", !grafo.BuscarRutaMinima(0, 5, out _, out _));
}
finally
{
    try
    {
        Directory.Delete(carpetaBase, true);
    }
    catch (Exception)
    {
        Console.WriteLine("(No se pudo borrar la carpeta temporal " + carpetaBase + ")");
    }
}

Console.WriteLine();
Console.WriteLine($"Resultado: {correctas} correctas, {fallidas} con falla. Ejecutado el {DateTime.Now:dd/MM/yyyy HH:mm:ss}.");
return fallidas == 0 ? 0 : 1;

/// <summary>Servicios de una ejecución de prueba, conectados como en MauiProgram.</summary>
record Entorno(JsonDataManager Datos, CatalogoService Catalogo, CarritoService Carritos, BoletaService Boletas, CompraService Compras);
```
