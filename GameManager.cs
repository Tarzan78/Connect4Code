using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] static GridManager gridManager;
    static public bool player1Turn = false;
    static public bool player2Turn = false;
    static public bool itIsUrTurn = false;
    static public bool playable = true;
    static bool gameEnded = false;
    bool conectionTest = true;
    int masterPoints = 0;
    int invitedPoints = 0;
    string masterName = "LoadingMaster";
    string invited2Name = "LoadingInvited";
    [SerializeField] TextMeshProUGUI UINamePlayer1; //Master
    [SerializeField] TextMeshProUGUI UIPointsPlayer1;
    [SerializeField] TextMeshProUGUI UINamePlayer2; //invited
    [SerializeField] TextMeshProUGUI UIPointsPlayer2;
    [SerializeField] TextMeshProUGUI UIWinner;
    [SerializeField] TextMeshProUGUI UITurnIndicator;
    [SerializeField] GameObject UIMenuRestart;
    [SerializeField] GameObject UIMenuGame;
    [SerializeField] GameObject UIRestartButton;
    [SerializeField] GameObject UIWinByOpponentDisconectedMessage;
    [SerializeField] TextMeshProUGUI UITextWinByOpponentDisconectedMessage;
    [SerializeField] ConnectionError connectionErrorCtrl;

    bool UareTheMaster = true;
    bool AllInfoSetted = false;
    public bool InMenuGame = false;
    bool inDebugPhoton = true;
    bool inDebugRPCConfirm = false;
    float timmerToShowConnectionError = 0;
    [SerializeField]
    float timmerToShowConnectionErrorValue = 2f;
    [SerializeField]
    TextMeshProUGUI pingKinda;

    //photon
    PhotonView MyPhotonView;

    // Start is called before the first frame update
    void Start()
    {
        timmerToShowConnectionError = timmerToShowConnectionErrorValue;
        //photon 
        MyPhotonView = GetComponent<PhotonView>();

        UareTheMaster = PhotonNetwork.IsMasterClient;

        //if (MyPhotonView.IsMine)
        //{
        //    GetPlayersNames();
        //}

        GetPlayersNames();

        if (inDebugPhoton)
        {
            //Debug.Log("Master PLayer Name -> " + masterName + " Invited Player Name -> " + invited2Name);
        }

        player1Turn = true;
        player2Turn = false;
        playable = true;
        gridManager = GameObject.Find("Grid").GetComponent<GridManager>();

        UpdateUIPLayersInfo();
        ResetUIPlayingTurnInfo();

        UIMenuRestart.SetActive(false);
        UIMenuGame.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (inDebugRPCConfirm) Debug.Log("Conection Situation outside Method -> " + conectionTest);

        ControllTimerError();

        conectionErrorController();

        if (conectionTest)
        {           
            ConfirmRPC();
        }

        

        if (!AllInfoSetted)
        {
            GetPlayersNames();

            SetAllInfoFromPLayers();

            if (inDebugPhoton)
            {
                Debug.Log("Master PLayer Name -> " + masterName + " Invited Player Name -> " + invited2Name);
            }
        }
        else
        {
            //Verify the number of players in the room 
            ConfirmHowManyPlayersInRoom();

            itIsUrTurn = ConfirmIfItIsUrTun();

            if (Input.GetKeyDown(KeyCode.L))
            {
                //gridManager.UpdateGridMatrix();

                gridManager.PrintMatrixGrid();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (gameEnded)
                {
                    RestartGame();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!UIMenuRestart.active)
                {
                    UIGameMenu();
                }
            }
        }


    }

    public void EndTurn()
    {
        if (MyPhotonView.IsMine)
        {
            if (inDebugPhoton) Debug.Log("Ending Turn call");

            MyPhotonView.RPC("RPCEndTurn", RpcTarget.AllBuffered, null);
        }

        //gameEnded = VerifyIfThereIsaWinner();

        //if (gameEnded)
        //{
        //    UIMenuRestart.SetActive(true);

        //    if (player1Turn)
        //    {
        //        UIWinner.text = UINamePlayer1.text;
        //        Debug.Log("/!\\ We have a winner -> Player 1");
        //    }
        //    else if (player2Turn)
        //    {
        //        UIWinner.text = UINamePlayer2.text;
        //        Debug.Log("/!\\ We have a winner -> Player 2");
        //    }
        //}
        //else if (!gameEnded)
        //{
        //    NextUIPlayingTurnInfo();

        //    if (player1Turn)
        //    {
        //        player1Turn = false;
        //        player2Turn = true;
        //        playable = true;

        //        UITurnIndicator.text = UINamePlayer2.text;
        //    }
        //    else if (player2Turn)
        //    {
        //        player1Turn = true;
        //        player2Turn = false;
        //        playable = true;

        //        UITurnIndicator.text = UINamePlayer1.text;
        //    }
        //}

    }

    static bool VerifyIfThereIsaWinner()
    {
        return gridManager.VerifyVictoryCondiction();
    }

    public void RestartGame()
    {
        //if (MyPhotonView.IsMine)
       // {
            if (inDebugPhoton) Debug.Log("Restart Turn call");

            MyPhotonView.RPC("RPCRestartGame", RpcTarget.AllBuffered, null);
        //}

        ////Increment points from the winner
        //IncremetPointsFromWinner();

        ////Restart the Game clearing the variables
        //gridManager.RestartTheGame();

        //player1Turn = true;
        //player2Turn = false;
        //playable = true;

        //UIMenuRestart.SetActive(false);
        //ResetUIPlayingTurnInfo();
    }

    void IncremetPointsFromWinner()
    {
        if (player1Turn)
        {
            masterPoints++;
        }
        else if (player2Turn)
        {
            invitedPoints++;
        }

        UpdateUIPLayersInfo();
    }

    void UpdateUIPLayersInfo()
    {
        if (UareTheMaster)
        {
            UINamePlayer1.text = masterName;
            UIPointsPlayer1.text = masterPoints.ToString();
            UINamePlayer2.text = invited2Name;
            UIPointsPlayer2.text = invitedPoints.ToString();
        }
        else
        {
            UINamePlayer1.text = invited2Name;
            UIPointsPlayer1.text = invitedPoints.ToString();
            UINamePlayer2.text = masterName;
            UIPointsPlayer2.text = masterPoints.ToString();
        }
    }

    void ResetUIPlayingTurnInfo()
    {
        if (UareTheMaster)
        {
            UITurnIndicator.text = masterName;
        }
        else
        {
            UITurnIndicator.text = masterName;
            //UITurnIndicator.text = invited2Name;
        }
    }

    void NextUIPlayingTurnInfo()
    {
        if (UITurnIndicator.text == masterName)
        {
            UITurnIndicator.text = invited2Name;
        } 
        else if (UITurnIndicator.text == invited2Name)
        {
            UITurnIndicator.text = masterName;
        }
    }   
    

    public void GoToStartMenu()
    {
        Debug.Log("Going to menu");

        DisconnectFromGame();
    }

    public void UIGameMenu()
    {
        if (!UIMenuGame.active)
        {
            UIMenuGame.SetActive(true);
            InMenuGame = true;
        }
        else
        {
            UIMenuGame.SetActive(false);
            InMenuGame = false;
        }
        
    }

    void SetAllInfoFromPLayers()
    {
        //if (!AllInfoSetted)
        //{
        //    UpdateUIPLayersInfo();
        //}

        if (ConfirmIfAllSetted())
        {
            ResetUIPlayingTurnInfo();

            AllInfoSetted = true;
        }   
    }

    bool ConfirmIfAllSetted()
    {
        if (UareTheMaster)
        {
            UINamePlayer1.text = masterName;
            UINamePlayer2.text = invited2Name;
            if (UINamePlayer1.text != "LoadingMaster" && UINamePlayer2.text != "LoadingInvited")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            UINamePlayer1.text = invited2Name;
            UINamePlayer2.text = masterName;

            if (UINamePlayer1.text != "LoadingInvited" && UINamePlayer2.text != "LoadingMaster")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    //confirm if it is ur turn
    bool ConfirmIfItIsUrTun()
    {
        if (UareTheMaster)
        {
            return player1Turn;
        }
        else
        {
            return player2Turn;
        }
    }

    void WinByOpponentDisconect()
    {
        MyPhotonView.RPC("RPCWinByOpponentDisconect", RpcTarget.AllBuffered, null);
    }

    //Photon
    [PunRPC]
    void SendMasterName(string masterNameRPC)
    {
        masterName = masterNameRPC; 
    }

    [PunRPC]
    void SendInvitedName(string invitedNameRPC)
    {
        invited2Name = invitedNameRPC;
    }

    void GetPlayersNames()
    {
        if (UareTheMaster)
        {
            MyPhotonView.RPC("SendMasterName", RpcTarget.AllBuffered, PhotonNetwork.NickName);
        }
        else
        {
            MyPhotonView.RPC("SendInvitedName", RpcTarget.AllBuffered, PhotonNetwork.NickName);
            //MyPhotonView.RPC("SendInvitedName", PhotonNetwork.PlayerList[0], PhotonNetwork.NickName);
        }
    }

    [PunRPC]
    void RPCEndTurn()
    {
        if (inDebugPhoton) Debug.Log("Ending Turn by RPC");

        gameEnded = VerifyIfThereIsaWinner();

        if (gameEnded)
        {
            UIMenuRestart.SetActive(true);

            if (player1Turn)
            {
                UIWinner.text = masterName;
                Debug.Log("/!\\ We have a winner -> Player 1");
            }
            else if (player2Turn)
            {
                UIWinner.text = invited2Name;
                Debug.Log("/!\\ We have a winner -> Player 2");
            }
        }
        else if (!gameEnded)
        {
            //NextUIPlayingTurnInfo();

            if (player1Turn)
            {
                player1Turn = false;
                player2Turn = true;
                playable = true;

                UITurnIndicator.text = invited2Name;
            }
            else if (player2Turn)
            {
                player1Turn = true;
                player2Turn = false;
                playable = true;

                UITurnIndicator.text = masterName;
            }
        }
    }

    [PunRPC]
    void RPCRestartGame()
    {
        //Increment points from the winner
        IncremetPointsFromWinner();

        //Restart the Game clearing the variables
        gridManager.RestartTheGame();

        player1Turn = true;
        player2Turn = false;
        playable = true;

        UIMenuRestart.SetActive(false);
        ResetUIPlayingTurnInfo();
    }

    void ConfirmHowManyPlayersInRoom()
    {
        int tempCountPLayers = 0;

        //foreach (Player player in PhotonNetwork.PlayerList)
        //{
        //    tempText.text = player.NickName;
        //
        //}

        tempCountPLayers = PhotonNetwork.PlayerList.Length;

        //if (inDebugPhoton)
        //{
        //    Debug.Log("players count in room -> " + tempCountPLayers);
        //}

        if (tempCountPLayers < 2)
        {
            WinByOpponentDisconect();
        }

    }


    [PunRPC]
    void RPCWinByOpponentDisconect()
    {
        //se eu que estou a ver isto for o master é o master name que ganha e vice versa 
        //Can´t play more 
        playable = false;

        string tempMessage = "";

        //Enable to restart Game
        UIRestartButton.SetActive(false);

        UIWinByOpponentDisconectedMessage.SetActive(true);

        UIMenuRestart.SetActive(true);

        if (PhotonNetwork.NickName == masterName)
        {
            UIWinner.text = masterName;
            Debug.Log("/!\\ We have a winner -> Master / player 1");

            //set message fmor winner
            tempMessage = "Player : " + invited2Name + " Has Disconected ";


        }
        else 
        {
            UIWinner.text = invited2Name;
            Debug.Log("/!\\ We have a winner -> Invited /Player 2");

            //set message fmor winner
            tempMessage = "Player : " + invited2Name + " Has Disconected ";
        }

        UITextWinByOpponentDisconectedMessage.text = tempMessage;
    }

    void DisconnectFromGame() 
    {
        PhotonNetwork.LeaveRoom();
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveLobby();
        }

        //chat disconect ?

        SceneManager.LoadScene(1);
    }

    [PunRPC]
    void RPCConfirmRPC(int indicePlayerList)
    {
        if (inDebugRPCConfirm) Debug.Log("Stage 2 i-> " + indicePlayerList);
        MyPhotonView.RPC("RPCConfirmRPCRecall", PhotonNetwork.PlayerList[indicePlayerList], null);
    }

    [PunRPC]
    void RPCConfirmRPCRecall()
    {
        conectionTest = true;
        if (inDebugRPCConfirm) Debug.Log("Stage 3 Connection -> " + conectionTest);

    }

    void ConfirmRPC()
    {
        conectionTest = false;

        if (inDebugRPCConfirm) Debug.Log("Conection Situation in Method -> " + conectionTest);

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.NickName == PhotonNetwork.PlayerList[i].NickName)
            {
                if (inDebugRPCConfirm) Debug.Log("Stage One i-> " + i);
                MyPhotonView.RPC("RPCConfirmRPC", RpcTarget.Others, i);


                break;
            }
        }

    }

    void conectionErrorController()
    {
        //if (inDebugPhoton) Debug.Log("Connection state -> " + conectionTest);

        if (conectionTest)
        {
            //if (inDebugPhoton) Debug.Log("Connection state -> " + conectionTest);
            connectionErrorCtrl.HideMessage();
        }
        else if(!conectionTest && timmerToShowConnectionError <= 0)
        {
            pingKinda.text = " + " + timmerToShowConnectionErrorValue + " Sec ";
            connectionErrorCtrl.ShowMessage();
        }
    }

    void ControllTimerError()
    {
        float temp = 0;

        if (conectionTest)
        {
            temp = timmerToShowConnectionErrorValue - timmerToShowConnectionError;

            pingKinda.text = temp.ToString();

            timmerToShowConnectionError = timmerToShowConnectionErrorValue;
        }
        else
        {
            timmerToShowConnectionError -= Time.deltaTime;
        }
    }
}
