using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using VRStandardAssets.Utils;

public class GameManager : Photon.PunBehaviour {

	public float timeLeft = 0f;
	public float totalTime = 180f;
	private bool timerOn = true;
	public bool gameOver = false;
	private GameObject cam;
	private SoundManager soundManager;

	public GameObject gameOverText;
	public GameObject whoWonText;
	public GameObject HUD;

//	private bool audioLock = true; // no audio collisions, do later
	private bool welcome = true;
	public bool tutorial1 = true;
	public bool tutorial2 = true;
	public bool tutorial3 = true;
	public bool insult = true;
	public bool remember = true;
	public bool hate = true;
	public bool cleanUp = true;
	public bool entertaining = true;
	public bool closing = true;

	void Awake() {
		gameOverText = GameObject.Find ("GameOverText");
		whoWonText = GameObject.Find ("WhoWonText");
		HUD = GameObject.Find ("HUD");
		cam = GameObject.Find ("PlayerCamera");
//		cam.GetComponent<VREyeRaycaster> ().enabled = false;
		soundManager = GameObject.Find ("SoundManager").GetComponent <SoundManager> ();

		if (!PhotonNetwork.isMasterClient) {
			cam.tag = "P2";

			GameObject camObject = GameObject.Find ("CameraObject");
			camObject.transform.position = new Vector3 (0f, 0f, 1000f);
			camObject.transform.rotation *= Quaternion.Euler (0, 180f, 0);

			GameObject.Find ("P2Char").GetComponentInChildren<SkinnedMeshRenderer> ().enabled = false;
		} else {
			GameObject.Find ("P1Char").GetComponentInChildren<SkinnedMeshRenderer> ().enabled = false;
		}
	}

	void Update() {
		if (timerOn && !gameOver) {
			timeLeft += Time.deltaTime;
		}

		if (timeLeft > 5.0f && welcome) {
			soundManager.playWelcome (); 
			welcome = false;
		}

		if (timeLeft > 15.0f && tutorial1) {
			soundManager.playSelect (); 
			tutorial1 = false;
			cam.GetComponent<VREyeRaycaster> ().enabled = true;
		}

		if (timeLeft > 15.0f + soundManager.tutorial1.length && tutorial2) {
			soundManager.playDraw ();
			tutorial2 = false;
		}

		if (timeLeft > 15.0f + soundManager.tutorial1.length
			+ soundManager.tutorial2.length && tutorial3) {
			soundManager.playDontActually ();
			tutorial3 = false;
		}

		if (timeLeft > 15.0f + soundManager.tutorial1.length
			+ soundManager.tutorial2.length + soundManager.tutorial3.length) {
		}

		if (timeLeft > 60.0f && insult) {
			soundManager.playInsult ();
			insult = false;
		}

		if (timeLeft > 70.0f && remember) {
			soundManager.playRemember ();
			remember = false;
		}

		if (timeLeft > 85.0f && hate) {
			soundManager.playHate ();
			hate = false;
		}

		if (timeLeft > 100.0f && cleanUp) {
			soundManager.playCleanUp ();
			cleanUp = false;
		}

		if (timeLeft > 120.0f && entertaining) {
			soundManager.playEntertaining ();
			entertaining = false;
		}

		if (timeLeft > 150.0f && closing) {
			soundManager.playClosing ();
			closing = false;
		}

		if (timeLeft > totalTime) {
			gameOver = true;
			handleGameOver ();
		}
	}


	public override void OnPhotonPlayerConnected( PhotonPlayer other  )
	{
		Debug.Log( "OnPhotonPlayerConnected() " + other.name ); // not seen if you're the player connecting

		if ( PhotonNetwork.isMasterClient ) 
		{
			Debug.Log ("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected
		}

//		PhotonView pv = PhotonView.Get (this);
//		pv.RPC ("startTimer", PhotonTargets.All, 60);
	}


	public override void OnPhotonPlayerDisconnected( PhotonPlayer other  )
	{
		Debug.Log( "OnPhotonPlayerDisconnected() " + other.name ); // seen when other disconnects

		if ( PhotonNetwork.isMasterClient ) 
		{
			Debug.Log( "OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient ); // called before OnPhotonPlayerDisconnected

		}
	}

	/// <summary>
	/// Called when the local player left the room. We need to load the launcher scene.
	/// </summary>
	public void OnLeftRoom()
	{
		SceneManager.LoadScene(0);
	}

	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
	}

//	[PunRPC]
//	private void startTimer(int totalTime) {
//		print ("start " + totalTime + " second timer!");
//		timerOn = true;
//	}

	private void handleGameOver() {
		float health = HUD.GetComponent<HealthBar> ().rectWidth;

		if (health > 250f && cam.tag == "P1" || health <= 250f && cam.tag == "P2") {
			whoWonText.GetComponent<Text>().text = "YOU WIN";
		} else {
			whoWonText.GetComponent<Text>().text = "YOU LOSE";
		}

		gameOverText.GetComponent<Text> ().enabled = true;
		whoWonText.GetComponent<Text> ().enabled = true;
	}
}
