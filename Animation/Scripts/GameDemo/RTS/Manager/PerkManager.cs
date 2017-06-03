using UnitedSolution;using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	public class PerkManager : MonoBehaviour {
		
		public delegate void UpdatePerkPointHandler(int point);
		public static event UpdatePerkPointHandler onPerkPointE;
		
		public delegate void PerkPurchasedHandler(Perk perk);
		public static event PerkPurchasedHandler onPerkPurchasedE;
		
		
		private int perkPoint=0;
		public static int GetPerkPoint(){ return instance.perkPoint; }
		
		public List<int> unavailableIDList=new List<int>(); 	//ID list of perk available for this level, modified in editor
		public List<int> purchasedIDList=new List<int>(); 	//ID list of perk pre-purcahsed for this level, modified in editor
		public List<Perk> perkList=new List<Perk>();			//actual perk list, filled in runtime based on unavailableIDList
		
		public static List<Perk> GetPerkList(){ return instance.perkList; }
		
		public static PerkManager instance;
		
		public static bool IsOn(){ return instance==null ? false : true; }
		
		public void Awake(){
			if(init){
				Destroy(gameObject);
				return;
			}
		}
		
		private bool init=false;
		public void Init(){
			if(init) return;
			
			init=true;
			
			if(instance!=null){
				Destroy(gameObject);
				return;
			}
			
			instance=this;
			
			//load your custom/saved purchasedIDList here if you are doing a persistent perk system
			
			List<Perk> dbList=PerkDB.Load();
			for(int i=0; i<dbList.Count; i++){
				if(!unavailableIDList.Contains(dbList[i].ID)){
					Perk perk=dbList[i].Clone();
					perkList.Add(perk);
				}
			}
			
			globalTowerModifier=new PerkTowerModifier();
			globalAbilityModifier=new PerkAbilityModifier();
			globalFPSWeaponModifier=new PerkFPSWeaponModifier();
			
			emptyTowerModifier=new PerkTowerModifier();
			emptyAbilityModifier=new PerkAbilityModifier();
			emptyFPSWeaponModifier=new PerkFPSWeaponModifier();
			
			int rscCount=ResourceManager.GetResourceCount();
			for(int i=0; i<rscCount; i++){
				rscRegen.Add(0);
				rscGain.Add(0);
				rscCreepKilledGain.Add(0);
				rscWaveClearedGain.Add(0);
				rscRscTowerGain.Add(0);
			}
			
			for(int i=0; i<perkList.Count; i++){
				if(purchasedIDList.Contains(perkList[i].ID)){
					if(perkList[i].type==_PerkType.NewTower || perkList[i].type==_PerkType.NewAbility || perkList[i].type==_PerkType.NewFPSWeapon){
						StartCoroutine(DelayPurchasePerk(perkList[i]));
					}
					else _PurchasePerk(perkList[i], false);	//dont use rsc since these are pre-purchased perk
				}
			}
			
			if(persistantProgress){
				transform.parent=null;
				DontDestroyOnLoad(gameObject);
			}
		}
		
		public bool persistantProgress=false;
		void OnDestroy(){ 
			if(!persistantProgress) instance=null;
		}
		
		
		
		
		//for initiating perk which add new tower & ability,
		//let everything else (UI) settle in first before adding the new element 
		IEnumerator DelayPurchasePerk(Perk perk){
			yield return null;
			_PurchasePerk(perk, false);
		}
		
		
		
		
		public static Perk GetPerk(int perkID){ return instance._GetPerk(perkID); }
		public Perk _GetPerk(int perkID){
			for(int i=0; i<perkList.Count; i++){ if(perkList[i].ID==perkID) return perkList[i];}
			return null;
		}
		public static string IsPerkAvailable(int perkID){ return instance._IsPerkAvailable(perkID); }
		public string _IsPerkAvailable(int perkID){
			for(int i=0; i<perkList.Count; i++){ if(perkList[i].ID==perkID) return perkList[i].IsAvailable();}
			return "PerkID doesnt correspond to any perk in the list   "+perkID;
		}
		public static bool IsPerkPurchased(int perkID){ return instance._IsPerkPurchased(perkID); }
		public bool _IsPerkPurchased(int perkID){
			for(int i=0; i<perkList.Count; i++){ if(perkList[i].ID==perkID) return perkList[i].purchased;}
			return false;
		}
		
		
		
		
		
		public static string PurchasePerk(int perkID, bool useRsc=true){ return instance._PurchasePerk(perkID, useRsc); }
		public string _PurchasePerk(int perkID, bool useRsc=true){ 
			for(int i=0; i<perkList.Count; i++){ if(perkList[i].ID==perkID) return instance._PurchasePerk(perkList[i], useRsc); }
			return "PerkID doesnt correspond to any perk in the list";
		}
		
		public static string PurchasePerk(Perk perk, bool useRsc=true){ return instance._PurchasePerk(perk, useRsc); }
		
		public string _PurchasePerk(Perk perk, bool useRsc=true){
			string text=perk.Purchase(useRsc);
			if(text!="") return text;
			
			if(onPerkPurchasedE!=null) onPerkPurchasedE(perk);
			
			//process the prereq for other perk
			for(int i=0; i<perkList.Count; i++){
				Perk perkTemp=perkList[i];
				if(perkTemp.purchased || perkTemp.prereq.Count==0) continue;
				perkTemp.prereq.Remove(perk.ID);
			}
			
			
			perkPoint+=1;
			if(onPerkPointE!=null) onPerkPointE(perkPoint);
			
			if(perk.type==_PerkType.NewTower){ 
				List<UnitTower> towerList=TowerDB.Load();
				for(int i=0; i<towerList.Count; i++){
					if(towerList[i].prefabID==perk.itemIDList[0]){
						unlockedTower.Add(towerList[i]);
						BuildManager.AddNewTower(towerList[i]);
					}
				}
			}
			else if(perk.type==_PerkType.NewAbility){
				List<Ability> abilityList=AbilityDB.Load();
				for(int i=0; i<abilityList.Count; i++){
					if(abilityList[i].ID==perk.itemIDList[0]){
						unlockedAbility.Add(abilityList[i]);
						AbilityManager.AddNewAbility(abilityList[i]);
					}
				}
			}
			else if(perk.type==_PerkType.NewFPSWeapon){
				List<FPSWeapon> FPSWeaponList=FPSWeaponDB.Load();
				for(int i=0; i<FPSWeaponList.Count; i++){
					if(FPSWeaponList[i].prefabID==perk.itemIDList[0]){
						unlockedWeapon.Add(FPSWeaponList[i]);
						FPSControl.AddNewWeapon(FPSWeaponList[i]);
					}
				}
			}
			
			else if(perk.type==_PerkType.GainLife){ GameControl.GainLife((int)Random.Range(perk.value, perk.valueAlt)); }
			else if(perk.type==_PerkType.LifeCap){ lifeCap+=(int)perk.value; GameControl.GainLife(0); }
			else if(perk.type==_PerkType.LifeRegen){ lifeRegen+=perk.value; }
			else if(perk.type==_PerkType.LifeWaveClearedBonus){ lifeWaveClearedBonus+=(int)perk.value; }
			
			else if(perk.type==_PerkType.GainRsc){ 
				List<int> valueList=new List<int>();
				for(int i=0; i<perk.valueRscList.Count; i++) valueList.Add((int)perk.valueRscList[i]);
				ResourceManager.GainResource(valueList, null, false);	//dont pass multiplier and dont use multiplier
			}
			else if(perk.type==_PerkType.RscRegen){ 
				for(int i=0; i<perk.valueRscList.Count; i++) rscRegen[i]+=perk.valueRscList[i];
			}
			else if(perk.type==_PerkType.RscGain){ 
				for(int i=0; i<perk.valueRscList.Count; i++) rscGain[i]+=perk.valueRscList[i];
			}
			else if(perk.type==_PerkType.RscCreepKilledGain){ 
				for(int i=0; i<perk.valueRscList.Count; i++) rscCreepKilledGain[i]+=perk.valueRscList[i];
			}
			else if(perk.type==_PerkType.RscWaveClearedGain){ 
				for(int i=0; i<perk.valueRscList.Count; i++) rscWaveClearedGain[i]+=perk.valueRscList[i];
			}
			else if(perk.type==_PerkType.RscResourceTowerGain){ 
				for(int i=0; i<perk.valueRscList.Count; i++) rscRscTowerGain[i]+=perk.valueRscList[i];
			}
			
			else if(perk.type==_PerkType.Tower){ ModifyTowerModifier(globalTowerModifier, perk); }
			else if(perk.type==_PerkType.TowerSpecific){ 
				for(int i=0; i<perk.itemIDList.Count; i++){
					int ID=TowerModifierExist(perk.itemIDList[i]);
					if(ID==-1){
						PerkTowerModifier towerModifier=new PerkTowerModifier();
						towerModifier.prefabID=perk.itemIDList[i];
						towerModifierList.Add(towerModifier);
						ID=towerModifierList.Count-1;
					}
					ModifyTowerModifierInList(ID, perk);
				}
			}
			else if(perk.type==_PerkType.Ability){ ModifyAbilityModifier(globalAbilityModifier, perk); }
			else if(perk.type==_PerkType.AbilitySpecific){ 
				for(int i=0; i<perk.itemIDList.Count; i++){
					int ID=AbilityModifierExist(perk.itemIDList[i]);
					if(ID==-1){
						PerkAbilityModifier abilityModifier=new PerkAbilityModifier();
						abilityModifier.abilityID=perk.itemIDList[i];
						abilityModifierList.Add(abilityModifier);
						ID=abilityModifierList.Count-1;
					}
					ModifyAbilityModifierInList(ID, perk);
				}
			}
			else if(perk.type==_PerkType.FPSWeapon){ ModifyFPSWeaponModifier(globalFPSWeaponModifier, perk); }
			else if(perk.type==_PerkType.FPSWeaponSpecific){ 
				for(int i=0; i<perk.itemIDList.Count; i++){
					int ID=FPSWeaponModifierExist(perk.itemIDList[i]);
					if(ID==-1){
						PerkFPSWeaponModifier weaponModifier=new PerkFPSWeaponModifier();
						weaponModifier.prefabID=perk.itemIDList[i];
						FPSWeaponModifierList.Add(weaponModifier);
						ID=FPSWeaponModifierList.Count-1;
					}
					ModifyFPSWeaponModifierInList(ID, perk);
				}
			}
			
			else if(perk.type==_PerkType.EnergyRegen){ energyRegen+=perk.value; }
			else if(perk.type==_PerkType.EnergyIncreaseCap){ energyCap+=perk.value; }
			else if(perk.type==_PerkType.EnergyCreepKilledBonus){ energyCreepKilledBonus+=perk.value; }
			else if(perk.type==_PerkType.EnergyWaveClearedBonus){ energyWaveClearedBonus+=perk.value; }
			
			return "";
		}
		
		
		
		
		private int TowerModifierExist(int prefabID){
			for(int i=0; i<towerModifierList.Count; i++){ if(towerModifierList[i].prefabID==prefabID) return i; }
			return -1;
		}
		private void ModifyTowerModifierInList(int ID, Perk perk){ ModifyTowerModifier(towerModifierList[ID], perk); }
		private void ModifyTowerModifier(PerkTowerModifier towerModifier, Perk perk){
			towerModifier.HP+=perk.HP;
			towerModifier.HPRegen+=perk.HPRegen;
			towerModifier.HPStagger+=perk.HPStagger;
			towerModifier.shield+=perk.shield;
			towerModifier.shieldRegen+=perk.shieldRegen;
			towerModifier.shieldStagger+=perk.shieldStagger;
			towerModifier.buildCost+=perk.buildCost;
			towerModifier.upgradeCost+=perk.upgradeCost;
			ModifyUnitStats(towerModifier.stats, perk.stats);
		}
		
		
		
		private int AbilityModifierExist(int abilityID){
			for(int i=0; i<abilityModifierList.Count; i++){ if(abilityModifierList[i].abilityID==abilityID) return i; }
			return -1;
		}
		private void ModifyAbilityModifierInList(int ID, Perk perk){ ModifyAbilityModifier(abilityModifierList[ID], perk);}
		private void ModifyAbilityModifier(PerkAbilityModifier abilityModifier, Perk perk){
			abilityModifier.cost=perk.abCost;
			abilityModifier.cooldown=perk.abCooldown;
			abilityModifier.aoeRadius=perk.abAOERadius;
			
			abilityModifier.effects.damageMin+=perk.effects.damageMin;
			abilityModifier.effects.damageMax+=perk.effects.damageMax;
			abilityModifier.effects.stunChance+=perk.effects.stunChance;
			
			abilityModifier.effects.slow.duration+=perk.effects.duration;
			abilityModifier.effects.slow.slowMultiplier+=perk.effects.slow.slowMultiplier;
			
			abilityModifier.effects.dot.duration+=perk.effects.duration;
			abilityModifier.effects.dot.interval+=perk.effects.dot.interval;
			abilityModifier.effects.dot.value+=perk.effects.dot.value;
			
			abilityModifier.effects.damageBuff+=perk.effects.damageBuff;
			abilityModifier.effects.rangeBuff+=perk.effects.rangeBuff;
			abilityModifier.effects.cooldownBuff+=perk.effects.cooldownBuff;
			abilityModifier.effects.HPGainMin+=perk.effects.HPGainMin;
			abilityModifier.effects.HPGainMax+=perk.effects.HPGainMax;
		}
		
		
		
		private int FPSWeaponModifierExist(int prefabID){
			for(int i=0; i<FPSWeaponModifierList.Count; i++){ if(FPSWeaponModifierList[i].prefabID==prefabID) return i; }
			return -1;
		}
		private void ModifyFPSWeaponModifierInList(int ID, Perk perk){ ModifyUnitStats(FPSWeaponModifierList[ID].stats, perk.stats); }
		private void ModifyFPSWeaponModifier(PerkFPSWeaponModifier weaponModifier, Perk perk){ ModifyUnitStats(weaponModifier.stats, perk.stats); }
		
		
		private void ModifyUnitStats(UnitStat tgtStats, UnitStat srcStats){
			tgtStats.damageMin+=srcStats.damageMin;
			tgtStats.cooldown+=srcStats.cooldown;
			tgtStats.clipSize+=srcStats.clipSize;
			tgtStats.reloadDuration+=srcStats.reloadDuration;
			tgtStats.attackRange+=srcStats.attackRange;
			tgtStats.aoeRadius+=srcStats.aoeRadius;
			tgtStats.hit+=srcStats.hit;
			tgtStats.dodge+=srcStats.dodge;
			tgtStats.shieldBreak+=srcStats.shieldBreak;
			tgtStats.shieldPierce+=srcStats.shieldPierce;
			
			tgtStats.crit.chance+=srcStats.crit.chance;
			tgtStats.crit.dmgMultiplier+=srcStats.crit.dmgMultiplier;
			
			tgtStats.stun.chance+=srcStats.stun.chance;
			tgtStats.stun.duration+=srcStats.stun.duration;
			
			tgtStats.slow.duration+=srcStats.slow.duration;
			tgtStats.slow.slowMultiplier+=srcStats.slow.slowMultiplier;
			
			tgtStats.dot.duration+=srcStats.dot.duration;
			tgtStats.dot.interval+=srcStats.dot.interval;
			tgtStats.dot.value+=srcStats.dot.value;
			
			tgtStats.instantKill.chance+=srcStats.instantKill.chance;
			tgtStats.instantKill.HPThreshold+=srcStats.instantKill.HPThreshold;
		}
		
		
		
		
		
		
		
		
		
//************************************************************************************************************************************
//modifiers goes here		
		
		//unlocked item, used in persistent mode
		private List<UnitTower> unlockedTower=new List<UnitTower>();
		private List<Ability> unlockedAbility=new List<Ability>();
		private List<FPSWeapon> unlockedWeapon=new List<FPSWeapon>();
		
		public static List<UnitTower> GetUnlockedTower(){ return instance==null ? new List<UnitTower>() : instance.unlockedTower; }
		public static List<Ability> GetUnlockedAbility(){ return instance==null ? new List<Ability>() : instance.unlockedAbility; }
		public static List<FPSWeapon> GetUnlockedWeapon(){ return instance==null ? new List<FPSWeapon>() : instance.unlockedWeapon; }
		
		public int lifeCap=0;
		public float lifeRegen=0;
		public int lifeWaveClearedBonus=0;	//bonus modifier when a wave is cleared
		
		public static int GetLifeCapModifier(){ return instance==null ? 0 : instance.lifeCap; }
		public static float GetLifeRegenModifier(){ return instance==null ? 0 : instance.lifeRegen; }
		public static int GetLifeWaveClearedModifier(){ return instance==null ? 0 : instance.lifeWaveClearedBonus; }
		
		
		public List<float> rscRegen=new List<float>();
		public List<float> rscGain=new List<float>();
		public List<float> rscCreepKilledGain=new List<float>();
		public List<float> rscWaveClearedGain=new List<float>();
		public List<float> rscRscTowerGain=new List<float>();
		
		public static List<float> GetRscRegen(){ return instance==null ? new List<float>() : instance.rscRegen; }
		public static List<float> GetRscGain(){ return instance==null ? new List<float>() : instance.rscGain; }
		public static List<float> GetRscCreepKilled(){ return instance==null ? new List<float>() : instance.rscCreepKilledGain; }
		public static List<float> GetRscWaveKilled(){ return instance==null ? new List<float>() : instance.rscWaveClearedGain; }
		public static List<float> GetRscTowerGain(){ return instance==null ? new List<float>() : instance.rscRscTowerGain; }
		
		
		
		public float energyRegen=0;
		public float energyCap=0;
		public float energyCreepKilledBonus=0;		//bonus modifier when a creep is killed
		public float energyWaveClearedBonus=0;	//bonus modifier when a wave is cleared
		
		public static float GetEnergyRegenModifier(){ return instance==null ? 0 : instance.energyRegen; }
		public static float GetEnergyCapModifier(){ return instance==null ? 0 : instance.energyCap; }
		public static float GetEnergyCreepKilledModifier(){ return instance==null ? 0 : instance.energyCreepKilledBonus; }
		public static float GetEnergyWaveClearedModifier(){ return instance==null ? 0 : instance.energyWaveClearedBonus; }
		
		
		
		
		
		public PerkTowerModifier emptyTowerModifier;
		public PerkTowerModifier globalTowerModifier;
		public List<PerkTowerModifier> towerModifierList=new List<PerkTowerModifier>();
		
		public static PerkTowerModifier GetTowerModifier(int prefabID){
			for(int i=0; i<instance.towerModifierList.Count; i++){
				if(instance.towerModifierList[i].prefabID==prefabID) return instance.towerModifierList[i];
			}
			return instance.emptyTowerModifier;
		}
		
		
		public static float GetTowerHP(int prefabID){
			if(instance==null) return 0;
			return instance.globalTowerModifier.HP+GetTowerModifier(prefabID).HP;
		}
		public static float GetTowerHPRegen(int prefabID){
			if(instance==null) return 0;
			return instance.globalTowerModifier.HPRegen+GetTowerModifier(prefabID).HPRegen;
		}
		public static float GetTowerHPStagger(int prefabID){
			if(instance==null) return 0;
			return instance.globalTowerModifier.HPStagger+GetTowerModifier(prefabID).HPStagger;
		}
		public static float GetTowerShield(int prefabID){
			if(instance==null) return 0;
			return instance.globalTowerModifier.shield+GetTowerModifier(prefabID).shield;
		}
		public static float GetTowerShieldRegen(int prefabID){
			if(instance==null) return 0;
			return instance.globalTowerModifier.shieldRegen+GetTowerModifier(prefabID).shieldRegen;
		}
		public static float GetTowerShieldStagger(int prefabID){
			if(instance==null) return 0;
			return instance.globalTowerModifier.shieldStagger+GetTowerModifier(prefabID).shieldStagger;
		}
		
		public static float GetTowerBuildCost(int prefabID){
			if(instance==null) return 0;
			return instance.globalTowerModifier.buildCost+GetTowerModifier(prefabID).buildCost;
		}
		public static float GetTowerUpgradeCost(int prefabID){
			if(instance==null) return 0;
			return instance.globalTowerModifier.upgradeCost+GetTowerModifier(prefabID).upgradeCost;
		}
		
		
		public static float GetTowerDamage(int prefabID){
			if(instance==null) return 0;
			return (instance.globalTowerModifier.stats.damageMin+GetTowerModifier(prefabID).stats.damageMin);
		}
		public static float GetTowerCD(int prefabID){
			if(instance==null) return 0;
			return (instance.globalTowerModifier.stats.cooldown+GetTowerModifier(prefabID).stats.cooldown);
		}
		public static float GetTowerClipSize(int prefabID){
			if(instance==null) return 0;
			return (instance.globalTowerModifier.stats.clipSize+GetTowerModifier(prefabID).stats.clipSize);
		}
		public static float GetTowerReloadDuration(int prefabID){
			if(instance==null) return 0;
			return (instance.globalTowerModifier.stats.reloadDuration+GetTowerModifier(prefabID).stats.reloadDuration);
		}
		public static float GetTowerRange(int prefabID){
			if(instance==null) return 0;
			return (instance.globalTowerModifier.stats.attackRange+GetTowerModifier(prefabID).stats.attackRange);
		}
		public static float GetTowerAOERadius(int prefabID){
			if(instance==null) return 0;
			return (instance.globalTowerModifier.stats.aoeRadius+GetTowerModifier(prefabID).stats.aoeRadius);
		}
		public static float GetTowerHit(int prefabID){
			if(instance==null) return 0;
			return instance.globalTowerModifier.stats.hit+GetTowerModifier(prefabID).stats.hit;
		}
		public static float GetTowerDodge(int prefabID){
			if(instance==null) return 0;
			return instance.globalTowerModifier.stats.dodge+GetTowerModifier(prefabID).stats.dodge;
		}
		public static float GetTowerCritChance(int prefabID){
			if(instance==null) return 0;
			return instance.globalTowerModifier.stats.crit.chance+GetTowerModifier(prefabID).stats.crit.chance;
		}
		public static float GetTowerCritMultiplier(int prefabID){
			if(instance==null) return 0;
			return (instance.globalTowerModifier.stats.crit.dmgMultiplier+GetTowerModifier(prefabID).stats.crit.dmgMultiplier);
		}
		
		public static float GetTowerShieldBreakMultiplier(int prefabID){
			if(instance==null) return 0;
			return (instance.globalTowerModifier.stats.shieldBreak+GetTowerModifier(prefabID).stats.shieldBreak);
		}
		public static float GetTowerShieldPierceMultiplier(int prefabID){
			if(instance==null) return 0;
			return (instance.globalTowerModifier.stats.shieldPierce+GetTowerModifier(prefabID).stats.shieldPierce);
		}
		
		public static Stun GetTowerStunMultiplier(int prefabID){
			if(instance==null) return new Stun(0, 0);
			Stun stunG=instance.globalTowerModifier.stats.stun;
			Stun stunT=GetTowerModifier(prefabID).stats.stun;
			return new Stun(stunG.chance+stunT.chance, stunG.duration+stunT.duration);
		}
		public static Slow GetTowerSlowMultiplier(int prefabID){
			if(instance==null) return new Slow(0, 0);
			Slow slowG=instance.globalTowerModifier.stats.slow;
			Slow slowT=GetTowerModifier(prefabID).stats.slow;
			return new Slow(slowG.slowMultiplier+slowT.slowMultiplier, slowG.duration+slowT.duration);
		}
		public static Dot GetTowerDotMultiplier(int prefabID){
			if(instance==null) return new Dot(0, 0, 0);
			Dot dotG=instance.globalTowerModifier.stats.dot;
			Dot dotT=GetTowerModifier(prefabID).stats.dot;
			return new Dot(dotG.duration+dotT.duration, dotG.interval+dotT.interval, dotG.value+dotT.value);
		}
		public static InstantKill GetTowerInstantKillMultiplier(int prefabID){
			if(instance==null) return new InstantKill(0, 0);
			InstantKill killG=instance.globalTowerModifier.stats.instantKill;
			InstantKill killT=GetTowerModifier(prefabID).stats.instantKill;
			return new InstantKill(killG.chance+killT.chance, killG.HPThreshold+killT.HPThreshold);
		}
		
		
		
		
		
		//shared among tower, ability & fpsWeapon. 0-tower, 1-fpsWeapon, 2-ability
		public static Stun ModifyStunWithPerkBonus(Stun stun, int prefabID, int type=0){	
			Stun stunMod=new Stun();
			if(type==0) stunMod=GetTowerStunMultiplier(prefabID);
			else if(type==1) stunMod=GetFPSWeaponStunMultiplier(prefabID);
			stun.chance*=(1+stunMod.chance);
			stun.duration*=(1+stunMod.duration);
			return stun;
		}
		public static Slow ModifySlowWithPerkBonus(Slow slow, int prefabID, int type=0){
			Slow slowMod=new Slow();
			if(type==0) slowMod=GetTowerSlowMultiplier(prefabID);
			else if(type==1) slowMod=GetFPSWeaponSlowMultiplier(prefabID);
			else if(type==2) slowMod=GetAbilitySlowMultiplier(prefabID);
			slow.slowMultiplier*=(1-slowMod.slowMultiplier);
			slow.duration*=(1+slowMod.duration);
			return slow;
		}
		public static Dot ModifyDotWithPerkBonus(Dot dot, int prefabID, int type=0){
			Dot dotMod=new Dot();
			if(type==0) dotMod=GetTowerDotMultiplier(prefabID);
			else if(type==1) dotMod=GetFPSWeaponDotMultiplier(prefabID);
			else if(type==2) dotMod=GetAbilityDotMultiplier(prefabID);
			dot.duration*=(1+dotMod.duration);
			//dot.interval*=(1+dotMod.interval);
			dot.value*=(1+dotMod.value);
			return dot;
		}
		public static InstantKill ModifyInstantKillWithPerkBonus(InstantKill instKill, int prefabID, int type=0){
			InstantKill ikMod=new InstantKill();
			if(type==0) ikMod=GetTowerInstantKillMultiplier(prefabID);
			else if(type==1) ikMod=GetFPSWeaponInstantKillMultiplier(prefabID);
			instKill.chance*=(1+ikMod.chance);
			instKill.HPThreshold*=(1+ikMod.HPThreshold);
			return instKill;
		}
		
		
		
		
		
		public PerkFPSWeaponModifier emptyFPSWeaponModifier;
		public PerkFPSWeaponModifier globalFPSWeaponModifier;
		public List<PerkFPSWeaponModifier> FPSWeaponModifierList=new List<PerkFPSWeaponModifier>();
		
		public static PerkFPSWeaponModifier GetFPSWeaponModifier(int prefabID){
			for(int i=0; i<instance.FPSWeaponModifierList.Count; i++){
				if(instance.FPSWeaponModifierList[i].prefabID==prefabID) return instance.FPSWeaponModifierList[i];
			}
			return instance.emptyFPSWeaponModifier;
		}
		
		public static float GetFPSWeaponDamage(int prefabID){
			if(instance==null) return 0;
			return (instance.globalFPSWeaponModifier.stats.damageMin+GetFPSWeaponModifier(prefabID).stats.damageMin);
		}
		public static float GetFPSWeaponCD(int prefabID){
			if(instance==null) return 0;
			return (instance.globalFPSWeaponModifier.stats.cooldown+GetFPSWeaponModifier(prefabID).stats.cooldown);
		}
		public static float GetFPSWeaponClipSize(int prefabID){
			if(instance==null) return 0;
			return (instance.globalFPSWeaponModifier.stats.clipSize+GetFPSWeaponModifier(prefabID).stats.clipSize);
		}
		public static float GetFPSWeaponReloadDuration(int prefabID){
			if(instance==null) return 0;
			return (instance.globalFPSWeaponModifier.stats.reloadDuration+GetFPSWeaponModifier(prefabID).stats.reloadDuration);
		}
		public static float GetFPSWeaponAOERadius(int prefabID){
			if(instance==null) return 0;
			return (instance.globalFPSWeaponModifier.stats.aoeRadius+GetFPSWeaponModifier(prefabID).stats.aoeRadius);
		}
		public static float GetFPSWeaponShieldBreak(int prefabID){
			if(instance==null) return 0;
			return instance.globalFPSWeaponModifier.stats.shieldBreak+GetFPSWeaponModifier(prefabID).stats.shieldBreak;
		}
		public static float GetFPSWeaponShieldPierce(int prefabID){
			if(instance==null) return 0;
			return (instance.globalFPSWeaponModifier.stats.shieldPierce+GetFPSWeaponModifier(prefabID).stats.shieldPierce);
		}
		
		public static float GetFPSWeaponCritChance(int prefabID){
			if(instance==null) return 0;
			return instance.globalFPSWeaponModifier.stats.crit.chance+GetFPSWeaponModifier(prefabID).stats.crit.chance;
		}
		public static float GetFPSWeaponCritMultiplier(int prefabID){
			if(instance==null) return 0;
			return (instance.globalFPSWeaponModifier.stats.crit.dmgMultiplier+GetFPSWeaponModifier(prefabID).stats.crit.dmgMultiplier);
		}
		
		public static Stun GetFPSWeaponStunMultiplier(int prefabID){
			if(instance==null) return new Stun(0, 0);
			Stun stunG=instance.globalFPSWeaponModifier.stats.stun;
			Stun stunT=GetFPSWeaponModifier(prefabID).stats.stun;
			return new Stun(stunG.chance+stunT.chance, stunG.duration+stunT.duration);
		}
		public static Slow GetFPSWeaponSlowMultiplier(int prefabID){
			if(instance==null) return new Slow(0, 0);
			Slow slowG=instance.globalFPSWeaponModifier.stats.slow;
			Slow slowT=GetFPSWeaponModifier(prefabID).stats.slow;
			return new Slow(slowG.slowMultiplier+slowT.slowMultiplier, slowG.duration+slowT.duration);
		}
		public static Dot GetFPSWeaponDotMultiplier(int prefabID){
			if(instance==null) return new Dot(0, 0, 0);
			Dot dotG=instance.globalFPSWeaponModifier.stats.dot;
			Dot dotT=GetFPSWeaponModifier(prefabID).stats.dot;
			return new Dot(dotG.duration+dotT.duration, dotG.interval+dotT.interval, dotG.value+dotT.value);
		}
		public static InstantKill GetFPSWeaponInstantKillMultiplier(int prefabID){
			if(instance==null) return new InstantKill(0, 0);
			InstantKill killG=instance.globalFPSWeaponModifier.stats.instantKill;
			InstantKill killT=GetFPSWeaponModifier(prefabID).stats.instantKill;
			return new InstantKill(killG.chance+killT.chance, killG.HPThreshold+killT.HPThreshold);
		}
		
		
		
		
		
		
		
		
		
		
		
		public PerkAbilityModifier emptyAbilityModifier;
		public PerkAbilityModifier globalAbilityModifier;
		public List<PerkAbilityModifier> abilityModifierList=new List<PerkAbilityModifier>();
		
		public static PerkAbilityModifier GetAbilityModifier(int prefabID){
			for(int i=0; i<instance.abilityModifierList.Count; i++){
				if(instance.abilityModifierList[i].abilityID==prefabID) return instance.abilityModifierList[i];
			}
			return instance.emptyAbilityModifier;
		}
		
		
		public static float GetAbilityCost(int abilityID){
			if(instance==null) return 0;
			return instance.globalAbilityModifier.cost+GetAbilityModifier(abilityID).cost;
		}
		public static float GetAbilityCooldown(int abilityID){
			if(instance==null) return 0;
			return instance.globalAbilityModifier.cooldown+GetAbilityModifier(abilityID).cooldown;
		}
		public static float GetAbilityAOERadius(int abilityID){
			if(instance==null) return 0;
			return instance.globalAbilityModifier.aoeRadius+GetAbilityModifier(abilityID).aoeRadius;
		}
		
		
		public static float GetAbilityDuration(int abilityID){
			if(instance==null) return 0;
			return instance.globalAbilityModifier.effects.duration+GetAbilityModifier(abilityID).effects.duration;
		}
		public static float GetAbilityDamage(int abilityID){
			if(instance==null) return 0;
			return instance.globalAbilityModifier.effects.damageMin+GetAbilityModifier(abilityID).effects.damageMin;
		}
		public static float GetAbilityStunChance(int abilityID){
			if(instance==null) return 0;
			return instance.globalAbilityModifier.effects.stunChance+GetAbilityModifier(abilityID).effects.stunChance;
		}
		
		public static float GetAbilityDamageBuff(int abilityID){
			if(instance==null) return 0;
			return instance.globalAbilityModifier.effects.damageBuff+GetAbilityModifier(abilityID).effects.damageBuff;
		}
		public static float GetAbilityRangeBuff(int abilityID){
			if(instance==null) return 0;
			return instance.globalAbilityModifier.effects.rangeBuff+GetAbilityModifier(abilityID).effects.rangeBuff;
		}
		public static float GetAbilityCooldownBuff(int abilityID){
			if(instance==null) return 0;
			return instance.globalAbilityModifier.effects.cooldownBuff+GetAbilityModifier(abilityID).effects.cooldownBuff;
		}
		public static float GetAbilityHPGain(int abilityID){
			if(instance==null) return 0;
			return instance.globalAbilityModifier.effects.HPGainMin+GetAbilityModifier(abilityID).effects.HPGainMin;
		}
		
		public static Slow GetAbilitySlowMultiplier(int prefabID){
			if(instance==null) return new Slow(0, 0);
			Slow slowG=instance.globalAbilityModifier.effects.slow;
			Slow slowT=GetAbilityModifier(prefabID).effects.slow;
			return new Slow(slowG.slowMultiplier+slowT.slowMultiplier, slowG.duration+slowT.duration);
		}
		public static Dot GetAbilityDotMultiplier(int prefabID){
			if(instance==null) return new Dot(0, 0, 0);
			Dot dotG=instance.globalAbilityModifier.effects.dot;
			Dot dotT=GetAbilityModifier(prefabID).effects.dot;
			return new Dot(dotG.duration+dotT.duration, dotG.interval+dotT.interval, dotG.value+dotT.value);
		}
		
	}

}