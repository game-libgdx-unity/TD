using System;
using System.Collections;
using UnitedSolution;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Game manager.
/// Connects and watch Photon Status, Instantiate Player
/// Deals with quiting the room and the game
/// Deals with level loading (outside the in room synchronization)
/// </summary>
public class NetworkService : PhotonSingleton<NetworkService>
{
    #region Fields
    public NetworkObservable Player { get; set; }
    public GameObject playerPrefab;
    public ChatGui chatGUI;
    #endregion

    #region Initialization
    void Start()
    {
        Application.runInBackground = true;
        if (!PhotonNetwork.connected)
        {
            SceneManager.LoadScene("launcher");
            return;
        }

        if (playerPrefab == null)
        {
            Debug.LogError("Missing player prefap");
        }
        else
        {
            if (Player == null)
            {
                Debug.Log("Create player");
#if LOGGING
                    Debug.Log("We are Instantiating LocalPlayer from " + SceneManagerHelper.ActiveSceneName);
#endif
                GameObject obj = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                Player = obj.GetComponent<NetworkObservable>();
                chatGUI.UserName = PhotonNetwork.playerName;
                chatGUI.Connect();
                PlayerPrefs.SetString(UserNamePlayerPref, chatGUI.UserName);
            }
        }
    }
    #endregion
    private const string UserNamePlayerPref = "NamePickUserName";

    #region Public Methods 

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region Overriden Members

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("launcher");
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        Debug.Log("OnPhotonPlayerConnected() " + other.NickName); // not seen if you're the player connecting
        Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
        Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName); // seen when other disconnects
        Debug.Log("OnPhotonPlayerDisconnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected
        PhotonNetwork.LeaveRoom(); //leave room After the opponent left
    }

    #endregion

}
