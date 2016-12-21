using UnityEngine;
using System.Collections;

public class PlayerManager : Photon.PunBehaviour {

	[Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
	public static GameObject LocalPlayerInstance;
	private string whichCamera = "P1";

	void Awake ()
	{
		// #Important
		// used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
		if (photonView.isMine) {
			PlayerManager.LocalPlayerInstance = this.gameObject;
		}
		// #Critical
		// we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
		DontDestroyOnLoad (this.gameObject);

		if (!PhotonNetwork.isMasterClient)
			whichCamera = "P2";

		this.gameObject.tag = whichCamera;
	}
}
