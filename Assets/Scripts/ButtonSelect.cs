using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VRStandardAssets.Utils;

namespace VRStandardAssets.Menu
{
	// This script is for loading scenes from the main menu.
	// Each 'button' will be a rendering showing the scene
	// that will be loaded and use the SelectionRadial.
	public class ButtonSelect : Photon.PunBehaviour
	{
		public event Action<ButtonSelect> OnButtonSelected;
		public Boolean outsideRoom;
		// This event is triggered when the selection of the button has finished.

		private VRCameraFade m_CameraFade;
		// This fades the scene out when a new scene is about to be loaded.
		private SelectionRadial m_SelectionRadial;
		// This controls when the selection is complete.
		[SerializeField] private VRInteractiveItem m_InteractiveItem;
		// The interactive item for where the user should click to load the level.
		[SerializeField] private GameObject Item3D;

		private bool m_GazeOver;
		// Whether the user is looking at the VRInteractiveItem currently.

		private Vector3 Item3Dposition;
		private Vector3 drawArea;
		private bool itemSelected = false;
		private bool selectionComplete = false;
		private GameObject cam;


		public void Start ()
		{
			cam = GameObject.Find ("PlayerCamera");
			m_CameraFade = cam.GetComponent<VRCameraFade> ();
			m_SelectionRadial = cam.GetComponent<SelectionRadial> ();
			m_SelectionRadial.OnSelectionComplete += HandleSelectionComplete;

			if (cam.tag == "P1") {
				drawArea = new Vector3 (0f, 0f, 25f);

			} else { // tag == "P2"
				drawArea = new Vector3 (0f, 0f, 975f);
			}
		}

		private void OnEnable ()
		{
			m_InteractiveItem.OnOver += HandleOver;
			m_InteractiveItem.OnOut += HandleOut;
			//			m_SelectionRadial.OnSelectionComplete += HandleSelectionComplete; // moved to Start
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
			//m_SelectionRadial.Hide();
			m_SelectionRadial.HandleUp ();

			m_GazeOver = false;
		}


		private void HandleSelectionComplete ()
		{
			// If the user is looking at the rendering of the scene when the radial's selection finishes, activate the button.
			if (m_GazeOver) {
				if (outsideRoom) {
					GameObject.Find ("Launcher").GetComponent<Launcher> ().Connect ();
				} else {
					GameObject.Find ("GameManager").GetComponent<GameManager> ().LeaveRoom ();
				}
				this.enabled = false;
			}
		}
	}
}
