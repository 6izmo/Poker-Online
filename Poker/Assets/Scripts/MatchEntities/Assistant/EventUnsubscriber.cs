using System;
using System.Collections.Generic;

public static class EventUnsubscriber 
{
    private static List<IDisposable> _disposables = new List<IDisposable>();

    public static void Sign(this IDisposable disposable) => _disposables.Add(disposable);
    
    public static void Execute()   
    {
        foreach (IDisposable disposable in _disposables)
            disposable.Dispose();
        _disposables.Clear();
    }
}
