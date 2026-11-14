using System;
using System.Collections;
using System.Collections.Generic;

namespace RipleyComprasMAUI.Structures;

/// <summary>
/// Cola FIFO enlazada con referencias al frente y al final: la primera compra
/// que llega es la primera que se atiende. Encolar y desencolar cuestan O(1).
/// </summary>
public class ColaCompras<T> : IEnumerable<T>
{
    private NodoSimple<T>? frente;
    private NodoSimple<T>? final;

    public int Cantidad { get; private set; }
    public bool EstaVacia => frente == null;

    /// <summary>O(1): entra por el final.</summary>
    public void Encolar(T dato)
    {
        NodoSimple<T> nuevo = new NodoSimple<T>(dato);
        if (final == null)
        {
            frente = nuevo;              // cola vacía: frente y final son el mismo nodo
            final = nuevo;
        }
        else
        {
            final.Siguiente = nuevo;     // el final anterior apunta al nuevo
            final = nuevo;
        }
        Cantidad++;
    }

    /// <summary>O(1): sale el frente (el que llegó primero).</summary>
    public T Desencolar()
    {
        if (frente == null)
        {
            throw new InvalidOperationException("La cola está vacía.");
        }
        T dato = frente.Dato;
        frente = frente.Siguiente;
        if (frente == null)
        {
            final = null;                // la cola quedó vacía
        }
        Cantidad--;
        return dato;
    }

    /// <summary>O(1): consulta el frente sin retirarlo.</summary>
    public T? VerFrente()
    {
        return frente == null ? default : frente.Dato;
    }

    /// <summary>Recorrido O(n) del frente al final, solo para mostrar la fila.</summary>
    public IEnumerator<T> GetEnumerator()
    {
        NodoSimple<T>? actual = frente;
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
