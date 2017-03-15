using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//public class HealthBar : Photon.PunBehaviour, IPunObservable {
public class HealthBar : Photon.PunBehaviour {

	public Image controlbar;
	public static bool changed;
	public static bool P2Changed;
	public Text gameOverText;

	public float rectWidth;
	public float rectHeight;
	private float totalWidth = 500f;

	private GameObject cam;
	private GameObject towel;

	public bool used = true;
	public float timer = 10.0f;
	public float globalTimer = 90.0f;

	// Use this for initialization
	void Start () {
		cam = GameObject.Find ("PlayerCamera");

		if (cam.tag == "P1") {
			towel = GameObject.Find ("P1PaperTowel");
		} else {
			towel = GameObject.Find ("P2PaperTowel");
		}

		rectWidth = controlbar.GetComponent<RectTransform> ().rect.width;
		rectHeight = controlbar.GetComponent<RectTransform>().rect.height;

	}

	// Update is called once per frame
	void Update () {
		if (photonView.isMine == false && PhotonNetwork.connected == true) {
			return;
		}

		if (used) {
			timer -= Time.deltaTime;
		}

//		if(timer <= 0.0f && used){
//			//reset timer
//			timer = 10.0f;
//
//			//can use
//			used = false;
//
//			towel.GetComponentInChildren<MeshRenderer>().enabled = true;
//			SpawnRandomPlace();
//		}
	}

	public void updateScore (float damage) {
		PhotonView pv = PhotonView.Get(this);
		pv.RPC ("changeHealthRPC", PhotonTargets.All, cam.tag, damage);
	}

	public void SpawnRandomPlace(){
		int randz;

		float[] randxs = new float[] { -120f, 120f };
		int randx = Random.Range(0, randxs.Length);

		float[] randys = new float[] { 41.5f, 10.1f, -21.6f, -53.0f, -84.6f, -141.4f };
		int randy = Random.Range(0, randys.Length);

		if (cam.tag == "P1") {
			randz = Random.Range (-115, 260);
		} else {
			randz = Random.Range (750, 1080);
		}

		//make sure enabled
		towel.GetComponentInChildren<MeshRenderer>().enabled = true;

		//set random position
		towel.transform.position = new Vector3(randxs[randx], randys[randy], randz);
	}

	public void heal() {
		PhotonView pv = PhotonView.Get(this);
		pv.RPC ("changeHealthRPC", PhotonTargets.All, cam.tag, 25f);

		towel.GetComponentInChildren<MeshRenderer>().enabled = false;
		used = true;
	}

	[PunRPC]
	private void changeHealthRPC(string attacker, float damage) {
		TextMesh YouScore1 = GameObject.Find("YouScore1").GetComponent<TextMesh>();
		TextMesh ThemScore1 = GameObject.Find ("ThemScore1").GetComponent<TextMesh>();
		TextMesh YouScore2 = GameObject.Find ("YouScore2").GetComponent<TextMesh>();
		TextMesh ThemScore2 = GameObject.Find ("ThemScore2").GetComponent<TextMesh>();

		print ("SCORES");
		print (YouScore1);
		print (ThemScore1);

		if (attacker == "P1") {
			int newScore = int.Parse(YouScore1.text) + (int)damage;
			YouScore1.text = newScore.ToString();
			ThemScore2.text = newScore.ToString();
		} else {
			int newScore = int.Parse(ThemScore1.text) + (int)damage;
			ThemScore1.text = newScore.ToString();
			YouScore2.text = newScore.ToString();
		}
	}
}
