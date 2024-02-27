using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    [SerializeField] MeshRenderer thisRenderer;
    [SerializeField] AudioSource starSFX;
    [SerializeField] bool isStaticStar; //if the star moves or not
    [SerializeField] float rotateAngle = 0; 

    [SerializeField] int positionIndex = 0; //this is the index for the star's position in the star managers position list
    [SerializeField] float moveSpeed = 0; //how fast the star moves towards its positon

    BoxCollider thisCollider;

    // Start is called before the first frame update
    void Start()
    {
        thisCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, rotateAngle); //rotate at the current position
        if (isStaticStar) return; //if the star is static than it doesn't need to move
        transform.position = Vector3.MoveTowards(transform.position, starManager.instance.starPositions[positionIndex].position, moveSpeed * Time.deltaTime);
        //move towards the stars position from the starManager list
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
            starSFX.Play();
            gameManager.instance.youWin();
            Destroy(gameObject, 3);
        }
    }
}
