using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSourceConstant : MonoBehaviour
{
    [SerializeField] float constantFillRate; //this is the rate the water fills at while in a constant water source
    float stopFillBackUp;
    bool isConstantFill;

    void Start()
    {
        stopFillBackUp = 0;
    }

    private void Update() //the whole point of this update section is to turn off the constant fill bool if the player somehow exits the collider without actually triggering it
    {
        stopFillBackUp += Time.deltaTime; //increment the timer
        if (stopFillBackUp > 3f) //if the player doesnt stand in the fill area for more than 3 seconds then set the constant fill bool to false
        {
            isConstantFill = false;
        }
    }

    private void FixedUpdate()
    {
        if (isConstantFill && gameManager.instance.playerScript.currentWater < 200)
        {
            gameManager.instance.playerScript.constFillTank(constantFillRate);
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("Player")) //when the player enters this trigger set the constant fill bool to true
        {
            isConstantFill = true;
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
            isConstantFill = false;
        }
    }
}
