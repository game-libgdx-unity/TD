using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnitedSolution
{
    [CanEditMultipleObjects]
    public class BaseInspector : Editor
    {
        protected Dictionary<string, bool> showFoldOutFlags = new Dictionary<string, bool>();

        public override void OnInspectorGUI()
        {
            DrawButtons();

            DrawFoldOut("Show Default Inspector", () => DrawDefaultInspector(), false);
        }

        protected void DrawListObject<T>(string label, string elementName, List<T> objects) where T : UnityEngine.Object
        {
            int objCount = objects.Count;
            label = "No. " + label;
            //label = CheckInFoldout(label);
            DrawInt(label, ref objCount);

            if (objCount != objects.Count)
            {
                while (objects.Count < objCount) objects.Add(null);
                while (objects.Count > objCount) objects.RemoveAt(objects.Count - 1);
            }

            EditorGUILayout.BeginHorizontal();
            DrawButton("Clone", () => { if (objCount > 0) objects.Add(objects[objects.Count - 1]); else objects.Add(null); });
            DrawButton("Remove First", () => { if (objCount > 0) objects.RemoveAt(0); });
            DrawButton("Remove Last", () => { if (objCount > 0) objects.RemoveAt(objects.Count - 1); });
            EndHorizontal();

            if (objects.Count > 0)
                DrawFoldOut(label, () =>
                {
                    for (int i = 0; i < objects.Count; i++)
                    {
                        objects[i] = (T)EditorGUILayout.ObjectField(elementName + i, objects[i], typeof(T), true);
                    }
                });
        }

        protected void DrawList<T>(string label, string elementName, List<T> objects, Action<T> DrawElement) where T : ICloneable<T>
        {
            T defaultElement = default(T);
            int objCount = objects.Count;
            label = "List size:";
            //label = CheckInFoldout(label);
            DrawInt(label, ref objCount);

            if (objCount != objects.Count)
            {
                while (objects.Count < objCount) objects.Add(defaultElement);
                while (objects.Count > objCount) objects.RemoveAt(objects.Count - 1);
            }

            EditorGUILayout.BeginHorizontal();
            if (objCount > 0)
                DrawButton("Clone", () => { objects.Add(objects[objects.Count - 1].Clone()); });
            else
                DrawButton("Create", () => { objects.Add(defaultElement); });

            DrawButton("Remove First", () => { if (objCount > 0) objects.RemoveAt(0); });
            DrawButton("Remove Last", () => { if (objCount > 0) objects.RemoveAt(objects.Count - 1); });
            EndHorizontal();

            if (objects.Count > 0)
                DrawFoldOut(label, () =>
                {
                    for (int i = 0; i < objects.Count; i++)
                    {
                        DrawElement(objects[i]);
                    }
                });
        }

        protected void DrawButtons()
        {
            BeginHorizontal();
            if (GUILayout.Button("Show In Editor"))
            {
                if (target is UnitTower)
                {
                    UnitTowerEditorWindow.Init();
                    UnitTowerEditorWindow.window.Select(((Unit)target).unitName);
                }
                if (target is UnitCreep)
                {
                    UnitCreepEditorWindow.Init();
                    UnitCreepEditorWindow.window.Select(((Unit)target).unitName);
                }
            }

            if (GUILayout.Button("Apply"))
            {
                //PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                EditorUtility.SetDirty(target);
                EditorUtility.SetDirty(PrefabUtility.GetPrefabObject(target));
                AssetDatabase.SaveAssets();
            }
            EndHorizontal();
        }

        protected Transform DrawTransform(string label, UnityEngine.Transform transform)
        {
            label = CheckInFoldout(label);

            transform = (Transform)EditorGUILayout.ObjectField(label: label, obj: transform, objType: typeof(Transform), allowSceneObjects: true);
            return transform;
        }

        protected GameObject DrawGameObject(string label, UnityEngine.GameObject gameObject)
        {
            label = CheckInFoldout(label);
            gameObject = (GameObject)EditorGUILayout.ObjectField(label: label, obj: gameObject, objType: typeof(GameObject), allowSceneObjects: true);
            return gameObject;
        }

        protected T DrawGameObject<T>(string label, ref T gameObject) where T : Component
        {
            label = CheckInFoldout(label);
            gameObject = (T)EditorGUILayout.ObjectField(label: label, obj: gameObject, objType: typeof(T), allowSceneObjects: true);
            return gameObject;
        }

        protected void DrawGO(string label, ref UnityEngine.Object gameObject)
        {
            label = CheckInFoldout(label);
            gameObject = (UnityEngine.Object)EditorGUILayout.ObjectField(label: label, obj: gameObject, objType: typeof(UnityEngine.Object), allowSceneObjects: true);
        }

        protected void DrawFoldOut(string label, Action draw, bool foldout = true)
        {
            if (!showFoldOutFlags.ContainsKey(label))
            {
                showFoldOutFlags.Add(label, foldout);
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
            showFoldOutFlags[label] = EditorGUILayout.Foldout(showFoldOutFlags[label], label, true);
            EditorGUILayout.EndHorizontal();
            inFoldout = true;
            if (showFoldOutFlags[label] && draw != null) draw();
            inFoldout = false;
        }

        protected void DrawButton(string label, System.Action onClicked)
        {
            if (GUILayout.Button(label))
            {
                onClicked();
            }
        }
        protected void DrawTextfield(string label, ref string unitName)
        {
            label = CheckInFoldout(label);
            unitName = EditorGUILayout.TextField(label, unitName);
        }
        static List<string> layers;
        static string[] layerNames;
        private bool inFoldout;

        protected void DrawMask(string label, ref LayerMask mask)
        {
            label = CheckInFoldout(label);
            if (layers == null)
            {
                layers = new List<string>();
                layerNames = new string[4];
            }
            else
            {
                layers.Clear();
            }

            int emptyLayers = 0;
            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);

                if (layerName != "")
                {

                    for (; emptyLayers > 0; emptyLayers--) layers.Add("Layer " + (i - emptyLayers));
                    layers.Add(layerName);
                }
                else
                {
                    emptyLayers++;
                }
            }

            if (layerNames.Length != layers.Count)
            {
                layerNames = new string[layers.Count];
            }
            for (int i = 0; i < layerNames.Length; i++) layerNames[i] = layers[i];
            mask.value = EditorGUILayout.MaskField(label, mask.value, layerNames);
        }
        protected void EndHorizontal()
        {
            EditorGUILayout.EndHorizontal();
        }
        protected LayerMask DrawLayer(string label, ref LayerMask customLayer)
        {
            label = CheckInFoldout(label);
            customLayer.value = EditorGUILayout.LayerField(label, customLayer.value);
            return customLayer;
        }

        private string CheckInFoldout(string label)
        {
            if (inFoldout)
            {
                StringBuilder sb = new StringBuilder("     ");
                label = sb.Append(label).ToString();
            }

            return label;
        }

        protected void BeginHorizontal()
        {
            EditorGUILayout.BeginHorizontal();
            //if (inFoldout)
            //{
            //    EditorGUILayout.LabelField("    ");
            //}
        }

        protected void Space()
        {
            EditorGUILayout.Space();
        }

        protected void DrawLayer2(string label, ref LayerMask customMask)
        {
            label = CheckInFoldout(label);
            customMask = EditorGUILayout.LayerField(label, customMask.value);
        }

        protected int DrawLayer(string label, int customMask)
        {
            label = CheckInFoldout(label);
            customMask = EditorGUILayout.LayerField(label, customMask);
            return customMask;
        }

        protected bool DrawToggle(string label, ref bool value)
        {
            label = CheckInFoldout(label);
            value = EditorGUILayout.Toggle(label, value);
            return value;
        }
        protected void DrawFloat(string label, ref float value)
        {
            label = CheckInFoldout(label);
            value = EditorGUILayout.FloatField(label, value);
        }

        protected int DrawInt(string label, ref int value)
        {
            label = CheckInFoldout(label);
            value = EditorGUILayout.IntField(label, value);
            return value;
        }

        protected void DrawSlider(string label, ref float value, float min, float max)
        {
            label = CheckInFoldout(label);
            value = EditorGUILayout.Slider(label, value, min, max);
        }
    }
}
