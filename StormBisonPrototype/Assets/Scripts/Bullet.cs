using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class bullet : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject explosion;

    [Header("Bullet Stats")]
    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] bool isHoming;
    [SerializeField] bool isExplosive;

    bool explode;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, destroyTime);
    }

    private void Update()
    {
        if (isHoming)
        {
            Vector3 playerDir = gameManager.instance.player.transform.position - rb.position;
            Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, playerDir.y, playerDir.z));
            rb.MoveRotation(Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * speed));
            rb.velocity = transform.forward * speed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null)
        {
            dmg.TakeDamage(damageAmount);
        }
        if (isExplosive && !explode)
        {
            Instantiate(explosion, transform.position, transform.rotation);
            explode = true;
        }
        Destroy(gameObject);
    }
}