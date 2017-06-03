using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnitedSolution
{
    [CustomEditor(typeof(UnitCreep))]
    [CanEditMultipleObjects]
    public class UnitCreepInspector : UnitInspector
    {
        public override void OnInspectorGUI()
        {
            UnitCreep unit = (UnitCreep)target;
            DrawButtons();
            DrawUnitGUI();
            DrawFoldOut("Creep Behavior: ", () =>
            {
                DrawToggle("Fight back:", ref unit.alarmWhenGotAttacked);
            });

            DrawFoldOut("Movement Behavior: ", () =>
            {
                DrawToggle("Flying:", ref unit.flying);
                DrawFloat("Move Speed:", ref unit.moveSpeed);
                DrawFloat("Rotation Speed:", ref unit.rotateSpd);
                DrawListObject<Unit>("Targets", "Target", unit.tgtList);
            });

            base.OnInspectorGUI();
        }
    }
}
