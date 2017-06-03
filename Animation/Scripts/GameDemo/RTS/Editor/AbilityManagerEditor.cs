using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution
{

    [CustomEditor(typeof(AbilityManager))]
    public class AbilityManagerEditor : Editor
    {

        private static AbilityManager instance;

        private bool showDefaultFlag = false;
        private bool showAbilityList = true;

        private static List<Ability> abilityList = new List<Ability>();

        private GUIContent cont;

        void Awake()
        {
            instance = (AbilityManager)target;

            GetAbility();

            EditorUtility.SetDirty(instance);
        }

        private static void GetAbility()
        {
            EditorDBManager.Init();

            abilityList = EditorDBManager.GetAbilityList();

            if (Application.isPlaying) return;

            List<int> abilityIDList = EditorDBManager.GetAbilityIDList();
            for (int i = 0; i < instance.unavailableIDList.Count; i++)
            {
                if (!abilityIDList.Contains(instance.unavailableIDList[i]))
                {
                    instance.unavailableIDList.RemoveAt(i); i -= 1;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            GUI.changed = false;

            EditorGUILayout.Space();

            cont = new GUIContent("Energy Capacity:", "The maximum amount of energy available to player. Energy is used to cast ability and is recharged overtime");
            instance.fullEnergy = EditorGUILayout.FloatField(cont, instance.fullEnergy);

            cont = new GUIContent("Energy Rate:", "The amount of energy to be recharged at each second");
            instance.energyRate = EditorGUILayout.FloatField(cont, instance.energyRate);

            cont = new GUIContent("Full Energy Start:", "Check to start the game with full energy");
            instance.startWithFullEnergy = EditorGUILayout.Toggle(cont, instance.startWithFullEnergy);

            if (!instance.startWithFullEnergy)
            {
                cont = new GUIContent(" - Starting Energy:", "The amount of energy player possess at the start of the game");
                instance.energy = EditorGUILayout.FloatField(cont, instance.energy);
            }

            cont = new GUIContent("Charge Before Spawn:", "Check to start energy charging before spawning");
            instance.onlyChargeOnSpawn = EditorGUILayout.Toggle(cont, instance.onlyChargeOnSpawn);

            EditorGUILayout.Space();

            cont = new GUIContent("Default Indicator:", "The default cursor indicator to used for ability target selecting in case the selected ability doesnt have a designated indicator");
            instance.defaultIndicator = (Transform)EditorGUILayout.ObjectField(cont, instance.defaultIndicator, typeof(Transform), true);


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
            showAbilityList = EditorGUILayout.Foldout(showAbilityList, "Show Ability List");
            EditorGUILayout.EndHorizontal();
            if (showAbilityList)
            {

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("EnableAll") && !Application.isPlaying)
                {
                    instance.unavailableIDList = new List<int>();
                }
                if (GUILayout.Button("DisableAll") && !Application.isPlaying)
                {
                    instance.unavailableIDList = new List<int>();
                    for (int i = 0; i < abilityList.Count; i++) instance.unavailableIDList.Add(abilityList[i].ID);
                }
                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < abilityList.Count; i++)
                {
                    Ability ability = abilityList[i];

                    if (ability.disableInAbilityManager)
                    {
                        if (!instance.unavailableIDList.Contains(ability.ID)) instance.unavailableIDList.Add(ability.ID);
                        continue;
                    }

                    GUILayout.BeginHorizontal();

                    GUILayout.Box("", GUILayout.Width(40), GUILayout.Height(40));
                    Rect rect = GUILayoutUtility.GetLastRect();
                    EditorUtilities.DrawSprite(rect, ability.icon, false);

                    GUILayout.BeginVertical();
                    EditorGUILayout.Space();
                    GUILayout.Label(ability.name, GUILayout.ExpandWidth(false));

                    GUILayout.BeginHorizontal();

                    bool flag = !instance.unavailableIDList.Contains(ability.ID) ? true : false;
                    if (Application.isPlaying) flag = !flag;    //switch it around in runtime
                    EditorGUILayout.LabelField(new GUIContent(" - enabled: ", "check to enable the ability in this level"), GUILayout.Width(70));
                    flag = EditorGUILayout.Toggle(flag);

                    if (!Application.isPlaying)
                    {
                        if (flag) instance.unavailableIDList.Remove(ability.ID);
                        else
                        {
                            if (!instance.unavailableIDList.Contains(ability.ID)) instance.unavailableIDList.Add(ability.ID);
                        }
                    }

                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                }

            }

            EditorGUILayout.Space();


            if (GUILayout.Button("Open AbilityEditor"))
            {
                AbilityEditorWindow.Init();
            }


            if (GUI.changed) EditorUtility.SetDirty(instance);
        }
    }

}