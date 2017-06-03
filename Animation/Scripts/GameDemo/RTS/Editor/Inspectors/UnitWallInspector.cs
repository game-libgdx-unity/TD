using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnitedSolution
{ 

    [CustomEditor(typeof(UnitWall))]
    public class UnitWallInspector : BaseInspector
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("Show In Editor"))
            {
                UnitTowerEditorWindow.Init();
                UnitTowerEditorWindow.window.Select(((UnitTower)target).unitName);
            }

            UnitWall unit = (UnitWall)target;
            unit.deadEffectObj = (GameObject)EditorGUILayout.ObjectField("Dead Effect:",  unit.deadEffectObj, typeof(GameObject));
            unit.delay = EditorGUILayout.FloatField("Delay:", unit.delay);
            base.OnInspectorGUI(); 
        }
    }
}
