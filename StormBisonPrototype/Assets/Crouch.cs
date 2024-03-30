using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crouch : MonoBehaviour
{
    [SerializeField] CharacterController playerController;
    [SerializeField] float crouchSpeed;
    [SerializeField] float normalHeight;
    [SerializeField] float crouchHeight;
    [SerializeField] Vector3 offset;
    [SerializeField] Transform player;
    bool crouching;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) //if the player pressed the crouch button 
        {
            crouching = !crouching;
        }
        if (crouching == true)
        {
            playerController.height = playerController.height - crouchSpeed * Time.deltaTime;
            if (playerController.height <= crouchHeight)
            {
                playerController.height = crouchHeight;
            }
        }
        if (crouching == false)
        {
            playerController.height = playerController.height + crouchSpeed * Time.deltaTime;
            if (playerController.height < normalHeight)
            {
                player.gameObject.SetActive(false);
                player.position = player.position + offset * Time.deltaTime;
                player.gameObject.SetActive(true);
            }
            if (playerController.height >= normalHeight)
            {
                playerController.height = normalHeight;
            }
        }
    }
}
