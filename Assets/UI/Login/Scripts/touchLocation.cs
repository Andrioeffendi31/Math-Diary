using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class touchLocation : MonoBehaviour
{
    public int touchId;
    public GameObject particle;

    public touchLocation(int newTouchId, GameObject newpParticle)
    {
        touchId = newTouchId;
        particle = newpParticle;
    }
}