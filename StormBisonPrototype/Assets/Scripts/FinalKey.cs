using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalKey : MonoBehaviour
{
    [SerializeField] MeshRenderer thisRenderer;
    [SerializeField] AudioSource KeySFX;

    public Transform keyMovePosition;

    [SerializeField] float moveSpeed = 0; //how fast the key moves towards its positon

    BoxCollider thisCollider;

    // Start is called before the first frame update
    void Start()
    {
        thisCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, keyMovePosition.position, moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (thisCollider != null)
            {
                thisCollider.enabled = false;
            }
            if (thisRenderer != null)
            {
                thisRenderer.enabled = false;
            }

            gameManager.instance.gotFinalKey = true;
            KeySFX.Play();
            gameManager.instance.youWin();
            Destroy(gameObject, 3);
        }
    }
}
