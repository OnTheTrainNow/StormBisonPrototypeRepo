using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickUpAnimator : MonoBehaviour
{
    [SerializeField] float pickUpAnimSpeed;
    void Update()
    {
        pickUpAnim();
    }

    // Handles the rotation for the pick ups
    void pickUpAnim()
    {
        // handles rotation on y axis
        transform.Rotate(new Vector3(0, 30, 0) * pickUpAnimSpeed * Time.deltaTime);
    }
}
