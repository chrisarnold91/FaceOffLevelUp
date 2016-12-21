using UnityEngine;
using System.Collections;

public class SoundFXManager : MonoBehaviour {

	public AudioSource efxSource;

	public AudioClip brushFX;
	public AudioClip throwFX;
	public AudioClip splatFX;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void playBrush() {
		efxSource.clip = brushFX;
		efxSource.Play ();
	}

	public void playThrow() {
		efxSource.clip = throwFX;
		efxSource.Play ();
	}

	public void playSplat() {
		efxSource.clip = splatFX;
		efxSource.Play ();
	}
}
