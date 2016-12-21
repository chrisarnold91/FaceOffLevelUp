using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
	
	public AudioSource efxSource;

	public AudioClip welcome;
	public AudioClip tutorial1;
	public AudioClip tutorial2;
	public AudioClip tutorial3;
	public AudioClip insult;
	public AudioClip remember;
	public AudioClip hate;
	public AudioClip cleanUp;
	public AudioClip entertaining;
	public AudioClip closing;

	public void playWelcome() {
		efxSource.clip = welcome;
		efxSource.Play ();
	}

	public void playSelect() {
		efxSource.clip = tutorial1;
		efxSource.Play ();
	}

	public void playDraw() {
		efxSource.clip = tutorial2;
		efxSource.Play ();
	}

	public void playDontActually() {
		efxSource.clip = tutorial3;
		efxSource.Play ();
	}

	public void playInsult() {
		efxSource.clip = insult;
		efxSource.Play ();
	}

	public void playRemember() {
		efxSource.clip = remember;
		efxSource.Play ();
	}

	public void playHate() {
		efxSource.clip = hate;
		efxSource.Play ();
	}

	public void playCleanUp() {
		efxSource.clip = cleanUp;
		efxSource.Play ();
	}

	public void playEntertaining() {
		efxSource.clip = entertaining;
		efxSource.Play ();
	}

	public void playClosing() {
		efxSource.clip = closing;
		efxSource.Play ();
	}
}
