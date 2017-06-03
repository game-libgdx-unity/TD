using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/// <summary>
/// Launch manager. Connect, join a random room or create one if none or all full.
/// </summary>
public class Launcher : Photon.PunBehaviour
{

    #region Fields

    private const float ORIGIN_X = 670f;
    private const float MOVED_X = 550f;
    public string SceneToPlayOnline = "PlayOnline";
    public string SceneToPlayOffline = "PlayWithAI";
    [SerializeField]
    private GameObject title;
    [SerializeField]
    private GameObject desc;
    [SerializeField]
    private GameObject controlPanel;
    [SerializeField]
    private Text feedbackText;
    [SerializeField]
    private byte maxPlayersPerRoom = 4;
    [SerializeField]
    private LoaderAnime loaderAnime;
    private Vector3 controlPanelPosition;
    [SerializeField]
    private GameObject btnLeaveRoom;
    #endregion

    #region Initialization

    void Awake()
    {
        if (loaderAnime == null)
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> loaderAnime Reference.", this);
        }
        PhotonNetwork.logLevel = PhotonLogLevel.ErrorsOnly;
        PhotonNetwork.autoJoinLobby = false; //ignore the lobby, join a room directly
        PhotonNetwork.automaticallySyncScene = true;
        controlPanelPosition = controlPanel.transform.localPosition;
        btnLeaveRoom.SetActive(false);
        btnLeaveRoom.GetComponent<Button>().onClick.AddListener(()=> { PhotonNetwork.LeaveRoom();  });
    }
    #endregion

    #region Public Members
    public void SetupFacebookLogin(string name)
    {
        controlPanelPosition.x = MOVED_X;
        controlPanel.transform.position = controlPanelPosition;
        foreach (Transform child in controlPanel.transform)
        {
            if (child.name.Equals("Label"))
            {
                child.gameObject.SetActive(false);
            }
            else if (child.name.Equals("InputField"))
            {
                InputField input = child.GetComponent<InputField>();
                input.text = name;
            }
        }
    }
    #endregion

    #region Overriden Members
    public override void OnConnectedToMaster()
    {
#if LOGGING
            Debug.Log("OnConnectedToMaster() >>> need to connect to lobby or room");
            Debug.Log("Region:" + PhotonNetwork.networkingPeer.CloudRegion);
#endif

        if (isConnecting)
        {
            LogFeedback("Connected to servers");
            PhotonNetwork.JoinRandomRoom(); //join a room
        }
    }

    public override void OnJoinedRoom()
    {
#if LOGGING
            LogFeedback("OnJoinedRoom() >>> " + PhotonNetwork.room.PlayerCount + " Player(s)");
#endif

        LogFeedback("Joining a room");
        if (PhotonNetwork.room.PlayerCount < 2)
        {
            StartCoroutine(WaitForTwoPlayer());
            LogFeedback("Waiting For the Second Player");
            btnLeaveRoom.SetActive(true);
        }
        else
        {
            PhotonNetwork.LoadLevel(SceneToPlayOnline);
        }
    }
    public override void OnLeftRoom()
    {
        StopCoroutine(WaitForTwoPlayer());
        PhotonNetwork.LoadLevel(0);
    }
    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
#if LOGGING
            LogFeedback("OnPhotonRandomJoinFailed() >>> Create new room");
#endif

        LogFeedback("Create a new Room");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = this.maxPlayersPerRoom }, null);
    }

    public override void OnDisconnectedFromPhoton()
    {
#if LOGGING
            Debug.LogError("OnDisconnectedFromPhoton() >>> Need to reconnect to servers");
#endif

        LogFeedback("<Color=Red>OnDisconnectedFromPhoton</Color>");
        loaderAnime.StopLoaderAnimation();
        isConnecting = false;
        controlPanel.SetActive(true);
    }

    #endregion

    #region Private Members

    bool isConnecting;
    string gameVersion = "1";

    IEnumerator WaitForTwoPlayer()
    {
        yield return new WaitUntil(()=>PhotonNetwork.room.PlayerCount == 2);
        PhotonNetwork.LoadLevel(SceneToPlayOnline);
    }

    void LogFeedback(string message)
    {
        if (feedbackText == null)
        {
            return;
        }
        feedbackText.text += System.Environment.NewLine + message;
    }

    private void Connect()
    { 
        feedbackText.text = "";
        isConnecting = true;
        controlPanel.SetActive(false);
        title.SetActive(false);
        desc.SetActive(false);
        loaderAnime.StartLoaderAnimation();
        if (PhotonNetwork.connected)
        {
            LogFeedback("Joining Room...");
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            LogFeedback("Connecting to servers...");
            PhotonNetwork.ConnectUsingSettings(gameVersion);
        }
    }

    public void PlayAI()
    { 
        feedbackText.text = "";
        isConnecting = true;
        controlPanel.SetActive(false);
        title.SetActive(false);
        desc.SetActive(false);
        SceneManager.LoadScene(SceneToPlayOffline);
    }
    #endregion

}
