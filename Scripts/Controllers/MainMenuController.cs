using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    bool inDebug = true;
    [SerializeField]
    GameObject MenuPainel;
    [SerializeField]
    GameObject LoobbyPainel;
    [SerializeField]
    GameObject RoomPainel;

    private void Start()
    {
        MenuPainel.SetActive(true);
        LoobbyPainel.SetActive(false);
        RoomPainel.SetActive(false);
    }
    public void Play()
    {
        SceneManager.LoadScene(3);

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void UsernameInput(TextMeshProUGUI username){

        if (inDebug)
        {
            Debug.Log("Player Name entered in input text -> " + username.text);
        }
        PhotonNetwork.NickName = username.text;
    }
}