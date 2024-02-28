using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boxSpawner : MonoBehaviour
{
    [SerializeField] GameObject ammoBox;
    [SerializeField] float respawnTime;
    [SerializeField] MeshRenderer MR;
    GameObject childBox;
    bool isRespawning;

    // Start is called before the first frame update
    void Start()
    {
        MR.enabled = false;
        childBox = Instantiate(ammoBox, transform.position, transform.rotation);
        childBox.transform.parent = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (childBox == null && !isRespawning) //if the childBox doesnt currently exists and a box hasnt started respawning
        {
            StartCoroutine(SpawnBox()); //start the coroutine to spawn a new one
        }
    }

    IEnumerator SpawnBox()
    {
        isRespawning = true; //set is respawning to true
        MR.enabled = true;
        yield return new WaitForSeconds(respawnTime); //wait for the respawn time before respawning
        childBox = Instantiate(ammoBox, transform.position, transform.rotation); //instantiate a new box
        childBox.transform.parent = transform; //set its parent to the spawner
        MR.enabled = false;
        isRespawning = false; //set is respawning to false
    }
}
