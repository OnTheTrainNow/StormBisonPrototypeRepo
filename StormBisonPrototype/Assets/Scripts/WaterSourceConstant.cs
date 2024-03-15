using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSourceConstant : MonoBehaviour
{
    float stopFillBackUp;

    void Start()
    {
        stopFillBackUp = 0;
    }

    private void Update() //the whole point of this update section is to turn off the constant fill bool if the player somehow exits the collider without actually triggering it
    {
        stopFillBackUp += Time.deltaTime; //increment the timer
        if (stopFillBackUp > 3f) //if the player doesnt stand in the fill area for more than 3 seconds then set the constant fill bool to false
        {
            gameManager.instance.playerScript.isConstantFill = false;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("Player")) //when the player enters this trigger set the constant fill bool to true
        {
            gameManager.instance.playerScript.isConstantFill = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        stopFillBackUp = 0; //reset the backup timer
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) //when the player exits this trigger set the contant fill bool to false
        {
            gameManager.instance.playerScript.isConstantFill = false;
        }
    }
}
