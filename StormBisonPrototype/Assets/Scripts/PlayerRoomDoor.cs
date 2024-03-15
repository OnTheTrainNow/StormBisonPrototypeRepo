using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerRoomDoor : MonoBehaviour
{
    [SerializeField] BoxCollider doorCollider; //the collider for the door
    [SerializeField] MeshRenderer doorRenderer; //the renderer for the door
    [SerializeField] Canvas keyDisplay;
    [SerializeField] bool isBackTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (gameManager.instance.gotFinalKey)
            {
                doorCollider.enabled = false;
                doorRenderer.enabled = false;
                keyDisplay.enabled = false;
            }
            if (isBackTrigger) //if this is the back of the door then just open
            {
                doorCollider.enabled = false;
                doorRenderer.enabled = false;
                keyDisplay.enabled = false;
            }
        }
    }
}
