using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBlock : MonoBehaviour
{
    [SerializeField] TempFXObject blockBreakFX; //this is an FX object for when the block breaks
    [SerializeField] GameObject gunPickUp; //this is the gun pickup the box spits out when broken
    [SerializeField] Vector3 instantiationOffset; //this is an offset for where the gun is instantiated
    [SerializeField] float velocityTunerY; //this is a tuner variable for the guns initial Y velocity when the block spits it out 
    [SerializeField] float velocityTunerXZ; //the is the range at which a random XZ force is generated for velocity

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (blockBreakFX != null)
            {
                Instantiate(blockBreakFX, transform.position, transform.rotation);
            }

            GameObject pickUp = Instantiate(gunPickUp, transform.position + instantiationOffset, transform.rotation);
            pickUp.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(0, velocityTunerXZ), velocityTunerY, Random.Range(0, velocityTunerXZ));

            Destroy(gameObject);
        }
    }
}
