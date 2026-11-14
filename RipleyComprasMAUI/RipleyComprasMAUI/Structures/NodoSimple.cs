namespace RipleyComprasMAUI.Structures;

/// <summary>
/// Nodo de enlace simple: guarda un dato y la referencia al siguiente nodo.
/// Lo reutilizan la lista enlazada, la pila y la cola.
/// </summary>
public class NodoSimple<T>
{
    public T Dato { get; set; }
    public NodoSimple<T>? Siguiente { get; set; }   // null cuando es el último

    public NodoSimple(T dato)
    {
        Dato = dato;
        Siguiente = null;
    }
}
