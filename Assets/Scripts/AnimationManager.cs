using UnityEngine;
using System.Collections;

public class AnimationManager : MonoBehaviour {

	Animator anim;
	private GameObject cam;

	private int ShieldingHash = Animator.StringToHash("shielding");
	private int GettingHitHash = Animator.StringToHash("gettingHit");

	void Start () {
		anim = GetComponent<Animator> ();
		cam = GameObject.Find ("PlayerCamera");
	}

	void Update() {
		if (cam.tag == anim.tag) {
			transform.eulerAngles = new Vector3 (0, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z);
		}
	}

	public void transitionToItemSelected() {
		PhotonView pv = PhotonView.Get (this);
		pv.RPC ("itemSelectedRPC", PhotonTargets.All);
	}

	public void transitionToFinishedDrawing() {
		PhotonView pv = PhotonView.Get (this);
		pv.RPC ("finishedDrawingRPC", PhotonTargets.All);
	}

	public void triggerShielding(string activeShield) {
		if (activeShield == "shield1" && cam.tag == "P1"
			|| activeShield == "shield2" && cam.tag == "P2") {
//		if (activeShield == cam.tag) {
			PhotonView pv = PhotonView.Get (this);
			pv.RPC ("triggerShieldingRPC", PhotonTargets.All);
		}
	}

	public void triggerGettingHit(string victim) {
		if (victim != cam.tag) {
			PhotonView pv = PhotonView.Get (this);
			pv.RPC ("triggerGettingHitRPC", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void itemSelectedRPC() {
		anim.SetBool ("itemSelected", true);
		anim.SetBool ("finishedDrawing", false);
	}

	[PunRPC]
	private void finishedDrawingRPC() {
		anim.SetBool ("finishedDrawing", true);
		anim.SetBool ("itemSelected", false);
	}

	[PunRPC]
	private void triggerShieldingRPC() {
		anim.SetTrigger (ShieldingHash);
	}

	[PunRPC]
	private void triggerGettingHitRPC() {
		anim.SetTrigger (GettingHitHash);
	}
}
