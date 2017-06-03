using UnitedSolution;using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnitedSolution
{
    [System.Serializable]
    public class UnitStat
    {
        public AbilityBehavior abilityHolder;
        public Sprite icon;
        public LayerMask customMask = LayerManager.LayerDefault();
        public float damageMin = 5;
        public float damageMax = 6;
        public float cooldown = 1;
        public float clipSize = -1;     //not in used when set to <0
        public float reloadDuration = 2;
        public float attackRange = 10;
        public float detectRange = 10;
        [HideInInspector]
        public float minRange = 0;
        public float aoeRadius = 0; //aoe radius when shootObject hit target

        public float hit = 0;
        public float dodge = 0;

        public float shieldBreak = 0;       //if >0, then tower has chance to break shield (disable shield)
        public float shieldPierce = 0;  //if >0, then tower has chance to bypass shield
        public bool damageShieldOnly = false;   //when set to true, tower damage shield only

        public Critical crit;
        public Stun stun;
        public Slow slow;
        public Dot dot;             //damage over time
        public InstantKill instantKill;

        public Buff buff;               //for support tower
        public List<int> rscGain = new List<int>(); //for resource tower

        public List<int> cost = new List<int>();
        public float buildDuration = 1;
        public float unBuildDuration = 1;

        public Transform shootObjectT;
        public List<ShootObject> shootObjects;
        public ShootObject shootObject; //legacy shootObject

        public bool useCustomDesp = false;
        public string desp = "";

        private int shootObjectIndex = 0;
        public bool multiShoot
        {
            get { return ShootObject?shootObjects[shootObjectIndex].multiShoot:false; }
        }

        public ShootObject ShootObject  //new shootObjects
        {
            get
            {
                if (shootObjects == null || shootObjects.Count == 0)
                {
                    return shootObject;
                }
                else
                {
                    return shootObjects[ShootObjectIndex];
                }
            }
        }

        public int ShootObjectIndex
        {
            get
            {
                return shootObjectIndex;
            }
            set
            {
                if (value > shootObjects.Count - 1)
                    value = 0;
                else if (value < 0)
                    value = shootObjects.Count - 1;

                shootObjectIndex = value;
            }
        }

        public UnitStat()
        {
            stun = new Stun();
            crit = new Critical();
            slow = new Slow();
            dot = new Dot();
            instantKill = new InstantKill();
            buff = new Buff();
            //if(shootObjects == null)
            //{
            //    shootObjects = new List<ShootObject>();
            //    shootObjects.Add(null);
            //}
        }

        public UnitStat Clone()
        {
            UnitStat stat = new UnitStat();
            stat.shootObjects = new List<ShootObject>(shootObjects);
            stat.icon = icon;
            stat.abilityHolder = abilityHolder;
            stat.customMask = customMask;
            stat.damageMin = damageMin;
            stat.damageMax = damageMax;
            stat.clipSize = clipSize;
            stat.reloadDuration = reloadDuration;
            stat.minRange = minRange;
            stat.attackRange = attackRange;
            stat.aoeRadius = aoeRadius;
            stat.hit = hit;
            stat.dodge = dodge;
            stat.shieldBreak = shieldBreak;
            stat.shieldPierce = shieldPierce;
            stat.damageShieldOnly = damageShieldOnly;
            stat.crit = crit.Clone();
            stat.stun = stun.Clone();
            stat.slow = slow.Clone();
            stat.dot = dot.Clone();
            stat.instantKill = instantKill.Clone();
            stat.buff = buff.Clone();
            stat.buildDuration = buildDuration;
            stat.unBuildDuration = unBuildDuration;
            stat.shootObjectT = shootObjectT;
            stat.desp = desp;
            //stat.despGeneral=despGeneral;
            //for(int i=0; i<rscGain.Count; i++) stat.rscGain.Add(rscGain[i]);
            //for(int i=0; i<cost.Count; i++) stat.cost.Add(cost[i]);

            stat.rscGain = new List<int>(rscGain);
            stat.cost = new List<int>(cost);

            return stat;
        }



    }

    [System.Serializable]
    public class Stun
    {
        public float chance = 0;
        public float duration = 0;

        public Stun(float c = 0, float dur = 0)
        {
            chance = c; duration = dur;
        }

        public bool IsValid()
        {   //funcion to determine if the stun is available
            if (duration > 0 && chance > 0) return true;
            return false;
        }

        public bool IsApplicable()
        {   //function to determine if the stun works on a unit
            if (duration > 0)
            {
                if (Random.Range(0f, 1f) < chance) return true;
            }
            return false;
        }

        public Stun Clone()
        {
            Stun stun = new Stun();
            stun.chance = chance;
            stun.duration = duration;
            return stun;
        }
    }

    [System.Serializable]
    public class Critical : ICloneable<Critical>
    {
        public float chance = 0;
        public float dmgMultiplier = 0;

        public Critical Clone()
        {
            Critical clone = new Critical();
            clone.chance = chance;
            clone.dmgMultiplier = dmgMultiplier;
            return clone;
        }
    }

    [System.Serializable]
    public class Slow
    {
        public int effectID = 0;
        public float duration = 0;
        public float slowMultiplier = 1f;

        public Slow(float s = 0, float dur = 0)
        {
            slowMultiplier = s; duration = dur;
        }

        public bool IsValid() { return (duration <= 0 || slowMultiplier <= 0) ? false : true; }

        public Slow Clone()
        {
            Slow clone = new Slow();
            clone.effectID = effectID;
            clone.slowMultiplier = slowMultiplier;
            clone.duration = duration;
            //~ clone.timeEnd=timeEnd;
            return clone;
        }
    }

    [System.Serializable]
    public class Dot
    {
        public int effectID = 0;
        public float duration = 0f;
        public float interval = 0f;
        public float value = 0f;

        public Dot(float dur = 0, float i = 0, float val = 0)
        {
            duration = dur; interval = i; value = val;
        }

        public float GetTotalDamage() { return (duration / interval) * value; }

        public Dot Clone()
        {
            Dot clone = new Dot();
            clone.effectID = effectID;
            clone.duration = duration;
            clone.interval = interval;
            clone.value = value;
            return clone;
        }
    }

    [System.Serializable]
    public class InstantKill
    {
        public float chance = 0;
        public float HPThreshold = 0.3f;

        public InstantKill(float c = 0, float th = 0)
        {
            chance = c; HPThreshold = th;
        }

        public bool IsValid()
        {   //funcion to determine if the effect is available
            if (HPThreshold > 0 && chance > 0) return true;
            return false;
        }

        public bool IsApplicable(float HP, float fullHP)
        {
            if ((HP / fullHP) <= HPThreshold)
            {
                if (Random.Range(0f, 1f) < chance) return true;
            }
            return false;
        }

        public InstantKill Clone()
        {
            InstantKill instantKill = new InstantKill();
            instantKill.chance = chance;
            instantKill.HPThreshold = HPThreshold;
            return instantKill;
        }
    }


    [System.Serializable]
    public class Buff
    {
        //in case of multiple buff instance, the most powerful buff from each stat is taken as the actual effective buff value
        public int effectID = 0;        //this is the instanceID of the casting unit
        public float damageBuff = 0f;
        public float cooldownBuff = 0f;
        public float rangeBuff = 0f;
        public float criticalBuff = 0f;
        public float hitBuff = 0f;
        public float dodgeBuff = 0f;
        public float regenHP = 0f;

        public Buff Clone()
        {
            Buff clone = new Buff();
            clone.effectID = effectID;
            clone.damageBuff = damageBuff;
            clone.cooldownBuff = cooldownBuff;
            clone.rangeBuff = rangeBuff;
            clone.criticalBuff = criticalBuff;
            clone.hitBuff = hitBuff;
            clone.dodgeBuff = dodgeBuff;
            clone.regenHP = regenHP;
            return clone;
        }
    }

}
