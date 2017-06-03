using System.Collections;
using System.Collections.Generic;
using UnitedSolution;using UnityEngine;
using UnityEngine.AI;

public class NoRotation : MonoBehaviour {
      
    void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(90,0,0); 
    }
} 