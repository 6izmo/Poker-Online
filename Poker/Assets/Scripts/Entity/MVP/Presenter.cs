using System;

public abstract class Presenter : IDisposable
{
    public Presenter() => this.Sign();

    public abstract void Dispose();
}
