using UnityEngine;
using System.Collections;

public class TransformMoon : MonoBehaviour {
	private bool isTransformMoon = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (isTransformMoon) {
			this.transform.localScale = Vector3.Lerp (this.transform.localScale, new Vector3 (40.0f, 40.0f, 40.0f), 
				Time.deltaTime * 2.0f);
		}
	}

	public bool transformMoon() {
		isTransformMoon = true;
		return isTransformMoon;
	}
}
