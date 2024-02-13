using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class ExitPipe : MonoBehaviour
{
    [SerializeField] bool isExitPipe;
    [SerializeField] MeshCollider pipeCover;

    // Update is called once per frame
    void Update()
    {
        if (!isExitPipe) { return; } //if its not an exit pipe than ignore the rest of the code
        if (gameManager.instance.player.transform.position.y > transform.position.y) //if the player is above the pipe
        {
            pipeCover.enabled = true; //turn the collider off
        }
        else //otherwise
        {
            pipeCover.enabled = false; //turn the collider off
        }
    }
}
