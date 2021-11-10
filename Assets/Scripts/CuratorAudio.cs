using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CardboardAudioSource))]
public class CuratorAudio : MonoBehaviour {
	public AudioClip audioClip1;
	public AudioClip audioClip2;
	private CardboardAudioSource audioSource;
	private bool haveBothClipsBeenPlayed = false;
	private bool hasFirstClipBeenPlayed = false;
	private bool hasFinalClipBeenPlayed = false;
	public GameObject theCurator;
	public AudioClip audioClip3;
	public GameObject galleryMusic;

	// Use this for initialization
	void Start () {
		audioSource = gameObject.GetComponent<CardboardAudioSource> ();
		audioSource.volume = 1.0f;
		audioSource.rolloffMode = AudioRolloffMode.Linear;
		audioSource.minDistance = 1.0f;
		audioSource.maxDistance = 500.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(!BedroomScene.isPaintingComplete) {
			//if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0) {
				if (!haveBothClipsBeenPlayed) {
					if (!hasFirstClipBeenPlayed) {
						audioSource.clip = audioClip1;
						audioSource.Play ();
						//this.gameObject.GetComponent<Animator> ().enabled = true;
						hasFirstClipBeenPlayed = true;
					}
					if (!audioSource.isPlaying) {
						audioSource.clip = audioClip2;
						audioSource.Play ();
						haveBothClipsBeenPlayed = true;
					}
				}
			//}
		} else {
			CardboardAudioSource galleryMusicAudioSource = galleryMusic.GetComponent<CardboardAudioSource> ();
			galleryMusicAudioSource.Stop ();
			if(!hasFinalClipBeenPlayed) {
				audioSource.clip = audioClip3;
				audioSource.Play ();
				hasFinalClipBeenPlayed = true;
			}

			if(!audioSource.isPlaying) {
				Application.Quit ();
			}
		}
	}
}
