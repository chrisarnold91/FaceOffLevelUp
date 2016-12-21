using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;
using System.Collections;

public class VRDetectAlphas : MonoBehaviour {

	[SerializeField] private VRInteractiveItem m_InteractiveItem;
	[SerializeField] private Renderer m_Renderer;

	public GameObject Item2D;
	private GameObject Item3D;
	public int thickness = 100;
	public float baseScore = 50.0f;
	public float roundScore = 0;
	public bool brushModeEnabled = false;
	public bool finishedDrawing = false;
	public bool drawingSound = false;

	// the pixel a player must look at to enter brush mode
	public Vector2 startPoint;
	public Vector2 hitCoord;
	public bool initiatedStartPoint = false;
	public bool leftStartPoint = false;

	private float timer = 0f;
	private float alphaTimer = 1f;
	private float alphaThreshold = 0.95f;
	private float accuracy = 0;

	private GameObject buzzword;
	private GameObject scoreText;
	int isThrowingHash = Animator.StringToHash ("isThrowing");
	int showScoreHash = Animator.StringToHash ("showScore");
	Animator anima;
	Animator animaScore;

	private GameObject cam;
	private GameObject HUD;
	private GameObject anim;
	private Texture2D tex;
	private VREyeRaycaster vrEyeRaycaster;
	private SoundFXManager soundFXManager;

		
	void Start()
	{
		cam = GameObject.Find("PlayerCamera");
		HUD = GameObject.Find ("HUD");
		buzzword = GameObject.Find ("Buzzword");
		scoreText = GameObject.Find ("Score");
		anima = buzzword.GetComponent<Animator> ();
		animaScore = scoreText.GetComponent<Animator> ();

		tex = Instantiate(m_Renderer.material.mainTexture) as Texture2D;
		vrEyeRaycaster = cam.GetComponent<VREyeRaycaster> ();
		soundFXManager = GameObject.Find ("SoundFXManager").GetComponent <SoundFXManager> ();

		if (cam.tag == "P1") {
			anim = GameObject.Find ("P1Char");
		} else { // tag == "P2"
			anim = GameObject.Find ("P2Char");
		}
	}

	void Update()
	{
		if (!initiatedStartPoint) {
			print ("turn off renderer!");
			SwipeTrail st = GameObject.Find ("swipe(Clone)").GetComponent<SwipeTrail> ();
			TrailRenderer tr = st.GetComponent<TrailRenderer> ();
			tr.enabled = false;
		}

		hitCoord = vrEyeRaycaster.hitCoord;

		hitCoord.x *= tex.width;
		hitCoord.y *= tex.height;

		if (!initiatedStartPoint) {
			if (lookForAlphas (hitCoord)) {
				startPoint = hitCoord;
				initiatedStartPoint = true;
				drawingSound = true;
			}
		}

		if (drawingSound) {
			soundFXManager.playBrush ();
			drawingSound = false;
		}

		// hitCoord will only be non zero when reticle is over a 2D drawing
		if (initiatedStartPoint && !finishedDrawing) {
			brushMode (hitCoord);
		}

		if (Item3D.GetComponentInParent<ThrowableObject>().collidedYet) {
			if (Item3D.GetComponentInParent<ThrowableObject>().hittingPlayer()) {
				HUD.GetComponent<HealthBar> ().updateScore (roundScore);
			}

			Destroy(gameObject);
		}
	}

	public void assign3DObject(GameObject obj) {
		Item3D = obj;
	}

	bool lookForAlphas(Vector2 hitCoord) {
		if (tex.GetPixel ((int)hitCoord.x, (int)hitCoord.y).a > alphaThreshold) {
			return true;
		} else {
			return false;
		}
	}

	void brushMode(Vector2 hitCoord) {
		if (closeToStartPoint (hitCoord, thickness/2)) {
			brushModeEnabled = true;

			// player finished drawing
			if (leftStartPoint) {
				brushModeEnabled = false;
				finishedDrawing = true;

				roundScore = calculateScore ();
				int roundedScore = (int)roundScore;
				ShowMessage (roundedScore.ToString ());
				ShowBuzzword (accuracy);
				Item2D.GetComponent<MeshRenderer> ().enabled = false;
				Item3D.GetComponentInParent<ThrowableObject>().itemThrown = true;
				anim.GetComponent<AnimationManager> ().transitionToFinishedDrawing ();
				soundFXManager.playThrow ();
			}
		} else {
			if (brushModeEnabled && !finishedDrawing) {
				if (!closeToStartPoint(hitCoord, thickness*5))
					leftStartPoint = true;
			}
		}

		Color pix = tex.GetPixel ((int)hitCoord.x, (int)hitCoord.y);

		if (pix.a > alphaThreshold)
			alphaTimer += 1f;

		timer += 1f;

//		print ("alpha time: " + alphaTimer + ", total time: " + timer + ", accuracy = " + alphaTimer / timer * 100f + "%");
	}

	void ShowMessage (string score) {
		scoreText.GetComponent<Text> ().enabled = true;
		triggerShowScore ();
		scoreText.GetComponent<Text> ().text = score;
	}

	void ShowBuzzword(float accuracyFloat) {
		int accuracy = (int)(accuracyFloat * 100);
		string message;
		buzzword.GetComponent<Text>().enabled = true;
		triggerIsThrowing();
		//buzzword.GetComponent<Animator>().enabled = true;
		//buzzword.GetComponent<Animator>().Play("popup");
		string[] words1 = new string[] {"bad", "terrible", "awful"};
		string[] words2 = new string[] {"Okay", "acceptable", "fine", "meh"};
		string[] words3 = new string[] {"mediocore", "alright"};
		string[] words4 = new string[] {"sick job my guy", "wild", "swaggy"};
		string[] words5 = new string[] {"gnarly", "radical", "hot"};
		print("ACCURACY IS" + accuracy);
		if (accuracy == 0) {
			print(0);
			message = "Uninstall";
		} 
		else if (accuracy < 20 && accuracy > 0) {
			print(20);
			message = words1[Random.Range(0, words1.Length -1)];
		}
		else if (accuracy < 40 && accuracy > 20) {
			print(40);
			message = words2[Random.Range(0, words2.Length -1)];
		}
		else if (accuracy < 60 && accuracy > 40) {
			print(60);
			message = words3[Random.Range(0, words3.Length -1)];
		}
		else if (accuracy < 80 && accuracy > 60) {
			print(80);
			message = words4[Random.Range(0, words4.Length -1)];
		}
		else {
			print(100);
			message = words5[Random.Range(0, words5.Length -1)];
		}
		buzzword.GetComponent<Text>().text = message; 
	}

	public void triggerShowScore() {
		animaScore.SetTrigger (showScoreHash);
	}

	public void triggerIsThrowing() {
		anima.SetTrigger (isThrowingHash);
	}

	// this will have to change when we make the start point a circle
	bool closeToStartPoint(Vector2 coord, int proximity)
	{
		if (Mathf.Abs (coord.x - startPoint.x) < proximity &&
			Mathf.Abs (coord.y - startPoint.y) < proximity)
			return true;

		return false;
	}

	float calculateScore() {
		accuracy = alphaTimer / timer;
		return baseScore * accuracy;
	}
}
