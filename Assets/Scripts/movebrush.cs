using UnityEngine;
using System.Collections;

public class movebrush : MonoBehaviour {
	public GameObject brushtable;
	public GameObject brushcam;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void moveb () {
		brushtable.SetActive (false);
		brushcam.SetActive (true);
	}
}
