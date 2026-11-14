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
