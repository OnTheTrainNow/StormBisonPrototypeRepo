using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPipe : MonoBehaviour
{
    [SerializeField] bool isExitPipe; //this bool toggles the functionality for testing
    [SerializeField] MeshCollider pipeCover; //the reference to the corresponding mesh collider for the cover
    [SerializeField] CapsuleCollider pipeTrigger; //the trigger volume of the exit pipe (for two way)
    [SerializeField] float disableTime; //how long the pipe gets disables

    [SerializeField] AudioSource exitSFX;

    public void disableCover() 
    {
        if (!isExitPipe) { return; } //ignore the script if exit pipe is turned off

        if (exitSFX != null)
        {
            exitSFX.Play();
        }
        StartCoroutine(disableTimer()); //start the disable timer coroutine
    }

    public void disableTrigger()
    {
        if (exitSFX != null)
        {
            exitSFX.Play();
        }
        StartCoroutine(disableTimerTrigger()); //start the disable timer coroutine
    }

    IEnumerator disableTimer() 
    {
        pipeCover.enabled = false; //disable the mesh collider
        yield return new WaitForSeconds(disableTime); //wait for the disable time
        pipeCover.enabled = true; //re enable it after the wait
    }

    IEnumerator disableTimerTrigger()
    {
        pipeTrigger.enabled = false; //disable the mesh collider
        yield return new WaitForSeconds(disableTime); //wait for the disable time
        pipeTrigger.enabled = true; //re enable it after the wait
    }
}
