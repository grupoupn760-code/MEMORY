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
