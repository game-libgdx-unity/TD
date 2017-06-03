using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution
{

    public class AbilityEditorWindow : EditorWindow
    {

        public static AbilityEditorWindow window;

        private static AbilityDB prefab;
        private static List<Ability> abilityList = new List<Ability>();
        private static List<int> abilityIDList = new List<int>();
        public void Select(string name, GameObject abilityPrefab)
        {
            int selectID = -1;
            List<Ability> towerList = prefab.abilityList;
            for (int i = 0; i < towerList.Count; i++)
            {
                if (towerList[i].name.Trim().Equals(name.Trim()))
                {
                    selectID = i;
                }
            }
            if(selectID >= 0)
                SelectAbility(selectID);
            else
                AddNewAbility(abilityPrefab);
        }
        public static void Init()
        {
            // Get existing open window or if none, make a new one:
            window = (AbilityEditorWindow)EditorWindow.GetWindow(typeof(AbilityEditorWindow), false, "Ability Editor");
            //~ window.minSize=new Vector2(375, 449);
            //~ window.maxSize=new Vector2(375, 800);

            prefab = AbilityDB.LoadDB();
            abilityList = prefab.abilityList;

            for (int i = 0; i < abilityList.Count; i++)
            {
                //towerList[i].prefabID=i;
                if (abilityList[i] != null)
                {
                    abilityIDList.Add(abilityList[i].ID);
                }
                else
                {
                    abilityList.RemoveAt(i);
                    i -= 1;
                }
            }

            InitLabel();
        }

        private static string[] abEffectLabel;
        private static string[] abEffectTooltip;
        private static string[] targetTypeLabel;
        private static string[] targetTypeTooltip;
        private static string[] abilityTypeLabel;
        private static string[] abilityTypeTooltip;

        private static void InitLabel()
        {
            int enumLength = Enum.GetValues(typeof(Ability._TargetType)).Length;
            targetTypeLabel = new string[enumLength];
            targetTypeTooltip = new string[enumLength];
            for (int i = 0; i < enumLength; i++)
            {
                targetTypeLabel[i] = ((Ability._TargetType)i).ToString();
                if ((Ability._TargetType)i == Ability._TargetType.Hostile) targetTypeTooltip[i] = "Ability can target hostile unit only";
                if ((Ability._TargetType)i == Ability._TargetType.Friendly) targetTypeTooltip[i] = "Ability can target friendly unit only";
                if ((Ability._TargetType)i == Ability._TargetType.Hybrid) targetTypeTooltip[i] = "Ability can target both air and ground unit";
            }
            enumLength = Enum.GetValues(typeof(Ability.AbilityType)).Length;
            abilityTypeLabel = new string[enumLength];
            abilityTypeTooltip = new string[enumLength];
            for (int i = 0; i < enumLength; i++)
            {
                abilityTypeLabel[i] = ((Ability.AbilityType)i).ToString();
                if ((Ability.AbilityType)i == Ability.AbilityType.RequiredTarget) abilityTypeTooltip[i] = "Ability Required Target";
                if ((Ability.AbilityType)i == Ability.AbilityType.Selfcast) abilityTypeTooltip[i] = "Cast for caster only";
                if ((Ability.AbilityType)i == Ability.AbilityType.FullRanged) abilityTypeTooltip[i] = "Affect all unit in layers";
                if ((Ability.AbilityType)i == Ability.AbilityType.Summoning) abilityTypeTooltip[i] = "Summoning another unit";
                if ((Ability.AbilityType)i == Ability.AbilityType.SingleUnitOnly) abilityTypeTooltip[i] = "Affect Single Unit Only";
            }
        }

        int GenerateNewID()
        {
            int ID = 0;
            while (abilityIDList.Contains(ID)) ID += 1;
            return ID;
        }

        public void SelectAbility(int ID)
        {
            selectID = ID;
            GUI.FocusControl("");

            if (selectID * 35 < scrollPos1.y) scrollPos1.y = selectID * 35;
            if (selectID * 35 > scrollPos1.y + listVisibleRect.height - 40) scrollPos1.y = selectID * 35 - listVisibleRect.height + 40;
        }


        private int selectID = 0;

        private Vector2 scrollPos1;
        private Vector2 scrollPos2;

        private GUIContent cont;
        private GUIContent[] contL;

        private float contentHeight = 0;
        private float contentWidth = 0;

        private float spaceX = 120;
        private float spaceY = 20;
        private float width = 150;
        private float height = 18;

        void OnGUI()
        {
            if (window == null) Init();

            if (GUI.Button(new Rect(window.position.width - 120, 5, 100, 25), "Save"))
            {
                EditorUtility.SetDirty(prefab);
            }

            EditorGUI.LabelField(new Rect(250, 7, 150, 17), "Add new ability:");
            GameObject abilityPrefab = null;
            abilityPrefab = (GameObject)EditorGUI.ObjectField(new Rect(350, 7, 140, 17), abilityPrefab, typeof(GameObject), false);
            AddNewAbility(abilityPrefab);

            if (GUI.Button(new Rect(5, 5, 120, 25), "Create New"))
            {
                Ability newAbility = new Ability();

                int ID = GenerateNewID();
                abilityIDList.Add(ID);
                newAbility.ID = ID;
                newAbility.name = "Ability " + ID;

                abilityList.Add(newAbility);
                SelectAbility(abilityList.Count - 1);

                GUI.changed = true;
            }
            if (abilityList.Count > 0 && GUI.Button(new Rect(130, 5, 100, 25), "Clone Selected"))
            {
                Ability newAbility = abilityList[selectID].Clone();

                int ID = GenerateNewID();
                abilityIDList.Add(ID);
                newAbility.ID = ID;
                newAbility.name += " (Clone)";

                abilityList.Insert(selectID + 1, newAbility);
                SelectAbility(selectID + 1);

                GUI.changed = true;
            }


            float startX = 5;
            float startY = 55;


            if (minimiseList)
            {
                if (GUI.Button(new Rect(startX, startY - 20, 30, 18), ">>")) minimiseList = false;
            }
            else
            {
                if (GUI.Button(new Rect(startX, startY - 20, 30, 18), "<<")) minimiseList = true;
            }
            Vector2 v2 = DrawAbilityList(startX, startY);

            startX = v2.x + 25;

            if (abilityList.Count == 0) return;


            Rect visibleRect = new Rect(startX, startY, window.position.width - startX - 10, window.position.height - startY - 5);
            Rect contentRect = new Rect(startX, startY, contentWidth - startY, contentHeight);

            //~ GUI.color=new Color(.8f, .8f, .8f, 1f);
            //~ GUI.Box(visibleRect, "");
            //~ GUI.color=Color.white;

            scrollPos2 = GUI.BeginScrollView(visibleRect, scrollPos2, contentRect);

            //float cachedX=startX;
            v2 = DrawAbilityConfigurator(startX, startY, abilityList[selectID]);
            //contentWidth=v2.x+50;
            contentHeight = v2.y;

            GUI.EndScrollView();

            contentWidth = startX + 280;


            if (GUI.changed)
            {
                EditorUtility.SetDirty(prefab);

                foreach (Ability ab in prefab.abilityList)
                {
                    if (ab.prefab)
                        EditorUtility.SetDirty(ab.prefab);
                }
            }

        }

        private void AddNewAbility(GameObject abilityPrefab)
        {
            AbilityBehavior abilityBehavior = abilityPrefab ? abilityPrefab.GetComponent<AbilityBehavior>() : null;
            Ability ability = abilityBehavior ? abilityBehavior.ability : null;
            if (ability != null)
            {
                int ID = GenerateNewID();
                abilityIDList.Add(ID);
                ability.ID = ID;
                if (abilityPrefab)
                {
                    ability.prefab = abilityPrefab;
                    ability.useDefaultEffect = false;
                }

                if (string.IsNullOrEmpty(ability.name.Trim())) ability.name = "Ability " + ID;
                abilityList.Add(ability);
                SelectAbility(abilityList.Count - 1);
                abilityBehavior.ability = ability;
                EditorUtility.SetDirty(abilityBehavior);
                GUI.changed = true;
                //if (newSelectID != -1) SelectTower(newSelectID);
            }
        }

        Vector2 DrawAbilityConfigurator(float startX, float startY, Ability ability)
        {

            float originX = startX;
            float originY = startY;
            //float cachedX=startX;
            float cachedY = startY;

            cont = new GUIContent("Prefab:", "The prefab object of the tower\nClick this to highlight it in the ProjectTab");
            EditorGUI.LabelField(new Rect(startX + 65, startY, width, height), cont);
            abilityList[selectID].prefab =(GameObject) EditorGUI.ObjectField(new Rect(startX + 120, startY, 130, height), abilityList[selectID].prefab, typeof(GameObject), false);

            cont = new GUIContent("ID :" + ability.ID, "The prefab object of the tower\nClick this to highlight it in the ProjectTab");
            EditorGUI.LabelField(new Rect(startX + 260, startY, width, height), cont);

            //EditorUtilities.DrawSprite(new Rect(startX, startY, 60, 60), ability.icon);
            startX += 65;

            cont = new GUIContent("Name:", "The ability name to be displayed in game");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
            ability.name = EditorGUI.TextField(new Rect(startX + spaceX - 65, startY, width - 5, height), ability.name);

            cont = new GUIContent("Selfcast:", "When checked, Selfcast ability");
            EditorGUI.LabelField(new Rect(startX + 235, startY, width + 10, height), cont);
            ability.selfCast = EditorGUI.Toggle(new Rect(startX + 290, startY, 40, height), ability.selfCast);

            if (!ability.selfCast)
            {
                cont = new GUIContent("At Caster:", "Cast at position of Caster");
                EditorGUI.LabelField(new Rect(startX + 310, startY, width + 10, height), cont);
                ability.castAtCaster = EditorGUI.Toggle(new Rect(startX + 365, startY, 40, height), ability.castAtCaster);
            }

            cont = new GUIContent("Layer:", "Layer of targetable object");
            EditorGUI.LabelField(new Rect(startX + 235, startY += spaceY, width + 10, height), cont);
            ability.customMask = EditorUtilities.LayerMaskField(new Rect(startX + 290, startY, 100, height), "", ability.customMask);

            //ability.customMask = EditorGUI.IntField(new Rect(startX + 390, startY+40, 185, height), ability.customMask);

            cont = new GUIContent("Icon:", "The ability icon to be displayed in game, must be a sprite");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
            ability.icon = (Sprite)EditorGUI.ObjectField(new Rect(originX, originY, 60, 60), ability.icon, typeof(Sprite), false);

            cont = new GUIContent("Disable in AbilityManager:", "When checked, the ability won't appear on AbilityManager list and thus can't be access from the get go\nThis is to mark ability that can only be unlocked from perk");
            EditorGUI.LabelField(new Rect(startX + 235, startY, width + 10, height), cont);
            ability.disableInAbilityManager = EditorGUI.Toggle(new Rect(startX + 400, startY , 185, height), ability.disableInAbilityManager);

            cont = new GUIContent("Belong to Hero:", "When checked, the ability won't appear on AbilityManager list and thus can't be access from the get go\nThis is to mark ability that can only be unlocked from perk");
            EditorGUI.LabelField(new Rect(startX + 235, startY += spaceY, width + 10, height), cont);
            ability.belongToHero = EditorGUI.Toggle(new Rect(startX + 400, startY, 185, height), ability.belongToHero);


            startX -= 65;
            startY += 10 + spaceY / 2;

            cachedY = startY + spaceY;

            cont = new GUIContent("Cost:", "The energy cost to use the ability");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
            ability.cost = EditorGUI.IntField(new Rect(startX + spaceX, startY, 40, height), ability.cost);

            cont = new GUIContent("Cooldown:", "The cooldown duration of the ability. Once used, the ability cannot be used again until the cooldown duration has passed");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
            ability.cooldown = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 40, height), ability.cooldown);


            cont = new GUIContent("Ability type:", "Check if ability need a specific position or unit as target. When checked, the user will need to select a position/unit before the ability can be cast. Otherwise the ability be cast without a target position/unit. If ability uses default effects and this is unchecked, the effect will be apply to all unit in the game");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
            contL = new GUIContent[abilityTypeLabel.Length];
            for (int i = 0; i < contL.Length; i++) contL[i] = new GUIContent(abilityTypeLabel[i], abilityTypeTooltip[i]);
            ability.abilityType = (Ability.AbilityType)EditorGUI.Popup(new Rect(startX + spaceX, startY, width - 20, height), new GUIContent(""), (int)ability.abilityType, contL);

            // ability.requireTargetSelection = EditorGUI.Toggle(new Rect(startX + spaceX, startY, 40, height), ability.requireTargetSelection);

            if (ability.requireTargetSelection)
            {
                cont = new GUIContent(" - Single Unit Only:", "Check if the ability require a specific unit as a target. Otherwise the ability can be cast anywhere without a specific target");
                EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
                ability.singleUnitTargeting = EditorGUI.Toggle(new Rect(startX + spaceX, startY, 40, height), ability.singleUnitTargeting);

                if (ability.singleUnitTargeting)
                {
                    //~ cont=new GUIContent(" - Target Friendly:", "Check if the ability is meant to target firendly unit. Otherwise it will target");
                    //~ EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
                    //~ ability.targetType=EditorGUI.Toggle(new Rect(startX+spaceX, startY, 40, height), ability.targetType);

                    cont = new GUIContent(" - Target :", "Determine which type of unit the tower can target. Hostile for hostile unit. Friendly for friendly unit. Hybrid for both.");
                    EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
                    contL = new GUIContent[targetTypeLabel.Length];
                }

                cont = new GUIContent(" - Indicator:", "(Optional) The cursor indicator that used to indicate the ability target position during target selection phase for the ability. If left unassigned, the default indicator specified in the AbilityManager will be used instead");
                EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
                ability.indicator = (Transform)EditorGUI.ObjectField(new Rect(startX + spaceX, startY, width, height), ability.indicator, typeof(Transform), false);

                if (ability.indicator == null)
                {
                    cont = new GUIContent(" - Scale Indicator:", "Automatically scale the indicator size to match the aoeRadius of the ability, or a unit width in case of a single unit targeting. Only applicable if ability is using default effects");
                    EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
                    ability.autoScaleIndicator = EditorGUI.Toggle(new Rect(startX + spaceX, startY, 40, height), ability.autoScaleIndicator);
                }
                else
                {
                    cont = new GUIContent(" - Scale Indicator:", "Automatically scale the indicator size to match the aoeRadius of the ability. Only applicable if ability is using default effects");
                    EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
                    EditorGUI.LabelField(new Rect(startX + spaceX, startY, width, height), "not applicable");
                }
            }
            else
            {
                cont = new GUIContent(" - Targets Unit:", "Check if the unit is immuned to critical hit");
                EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
                EditorGUI.LabelField(new Rect(startX + spaceX, startY, width, height), "not applicable");

                cont = new GUIContent(" - Indicator:", "(Optional) The cursor indicator that used to indicate the ability target position during target selection phase for the ability. If left unassigned, the default indicator specified in the AbilityManager will be used instead");
                EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
                EditorGUI.LabelField(new Rect(startX + spaceX, startY, width, height), "not applicable");

                cont = new GUIContent(" - Scale Indicator:", "Automatically scale the indicator size to match the aoeRadius of the ability. Only applicable if ability is using default effects");
                EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
                EditorGUI.LabelField(new Rect(startX + spaceX, startY, width, height), "not applicable");
            }


            cont = new GUIContent("Max Use Count:", "The maximum amount which the ability can be used in a level. Indicate unlimited usage when set to <0");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
            ability.maxUseCount = EditorGUI.IntField(new Rect(startX + spaceX, startY, 40, height), ability.maxUseCount);


            //cont = new GUIContent("Effect Object:", "The effect object spawned at the selected position when the ability is used. This object can contain custom script to do custom effect for the ability");
            //EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
            //ability.effectObj = (GameObject)EditorGUI.ObjectField(new Rect(startX + spaceX, startY, width, height), ability.effectObj, typeof(GameObject), false);
            
            //if(!ability.effectObj && ability.prefab)
            //{
            //    ability.effectObj = ability.prefab;
            //}

            cont = new GUIContent("Use Default Effect:", "Check to use default built in ability effects. Alternative you can script your custom effect and have it spawn as the ability's EffectObject");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
            ability.useDefaultEffect = EditorGUI.Toggle(new Rect(startX + spaceX, startY, 40, height), ability.useDefaultEffect);

            if (ability.useDefaultEffect)
            {
                if (ability.requireTargetSelection)
                {
                    cont = new GUIContent(" - AOE Radius:", "The Area of Effective radius of the effect. Only target within the radius of the target position will be affected by the ability");
                    EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
                    ability.aoeRadius = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 40, height), ability.aoeRadius);
                }
                else
                {
                    cont = new GUIContent(" - AOE Radius:", "The Area of Effective radius of the effect. Only target within the radius of the target position will be affected by the ability");
                    EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
                    EditorGUI.LabelField(new Rect(startX + spaceX, startY, width, height), "not applicable");
                }

                cont = new GUIContent(" - Effect Delay:", "The delay in second before the effect actually hit after the ability is cast. This is mostly used to sync-up the visual effect with the effects.");
                EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
                ability.effectDelay = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 40, height), ability.effectDelay);
            }
            else
            {
                cont = new GUIContent(" - AOE Radius:", "The Area of Effective radius of the effect. Only target within the radius of the target position will be affected by the ability");
                EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
                EditorGUI.LabelField(new Rect(startX + spaceX, startY, width, height), "not applicable");

                cont = new GUIContent(" - Effect Delay:", "The delay in second before the effect actually hit after the ability is cast. This is mostly used to sync-up the visual effect with the effects.");
                EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
                EditorGUI.LabelField(new Rect(startX + spaceX, startY, width, height), "not applicable");
            }

            startY += 10;


            cont = new GUIContent("Custom Description:", "Check to use use custom description. If not, the default one (generated based on the effect) will be used");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
            ability.useCustomDesp = EditorGUI.Toggle(new Rect(startX + spaceX, startY, 40, height), ability.useCustomDesp);
            if (ability.useCustomDesp)
            {
                GUIStyle style = new GUIStyle("TextArea");
                style.wordWrap = true;
                cont = new GUIContent("Perk description (to be used in runtime): ", "");
                EditorGUI.LabelField(new Rect(startX, startY += spaceY, 400, 20), cont);
                ability.desp = EditorGUI.TextArea(new Rect(startX, startY + spaceY - 3, 270, 150), ability.desp, style);
            }


            if (ability.useDefaultEffect)
            {
                startY = cachedY;
                startX += 300;
                Vector2 v2 = DrawAbilityEffect(ability.effect, startX, startY);
                startX -= 300;
                startY = v2.y;
            }

            if(abilityList[selectID].prefab)
            {
                AbilityBehavior ab = abilityList[selectID].prefab.GetComponent<AbilityBehavior>();
                ab.ability = ability;

                EditorUtility.SetDirty(ab);
                EditorUtility.SetDirty(abilityList[selectID].prefab);
            }
            //UnitEditorWindow.DrawStat(ability.effect.stat, startX, startY, 700);


            float contWidth = spaceX + width;

            return new Vector2(contWidth, startY + 700);
        }


        Vector2 DrawAbilityEffect(AbilityEffect effect, float startX, float startY)
        {
            float spaceX = 110;

            EditorGUI.LabelField(new Rect(startX, startY, width, height), "Effects:");

            GUI.Box(new Rect(startX, startY += spaceY, spaceX + 95, 300), "");

            startX += 5;
            startY += 10;

            cont = new GUIContent("Duration:", "Duration of the effects. This is shared by all the effects that may have a duration (stun, dot, slot, buff)");
            EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
            effect.duration = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 40, height), effect.duration);

            effect.slow.duration = effect.duration;
            effect.dot.duration = effect.duration;

            startY += 10;

            cont = new GUIContent("Damage Min/Max:", "Damage to be done to target (creep only)");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
            effect.damageMin = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 40, height), effect.damageMin);
            effect.damageMax = EditorGUI.FloatField(new Rect(startX + spaceX + 40, startY, 40, height), effect.damageMax);

            cont = new GUIContent("Stun Chance:", "Chance to stun target (creep only). Takes value from 0-1 with 0.3 being 30% to stun the target");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
            effect.stunChance = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 40, height), effect.stunChance);


            //~ cont=new GUIContent("Slow:", "Slow speed multiplier to be applied to target (creep only). Takes value from 0-1 with with 0.7 being decrese default speed by 30%");
            cont = new GUIContent("Slow:", "Slow speed multiplier to be applied to target (creep only)");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont); startY -= 3;

            cont = new GUIContent(" - Slow Multiplier:", "Slow speed multiplier to be applied to target (creep only). Takes value from 0-1 with with 0.7 being decrese default speed by 30%");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
            effect.slow.slowMultiplier = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 40, height), effect.slow.slowMultiplier);


            cont = new GUIContent("Dot:", "Damage over time to be applied to target (creep only)");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont); startY -= 3;
            cont = new GUIContent(" - Interval:", "Duration between each tick. Damage is applied at each tick.");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
            effect.dot.interval = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 40, height), effect.dot.interval);

            cont = new GUIContent(" - Damage:", "Damage applied at each tick");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
            effect.dot.value = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 40, height), effect.dot.value);

            startY += 10;


            cont = new GUIContent("HP-Gain Min/Max:", "HP to restored to target (tower only)");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
            effect.HPGainMin = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 40, height), effect.HPGainMin);
            effect.HPGainMax = EditorGUI.FloatField(new Rect(startX + spaceX + 40, startY, 40, height), effect.HPGainMax);

            cont = new GUIContent("Buff:", "Buffs to be applied to the target (tower only)");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
            cont = new GUIContent(" - Damage Buff:", "Damage buff multiplier. Takes value from 0 and above with 0.4 being increase damage by 40%");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
            effect.damageBuff = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 40, height), effect.damageBuff);

            cont = new GUIContent(" - Range Buff:", "Range buff multiplier. Takes value from 0 and above with 0.3 being increase effective range by 30%");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
            effect.rangeBuff = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 40, height), effect.rangeBuff);

            cont = new GUIContent(" - Cooldown Buff:", "Cooldown buff multiplier. Takes value from 0 and above with 0.5 being decrease attack cooldown by 50%");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
            effect.cooldownBuff = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 40, height), effect.cooldownBuff);

            cont = new GUIContent(" - Dodge buff:", "Dodge buff multiplier. Takes value from 0 and above with 1 being avoid attack");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
            effect.dodgeChance = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 40, height), effect.dodgeChance);

            return new Vector2(startX, startY);
        }



        private Rect listVisibleRect;
        private Rect listContentRect;

        private int deleteID = -1;
        private bool minimiseList = false;
        Vector2 DrawAbilityList(float startX, float startY)
        {

            float width = 260;
            if (minimiseList) width = 60;


            if (!minimiseList)
            {
                if (GUI.Button(new Rect(startX + 180, startY - 20, 40, 18), "up"))
                {
                    if (selectID > 0)
                    {
                        Ability ability = abilityList[selectID];
                        abilityList[selectID] = abilityList[selectID - 1];
                        abilityList[selectID - 1] = ability;
                        selectID -= 1;

                        if (selectID * 35 < scrollPos1.y) scrollPos1.y = selectID * 35;
                    }
                }
                if (GUI.Button(new Rect(startX + 222, startY - 20, 40, 18), "down"))
                {
                    if (selectID < abilityList.Count - 1)
                    {
                        Ability ability = abilityList[selectID];
                        abilityList[selectID] = abilityList[selectID + 1];
                        abilityList[selectID + 1] = ability;
                        selectID += 1;

                        if (listVisibleRect.height - 35 < selectID * 35) scrollPos1.y = (selectID + 1) * 35 - listVisibleRect.height + 5;
                    }
                }
            }


            listVisibleRect = new Rect(startX, startY, width + 15, window.position.height - startY - 5);
            listContentRect = new Rect(startX, startY, width, abilityList.Count * 35 + 5);

            GUI.color = new Color(.8f, .8f, .8f, 1f);
            GUI.Box(listVisibleRect, "");
            GUI.color = Color.white;

            scrollPos1 = GUI.BeginScrollView(listVisibleRect, scrollPos1, listContentRect);

            startY += 5; startX += 5;

            for (int i = 0; i < abilityList.Count; i++)
            {

                EditorUtilities.DrawSprite(new Rect(startX, startY + (i * 35), 30, 30), abilityList[i].icon);

                if (minimiseList)
                {
                    if (selectID == i) GUI.color = new Color(0, 1f, 1f, 1f);
                    if (GUI.Button(new Rect(startX + 35, startY + (i * 35), 30, 30), "")) SelectAbility(i);
                    GUI.color = Color.white;

                    continue;
                }



                if (selectID == i) GUI.color = new Color(0, 1f, 1f, 1f);
                if (GUI.Button(new Rect(startX + 35, startY + (i * 35), 150, 30), abilityList[i].name)) SelectAbility(i);
                GUI.color = Color.white;

                if (deleteID == i)
                {

                    if (GUI.Button(new Rect(startX + 190, startY + (i * 35), 60, 15), "cancel")) deleteID = -1;

                    GUI.color = Color.red;
                    if (GUI.Button(new Rect(startX + 190, startY + (i * 35) + 15, 60, 15), "confirm"))
                    {
                        if (selectID >= deleteID) SelectAbility(Math.Max(0, selectID - 1));
                        abilityList.RemoveAt(deleteID);
                        deleteID = -1;
                    }
                    GUI.color = Color.white;
                }
                else
                {
                    if (GUI.Button(new Rect(startX + 190, startY + (i * 35), 60, 15), "remove")) deleteID = i;
                }
            }

            GUI.EndScrollView();

            return new Vector2(startX + width, startY);
        }
    }
}