using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossStar : MonoBehaviour
{
    [SerializeField] GameObject bossStar; //this is the star object to be dropped
    [SerializeField] int positionIndex; //this is the position index for the instantiated star
    [SerializeField] int starID; //this is the ID to be passed to the star

    public void spawnStar()
    {
        GameObject spawned = Instantiate(bossStar, transform.position, transform.rotation); //instantiate the star
        spawned.GetComponent<Star>().positionIndex = positionIndex; //set its position index to the position index here
        spawned.GetComponent<Star>().starArrayID = starID; //set its ID 
    }
}
