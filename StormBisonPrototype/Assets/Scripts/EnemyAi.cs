using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
    [SerializeField] Image HPCircle; //the enemy HP circle

    float HPOriginal;

    bool isShooting;
    bool playerInRange;
    float angleToPlayer;
    Vector3 playerDir;

    private Color originalColor;
    private Renderer rend;

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

        if (HP <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
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

        Instantiate(bullet, shootPos.position, transform.rotation);

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