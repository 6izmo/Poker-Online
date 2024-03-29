using UnityEngine;
using System.Collections.Generic;

public static class MyExtensions
{
    public static LinkedListNode<T> RemoveAndGetFirst<T>(this LinkedList<T> list)
    {
        LinkedListNode<T> elem = list.First;
        list.RemoveFirst();
        return elem;
    }

    public static LinkedListNode<T> RemoveAndGetLast<T>(this LinkedList<T> list)
    {
        LinkedListNode<T> elem = list.Last;
        list.RemoveLast();
        return elem;
    }

    public static void Activate(this Component component) => component.gameObject.SetActive(true);

    public static void Activate(this GameObject gameObject) => gameObject.SetActive(true);

    public static void Deactivate(this Component component) => component.gameObject.SetActive(false);

    public static void Deactivate(this GameObject gameObject) => gameObject.SetActive(false);
}

