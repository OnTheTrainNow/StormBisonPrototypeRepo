using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] Rigidbody rb; //since this is a projectile it needs a rigidbody

    [SerializeField] int speed; //the speed at which the projectile is fired
    [SerializeField] float yVelocity; //the vertical velocity of the projectile
    [SerializeField] int grenadeTimer; //how long before the projectile explodes

    [SerializeField] GameObject explosion; //the explosion trigger object that is instantiated by the grenade

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = (transform.forward + new Vector3(0, yVelocity, 0)) * speed; //set the grenades velocity based on the vertical velocity and speed of the projectile
        StartCoroutine(BlowUp()); //start the coroutine for blowing up
    }

    IEnumerator BlowUp()
    {
        yield return new WaitForSeconds(grenadeTimer); //wait for the grenades timer
        if (explosion) //if an explosion object exists
        {
            Instantiate(explosion, transform.position, transform.rotation); //instantiate it at the grenades positon
        }
        Destroy(gameObject); //destroy the grenade
    }
}
