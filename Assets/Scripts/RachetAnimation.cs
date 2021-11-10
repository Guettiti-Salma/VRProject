using UnityEngine;
using System.Collections;

public class RachetAnimation : MonoBehaviour {

	// Use this for initialization
	public bool idle = false;
	public float timeToIdle = 2.0f; //2 seconds
	float currentTime = 0f;
	public GameObject myself;
	void Start () {
		//yield WaitForSeconds(2); 
		currentTime = Time.time + timeToIdle;

	}
	
	// Update is called once per frame
	void Update () {
		if(!idle) //this can be replaced by something to check if there is no input etc..
		{
			checkIdle();
		}
	}

	void checkIdle()
	{
		if(Time.time > currentTime)
		{
			idle = true;
			myself.GetComponent<Animator> ().enabled = true;
			//run your anim here or a seperate function to translate the boolean value
			currentTime = Time.time + timeToIdle;
		}
	}
}
