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
