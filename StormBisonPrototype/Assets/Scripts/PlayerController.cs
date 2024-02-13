using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage 
{
    [SerializeField] CharacterController playerController; //the character controller for the player
    //a camera field may be added later 
    [SerializeField] int HP = 10; //the player health points
    [SerializeField] float movementSpeed = 5f; //the movement speed tuning variable for the player
    [SerializeField] float sprintSpeed = 10f; //the movement speed while sprinting
    [SerializeField] int maxJumps = 2; //the amount of times the player can jump
    [SerializeField] float jumpForce = 7f; //this controls how high a player can jump
    [SerializeField] float gravity = -9.8f; //gravity is used for determining verticle velocity (falling)

    
    [SerializeField] int shootDamage = 1; //how much damage is done by each shot
    [SerializeField] int shootRange = 20; //how far the player can shoot (this is the raycast range)
    [SerializeField] float firingRate = .2f; //the time between shots (determines how many times the player can fire in a specified time frame)

    Vector3 movement; //this vector handles movement along the X and Z axis (WASD, Up Down Left Right)
    Vector3 verticleVelocity; //this vector handles verticle velocity (jumping or falling)
    int currentJumps; //the current amount of jumps the player has made
    float currentSpeed; //the players current speed (switches between sprint speed and mvoement speed)
    bool isShooting; //this bool determines whether the player is currently shooting or not (you cant shoot again while this is true)
    bool isSpeedChangeable; //this bool determines if the speed can currently be changed or not (you cant change speed when jumping)

    //LaunchControls
    bool isLaunching;

    void Start()
    {
        playerController = GetComponent<CharacterController>(); //searches the current gameObject and returns the CharacterController
        currentSpeed = movementSpeed; //to avoid issues the default current speed is the same as movement when the program starts
    }

    
    void Update()
    {
        if (!gameManager.instance.isPaused) //if the gameManager is not set to paused 
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootRange, Color.blue); //show the rayCast for debug purposes

            ProcessMovement(); //process any current movement

            if (Input.GetButton("Shoot") && !isShooting) //check if the player is trying to shoot and if it is currently allowed (if they arent already shooting)
            {
                StartCoroutine(Shoot()); //start the shoot coroutine
            }
        }
    }

    private void ProcessMovement()
    {
        if (playerController.isGrounded) //if the player touches the ground
        {
            currentJumps = 0; //reset their current jumps
            verticleVelocity = Vector3.zero; //zero out their verticleVelocity so it doesnt build force while grounded
            isSpeedChangeable = true; //you can only change from normal speed to sprinting while on the ground
            isLaunching = false; //if you are grounded then you would be at the end of a launch
        }

        if (!isLaunching) //if the player is launching out of a pipe then ignore new inputs until they land or jump
        {
            //GetAxis returns the direction value along the given axis. transform.right and transform.forward return a Vector3 value for the given axis (X,0,0) and (0,0,Z)
            //When added together they give the final Vector which will represent movement on the X and Z axis (X, 0, Z)
            movement = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
            if (movement.magnitude > 1) { movement.Normalize(); } //diagonal movement returns a magnitude of 1.41 meaning the player moves faster than normal in that direction. If thats not intended then this line normalizes it to 1.
        }

        if (isSpeedChangeable) //if the speed is changeable
        {
            if (Input.GetKey(KeyCode.LeftShift)) //check if the player is sprinting
            {
                currentSpeed = sprintSpeed; //change the current speed to sprint speed
            }
            else
            {
                currentSpeed = movementSpeed; //otherwise keep at at movement speed
            }
        }

        playerController.Move(movement * currentSpeed * Time.deltaTime); //use the player controller to move the object based on the movement direction above multiplied by the speed (velocity). Time.deltaTime makes it frame rate independan

        if (Input.GetButtonDown("Jump") && currentJumps < maxJumps) //if the jump button is pressed and the current jumps coount isn't more than the max jumps
        {
            isSpeedChangeable = false; //you cant change speed when already in the air
            isLaunching = false; //jumping cancels out the launch
            verticleVelocity.y = jumpForce; //set the verticle velocity to the jump force (this makes the player go up)
            currentJumps++; //increment the current jump count
        }

        verticleVelocity.y += gravity * Time.deltaTime; //apply gravity to the verticle velocity and make sure its frame rate independant 
        playerController.Move(verticleVelocity * Time.deltaTime); //use the player controller to move the object based on vertical velocity
    }

    IEnumerator Shoot()
    {
        isShooting = true; //set is shooting to true which prevents another shot from being fired
        
        RaycastHit hit;
        //send a raycast out using the viewportPointToRay function of camera. (The Vector2 is the position, which is 0.5x0.5 for the center point)
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f,0.5f)), out hit, shootRange)) //The raycast is a bool which can output a Raycast hit. The shootRange is how far the ray shoots
        {

            Debug.Log(hit.collider.name); //out put the hit object for testing purposes. this can be removed later
            IDamage dmg = hit.collider.GetComponent<IDamage>(); //get the IDamage component from the hit collider

            if (dmg != null) //if the IDamage componenet was found
            {
                dmg.TakeDamage(shootDamage); //then call the takeDamage method for the hit object
            }
        }

        yield return new WaitForSeconds(firingRate); //halt the function for the firingRate duration

        isShooting = false; //set is shooting to false which means the player can shoot again
    }

    public void TakeDamage(int damageTaken) //this is the player implementation of the IDamage interface
    {
        HP -= damageTaken; //reduce the players current HP by the damage pass in
        if (HP <= 0) //if the players HP is less than or equal to zero
        {
            gameManager.instance.youLose(); //tell the game manager to display the Loss screen
        }
    }

    public void PipeLaunch(Vector3 LaunchMovement)
    {
        currentJumps = 0; //launching from a pipe resets the jump count
        movement += new Vector3(LaunchMovement.x, 0, LaunchMovement.z); //get the non vertical movement of the player from the launch force
        verticleVelocity = new Vector3(0, LaunchMovement.y, 0); //get the vertical movement from the launch force 
        isLaunching = true; //set is launching to true
    }
}
