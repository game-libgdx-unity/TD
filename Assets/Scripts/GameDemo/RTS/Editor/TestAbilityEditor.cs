//using System.Collections;
//using System.Collections.Generic;
//using UnitedSolution;
//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(AbilityBehavior))]
//public class TestAbilityEditor : Editor
//{

//    private GUIContent cont;

//    private int selectID = 0;

//    private Vector2 scrollPos1;
//    private Vector2 scrollPos2;

//    private GUIContent[] contL;

//    private float contentHeight = 0;
//    private float contentWidth = 0;

//    private float startX = 120;
//    private float startY = 5;
//    private float spaceX = 120;
//    private float spaceY = 20;
//    private float width = 150;
//    private float height = 18;
//    private bool showDefaultFlag;

//    public override void OnInspectorGUI()
//    {
//        AbilityBehavior ability = (AbilityBehavior)target;
//        scrollPos2 = GUI.BeginScrollView(new Rect(0, startX, 900, 300), scrollPos2, new Rect(0, startX, 470, 1000), false, false);
//        DrawAbilityConfigurator(startY, startX, ability.ability);
//        //base.OnInspectorGUI();
//    }

//    void DrawAbilityConfigurator(float startX, float startY, Ability ability)
//    {

//        float originX = startX;
//        float originY = startY;
//        //float cachedX=startX;
//        float cachedY = startY;

//        //EditorUtilities.DrawSprite(new Rect(startX, startY, 60, 60), ability.icon);
//        startX += 65;

//        cont = new GUIContent("Name:", "The ability name to be displayed in game");
//        EditorGUI.LabelField(new Rect(startX, startY += spaceY / 2, width, height), cont);
//        ability.name = EditorGUI.TextField(new Rect(startX + spaceX - 65, startY, width - 5, height), ability.name);

//        cont = new GUIContent("Selfcast:", "When checked, Selfcast ability");
//        EditorGUI.LabelField(new Rect(startX + 235, startY, width + 10, height), cont);
//        ability.selfCast = EditorGUI.Toggle(new Rect(startX + 290, startY, 40, height), ability.selfCast);

//        if (!ability.selfCast)
//        {
//            cont = new GUIContent("At Caster:", "Cast at position of Caster");
//            EditorGUI.LabelField(new Rect(startX + 310, startY, width + 10, height), cont);
//            ability.castAtCaster = EditorGUI.Toggle(new Rect(startX + 365, startY, 40, height), ability.castAtCaster);
//        }

//        cont = new GUIContent("Layer:", "Layer of targetable object");
//        EditorGUI.LabelField(new Rect(startX + 235, startY += spaceY, width + 10, height), cont);
//        ability.customMask = EditorUtils.LayerMaskField(new Rect(startX + 290, startY, 100, height), "", ability.customMask);

//        //ability.customMask = EditorGUI.IntField(new Rect(startX + 390, startY+40, 185, height), ability.customMask);

//        cont = new GUIContent("Icon:", "The ability icon to be displayed in game, must be a sprite");
//        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
//        ability.icon = (Sprite)EditorGUI.ObjectField(new Rect(originX, originY, 60, 60), ability.icon, typeof(Sprite), false);

//        cont = new GUIContent("Disable in AbilityManager:", "When checked, the ability won't appear on AbilityManager list and thus can't be access from the get go\nThis is to mark ability that can only be unlocked from perk");
//        EditorGUI.LabelField(new Rect(startX + 235, startY, width + 10, height), cont);
//        ability.disableInAbilityManager = EditorGUI.Toggle(new Rect(startX + 365, startY, 185, height), ability.disableInAbilityManager);

//        startX -= 65;
//        startY += 10 + spaceY / 2;

//        cachedY = startY + spaceY;

//        cont = new GUIContent("Cost:", "The energy cost to use the ability");
//        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
//        ability.cost = EditorGUI.IntField(new Rect(startX + spaceX, startY, 40, height), ability.cost);

//        cont = new GUIContent("Cooldown:", "The cooldown duration of the ability. Once used, the ability cannot be used again until the cooldown duration has passed");
//        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
//        ability.cooldown = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 40, height), ability.cooldown);


//        cont = new GUIContent("Require Targeting:", "Check if ability need a specific position or unit as target. When checked, the user will need to select a position/unit before the ability can be cast. Otherwise the ability be cast without a target position/unit. If ability uses default effects and this is unchecked, the effect will be apply to all unit in the game");
//        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
//        ability.requireTargetSelection = EditorGUI.Toggle(new Rect(startX + spaceX, startY, 40, height), ability.requireTargetSelection);

//        if (ability.requireTargetSelection)
//        {
//            cont = new GUIContent(" - Single Unit Only:", "Check if the ability require a specific unit as a target. Otherwise the ability can be cast anywhere without a specific target");
//            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
//            ability.singleUnitTargeting = EditorGUI.Toggle(new Rect(startX + spaceX, startY, 40, height), ability.singleUnitTargeting);

//            if (ability.singleUnitTargeting)
//            {
//                //~ cont=new GUIContent(" - Target Friendly:", "Check if the ability is meant to target firendly unit. Otherwise it will target");
//                //~ EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//                //~ ability.targetType=EditorGUI.Toggle(new Rect(startX+spaceX, startY, 40, height), ability.targetType);

//                int targetType = (int)ability.targetType;
//                cont = new GUIContent(" - Target :", "Determine which type of unit the tower can target. Hostile for hostile unit. Friendly for friendly unit. Hybrid for both.");
//                EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
//                //contL = new GUIContent[targetTypeLabel.Length];
//                //for (int i = 0; i < contL.Length; i++) contL[i] = new GUIContent(targetTypeLabel[i], targetTypeTooltip[i]);
//                //targetType = EditorGUI.Popup(new Rect(startX + spaceX, startY, width - 20, height), new GUIContent(""), targetType, contL);
//                //ability.targetType = (Ability._TargetType)targetType;
//            }

