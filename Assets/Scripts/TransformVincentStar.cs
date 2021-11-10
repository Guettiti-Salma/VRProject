using UnityEngine;
using System.Collections;

public class TransformVincentStar : MonoBehaviour {
	private bool isTransformVincentStar = false;
	private Vector3 originalScale;

	// Use this for initialization
	void Start () {
		originalScale = this.transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if (isTransformVincentStar) {
			this.transform.localScale = Vector3.Lerp (this.transform.localScale, new Vector3 (originalScale.x + 5.0f, 
				originalScale.y + 5.0f, originalScale.z + 5.0f), 
				Time.deltaTime * 2.0f);
		}
	}

	public bool transformVincentStar() {
		isTransformVincentStar = true;
		return isTransformVincentStar;
	}
}
