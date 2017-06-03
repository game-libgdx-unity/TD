using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnitedSolution
{
    public abstract class BaseEditorWindow<T, V> : EditorWindow where T : EditorWindow where V : Unit
    {
        protected List<V> units = new List<V>();
        protected int selectedIndex = 0;

        public static T instance;
        public static void Init(string title)
        {
            // Get existing open window or if none, make a new one:
            instance = (T)EditorWindow.GetWindow(typeof(T), false, title);
            instance.minSize = new Vector2(480, 620);
            instance.SendEvent(new Event(101));
        }

        protected virtual void OnGUI()
        {
            if (!setupWindow)
            {
                Initialize();
                setupWindow = true;
            }
            GUI.changed = false;

        }

        protected abstract void Initialize();

        protected void AddUnit(V unit)
        {
            units.Add(unit);

        }

        protected bool setupWindow = false;
        protected T Window { get { return instance; } }
        protected V SelectedUnit { get { return units[selectedIndex]; } }

        protected Dictionary<string, bool> showFoldOutFlags = new Dictionary<string, bool>();
        protected bool inFoldout;

        protected void DrawFoldOut(string label, System.Action draw)
        {
            if (!showFoldOutFlags.ContainsKey(label))
            {
                showFoldOutFlags.Add(label, false);
            }
            BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
            showFoldOutFlags[label] = EditorGUILayout.Foldout(showFoldOutFlags[label], label);
            EndHorizontal();
            inFoldout = true;
            if (showFoldOutFlags[label] && draw != null) draw();
            inFoldout = false;

        }

        protected void EndHorizontal()
        {
            EditorGUILayout.EndHorizontal();
        }

        protected void BeginHorizontal()
        {
            EditorGUILayout.BeginHorizontal();
        }

        protected void Space()
        {
            EditorGUILayout.Space();
        }
        
        protected void DrawObject(string label, Object obj)
        {
            if (inFoldout)
            {
                StringBuilder sb = new StringBuilder("   ");
                label = sb.Append(label).ToString();
            }

            obj = EditorGUILayout.ObjectField(label, obj, obj.GetType());
        }

        protected void DrawButton(string label, System.Action onClicked)
        {
            if (GUILayout.Button(label))
            {
                onClicked();
            }
        }

        protected void DrawTextfield(string label, ref string text)
        {
            text = EditorGUILayout.TextField(label, text);
        }

        protected void DrawLayer(string label, ref LayerMask customMask)
        {
            customMask = EditorGUILayout.LayerField(label, customMask.value);
        }

        protected void DrawToggle(string label, ref bool value)
        {
            value = EditorGUILayout.Toggle(label, value);
        }
        protected void DrawFloat(string label, ref float value)
        {
            value = EditorGUILayout.FloatField(label, value);
        }

        protected int DrawInt(string label, ref int value)
        {
            value = EditorGUILayout.IntField(label, value);
            return value;
        }

        protected void DrawSlider(string label, ref float value, float min, float max)
        {
            value = EditorGUILayout.Slider(label, value, min, max);
        }
    }
}
