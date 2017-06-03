using UnitedSolution;using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

    public class UnitWall : Unit
    {
        public float delay = .1f;
        public new void Dead()
        {
            dead = true;
            if (deadEffectObj != null) ObjectPoolManager.Spawn(deadEffectObj, targetPoint.position, thisT.rotation);
            DestroyObject(gameObject, delay);
        }
    }

}