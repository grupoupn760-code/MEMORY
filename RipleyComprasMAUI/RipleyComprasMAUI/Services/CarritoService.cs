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
