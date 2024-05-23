using UnityEngine;
using System.Collections.Generic;

public static class CollectionExtensions 
{
    public static void Shuffle<T>(this IList<T> list)
    {
        for (int i = list.Count - 1; i >= 1; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = list[j];
            list[j] = list[i];
            list[i] = temp;
        }
    }

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
}
