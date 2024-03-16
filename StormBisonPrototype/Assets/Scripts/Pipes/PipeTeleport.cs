using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeTeleport : MonoBehaviour
{
    [SerializeField] Transform teleportPosition; //set the transform positon of the exit pipe
    [SerializeField] Vector3 LaunchForce; //set the launch force of the exit pipe
    [SerializeField] ExitPipe exitPipeCover; //the exit pipe script for the corresponding exit pipes cover

    [SerializeField] bool isTwoWayPipe;

    [SerializeField] AudioSource enterSFX;

    private void OnTriggerEnter(Collider other) //when the player enters the trigger volume
    {
        if (other.CompareTag("Player")) //check if the other object is the player
        {
            if (enterSFX != null)
            {
                enterSFX.Play();
            }
            if (!isTwoWayPipe && exitPipeCover != null) { exitPipeCover.disableCover(); } //if the exit pipe is set then call its disable method before teleporting
            if (isTwoWayPipe) { exitPipeCover.disableTrigger(); } //if its a tool way pipe then the trigger version of disable pipecover gets called
            gameManager.instance.player.transform.position = teleportPosition.position; //set their position to that of the exit position
            Physics.SyncTransforms(); //tell the physics systems to sync transforms in order for the teleport to work
            gameManager.instance.playerScript.Launch(LaunchForce); //call the pipe launch method on the player passing it the launch force        
        }

    }
}
