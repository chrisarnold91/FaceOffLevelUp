using UnityEngine;
using System.Collections;

namespace VRStandardAssets.Utils
{
	public class SwipeTrail : MonoBehaviour {

		public GameObject drawing;
		public Vector3 fwd;
		public Vector3 P2correction = new Vector3(0f, 0f, 0f);

		private GameObject cam;
		private TrailRenderer tr;

		void Start() {
			cam = GameObject.Find("PlayerCamera");
			tr = GetComponent<TrailRenderer> ();

			if (cam.tag == "P2") {
				P2correction = new Vector3 (0f, 0f, 1000f);
			}
		}

		void Update ()
		{
			// check if brush mode is enabled
			if (drawing.GetComponent<VRDetectAlphas> ().brushModeEnabled) {
				GetComponent<TrailRenderer> ().enabled = true;
				fwd = cam.transform.TransformDirection (Vector3.forward) * 10 + P2correction;

				// uh LOL WE DON'T EVEN NEED THESE TWO LINES EITHER... this is a 2 line function -_-
//				Plane objPlane = new Plane (Camera.main.transform.forward * -1, this.transform.position);
//				Ray mRay = Camera.main.ScreenPointToRay (fwd);

				// move trail renderer to where the camera is pointing
				this.transform.position = fwd;
			}

			// doesn't work with VRDetectAlphas: changing namespace to UnityEditor to make SerializedObject work
//			SerializedObject so = new SerializedObject (tr);
//
//			for (int c = 0; c < 5; c++) {
//				so.FindProperty ("m_Colors.m_Color[" + c + "]").colorValue = Color.red;
//			}

			// when finished drawing, hide the Trail Renderer
			if (drawing.GetComponent<VRDetectAlphas> ().finishedDrawing) {
				GetComponent<TrailRenderer> ().enabled = false;
				Destroy (gameObject);
			}
		}
	}
}
