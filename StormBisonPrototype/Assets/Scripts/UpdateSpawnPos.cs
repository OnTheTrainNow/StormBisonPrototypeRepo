using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSpawnPos : MonoBehaviour
{
    [SerializeField] GameObject newSpawnPos;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameManager.instance.playerSpawnPosition = newSpawnPos;
        }
    }
}
