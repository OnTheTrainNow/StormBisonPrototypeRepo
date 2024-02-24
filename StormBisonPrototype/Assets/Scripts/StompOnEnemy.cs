using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompOnEnemy : MonoBehaviour
{
    [SerializeField] TempFXObject stompedFX; //the FX object created when the player stomps on an enemy

    enemyAI parentScript; //get the reference to the enemy parent object
    private void Start()
    {
        parentScript = transform.parent.gameObject.GetComponent<enemyAI>(); //get the parent objects enemyAI component;
    }

    private void OnTriggerEnter(Collider other) //the trigger goes above the enemies head
    {
        if(other.CompareTag("Player")) //check if the player entered the trigger area
        {
            if (stompedFX != null)
            {
                Instantiate(stompedFX, transform.position, transform.rotation); //instantiate an FX object at the colliders position
            }
            parentScript.StompedOn(); //call the enemys StompedOn script to kill it
        }
    }
}
