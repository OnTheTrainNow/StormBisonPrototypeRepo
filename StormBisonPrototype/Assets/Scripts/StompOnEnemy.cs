using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompOnEnemy : MonoBehaviour
{
    enemyAI parentScript; //get the reference to the enemy parent object
    private void Start()
    {
        parentScript = transform.parent.gameObject.GetComponent<enemyAI>(); //get the parent objects enemyAI component;
    }

    private void OnTriggerEnter(Collider other) //the trigger goes above the enemies head
    {
        if(other.CompareTag("Player")) //check if the player entered the trigger area
        {
            parentScript.StompedOn(); //call the enemys StompedOn script to kill it
        }
    }
}
