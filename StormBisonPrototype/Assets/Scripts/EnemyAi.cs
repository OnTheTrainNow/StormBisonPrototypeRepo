using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class enemyAI : MonoBehaviour, IDamage, IPushBack
{
    [Header("Components")]
    [SerializeField] Animator animator;
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform shootPos2;
    [SerializeField] Transform shootPos3;
    [SerializeField] Transform shootPos4;
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

    [Header("Grass Boss")]
    [SerializeField] bool isGrassBoss;
    [SerializeField] int distanceTillShotgun;
    [SerializeField] float shotgunShootRate;

    [Header("Sewer Boss")]
    [SerializeField] bool isSewerBoss;
    [SerializeField] int bulletsUntilExplosive;
    [SerializeField] GameObject sewerBossBullet;

    [Header("Mini Boss")]
    [SerializeField] bool isMiniBoss;

    [Header("Patrolling")]
    [SerializeField] bool isPatrolling;
    [SerializeField] int patrolPauseTime;
    [SerializeField] Transform[] patrolPos;

    [Header("Stationary")]
    [SerializeField] bool isStationary;
    [SerializeField] bool isTurret;
    [SerializeField] Transform barrelPos;
    [SerializeField] Transform barrelAlinPos;
    [SerializeField] int panSpeed;
    [SerializeField] int panPauseTime;
    [SerializeField] Transform[] stationaryLookPos;

    [Header("Drops")]
    [SerializeField] bool isRNG;
    [SerializeField] GameObject drop;
    [SerializeField] GameObject[] rngDrops;
    [SerializeField] float[] rngDropsRate;


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
    int stationaryItr;
    bool stationaryDir;
    float angleToPanPos;
    bool sawPlayer;
    int sewerBulletCount;
    float origShootrate;
    Vector3 barrelAl;
    int miniBossBarrelItr = 1;

    bool isDead; // bool to prevent player shotgun pellets from causing issue with enemycount
    Color defaultColor;

    void Start()
    {   
        stoppingDistOrig = agent.stoppingDistance;
        HPOriginal = HP;
        updateUI();
        if (!isGrassBoss || !isSewerBoss || !isMiniBoss)
        {
            gameManager.instance.updateGameGoal(1);
        }
        defaultColor = model.material.GetColor("_Color"); //get the default color of the enemy;
        patrolItr = -1; //set patrol iterator to -1
        patrolDir = true;//set patrol direction to forward
        stationaryItr = 0; //set patrol iterator to -1
        stationaryDir = true;//set patrol direction to forward
        sewerBulletCount = 0;
        origShootrate = shootRate;
    }

    void Update()
    {
        if (!isTurret)
        {
            float animSpeed = agent.velocity.normalized.magnitude;
            animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), animSpeed, Time.deltaTime * animSpeedTrans));
        }
        if (playerInRange && !canSeePlayer())
        {   
            if (isGrassBoss || isSewerBoss || isMiniBoss)
            {
                if (isMiniBoss)
                {
                    playerDir = gameManager.instance.player.transform.position - headPos.position;
                    faceTargetStationary(playerDir);
                }
                else
                {
                    faceTarget();
                }
            }
            else if (sawPlayer) //if enemy lost sight of player
            {
                playerDir = gameManager.instance.player.transform.position - headPos.position;
                RaycastHit hit;
                if (Physics.Raycast(headPos.position, playerDir, out hit))
                {
                    if (!gameManager.instance.isPaused)
                    {
                        if (hit.collider.CompareTag("Player")) //if player is not obstructed face player else set sawplayer to false
                        {
                            faceTarget();
                        }
                        else
                        {
                            sawPlayer = false;
                        }
                    }
                }
                
            }
            //if Stationary pan between designated points
            else if (isStationary)
            {
                BarrelAlign();
                StartCoroutine(stationaryPan());
            }
            // roam/patroling if I'm in your range but i can't see you
            else if (isPatrolling)
            {
                StartCoroutine(patrol());
            }
            else
            {
                if (!isTurret)
                {
                    StartCoroutine(roam());
                }
            }
            
        }
        else if (!playerInRange)
        {
            if (isGrassBoss || isSewerBoss || isMiniBoss)
            { }
            //if Stationary pan between designated points
            else if (isStationary)
            {
                BarrelAlign();
                StartCoroutine(stationaryPan());
            }
            // roam/patroling because you are not in range
            else if (isPatrolling)
            {
                StartCoroutine(patrol());
            }
            else
            {
                if (!isTurret)
                {
                    StartCoroutine(roam());
                }
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
            SetDesti(hit.position);

            destChosen = false;
        }
        if (agent.velocity.normalized.magnitude > .3 && !isPlayingSteps)
        {
            StartCoroutine(playEnemyFootSteps());
        }
    }

    IEnumerator patrol()
    {   
        agent.stoppingDistance = 0;
        if (agent.remainingDistance < 0.05f && !destChosen)
        {
            destChosen = true;
            //if patrolDir is true iterate forward through array and if its false reverse
            if (patrolDir)
            {
                patrolItr++;
            }
            else
            {
                patrolItr--;
            }
            yield return new WaitForSeconds(patrolPauseTime);
            SetDesti(patrolPos[patrolItr].position);
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

    IEnumerator stationaryPan() 
    {
        angleToPanPos = Vector3.Angle(headPos.position, stationaryLookPos[stationaryItr].position);
        if (angleToPanPos <= 15 && !destChosen)
        {
            destChosen = true;

            yield return new WaitForSeconds(panPauseTime);
            faceTargetStationaryPan(stationaryLookPos[stationaryItr].position - headPos.position);
            //if stationaryDir is true iterate forward through array and if its false reverse
            if (stationaryDir)
            {
                stationaryItr++;
            }
            else
            {
                stationaryItr--;
            }
            //if stationaryItr is at the end of stationaryPos reverse direction and if its at the start set direction to forward
            if (stationaryItr == stationaryLookPos.Length - 1)
            {
                stationaryDir = false;
            }
            if (stationaryItr == 0)
            {
                stationaryDir = true;
            }
            destChosen = false;
        }
        else
        {
            faceTargetStationaryPan(stationaryLookPos[stationaryItr].position - headPos.position);
        }

    }

    public bool IsDead() { return isDead; } //getter method for if the enemy isDead

    bool canSeePlayer()
    {
        agent.stoppingDistance = stoppingDistOrig;
       
        playerDir = gameManager.instance.player.transform.position - headPos.position;

        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        //Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            //Debug.Log(hit.collider.name);

            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewCone)
            {
                SetDesti(gameManager.instance.player.transform.position);                
                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    if (isTurret)
                    {
                        faceTargetStationary(playerDir);
                    }
                    else
                    {
                        faceTarget();
                    }
                    
                }
                if (!isShooting && angleToPlayer <= shootCone)
                {
                    StartCoroutine(shoot());
                    enemyAudio.PlayOneShot(enemyShots, enemyShotsVol);
                }

                agent.stoppingDistance = stoppingDistOrig;
                sawPlayer = true;
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
    void faceTargetStationary(Vector3 target)
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(target.x, transform.position.y, target.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * targetFaceSpeed);
        Quaternion rot1 = Quaternion.LookRotation(new Vector3(transform.rotation.x + target.x, target.y+1, target.z));
        barrelPos.transform.rotation = Quaternion.Lerp(barrelPos.transform.rotation, rot1, Time.deltaTime * targetFaceSpeed);
    }
    void faceTargetStationaryPan(Vector3 target)
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(target.x, transform.position.y, target.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * panSpeed);
        //Quaternion rot1 = Quaternion.LookRotation(new Vector3(transform.rotation.x + target.x, target.y+1, target.z));
        //barrelPos.transform.rotation = Quaternion.Lerp(barrelPos.transform.rotation, rot1, Time.deltaTime * panSpeed);
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

    void SetDesti(Vector3 desti) 
    {
        if (!isDead)
        {
            agent.SetDestination(desti);
        }
    }

    public void TakeDamage(float amount)
    {
        if (!isTurret)
        {
            agent.stoppingDistance = 0;
        }
        HP -= amount;
        if (!isDead)
        {
            SetDesti(gameManager.instance.player.transform.position); //have the enemy move to the position they were shot from
            if (isTurret)
            {
                playerDir = gameManager.instance.player.transform.position - headPos.position;
                faceTarget();
                sawPlayer = true;
            }
        }
        
        enemyAudio.PlayOneShot(enemyHurtSound[Random.Range(0, enemyHurtSound.Length)], enemyHurtVol);

        updateUI();
        if (!isTurret) 
        {
            StartCoroutine(flashMat());
        }
        if (HP <= 0 && !isDead)
        {
            isDead = true;
            checkForStarDrop();
            if (!isGrassBoss || !isSewerBoss || !isMiniBoss)
            {
                gameManager.instance.updateGameGoal(-1);
            }
            Destroy(gameObject);
            Drop();
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
            Drop();
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
        if (!isTurret)
        {
            Quaternion rot1 = Quaternion.LookRotation(new Vector3(transform.rotation.x + playerDir2.x, playerDir2.y, playerDir2.z));
            shootPos.transform.rotation = rot1;
        }
        else
        {
            Quaternion rot1 = Quaternion.LookRotation(new Vector3(transform.rotation.x + playerDir2.x, playerDir2.y, playerDir2.z));
            barrelPos.transform.rotation = rot1;
        }
        //shootPos 2 and 3 rotate with shootPos1 because they are attached to it
        
        if (isGrassBoss) 
        {
            shootRate = shotgunShootRate;
            float distance = Vector3.Distance(transform.position, gameManager.instance.player.transform.position);
            if (distance <= distanceTillShotgun)
            {
                playerDir2.y = playerDir2.y - 1;
                Quaternion rot1 = Quaternion.LookRotation(new Vector3(transform.rotation.x + playerDir2.x, playerDir2.y, playerDir2.z));
                shootPos.transform.rotation = rot1;
                Instantiate(bullet, shootPos.position, shootPos.transform.rotation);
                if (shootPos2 != null)
                {
                    Instantiate(bullet, shootPos2.position, shootPos2.transform.rotation);
                }
                if (shootPos3 != null)
                {
                    Instantiate(bullet, shootPos3.position, shootPos3.transform.rotation);
                }
            }
            else
            {
                shootRate = origShootrate;
                Instantiate(bullet, shootPos.position, shootPos.transform.rotation);
            }
        }
        else if (isSewerBoss) 
        {
            if (sewerBulletCount >= bulletsUntilExplosive)
            {
                Instantiate(sewerBossBullet, shootPos.position, shootPos.transform.rotation);
                sewerBulletCount = 0;
            }
            else
            {
                Instantiate(bullet, shootPos.position, shootPos.transform.rotation);
                sewerBulletCount++;
            }
        }
        else if (isMiniBoss)
        {
            if (miniBossBarrelItr == 1)
            {
                Instantiate(bullet, shootPos.position, shootPos.transform.rotation);
                miniBossBarrelItr++;
            }
            else if (miniBossBarrelItr == 2)
            {
                Instantiate(bullet, shootPos2.position, shootPos2.transform.rotation);
                miniBossBarrelItr++;
            }
            else if (miniBossBarrelItr == 3)
            {
                Instantiate(bullet, shootPos3.position, shootPos3.transform.rotation);
                miniBossBarrelItr++;
            }
            else if (miniBossBarrelItr == 4)
            {
                Instantiate(bullet, shootPos4.position, shootPos4.transform.rotation);
                miniBossBarrelItr = 1;
            }
        }
        else
        {
            Instantiate(bullet, shootPos.position, shootPos.transform.rotation);
            if (shootPos2 != null)
            {
                Instantiate(bullet, shootPos2.position, shootPos2.transform.rotation);
            }
            if (shootPos3 != null)
            {
                Instantiate(bullet, shootPos3.position, shootPos3.transform.rotation);
            }
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    void Drop()
    {
        Vector3 dropPos = transform.position;
        dropPos.y += 1;
        if (isRNG)
        {
            float dropFloat = Random.Range((float)0.0, (float)1.0);
            for (int i = 0; i < rngDrops.Length; i++)
            {
                if (dropFloat <= rngDropsRate[i])
                {
                    Instantiate(rngDrops[i], dropPos, transform.rotation);
                    i = rngDrops.Length;
                }
            }
        }
        else if(drop != null)
        {
            Instantiate(drop, dropPos, transform.rotation);
        }
    }

    void BarrelAlign()
    {
        barrelAl = barrelAlinPos.position - headPos.position;
        Quaternion rot1 = Quaternion.LookRotation(new Vector3(transform.rotation.x + barrelAl.x, barrelAl.y + 1, barrelAl.z));
        barrelPos.transform.rotation = Quaternion.Lerp(barrelPos.transform.rotation, rot1, Time.deltaTime * targetFaceSpeed);
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