//            cont = new GUIContent(" - Indicator:", "(Optional) The cursor indicator that used to indicate the ability target position during target selection phase for the ability. If left unassigned, the default indicator specified in the AbilityManager will be used instead");
//            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
//            ability.indicator = (Transform)EditorGUI.ObjectField(new Rect(startX + spaceX, startY, width, height), ability.indicator, typeof(Transform), false);

//            if (ability.indicator == null)
//            {
//                cont = new GUIContent(" - Scale Indicator:", "Automatically scale the indicator size to match the aoeRadius of the ability, or a unit width in case of a single unit targeting. Only applicable if ability is using default effects");
//                EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
//                ability.autoScaleIndicator = EditorGUI.Toggle(new Rect(startX + spaceX, startY, 40, height), ability.autoScaleIndicator);
//            }
//            else
//            {
//                cont = new GUIContent(" - Scale Indicator:", "Automatically scale the indicator size to match the aoeRadius of the ability. Only applicable if ability is using default effects");
//                EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
//                EditorGUI.LabelField(new Rect(startX + spaceX, startY, width, height), "not applicable");
//            }
//        }
//        else
//        {
//            cont = new GUIContent(" - Targets Unit:", "Check if the unit is immuned to critical hit");
//            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
//            EditorGUI.LabelField(new Rect(startX + spaceX, startY, width, height), "not applicable");

//            cont = new GUIContent(" - Indicator:", "(Optional) The cursor indicator that used to indicate the ability target position during target selection phase for the ability. If left unassigned, the default indicator specified in the AbilityManager will be used instead");
//            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
//            EditorGUI.LabelField(new Rect(startX + spaceX, startY, width, height), "not applicable");

//            cont = new GUIContent(" - Scale Indicator:", "Automatically scale the indicator size to match the aoeRadius of the ability. Only applicable if ability is using default effects");
//            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
//            EditorGUI.LabelField(new Rect(startX + spaceX, startY, width, height), "not applicable");
//        }


//        cont = new GUIContent("Max Use Count:", "The maximum amount which the ability can be used in a level. Indicate unlimited usage when set to <0");
//        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
//        ability.maxUseCount = EditorGUI.IntField(new Rect(startX + spaceX, startY, 40, height), ability.maxUseCount);


//        cont = new GUIContent("Effect Object:", "The effect object spawned at the selected position when the ability is used. This object can contain custom script to do custom effect for the ability");
//        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
//        ability.effectObj = (GameObject)EditorGUI.ObjectField(new Rect(startX + spaceX, startY, width, height), ability.effectObj, typeof(GameObject), false);




//        cont = new GUIContent("Use Default Effect:", "Check to use default built in ability effects. Alternative you can script your custom effect and have it spawn as the ability's EffectObject");
//        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
//        ability.useDefaultEffect = EditorGUI.Toggle(new Rect(startX + spaceX, startY, 40, height), ability.useDefaultEffect);

//        if (ability.useDefaultEffect)
//        {
//            if (ability.requireTargetSelection)
//            {
//                cont = new GUIContent(" - AOE Radius:", "The Area of Effective radius of the effect. Only target within the radius of the target position will be affected by the ability");
//                EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
//                ability.aoeRadius = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 40, height), ability.aoeRadius);
//            }
//            else
//            {
//                cont = new GUIContent(" - AOE Radius:", "The Area of Effective radius of the effect. Only target within the radius of the target position will be affected by the ability");
//                EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
//                EditorGUI.LabelField(new Rect(startX + spaceX, startY, width, height), "not applicable");
//            }

//            cont = new GUIContent(" - Effect Delay:", "The delay in second before the effect actually hit after the ability is cast. This is mostly used to sync-up the visual effect with the effects.");
//            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
//            ability.effectDelay = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 40, height), ability.effectDelay);
//        }
//        else
//        {
//            cont = new GUIContent(" - AOE Radius:", "The Area of Effective radius of the effect. Only target within the radius of the target position will be affected by the ability");
//            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
//            EditorGUI.LabelField(new Rect(startX + spaceX, startY, width, height), "not applicable");

//            cont = new GUIContent(" - Effect Delay:", "The delay in second before the effect actually hit after the ability is cast. This is mostly used to sync-up the visual effect with the effects.");
//            EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
//            EditorGUI.LabelField(new Rect(startX + spaceX, startY, width, height), "not applicable");
//        }

//        startY += 10;


//        cont = new GUIContent("Custom Description:", "Check to use use custom description. If not, the default one (generated based on the effect) will be used");
//        EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
//        ability.useCustomDesp = EditorGUI.Toggle(new Rect(startX + spaceX, startY, 40, height), ability.useCustomDesp);
//        if (ability.useCustomDesp)
//        {
//            GUIStyle style = new GUIStyle("TextArea");
//            style.wordWrap = true;
//            cont = new GUIContent("Perk description (to be used in runtime): ", "");
//            EditorGUI.LabelField(new Rect(startX, startY += spaceY, 400, 20), cont);
//            ability.desp = EditorGUI.TextArea(new Rect(startX, startY + spaceY - 3, 270, 150), ability.desp, style);
//        }

//        EditorGUILayout.Space();

//        EditorGUILayout.BeginHorizontal();
//        EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
//        showDefaultFlag = EditorGUILayout.Foldout(showDefaultFlag, "Show default editor");
//        EditorGUILayout.EndHorizontal();
//        if (showDefaultFlag) DrawDefaultInspector();

//        //if (GUI.changed) EditorUtility.SetDirty(ability.prefab);
//    }
//}
