using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;


public class DoorGrabbable : OVRGrabbable
{
    public Transform handler;



    public override void GrabEnd(Vector3 linearvelocity, Vector3 angularvelocity)
    {
        base.GrabEnd(Vector3.zero, Vector3.zero);
        transform.position = handler.transform.position;
        transform.rotation = handler.transform.rotation;

        Rigidbody rbHandler = handler.GetComponent<Rigidbody>();
        rbHandler.velocity = Vector3.zero;
        rbHandler.angularVelocity = Vector3.zero;
    }
    private void update()
    {
         //float angleAfterDrag = transform.rotation.eulerAngles.z;
        //if (angleAfterDrag > -57.8f) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); 


        if (Vector3.Distance(handler.position,transform.position) > 0.04f)
        {
            grabbedBy.ForceRelease(this);
        }
    }

}
  
    
   

