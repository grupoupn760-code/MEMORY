using System;
using System.Collections.Generic;

namespace RipleyComprasMAUI.Structures;

/// <summary>
/// Árbol binario de búsqueda con balanceo AVL. Las claves menores van a la izquierda y las
/// mayores a la derecha; tras cada inserción se corrige el desequilibrio con rotaciones, así la
/// altura se mantiene cerca de log2(n) y buscar cuesta O(log n).
/// CatalogoService lo usa con clave Codigo (string) y BoletaService con clave Boleta.Numero (int).
/// Solo incluye inserción, búsqueda, recorrido inorden, altura y rotaciones (no se necesita eliminar).
/// </summary>
public class ArbolBusqueda<TClave, TValor>
{
    private readonly IComparer<TClave> comparador;
    private NodoArbol<TClave, TValor>? raiz;

    public ArbolBusqueda(IComparer<TClave>? comparador = null)
    {
        // Para códigos se usa comparación ordinal: compara carácter por carácter, sin reglas de idioma.
        this.comparador = comparador ?? Comparer<TClave>.Default;
    }

    public int Cantidad { get; private set; }
    public int Altura => AlturaDe(raiz);
    public int Rotaciones { get; private set; }               // rotaciones realizadas desde que se creó
    public int UltimasComparaciones { get; private set; }     // nodos visitados en la última búsqueda
    public bool EstaVacio => raiz == null;
    public TClave? ClaveRaiz => raiz == null ? default : raiz.Clave;

    /// <summary>Inserta la clave; devuelve false si ya existía (no se duplican claves).</summary>
    public bool Insertar(TClave clave, TValor valor)
    {
        bool insertado = false;
        raiz = Insertar(raiz, clave, valor, ref insertado);
        if (insertado)
        {
            Cantidad++;
        }
        return insertado;
    }

    private NodoArbol<TClave, TValor> Insertar(NodoArbol<TClave, TValor>? nodo, TClave clave, TValor valor, ref bool insertado)
    {
        if (nodo == null)
        {
            insertado = true;
            return new NodoArbol<TClave, TValor>(clave, valor);       // lugar vacío: nueva hoja
        }

        int comparacion = comparador.Compare(clave, nodo.Clave);
        if (comparacion < 0)
        {
            nodo.Izquierdo = Insertar(nodo.Izquierdo, clave, valor, ref insertado);
        }
        else if (comparacion > 0)
        {
            nodo.Derecho = Insertar(nodo.Derecho, clave, valor, ref insertado);
        }
        else
        {
            return nodo;                                               // clave repetida: no se inserta
        }

        return Balancear(nodo);                                        // al volver de la recursión se reequilibra
    }

    /// <summary>
    /// O(log n): baja desde la raíz comparando la clave; cuenta cada nodo visitado.
    /// </summary>
    public bool Buscar(TClave clave, out TValor? valor)
    {
        UltimasComparaciones = 0;
        NodoArbol<TClave, TValor>? actual = raiz;

        while (actual != null)
        {
            UltimasComparaciones++;
            int comparacion = comparador.Compare(clave, actual.Clave);
            if (comparacion == 0)
            {
                valor = actual.Valor;
                return true;
            }
            actual = comparacion < 0 ? actual.Izquierdo : actual.Derecho;
        }

        valor = default;
        return false;
    }

    /// <summary>O(n): izquierdo, nodo, derecho; devuelve los valores ordenados por clave.</summary>
    public ListaEnlazada<TValor> RecorrerInOrden()
    {
        ListaEnlazada<TValor> resultado = new ListaEnlazada<TValor>();
        RecorrerInOrden(raiz, resultado);
        return resultado;
    }

    private static void RecorrerInOrden(NodoArbol<TClave, TValor>? nodo, ListaEnlazada<TValor> resultado)
    {
        if (nodo == null)
        {
            return;
        }
        RecorrerInOrden(nodo.Izquierdo, resultado);
        resultado.AgregarAlFinal(nodo.Valor);
        RecorrerInOrden(nodo.Derecho, resultado);
    }

    // ===================== Balanceo AVL =====================

    private static int AlturaDe(NodoArbol<TClave, TValor>? nodo)
    {
        return nodo == null ? 0 : nodo.Altura;
    }

    private static void ActualizarAltura(NodoArbol<TClave, TValor> nodo)
    {
        nodo.Altura = 1 + Math.Max(AlturaDe(nodo.Izquierdo), AlturaDe(nodo.Derecho));
    }

    /// <summary>Factor de equilibrio = altura izquierda − altura derecha (debe quedar entre −1 y 1).</summary>
    private static int FactorEquilibrio(NodoArbol<TClave, TValor> nodo)
    {
        return AlturaDe(nodo.Izquierdo) - AlturaDe(nodo.Derecho);
    }

    private NodoArbol<TClave, TValor> Balancear(NodoArbol<TClave, TValor> nodo)
    {
        ActualizarAltura(nodo);
        int factor = FactorEquilibrio(nodo);

        if (factor > 1)                                    // cargado a la izquierda
        {
            if (FactorEquilibrio(nodo.Izquierdo!) < 0)
            {
                nodo.Izquierdo = RotarIzquierda(nodo.Izquierdo!);   // caso izquierda-derecha: rotación doble
            }
            return RotarDerecha(nodo);
        }

        if (factor < -1)                                   // cargado a la derecha
        {
            if (FactorEquilibrio(nodo.Derecho!) > 0)
            {
                nodo.Derecho = RotarDerecha(nodo.Derecho!);         // caso derecha-izquierda: rotación doble
            }
            return RotarIzquierda(nodo);
        }

        return nodo;                                       // ya está equilibrado
    }

    /// <summary>
    /// Rotación a la derecha: el hijo izquierdo sube y el nodo baja a su derecha.
    /// El subárbol derecho del hijo pasa a ser el izquierdo del nodo.
    /// </summary>
    private NodoArbol<TClave, TValor> RotarDerecha(NodoArbol<TClave, TValor> nodo)
    {
        NodoArbol<TClave, TValor> hijo = nodo.Izquierdo!;
        nodo.Izquierdo = hijo.Derecho;
        hijo.Derecho = nodo;
        ActualizarAltura(nodo);          // primero el que quedó abajo
        ActualizarAltura(hijo);
        Rotaciones++;
        return hijo;                     // nueva raíz de este subárbol
    }

    /// <summary>
    /// Rotación a la izquierda: el hijo derecho sube y el nodo baja a su izquierda.
    /// El subárbol izquierdo del hijo pasa a ser el derecho del nodo.
    /// </summary>
    private NodoArbol<TClave, TValor> RotarIzquierda(NodoArbol<TClave, TValor> nodo)
    {
        NodoArbol<TClave, TValor> hijo = nodo.Derecho!;
        nodo.Derecho = hijo.Izquierdo;
        hijo.Izquierdo = nodo;
        ActualizarAltura(nodo);
        ActualizarAltura(hijo);
        Rotaciones++;
        return hijo;
    }
}
