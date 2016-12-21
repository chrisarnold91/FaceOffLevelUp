using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShieldManager : MonoBehaviour {

	public void activateShield(GameObject shield) {
		PhotonView pv = PhotonView.Get (this);
		pv.RPC ("activateShieldRPC", PhotonTargets.All, shield.tag);
	}

	[PunRPC]
	private void activateShieldRPC(string shieldTag) {
		GameObject shield = GameObject.FindGameObjectWithTag (shieldTag);
		shield.GetComponent<Shield>().putUpShield (shieldTag);
	}

}