using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnitedSolution
{

    [CustomEditor(typeof(UnitHero))]
    public class UnitHeroInspector : UnitTowerInspector
    {
        public override void OnInspectorGUI()
        {
            UnitHero hero = (UnitHero)target;
            //EditorGUILayout.Space();

            //DrawLayer("Layer:", ref hero.customMask);
            DrawToggle("Controllable:", ref hero.controlable);
            base.OnInspectorGUI();
        }
    }
}
