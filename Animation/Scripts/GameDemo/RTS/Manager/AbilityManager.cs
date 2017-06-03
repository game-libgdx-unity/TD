using UnitedSolution;using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnitedSolution
{

    public class AbilityManager : MonoBehaviour
    {

        public delegate void AddNewAbilityHandler(Ability ab);
        public static event AddNewAbilityHandler onAddNewAbilityE;      //fire when a new ability is added via perk

        public delegate void ABActivatedHandler(Ability ab);
        public static event ABActivatedHandler onAbilityActivatedE;     //fire when an ability is used

        public delegate void ABTargetSelectModeHandler(bool flag);
        public static event ABTargetSelectModeHandler onTargetSelectModeE;  //fire when enter/exit target selection for ability

        //[HideInInspector] 
        public List<int> unavailableIDList = new List<int>();   //ID list of perk available for this level, modified in editor
                                                                //[HideInInspector] 

        public List<Ability> abilityList = new List<Ability>(); //actual ability list, filled in runtime based on unavailableIDList
        public static List<Ability> GetAbilityList()
        {
            //if (UITowerInfo.instance.currentTower == null)
            return instance.abilityList;
            //else
            //    return new List<Ability>();

        }

        public Transform defaultIndicator;      //generic indicator use for ability without any specific indicator

        private bool inTargetSelectMode = false;
        public static bool InTargetSelectMode() { return instance == null ? false : instance.inTargetSelectMode; }

        private bool validTarget = false;   //used for targetSelectMode, indicate when the cursor is in a valid position or on a valid target

        public int selectedAbilityID = -1;
        public Transform currentIndicator;      //active indicator in used

        public bool startWithFullEnergy = false;
        public bool onlyChargeOnSpawn = false;
        public float energyRate = 2;
        public float fullEnergy = 100;
        public float energy = 0;


        private Transform thisT;
        public static AbilityManager instance;

        public static bool IsOn() { return instance == null ? false : true; }

        void Awake()
        {
            instance = this;
            thisT = transform;

            if (startWithFullEnergy) energy = fullEnergy;

            List<Ability> dbList = AbilityDB.Load();

            abilityList = new List<Ability>();
            for (int i = 0; i < dbList.Count; i++)
            {
                if (!unavailableIDList.Contains(dbList[i].ID))
                {
                    abilityList.Add(dbList[i].Clone());
                }
            }

            List<Ability> newList = PerkManager.GetUnlockedAbility();
            for (int i = 0; i < newList.Count; i++) abilityList.Add(newList[i]);

            for (int i = 0; i < abilityList.Count; i++) abilityList[i].ID = i;

            if (defaultIndicator)
            {
                defaultIndicator = (Transform)Instantiate(defaultIndicator);
                defaultIndicator.parent = thisT;
                defaultIndicator.gameObject.SetActive(false);
            }
        }

        void Start()
        {
            if (GameControl.GetSelectedTower() == null)
            {
                foreach (Ability ability in abilityList)
                {
                    if (ability.belongToHero)
                        ability.abilityButton.rootObj.SetActive(false);
                }
            }
        }


        void onTowerDeselected()
        {
            UIAbilityButton.AlignButtonBegin();

            foreach (Ability ability in abilityList)
            {
                if (ability.belongToHero)
                    ability.abilityButton.rootObj.SetActive(false);
            }

            UIAbilityButton.AlignButtonEnd();
        }

        void onTowerSelected(UnitTower unit)
        {
            print(unit.unitName + " selected");

            UIAbilityButton.AlignButtonBegin();

            List<Ability> abList = new List<Ability>();
            foreach (UnitStat unitStat in unit.stats)
            {
                if (unitStat.abilityHolder)
                {
                    Ability ability = unitStat.abilityHolder.ability;
                    int id = ability.ID;

                    foreach (Ability ab in abilityList)
                    {
                        if (ab.ID == id)
                            ab.abilityButton.rootObj.SetActive(true);
                    }
                }
            }
            UIAbilityButton.AlignButtonEnd();

            //BuildAbilityButtons(abList); 
        }

        public static void AddNewAbility(Ability ab) { instance._AddNewAbility(ab); }
        public void _AddNewAbility(Ability ab)
        {
            for (int i = 0; i < abilityList.Count; i++) { if (ab.ID == abilityList[i].ID) return; }
            abilityList.Add(ab.Clone());
            if (onAddNewAbilityE != null) onAddNewAbilityE(ab);
        }

        void FixedUpdate()
        {
            if ((onlyChargeOnSpawn && GameControl.IsGameStarted()) || !onlyChargeOnSpawn)
            {
                if (energy < fullEnergy)
                {
                    float valueGained = Time.fixedDeltaTime * GetEnergyRate();
                    energy += valueGained;
                    energy = Mathf.Min(energy, GetEnergyFull());
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

            SelectAbilityTarget();
        }


        //called in every frame, execute if there's an ability is selected and pending target selection
        //use only mouse input atm.
        void SelectAbilityTarget()
        {
            if (selectedAbilityID < 0) return;

            //only cast on terrain and platform
            LayerMask mask = 1 << LayerManager.LayerPlatform();
            int terrainLayer = LayerManager.LayerTerrain();
            if (terrainLayer >= 0) mask |= 1 << terrainLayer;

            Ability ability = abilityList[selectedAbilityID];

            if (ability.singleUnitTargeting)
            {
                if (ability.targetType == Ability._TargetType.Hybrid)
                {
                    mask |= 1 << LayerManager.LayerTower();
                    mask |= 1 << LayerManager.LayerCreep();
                }
                else if (ability.targetType == Ability._TargetType.Friendly) mask |= 1 << LayerManager.LayerTower();
                else if (ability.targetType == Ability._TargetType.Hostile) mask |= 1 << LayerManager.LayerCreep();
            }


#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
				Unit targetUnit=null;
				if(Input.touchCount>=1){
					Camera mainCam=Camera.main;
					if(mainCam!=null){
						Ray ray = mainCam.ScreenPointToRay(Input.touches[0].position);
						RaycastHit hit;
						if(Physics.Raycast(ray, out hit, Mathf.Infinity, mask)){
							currentIndicator.position=hit.point+new Vector3(0, 0.1f, 0);
							validTarget=true;
							
							if(ability.singleUnitTargeting){
								targetUnit=hit.transform.GetComponent<Unit>();
								if(targetUnit!=null) currentIndicator.position=targetUnit.thisT.position;
								else validTarget=false;
							}
						}
						else validTarget=false;
					}
				}
				else{
					if(validTarget) ActivateAbility(ability, currentIndicator.position, targetUnit);
					else GameControl.DisplayMessage("Invalid target for ability");
					ClearSelectedAbility();
				}
#else   
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
                {
                    currentIndicator.position = hit.point + new Vector3(0, 0.1f, 0);

                    Unit targetUnit = null;

                    validTarget = true;
                    if (ability.singleUnitTargeting)
                    {
                        targetUnit = hit.transform.GetComponent<Unit>();
                        if (targetUnit != null) currentIndicator.position = targetUnit.thisT.position;
                        else validTarget = false;
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        if (validTarget)
                        {
                            ActivateAbility(ability, currentIndicator.position, targetUnit);
                            ClearSelectedAbility();
                        }
                        else GameControl.DisplayMessage("Invalid target for ability");
                    }
                }
            }


            if (Input.GetMouseButtonDown(1))
            {
                ClearSelectedAbility();
            }
#endif
        }


        //called by ability button from UI, select an ability
        public static string SelectAbility(int ID) { return instance._SelectAbility(ID); }
        public string _SelectAbility(int ID)
        {
            Ability ab = abilityList[ID];

            Debug.Log(ab.name);

            string exception = ab.IsAvailable();
            if (exception != "") return exception;

            if (!ab.requireTargetSelection) ActivateAbility(ab, null);        //no target selection required, fire it away
            else
            {
                if (onTargetSelectModeE != null) onTargetSelectModeE(true); //enter target selection phase

                inTargetSelectMode = true;
                validTarget = false;

                selectedAbilityID = ID;

                if (ab.indicator != null) currentIndicator = ab.indicator;
                else
                {
                    currentIndicator = defaultIndicator;
                    if (ab.autoScaleIndicator)
                    {
                        if (ab.singleUnitTargeting)
                        {
                            float gridSize = BuildManager.GetGridSize();
                            currentIndicator.localScale = new Vector3(gridSize, 1, gridSize);
                        }
                        else currentIndicator.localScale = new Vector3(ab.GetAOERadius() * 2, 1, ab.GetAOERadius() * 2);
                    }
                }

                currentIndicator.gameObject.SetActive(true);
            }

            return "";
        }
        public static void ClearSelectedAbility() { instance._ClearSelectedAbility(); }
        public void _ClearSelectedAbility()
        {
            currentIndicator.gameObject.SetActive(false);
            selectedAbilityID = -1;
            currentIndicator = null;

            inTargetSelectMode = false;

            if (onTargetSelectModeE != null) onTargetSelectModeE(false);
        }
        //check for selfcast or cast at position of caster
        public void ActivateAbility(int ID, Unit caster = null, Vector3 pos = default(Vector3), Unit target = null, System.Action<Unit> OnCasterCallback = null)
        {
            ActivateAbility(abilityList[ID], caster, pos, target, OnCasterCallback);
        }

        //check for selfcast or cast at position of caster
        public void ActivateAbility(Ability ability, Unit caster = null, Vector3 pos = default(Vector3), Unit target = null, System.Action<Unit> OnCasterCallback = null)
        {
            ability.caster = caster;
            if (caster && ability.selfCast)
            {
                ActivateAbility(ability, caster.GetTargetT().position, caster);
            }
            else if (caster && ability.castAtCaster)
            {
                ActivateAbility(ability, caster.GetTargetT().position, target);
            }
            else
            {
                ActivateAbility(ability, pos, target);
            }
        }
        // be called when an ability is fired, reduce the energy, start the cooldown and what not
        public void ActivateAbility(Ability ab, Vector3 pos = default(Vector3), Unit target = null)
        {
            ab.usedCount += 1;
            energy -= ab.GetCost();
            StartCoroutine(ab.CooldownRoutine());

            CastAbility(ab, pos, target);

            if (onAbilityActivatedE != null) onAbilityActivatedE(ab);
        }

        //called from ActivateAbility, cast the ability, visual effect and actual effect goes here
        public void CastAbility(Ability ab, Vector3 pos = default(Vector3), Unit target = null)
        {
            GameObject effect = null;

            //print((ab.Caster ? "Unknown" : ab.Caster.unitName) + " casted " + ab.name + " on " + target);

            if (ab.prefab != null)
            {
                if (ab.abilityType == Ability.AbilityType.Summoning)
                {
                    effect = ObjectPoolManager.Spawn(ab.prefab, ab.caster.GetTargetT().position + Vector3.right * -2f, Quaternion.identity);
                    BuildManager.PreBuildTower(effect.GetComponent<UnitDefender>());
                }
                else if (ab.selfCast || ab.castAtCaster)
                {
                    effect = ObjectPoolManager.Spawn(ab.prefab, ab.caster.thisT.position, Quaternion.identity);
                    if (ab.caster)
                    {
                        print("Set parent");
                        effect.transform.parent = ab.caster.transform;
                    }
                }
                else
                {
                    effect = ObjectPoolManager.Spawn(ab.prefab, target ? target.GetTargetT().position : pos, Quaternion.identity);
                }
            }

            if (ab.useDefaultEffect)
            {
                StartCoroutine(ApplyAbilityEffect(ab, pos, target));
            }
            //else if (effect)
            //{
            //    AbilityBehavior abilityBehavior = effect.GetComponent<AbilityBehavior>();
            //    if (abilityBehavior)
            //    {
            //        StartCoroutine(ApplyAbilityEffect(abilityBehavior.ability, pos, target));
            //    }
            //}
        }

        void OnDestroy() { instance = null; }

        //apply the ability effect, damage, stun, buff and so on 
        IEnumerator ApplyAbilityEffect(Ability ab, Vector3 pos = default(Vector3), Unit target = null)
        {
            yield return new WaitForSeconds(ab.effectDelay);
            LayerMask mask = ab.customMask;
            List<Unit> unitList = new List<Unit>();
            //List<Unit> towerList = new List<Unit>();
            if (target)
            {
                unitList.Add(target);
            }
            if (ab.GetAOERadius() > 0)
            {
                float radius = ab.requireTargetSelection ? ab.GetAOERadius() : Mathf.Infinity;
                Collider[] cols = Physics.OverlapSphere(pos, radius, mask);

                if (cols.Length > 0)
                {
                    for (int i = 0; i < cols.Length; i++)
                    {
                        Unit unit = cols[i].gameObject.GetComponent<Unit>();
                        if (unit && !unit.dead)
                            unitList.Add(unit);
                    }
                }
            }

            AbilityEffect eff = ab.GetActiveEffect();

            for (int n = 0; n < unitList.Count; n++)
            {
                if (eff.damageMax > 0)
                {
                    unitList[n].ApplyDamage(Random.Range(eff.damageMin, eff.damageMax));
                }
                else if (eff.stunChance > 0 && eff.duration > 0)
                {
                    if (Random.Range(0f, 1f) < eff.stunChance) unitList[n].ApplyStun(eff.duration);
                }
                else if (eff.slow.IsValid())
                {
                    unitList[n].ApplySlow(eff.slow);
                }
                else if (eff.dot.GetTotalDamage() > 0)
                {
                    unitList[n].ApplyDot(eff.dot);
                }
                if (eff.duration > 0)
                {
                    if (eff.damageBuff > 0)
                    {
                        unitList[n].ApplyBuffDamage(eff.damageBuff, eff.duration);
                    }
                    if (eff.rangeBuff > 0)
                    {
                        unitList[n].ApplyBuffRange(eff.rangeBuff, eff.duration);
                    }
                    if (eff.cooldownBuff > 0)
                    {
                        unitList[n].ApplyBuffCooldown(eff.cooldownBuff, eff.duration);
                    }
                    if (eff.dodgeChance > 0)
                    {
                        unitList[n].ApplyBuffDodgeChance(eff.dodgeChance, eff.duration);
                    }
                }
                else if (eff.HPGainMax > 0)
                {
                    unitList[n].RestoreHP(Random.Range(eff.HPGainMin, eff.HPGainMax));
                }
            }
        }




        public static void GainEnergy(int value) { if (instance != null) instance._GainEnergy(value); }
        public void _GainEnergy(int value)
        {
            energy += value;
            energy = Mathf.Min(energy, GetEnergyFull());
        }


        public static float GetAbilityCurrentCD(int ID) { return instance.abilityList[ID].currentCD; }

        public static float GetEnergyFull() { return instance.fullEnergy + PerkManager.GetEnergyCapModifier(); }
        public static float GetEnergy() { return instance.energy; }

        private float GetEnergyRate() { return energyRate + PerkManager.GetEnergyRegenModifier(); }

        void OnEnable()
        {
            GameControl.onTowerSelected += onTowerSelected;
            GameControl.onDeselectedTower += onTowerDeselected;
        }

        void OnDisable()
        {
            GameControl.onDeselectedTower -= onTowerDeselected;
            GameControl.onTowerSelected -= onTowerSelected;
        }
    }
}