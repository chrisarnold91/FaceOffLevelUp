using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Shield : MonoBehaviour {
	public Image indicator;
	public float CooldownTimer = 5.0f;
	public float shieldDuration = 2.0f;
	public bool ShieldIsOn = false;
	public bool ShieldAvailable = true;

	private GameObject cam;
	private GameObject anim;
	private Text coolDown;

	public void Start() {
		cam = GameObject.Find ("PlayerCamera");
		indicator = GameObject.Find ("shielded").GetComponent<Image>();
		if (cam.tag == "P1") {
			anim = GameObject.Find ("P1Char");
		} else {
			anim = GameObject.Find ("P2Char");
		}
		coolDown = GameObject.Find ("CoolDownTimer").GetComponent<Text>();
	}

	public void Update () {
		if(ShieldIsOn){
			shieldDuration -= Time.deltaTime;
		}

		// put up shield on click
		if(Input.GetMouseButtonDown(0) && ShieldAvailable) {
			if (gameObject.tag == "shield1" && cam.tag == "P1"
				|| gameObject.tag == "shield2" && cam.tag == "P2") {
				GameObject.Find ("GameManager").GetComponent<ShieldManager> ().activateShield (gameObject);
			}
		}

		// shield has expired, put down shield
		if(shieldDuration <= 0.0f){
			ShieldIsOn = false;
			GetComponentInChildren<Renderer>().enabled = false;
			GetComponentInChildren<BoxCollider> ().enabled = false;
			shieldDuration = 2.0f;
		}

		// shield is cooling down
		if (gameObject.tag == "shield1" && cam.tag == "P1"
		    || gameObject.tag == "shield2" && cam.tag == "P2") {
			if (!ShieldAvailable) {
				CooldownTimer -= Time.deltaTime;
				Color c = indicator.color;
				c.a = 0.3f;
				indicator.color = c;
//			indicator.GetComponent<Image>().enabled = false;

				if (CooldownTimer <= 1.0f) {
					coolDown.text = "1";
				} else if (CooldownTimer <= 2.0f) {
					coolDown.text = "2";
				} else if (CooldownTimer <= 3.0f) {
					coolDown.text = "3";
				} else if (CooldownTimer <= 4.0f) {
					coolDown.text = "4";
				}
			}
		}

		// shield has finished cooling down
		if(CooldownTimer <= 0.0f){
			ShieldAvailable = true;
			CooldownTimer = 5.0f;
			Color c = indicator.color;
			c.a = 1.0f;
			indicator.color = c;
//			indicator.GetComponent<Image>().enabled = true;

			coolDown.text = "5";
			coolDown.enabled = false;
		}
	}

	// called by ShieldManager
	public void putUpShield(string activeShield) {
		GetComponentInChildren<MeshRenderer> ().enabled = true;
		GetComponentInChildren<BoxCollider> ().enabled = true;
		anim.GetComponent<AnimationManager> ().triggerShielding (activeShield);
		ShieldAvailable = false;
		ShieldIsOn = true;
		coolDown.enabled = true;
	}
}