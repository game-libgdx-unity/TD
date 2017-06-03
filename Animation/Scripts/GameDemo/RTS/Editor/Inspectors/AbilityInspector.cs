using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnitedSolution
{ 

    [CustomEditor(typeof(AbilityBehavior))]
    public class AbilityInspector : BaseInspector
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            AbilityBehavior abilityHolder = (AbilityBehavior)target;
            Ability ability = abilityHolder.ability;

            if (ability == null)
                return;

            if (GUILayout.Button("Show In Editor"))
            {
                AbilityEditorWindow.Init();
                AbilityEditorWindow.window.Select(ability.name, abilityHolder.gameObject);
            }

            //EditorGUI.BeginChangeCheck();


            //ability.name = EditorGUILayout.TextField("Name:", ability.name);

            //if(EditorGUI.EndChangeCheck())
            //{
            //    EditorUtility.SetDirty(abilityHolder);
            //    AbilityEditorWindow.Init();
            //}

            //base.OnInspectorGUI(); 
        }
    }
}
