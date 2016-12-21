using UnityEngine;
//using UnityEditor;
using System.Collections;

public class ObjectManager : Photon.PunBehaviour {

	private GameObject cam;
	private bool holdingSomething = false;

	// Use this for initialization
	void Start () {
		cam = GameObject.Find ("PlayerCamera");
	}

	public void moveObject(GameObject obj) {
		if (!holdingSomething) {
			PhotonView pv = PhotonView.Get (this);
			pv.RPC ("moveObjectRPC", PhotonTargets.All, cam.tag, obj.tag);
		}
		holdingSomething = true;
	}

	public void throwObject(GameObject obj) {
		PhotonView pv = PhotonView.Get (this);
		pv.RPC ("throwObjectRPC", PhotonTargets.All, cam.tag, obj.tag);
		holdingSomething = false;
	}

	public void resetObject(GameObject obj) {
		PhotonView pv = PhotonView.Get (this);
		pv.RPC ("resetObjectRPC", PhotonTargets.All, obj.tag);
	}

	[PunRPC]
	private void moveObjectRPC(string cameraTag, string itemTag) {
		GameObject Item3D = GameObject.FindGameObjectWithTag (itemTag);
		Item3D.GetComponentInChildren<CubeButton> ().readyToMove (cameraTag);
	}

	[PunRPC]
	private void throwObjectRPC(string cameraTag, string itemTag) {
		GameObject Item3D = GameObject.FindGameObjectWithTag (itemTag);
		Item3D.GetComponent<ThrowableObject> ().throwItem (cameraTag);
	}

	[PunRPC]
	private void resetObjectRPC(string itemTag) {
		GameObject Item3D = GameObject.FindGameObjectWithTag (itemTag);
		Item3D.GetComponent<ThrowableObject> ().collidedYet = false;
	}
}
