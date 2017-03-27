using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShieldManager : MonoBehaviour {

	private SoundFXManager soundFXManager;

	void Awake() {
		soundFXManager = GameObject.Find ("SoundFXManager").GetComponent <SoundFXManager> ();
	}

	public void activateShield(GameObject shield) {
		PhotonView pv = PhotonView.Get (this);
		pv.RPC ("activateShieldRPC", PhotonTargets.All, shield.tag);
	}

	[PunRPC]
	private void activateShieldRPC(string shieldTag) {
		soundFXManager.playDefend ();
		GameObject shield = GameObject.FindGameObjectWithTag (shieldTag);
		shield.GetComponent<Shield>().putUpShield (shieldTag);
	}
}
