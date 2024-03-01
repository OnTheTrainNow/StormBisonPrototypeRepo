using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAttachment : MonoBehaviour //this script handles the player attachment for moving platforms
{
    //The platform will need a box collider trigger that covers the area on the platform where they player should be standing on this platform
    void OnTriggerEnter(Collider other) //when the player enters the platforms trigger area
    {
        //if (other.gameObject.CompareTag("GunModel")) { return; } //since the platform likes to steal the gunModel (ignore the colliding object if its the gun)
        other.transform.SetParent(transform); //set the players parent to the transform of the platform so they will move a long with it
    }

    private void OnTriggerExit(Collider other) //when the player exits the platforms trigger area (jumps or walks off) 
    {
        //if (other.gameObject.CompareTag("GunModel")) { return; } //since the platform likes to steal the gunModel (ignore the colliding object if its the gun)
        other.transform.SetParent(null); //set the players parent back to null 
        //other.transform.localScale = Vector3.one; //the platform slightly messes up the scale of the object (so set the scale back to 1,1,1)
    }
}
