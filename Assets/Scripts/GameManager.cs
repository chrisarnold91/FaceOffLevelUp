﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using VRStandardAssets.Utils;

public class GameManager : Photon.PunBehaviour {

	public float timeLeft = 0f;
	public float totalTime = 20f;
	private bool timerOn = true;
	public bool gameOver = false;
	private GameObject cam;
	private SoundManager soundManager;
	private SoundFXManager soundFXManager;
	private Text timer;

//	public GameObject gameOverText;
//	public GameObject whoWonText;
	public GameObject HUD;

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
	public bool newWelcome = true;
	public bool newTutorial = true;

	public int countP1 = 0;
	public int countP2 = 0;
	public int blocks1 = 0;
	public int blocks2 = 0;
	public List<float> accuracies1;
	public List<float> accuracies2;
	public int streakP1 = 1;
	public int streakP2 = 1;
	private int highestStreakP1 = 0;
	private int highestStreakP2 = 0;

	void Awake() {
//		gameOverText = GameObject.Find ("GameOverText");
//		whoWonText = GameObject.Find ("WhoWonText");
		HUD = GameObject.Find ("HUD");
		cam = GameObject.Find ("PlayerCamera");
		soundManager = GameObject.Find ("SoundManager").GetComponent <SoundManager> ();
		soundFXManager = GameObject.Find ("SoundFXManager").GetComponent <SoundFXManager> ();
		timer = GameObject.Find ("Timer").GetComponent<Text> ();

		if (!PhotonNetwork.isMasterClient) {
			cam.tag = "P2";

			GameObject camObject = GameObject.Find ("CameraObject");
			camObject.transform.position = new Vector3 (0f, 0f, 1000f);
			camObject.transform.rotation *= Quaternion.Euler (0, 180f, 0);

			GameObject.Find ("P2Char").GetComponentInChildren<SkinnedMeshRenderer> ().enabled = false;
		} else {
			GameObject.Find ("P1Char").GetComponentInChildren<SkinnedMeshRenderer> ().enabled = false;
		}

		accuracies1 = new List<float> ();
		accuracies2 = new List<float> ();
	}

