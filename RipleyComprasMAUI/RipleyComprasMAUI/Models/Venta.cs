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
