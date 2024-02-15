using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeTeleport : MonoBehaviour
{
    [SerializeField] Transform teleportPosition; //set the transform positon of the exit pipe
    [SerializeField] Vector3 LaunchForce; //set the launch force of the exit pipe
    [SerializeField] ExitPipe exitPipeCover; //the exit pipe script for the corresponding exit pipes cover

    private void OnTriggerEnter(Collider other) //when the player enters the trigger volume
    {
        if (other.CompareTag("Player")) //check if the other object is the player
        {
            if (exitPipeCover != null) { exitPipeCover.disableCover(); } //if the exit pipe is set then call its disable method before teleporting
            gameManager.instance.player.transform.position = teleportPosition.position; //set their position to that of the exit position
            Physics.SyncTransforms(); //tell the physics systems to sync transforms in order for the teleport to work
            gameManager.instance.playerScript.PipeLaunch(LaunchForce); //call the pipe launch method on the player passing it the launch force        
        }

    }
}
