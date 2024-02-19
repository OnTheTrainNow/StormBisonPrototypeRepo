using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform shootPos2;
    [SerializeField] Transform shootPos3;
    [SerializeField] Transform headPos;

    [SerializeField] float HP;
    [SerializeField] int viewCone;
    [SerializeField] int targetFaceSpeed;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    [SerializeField] Canvas HPUI; //the enemy HP UI canvas
    [SerializeField] UnityEngine.UI.Image HPCircle; //the enemy HP circle

    [SerializeField] float playerStompBounceForce = 0; //how high the player bounces when they stomp on the enemy

    float HPOriginal;

    bool isShooting;
    bool playerInRange;
    float angleToPlayer;
    Vector3 playerDir;
    Vector3 playerDir2;

    Color originalColor;
    Renderer rend;

    bool isDead; // bool to prevent player shotgun pellets from causing issue with enemycount

    void Start()
    {
        HPOriginal = HP;
        updateUI();
        gameManager.instance.updateGameGoal(1);

        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    void Update()
    {
        if (playerInRange && canSeePlayer())
        {

        }
    }

    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;

        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            Debug.Log(hit.collider.name);

            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewCone)
            {
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    faceTarget();
                }

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

        updateUI();
        StartCoroutine(flashMat());

        if (HP <= 0 && !isDead)
        {
            isDead = true;
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    public void StompedOn() //this gets called by a seperate child trigger object.
    {
        if (!isDead) //to prevent issues make sure this can only trigger if the isDead bool is false
        {
            isDead = true; //set the bool to true to prevent player from killing the enemy twice by shooting at the same time
            gameManager.instance.updateGameGoal(-1); //update the game goal
            gameManager.instance.playerScript.EnemyBounce(playerStompBounceForce); //call the players bounce method and pass in the bounce force
            Destroy(gameObject); //destroy the enemy
        }
    }

    IEnumerator flashMat()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = originalColor;
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
}