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
