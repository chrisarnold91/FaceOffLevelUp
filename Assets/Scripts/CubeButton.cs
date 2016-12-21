using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VRStandardAssets.Utils;

// This script is for loading scenes from the main menu.
// Each 'button' will be a rendering showing the scene
// that will be loaded and use the SelectionRadial.
public class CubeButton : Photon.PunBehaviour
{
	public event Action<CubeButton> OnButtonSelected;
	// This event is triggered when the selection of the button has finished.

	private SelectionRadial m_SelectionRadial;
	// This controls when the selection is complete.

	[SerializeField] private VRInteractiveItem m_InteractiveItem;
	// The interactive item for where the user should click to load the level.

	[SerializeField] private GameObject Item2D;
	[SerializeField] private GameObject Swipe;

	// Whether the user is looking at the VRInteractiveItem currently.
	private bool m_GazeOver;

	private GameObject cam;
	private GameObject anim;
	private String activePlayer;

	private Vector3 drawArea = new Vector3 (0f, 0f, 100f);
	private bool itemSelected = false;
	public bool finishedMoving = true;

	public void Start ()
	{
		cam = GameObject.Find ("PlayerCamera");
		m_SelectionRadial = cam.GetComponent<SelectionRadial> ();
		m_SelectionRadial.OnSelectionComplete += HandleSelectionComplete;

		if (cam.tag == "P1") {
			anim = GameObject.Find ("P1Char");
		} else { // tag == "P2"
			anim = GameObject.Find ("P2Char");
		}
	}

	public void Update ()
	{
		if (itemSelected && !Item2D.GetComponent<VRDetectAlphas> ().finishedDrawing) {
			GameObject parent = transform.parent.gameObject;
			// tell game manager that you selected an item
			GameObject.Find ("ObjectManager").GetComponent<ObjectManager> ().moveObject(parent);
			// reset collidedYet in case object gets picked up again
			GameObject.Find ("ObjectManager").GetComponent<ObjectManager> ().resetObject(parent);
			GetComponent<BoxCollider> ().enabled = false;
			itemSelected = false;
			print ("2D OBJECT pre instantiated:" + Item2D);
		}

		if (!finishedMoving) {
			moveItem ();
		}
	}

	// called by ObjectManager
	public void readyToMove(string who) {
		activePlayer = who;
		if (activePlayer == "P2") {
			drawArea = new Vector3 (0f, 0f, 900f);
		} else {
			drawArea = new Vector3 (0f, 0f, 100f);
		}

		GetComponentInParent<Rigidbody> ().useGravity = false;
		finishedMoving = false;
	}

	private void moveItem() {
		// you should see your opponent's item go to their hand
		if (activePlayer != cam.tag) {
			if (cam.tag == "P2") {
				drawArea = new Vector3 (20f, -40f, -10f);
			} else {
				drawArea = new Vector3 (-20f, -40f, 1010f);
			}
				
			transform.parent.position = drawArea;
		}

		if (activePlayer == cam.tag) {
			// if you are grabbing food, food should float to draw area
			transform.parent.position = Vector3.MoveTowards (transform.parent.position, drawArea, 10);
		} else {
			// if your opponent is grabbing food, you should see their grabbing animation
			StartCoroutine (grabItem (1));
		}

		if (transform.parent.position == drawArea) {
			finishedMoving = true;

			// if you are the active player, 2D drawing should appear in front of you
			if (activePlayer == cam.tag) {
				GetComponentInChildren<MeshRenderer>().enabled = false;
				create2DDrawing ();
			} else {
				// else you are not the active player, you should see your opponent idling (tossing food in their hand)
				GetComponentInParent<ThrowableObject> ().idling = true;
			}
		}
	}
	
	IEnumerator grabItem(int delay) {
		GetComponentInChildren<MeshRenderer>().enabled = false;
		yield return new WaitForSeconds (delay);
		transform.parent.position = drawArea;
		GetComponentInChildren<MeshRenderer>().enabled = true;
	}

	private void create2DDrawing ()
	{
		GameObject clone2D = GameObject.Instantiate (Item2D);
		GameObject cloneSwipe = GameObject.Instantiate (Swipe);
		print ("2D OBJECT:" + clone2D);
		cloneSwipe.GetComponent<SwipeTrail> ().drawing = clone2D;
		clone2D.GetComponent<VRDetectAlphas> ().assign3DObject (gameObject);

		if (cam.tag == "P2") {
			clone2D.transform.position = new Vector3 (0f, 0f, 990f);
			clone2D.transform.rotation = Quaternion.Euler (90f, 0f, 0f);
		}
	}

	private void OnEnable ()
	{
		m_InteractiveItem.OnOver += HandleOver;
		m_InteractiveItem.OnOut += HandleOut;
	}


	private void OnDisable ()
	{
		m_InteractiveItem.OnOver -= HandleOver;
		m_InteractiveItem.OnOut -= HandleOut;
		m_SelectionRadial.OnSelectionComplete -= HandleSelectionComplete;
	}


	private void HandleOver ()
	{
		// When the user looks at the rendering of the scene, show the radial.
		m_SelectionRadial.Show ();
		m_SelectionRadial.HandleDown ();

		m_GazeOver = true;
	}


	private void HandleOut ()
	{
		// When the user looks away from the rendering of the scene, hide the radial.
		m_SelectionRadial.HandleUp ();

		m_GazeOver = false;
	}


	private void HandleSelectionComplete ()
	{
		bool gameOver = GameObject.Find ("GameManager").GetComponent<GameManager> ().gameOver;
		// If the user is looking at the rendering of the scene when the radial's selection finishes, activate the button.
		if (m_GazeOver && !gameOver) {
			if (tag == "papertowel") {
				//heal yourself
				GameObject.Find("HUD").GetComponent<HealthBar>().heal();
			} else {
				itemSelected = true;
				anim.GetComponent<AnimationManager> ().transitionToItemSelected ();
			}
		}
	}
}
