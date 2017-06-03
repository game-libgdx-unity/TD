using UnitedSolution;using UnityEngine;
using System.Collections;

using UnitedSolution;

namespace UnitedSolution
{ 
    public class AttackInstance2D
    {

        public bool processed = false;
         
        public Unit2D srcUnit;
        public Unit2D tgtUnit;

        public Vector3 impactPoint; 

        public bool missed = false;
        public bool critical = false;
        public bool destroy = false;

        public bool stunned = false;
        public bool slowed = false;
        public bool dotted = false;

        public bool instantKill = false;
        public bool breakShield = false;
        public bool pierceShield = false;

        public float damage = 0;
        public float damageHP = 0;
        public float damageShield = 0;

        public Stun stun;
        public Slow slow;
        public Dot dot;


        //do the stats processing
        public void Process()
        {
            if (processed) return;

            processed = true;

          //  if (srcUnit != null) Process_SrcUnit();
        } 

        //clone an instance
        public AttackInstance2D Clone()
        {
            AttackInstance2D attInstance = new AttackInstance2D();

            attInstance.processed = processed; 
            attInstance.srcUnit = srcUnit;
            attInstance.tgtUnit = tgtUnit;

            attInstance.missed = missed;
            attInstance.critical = critical;
            attInstance.destroy = destroy;

            attInstance.stunned = stunned;
            attInstance.slowed = slowed;
            attInstance.dotted = dotted;

            attInstance.instantKill = instantKill;
            attInstance.breakShield = breakShield;
            attInstance.pierceShield = pierceShield;

            attInstance.damage = damage;
            attInstance.damageHP = damageHP;
            attInstance.damageShield = damageShield;

            attInstance.stun = stun;
            attInstance.slow = slow;
            attInstance.dot = dot;

            return attInstance;
        }

    }

}