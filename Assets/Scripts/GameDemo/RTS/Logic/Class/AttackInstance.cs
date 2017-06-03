using UnitedSolution;using UnityEngine;
using System.Collections;

using UnitedSolution;

namespace UnitedSolution
{ 
    public class AttackInstance
    {

        public bool processed = false;

        public FPSWeapon srcWeapon;
        public Unit srcUnit;
        public Unit tgtUnit;

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

            if (srcUnit != null) Process_SrcUnit();
            else if (srcWeapon != null) Process_SrcWeapon();
        }

        //stats processing for attack from fps-weapon
        public void Process_SrcWeapon()
        {
            if (srcWeapon.GetInstantKill().IsApplicable(tgtUnit.HP, tgtUnit.fullHP))
            {
                damage = tgtUnit.HP;
                damageHP = tgtUnit.HP;
                instantKill = true;
                destroy = true;
                return;
            }

            damage = Random.Range(srcWeapon.GetDamageMin(), srcWeapon.GetDamageMax());
            damage /= (float)srcWeapon.GetShootPointCount();    //divide the damage by number of shootPoint

            float critChance = srcWeapon.GetCritChance();
            if (tgtUnit.immuneToCrit) critChance = -1f;
            if (Random.Range(0f, 1f) < critChance)
            {
                critical = true;
                damage *= srcWeapon.GetCritMultiplier();
                //new TextOverlay(impactPoint, "Critical", new Color(1f, .6f, 0f, 1f));
            }

            float dmgModifier = DamageTable.GetModifier(tgtUnit.armorType, srcWeapon.damageType);
            damage *= dmgModifier;

            if (damage >= tgtUnit.shield)
            {
                damageShield = tgtUnit.shield;
                damageHP = damage - tgtUnit.shield;
            }
            else
            {
                damageShield = damage;
                damageHP = 0;
            }


            if (Random.Range(0f, 1f) < srcWeapon.GetShieldPierce() && damageShield > 0)
            {
                damageHP += damageShield;
                damageShield = 0;
                pierceShield = true;
                //new TextOverlay(impactPoint,"Shield Pierced", new Color(0f, 1f, 1f, 1f));
            }
            if (srcWeapon.DamageShieldOnly()) damageHP = 0;

            if (damageHP >= tgtUnit.HP)
            {
                destroy = true;
                return;
            }

            if (Random.Range(0f, 1f) < srcWeapon.GetShieldBreak() && tgtUnit.fullShield > 0) breakShield = true;

            stunned = srcWeapon.GetStun().IsApplicable();
            if (tgtUnit.immuneToStun) stunned = false;

            slowed = srcWeapon.GetSlow().IsValid();
            if (tgtUnit.immuneToSlow) slowed = false;

            if (srcWeapon.GetDot().GetTotalDamage() > 0) dotted = true;


            if (stunned) stun = srcWeapon.GetStun().Clone();
            if (slowed) slow = srcWeapon.GetSlow().Clone();
            if (dotted) dot = srcWeapon.GetDot().Clone();
        }

        //stats processing for attack from unit (tower/creep)
        public void Process_SrcUnit()
        {
            if (srcUnit.GetHit() <= 0)
            {
                Debug.LogWarning("Attacking unit (" + srcUnit.unitName + ") has default hitChance of 0%, is this intended?", srcUnit);
            }

            float hitChance = Mathf.Clamp(srcUnit.GetHit() - tgtUnit.GetDodge(), 0, 1);
            if (Random.Range(0f, 1f) > hitChance)
            {
                missed = true;
                return;
            }

            if (srcUnit.GetInstantKill().IsApplicable(tgtUnit.HP, tgtUnit.fullHP))
            {
                damage = tgtUnit.HP;
                damageHP = tgtUnit.HP;
                instantKill = true;
                destroy = true;
                return;
            }


            damage = Random.Range(srcUnit.GetDamageMin(), srcUnit.GetDamageMax());
            damage /= (float)srcUnit.GetShootPointCount();  //divide the damage by number of shootPoint


            float critChance = srcUnit.GetCritChance();
            if (tgtUnit.immuneToCrit) critChance = -1f;
            if (Random.Range(0f, 1f) < critChance)
            {
                critical = true;
                damage *= srcUnit.GetCritMultiplier();
                new TextOverlay(impactPoint, "Critical", new Color(1f, .6f, 0f, 1f));
            }

            float dmgModifier = DamageTable.GetModifier(tgtUnit.armorType, srcUnit.damageType);
            damage *= dmgModifier;

            if (damage >= tgtUnit.shield)
            {
                damageShield = tgtUnit.shield;
                damageHP = damage - tgtUnit.shield;
            }
            else
            {
                damageShield = damage;
                damageHP = 0;
            }


            if (Random.Range(0f, 1f) < srcUnit.GetShieldPierce() && damageShield > 0)
            {
                damageHP += damageShield;
                damageShield = 0;
                pierceShield = true;
                new TextOverlay(impactPoint, "Shield Pierced", new Color(0f, 1f, 1f, 1f));
            }
            if (srcUnit.DamageShieldOnly()) damageHP = 0;

            if (damageHP >= tgtUnit.HP)
            {
                destroy = true;
                return;
            }

            if (Random.Range(0f, 1f) < srcUnit.GetShieldBreak() && tgtUnit.fullShield > 0) breakShield = true;

            stunned = srcUnit.GetStun().IsApplicable();
            if (tgtUnit.immuneToStun) stunned = false;

            slowed = srcUnit.GetSlow().IsValid();
            if (tgtUnit.immuneToSlow) slowed = false;

            if (srcUnit.GetDot().GetTotalDamage() > 0) dotted = true;

            if (stunned) stun = srcUnit.GetStun().Clone();
            if (slowed) slow = srcUnit.GetSlow().Clone();
            if (dotted) dot = srcUnit.GetDot().Clone();
        }

        //clone an instance
        public AttackInstance Clone()
        {
            AttackInstance attInstance = new AttackInstance();

            attInstance.processed = processed;
            attInstance.srcWeapon = srcWeapon;
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