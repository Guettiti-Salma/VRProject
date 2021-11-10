using UnityEngine;
using System.Collections;

public class ray : MonoBehaviour {
	public GameObject brushtable;
	public GameObject brushcam;
	private GameObject objectInHand;
	private int canvcount = 0;
	// Use this for initialization
	public LayerMask layerMask;
	public bool isbrushtake = false;
	public GameObject p0;
	public GameObject p;
	public GameObject p1;
	public GameObject p2;
	public GameObject p3;
	public GameObject p4;
	public GameObject p5;
	private AudioSource audioSource;
	public AudioClip sleepClip;
	public AudioClip makeSketchClip;
	public AudioClip landBlueClip;
	public AudioClip cypressTreeClip;
	public AudioClip starsYellowClip;
	public AudioClip windSwirlingClip;
	public AudioClip pickBrushClip;

	void Start () {
		audioSource = gameObject.GetComponent<AudioSource> ();
		audioSource.volume = 0.5f;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire1")) {
			pickObject();
		}
	}

	IEnumerator movebru () {
		for (int i = 0; i < 10; i++) {
			brushcam.transform.Rotate (0.6f, 0, 0);
			yield return new WaitForSeconds(0.01f);
		}
		for (int i = 0; i < 20; i++) {
			brushcam.transform.Rotate (-0.6f, 0, 0);
			yield return new WaitForSeconds(0.01f);
		}
		for (int i = 0; i < 20; i++) {
			brushcam.transform.Rotate (0.6f, 0, 0);
			yield return new WaitForSeconds(0.01f);
		}
		for (int i = 0; i < 10; i++) {
			brushcam.transform.Rotate (-0.6f, 0, 0);
			yield return new WaitForSeconds(0.01f);
		}
	}

	void pickObject() {
		RaycastHit hitInfo;
		objectInHand = null;
		if (Physics.Raycast (Cardboard.SDK.GetComponentInChildren<CardboardHead> ().Gaze, out hitInfo, Mathf.Infinity, layerMask)) {
			//Debug.Log ("Hit something " + hitInfo.transform.name);
			if (hitInfo.transform.gameObject.name == "Brush 2") {
				if(!BedroomScene.isDreamComplete) {
					//Debug.Log ("I should sleep for sometime!");
					audioSource.clip = sleepClip;
					audioSource.Play ();
				} else {
					if (!isbrushtake) {
						brushtable.SetActive (false);
						brushcam.SetActive (true);
						isbrushtake = true;
						//Debug.Log ("First I'll make a sketch of the landscape.");
						audioSource.clip = makeSketchClip;
						audioSource.Play ();
					}
				}
			}
			if (hitInfo.transform.gameObject.tag == "canv") {
				if(!BedroomScene.isDreamComplete) {
					//Debug.Log ("I should sleep for sometime!");
					audioSource.clip = sleepClip;
					audioSource.Play ();
				} else {
					if (isbrushtake) {
						switch (canvcount) {
						case 0:
							StartCoroutine (movebru ());
							p0.SetActive (false);
							p.SetActive (true);
							//Debug.Log ("Next I should paint the land blue to emphasize the peaceful country night!");
							audioSource.clip = landBlueClip;
							audioSource.Play ();
							canvcount++;
							break;
						case 1:
							StartCoroutine (movebru ());
							p.SetActive (false);
							p1.SetActive (true);
							/*Debug.Log ("Here I'll add a Cypress tree to add depth and a reminder of death which transports " +
								"us from our world to the celestial light. ");*/
							audioSource.clip = cypressTreeClip;
							audioSource.Play ();
							canvcount++;
							break;
						case 2:
							StartCoroutine (movebru ());
							p1.SetActive (false);
							p2.SetActive (true);
							/*Debug.Log ("I paint the stars in bright yellow with the energy radiating all " +
								"around");*/
							audioSource.clip = starsYellowClip;
							audioSource.Play ();
							canvcount++;
							break;
						case 3:
							StartCoroutine (movebru ());
							p2.SetActive (false);
							p3.SetActive (true);
							canvcount++;
							break;
						case 4:
							StartCoroutine (movebru ());
							p3.SetActive (false);
							p4.SetActive (true);
							/*Debug.Log ("I should paint the wind swirling through the air burning, " +
								"bursting through into the solidity of the earth.");*/
							audioSource.clip = windSwirlingClip;
							audioSource.Play ();
							canvcount++;
							break;
						case 5:
							p4.SetActive (false);
							p5.SetActive (true);
							StartCoroutine (movebru ());
							canvcount++;
							break;
						case 6:
							brushtable.SetActive (true);
							brushcam.SetActive (false);
							canvcount++;
							BedroomScene.isPaintingComplete = true;
							break;
						default:
							break;
						}
					} else {
						//Debug.Log ("I should pick up the brush from the table!");
						audioSource.clip = pickBrushClip;
						audioSource.Play ();
					}
				}
			}
		} else {
			Debug.Log("Did not hit");
		}
	}
}