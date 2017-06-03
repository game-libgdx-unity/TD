using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution{

	[CustomEditor(typeof(CameraControl))]
	public class CameraControlEditor : Editor {

		private static CameraControl instance;
		
		private static bool showDefaultFlag=false;
		
		
		private GUIContent cont;
		//private GUIContent[] contList;
		
		
		void Awake(){
			instance = (CameraControl)target;
			
			
			
			EditorUtility.SetDirty(instance);
		}
		
		private float width=116;
		
		public override void OnInspectorGUI(){
			
			GUI.changed = false;
			
			EditorGUILayout.Space();
			
			cont=new GUIContent("Pan Speed:", "The speed at which the camera pans on the horizontal axis");
			instance.panSpeed=EditorGUILayout.FloatField(cont, instance.panSpeed);
			
			cont=new GUIContent("Zoom Speed:", "The speed at witch the camera zooms");
			instance.zoomSpeed=EditorGUILayout.FloatField(cont, instance.zoomSpeed);
			
			EditorGUILayout.Space();
			
			width=150;
			
			#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
				
				EditorGUILayout.BeginHorizontal();
					cont=new GUIContent("EnableTouchPan:", "Check to enable finger drag on screen to pan the camera");
					EditorGUILayout.LabelField(cont, GUILayout.Width(width));
					instance.enableTouchPan=EditorGUILayout.Toggle(instance.enableTouchPan);
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
					cont=new GUIContent("EnableTouchRotate:", "Check to enable two fingers drag on screen to rotate the camera angle");
					EditorGUILayout.LabelField(cont, GUILayout.Width(width));
					instance.enableTouchRotate=EditorGUILayout.Toggle(instance.enableTouchRotate);
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
					cont=new GUIContent("enableTouchZoom:", "Check to enable two fingers pinching on screen to rotate the camera angle");
					EditorGUILayout.LabelField(cont, GUILayout.Width(width));
					instance.enableTouchZoom=EditorGUILayout.Toggle(instance.enableTouchZoom);
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
					cont=new GUIContent("RotateSensitivity:", "The input sensitivity to the rotate input (two fingers drag)");
					EditorGUILayout.LabelField(cont, GUILayout.Width(width));
					instance.rotationSpeed=EditorGUILayout.FloatField(instance.rotationSpeed);
				EditorGUILayout.EndHorizontal();
				
			#else
				
				EditorGUILayout.BeginHorizontal();
					cont=new GUIContent("EnableKeyPanning:", "Check to enable camera panning using 'wasd' key");
					EditorGUILayout.LabelField(cont, GUILayout.Width(width));
					instance.enableKeyPanning=EditorGUILayout.Toggle(instance.enableKeyPanning);
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
					cont=new GUIContent("EnableMousePanning:", "Check to enable camera panning when the mouse cursor is moved to the edge of the screen");
					EditorGUILayout.LabelField(cont, GUILayout.Width(width));
					instance.enableMousePanning=EditorGUILayout.Toggle(instance.enableMousePanning);
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
					cont=new GUIContent("EnableMouseRotate:", "Check to enable right-mouse-click drag to rotate the camera angle");
					EditorGUILayout.LabelField(cont, GUILayout.Width(width));
					instance.enableMouseRotate=EditorGUILayout.Toggle(instance.enableMouseRotate);
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
					cont=new GUIContent("EnableMouseZoom:", "Check to enable using mouse wheel to zoom the camera");
					EditorGUILayout.LabelField(cont, GUILayout.Width(width));
					instance.enableMouseZoom=EditorGUILayout.Toggle(instance.enableMouseZoom);
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
					cont=new GUIContent("MousePanningZoneWidth:", "The clearing from the edge of the screen where the mouse panning will start");
					EditorGUILayout.LabelField(cont, GUILayout.Width(width));
					instance.mousePanningZoneWidth=EditorGUILayout.IntField(instance.mousePanningZoneWidth);
				EditorGUILayout.EndHorizontal();
				
			#endif
			
			EditorGUILayout.Space();
			
			width=116;
			
			EditorGUILayout.BeginHorizontal();
				cont=new GUIContent("X-Axis Limit:", "The min/max X-axis position limit of the camera pivot");
				EditorGUILayout.LabelField(cont, GUILayout.Width(width));
				instance.minPosX=EditorGUILayout.FloatField(instance.minPosX);
				instance.maxPosX=EditorGUILayout.FloatField(instance.maxPosX);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
				cont=new GUIContent("Z-Axis Limit:", "The min/max Z-axis position limit of the camera pivot");
				EditorGUILayout.LabelField(cont, GUILayout.Width(width));
				instance.minPosZ=EditorGUILayout.FloatField(instance.minPosZ);
				instance.maxPosZ=EditorGUILayout.FloatField(instance.maxPosZ);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
				cont=new GUIContent("Zoom Limit:", "The limit of the camera zoom. This is effectively the local Z-axis position limit of the camera transform as a child of the camera pivot");
				EditorGUILayout.LabelField(cont, GUILayout.Width(width));
				instance.minZoomDistance=EditorGUILayout.FloatField(instance.minZoomDistance);
				instance.maxZoomDistance=EditorGUILayout.FloatField(instance.maxZoomDistance);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
				cont=new GUIContent("Elevation Limit:", "The limit of the elevation of the camera pivot, effectively the X-axis rotation. Recommend to keep the value between 10 to 89");
				EditorGUILayout.LabelField(cont, GUILayout.Width(width));
				instance.minRotateAngle=EditorGUILayout.FloatField(instance.minRotateAngle);
				instance.maxRotateAngle=EditorGUILayout.FloatField(instance.maxRotateAngle);
			EditorGUILayout.EndHorizontal();
			
			
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
			showDefaultFlag=EditorGUILayout.Foldout(showDefaultFlag, "Show default editor");
			EditorGUILayout.EndHorizontal();
			if(showDefaultFlag) DrawDefaultInspector();
			
			
			if(GUI.changed) EditorUtility.SetDirty(instance);
			
		}

	}
	
}
