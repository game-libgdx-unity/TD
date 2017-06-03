using System.Collections;
using System.Collections.Generic;
using UnitedSolution;using UnityEngine;

public class ParticleSize : MonoBehaviour {

    public float size = 3f;
    public ParticleSystem particleSystem;
	// Use this for initialization
	void Start () {
        particleSystem.Scale(size);
    }
}
