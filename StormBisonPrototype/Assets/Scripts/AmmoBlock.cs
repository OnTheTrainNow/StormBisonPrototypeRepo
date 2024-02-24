using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBlock : MonoBehaviour
{
    [SerializeField] TempFXObject blockBreakFX;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (blockBreakFX != null)
            {
                Instantiate(blockBreakFX, transform.position, transform.rotation);
            }

            Destroy(gameObject);
        }
    }
}
