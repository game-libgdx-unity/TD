using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnitedSolution
{

    [CustomEditor(typeof(   SelfDeactivator))]
    public class SelfDeactivatorInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            SelfDeactivator sa = (SelfDeactivator)(target);
            sa.useObjectPool = EditorGUILayout.Toggle("Use pool: ", sa.useObjectPool);
            sa.duration = EditorGUILayout.FloatField("Duration: ", sa.duration);

        }
    }
}
