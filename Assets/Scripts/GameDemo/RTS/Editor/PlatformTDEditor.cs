using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	[CustomEditor(typeof(PlatformTD))]
	public class PlatformTDEditor : Editor {

		static private PlatformTD instance;
		
		private static bool showTowerList=true;
		private static bool showDefaultFlag=false;
		
		private static List<UnitTower> towerList=new List<UnitTower>();
		
		private GUIContent cont;
		
		void Awake(){
			instance = (PlatformTD)target;
			
			GetTower();
		}
		
		
		private static void GetTower(){
			EditorDBManager.Init();
			
			towerList=EditorDBManager.GetTowerList();
			
			if(Application.isPlaying) return;
			
			List<int> towerIDList=EditorDBManager.GetTowerIDList();
			for(int i=0; i<instance.availableTowerIDList.Count; i++){
				if(!towerIDList.Contains(instance.availableTowerIDList[i])){
					instance.availableTowerIDList.RemoveAt(i);	i-=1;
				}
			}
		}
		
		
		
		
		
		public override void OnInspectorGUI(){
			GUI.changed = false;
			
			EditorGUILayout.Space();
			cont=new GUIContent("Gizmo(Show Nodes):", "Check to enable gizmo to show path-finding nodes generated on the grid\nThe gizmo are only visible in runtime and are for debug purpose only");
			instance.GizmoShowNodes=EditorGUILayout.Toggle(cont, instance.GizmoShowNodes);
			//platform.GizmoShowNodes=GUILayout.Toggle(platform.GizmoShowNodes, "Gizmo - Show Nodes");
			
			showTowerList=EditorGUILayout.Foldout(showTowerList, "Show Valid Tower List");
			if(showTowerList){
				
				EditorGUILayout.BeginHorizontal();
				if(GUILayout.Button("EnableAll") && !Application.isPlaying){
					instance.availableTowerIDList=new List<int>();
				}
				if(GUILayout.Button("DisableAll") && !Application.isPlaying){
					instance.availableTowerIDList=new List<int>();
					for(int i=0; i<towerList.Count; i++) instance.availableTowerIDList.Add(towerList[i].prefabID);
				}
				EditorGUILayout.EndHorizontal ();
				
				GUIStyle style=new GUIStyle("Label");
				style.wordWrap=true;
				EditorGUILayout.LabelField("Please note that the unchecked towers are those invalid on this platform", style);
				
				for(int i=0; i<towerList.Count; i++){
					UnitTower tower=towerList[i];
					
					if(tower.disableInBuildManager) continue;
					
					
					GUILayout.BeginHorizontal();
						
						GUILayout.Box("", GUILayout.Width(40),  GUILayout.Height(40));
						Rect rect=GUILayoutUtility.GetLastRect();
						EditorUtilities.DrawSprite(rect, tower.iconSprite, false);
						
						GUILayout.BeginVertical();
							EditorGUILayout.Space();
							GUILayout.Label(tower.name, GUILayout.ExpandWidth(false));
							bool flag=!instance.availableTowerIDList.Contains(tower.prefabID) ? true : false;
							if(Application.isPlaying) flag=!flag;	//switch it around in runtime
							flag=EditorGUILayout.Toggle(" - enabled:", flag);
							
							if(!Application.isPlaying){
								if(flag) instance.availableTowerIDList.Remove(tower.prefabID);
								else{
									if(!instance.availableTowerIDList.Contains(tower.prefabID)) 
										instance.availableTowerIDList.Add(tower.prefabID);
								}
							}
							
						GUILayout.EndVertical();
					
					GUILayout.EndHorizontal();
				}
			}	
			
			EditorGUILayout.Space();
			
			
			
			showDefaultFlag=EditorGUILayout.Foldout(showDefaultFlag, "Show default editor");
			if(showDefaultFlag) DrawDefaultInspector();
			
			if(GUI.changed) EditorUtility.SetDirty(instance);
			

		}
		
	}

}