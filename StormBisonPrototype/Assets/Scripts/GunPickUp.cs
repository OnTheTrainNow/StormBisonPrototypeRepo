using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickUp : MonoBehaviour
{
    [SerializeField] GunStats gun;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.getGunstats(gun);
            Destroy(gameObject);
        }
    }
}
