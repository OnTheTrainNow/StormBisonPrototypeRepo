using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeTeleport : MonoBehaviour
{
    [SerializeField] Transform teleportPosition; //set the transform positon of the exit pipe

    private void OnTriggerEnter(Collider other) //when the player enters the trigger volume
    {
        gameManager.instance.player.transform.position = teleportPosition.position; //set their position to that of the exit position
        Physics.SyncTransforms(); //tell the physics systems to sync transforms in order for the teleport to work
    }
}
