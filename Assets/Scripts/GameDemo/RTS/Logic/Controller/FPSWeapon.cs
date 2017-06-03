using UnitedSolution;using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	public class FPSWeapon : MonoBehaviour {

		public int prefabID=0;
		public string weaponName="Weapon";
		public Sprite icon;
		
		public int currentActiveStat=0;
		public List<UnitStat> stats=new List<UnitStat>(){ new UnitStat() };
		
		public int damageType=0;
		
		public float recoil=1;
		
		private float currentCD=0;
		private int currentAmmo=10;
		public int GetCurrentAmmo(){ return currentAmmo; }
		
		public List<Transform> shootPoints=new List<Transform>();
		public int GetShootPointCount(){ return shootPoints.Count; }
		
		public string desp="";
		
		
		
		void Awake(){
			currentAmmo=GetClipSize();
		}
		
		public bool ReadyToFire(){
			if(IsOnCooldown()) return false;
			if(OutOfAmmo()) return false;
			return true;
		}
		
		public bool Shoot(){
			if(IsReloading()) return false;
			if(IsOnCooldown()) return false;
			if(OutOfAmmo()){
				StartCoroutine(ReloadRoutine());
				return false;
			}
			
			StartCoroutine(CooldownRoutine());
			
			currentAmmo-=1;
			if(OutOfAmmo()) StartCoroutine(ReloadRoutine());
			
			return true;
		}
		public void Reload(){ 
			if(currentAmmo==GetClipSize()) return;
			StartCoroutine(ReloadRoutine());
		}
		
		public IEnumerator CooldownRoutine(){
			currentCD=GetCooldown();
			while(currentCD>0){
				currentCD-=Time.fixedDeltaTime;
				yield return new WaitForSeconds(Time.fixedDeltaTime);
			}
		}
		
		private float reloadDuration=0;
		public IEnumerator ReloadRoutine(){
			FPSControl.StartReload(this);
			
			reloadDuration=GetReloadDuration();
			while(reloadDuration>0){
				reloadDuration-=Time.deltaTime;
				yield return null;
			}
			currentAmmo=GetClipSize();
			
			FPSControl.ReloadComplete(this);
		}
		
		
		
		
		public bool IsOnCooldown(){ return currentCD>0 ? true : false;}
		public bool OutOfAmmo(){ return currentAmmo<=0 ? true : false; }
		public bool IsReloading(){ return reloadDuration>0 ? true : false; }
		
		
		
		
		public float GetDamageMin(){ return Mathf.Max(0, stats[currentActiveStat].damageMin * (1+PerkManager.GetFPSWeaponDamage(prefabID))); }
		public float GetDamageMax(){ return Mathf.Max(0, stats[currentActiveStat].damageMax * (1+PerkManager.GetFPSWeaponDamage(prefabID))); }
		public float GetCooldown(){ return Mathf.Max(0.05f, stats[currentActiveStat].cooldown * (1+PerkManager.GetFPSWeaponCD(prefabID))); }
		public int GetClipSize(){ return (int)(stats[currentActiveStat].clipSize * (1+PerkManager.GetFPSWeaponClipSize(prefabID))); }
		public float GetReloadDuration(){ return Mathf.Max(0.05f, stats[currentActiveStat].reloadDuration * (1+PerkManager.GetFPSWeaponReloadDuration(prefabID))); }
		public float GetAOERange(){ return stats[currentActiveStat].aoeRadius * (1+PerkManager.GetFPSWeaponAOERadius(prefabID)); }
		
		public float GetCritChance(){ return stats[currentActiveStat].crit.chance + PerkManager.GetFPSWeaponDamage(prefabID); }
		public float GetCritMultiplier(){ return stats[currentActiveStat].crit.dmgMultiplier + PerkManager.GetFPSWeaponDamage(prefabID); }
		
		public float GetShieldBreak(){ return stats[currentActiveStat].shieldBreak+ PerkManager.GetFPSWeaponShieldBreak(prefabID); }
		public float GetShieldPierce(){ return stats[currentActiveStat].shieldPierce + PerkManager.GetFPSWeaponShieldPierce(prefabID); }
		public bool DamageShieldOnly(){ return stats[currentActiveStat].damageShieldOnly; }
		
		public Stun GetStun(){ return PerkManager.ModifyStunWithPerkBonus(stats[currentActiveStat].stun.Clone(), prefabID, 1); }	//pass 1 to indicate this is for FPSWeapon
		public Slow GetSlow(){ return PerkManager.ModifySlowWithPerkBonus(stats[currentActiveStat].slow.Clone(), prefabID, 1); }
		public Dot GetDot(){ return PerkManager.ModifyDotWithPerkBonus(stats[currentActiveStat].dot.Clone(), prefabID, 1); }
		public InstantKill GetInstantKill(){ return PerkManager.ModifyInstantKillWithPerkBonus(stats[currentActiveStat].instantKill.Clone(), prefabID, 1); }
		
		
		
		public Transform GetShootObject(){ return stats[currentActiveStat].ShootObject.transform; }
	}

}