using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarTrigger : MonoBehaviour
{
    [SerializeField] GameObject starToSpawn; //the star prefab to be spawned
    [SerializeField] int starPosIndex; //the position index for the star

    bool isTriggered; //this bool is used to make sure duplicate stars dont spawn;
    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return; //if the star is spawned already return
        isTriggered = true; //set is triggered to true
        GameObject spawned = Instantiate(starToSpawn, transform.position, transform.rotation); //spawn the star
        spawned.GetComponent<Star>().positionIndex = starPosIndex; //set its position index 
    }
}
