using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class ExitPipe : MonoBehaviour
{
    [SerializeField] bool isExitPipe; //this bool toggles the functionality for testing
    [SerializeField] MeshCollider pipeCover; //the reference to the corresponding mesh collider for the cover
    [SerializeField] float disableTime; //how long the pipe gets disables

    public void disableCover() 
    {
        if (!isExitPipe) { return; } //ignore the script if exit pipe is turned off

        StartCoroutine(disableTimer()); //start the disable timer coroutine
    }

    IEnumerator disableTimer() 
    {
        pipeCover.enabled = false; //disable the mesh collider
        yield return new WaitForSeconds(disableTime); //wait for the disable time
        pipeCover.enabled = true; //re enable it after the wait
    }
}
