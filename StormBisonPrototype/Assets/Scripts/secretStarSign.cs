using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class secretStarSign : MonoBehaviour, IDamage
{
    [SerializeField] GameObject starToSpawn; //the star prefab to be spawned
    [SerializeField] int starPosIndex; //the position index for the star

    bool isShot; //this bool is used to make sure duplicate stars dont spawn;
    public void TakeDamage(float damage)
    {
        if (isShot) return; //if the sign was already hit than return
        isShot = true; //set isShot to true 
        GameObject spawned = Instantiate(starToSpawn, transform.position, transform.rotation); //spawn the star
        spawned.GetComponent<Star>().positionIndex = starPosIndex; //set its position index 
    }
}
