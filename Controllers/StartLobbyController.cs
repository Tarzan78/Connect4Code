using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class StartLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject startButton;
    [SerializeField]
    private GameObject cancelButton;
    [SerializeField]
    private int roomSize;

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        startButton.SetActive(true);
    }

    public void QuickStart(){
        startButton.SetActive(false);
        cancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Quick Start");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a room");
        CreateRoom();
    }

    private void CreateRoom(){
        Debug.Log("Creating a new room");
        int randomRoomNumber = Random.Range(0, 10000);
        RoomOptions roomOps =
        new RoomOptions(){IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize};
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOps);
        Debug.Log(randomRoomNumber);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create a new room... trying again");
        CreateRoom();
    }

    public void QuickCancel(){
        cancelButton.SetActive(false);
        startButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }
}
