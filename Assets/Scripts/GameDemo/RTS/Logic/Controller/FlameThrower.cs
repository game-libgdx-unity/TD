using System.Collections;
using System.Collections.Generic;
using UnitedSolution;using UnityEngine;

public class FlameThrower : MonoBehaviour
{

    public ParticleSystem gunEffect1;
    public ParticleSystem gunEffect2;
    // Use this for initialization
    void OnAttackTargetStarted()
    {
        if (gunEffect1) gunEffect1.Play();
        if (gunEffect2) gunEffect2.Play();
    }

    void OnAttackTargetStopped()
    {
        if (gunEffect1) gunEffect1.Stop();
        if (gunEffect2) gunEffect2.Stop();
    }
}
