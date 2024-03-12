using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthPickUps : MonoBehaviour
{
    public float healAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        { 
            gameManager.instance.playerScript.HealAmount(healAmount);
            Destroy(gameObject);
        }
    }
}
