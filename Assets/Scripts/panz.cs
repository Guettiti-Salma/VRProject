using UnityEngine;
using System.Collections;

public class panz : MonoBehaviour {

	public GameObject g0;
	public GameObject g1;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}
	void change () {
		g0.SetActive (false);
		g1.SetActive (true);
	}
}
