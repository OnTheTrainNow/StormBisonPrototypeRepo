using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StarLockDoorTrigger : MonoBehaviour
{
    [SerializeField] BoxCollider doorCollider; //the collider for the door
    [SerializeField] MeshRenderer doorRenderer; //the renderer for the door
    [SerializeField] Canvas starDisplay;

    [SerializeField] TMP_Text starRequirementDisplay;

    [SerializeField] int starRequirement; //how many stars are needed to open the door

    // Start is called before the first frame update
    void Start()
    {
        starRequirementDisplay.text = starRequirement.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (starManager.instance.getStarCount() >= starRequirement)
            {
                doorCollider.enabled = false;
                doorRenderer.enabled = false;
                starDisplay.enabled = false;
            }
        }
    }
}
