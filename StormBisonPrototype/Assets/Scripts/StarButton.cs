using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarButton : MonoBehaviour
{
    [SerializeField] GameObject Star;
    [SerializeField] int positionIndex;
    [SerializeField] int StarID;
    [SerializeField] GameObject SpawnPositon;

    bool activated;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !activated)
        {
            activated = true;
            GameObject spawned = Instantiate(Star, SpawnPositon.transform.position, SpawnPositon.transform.rotation); 
            spawned.GetComponent<Star>().positionIndex = positionIndex;
            spawned.GetComponent<Star>().starArrayID = StarID;
        }
    }
}
