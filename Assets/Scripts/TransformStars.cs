using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransformStars : MonoBehaviour {
	private bool isTransformStar = false;
	public GameObject thePlayer;
	private float waitBeforeChangingStar = 1.0f;
	private bool isTransformVincentStar = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (isTransformStar) {
			this.transform.localScale = Vector3.Lerp (this.transform.localScale, new Vector3 (40.0f, 40.0f, 40.0f), 
				Time.deltaTime * 2.0f);
			waitBeforeChangingStar -= Time.deltaTime;
			if (waitBeforeChangingStar <= 0.7) {
				Dictionary<string, GameObject> starsInVincentVisionReference = thePlayer.GetComponent<DreamSequence> ().starsInVincentVisionReference;
				GameObject starInVincentVision = null;
				starsInVincentVisionReference.TryGetValue ("Galaxy_Small_" + gameObject.name, out starInVincentVision);
				if (starInVincentVision != null) {
					Debug.Log ("starInVincentVision :: " + starInVincentVision);
					starInVincentVision.SetActive (true);
					isTransformVincentStar = starInVincentVision.GetComponentInChildren<TransformVincentStar> ().transformVincentStar ();
					gameObject.SetActive (false);
				}
			}
		}
	}

	public bool transformStar() {
		isTransformStar = true;
		return isTransformStar;
	}
}
