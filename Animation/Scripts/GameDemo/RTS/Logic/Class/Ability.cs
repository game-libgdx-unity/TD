using UnitedSolution;using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnitedSolution {

	[System.Serializable]
	public class Ability : UnitedSolutionItem {

        public GameObject prefab;
        public UnityButton abilityButton;
        public bool belongToHero = false;
        public Unit caster;
		public enum _TargetType {Hostile, Friendly, Hybrid}
		public enum AbilityType { FullRanged, RequiredTarget, Selfcast, Summoning , SingleUnitOnly}
        public AbilityType abilityType = AbilityType.RequiredTarget;

        public bool selfCast = false;
        public bool castAtCaster = false;
        public bool disableInAbilityManager=false;
		
		public int cost=0;
		public float cooldown=0;	//cd duration
		public float currentCD=0;		//variable for counting cd during runtime, ability only available when this is <0
		
		public bool requireTargetSelection=true;
		public bool singleUnitTargeting=false;
		public _TargetType targetType=_TargetType.Hostile;
		
		public int maxUseCount=-1;
		public int usedCount=0;		////variable for counting usage during runtime
		
		public Transform indicator;
		public bool autoScaleIndicator=true;
		
		public bool useDefaultEffect=true;
		public List<AbilityEffect> effectList=new List<AbilityEffect>();
		public AbilityEffect effect=new AbilityEffect();
		
		public float aoeRadius=2;
		public float effectDelay=0.25f;

        public bool useCustomDesp=false;
		public string desp="";
        public LayerMask customMask = LayerManager.LayerDefault();

        public Ability Clone(){
			Ability ab=new Ability();
            ab.belongToHero = belongToHero;
            ab.selfCast = selfCast;
            ab.castAtCaster = castAtCaster;
            ab.customMask = customMask;
            ab.abilityType = abilityType;
			ab.ID=ID;
            ab.prefab = prefab;
			ab.name=name;
			ab.icon=icon;
			ab.cost=cost;
			ab.cooldown=cooldown;
			ab.currentCD=currentCD;
			ab.requireTargetSelection=requireTargetSelection;
			ab.singleUnitTargeting=singleUnitTargeting;
			ab.targetType=targetType; 
			ab.maxUseCount=maxUseCount;
			ab.usedCount=usedCount;
			ab.indicator=indicator;
			ab.autoScaleIndicator=autoScaleIndicator;
			ab.useDefaultEffect=useDefaultEffect;
			for(int i=0; i<effectList.Count; i++) ab.effectList.Add(effectList[i].Clone());
			ab.effect=effect.Clone();
			ab.aoeRadius=aoeRadius;
			ab.effectDelay=effectDelay;
			ab.useCustomDesp=useCustomDesp;
			ab.desp=desp;
            return ab;
		}
		
		public string IsAvailable(){
			if(GetCost()>AbilityManager.GetEnergy()) return "Insufficient Energy";
			if(currentCD>0) return "Ability is on cooldown";
			if(maxUseCount>0 && usedCount>=maxUseCount) return "Usage limit exceed";
			return "";
		}
		
		public IEnumerator CooldownRoutine(){
			currentCD=GetCooldown();
			while(currentCD>0){
				currentCD-=Time.deltaTime;
				yield return null;
			}
		}
		
		public float GetCost(){ return cost * (1-PerkManager.GetAbilityCost(ID)); }
		public float GetCooldown(){ return cooldown * (1-PerkManager.GetAbilityCooldown(ID)); }
		public float GetAOERadius(){ return aoeRadius * (1+PerkManager.GetAbilityAOERadius(ID)); }
		
		
		public AbilityEffect GetActiveEffect(){
            AbilityEffect eff = effect.Clone();
			eff.duration=effect.duration * (1+PerkManager.GetAbilityDuration(ID));
			eff.damageMin=effect.damageMin * (1+PerkManager.GetAbilityDamage(ID));
			eff.damageMax=effect.damageMax * (1+PerkManager.GetAbilityDamage(ID));
			eff.stunChance=effect.stunChance * (1+PerkManager.GetAbilityStunChance(ID));
			
			eff.slow=PerkManager.ModifySlowWithPerkBonus(effect.slow, ID, 2);	//pass 2 to indicate this is for ability
			eff.dot=PerkManager.ModifyDotWithPerkBonus(effect.dot, ID, 2);
			
			eff.damageBuff=effect.damageBuff * (1+PerkManager.GetAbilityDamageBuff(ID));
			eff.rangeBuff=effect.rangeBuff * (1+PerkManager.GetAbilityRangeBuff(ID));
			eff.cooldownBuff=effect.cooldownBuff * (1+PerkManager.GetAbilityCooldownBuff(ID));
			eff.HPGainMin=effect.HPGainMin * (1+PerkManager.GetAbilityHPGain(ID));
			eff.HPGainMax=effect.HPGainMax * (1+PerkManager.GetAbilityHPGain(ID));
			
			return eff;
		}
		
		
		public string GetDesp(){
			if(useCustomDesp) return desp;
			
			string text="";
			
			AbilityEffect eff=GetActiveEffect();
			
			if(eff.damageMax>0){
				if(requireTargetSelection) text+="Deals "+eff.damageMin+"-"+eff.damageMax+" to hostile target in range\n";
				else text+="Deals "+eff.damageMin+"-"+eff.damageMax+" to all hostile on the map\n";
			}
			if(eff.stunChance>0 && eff.duration>0){
				if(requireTargetSelection) text+=(eff.stunChance*100).ToString("f0")+"% chance to stun hostile target for "+eff.duration+"s\n";
				else text+=(eff.stunChance*100).ToString("f0")+"% chance to stun all hostile on the map for "+eff.duration+"s\n";
			}
			if(eff.slow.IsValid()){
				if(requireTargetSelection) text+="Slows hostile target down for "+eff.duration+"s\n";
				else text+="Slows all hostile on the map down for "+eff.duration+"s\n";
			}
			if(eff.dot.GetTotalDamage()>0){
				if(requireTargetSelection) text+="Deals "+eff.dot.GetTotalDamage().ToString("f0")+" to hostile target over "+eff.duration+"s\n";
				else text+="Deals "+eff.dot.GetTotalDamage().ToString("f0")+" to all hostile on the map over "+eff.duration+"s\n";
			}
			
			
			if(eff.HPGainMax>0){
				if(requireTargetSelection) text+="Restore "+eff.HPGainMin+"-"+eff.HPGainMax+" of friendly target HP\n";
				else text+="Restore "+eff.HPGainMin+"-"+eff.HPGainMax+" of all tower HP\n";
			}
			if(eff.duration>0){
				if(eff.damageBuff>0){
					if(requireTargetSelection) text+="Increase friendly target damage by "+(eff.damageBuff*100).ToString("f0")+"%\n";
					else text+="Increase all towers damage by "+(eff.damageBuff*100).ToString("f0")+"%\n";
				}
				if(eff.rangeBuff>0){
					if(requireTargetSelection) text+="Increase friendly target range by "+(eff.rangeBuff*100).ToString("f0")+"%\n";
					else text+="Increase all towers range by "+(eff.rangeBuff*100).ToString("f0")+"%\n";
				}
				if(eff.cooldownBuff>0){
					if(requireTargetSelection) text+="Decrease friendly target cooldown by "+(eff.cooldownBuff*100).ToString("f0")+"%\n";
					else text+="Decrease all towers cooldown by "+(eff.cooldownBuff*100).ToString("f0")+"%\n";
				}
			}
			
			
			return text;
		}
	}
	
	
	[System.Serializable]
	public class AbilityEffect{

        public GameObject effectAtTarget;
		public float duration=0;    //duration of the effect, shared by all (stun & buff)
        public UnitStat stat;
		//offsense stat
		public float damageMin=0;
		public float damageMax=0;
		public float stunChance=0;
		public Slow slow=new Slow();
		public Dot dot=new Dot();
		
		//defense stat
		public float damageBuff=0;
		public float rangeBuff=0;
		public float cooldownBuff=0;
		public float HPGainMin=0;
		public float HPGainMax=0;
        public float dodgeChance;

        public AbilityEffect Clone(){
			AbilityEffect eff=new AbilityEffect();
			
			eff.duration=duration;
            eff.dodgeChance = dodgeChance;
            eff.effectAtTarget = effectAtTarget;
            //eff.stat = stat.Clone();
            eff.damageMin=damageMin;
			eff.damageMax=damageMax;
			eff.stunChance=stunChance;
			eff.slow=slow.Clone();
			eff.dot=dot.Clone();
			
			eff.slow.duration=eff.duration;
			eff.dot.duration=eff.duration;
			
			eff.damageBuff=damageBuff;
			eff.rangeBuff=rangeBuff;
			eff.cooldownBuff=cooldownBuff;
			eff.HPGainMin=HPGainMin;
			eff.HPGainMax=HPGainMax;
			
			return eff;
		}
	}
	
	
	
}
