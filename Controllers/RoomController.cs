using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomController : MonoBehaviourPunCallbacks
{
    bool inTest = false;
    [SerializeField]
    private int multiplayerSceneIndex;
    [SerializeField]
    private GameObject lobbyPanel;
    [SerializeField]
    private GameObject roomPanel;
    [SerializeField]
    private GameObject startButton;
    [SerializeField]
    private GameObject waittingStartButton;
    [SerializeField]
    private Transform playersContainer;
    [SerializeField]
    private GameObject playerListingsPrefab;
    [SerializeField]
    private Text roomNameDisplay;
    [SerializeField]
    int roomSize = 2;

    private void ClearPlayerListings(){
        for (int i = playersContainer.childCount - 1; i >= 0; i--){
            Destroy(playersContainer.GetChild(i).gameObject);
        }
    }

    private void Update()
    {
        if (inTest)
        {
            ClearPlayerListings();
            ListPlayers();
        }

        UpdateStratButton();

    }
    private void ListPlayers(){
        foreach(Player player in PhotonNetwork.PlayerList){
            GameObject tempListing = Instantiate(playerListingsPrefab, playersContainer);
            TextMeshProUGUI tempText = tempListing.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            tempText.text = player.NickName;
            if (inTest)
            {
                Debug.Log("Player Nick Name -> " + player.NickName + " tempText -> " + tempText.text);
            }
        }
    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomNameDisplay.text = PhotonNetwork.CurrentRoom.Name;
        if(PhotonNetwork.IsMasterClient){
            startButton.SetActive(false);
            waittingStartButton.SetActive(true);
        }else{
            startButton.SetActive(false);
            waittingStartButton.SetActive(false);
        }
        ClearPlayerListings();
        ListPlayers();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        ClearPlayerListings();
        ListPlayers();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ClearPlayerListings();
        ListPlayers();
        if(PhotonNetwork.IsMasterClient){
            startButton.SetActive(false);
            waittingStartButton.SetActive(true);
        }
    }

    public void StartGame(){
        if(PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length == roomSize)
        {
            startButton.SetActive(true);
            waittingStartButton.SetActive(false);
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(multiplayerSceneIndex);
        }
    }

    void UpdateStratButton()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length == roomSize)
        {
            startButton.SetActive(true);
            waittingStartButton.SetActive(false);
        }
    }

    public void BackOnClick(){
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
        PhotonNetwork.LeaveRoom();

        if (PhotonNetwork.IsConnected)
        {   
            PhotonNetwork.LeaveLobby();
        }

        Invoke("RejoinLobby", 1f);
    }

    private void RejoinLobby(){
        PhotonNetwork.JoinLobby();
    }
}
