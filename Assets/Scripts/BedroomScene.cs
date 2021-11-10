using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CardboardAudioSource))]
public class BedroomScene : MonoBehaviour {
	public GameObject easel;
	public LayerMask layerMask;
	private bool hasIntroAudioBeenPlayed = false;
	private AudioSource audioSource;
	private bool hasFurnitureAudioBeenPlayed = false;
	private bool shouldStartDreamSequence = false;
	private bool hasWallAudioBeenPlayed = false;
	private bool hasPerspectiveAudioBeenPlayed = false;
	public static bool isDreamComplete = false;
	public static bool isPaintingComplete = false;
	public AudioClip introAudioClip;
	public AudioClip paintStarryNightClip;
	public AudioClip paintedFurnitureClip;
	public AudioClip sleepClip;
	public AudioClip wallsClip;
	public AudioClip perspectiveRoomClip;
	public GameObject ambientSounds;
	public AudioClip ambientSoundClip;
	private float waitBeforeSwitch = 1.2f;
	private bool isAmbientAudioStarted = false;
	private CardboardAudioSource ambientAudioSource;
	public GameObject reticle;

	// Use this for initialization
	void Start () {
		audioSource = gameObject.GetComponent<AudioSource> ();
		//audioSource.volume = 0.3f;

		/*ambientAudioSource = ambientSounds.GetComponent<CardboardAudioSource> ();
		ambientAudioSource.clip = ambientSoundClip;
		ambientAudioSource.loop = true;
		ambientAudioSource.volume = 0.33f;
		ambientAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
		ambientAudioSource.minDistance = 1;
		ambientAudioSource.maxDistance = 2;
		ambientAudioSource.Play ();*/
	}
	
