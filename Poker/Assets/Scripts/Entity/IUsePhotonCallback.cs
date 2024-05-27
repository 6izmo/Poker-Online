using System;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public abstract class IUsePhotonCallback : IOnEventCallback, IDisposable
{
    public IUsePhotonCallback()
    {
        PhotonNetwork.AddCallbackTarget(this);
        this.Sign();
    }

    public void Dispose() => PhotonNetwork.RemoveCallbackTarget(this);

    public abstract void OnEvent(EventData photonEvent);
}
