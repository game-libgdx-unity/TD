using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	[CustomEditor(typeof(PathTD))]
	public class PathTDEditor : Editor {

		private static PathTD instance;
		
		private bool showPath=true;
		private bool showDefaultFlag=false;
		
		private GUIContent cont;
		
		void Awake(){
			instance = (PathTD)target;
			
			EditorUtility.SetDirty(instance);
		}
		
		
		void InsertWaypoints(int ID){
			if(Application.isPlaying) return;
			instance.wpList.Insert(ID, instance.wpList[ID]);
		}
		int RemoveWaypoints(int ID){
			if(Application.isPlaying) return 0;
			instance.wpList.RemoveAt(ID);
			return 1;
		}
		void AddWaypoint(){
			if(Application.isPlaying) return;
			if(instance.wpList.Count==0) instance.wpList.Add(null);
			else instance.wpList.Add(instance.wpList[instance.wpList.Count-1]);
		}
		void RemoveWaypoint(){
			if(Application.isPlaying) return;
			if(instance.wpList.Count==0) return;
			instance.wpList.RemoveAt(instance.wpList.Count-1);
		}
		
		
		public override void OnInspectorGUI(){
			GUI.changed = false;
			
			EditorGUILayout.Space();
			
			showPath=EditorGUILayout.Foldout(showPath, "Show Waypoint List");
			if(showPath){
				GUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if(GUILayout.Button("Add Waypoint", GUILayout.MaxWidth(120))){
					AddWaypoint();
				}
				if(GUILayout.Button("Reduce Waypoint", GUILayout.MaxWidth(120))){
					RemoveWaypoint();
				}
				GUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
				
				for(int i=0; i<instance.wpList.Count; i++){
					GUILayout.BeginHorizontal();
					
					GUILayout.Label("    Element "+(i+1));
					
					instance.wpList[i]=(Transform)EditorGUILayout.ObjectField(instance.wpList[i], typeof(Transform), true);
					
					if(GUILayout.Button("+", GUILayout.MaxWidth(20))){
						InsertWaypoints(i);
					}
					if(GUILayout.Button("-", GUILayout.MaxWidth(20))){
						i-=RemoveWaypoints(i);
					}
					GUILayout.EndHorizontal();
				}
			}
			
			EditorGUILayout.Space();
			
			cont=new GUIContent("Dynamic Offset:", "A random offset range which somewhat randomize the waypoint for each individual creep\nSet to 0 to disable and any value >0 to enable\nNot recommend for any value larger than BuildManager's grid-size\nNot recommend for path with varying height");
			instance.dynamicOffset=EditorGUILayout.FloatField(cont, instance.dynamicOffset);
			
			cont=new GUIContent("Loop:", "Check to enable path-looping. On path that loops, creep will carry on to the looping point and repeat the path until they are destroyed");
			instance.loop=EditorGUILayout.Toggle(cont, instance.loop);
			
			if(instance.loop){
				cont=new GUIContent("Loop Point:", "The ID of the waypoint which will act as the loop start point. Creep will move to this waypoint to start the path again after reaching destination\nStarts from 0.");
				instance.loopPoint=EditorGUILayout.IntField(cont, instance.loopPoint);
			}
			
			cont=new GUIContent("Generate Path Line:", "Check to generate the a line which make the path visible in game during runtime");
			instance.createPathLine=EditorGUILayout.Toggle(cont, instance.createPathLine);
			
			cont=new GUIContent("Show Gizmo:", "Check to enable gizmo to show the active path");
			instance.showGizmo=EditorGUILayout.Toggle(cont, instance.showGizmo);
			
			if(instance.showGizmo){
				cont=new GUIContent("Gizmo Color:", "Color of the gizmo\nSet different path's gizmo color to different color to help you differentiate them");
				instance.gizmoColor=EditorGUILayout.ColorField(cont, instance.gizmoColor);
			}
			

			
			EditorGUILayout.Space();
			
			showDefaultFlag=EditorGUILayout.Foldout(showDefaultFlag, "Show default editor");
			if(showDefaultFlag) DrawDefaultInspector();
			
			if(GUI.changed) EditorUtility.SetDirty(instance);
		}
		
		
	}

}