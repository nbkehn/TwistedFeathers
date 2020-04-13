using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class Launcher : MonoBehaviourPunCallbacks
{
    #region Private Serializable Fields

    /// <summary>
    /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so a new room will be created.
    /// </summary>
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so a new room will be created.")]
    [SerializeField]
    private byte maxPlayersPerRoom = 2;

    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    [SerializeField]
    private GameObject controlPanel;

    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField]
    private GameObject progressLabel;

    #endregion

    #region Private Fields

    /// <summary>
    /// This client's version number. Users are seperated from each other by gameVersion (which allows you to make breaking changes).
    /// </summary>
    string gameVersion = "1";

    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
    /// We need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// </summary>
    bool isConnecting;

    #endregion

    #region MonoBehaviour CallBacks

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>
    void Awake()
    {
        // #critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    // Start is called before the first frame update
    void Start()
    {
        //Connect();
        //function has been moved to the play button in the scene under onClick()
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Start the connection process.
    /// - If already connnected, we attempt joining a random room
    /// - If not yet connnected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void Connect()
    {
        // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
        isConnecting = PhotonNetwork.ConnectUsingSettings();
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);

        // we check if we are connected or not, we join if we are, else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public void Create()
    {
        // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
        isConnecting = PhotonNetwork.ConnectUsingSettings();
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    IEnumerator changeText()
    {
        yield return new WaitForSeconds(1f);
        progressLabel.GetComponent<Text>().text = "Connecting...";
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    #endregion

    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("PUN Basics Launcher: OnConnectedToMaster() was called by PUN");
        // #Critica: The first thing we try to do is to join a potnetial existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
        // we don't want to do anything if we are not attempting to join a room.
        // this case where isConnecting is false is typically when you lost or quit the game,when this level is loaded, OnConnectedToMaster will be called, in that case 
        // we don't want to do anything.
        if (isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
        }

    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        Debug.LogWarningFormat("PUN Basics Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Launcher: OnJoinRandomFailed() was called by PUN. No random room available");
        progressLabel.GetComponent<Text>().text = "No rooms found";
        StartCoroutine(changeText());
        
        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        //PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Launcher: OnJoinedRoom() was called by PUN. Now this client is in a room.");
        progressLabel.SetActive(false);
        // #Critical: We only Load if we are the first player, else we rely on 'PhotonNetwork.AutomaticallySyncScene' to sync our instance scene.
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("We load the next scene.");

            // #Critical
            // Load the Room Level.
            PhotonNetwork.LoadLevel("EnvironmentSwitching");
        }
    }

    #endregion
}