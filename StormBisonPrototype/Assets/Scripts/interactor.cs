using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class interactor : MonoBehaviour
{
    public LayerMask interactableLayerMask = 9;
    UnityEvent onInteract;
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 6, interactableLayerMask))
        {
            if (hit.collider.GetComponent<interactable>() != false)
            {
                onInteract = hit.collider.GetComponent<interactable>().onInteract;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    gameManager.instance.ShopUI();
                }
            }
        }
    }
}
