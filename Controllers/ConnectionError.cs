using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;

public class ConnectionError : MonoBehaviour
{
    [SerializeField]
    GameObject TryToConnectErrorMessagePrefab;
    [SerializeField]
    GameObject TryToConnectErrorMessage;
  
    //bool isMessageSpawned = false;

    // Update is called once per frame
    //void Update()
    //{
    //    if (!isMessageSpawned)
    //    {
    //        SpawnPrefabMessage();
    //    }
    //
    //    if (PhotonNetwork.IsConnected)
    //    {
    //        HideMessage();
    //    }
    //    else
    //    {
    //        ShowMessage();
    //    }
    //}

    public void ShowMessage()
    {
        //if (!isMessageSpawned)
        //{
        //    SpawnPrefabMessage();
        //
        //    isMessageSpawned = true;
        //}

        TryToConnectErrorMessage.SetActive(true);
    }

    public void HideMessage()
    {
        //if (!isMessageSpawned)
        //{
        //    SpawnPrefabMessage();
        //
        //    isMessageSpawned = true;
        //}

        TryToConnectErrorMessage.SetActive(false);
    }

    void SpawnPrefabMessage()
    {
        GameObject tempCanvas = FindCanvasObj();
        TryToConnectErrorMessage = Instantiate(TryToConnectErrorMessagePrefab, TryToConnectErrorMessagePrefab.transform.position, Quaternion.identity, tempCanvas.transform);
        TryToConnectErrorMessage.SetActive(false);
    }

    GameObject FindCanvasObj()
    {
        return GameObject.Find("Canvas");
    }


}
