using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMoverWaypoint : MonoBehaviour //this version of the platform mover moves the platform along a path instead of between two points
{
    [SerializeField] WaypointPath platformPath; //this is the path parent object that contains the waypoints
    [SerializeField] List<Transform> waypointPath = new List<Transform>();
    [SerializeField] float platformSpeed; //this is the movement speed for the platform

    int targetWaypointIndex; //this is the index of the waypoint the platform is currently moving towards

    Transform prevWaypoint; //this is the transform componenet of the last waypoint
    Transform targetWaypoint; //this is the transform componenet for the target waypoint

    float timeToReachWaypoint; //these will be used to determine path percentage
    float currentElapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        TargetNextWaypoint();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentElapsedTime += Time.deltaTime;

        float percentage = currentElapsedTime / timeToReachWaypoint; //get the percentage of the distance traveled so far using the elapsed to and time to reach waypoint
        percentage = Mathf.SmoothStep(0,1,percentage); //this smooths out the movement when the platform approaches one of the waypoints
        transform.position = Vector3.Lerp(prevWaypoint.position, targetWaypoint.position, percentage); //lerp to the next position based on percentage
        transform.rotation = Quaternion.Lerp(prevWaypoint.rotation, targetWaypoint.rotation, percentage); //interpolate the rotation between the two waypoints (dont rotate the waypoints if you want no rotation)

        if (percentage >= 1) //if the percentage is at 1 that means the current movement between positions was finished
        {
            TargetNextWaypoint(); //so the next waypoint can now be targetted 
        }
    }

    void TargetNextWaypoint()
    {
        //prevWaypoint = platformPath.GetWaypoint(targetWaypointIndex); //set the previous waypoint to the current target waypoint
        prevWaypoint = waypointPath[targetWaypointIndex];

        //targetWaypointIndex = platformPath.NextWaypointIndex(targetWaypointIndex); //set the current target waypoint index to the next index
        int nextIndex = targetWaypointIndex + 1;
        if (nextIndex == waypointPath.Count)
        {
            nextIndex = 0;
        }
        targetWaypointIndex = nextIndex;

        //targetWaypoint = platformPath.GetWaypoint(targetWaypointIndex); //set the target waypoint to the next waypoint
        targetWaypoint = waypointPath[targetWaypointIndex];

        currentElapsedTime = 0; //set the elapsed time to zero

        float distance = Vector3.Distance(prevWaypoint.position, targetWaypoint.position); //get the distance between the prev waypoint and the target waypoint
        timeToReachWaypoint = distance / platformSpeed;
    }
}
