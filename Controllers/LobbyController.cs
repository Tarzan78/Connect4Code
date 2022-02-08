using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyController : MonoBehaviourPunCallbacks
{
    bool inDebugLobby = true;
    [SerializeField]
    private GameObject lobbyConnectButton;
    [SerializeField]
    private GameObject lobbyLoadingButton;
    [SerializeField]
    private GameObject lobbyPanel;
    [SerializeField]
    private GameObject menuPanel;
    //[SerializeField]
    //private InputField playerNameInput;
    [SerializeField]
    private TMP_InputField playerNameInputText;


    private string roomName;
    [SerializeField]
    private int roomSize;

    private List<RoomInfo> roomListings;
    [SerializeField]
    private Transform roomContainer;
    [SerializeField]
    private GameObject roomListingPrefab;

    //private void Start()
    //{
    //    menuPanel.SetActive(true);
    //    lobbyPanel.SetActive(false);
    //}

    //First wait connection 
    public override void OnConnectedToMaster()
    {
        /////////////Problema dos nomes aqui 
        /// porquê a necessidade desta func ?
        /// 
        PhotonNetwork.AutomaticallySyncScene = true;
        lobbyConnectButton.SetActive(true);
        lobbyLoadingButton.SetActive(false);
        roomListings = new List<RoomInfo>();
        if(PlayerPrefs.GetString("NickName") != ""){

            //playerNameInput.text = PlayerPrefs.GetString("NickName");
            playerNameInputText.text = PlayerPrefs.GetString("NickName");

            if (inDebugLobby) Debug.Log("Player Name is -> " + playerNameInputText.text);
        }
    }

    //Second: Pressing Start  go to lobby 
    //there is a need to create the lobby ? 
    public void JoinLobbyOnClick(){
        PlayerPrefs.SetString("NickName", PhotonNetwork.NickName);
        menuPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        PhotonNetwork.JoinLobby();
    }

    //Step 3 create/update list of lobbys available
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int tempIndex = -1;

        //Destroy on scene (Update the rooms available)
        foreach (RoomInfo room in roomList){
            if (roomListings != null){

                //PROF/1\\
                //Sinc the roomListings with the list roomList
                //Finding the one with the same name and get his index 
                for (int i = 0; i < roomListings.Count; i++)
                {
                    if (roomListings[i].Name == room.Name) tempIndex = i;
                }

            }else{

                //Empty
                tempIndex = -1;
            }

            if(tempIndex != -1){
                //talvez desnecessário pois não tem listas porque já foram limpas, mas está simplesmente a ser atualizado é justo, dizer os players a mudar de salas e afins 
                //Destroy the obj in content in UI
                roomListings.RemoveAt(tempIndex);
                Destroy(roomContainer.GetChild(tempIndex).gameObject);
            }

            //if the room have more then 1 player (is created) add to the content in UI
            if(room.PlayerCount > 0){
                roomListings.Add(room);
                ListRoom(room);
            }
        }
    }

    //Step 4 create the objt in list of content in UI
    private void ListRoom(RoomInfo room){

        if (inDebugLobby) Debug.Log("Creating room in content one room with the name: " + room.Name);
        if(room.IsOpen && room.IsVisible){
            GameObject tempListing = Instantiate(roomListingPrefab, roomContainer);
            RoomButton tempButton = tempListing.GetComponent<RoomButton>();
            tempButton.SetRoom(room.Name, room.MaxPlayers, room.PlayerCount);
            if (inDebugLobby) Debug.Log("Creating room in if");
        }
    }

    public void OnRoomNameChanged(TextMeshProUGUI nameIn){
        if (inDebugLobby) Debug.Log("Name Room enter Changed -> " + nameIn.text);
        roomName = nameIn.text;
    }

    public void CreateRoom(){
        Debug.Log("Creating a new room");
        RoomOptions roomOps = new RoomOptions() {IsVisible = true, IsOpen = true, 
            MaxPlayers = (byte)roomSize};
        PhotonNetwork.CreateRoom(roomName, roomOps);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create room failed, there must already be a room with the same name!");
    }

    //back to sart menu
    public void ReturnToMainMenu(){
        menuPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveLobby();
        }

    }

    //Destroy on scene (Update the rooms available)
    public override void OnJoinedLobby()
    {
        for (int i = roomContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(roomContainer.GetChild(i).gameObject);
        }
    }

    public void GoToStartScreen()
    {
        PhotonNetwork.Disconnect();
        //PhotonNetwork.LeaveLobby();
        SceneManager.LoadScene(0);
    }
}
