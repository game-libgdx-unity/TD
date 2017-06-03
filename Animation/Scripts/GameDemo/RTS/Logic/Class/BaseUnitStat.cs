using System.Collections.Generic;
using UnityEngine;

namespace UnitedSolution
{
    public class BaseUnitStat<T> where T : BaseUnitStat<T>
    {
        public Sprite icon;
        public LayerMask customMask = LayerManager.LayerDefault();
        public float damageMin = 5;
        public float damageMax = 6;
        public float attackCooldown = 1;
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
        public bool useCustomDesp = false;
        public string desp = "";

        public virtual T Clone()
        {
            BaseUnitStat<T> stat = new BaseUnitStat<T>();

            stat.icon = icon;
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
            stat.rscGain = new List<int>(rscGain);
            stat.cost = new List<int>(cost);

            return (T)stat;
        }

    }
}