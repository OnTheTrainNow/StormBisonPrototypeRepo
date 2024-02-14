using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeTeleport : MonoBehaviour
{
    [SerializeField] Transform teleportPosition; //set the transform positon of the exit pipe
    [SerializeField] Vector3 LaunchForce; //set the launch force of the exit pipe

    private void OnTriggerEnter(Collider other) //when the player enters the trigger volume
    {
        if (other.CompareTag("Player")) //check if the other object is the player
        {
            gameManager.instance.player.transform.position = teleportPosition.position; //set their position to that of the exit position
            Physics.SyncTransforms(); //tell the physics systems to sync transforms in order for the teleport to work
            PlayerController PC = gameManager.instance.player.GetComponent<PlayerController>(); //get the player script component
            if (PC != null) //this check shoudlnt really be needed but is here just in case
            {
                PC.PipeLaunch(LaunchForce); //call the pipe launch method on the player passing it the launch force
            }
        }

    }
}