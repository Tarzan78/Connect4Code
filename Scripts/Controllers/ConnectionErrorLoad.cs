using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;

public class ConnectionErrorLoad : LoadBalancingClient
{
    //it´s not working
    public override void DebugReturn(DebugLevel level, string message)
    {
        if (this.LoadBalancingPeer.DebugOut != DebugLevel.ALL && level > this.LoadBalancingPeer.DebugOut)
        {
            return;
        }

        if (level == DebugLevel.ERROR)
        {
            //ConnectionError.ShowMessage();
            Debug.LogError(message);
        }
        else if (level == DebugLevel.WARNING)
        {
            Debug.LogWarning(message);
        }
        else if (level == DebugLevel.INFO)
        {
            Debug.Log(message);
        }
        else if (level == DebugLevel.ALL)
        {
            Debug.Log(message);
        }

    }
}
