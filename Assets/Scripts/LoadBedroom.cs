using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadBedroom : MonoBehaviour {
	public GameObject bedroomPainting;
	public LayerMask layerMask;
	public GameObject reticle;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hitInfo;
		if (Physics.Raycast (Cardboard.SDK.GetComponentInChildren<CardboardHead> ().Gaze, out hitInfo, Mathf.Infinity, layerMask)) {
			GameObject hitObject = hitInfo.transform.gameObject;
			if(hitObject!= null && hitObject.name.Contains ("bedroom")) {
				reticle.GetComponent<CardboardReticle> ().OnGazeStart (this.gameObject.GetComponent<Camera> (), hitObject, hitInfo.point);
			} else {
				reticle.GetComponent<CardboardReticle> ().OnGazeExit (this.gameObject.GetComponent<Camera> (), hitObject);
			}
		}

		if (Input.GetButtonDown ("Fire1")) {
			if (Physics.Raycast (Cardboard.SDK.GetComponentInChildren<CardboardHead> ().Gaze, out hitInfo, Mathf.Infinity, layerMask)) {
				GameObject hitObject = hitInfo.transform.gameObject;
				if (hitObject != null && hitObject.name.Contains ("bedroom")) {
					SceneManager.LoadScene (1);
				}
			}
		}
	}
}
