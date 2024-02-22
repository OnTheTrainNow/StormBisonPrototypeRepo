using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    [SerializeField] Transform startPosition; //the platforms starting position (these should be emtpy game objects placed at the desired positions for the platform)
    [SerializeField] Transform endPosition; //the platforms ending position
    [SerializeField] float platformSpeed; //how fast the platform moves
    [SerializeField] bool movementEnabled; //this can be used to turn the movement on or off

    float journeyPercentage; //this is a point along the path between the two positions (its the percentage of the distance between the two) 

    void Start()
    {

    }

    void FixedUpdate() //fixed update is needed for moving platforms since the fixed frame rates keeps the player from being unsynced with the platform
    {
        if (movementEnabled) 
        {
            journeyPercentage = Mathf.PingPong(Time.time * platformSpeed, 1f); //mathf ping pong can be used to oscillate between 0 and 1, which gives us a percentage to use for interpolation
            journeyPercentage = Mathf.SmoothStep(0,1, journeyPercentage); //smooth out the movement when the platform approaches one of the two points 
            transform.position = Vector3.Lerp(startPosition.position, endPosition.position, journeyPercentage); //lerp interpolates between the two points based on the journey percentage 
        }
    }
}
