using UnityEngine;
using System.Collections;

public class SoundFXManager : MonoBehaviour {

	public AudioSource efxSource;

	public AudioClip brushFX;
	public AudioClip throwFX;
	public AudioClip splatFX;
	public AudioClip splat1FX;
	public AudioClip splat2FX;
	public AudioClip splat3FX;
	public AudioClip splat4FX;
	public AudioClip splat5FX;
	public AudioClip defend1FX;
	public AudioClip defend2FX;
	public AudioClip hitShield1FX;
	public AudioClip hitShield2FX;

	public void playBrush() {
		efxSource.clip = brushFX;
		efxSource.Play ();
	}

	public void playThrow() {
		efxSource.clip = throwFX;
		efxSource.Play ();
	}

	public void playSplat() {
		AudioClip[] splat = { splatFX, splat1FX, splat2FX, splat3FX, splat4FX, splat5FX };
		efxSource.clip = splat[Random.Range(0, splat.Length)];
		efxSource.Play ();
	}

	public void playDefend() {
		AudioClip[] defend = { defend1FX, defend2FX };
		efxSource.clip = defend[Random.Range(0, defend.Length)];
		efxSource.Play ();
	}

	public void playHitShield() {
		AudioClip[] hit = { hitShield1FX, hitShield2FX };
		efxSource.clip = hit[Random.Range(0, hit.Length)];
		efxSource.Play ();
	}
}
