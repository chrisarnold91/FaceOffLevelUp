using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;
using System.Collections;
using UnityEngine.SceneManagement;

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
	private GameManager gameManager;
	private GameObject streakBonus;
	private SpriteRenderer bonus;
	private GameObject streakTimer;
	private SpriteRenderer sale;
	private GameObject sparkle;

	private int streak;
	private int timeLimit = 9;
	public float drawingTimer;
		
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
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager>();
		streakBonus = GameObject.Find ("StreakBonus");
		bonus = GameObject.Find ("Bonus").GetComponent<SpriteRenderer> ();
		streakTimer = GameObject.Find ("StreakTime");
		sale = GameObject.Find ("Sale").GetComponent<SpriteRenderer> ();

		if (cam.tag == "P1") {
			anim = GameObject.Find ("P1Char");
		} else { // tag == "P2"
			anim = GameObject.Find ("P2Char");
		}

		if (SceneManager.GetActiveScene ().name != "Splash") {
			if (cam.tag == "P1") {
				streak = gameManager.streakP1;
			} else {
				streak = gameManager.streakP2;
			}
		}

		drawingTimer = timeLimit - streak;
		if (drawingTimer < 3) {
			drawingTimer = 3;
		}
	}

	void Update()
	{
		if (!initiatedStartPoint) {
//			print ("turn off renderer!");
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
			if (SceneManager.GetActiveScene ().name != "Splash") {
				if (streak > 1) {
					streakBonus.GetComponent<MeshRenderer> ().enabled = true;
					streakBonus.GetComponent<TextMesh> ().text = (streak-1).ToString ();
					bonus.enabled = true;
				}
				streakTimer.GetComponent<MeshRenderer> ().enabled = true;
				sale.enabled = true;
			}
			brushMode (hitCoord);
		}

		if (SceneManager.GetActiveScene ().name != "Splash") {
			if (Item3D.GetComponentInParent<ThrowableObject> ().collidedYet) {
				if (Item3D.GetComponentInParent<ThrowableObject> ().hittingPlayer ()) {
					HUD.GetComponent<HealthBar> ().updateScore (roundScore);
				}

				Destroy (gameObject);
			}
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

				print (SceneManager.GetActiveScene ());
				if (SceneManager.GetActiveScene ().name == "Splash") {
					GameObject.Find ("Launcher").GetComponent<Launcher> ().Connect ();
				} else {

					sparkle = GameObject.Find ("Sparkle" + cam.tag);
					sparkle.GetComponent<ParticleSystem> ().Play ();

					roundScore = calculateScore ();
					int roundedScore = (int)roundScore;
					ShowMessage (roundedScore.ToString ());

					if (accuracy > 0.5 && drawingTimer > 0) {
						roundScore *= streak;
						gameManager.updateStreaks (cam.tag, streak + 1);
					} else {
						// reset streak to 1
						gameManager.updateStreaks (cam.tag, 1);
					}

					ShowBuzzword (accuracy);
					streakBonus.GetComponent<MeshRenderer> ().enabled = false;
					bonus.enabled = false;
					streakTimer.GetComponent<MeshRenderer> ().enabled = false;
					sale.enabled = false;
					Item2D.GetComponent<MeshRenderer> ().enabled = false;
					Item3D.GetComponentInParent<ThrowableObject> ().itemThrown = true;
					anim.GetComponent<AnimationManager> ().transitionToFinishedDrawing ();
					soundFXManager.playThrow ();
					gameManager.updateAccuracy (cam.tag, accuracy);
				}
			}
		} else {
			if (brushModeEnabled && !finishedDrawing) {
				if (!closeToStartPoint(hitCoord, thickness*5))
					leftStartPoint = true;
			}
		}

		Color pix = tex.GetPixel ((int)hitCoord.x, (int)hitCoord.y);

		if (pix.a > alphaThreshold) {
			alphaTimer += 1f;
		}

		timer += 1f;
		if (drawingTimer > 0) {
			drawingTimer -= Time.deltaTime;
		} else {
			drawingTimer = 0;
		}

		displayStreakTime (drawingTimer);


//		print ("alpha time: " + alphaTimer + ", total time: " + timer + ", accuracy = " + alphaTimer / timer * 100f + "%");
	}

	private void displayStreakTime(float time) {
		streakTimer.GetComponent<TextMesh> ().text = ((int)time).ToString ();
	}

	void ShowMessage (string score) {
		scoreText.GetComponent<Text> ().enabled = true;
		triggerShowScore ();
		string multiplier = "";
		if (streak > 1) {
			multiplier = " x" + streak.ToString ();
		}
		scoreText.GetComponent<Text> ().text = score + multiplier;
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
		string[] words3 = new string[] {"pretty good", "not bad I guess", "alright"};
		string[] words4 = new string[] {"sick job my guy", "wild", "swaggy"};
		string[] words5 = new string[] {"gnarly", "radical", "hot"};
		print("ACCURACY IS" + accuracy);
		if (accuracy <= 1) {
			print(0);
			message = "Uninstall";
		} 
		else if (accuracy <= 10 && accuracy > 1) {
			print(20);
			message = words1[Random.Range(0, words1.Length)];
		}
		else if (accuracy <= 30 && accuracy > 10) {
			print(40);
			message = words2[Random.Range(0, words2.Length)];
		}
		else if (accuracy <= 50 && accuracy > 30) {
			print(60);
			message = words3[Random.Range(0, words3.Length)];
		}
		else if (accuracy <= 70 && accuracy > 50) {
			print(80);
			message = words4[Random.Range(0, words4.Length)];
		}
		else {
			print(100);
			message = words5[Random.Range(0, words5.Length)];
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
