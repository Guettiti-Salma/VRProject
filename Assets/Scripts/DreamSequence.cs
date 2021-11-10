using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DreamSequence : MonoBehaviour {
	private float waitBeforeSequenceStarts = 1.2f;
	private bool isDreamStartAudioPlayed = false;
	public LayerMask layerMask;
	private bool isMoonTransformed = false;
	private bool isStarTransformed;
	private GameObject[] gameObjectsForTransform;
	public Dictionary<string, GameObject> starsInVincentVisionReference;
	private bool shouldStartGraveSequence = false;
	private GameObject[] graveSequenceGameObjects;
	private GameObject[] graveSequenceGameObjectsReference;
	private bool shouldWakeUp = false;
	public Material secondSkyboxMaterial;
	private float blend = 0.0f;
	public GameObject vincentFriend;
	public GameObject gravestone;
	public GameObject thePlayer;
	private bool hasSkyMonologueBeenPlayed = false;
	private bool hasDepressionClipBeenPlayed = false;
	private AudioSource audioSource;
	public AudioClip dreamStartClip;
	public AudioClip gachetClip;
	public AudioClip iSeeSkyClip;
	public AudioClip illnessClip;
	private float waitBeforeGachet;
	private float waitBeforeGrave;
	private float waitBeforeWake;
	private CardboardAudioSource gachetAudioSource;
	private bool hasGachetAudioBeenPlayed = false;
	public GameObject reticle;
	public GameObject hallucinationAudio;
	private bool isHallucinationAudioPlayed = false;

	// Use this for initialization
	void Start () {
		gameObjectsForTransform = GameObject.FindGameObjectsWithTag ("ForVincentVision");
		GameObject[] starsInVincentVision = GameObject.FindGameObjectsWithTag ("VincentVisionStars");
		starsInVincentVisionReference = new Dictionary<string, GameObject> ();
		foreach (GameObject gameObject in starsInVincentVision) {
			starsInVincentVisionReference.Add (gameObject.name, gameObject);
			gameObject.SetActive (false);
		}

		graveSequenceGameObjects = GameObject.FindGameObjectsWithTag ("GraveDream");
		graveSequenceGameObjectsReference = graveSequenceGameObjects;
		foreach (GameObject gameObject in graveSequenceGameObjects) {
			gameObject.SetActive (false);
		}

		audioSource = GetComponent<AudioSource> ();
		audioSource.volume = 0.2f;

		gachetAudioSource = vincentFriend.GetComponent<CardboardAudioSource> ();
		gachetAudioSource.clip = gachetClip;
		gachetAudioSource.volume = 0.6f;

		waitBeforeGachet = dreamStartClip.length + 0.1f;
		waitBeforeGrave = iSeeSkyClip.length;
		waitBeforeWake = illnessClip.length + 2f;
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hitInfo;
		if (waitBeforeSequenceStarts > 0) {
			waitBeforeSequenceStarts -= Time.deltaTime;
		} else {
			if (!isDreamStartAudioPlayed) {
				//Debug.Log ("Whoa! Where am I? Is this a dream? Dr. Gachet, is that you?");
				audioSource.clip = dreamStartClip;
				audioSource.Play ();
				isDreamStartAudioPlayed = true;
			}
		}

		if(waitBeforeGachet > 0) {
			waitBeforeGachet -= Time.deltaTime;
		} else {
			if (isDreamStartAudioPlayed && !shouldStartGraveSequence) {
				if(!hasGachetAudioBeenPlayed) {
					//Debug.Log ("Yes Vincent, my friend! Look at the beautiful night sky above you!");
					gachetAudioSource.Play ();
					hasGachetAudioBeenPlayed = true;
				}
				Vector3 lookDirection = Cardboard.SDK.GetComponentInChildren<CardboardHead> ().Gaze.direction;
				foreach (GameObject gameObject in gameObjectsForTransform) {
					if (gameObject.tag == "ForVincentVision") {

						//Transform moon
						if (gameObject.name == "crescent moon" && !isMoonTransformed) {
							Vector3 moonDirection = gameObject.transform.position - transform.position;
							float moonAngle = Vector3.Angle (moonDirection, lookDirection);
							if (moonAngle <= 5.0f) {
								isMoonTransformed = gameObject.GetComponentInChildren<TransformMoon> ().transformMoon ();
								if (isMoonTransformed) {
									gameObject.tag = "InVincentVision";
								}
							}
						}

						//Transform Stars
						if (gameObject.name.Contains ("Star")) {
							Vector3 starDirection = gameObject.transform.position - transform.position;
							float starAngle = Vector3.Angle (starDirection, lookDirection);
							if (starAngle <= 10.0f) {
								isStarTransformed = gameObject.GetComponentInChildren<TransformStars>().transformStar();
								if (isStarTransformed) {
									gameObject.tag = "InVincentVision";
								}
								if(!hasSkyMonologueBeenPlayed) {
									/*Debug.Log ("I see. I see things differently. The sky appears calm and blue. But the stars! " +
									"Can you see how they roll their light and energy through the sky?");*/
									audioSource.clip = iSeeSkyClip;
									audioSource.Play ();
									hasSkyMonologueBeenPlayed = true;
								}
							}
						}
					}
				}

				if(hasSkyMonologueBeenPlayed && !audioSource.isPlaying) {
					shouldStartGraveSequence = true;
				}
			}
		}

		if (shouldStartGraveSequence && !shouldWakeUp) {
			/*if(waitBeforeGrave > 0) {
				waitBeforeGrave -= Time.deltaTime * 1.0f;
			} else {*/
				GameObject[] skyDreamObjects = GameObject.FindGameObjectsWithTag ("SkyDream");
				foreach (GameObject gameObject in skyDreamObjects) {
					gameObject.transform.Translate (0, Time.deltaTime * -15.0f, 0, Space.Self);
				}

				GameObject[] vincentVisionObjects = GameObject.FindGameObjectsWithTag ("InVincentVision");
				foreach(GameObject gameObject in vincentVisionObjects) {
					Destroy (gameObject);
				}

				foreach (GameObject gameObject in graveSequenceGameObjectsReference) {
					gameObject.SetActive (true);
				}

				float distanceBetweenPlayerAndSarcophagus = Vector3.Distance (thePlayer.transform.position, gravestone.transform.position);
				if (distanceBetweenPlayerAndSarcophagus <= 6.0f && !hasDepressionClipBeenPlayed) {
					/*Debug.Log ("(Sigh) I know its going to end! I know I can do nothing about my illness. " +
					"Suffering through it is exhausting me. I see no happy future at all!");*/
					audioSource.clip = illnessClip;
					audioSource.Play ();
					hasDepressionClipBeenPlayed = true;
					shouldWakeUp = true;
				}
			/*}*/
		}

		if (shouldWakeUp) {
			if(waitBeforeWake > 0) {
				waitBeforeWake -= Time.deltaTime * 1.0f;
			} else {
				foreach (GameObject gameObject in graveSequenceGameObjectsReference) {
					if(gameObject != null) {
						gameObject.transform.Translate (0, Time.deltaTime * -15.0f, 0, Space.Self);
					}
				}

				GameObject[] celestialObjects = GameObject.FindGameObjectsWithTag ("InVincentVision");
				foreach (GameObject gameObject in celestialObjects) {
					Destroy (gameObject);
				}

				foreach(KeyValuePair<string, GameObject> entry in starsInVincentVisionReference) {
					Destroy (entry.Value);
				}

				if(!isHallucinationAudioPlayed) {
					CardboardAudioSource hallucinationAudioSource = hallucinationAudio.GetComponent<CardboardAudioSource> ();
					hallucinationAudioSource.Play ();
					isHallucinationAudioPlayed = true;
				}

				if (blend <= 1.0f) {
					secondSkyboxMaterial.SetFloat ("_Blend", blend);
					RenderSettings.skybox = secondSkyboxMaterial;
				}
				blend += Time.deltaTime * 0.2f;

				if(blend >= 1.4f) {
					BedroomScene.isDreamComplete = true;
					SceneManager.LoadScene (1);
				}
			}
		}
	}
}
