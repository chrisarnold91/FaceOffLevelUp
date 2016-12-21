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
			pv.RPC ("moveObjectRPC", PhotonTargets.All, cam.tag, obj.name);
		}
		holdingSomething = true;
	}

	public void throwObject(GameObject obj) {
		PhotonView pv = PhotonView.Get (this);
		pv.RPC ("throwObjectRPC", PhotonTargets.All, cam.tag, obj.name);
		holdingSomething = false;
	}

	public void resetObject(GameObject obj) {
		PhotonView pv = PhotonView.Get (this);
		pv.RPC ("resetObjectRPC", PhotonTargets.All, obj.name);
	}

	[PunRPC]
	private void moveObjectRPC(string cameraTag, string itemName) {
		print ("ITEM NAME: " + itemName);
		GameObject Item3D = GameObject.Find (itemName);
		print ("ITEM 3D: " + Item3D);
		Item3D.GetComponentInChildren<CubeButton> ().readyToMove (cameraTag);
	}

	[PunRPC]
	private void throwObjectRPC(string cameraTag, string itemName) {
		GameObject Item3D = GameObject.Find (itemName);
		Item3D.GetComponent<ThrowableObject> ().throwItem (cameraTag);
	}

	[PunRPC]
	private void resetObjectRPC(string itemName) {
		GameObject Item3D = GameObject.Find (itemName);
		Item3D.GetComponent<ThrowableObject> ().collidedYet = false;
	}
}
