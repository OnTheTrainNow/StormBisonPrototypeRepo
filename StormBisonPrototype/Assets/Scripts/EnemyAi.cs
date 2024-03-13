using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class enemyAI : MonoBehaviour, IDamage, IPushBack
{
    [Header("Components")]
    [SerializeField] Animator animator;
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform shootPos2;
    [SerializeField] Transform shootPos3;
    [SerializeField] Transform headPos;
    [SerializeField] AudioSource enemyAudio;

    [Header("Enemy Stats")]
    [SerializeField] float HP;
    [SerializeField] int viewCone;
    [SerializeField] int shootCone;
    [SerializeField] int targetFaceSpeed;
    [SerializeField] int animSpeedTrans;
    [SerializeField] int roamPauseTime;
    [SerializeField] int roamDist;

    [Header("Weapon Stats")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    [Header("Enemy UI")]
    [SerializeField] Canvas HPUI; //the enemy HP UI canvas
    [SerializeField] UnityEngine.UI.Image HPCircle; //the enemy HP circle

    [Header("Player Stomp Stats")]
    [SerializeField] float playerStompBounceForce = 0; //how high the player bounces when they stomp on the enemy
    [SerializeField] bool dropsStar;

    [Header("Audio")]
    [SerializeField] AudioClip[] enemySteps;
    [Range(0, 1)][SerializeField] float enemyStepsVol = 0.5f;
    [SerializeField] AudioClip[] enemyHurtSound;
    [Range(0, 1)] [SerializeField] float enemyHurtVol = 0.5f;
    [SerializeField] AudioClip enemyShots;
    [Range(0, 1)] [SerializeField] float enemyShotsVol = 0.5f;

    [Header("Patrolling")]
    [SerializeField] bool patrolling;
    [SerializeField] Transform[] patrolPos;

    float HPOriginal;

    bool isShooting;
    bool playerInRange;
    float angleToPlayer;
    Vector3 playerDir;
    Vector3 playerDir2;
    float stoppingDistOrig;
    Vector3 startingPos;
    bool destChosen;
    bool isPlayingSteps;
    int patrolItr;
    bool patrolDir;

    bool isDead; // bool to prevent player shotgun pellets from causing issue with enemycount
    Color defaultColor;

    void Start()
    {
        stoppingDistOrig = agent.stoppingDistance;
        HPOriginal = HP;
        updateUI();
        gameManager.instance.updateGameGoal(1);
        defaultColor = model.material.GetColor("_Color"); //get the default color of the enemy;
        patrolItr = 0; //set patrol iterator to 0
        patrolDir = true;//set patrol direction to forward
    }

    void Update()
    {
        float animSpeed = agent.velocity.normalized.magnitude;
        animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), animSpeed, Time.deltaTime * animSpeedTrans));

        if (playerInRange && !canSeePlayer())
        {
            // roam/patroling if I'm in your range but i can't see you
            if (patrolling)
            {
                StartCoroutine(patrol());
            }
            else
            {
                StartCoroutine(roam());
            }
            
        }
        else if (!playerInRange)
        {
            // roam/patroling because you are not in range
            if (patrolling)
            {
                StartCoroutine(patrol());
            }
            else
            {
                StartCoroutine(roam());
            }
        }
    }

    IEnumerator roam()
    {
        if (agent.remainingDistance < 0.05f && !destChosen)
        {
            destChosen = true;
            agent.stoppingDistance = 0;
            yield return new WaitForSeconds(roamPauseTime);

            Vector3 randomPos = Random.insideUnitSphere * roamDist;
            randomPos += startingPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
            agent.SetDestination(hit.position);

            destChosen = false;
        }
        if (agent.velocity.normalized.magnitude > .3 && !isPlayingSteps)
        {
            StartCoroutine(playEnemyFootSteps());
        }
    }

    IEnumerator patrol()
    {   
        
        if (agent.remainingDistance < 0.05f && !destChosen)
        {
            destChosen = true;
            agent.stoppingDistance = 0;
            yield return new WaitForSeconds(roamPauseTime);
            agent.SetDestination(patrolPos[patrolItr].position);
            //if patrolDir is true iterate forward through array and if its false reverse
            if (patrolDir)
            {
                patrolItr++;
            }
            else
            {
                patrolItr--;
            }
            //if patrolItr is at the end of patrolPos reverse direction and if its at teh start set direction to forward
            if (patrolItr == patrolPos.Length - 1)
            {
                patrolDir = false;
            }
            if (patrolItr == 0)
            {
                patrolDir = true;
            }
            destChosen = false;
        }
        if (agent.velocity.normalized.magnitude > .3 && !isPlayingSteps)
        {
            StartCoroutine(playEnemyFootSteps());
        }
    }

    public bool IsDead() { return isDead; } //getter method for if the enemy isDead

    bool canSeePlayer()
    {
        agent.stoppingDistance = stoppingDistOrig;
        playerDir = gameManager.instance.player.transform.position - headPos.position;

        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            Debug.Log(hit.collider.name);

            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewCone)
            {
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (!isShooting && angleToPlayer <= shootCone)
                {
                    StartCoroutine(shoot());
                    enemyAudio.PlayOneShot(enemyShots, enemyShotsVol);
                }

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    faceTarget();
                }
                agent.stoppingDistance = stoppingDistOrig;

                return true;
            }
        }
        return false;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * targetFaceSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void TakeDamage(float amount)
    {
        agent.SetDestination(gameManager.instance.player.transform.position); //have the enemy move to the position they were shot from
        HP -= amount;

        enemyAudio.PlayOneShot(enemyHurtSound[Random.Range(0, enemyHurtSound.Length)], enemyHurtVol);

        updateUI();
        StartCoroutine(flashMat());

        if (HP <= 0 && !isDead)
        {
            isDead = true;
            checkForStarDrop();
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    public void StompedOn() //this gets called by a seperate child trigger object.
    {
        if (!isDead) //to prevent issues make sure this can only trigger if the isDead bool is false
        {
            isDead = true; //set the bool to true to prevent player from killing the enemy twice by shooting at the same time
            checkForStarDrop();
            gameManager.instance.updateGameGoal(-1); //update the game goal
            gameManager.instance.playerScript.BounceOff(playerStompBounceForce); //call the players bounce method and pass in the bounce force
            Destroy(gameObject); //destroy the enemy
        }
    }

    void checkForStarDrop()
    {
        if (dropsStar)
        {
            BossStar bossStar = GetComponent<BossStar>();
            if (bossStar != null)
            {
                bossStar.spawnStar();
            }
        }
    }

    IEnumerator flashMat()
    {
        model.material.color = Color.red; //set the material color to red
        yield return new WaitForSeconds(0.1f);
        model.material.color = defaultColor; //set the material color to default
    }

    IEnumerator shoot()
    {
        isShooting = true;
        playerDir2 = playerDir;//copy player direction
        playerDir2.y = playerDir2.y + 1; //raise target by 1
        //rotate shootPos to player
        Quaternion rot1 = Quaternion.LookRotation(new Vector3(transform.rotation.x + playerDir2.x, playerDir2.y, playerDir2.z));
        shootPos.transform.rotation = rot1;
        //shootPos 2 and 3 rotate with shootPos1 because they are attached to it
        Instantiate(bullet, shootPos.position, shootPos.transform.rotation);
        if (shootPos2 != null)
        {
            Instantiate(bullet, shootPos2.position, shootPos2.transform.rotation);
        }
        if (shootPos3 != null)
        {
            Instantiate(bullet, shootPos3.position, shootPos3.transform.rotation);
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    void updateUI()
    {
        HPCircle.fillAmount = HP / HPOriginal; //change the fill percentage on the HP bar image to match the current HP percentage
        if (HP < HPOriginal)
        {
            HPUI.enabled = true; //set the UI components to enabled
        }
        else
        {
            HPUI.enabled = false;  //set the UI components to disabled
        }
    }

    public void pushBackDir(Vector3 dir)
    {
        agent.velocity += (dir / 2);
    }

    public void PushBack(Vector3 dirForce)
    {
        //throw new System.NotImplementedException();
    }

    public void Launch(Vector3 LaunchMovement)
    {
        //throw new System.NotImplementedException();
    }

    public void BounceOff(float BounceForce)
    {
        //throw new System.NotImplementedException();
    }
    IEnumerator playEnemyFootSteps()
    {
        isPlayingSteps = true;
        enemyAudio.PlayOneShot(enemySteps[Random.Range(0, enemySteps.Length)], enemyStepsVol);
        yield return new WaitForSeconds(0.3f);
        isPlayingSteps = false;
    }
}