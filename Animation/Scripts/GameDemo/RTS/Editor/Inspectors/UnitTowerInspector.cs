using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnitedSolution
{ 

    [CustomEditor(typeof(UnitTower))]
    public class UnitTowerInspector : BaseInspector
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("Show In Editor"))
            {
                UnitTowerEditorWindow.Init();
                UnitTowerEditorWindow.window.Select(((UnitTower)target).unitName);
            }

            base.OnInspectorGUI();
        }
    }
}