	void Update() {
		if (timerOn && !gameOver) {
			timeLeft += Time.deltaTime;
			timer.text = displayTime (timeLeft);
		}

		if (timeLeft > 1.0f && welcome) {
			soundManager.playNewTutorial (); 
			welcome = false;
		}

//		if (timeLeft > 15.0f && tutorial1) {
//			soundManager.playSelect (); 
//			tutorial1 = false;
//			cam.GetComponent<VREyeRaycaster> ().enabled = true;
//		}

//		if (timeLeft > 15.0f + soundManager.tutorial1.length && tutorial2) {
//			soundManager.playDraw ();
//			tutorial2 = false;
//		}

//		if (timeLeft > 15.0f + soundManager.tutorial1.length
//			+ soundManager.tutorial2.length && tutorial3) {
//			soundManager.playDontActually ();
//			tutorial3 = false;
//		}

		if (timeLeft > 45.0f && insult) {
			soundManager.playInsult ();
			insult = false;
		}

//		if (timeLeft > 70.0f && remember) {
//			soundManager.playRemember ();
//			remember = false;
//		}

		if (timeLeft > 55.0f && cleanUp) {
			soundManager.playCleanUp ();
			cleanUp = false;
		}

		if (timeLeft > 67.0f && entertaining) {
			soundManager.playEntertaining ();
			entertaining = false;
		}

		if (timeLeft > 88.0f && closing) {
			soundManager.playClosing ();
			closing = false;
		}

		if (timeLeft > 110.0f && hate) {
			soundManager.playHate ();
			hate = false;
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

	public void updateAccuracy(string who, float accuracy) {
		PhotonView pv = PhotonView.Get (this);
		pv.RPC ("accuracyRPC", PhotonTargets.All, who, accuracy);
	}

	public void updateBlocks(string whichShield) {
		PhotonView pv = PhotonView.Get (this);
		pv.RPC ("blocksRPC", PhotonTargets.All, whichShield);
	}

	public void updateStreaks(string who, int streak) {
		PhotonView pv = PhotonView.Get (this);
		pv.RPC ("streakRPC", PhotonTargets.All, who, streak);
	}

	[PunRPC]
	private void accuracyRPC(string who, float accuracy) {
		if (who == "P1") {
			accuracies1.Add (accuracy);
		} else {
			accuracies2.Add (accuracy);
		}
	}

	[PunRPC]
	private void blocksRPC(string whichShield) {
		soundFXManager.playHitShield ();
		if (whichShield == "shield1") {
			blocks1++;
		} else {
			blocks2++;
		}
	}

	[PunRPC]
	private void streakRPC(string who, int streak) {
		if (who == "P1") {
			streakP1 = streak;
			if (streak > highestStreakP1) {
				highestStreakP1 = streak;
			}
		} else {
			streakP2 = streak;
			if (streak > highestStreakP2) {
				highestStreakP2 = streak;
			}
		}
	}

	private void handleGameOver() {
//		float health = HUD.GetComponent<HealthBar> ().rectWidth;
//
//		if (health > 250f && cam.tag == "P1" || health <= 250f && cam.tag == "P2") {
//			whoWonText.GetComponent<Text>().text = "YOU WIN";
//		} else {
//			whoWonText.GetComponent<Text>().text = "YOU LOSE";
//		}

//		gameOverText.GetComponent<Text> ().enabled = true;
//		whoWonText.GetComponent<Text> ().enabled = true;

		TextMesh YouScore1 = GameObject.Find("YouScore1").GetComponent<TextMesh>();
		TextMesh ThemScore1 = GameObject.Find ("ThemScore1").GetComponent<TextMesh>();

		string sep = "\r\n\r\n";

		string p1numbers = YouScore1.text + sep + countP1 + sep + (highestStreakP1).ToString() + sep
			+ getAvgAccuracy (accuracies1) + "%" + sep + blocks1;

		string p2numbers = ThemScore1.text + sep + countP2 + sep + (highestStreakP2).ToString() + sep
			+ getAvgAccuracy (accuracies2) + "%" + sep + blocks2;

		GameObject numbersTextYou1 = GameObject.Find ("NumbersTextYou" + cam.tag);
		GameObject numbersTextThem1 = GameObject.Find ("NumbersTextThem" + cam.tag);
		GameObject result1 = GameObject.Find ("ResultP1");
		GameObject result2 = GameObject.Find ("ResultP2");

		if (cam.tag == "P1") {
			numbersTextYou1.GetComponent<TextMesh> ().text = p1numbers;
			numbersTextThem1.GetComponent<TextMesh> ().text = p2numbers;
		} else {
			numbersTextYou1.GetComponent<TextMesh> ().text = p2numbers;
			numbersTextThem1.GetComponent<TextMesh> ().text = p1numbers;
		}

		if (int.Parse (YouScore1.text) > int.Parse (ThemScore1.text)) {
			result1.GetComponent<TextMesh> ().text = "YOU WIN";
			result2.GetComponent<TextMesh> ().text = "YOU LOSE";
		} else if (int.Parse (YouScore1.text) < int.Parse (ThemScore1.text)) {
			result1.GetComponent<TextMesh> ().text = "YOU LOSE";
			result2.GetComponent<TextMesh> ().text = "YOU WIN";
		} else {
			result1.GetComponent<TextMesh> ().text = "TIE (which is basically losing...)";
			result1.GetComponent<TextMesh> ().text = "TIE (which is basically losing...)";
		}

		numbersTextYou1.GetComponent<MeshRenderer>().enabled = true;
		numbersTextThem1.GetComponent<MeshRenderer>().enabled = true;
		result1.GetComponent<MeshRenderer> ().enabled = true;
		result2.GetComponent<MeshRenderer> ().enabled = true;

		GameObject.Find ("StatsText" + cam.tag).GetComponent<MeshRenderer> ().enabled = true;
		GameObject.Find ("PlayAgainText" + cam.tag).GetComponent<MeshRenderer> ().enabled = true;
		GameObject.Find ("PlayAgainButton" + cam.tag).GetComponent<MeshRenderer> ().enabled = true;
		GameObject.Find ("GameOverScreen" + cam.tag).GetComponent<MeshRenderer> ().enabled = true;
	}

	private string displayTime(float time) {
		time = totalTime - time;
		int minutes = (int) time / 60;
		int seconds = (int) time - (minutes * 60);
		string secondsString = "";
		secondsString = seconds.ToString();
		if (seconds < 10) {
			secondsString = "0" + seconds;
		}
		return minutes + " : " + secondsString;
	}

	private float getAvgAccuracy(List<float> accuracies) {
		if (accuracies.Count == 0) {
			return 0;
		}

		float tally = 0f;
		foreach (float acc in accuracies) {
			tally += acc;
		}
		return Mathf.Round ((tally * 100 / accuracies.Count) * 10) / 10;
//		return System.Math.Round(((double)tally / (double)accuracies.Count), 1);

	}

	public void addBlock1() {
		blocks1 = blocks1 + 1;
	}

	public void addBlock2() {
		blocks2 = blocks2 + 1;
	}
}