	// Update is called once per frame
	void Update () {
		audioSource.volume = 0.3f;
		RaycastHit hitInfo;

		if (Physics.Raycast (Cardboard.SDK.GetComponentInChildren<CardboardHead> ().Gaze, out hitInfo, Mathf.Infinity, layerMask)) {
			GameObject hitObject = hitInfo.transform.gameObject;
			if (hitObject.name.Contains ("bad")) {
				reticle.GetComponent<CardboardReticle> ().OnGazeStart (this.gameObject.GetComponent<Camera> (), hitObject, hitInfo.point);
			} else if (hitObject.name.Contains ("chair")) {
				reticle.GetComponent<CardboardReticle> ().OnGazeStart (this.gameObject.GetComponent<Camera> (), hitObject, hitInfo.point);
			} else if (hitObject.name.Contains ("walls")) {
				reticle.GetComponent<CardboardReticle> ().OnGazeStart (this.gameObject.GetComponent<Camera> (), hitObject, hitInfo.point);
			} else {
				reticle.GetComponent<CardboardReticle> ().OnGazeExit (this.gameObject.GetComponent<Camera> (), hitObject);
			}
		}

		if(!isAmbientAudioStarted) {
			ambientAudioSource = ambientSounds.GetComponent<CardboardAudioSource> ();
			ambientAudioSource.clip = ambientSoundClip;
			ambientAudioSource.loop = true;
			ambientAudioSource.volume = 0.33f;
			ambientAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
			ambientAudioSource.minDistance = 1;
			ambientAudioSource.maxDistance = 2;
			ambientAudioSource.Play ();
			isAmbientAudioStarted = true;
		}
		if (!isDreamComplete) {
			if (!hasIntroAudioBeenPlayed) {
				/*Debug.Log ("Ahh! Home! This is the only place where I can find peace and comfort. " +
				"When I painted this room on canvas, I made sure that color was doing everything.");*/
				audioSource.clip = introAudioClip;
				if (!audioSource.isPlaying) {
					audioSource.Play ();
					hasIntroAudioBeenPlayed = true;
				}
			}
		} else {
			if (!hasIntroAudioBeenPlayed) {
				easel.SetActive (true);
				//Debug.Log ("Huh! I should put what I saw onto the canvas.");
				audioSource.clip = paintStarryNightClip;
				if (!audioSource.isPlaying) {
					audioSource.Play ();
					hasIntroAudioBeenPlayed = true;
				}
			}
		}

		if (Input.GetButtonDown ("Fire1")) {
			if (!isDreamComplete) {
				if (!hasIntroAudioBeenPlayed) {
					/*Debug.Log ("Ahh! Home! This is the only place where I can find peace and comfort. " +
						"When I painted this room, I made sure that color was doing everything.");*/
					audioSource.clip = introAudioClip;
					if (!audioSource.isPlaying) {
						audioSource.Play ();
						hasIntroAudioBeenPlayed = true;
					}
				} else {
					if (Physics.Raycast (Cardboard.SDK.GetComponentInChildren<CardboardHead> ().Gaze, out hitInfo, Mathf.Infinity, layerMask)) {
						GameObject hitObject = hitInfo.transform.gameObject;
						//Debug.Log ("hitObject :: " + hitObject.name);
						if (hitObject.name.Contains ("bad")) {
							if (!hasFurnitureAudioBeenPlayed) {
								/*Debug.Log ("I have painted the wood of the furnitures in fresh and warm yellow to express a sense of rest." +
									"Come to think of it, I should sleep for sometime.");*/
								audioSource.clip = paintedFurnitureClip;
								if (!audioSource.isPlaying) {
									audioSource.Play ();
									//shouldStartDreamSequence = true;
								}
								hasFurnitureAudioBeenPlayed = true;
							} else {
								//Debug.Log ("I should sleep for sometime.");
								audioSource.clip = sleepClip;
								if (!audioSource.isPlaying) {
									//audioSource.Play ();
									shouldStartDreamSequence = true;
								}
							}
						} else if (hitObject.name.Contains ("chair")) {
							if (!hasFurnitureAudioBeenPlayed) {
								//Debug.Log ("I have painted the wood of the furnitures in fresh and warm yellow to express a sense of rest.");
								audioSource.clip = paintedFurnitureClip;
								if (!audioSource.isPlaying) {
									audioSource.Play ();
								}
								hasFurnitureAudioBeenPlayed = true;
							} else {
								//Debug.Log ("I should sleep for sometime.");
								audioSource.clip = sleepClip;
								if (!audioSource.isPlaying) {
									audioSource.Play ();
								}
							}
						} else if (hitObject.name.Contains ("walls")) {
							if (!hasWallAudioBeenPlayed) {
								//Debug.Log ("The walls are pale blue to evoke a sense of inviolable calmness.");
								audioSource.clip = wallsClip;
								if (!audioSource.isPlaying) {
									audioSource.Play ();
								}
								if (!audioSource.isPlaying) {
									hasWallAudioBeenPlayed = true;
								}
							} else {
								//Debug.Log ("I should sleep for sometime.");
								audioSource.clip = sleepClip;
								if(!audioSource.isPlaying) {
									audioSource.Play ();
								}
							}
						} else {
							if (!hasPerspectiveAudioBeenPlayed) {
								//Debug.Log ("I have painted the bedroom in a skewed perspective so as to put the viewer in the room.");
								audioSource.clip = perspectiveRoomClip;
								if(!audioSource.isPlaying) {
									audioSource.Play ();
								}
								if (!audioSource.isPlaying) {
									hasPerspectiveAudioBeenPlayed = true;
								}
							} else {
								//Debug.Log ("I should sleep for sometime.");
								audioSource.clip = sleepClip;
								audioSource.Play ();
							}
						}
					}
				}
			} else {
				if (!hasIntroAudioBeenPlayed) {
					//Debug.Log ("Huh! I should put what I saw onto the canvas.");
					audioSource.clip = paintStarryNightClip;
					audioSource.Play ();
					if (!audioSource.isPlaying) {
						hasIntroAudioBeenPlayed = true;
					}
				}
			}

		}

		if(shouldStartDreamSequence) {
			if(waitBeforeSwitch <= 0) {
				RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
				SceneManager.LoadScene (2);
			}
			waitBeforeSwitch -= Time.deltaTime;
		}

		if(isPaintingComplete) {
			SceneManager.LoadScene (0);
		}
	}
}