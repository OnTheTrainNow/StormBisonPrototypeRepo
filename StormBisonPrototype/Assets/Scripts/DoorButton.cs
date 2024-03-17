using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    [SerializeField] GameObject objectToDisable;

    MeshRenderer meshRendered;
    BoxCollider boxCollider;
    void Start()
    {
        meshRendered = objectToDisable.GetComponent<MeshRenderer>();
        boxCollider = objectToDisable.GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            meshRendered.enabled = false;
            boxCollider.enabled = false; 
        }
    }
}
