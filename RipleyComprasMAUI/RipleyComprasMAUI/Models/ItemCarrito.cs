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
