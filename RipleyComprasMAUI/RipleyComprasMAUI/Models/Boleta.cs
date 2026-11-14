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
