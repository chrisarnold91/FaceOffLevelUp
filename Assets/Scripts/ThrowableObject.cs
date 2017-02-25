using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ThrowableObject : Photon.PunBehaviour {

	public bool collidedYet = false;
	public bool itemThrown = false;
	public bool idling = false;
	private bool movingUp = true;

	private GameObject cam;
	private GameObject anim;
	private Rigidbody rb;
	private VRDetectAlphas drawingScript;
	private bool hitPlayer = false;
	private Vector3 highPoint;
	private Vector3 lowPoint;
	private SoundFXManager soundFXManager;
	private SpriteRenderer overlay;
	Animator anima;
	int showverlay = Animator.StringToHash ("showverlay");
	// Use this for initialization
	void Start () {
		cam = GameObject.Find("PlayerCamera");
		rb = GetComponent<Rigidbody> ();
		soundFXManager = GameObject.Find ("SoundFXManager").GetComponent <SoundFXManager> ();
		overlay = GameObject.Find ("overlay").GetComponent<SpriteRenderer>();
		anima = overlay.GetComponent<Animator> ();

		if (cam.tag == "P1") {
			highPoint = new Vector3 (-20f, 0f, 1010f);
			lowPoint = new Vector3 (-20f, -40f, 1010f);
		} else {
			anim = GameObject.Find ("P2Char");
			highPoint = new Vector3 (20f, 0f, -10f);
			lowPoint = new Vector3 (20f, -40f, -10f);
		}
	}

	void Update () {
		if (idling) {
			tossItem ();
		}

		if (itemThrown && !collidedYet) {
			GameObject.Find ("ObjectManager").GetComponent<ObjectManager> ().throwObject(gameObject);
		}
	}

	// called by ObjectManager
	public void throwItem(string who) {
		// reposition item so it hits opponent
		transform.position = new Vector3 (0f, 0f, transform.position.z);

		GetComponentInChildren<BoxCollider> ().enabled = true;
		foreach (MeshRenderer child in GetComponentsInChildren<MeshRenderer>()) {
			child.enabled = true;
		}

		if (who == "P2") {
			rb.velocity = new Vector3(0f, 100 * Mathf.Sin(15), 1600 * Mathf.Cos(90));
		} else {
			rb.velocity = new Vector3(0f, 90 * Mathf.Sin(19), 1600 * Mathf.Cos(0));
		}

		rb.useGravity = true;
		rb.AddTorque (-transform.right * 10000); 

		itemThrown = false;
		idling = false;
	}

	private void tossItem() {
		if (movingUp) {
			print ("moving up");
			transform.position = Vector3.MoveTowards (transform.position, highPoint, 1.25f);
		} else {
			print ("moving down");
			transform.position = Vector3.MoveTowards (transform.position, lowPoint, 1.25f);
		}

		if (transform.position == highPoint) {
			movingUp = false;
		}

		if (transform.position == lowPoint) {
			movingUp = true;
		}
	}


	void OnCollisionEnter (Collision col) {
		collidedYet = true;
		if (col.gameObject.tag == "P1" || col.gameObject.tag == "P2") {
			if (col.gameObject.tag == "P1") {
				anim = GameObject.Find ("P1Char");
				if (cam.tag == "P1") {
					anima.SetTrigger (showverlay);
				}
			} else {
				anim = GameObject.Find ("P2Char");
				if (cam.tag == "P2") {
				anima.SetTrigger (showverlay);
				}
			}

			anim.GetComponent<AnimationManager> ().triggerGettingHit (col.gameObject.tag);
			hitPlayer = true;
			soundFXManager.playSplat ();
		}
	}

	public bool hittingPlayer() {
		return hitPlayer;
	}
}
