using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class RoomButton : MonoBehaviour
{
    //[SerializeField]
    //private Text nameText;
    //[SerializeField]
    //private Text sizeText;
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI sizeText;

    private string roomName;
    private int roomSize;
    private int playerCount;

    public void JoinRoomOnClick(){
        PhotonNetwork.JoinRoom(roomName);
    }

    public void SetRoom(string nameInput, int sizeInput, int countInput){
        roomName = nameInput;
        roomSize = sizeInput;
        playerCount = countInput;
        nameText.text = roomName;
        sizeText.text = playerCount + "/" + roomSize;
    }
}
