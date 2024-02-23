using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    private void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }
    private void OnTriggerEnter(Collider other) 
    {
        IKillBox killTarget = other.GetComponent<IKillBox>(); //attempt to get the IKillBox component on the other object
        if (killTarget != null ) //if killTarget exists
        {
            killTarget.killBox(); //call their killBox method
        }
    }
}
