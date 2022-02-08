using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;
using TMPro;

public class ChatMenager : MonoBehaviour, IChatClientListener
{
    //variables 
    bool inDebug = true;
    private ChatClient chatClient;

    [SerializeField]
    private TextMeshProUGUI chatText;

    public void DebugReturn(DebugLevel level, string message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnChatStateChange(ChatState state)
    {
       // throw new System.NotImplementedException();
    }

    //Important
    public void OnConnected()
    {
        chatClient.Subscribe(new string[] { PhotonNetwork.CurrentRoom.Name, "global" });
        Debug.Log("Connected to Chat");
    }
    
    public void OnDisconnected()
    {
        //TMP_InputField tempTExt = new TMP_InputField();
        //
        //tempTExt.text = "Left The Game";
        //
        //SendMessage(tempTExt);
    }

    //Important
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        string msgs = "";
        for (int i = 0; i < senders.Length; i++)
        {
            msgs = string.Format("{0}<b>{1}:</b> {2} \n", msgs, senders[i], messages[i]);
        }
        chatText.text += msgs;
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        //throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
       // throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUnsubscribed(string[] channels)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
       // throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
            PhotonNetwork.AppVersion, new AuthenticationValues(PhotonNetwork.NickName));
    }

    // Update is called once per frame
    void Update()
    {
        chatClient.Service();
    }

    public void SendMessageInput(TMP_InputField message)
    {
        chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, message.text);
        
        message.text = "";
    }
}
