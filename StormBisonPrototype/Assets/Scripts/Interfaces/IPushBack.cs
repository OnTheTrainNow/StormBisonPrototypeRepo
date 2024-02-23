using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPushBack 
{
    void PushBack(Vector3 dirForce); //the push back method needs to take in a directional force
    void Launch(Vector3 LaunchMovement); //the launch method needs a directional force
    void BounceOff(float BounceForce); //bounce is only really supposed to affect the vertical velocity
}
