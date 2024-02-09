using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour //this script goes on the waypoint path parent object not the waypoints themselves (it is used for moving a platform along a path)
{
    public Transform GetWaypoint(int currWaypointIndex) //use the index of the current waypoint to return its transform
    {
        return transform.GetChild(currWaypointIndex); //get the child objects transform component at the current index (the childs transform is the actual waypoint)
    }

    public int NextWaypointIndex(int currWaypointIndex) //use the current waypoint index to get the next waypoint index
    {
        int nextWaypointIndex = currWaypointIndex+1; //increment the index (you have to manually add by 1 here for the initial movement to work in the platform mover class)

        if (nextWaypointIndex == transform.childCount) //check if the index has reached the parent path object's child count (which means its at the end of the path)
        {
            nextWaypointIndex = 0; //go back to the beginning of the path
        }

        return nextWaypointIndex; //return the next index
    }
}
