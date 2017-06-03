using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnitedSolution
{ 

    [CustomEditor(typeof(UnitDefender))]
    public class DefenderInspector : UnitInspector
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            DrawButtons();
            DrawUnitGUI();

            UnitDefender unit = (UnitDefender)target;

            unit.moveSpeed = EditorGUILayout.IntField("Move Speed:", unit.moveSpeed);
            unit.rotateSpd = EditorGUILayout.FloatField("Rotation Speed:", unit.rotateSpd);
            unit.allowWandering = EditorGUILayout.Toggle("Allow wandering:", unit.allowWandering);
            
            if (unit.allowWandering)
            {
                unit.timeToNextMove = EditorGUILayout.FloatField("Time To Next Move:", unit.timeToNextMove);
                unit.maxDistance = EditorGUILayout.FloatField("Max Distance:", unit.maxDistance);
            }
            if (unit.behaviour == Behaviour.TacticallyMove)
            {
                unit.evasionRange = EditorGUILayout.FloatField("Evasion Range:", unit.evasionRange);
            }
            base.OnInspectorGUI(); 
        }
    }
}
