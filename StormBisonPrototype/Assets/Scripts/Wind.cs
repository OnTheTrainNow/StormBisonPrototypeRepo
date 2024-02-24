using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    [SerializeField] float windSpeed; //the strength of the wind
    [SerializeField] AudioSource windSFX;

    private void Start()
    {
        windSFX.enabled = false; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            windSFX.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            windSFX.enabled = false;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        IPushBack pushed = other.GetComponent<IPushBack>(); //try to get the IPushBack component of the object

        if (pushed != null ) //if the pushed object exists
        {
            pushed.PushBack(transform.forward * windSpeed * Time.deltaTime); //the direction of the force is forward, the windspeed is how strong the force is
        }
    }
}
