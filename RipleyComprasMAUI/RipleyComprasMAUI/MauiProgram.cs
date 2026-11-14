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
