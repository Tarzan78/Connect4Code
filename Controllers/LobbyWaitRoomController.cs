using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class LobbyWaitRoomController : MonoBehaviourPunCallbacks
{
    private PhotonView myPhotonView;

    [SerializeField]
    private int multiplayerLevelSceneIndex;
    [SerializeField]
    private int menuSceneIndex;

    private int playerCount;
    private int roomSize;
    [SerializeField]
    private int minPlayersToStart;
    [SerializeField]
    private Text playerCountDisplay;
    [SerializeField]
    private Text timerDisplay;

    private bool readyToCountDown;
    private bool readyToStart;
    private bool startingGame;

    private float timerToStartGame;
    private float notFullGameTimer;
    private float fullGameTimer;

    [SerializeField]
    private float maxWaitTimer;
    [SerializeField]
    private float maxFullGameTimer;

    private void Start(){
        myPhotonView = GetComponent<PhotonView>();

        fullGameTimer = maxFullGameTimer;
        notFullGameTimer = maxWaitTimer;
        timerToStartGame = maxWaitTimer;

        PlayerCountUpdate();
    }

    private void PlayerCountUpdate(){
        playerCount = PhotonNetwork.PlayerList.Length;
        roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
        playerCountDisplay.text = playerCount + ":" + roomSize;

        if(playerCount == roomSize){
            readyToStart = true;
        }
        else if(playerCount >= minPlayersToStart){
            readyToCountDown = true;
        }
        else{
            readyToCountDown = false;
            readyToStart = false;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerCountUpdate();
        if(PhotonNetwork.IsMasterClient){
            myPhotonView.RPC("RPC_SendTimer", RpcTarget.Others, timerToStartGame);
        }
    }

    [PunRPC]
    private void RPC_SendTimer(float timeIn){
        timerToStartGame = timeIn;
        notFullGameTimer = timeIn;
        if(timeIn < fullGameTimer){
            fullGameTimer = timeIn;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayerCountUpdate();
    }

    private void Update(){
        WaitForMorePlayers();
    }

    private void WaitForMorePlayers(){
        if(playerCount < minPlayersToStart){
            ResetTimer();
        }

        if(readyToStart){
            fullGameTimer -= Time.deltaTime;
            timerToStartGame = fullGameTimer;
        }
        else if(readyToCountDown){
            notFullGameTimer -= Time.deltaTime;
            timerToStartGame = notFullGameTimer;
        }

        string tempTimer = string.Format("{0:00}", timerToStartGame);
        timerDisplay.text = tempTimer;

        if(timerToStartGame <= 0){
            if(startingGame){
                return;
            }
            StartGame();
        }
    }

    private void ResetTimer(){
        timerToStartGame = maxWaitTimer;
        notFullGameTimer = maxWaitTimer;
        fullGameTimer = maxFullGameTimer;
    }

    private void StartGame(){
        startingGame = true;
        if(!PhotonNetwork.IsMasterClient){
            return;
        }
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(multiplayerLevelSceneIndex);
    }

    public void WaitCancel(){
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(menuSceneIndex);
    }

}
