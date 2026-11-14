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
