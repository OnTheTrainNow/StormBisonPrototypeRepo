using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterPickup : MonoBehaviour
{
    public int waterRefillAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.fillTank(waterRefillAmount);
            Destroy(gameObject);
        }
    }
}
