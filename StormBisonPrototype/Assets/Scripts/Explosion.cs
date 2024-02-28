using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] float duration; //the duration of the explosion
    [SerializeField] int damageAmount; //how much damage it deals
    [SerializeField] int knockBackStrength; //how strong the knockback from the grenade is
    [SerializeField] GameObject FX; //the FX object created after the explosion

    // Start is called before the first frame update
    IEnumerator Start()
    {
        if(FX != null) //if the FX exits than instantiate it (destroy is handled by the FX objects script)
        {
            Instantiate(FX, transform.position, transform.rotation);
        }
        yield return new WaitForSeconds(duration); //the explosion last for this duration before destroying itself
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) //ignore the other object if it is also a trigger (bullets and other explosions)
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>(); //try and get the IDamage component on the other object
        IPushBack pushed = other.GetComponent<IPushBack>(); //try and get the IPushBack component on the other object

        if (dmg != null) //if the damage component exists
        {
            dmg.TakeDamage(damageAmount); //call others take damage method
        }

        if (pushed != null) //if the pushback component exists
        {
            pushed.PushBack((other.transform.position - transform.position).normalized * knockBackStrength); //the direction of the pushback is the direction of the player from the grenade
            //the knockbackstrength is how strong the force is
        }
    }
}
