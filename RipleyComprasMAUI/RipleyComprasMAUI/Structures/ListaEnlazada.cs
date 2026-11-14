using System;
using System.Collections;
using System.Collections.Generic;

namespace RipleyComprasMAUI.Structures;

/// <summary>
/// Lista enlazada simple implementada a mano. Guarda la cabeza y el último nodo
/// para agregar al final en O(1). Se usa para el carrito, el historial de ventas,
/// las bitácoras y las listas de adyacencia del grafo.
/// </summary>
public class ListaEnlazada<T> : IEnumerable<T>
{
    private NodoSimple<T>? cabeza;
    private NodoSimple<T>? ultimo;

    public int Cantidad { get; private set; }
    public bool EstaVacia => cabeza == null;

    /// <summary>O(1): el nuevo nodo se enlaza después del último.</summary>
    public void AgregarAlFinal(T dato)
    {
        NodoSimple<T> nuevo = new NodoSimple<T>(dato);
        if (ultimo == null)
        {
            cabeza = nuevo;              // lista vacía: el nuevo es cabeza y último
            ultimo = nuevo;
        }
        else
        {
            ultimo.Siguiente = nuevo;    // el último anterior apunta al nuevo
            ultimo = nuevo;
        }
        Cantidad++;
    }

    /// <summary>O(1): el nuevo nodo apunta a la cabeza anterior y pasa a ser la cabeza.</summary>
    public void AgregarAlInicio(T dato)
    {
        NodoSimple<T> nuevo = new NodoSimple<T>(dato);
        nuevo.Siguiente = cabeza;
        cabeza = nuevo;
        if (ultimo == null)
        {
            ultimo = nuevo;
        }
        Cantidad++;
    }

    /// <summary>
    /// O(n): inserta en una posición (0 = inicio). Deshacer la usa para devolver
    /// una prenda quitada a su lugar original.
    /// </summary>
    public void InsertarEn(int posicion, T dato)
    {
        if (posicion <= 0 || cabeza == null)
        {
            AgregarAlInicio(dato);
            return;
        }
        if (posicion >= Cantidad)
        {
            AgregarAlFinal(dato);
            return;
        }

        NodoSimple<T> anterior = cabeza;
        for (int i = 0; i < posicion - 1; i++)
        {
            anterior = anterior.Siguiente!;            // avanza hasta el nodo previo a la posición
        }
        NodoSimple<T> nuevo = new NodoSimple<T>(dato);
        nuevo.Siguiente = anterior.Siguiente;          // el nuevo apunta al que estaba en esa posición
        anterior.Siguiente = nuevo;                    // el anterior apunta al nuevo
        Cantidad++;
    }

    /// <summary>O(n): devuelve el primer dato que cumple la condición, o default si no existe.</summary>
    public T? Buscar(Func<T, bool> condicion)
    {
        NodoSimple<T>? actual = cabeza;
        while (actual != null)
        {
            if (condicion(actual.Dato))
            {
                return actual.Dato;
            }
            actual = actual.Siguiente;
        }
        return default;
    }

    /// <summary>
    /// O(n): quita el primer dato que cumple la condición. Localizarlo cuesta O(n);
    /// después solo se reenlaza un nodo, sin desplazar elementos. Devuelve la posición que ocupaba.
    /// </summary>
    public bool Eliminar(Func<T, bool> condicion, out T? eliminado, out int posicion)
    {
        NodoSimple<T>? anterior = null;
        NodoSimple<T>? actual = cabeza;
        posicion = 0;

        while (actual != null)
        {
            if (condicion(actual.Dato))
            {
                if (anterior == null)
                {
                    cabeza = actual.Siguiente;               // se quitó la cabeza
                }
                else
                {
                    anterior.Siguiente = actual.Siguiente;   // el anterior "salta" el nodo quitado
                }
                if (actual == ultimo)
                {
                    ultimo = anterior;                       // se quitó el último
                }
                Cantidad--;
                eliminado = actual.Dato;
                return true;
            }
            anterior = actual;
            actual = actual.Siguiente;
            posicion++;
        }

        eliminado = default;
        posicion = -1;
        return false;
    }

    public void Limpiar()
    {
        cabeza = null;
        ultimo = null;
        Cantidad = 0;
    }

    /// <summary>Recorrido O(n) desde la cabeza hasta el último nodo (permite usar foreach).</summary>
    public IEnumerator<T> GetEnumerator()
    {
        NodoSimple<T>? actual = cabeza;
        while (actual != null)
        {
            yield return actual.Dato;
            actual = actual.Siguiente;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
