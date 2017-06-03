using UnitedSolution;using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnitedSolution {
	
	
	
	public class PerkTowerModifier{
		public int prefabID=-1;
		
		public float HP=0;
		public float HPRegen=0;
		public float HPStagger=0;
		
		public float shield=0;
		public float shieldRegen=0;
		public float shieldStagger=0;
		
		public float buildCost=0;
		public float upgradeCost=0;
		
		public UnitStat stats=new UnitStat();
		
		public PerkTowerModifier(){
			stats.damageMin=0;
			stats.cooldown=0;
			stats.clipSize=0;
			stats.reloadDuration=0;
			stats.attackRange=0;
			stats.aoeRadius=0;
			stats.hit=0;
			stats.dodge=0;
			stats.crit.chance=0;
			stats.crit.dmgMultiplier=0;
		}
	}
	
	public class PerkAbilityModifier{
		public int abilityID=-1;
		
		public float cost=0;
		public float cooldown=0;
		public float aoeRadius=0;
		
		public AbilityEffect effects=new AbilityEffect();
	}
	
	public class PerkFPSWeaponModifier{
		public int prefabID=-1;
		
		public UnitStat stats=new UnitStat();
		
		public PerkFPSWeaponModifier(){
			stats.damageMin=0;
			stats.cooldown=0;
			stats.clipSize=0;
			stats.reloadDuration=0;
			stats.aoeRadius=0;
			stats.aoeRadius=0;
			stats.crit.chance=0;
			stats.crit.dmgMultiplier=0;
		}
	}
	
	
	
	public enum _PerkType{
		NewTower,
		NewAbility,
		NewFPSWeapon,
		
		GainLife, 
		LifeCap, 
		LifeRegen,
		LifeWaveClearedBonus, 
		
		GainRsc,
		RscRegen,					//generate overtime
		RscGain,						//modifier for any gain (global)
		RscCreepKilledGain,		//modifier for gain when a creep is killed
		RscWaveClearedGain,	//modifier for gain when a wave is cleared
		RscResourceTowerGain,	//modifier for gain from rscTower
		
		Tower,					//global for all towers
		TowerSpecific,		//only for specific tower (that uses the same prefabID)
		Ability,
		AbilitySpecific,
		FPSWeapon,
		FPSWeaponSpecific,
		
		EnergyRegen,
		EnergyIncreaseCap,
		EnergyCreepKilledBonus,
		EnergyWaveClearedBonus,
	}
	
	
	[System.Serializable]
	public class Perk : UnitedSolutionItem{
		public Sprite iconUnavailable;
		public Sprite iconPurchased;
		
		public bool repeatable=false;		//can the perk can be purchased repeatably
		public bool purchased=false;		//has the perk been purchased
		//public bool enableInlvl=true;		
		
		public _PerkType type;
		
		public List<int> cost=new List<int>();
		public int minLevel=1;								//min level to reach before becoming available (check GameControl.levelID)
		public int minWave=0;								//min wave to reach before becoming available
		public int minPerkPoint=0;						//min perk point 
		public List<int> prereq=new List<int>();	//prerequisite perk before becoming available, element is removed as the perk is unlocked in runtime
		
		public List<int> itemIDList=new List<int>();
		public int itemID;
		public float value;
		public float valueAlt;	//act as min/max in some case
		public List<float> valueRscList=new List<float>();
		public UnitStat stats=new UnitStat();
		public AbilityEffect effects=new AbilityEffect();
		
		
		
		
		//for tower
		public float HP=0;
		public float HPRegen=0;
		public float HPStagger=0;
		public float shield=0;
		public float shieldRegen=0;
		public float shieldStagger=0;
		public float buildCost=0;
		public float upgradeCost=0;
		
		
		//for ability
		public float abCost=0;
		public float abCooldown=0;
		public float abAOERadius=0;
		
		public string desp="";
		
		
		public Perk(){
			stats.damageMin=0;
			stats.damageMax=0;
			stats.cooldown=0;
			stats.clipSize=0;
			stats.reloadDuration=0;
			stats.attackRange=0;
			stats.aoeRadius=0;
		}
		
		
		public Perk Clone(){
			Perk perk=new Perk();
			perk.ID=ID;
			perk.name=name;
			perk.icon=icon;
			perk.iconUnavailable=iconUnavailable;
			
			perk.repeatable=repeatable;
			perk.purchased=purchased;
			
			perk.type=type;
			
			//perk.cost=cost;
			perk.cost=new List<int>(cost);
			perk.minLevel=minLevel;
			perk.minWave=minWave;
			perk.minPerkPoint=minPerkPoint;
			//perk.prereq=prereq;
			perk.prereq=new List<int>(prereq);
			
			//perk.itemIDList=itemIDList;
			perk.itemIDList=new List<int>(itemIDList);
			perk.itemID=itemID;
			perk.value=value;
			perk.valueAlt=valueAlt;
			//perk.valueRscList=valueRscList;
			perk.valueRscList=new List<float>(valueRscList);
			perk.stats=stats;
			
			perk.HP=HP;
			perk.HPRegen=HPRegen;
			perk.HPStagger=HPStagger;
			perk.shield=shield;
			perk.shieldRegen=shieldRegen;
			perk.shieldStagger=shieldStagger;
			perk.buildCost=buildCost;
			perk.upgradeCost=upgradeCost;
			
			perk.abCost=abCost;
			perk.abCooldown=abCooldown;
			perk.abAOERadius=abAOERadius;
			
			perk.desp=desp;
			
			return perk;
		}
		
		public string IsAvailable(){
			//Debug.Log("  "+SpawnManager.GetCurrentWaveID());
			if(purchased) return "Purchased";
			if(GameControl.GetLevelID()<minLevel) return "Unlocked at level "+minLevel;
			if(Mathf.Max(SpawnManager.GetCurrentWaveID()+1, 1)<minWave) return "Unlocked at Wave "+minWave;
			if(PerkManager.GetPerkPoint()<minPerkPoint) return "Insufficient perk point";
			if(prereq.Count>0){
				string text="Require: ";
				bool first=true;
				List<Perk> perkList=PerkManager.GetPerkList();
				for(int i=0; i<prereq.Count; i++){
					for(int n=0; n<perkList.Count; n++){
						if(perkList[n].ID==prereq[i]){
							text+=((!first) ? ", " : "")+perkList[n].name;
							first=false;
							break;
						}
					}
				}
				return text;
				//return "Not all prerequisite perk has been unlocked";
			}
			return "";
		}
		
		public string Purchase(bool useRsc=true){
			if(purchased) return "Purchased";
			
			if(useRsc){
				int temp=ResourceManager.HasSufficientResource(cost);
				if(temp!=-1){
					Debug.Log(temp);
					return "Insufficient "+ResourceManager.GetResourceList()[temp].name;
				}
				ResourceManager.SpendResource(cost);
			}
			
			if(!repeatable) purchased=true;
			
			return "";
		}
		
		public List<int> GetCost(){ return cost; }
		
	}
	
	
	
}
