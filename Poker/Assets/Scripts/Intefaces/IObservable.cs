using System;

public interface IObservable<T> 
{
    event Action<T> OnChanged;
}
