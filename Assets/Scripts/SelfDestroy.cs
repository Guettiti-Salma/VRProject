using UnityEngine;
using System.Collections;

public class SelfDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (this.transform.position.y <= -50.0f) {
			Destroy (this.gameObject);
		}
	}
}
