namespace RipleyComprasMAUI.Structures;

/// <summary>
/// Nodo del árbol de búsqueda: clave, valor, hijos y altura (necesaria para el balanceo AVL).
/// </summary>
public class NodoArbol<TClave, TValor>
{
    public TClave Clave { get; }
    public TValor Valor { get; set; }
    public NodoArbol<TClave, TValor>? Izquierdo { get; set; }   // claves menores
    public NodoArbol<TClave, TValor>? Derecho { get; set; }     // claves mayores
    public int Altura { get; set; }                             // una hoja tiene altura 1

    public NodoArbol(TClave clave, TValor valor)
    {
        Clave = clave;
        Valor = valor;
        Altura = 1;
    }
}
