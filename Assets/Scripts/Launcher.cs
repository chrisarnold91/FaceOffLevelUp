using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class Launcher : Photon.PunBehaviour {
	/// <summary>
	/// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
	/// </summary>   
	[Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
	public byte MaxPlayersPerRoom = 2;
	string _gameVersion = "1";
	//  [Tooltip("The Ui Panel to let the user enter name, connect and play")]
	//  public GameObject controlPanel;
	[Tooltip("The UI Label to inform the user that the connection is in progress")]
	public GameObject progressLabel;
	/// <summary>
	/// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
	/// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
	/// Typically this is used for the OnConnectedToMaster() callback.
	/// </summary>
	bool isConnecting;
	/// <summary>
	/// MonoBehaviour method called on GameObject by Unity during early initialization phase.
	/// </summary>

	public AudioSource efxSource;
	public AudioClip newWelcome;

	void Awake()
	{
		// #NotImportant
		// Force Full LogLevel
		PhotonNetwork.logLevel = PhotonLogLevel.Full;
		// #Critical
		// we don't join the lobby. There is no need to join a lobby to get the list of rooms.
		PhotonNetwork.autoJoinLobby = false;
		// #Critical
		// this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
		PhotonNetwork.automaticallySyncScene = true;

//		efxSource.clip = newWelcome;
		efxSource.PlayOneShot (newWelcome);
	}
	/// <summary>
	/// MonoBehaviour method called on GameObject by Unity during initialization phase.
	/// </summary>
	void Start()
	{
		progressLabel.SetActive(false);
		//      controlPanel.SetActive(true);
	}
	void Update()
	{
		if (PhotonNetwork.playerList.Length == 2)
		{
			PhotonNetwork.LoadLevel("Supermarket");
			Text label = progressLabel.GetComponent<Text>();
			label.text = "Connecting...";
		}
		else if (PhotonNetwork.playerList.Length == 1)
		{
			Text label = progressLabel.GetComponent<Text>();
			label.text = "Waiting for other player...";
		}
	}
	/// <summary>
	/// Start the connection process. 
	/// - If already connected, we attempt joining a random room
	/// - if not yet connected, Connect this application instance to Photon Cloud Network
	/// </summary>
	public void Connect()
	{
		// keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
		isConnecting = true;
		progressLabel.SetActive(true);
		//      controlPanel.SetActive(false);
		// we check if we are connected or not, we join if we are , else we initiate the connection to the server.
		if (PhotonNetwork.connected)
		{
			// #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
			PhotonNetwork.JoinRandomRoom ();

		}else{
			// #Critical, we must first and foremost connect to Photon Online Server.
			PhotonNetwork.ConnectUsingSettings(_gameVersion);
		}
	}
	public override void OnConnectedToMaster()
	{
		Debug.Log("DemoAnimator/Launcher: OnConnectedToMaster() was called by PUN");
		// we don't want to do anything if we are not attempting to join a room. 
		// this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
		// we don't want to do anything.
		if (isConnecting)
		{
			// #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed()
			PhotonNetwork.JoinRandomRoom();
		}
	}
	public override void OnDisconnectedFromPhoton()
	{
		progressLabel.SetActive(false);
		//      controlPanel.SetActive(true);
		Debug.LogWarning("DemoAnimator/Launcher: OnDisconnectedFromPhoton() was called by PUN");        
	}
	public override void OnPhotonRandomJoinFailed (object[] codeAndMsg)
	{
		Debug.Log("DemoAnimator/Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
		// #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
		PhotonNetwork.CreateRoom(null, new RoomOptions() { maxPlayers = MaxPlayersPerRoom }, null);
	}
	public override void OnJoinedRoom()
	{
		Debug.Log("DemoAnimator/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
		// #Critical
		// Load the Room Level. 
		// Only load level until both players have joined
		if (PhotonNetwork.playerList.Length == 2)
		{
//			StartCoroutine (playWelcome ());
			//Tell other player they are connected
			PhotonNetwork.LoadLevel("Supermarket");
		}
		else if(PhotonNetwork.playerList.Length == 1)
		{
			Text label = progressLabel.GetComponent<Text>();
			label.text = "Waiting for Player 2....";
		}
	}

//	IEnumerator playWelcome() {
//		efxSource.clip = newWelcome;
//		efxSource.Play ();
////		yield return new WaitWhile (() => efxSource.isPlaying);
//		yield return new WaitForSeconds(15.0f);
//		PhotonNetwork.LoadLevel("Supermarket");
//	}
}
