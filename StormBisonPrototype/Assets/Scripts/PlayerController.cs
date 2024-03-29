using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IDamage, IPushBack, IKillBox
{
    [Header("Components")]
    [SerializeField] CharacterController playerController; //the character controller for the player
    [SerializeField] CapsuleCollider playerCollider; //the characters main collider
    [SerializeField] GameObject playerDummy; //fix for enemy not having a target and erroring after player death
    //a camera field may be added later 

    [Header("Stats")]
    [SerializeField] public float HP = 10; //the player health points
    [SerializeField] float movementSpeed = 5f; //the movement speed tuning variable for the player
    [SerializeField] float sprintSpeed = 10f; //the movement speed while sprinting

    //Player Lives
    public int playerLives;

    [Header("Jumping & Gravity")]
    //jumping and gravity 
    [SerializeField] float jumpForce = 10f; //this controls how high a player can jump
    [SerializeField] float jump2Force = 15f; //this is how high the player jumps in combo jump 2
    [SerializeField] float jump3Force = 20f; //this is how high the player jumps in combo jump 3
    [SerializeField] float wallJumpForce = 10f; //this controls how high a player can jump from walls
    [SerializeField] float longJumpForce = 12f;
    [Range(0, 1)][SerializeField] float variableJumpPercentage = .75f;
    [SerializeField] float longJumpSpeedTuner = 2.5f;
    [SerializeField] float groundedJumpCutOffTime = .25f; //this is time time window from which grounded jumps can be made (the grounded timer is reset whenever the player is grounded)

    [SerializeField] float jetPackForce = 2f;
    [SerializeField] int jetPackUsage;
    [SerializeField] float jetPackSprintUsageScaler = 2f; //this is a scaler variable for how fast the jetpack drains while sprinting
    [SerializeField] float jetPackDisableLaunchTime = .5f; //how long the jetpack is disabled for after launching 
    [SerializeField] float jetPackDisableWallTime = .5f; //how long the jetpack is disabled for after  wall jumping

    //jump timers
    [SerializeField] float jump2Time; //how long the jump timer can last in between jump 1 and jump 2 (this is the time the player has to combo into the next jump)
    [SerializeField] float jump3Time; //how long the jump timer can last in between jump 2 and jump 3

    [SerializeField] float gravity = -9.8f; //gravity is used for determining verticle velocity (falling)
    [SerializeField] int pushBackResolve; //how fast/slow pushBack force is resolved on the player

    [Header("Crouching")]
    [SerializeField]float crouchControllerHeight; //the controller height for crouching
    [SerializeField]float crouchColliderHeight; //the collider height for crouching

    [SerializeField] float slideSpeedTuner = 2.5f;
    [SerializeField] float slideSpeedFallOff = 2f;

    [Header("Weapon Stats")]
    [SerializeField] List<GunStats> gunList = new List<GunStats>(); //this list represents the players weapon loadout
    [SerializeField] GameObject gunModel; //the physical game object that represents the players current gun
    [SerializeField] Image gunFireSprite; //the sprite for the firing animation

    [SerializeField] int shootDamage = 1; //how much damage is done by each shot
    [SerializeField] int shootRange = 20; //how far the player can shoot (this is the raycast range)
    [SerializeField] float firingRate = .2f; //the time between shots (determines how many times the player can fire in a specified time frame)

    // shotgun
    [SerializeField] float pelletDmg; // controls the damage each individual pellet does
    [SerializeField] int pellets; // this controls the number of pellets in each shot (the lower the amount the lower the damage, the higher the amount the higher the damage)
    [SerializeField] float pelletsSpreadAngle; // this sets the spread angle (smaller values = tighter spread, higher values = wider spread)

    //jetpack damage variables
    [SerializeField] int JetPackShootRange = 10;
    [SerializeField] float JetPackShootDamage = 1f;
    [SerializeField] float JetPackFiringRate = .2f;
    [SerializeField] ParticleSystem JetPackFX;

    [Header("Sound & Visual Effects")]
    [SerializeField] AudioSource jetPackSoundSource;
    [SerializeField] AudioSource characterSoundsSource; //this is the sound source for the player character (most player sounds shouldn't overlap)
    [SerializeField] AudioSource characterMovementSource; //this is the sound source for moving
    [SerializeField] List<AudioClip> jumpSounds = new List<AudioClip>(); //this list is the sound of each jump in the combo in order
    [Range(0, 1)][SerializeField] float jumpVolume;
    [SerializeField] List<AudioClip> hurtSounds = new List<AudioClip>(); //this list is the random collection of hurt sounds
    [Range(0, 1)][SerializeField] float hurtVolume;
    [SerializeField] List<AudioClip> stepSounds = new List<AudioClip>(); //this list is the random collection of hurt sounds
    [Range(0, 1)][SerializeField] float stepVolume;
    [SerializeField] AudioClip slideSound; //the sound that plays when the player slides
    [Range(0, 1)][SerializeField] float slideVolume;
    [SerializeField] AudioClip PickUpSound; //the sound that plays when the player slides
    [Range(0, 1)][SerializeField] float pickUpVolume;
    [SerializeField] float stepTimeNormal; //how long between step sounds
    [SerializeField] float stepTimeSprint; //how long between step soudns while sprinting
    [SerializeField] AudioSource pickUpSoundSource; //this is the sound source for when the player picks up guns
    [SerializeField] AudioSource gunSoundsSource; //this is the sound source for the gun (gun sounds can overlap with player sounds)
    [SerializeField] ParticleSystem jumpVFX; //this is the particle system attached to the player that creates dust clouds when they jump
    [SerializeField] GameObject bulletImpactFX; //this is the particle system for the players bullet impact (it works better if its not a child of the player object)

    ParticleSystem bulletParticleSystem;
    public int selectedGun = 0; //the indexer for the gunList (used by the player to select their active gun)
    //public List<int> currAmmo = new List<int>(); //the ammo for the current gun
    //public List<int> gunCosts = new List<int>();
    float gunVolume;

    Vector3 movement; //this vector handles movement along the X and Z axis (WASD, Up Down Left Right)
    Vector3 verticleVelocity; //this vector handles verticle velocity (jumping or falling)
    int currentJumps; //the current amount of jumps the player has made
    float currentSpeed; //the players current speed (switches between sprint speed and mvoement speed)
    bool isShooting; //this bool determines whether the player is currently shooting or not (you cant shoot again while this is true)
    bool isSpeedChangeable; //this bool determines if the speed can currently be changed or not (you cant change speed when jumping)

    //water tank
    [SerializeField] public float maxWater;
    [Range(0,1)][SerializeField] float startingWaterPercentage;
    public float currentWater;
    public int fillAmountMultiplier = 1;

    bool isJetPackShooting;
    bool isJetPacking; 

    // bools related to the weapon ui
    public bool isShotgunEquipped; // bool to help check if shotgun is equipped
    public bool isPistolEquipped;
    public bool isRifleEquipped;

    public float HPOriginal; //player starting HP
    bool isDead; //a bool that checks if the player is dead already when processing bullet hits

    //crouch/sprint values
    float defaultControllerHeight; //the regular controller height (these need to be set at start)
    float defaultColliderHeight; //the regular collider height
    bool isCrouched; //a bool to determine if the player is currently crouched or not
    bool isSprinting;
    bool isSliding;

    Quaternion slideDirection;

    //JumpControls
    float jumpTimer; //jump timer is a float that increases with time and is reset when the player jumps (this functionality will be used for the jump mechanic
    bool canJump; //a bool used to determine whether the player can currently jump
    bool isJumping;
    bool isWallJumping;
    bool isLongJumping;
    float groundedTimer; //since isGrounded is toggles itself on and off alot while grounded its too inconsistent for jumping (this timer will be used isntead)

    //LaunchControls
    Vector3 pushBack;
    bool isLaunching; //bool for if the player is launching
    float launchTimer;

    //soundcontrols
    bool isPlayingSteps;

    bool upgradeBool;

    void Start()
    {
        currentWater = (int)(maxWater * startingWaterPercentage); //current water is based on starting water percentage
        HPOriginal = HP; //set the original HP to the initial HP value set
        gunFireSprite.enabled = false; //turn the gun sprite off
        bulletParticleSystem = bulletImpactFX.GetComponent<ParticleSystem>();
        playerController = GetComponent<CharacterController>(); //searches the current gameObject and returns the CharacterController
        playerCollider = GetComponent<CapsuleCollider>(); //searches the curretn gameObject for its capsule collider
        defaultControllerHeight = playerController.height; //get the original controller height
        defaultColliderHeight = playerCollider.height; //get the orginal collider height
        currentSpeed = movementSpeed; //to avoid issues the default current speed is the same as movement when the program starts
        respawn();
        UpdateWaterUI(); //update the water UI
    }

    void Update()
    {
        gameManager.instance.updateWeaponEquipped();

        if (isJetPacking && !gameManager.instance.isPaused)
        {
            jetPackSoundSource.enabled = true;
        }
        else
        {
            jetPackSoundSource.enabled = false;
        }
        

        if (!gameManager.instance.isPaused) //if the gameManager is not set to paused 
        {
            //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootRange, Color.blue); //show the rayCast for debug purposes

            ProcessCrouch(); //process if the player is crouching (the player can only crouch/sprint when unpaused)
            ProcessSprint(); //process if the player is moving
            ProcessMovement(); //process any current movement

            if (gunList.Count > 0) //if the player has a gun
            {
                selectGun(); //check if they are changing guns

                if (Input.GetButton("Shoot") && !isShooting && currentWater >= gunList[selectedGun].gunUsage) //check if the player is trying to shoot and if it is currently allowed (if they arent already shooting)
                {
                    StartCoroutine(gunFireEffect());

                    if (isShotgunEquipped) // Will Check if the bool for isShotgunEquipped is true. If true then the shotgun coroutine is started otherwise it will resort to our default shoot
                    {
                        StartCoroutine(shotgunShoot()); // starts the shotgunShoot coroutine
                    }
                    else
                    {
                        StartCoroutine(Shoot()); //start the shoot coroutine
                    }
                }
            }
        }

        jumpTimer += Time.deltaTime; 
        launchTimer += Time.deltaTime;
        groundedTimer += Time.deltaTime;

        /*
        // Self Damage Tool
        if (Input.GetButtonDown("Self Damage Tool"))
        {
            TakeDamage(1.0f);
        }

        //refill test tool
        if (Input.GetButtonDown("Refill Test Tool"))
        {
            fillTank(7);
        }
        */

        if (upgradeBool == false)
        {
            upgradeBool = true;
            upgradeHandler();
        }
    }

    private void ProcessMovement()
    {
        pushBack = Vector3.Lerp(pushBack, Vector3.zero, Time.deltaTime * pushBackResolve);

        if (playerController.isGrounded) //if the player touches the ground
        {
            //currentJumps = 0; //reset their current jumps
            groundedTimer = 0;
            verticleVelocity = Vector3.zero; //zero out their verticleVelocity so it doesnt build force while grounded
            isSpeedChangeable = true; //you can only change from normal speed to sprinting while on the ground
            isLaunching = false; //if you are grounded then you would be at the end of a launch
            canJump = true; //isGrounded is kinda buggy to use on its own for jumping, so a can jump bool is set to true when the player touches the ground
            isJumping = false;
            isWallJumping = false;
            isJetPacking = false;
            isLongJumping = false;
        }

        if (!isLaunching && !isSliding && !isLongJumping) //if the player is launching out of a pipe then ignore new inputs until they land or jump
        {
            //GetAxis returns the direction value along the given axis. transform.right and transform.forward return a Vector3 value for the given axis (X,0,0) and (0,0,Z)
            //When added together they give the final Vector which will represent movement on the X and Z axis (X, 0, Z)
            movement = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
            if (movement.magnitude > 1) { movement.Normalize(); } //diagonal movement returns a magnitude of 1.41 meaning the player moves faster than normal in that direction. If thats not intended then this line normalizes it to 1.
        }

        if (isLongJumping) //if the player is longjumping then their movement gets handled differently
        {
            if (jumpTimer <= 0.5) //the player has to wait half a second after jumping before changing direction/speed
            {
                movement = transform.forward; //the player has to move forward when the long jump starts
            }
            else
            {
                float direction = Input.GetAxis("Vertical"); //get the direction of players vertical input
                if (direction < 0) { currentSpeed = sprintSpeed; } //if its below 0 (backwards) then reduce the long jumps speed (im using sprint since its slower)
                movement = direction * transform.forward; //apply the direction to the movement
            }
            if (movement.magnitude > 1) { movement.Normalize(); } //normalize the movement
            
        }

        playerController.Move(movement * currentSpeed * Time.deltaTime); //use the player controller to move the object based on the movement direction above multiplied by the speed (velocity). Time.deltaTime makes it frame rate independan

        ProcessSlide();

        if (!isSliding) //if the player is sliding than long jumping will be handled by process slide
        {
            //this is the jump combo system
            if (groundedTimer <= groundedJumpCutOffTime && canJump && currentJumps == 1 && jumpTimer <= jump2Time && Input.GetButtonDown("Jump")) //if the player can jump, presses jump, is on their second jump, and the jump timer is less then jump 2 combo allowed time
            {
                PlayJumpSound(1); //play the second sound in the list
                ProcessJump(jump2Force); //process a jump with the jump 2 force
            }
            else if (groundedTimer <= groundedJumpCutOffTime && canJump && currentJumps == 2 && jumpTimer <= jump3Time && (SceneManager.GetActiveScene().name != ("MainLobby")) && Input.GetButtonDown("Jump")) //if the player can jump, presses jump, is on their third jump, and the jump timer is less then jump 3 combo allowed time
            {
                PlayJumpSound(2); //play the third sound in the list
                ProcessJump(jump3Force); //process a jump with the jump 3 force
            }
            else if (groundedTimer <= groundedJumpCutOffTime && canJump && Input.GetButtonDown("Jump")) //this is the default jump for when the player doesnt reach any of the above conditions
            {
                PlayJumpSound(0); //play the first sound in the list
                currentJumps = 0; //this jump starts the combo, but is not guaranteed to be zero when the player jumps so reset it to zero
                ProcessJump(jumpForce); //process a jump with regular jump force
            }

            //this if statement is the logic for variable jump
            if (isJumping && Input.GetButtonUp("Jump") && verticleVelocity.y > 0f) //if the player has upward velocity and is jumping and lets off the jump button
            {
                verticleVelocity.y *= variableJumpPercentage;
            }
        }

        ProcessJetpack();

        verticleVelocity.y += gravity * Time.deltaTime; //apply gravity to the verticle velocity and make sure its frame rate independant 
        playerController.Move((verticleVelocity + pushBack) * Time.deltaTime); //use the player controller to move the object based on vertical velocity

        if (playerController.isGrounded && movement.normalized.magnitude > .3f && !isPlayingSteps)
        {
            StartCoroutine(PlayMoveSound());
        }
    }

    private void PlayJumpSound(int index)
    {
        if (index >= 0 && index < jumpSounds.Count) //make sure the index is valid
        {
            characterSoundsSource.Stop(); //stop the current sound
            characterSoundsSource.PlayOneShot(jumpSounds[index], jumpVolume); //play the jump sound
        }
    }

    IEnumerator PlayMoveSound()
    {
        isPlayingSteps = true; //set playing steps to true
        if (!isSliding)
        {
            characterMovementSource.PlayOneShot(stepSounds[UnityEngine.Random.Range(0, stepSounds.Count)], stepVolume); //play a random step sound
        }
        if (!isSprinting)
        {
            yield return new WaitForSeconds(stepTimeNormal);
        }
        else
        {
            yield return new WaitForSeconds(stepTimeSprint);
        }
        isPlayingSteps = false;
    }

    private void ProcessCrouch()
    {
        if (isSliding || isSprinting || isJumping || isJetPacking) { return; }
        if (Input.GetButtonDown("Crouch")) //if the player pressed the crouch button 
        {
            if (!isCrouched) //if they arent crouched
            {
                isCrouched = true; //set crouched bool to true
                playerController.height = crouchControllerHeight; //reduce the controller and collider height
                playerCollider.height = crouchColliderHeight;
            }
            else if (isCrouched) //if they are crouching already
            {
                isCrouched = false; //set crouched bool to false
                playerController.height = defaultControllerHeight; //set the controller and collider heights back to default
                playerCollider.height = defaultColliderHeight;
            }
        }
    }

    private void ProcessSprint()
    {
        if (isCrouched) { return; }
        if (isSpeedChangeable) //if the speed is changeable
        {
            if (Input.GetKey(KeyCode.LeftShift)) //check if the player is sprinting
            {
                isSprinting = true;
                currentSpeed = sprintSpeed; //change the current speed to sprint speed
            }
            else
            {
                isSprinting = false;
                currentSpeed = movementSpeed; //otherwise keep at at movement speed
            }
        }
    }

    private void ProcessSlide()
    {
        if (isJumping || isWallJumping || isJetPacking || isLaunching) { return; } //the player can't slide in the follow conditons

         float direction = Input.GetAxis("Vertical"); //get the direction of players vertical input (this will be used to make sure the player is moving forward)

        if (isSprinting && direction > 0 && Input.GetButtonDown("Crouch")) //this activates the slide (sprinting forward and pressing crouch)
        {
            isSliding = true; //set the sliding state to true
            characterMovementSource.PlayOneShot(slideSound, slideVolume); //play the slide sfx

            slideDirection = Quaternion.LookRotation(playerController.transform.forward, Vector3.up);  //store the players current forward direction

            isSprinting = false; //the player is no longer considered sprinting       
            currentSpeed = sprintSpeed * slideSpeedTuner; //set the current speed to faster than sprint (it will drop quickly)
            isCrouched = true; //set crouched bool to true
            playerController.height = crouchControllerHeight; //reduce the controller and collider height
            playerCollider.height = crouchColliderHeight; //reduce the collider height
        }
        else if (isSliding && Input.GetButtonDown("Crouch")) //this cancels the slide early
        {
            characterMovementSource.Stop();
            isCrouched = false; //set crouched bool to false
            isSliding = false; //uncrouching stops sliding
            playerController.height = defaultControllerHeight; //set the controller and collider heights back to default
            playerCollider.height = defaultColliderHeight; //reset the collider height
            currentSpeed = movementSpeed; //set the current speed back to default
        }
        if (isSliding) //this handles lerp the slide speed back to near zero
        {
            playerController.transform.rotation = slideDirection; //set the players rotation to the slide rotation while sliding
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime * slideSpeedFallOff); //lerp the speed down to zero

            if (groundedTimer <= groundedJumpCutOffTime && Input.GetButtonDown("Jump"))
            {
                ProcessLongJump();
            }
            if (currentSpeed <= 0.5f) //if the current speed drops low enough (its hard to check zero itself) 
            { 
                isSliding = false; //the player is no longer sliding (this leaves them in crouch state without sliding restrictions on speed and rotation)
                currentSpeed = movementSpeed; //reset their speed to default
            }
        }
    }

    private void ProcessJump(float jumpforce)
    {
        jumpVFX.Play(); //play the jump particle effect
        isSliding = false;
        isJumping = true;
        canJump = false;
        isSpeedChangeable = false; //you cant change speed when already in the air
        isLaunching = false; //jumping cancels out the launch
        verticleVelocity.y = jumpforce; //set the verticle velocity to the jump force (this makes the player go up)
        currentJumps++; //increment the current jump count
        jumpTimer = 0; //reset the jump timer if the player is grounded
    }

    private void ProcessLongJump()
    {
        PlayJumpSound(2);
        jumpVFX.Play(); //play the jump particle effect
        isJumping = true;
        isLongJumping = true;
        canJump = false;
        isSliding = false;

        isCrouched = false;
        playerController.height = defaultControllerHeight; //set the controller and collider heights back to default
        playerCollider.height = defaultColliderHeight; //reset the collider height

        currentSpeed = sprintSpeed * longJumpSpeedTuner;

        isSpeedChangeable = false; //you cant change speed when already in the air
        verticleVelocity.y = longJumpForce; //set the verticle velocity to the jump force (this makes the player go up)
        currentJumps++; //increment the current jump count
        jumpTimer = 0; //reset the jump timer if the player is grounded
    }

    private void ProcessJetpack()
    {
        if (launchTimer <= jetPackDisableLaunchTime) { return; } //if the launch timer is too low than don't give access to the jetpack
        if (isWallJumping && jumpTimer <= jetPackDisableWallTime) { return; } //if the player is wall jumping and the jump timer is low
        
        if(Input.GetButton("Jetpack M2") && currentWater > 0) //if the player holds the jetpack button and has water (being grounded is in debate as a requirment)
        {
            if (isSliding) //if the player is sliding when they use the jetpack
            {
                characterMovementSource.Stop();
                isSliding = false; //disable sliding
                isCrouched = false; //disable crouching
                playerController.height = defaultControllerHeight; //set the controller and collider heights back to default
                playerCollider.height = defaultColliderHeight; //reset the collider height
            }
            isJetPacking = true;
            isSpeedChangeable = true; //allow them to control sprint speed
            isJumping = false; //they are no longer considered jumping after using the jetpack
            isLongJumping = false;
            isLaunching = false; //they are no longer considered launchign after using the jetpack
            verticleVelocity.y = jetPackForce; //set the y velocity to jetpack force each frame they hold
            if (isSprinting) //if the player is sprinting while using the jetpack
            {
                currentWater -= (int)(jetPackUsage) * jetPackSprintUsageScaler * Time.deltaTime; //the water drains faster based on the usage scaler
            }
            else //otherwise
            {
                currentWater -= (int)(jetPackUsage) * Time.deltaTime; //drain the water based on regular usage amount
            }

            JetPackFX.gameObject.SetActive(true);
            if (!isJetPackShooting) { StartCoroutine(JetPackShoot()); }
            if (currentWater < 0) { currentWater = 0; } //if the water drops below 0 during this process set it back to 0
            UpdateWaterUI(); //update the water UI
        }
        else
        {
            JetPackFX.gameObject.SetActive(false);
            isJetPacking = false;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit other)
    {
        if (other.gameObject.CompareTag("Enemy")) { return; } //if the hit object is an enemy than ignore it

        if (!playerController.isGrounded && (other.normal.y < .1f && other.normal.y >= 0)) //when the player collides, isn't grounded, and the other objects normal is near vertical
        {
            if (Input.GetButtonDown("Jump") || Input.GetButtonUp("Jump")) //the timing is kinda hard so wall jumping checks for either button up or down
            {
                //Debug.DrawRay(playerController.transform.position, playerController.transform.forward * 1f, Color.yellow, 1f); //debug ray 
                if (!Physics.Raycast(playerController.transform.position, playerController.transform.forward, 1f)) { return; } //use a forward facing raycast to determine if the player is facing the wall enough
                
                Vector3 direction = Vector3.Reflect(playerController.transform.forward, other.normal); //get their reflected direction off the normal
                //Debug.DrawRay(other.point, direction, Color.red, 2f); //this draws the direction for debugging
                playerController.transform.rotation = Quaternion.LookRotation(direction, Vector3.up); //set the players rotation to that of the direction
                movement = direction; //change the players movement direction to match the direction
                ProcessWallJump();
            }
        }
    }
    private void ProcessWallJump()
    {
        if (isSliding) //if the player is sliding when wall jump (this isn't common but will bug out noticably if this isnt checked)
        {
            characterMovementSource.Stop();
            isSliding = false; //disable sliding
            isCrouched = false; //disable crouching
            playerController.height = defaultControllerHeight; //set the controller and collider heights back to default
            playerCollider.height = defaultColliderHeight; //reset the collider height
        }
        jumpVFX.Play(); //play the jump particle effect
        PlayJumpSound(1);
        isWallJumping = true;
        isJumping = true;
        isLongJumping = false;
        canJump = false;
        isSpeedChangeable = false; //you cant change speed when already in the air
        verticleVelocity.y = wallJumpForce; //set the verticle velocity to the jump force (this makes the player go up)
        jumpTimer = 0; //reset the jump timer (you can continue jump combos from wall jumps)
    }

    //Water Tank
    public void fillTank(int fillAmount) //this version of filltank is for collectable water sources
    {
        if (currentWater < maxWater) //if the current water is less than max water than follow fill logic
        {
            if ((currentWater + fillAmount) >= maxWater) //if curr water and fill amount combined are greater than or equal to tank capacity 
            {
                currentWater = maxWater; //fill to max
            }
            else
            {
                currentWater += fillAmount; //otherwise the fill amount is low enough to just add it 
            }
        }
        if (currentWater > maxWater) //if current water somehow manages to go over tank capacity
        {
            currentWater = maxWater; //set it equal to max water
        }
        UpdateWaterUI(); //update the water UI
    }

    public void constFillTank(float fillAmount) //this version of filltank is used for the constant source
    {
        if (isJetPacking) { return; } //disable filling while jetpacking 
        if (currentWater < maxWater) //if the current water is less than max water than follow fill logic
        {
            if ((currentWater + fillAmount) >= maxWater) //if curr water and fill amount combined are greater than or equal to tank capacity 
            {
                currentWater = maxWater; //fill to max
            }
            else
            {
                currentWater += (fillAmount * fillAmountMultiplier); //otherwise the fill amount is low enough to just add it 
            }
        }
        if (currentWater > maxWater) //if current water somehow manages to go over tank capacity
        {
            currentWater = maxWater; //set it equal to max water
        }
        UpdateWaterUI(); //update the water UI
    }

    //shooting
    IEnumerator Shoot()
    {
        isShooting = true; //set is shooting to true which prevents another shot from being fired

        gunSoundsSource.Stop(); //stop the current sound
        gunSoundsSource.PlayOneShot(gunSoundsSource.clip, gunVolume); //play the current gun sound

        //currAmmo[selectedGun]--; //reduce the selected guns ammo by 1
        currentWater -= gunList[selectedGun].gunUsage;
        if (currentWater < 0) { currentWater = 0; }
        UpdateWaterUI(); //update the water UI


        RaycastHit hit;
        //send a raycast out using the viewportPointToRay function of camera. (The Vector2 is the position, which is 0.5x0.5 for the center point)
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f,0.5f)), out hit, shootRange)) //The raycast is a bool which can output a Raycast hit. The shootRange is how far the ray shoots
        {
            bulletImpact(hit); //tell the bullet impact effect to move to the hit location

            //Debug.Log(hit.collider.name); //out put the hit object for testing purposes. this can be removed later
            IDamage dmg = hit.collider.GetComponent<IDamage>(); //get the IDamage component from the hit collider

            if (hit.transform != transform && dmg != null) //if the IDamage componenet was found
            {
                dmg.TakeDamage(shootDamage); //then call the takeDamage method for the hit object
            }
        }

        yield return new WaitForSeconds(firingRate); //halt the function for the firingRate duration

        isShooting = false; //set is shooting to false which means the player can shoot again
    }

    IEnumerator shotgunShoot()
    {
        isShooting = true;

        gunSoundsSource.Stop(); //stop the current sound
        gunSoundsSource.PlayOneShot(gunSoundsSource.clip, gunVolume); //play the curren gun sound

        //currAmmo[selectedGun]--; //reduce the selected guns ammo by 1
        currentWater -= gunList[selectedGun].gunUsage;
        if (currentWater < 0) { currentWater = 0; }
        UpdateWaterUI(); //update the water UI

        // This is so that the pelletRays have the same ray origin as Shoot()'s rays
        Ray pelletRay = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f));

        for (int i = 0; i < pellets; i++)
        {
            // This is to calculate the spread direction within a cone
            Vector3 spread = pelletRay.direction + UnityEngine.Random.insideUnitSphere * pelletsSpreadAngle;
            spread.Normalize();

            RaycastHit hit;
            if (Physics.Raycast(pelletRay.origin, spread, out hit, shootRange))
            {
                bulletImpact(hit); //tell the bullet impact effect to move to the hit location

                //Debug.Log(hit.collider.name);

                IDamage dmg = hit.collider.GetComponent<IDamage>();

                if (hit.transform != transform && dmg != null)
                {
                    dmg.TakeDamage(pelletDmg);
                }

                // This is for Debugging purposes
                //Debug.DrawLine(pelletRay.origin, hit.point, Color.red, 0.5f);
            }/*
            else
            {
                // This is for Debuggin purposes
                Debug.DrawLine(pelletRay.origin, pelletRay.origin + spread * shootRange, Color.red, 0.5f);
            }*/
        }

        yield return new WaitForSeconds(firingRate);
        isShooting = false;
    }

    IEnumerator JetPackShoot()
    {
        isJetPackShooting = true; //set is shooting to true which prevents another shot from being fired

        /* //this will be changed for the jetpack later
        gunSoundsSource.Stop(); //stop the current sound
        gunSoundsSource.PlayOneShot(gunSoundsSource.clip, gunVolume); //play the current gun sound
        */

        RaycastHit hit;
        //send a raycast out below the player
        //Debug.DrawRay(playerController.transform.position, Vector3.down * 10, Color.yellow, 1f);
        if (Physics.Raycast(playerController.transform.position, Vector3.down , out hit, JetPackShootRange)) //The raycast is a bool which can output a Raycast hit. The shootRange is how far the ray shoots
        {
            //bulletImpact(hit); //tell the bullet impact effect to move to the hit location

            //Debug.Log(hit.collider.name); //out put the hit object for testing purposes. this can be removed later
            IDamage dmg = hit.collider.GetComponent<IDamage>(); //get the IDamage component from the hit collider

            if (hit.transform != transform && dmg != null) //if the IDamage componenet was found
            {
                dmg.TakeDamage(JetPackShootDamage); //then call the takeDamage method for the hit object
            }
        }

        yield return new WaitForSeconds(JetPackFiringRate); //halt the function for the firingRate duration

        isJetPackShooting = false; //set is shooting to false which means the player can shoot again
    }

    IEnumerator gunFireEffect()
    {
        gunFireSprite.enabled = true; //enable the sprite
        gunFireSprite.transform.localRotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0,360)); //randomly rotate the sprite on the z axis 
        yield return new WaitForSeconds(0.1f); //wait for a split second
        gunFireSprite.enabled = false; //disable the sprite again
    }

    void bulletImpact(RaycastHit hit)
    {
        if (bulletImpactFX != null)
        {
            bulletImpactFX.transform.position = hit.point; //move to the hit location
            bulletImpactFX.transform.LookAt(transform.position); //rotate the bullet impact to face the player
            bulletParticleSystem.Play(); //play the particle effect (there are 3 different ones)
        }
    }

    public void getGunstats(GunStats gun) //gets a gun to add to the list
    {
        if (gunList.Count < 2)
        {
            bool newGun = false; //by default treat the gun like its not new
            pickUpSoundSource.PlayOneShot(PickUpSound, pickUpVolume); //play the pickup sound

            if (!gunList.Contains(gun)) //if the gun isnt in the list
            {
                gunList.Add(gun); //add the passed in gun to the list                                
  
                newGun = true; //set the newGun bool to true
            }

            shootDamage = gun.shootDamage; //set the current gun values to match the new gun
            shootRange = gun.shootRange;
            firingRate = gun.firingRate;
            pelletDmg = gun.pelletDmg;
            pelletsSpreadAngle = gun.pelletsSpreadAngle;
            pellets = gun.pellets;
            isShotgunEquipped = gun.isShotgunEquipped;
            isPistolEquipped = gun.isPistolEquipped;
            isRifleEquipped = gun.isRifleEquipped;

            gunSoundsSource.clip = gun.shootSFX;
            gunVolume = gun.shootSoundVol;

            bulletParticleSystem = bulletImpactFX.transform.GetChild(gun.hitEffectIndex).GetComponent<ParticleSystem>(); //get the child particle system at the index


            gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh; //make the mesh and material on the players gunModel match the new gun
            gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;

            if (newGun) //if the was new 
            {
                selectedGun = gunList.Count - 1; //the new gun is appended to the end of the list so set the selected gun to match that
            }
            else //if the gun is already in the list
            {
                selectedGun = gunList.IndexOf(gun); //get the index of the gun and change the selected gun to match it   
            }

            //currAmmo[selectedGun] = gunList[selectedGun].maxAmmo; //set the guns current ammo to max
        }
        if (gunList.Count == 2)
        {
            bool newGun = false;
            pickUpSoundSource.PlayOneShot(PickUpSound, pickUpVolume);

            if (!gunList.Contains(gun))
            {
                gunList.Add(gun);

                newGun = true;
            }

            if (newGun) //if the gun is new then it will replace the current weapon being held
            {
                int currentIndex = selectedGun;
                gunList[currentIndex] = gun;
                changeGun();
                gunList.Remove(gunList[index:2]);
            }
            else
            {
                //if the gun is already in the list it will not add the same gun instead it will just equip your weapon
                //(if your holding a rifle and have a shotgun as secondary and pick up another shotgun you will automatically switch to the shotgun)
                selectedGun = gunList.IndexOf(gun);
                changeGun();
            }
        }
    }

    void selectGun()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0) //if the player scrolls up
        {
            if (selectedGun == gunList.Count - 1) //if they are at the end of the list already
            {
                selectedGun = 0; //wrap around to the beginning of the list
            }
            else //otherwise
            {
                selectedGun++; //increment the selected gun indexer
            }
            changeGun(); //change the gun to the selected gun
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) //if the player scrolls down
        {
            if (selectedGun == 0) //if they are at the beginning of the list
            {
                selectedGun = gunList.Count - 1; //wrap around to the end of the list
            }
            else //otherwise
            {
                selectedGun--; //decrement the selected gun indexer
            }
            changeGun(); //change the gun to the selected gun
        }
    }

    private void changeGun()
    {
        shootDamage = gunList[selectedGun].shootDamage; //change the current gun stats to that of the selectedGun
        shootRange = gunList[selectedGun].shootRange;
        firingRate = gunList[selectedGun].firingRate;
        pelletDmg = gunList[selectedGun].pelletDmg;
        pelletsSpreadAngle = gunList[selectedGun].pelletsSpreadAngle;
        pellets = gunList[selectedGun].pellets;
        isShotgunEquipped = gunList[selectedGun].isShotgunEquipped;
        isPistolEquipped = gunList[selectedGun].isPistolEquipped;
        isRifleEquipped = gunList[selectedGun].isRifleEquipped;

        gunSoundsSource.clip = gunList[selectedGun].shootSFX;
        gunVolume = gunList[selectedGun].shootSoundVol;

        bulletParticleSystem = bulletImpactFX.transform.GetChild(gunList[selectedGun].hitEffectIndex).GetComponent<ParticleSystem>(); //get the child particle system at the index

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh; //change the mesh and materials
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    //damage and UI handling
    public void TakeDamage(float damageTaken) //this is the player implementation of the IDamage interface
    {
        HP -= damageTaken; //reduce the players current HP by the damage pass in

        UpdatePlayerUI(); //update the player UI
        PlayHurtSound(); //play the hurt sound
        StartCoroutine(flashDamage()); //flash the damage effect panel with a coroutine

        if (HP <= 0 && !isDead) //if the players HP is less than or equal to zero (and if they arent already dead to prevent double triggers)
        {
            playerLives -= 1;
            isDead = true; //set the player to dead
            if (playerLives <= 0 && isDead == true)
            {
                Instantiate(playerDummy,transform.position, Quaternion.identity); //fix for enemy not having a target and erroring after player death
                gameManager.instance.youLose();
                //Debug.Log("Oops No More Lives For You");
                
            }
            else
            {
                Instantiate(playerDummy, transform.position, Quaternion.identity); //fix for enemy not having a target and erroring after player death
                gameManager.instance.youDied(); //tell the game manager to display the death screen
            }
        }
    }

    public void HealAmount(float healAmount) // this Method handles the healing for the player.
    {
        // make sure that when you mess with the blue coin(MAX HP HEAL) healAmount it is always greater than the players max Hp, so if the max hp is 10 than do 11 or higher on heal amount.
        // i have the blue coin(MAX HP HEAL) at 100 since i dont remember if we wanted to have health Upgrades
        HP += healAmount;
        HP = Mathf.Min(HP, HPOriginal); // this is so that the HP healed never goes over our maximum health, so if you are at 6/10 hp a coin that heals 5 will only heal 4 HP
        UpdatePlayerUI();
    }

    public void killBox() //this method is called when the player falls off the world into the kill box
    {
        UpdatePlayerUI(); //update the player UI
        PlayHurtSound(); //play the hurt sound
        StartCoroutine(flashDamage()); //flash the damage effect panel with a coroutine

        if (!isDead) //if the isDead bool is not set
        {
            isDead = true; //set the player to dead
            playerLives -= 1;

            if (playerLives <= 0 && isDead == true)
            {
                Instantiate(playerDummy, transform.position, Quaternion.identity); //fix for enemy not having a target and erroring after player death
                gameManager.instance.youLose();
                //Debug.Log("Oops No More Lives For You");

            }
            else
            {
                Instantiate(playerDummy, transform.position, Quaternion.identity); //fix for enemy not having a target and erroring after player death
                gameManager.instance.youDied(); //tell the game manager to display the death screen
            }
        }
    }

    private void PlayHurtSound()
    {
        if (hurtSounds.Count > 0) //if the list is not empty
        {
            characterSoundsSource.Stop(); //stop the current sound
            characterSoundsSource.PlayOneShot(hurtSounds[UnityEngine.Random.Range(0, hurtSounds.Count)], hurtVolume); //play a random hurt sound
        }
    }

    void UpdatePlayerUI()
    {
        gameManager.instance.playerHPCircle.fillAmount = HP / HPOriginal; //get the HP percentage and set the HP bar fill percentage to match it
        if (HP < HPOriginal) //if the player HP is less than the original HP value
        {
            gameManager.instance.playerHPCircleBackground.enabled = true; //set the parent HP circle object to enabled
            gameManager.instance.playerHPCircle.enabled = true; //set the HP circle object to enabled
        }
        else
        {
            gameManager.instance.playerHPCircleBackground.enabled = false; //otherwise disable it
            gameManager.instance.playerHPCircle.enabled = false; //otherwise disable it
        }
    }

    public void UpdateWaterUI()
    {
        gameManager.instance.playerWaterBar.fillAmount = currentWater / maxWater;
        int waterInt = (int)currentWater;
        gameManager.instance.waterCountText.text = waterInt.ToString(); //update the ammo count on the UI
    }

    IEnumerator flashDamage()
    {
        gameManager.instance.playerDamageFlash.SetActive(true); //set the damage effect panel to active
        yield return new WaitForSeconds(0.1f); //wait for a moment
        gameManager.instance.playerDamageFlash.SetActive(false); //set the damage effect panel to inactive
    }

    public void respawn()
    {
        currentJumps = 0;
        isDead = false;
        HP = HPOriginal; //reset the players HP
        UpdatePlayerUI(); //update the players UI

        playerController.enabled = false; //disable the controller
        transform.SetParent(null); //this fixes any issues where the player dies on a moving platform
        transform.position = gameManager.instance.playerSpawnPosition.transform.position; //change the players position to the spawn point position
        playerController.enabled = true; //re enable the controller

        pushBack = Vector3.zero;
    }

    //bouncing and launching
    public void PushBack(Vector3 dirForce)
    {
        pushBack += dirForce;
    }

    public void Launch(Vector3 LaunchMovement)
    {
        if (isSliding) //if the player is sliding when they launch out of a pipe
        {
            characterMovementSource.Stop();
            isSliding = false; //disable sliding
            isCrouched = false; //disable crouching
            playerController.height = defaultControllerHeight; //set the controller and collider heights back to default
            playerCollider.height = defaultColliderHeight; //reset the collider height
        }
        currentJumps = 0; //launching from a pipe resets the jump count
        launchTimer = 0; //reset the launch timer
        movement = new Vector3(LaunchMovement.x, 0, LaunchMovement.z); //get the non vertical movement of the player from the launch force
        verticleVelocity = new Vector3(0, LaunchMovement.y, 0); //get the vertical movement from the launch force 
        isLaunching = true; //set is launching to true
        canJump = false; //if the player launches they shouldn't be able to jump until after landing
        isJumping = false; //if the player launches while jumping they should no longer be considered jumping
        isWallJumping = false; //if the player launches while wall jumping they are no longer considered wall jumping
        isLongJumping = false;
    }

    public void BounceOff(float BounceForce) //this is called if the player stomps on an enemies head
    {
        if (isSliding) //if the player is sliding when they bounce off an enemy
        {
            characterMovementSource.Stop();
            isSliding = false; //disable sliding
            isCrouched = false; //disable crouching
            playerController.height = defaultControllerHeight; //set the controller and collider heights back to default
            playerCollider.height = defaultColliderHeight; //reset the collider height
        }
        verticleVelocity.y = BounceForce; //set the players vertical velocity to the bounce force
    }

    public void upgradeHandler()
    {
        if (gameManager.instance.boughtExtraLife == true)
        {
            playerLives += 1;
        }

        if (gameManager.instance.boughtMaxHPUpgrade == true)
        {
            HP = 20.0f;
            HPOriginal = HP;
        }

        if (gameManager.instance.boughtWaterCapUpgrade == true)
        {
            maxWater = 500.0f;
            UpdateWaterUI();
        }

        if (gameManager.instance.boughtWaterRefillEff == true)
        {
            fillAmountMultiplier = 3;
        }
    }
}
