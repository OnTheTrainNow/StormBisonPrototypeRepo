using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crouch : MonoBehaviour
{
    [SerializeField] CharacterController playerController;
    [SerializeField] public float crouchSpeed;
    [SerializeField] float normalHeight;
    [SerializeField] float crouchHeight;
    [SerializeField] Vector3 offset;
    [SerializeField] Transform player;
    public bool crouch;
    void Update()
    {
        Crouching();
    }

    public void Crouching()
    {
        if (Input.GetKeyDown(KeyCode.C)) //if the player pressed the crouch button 
        {
            crouch = !crouch;
        }
        if (crouch == true)
        {
            playerController.height = playerController.height - crouchSpeed * Time.deltaTime;
            if (playerController.height <= crouchHeight)
            {
                playerController.height = crouchHeight;
            }
        }
        if (crouch == false)
        {
            playerController.height = playerController.height + crouchSpeed * Time.deltaTime;
            if (playerController.height < normalHeight)
            {
                player.position = player.position + offset * Time.deltaTime;
            }
            if (playerController.height >= normalHeight)
            {
                playerController.height = normalHeight;
            }
        }
    }
}
