using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class enlarge : MonoBehaviour {
	public GameObject room;
	//public AudioSource audios;
	// Use this for initialization
	public bool trig = false;
	//public bool trig2 = false;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (transform.localPosition.z);

		if(!BedroomScene.isPaintingComplete) {
			if ((transform.localPosition.z > 3.6f) && (transform.localPosition.x < -0.8f) && trig == false) {
				trig = true;
				StartCoroutine (Goodasdasd ());
			}
			/*if ((transform.localPosition.z > 10f) && (transform.localPosition.x < -0.4f) && trig2 == true) {
				SceneManager.LoadSceneAsync (1);
			}*/
		}
	}

	IEnumerator Goodasdasd () {
		yield return new WaitForSeconds(1f);
		for (int i = 1; i < 80; i++) {
			yield return new WaitForSeconds(0.01f);
			room.transform.localScale = new Vector3 (room.transform.localScale.x*1.01f, room.transform.localScale.y, room.transform.localScale.z*1.01f);
			room.transform.localPosition = new Vector3 (room.transform.localPosition.x, room.transform.localPosition.y - 0.03f, room.transform.localPosition.z-0.01f);
		}

		//trig2 = true;
		yield return new WaitForSeconds(2.0f);
		SceneManager.LoadScene (1);
	
	}
}
