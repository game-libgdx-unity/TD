using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	[CustomEditor(typeof(BuildManager))]
	public class BuildManagerEditor : Editor {

		private static BuildManager instance;
		
		private static List<UnitTower> towerList=new List<UnitTower>();
		private static bool showTowerList=true;
		
		private static string[] cursorIndModeLabel=new string[0];
		private static string[] cursorIndModeTooltip=new string[0];
		
		void Awake(){
			instance = (BuildManager)target;
			
			GetTower();
			
			int enumLength = Enum.GetValues(typeof(BuildManager._CursorIndicatorMode)).Length;
			cursorIndModeLabel=new string[enumLength];
			cursorIndModeTooltip=new string[enumLength];
			for(int i=0; i<enumLength; i++){
				cursorIndModeLabel[i]=((BuildManager._CursorIndicatorMode)i).ToString();
				if((BuildManager._CursorIndicatorMode)i==BuildManager._CursorIndicatorMode.All) 
					cursorIndModeTooltip[i]="Always show the tile currently being hovered over by the cursor";
				if((BuildManager._CursorIndicatorMode)i==BuildManager._CursorIndicatorMode.ValidOnly) 
					cursorIndModeTooltip[i]="Only show the tile currently being hovered over by the cursor if it's available to be built on";
				if((BuildManager._CursorIndicatorMode)i==BuildManager._CursorIndicatorMode.None) 
					cursorIndModeTooltip[i]="Never show the tile currently being hovered over by the cursor";
			}
			
			EditorUtility.SetDirty(instance);
		}
		
		
		private static void GetTower(){
			EditorDBManager.Init();
			
			towerList=EditorDBManager.GetTowerList();
			
			if(Application.isPlaying) return;
			//instance.fullTowerList=towerList;
			
			List<int> towerIDList=EditorDBManager.GetTowerIDList();
			for(int i=0; i<instance.unavailableTowerIDList.Count; i++){
				if(!towerIDList.Contains(instance.unavailableTowerIDList[i])){
					instance.unavailableTowerIDList.RemoveAt(i);	i-=1;
				}
			}
		}
		

		
		//private Vector2 scrollPosition;
		private static bool showDefaultFlag=false;
		
		private GUIContent cont;
		private GUIContent[] contList;
		
		private static bool showPlatforms=false;
		
		public override void OnInspectorGUI(){
			
			GUI.changed = false;
			
			EditorGUILayout.Space();
			
				cont=new GUIContent("Grid Size:", "The grid size of the grid on the platform");
				instance.gridSize=EditorGUILayout.FloatField(cont, instance.gridSize);
				
				cont=new GUIContent("AutoSearchForPlatform:", "Check to let the BuildManager automatically serach of all the build platform in game\nThis will override the BuildPlatform list");
				instance.autoSearchForPlatform=EditorGUILayout.Toggle(cont, instance.autoSearchForPlatform);
			
				cont=new GUIContent("AutoAdjustTextureToGrid:", "Check to let the BuildManager reformat the texture tiling of the platform to fix the gridsize");
				instance.AutoAdjustTextureToGrid=EditorGUILayout.Toggle(cont, instance.AutoAdjustTextureToGrid);
				
			EditorGUILayout.Space();
			
				if(GUILayout.Button("Enable All Towers On All Platforms") && !Application.isPlaying){
					EnableAllToweronAllPlatform();
				}
			
				cont=new GUIContent("Build Platforms", "Build Platform in this level\nOnly applicable when AutoSearchForPlatform is unchecked");
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
				showPlatforms = EditorGUILayout.Foldout(showPlatforms, cont);
				EditorGUILayout.EndHorizontal();
			
				if(showPlatforms){
					cont=new GUIContent("Build Platforms:", "The grid size of the grid on the platform");
					float listSize=instance.buildPlatforms.Count;
					listSize=EditorGUILayout.FloatField("    Size:", listSize);
					
					//if(!EditorGUIUtility.editingTextField && listSize!=instance.buildPlatforms.Count){
					if(listSize!=instance.buildPlatforms.Count){
						while(instance.buildPlatforms.Count<listSize) instance.buildPlatforms.Add(null);
						while(instance.buildPlatforms.Count>listSize) instance.buildPlatforms.RemoveAt(instance.buildPlatforms.Count-1);
					}
					
					for(int i=0; i<instance.buildPlatforms.Count; i++){
						instance.buildPlatforms[i]=(PlatformTD)EditorGUILayout.ObjectField("    Element "+i, instance.buildPlatforms[i], typeof(PlatformTD), true);
					}
				}
			
				
				
				
			
				
			
			
			EditorGUILayout.Space();
			
				
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
			showTowerList=EditorGUILayout.Foldout(showTowerList, "Show Tower List");
			EditorGUILayout.EndHorizontal();
			if(showTowerList){
				
				EditorGUILayout.BeginHorizontal();
				if(GUILayout.Button("EnableAll") && !Application.isPlaying){
					instance.unavailableTowerIDList=new List<int>();
				}
				if(GUILayout.Button("DisableAll") && !Application.isPlaying){
					instance.unavailableTowerIDList=new List<int>();
					for(int i=0; i<towerList.Count; i++) instance.unavailableTowerIDList.Add(towerList[i].prefabID);
				}
				EditorGUILayout.EndHorizontal ();
				
				//scrollPosition = GUILayout.BeginScrollView (scrollPosition);
				
				for(int i=0; i<towerList.Count; i++){
					UnitTower tower=towerList[i];
					
					if(tower.disableInBuildManager) continue;
					
					GUILayout.BeginHorizontal();
						
						GUILayout.Box("", GUILayout.Width(40),  GUILayout.Height(40));
						Rect rect=GUILayoutUtility.GetLastRect();
						EditorUtilities.DrawSprite(rect, tower.iconSprite, false);
						
						GUILayout.BeginVertical();
							EditorGUILayout.Space();
							GUILayout.Label(tower.unitName, GUILayout.ExpandWidth(false));
					
							bool flag=!instance.unavailableTowerIDList.Contains(tower.prefabID) ? true : false;
							if(Application.isPlaying) flag=!flag;	//switch it around in runtime
							flag=EditorGUILayout.Toggle(new GUIContent(" - enabled: ", "check to enable the tower in this level"), flag);
							
							if(!Application.isPlaying){
								if(flag) instance.unavailableTowerIDList.Remove(tower.prefabID);
								else{
									if(!instance.unavailableTowerIDList.Contains(tower.prefabID)) 
										instance.unavailableTowerIDList.Add(tower.prefabID);
								}
							}
							
						GUILayout.EndVertical();
					
					GUILayout.EndHorizontal();
				}

				//GUILayout.EndScrollView ();
			}
			
			EditorGUILayout.Space();
			
				int cursorMode=(int)instance.cursorIndicatorMode;
				cont=new GUIContent("Tile Cursor Mode:", "The way to indicate a tile on a grid when it's currently being hovered on by the cursor");
				contList=new GUIContent[cursorIndModeLabel.Length];
				for(int i=0; i<contList.Length; i++) contList[i]=new GUIContent(cursorIndModeLabel[i], cursorIndModeTooltip[i]);
				cursorMode = EditorGUILayout.Popup(cont, cursorMode, contList);
				instance.cursorIndicatorMode=(BuildManager._CursorIndicatorMode)cursorMode;
			
			EditorGUILayout.Space();
			
			
			
			if(GUILayout.Button("Open TowerEditor")){
				UnitTowerEditorWindow.Init();
			}
			EditorGUILayout.Space();
			
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
			showDefaultFlag=EditorGUILayout.Foldout(showDefaultFlag, "Show default editor");
			EditorGUILayout.EndHorizontal();
			if(showDefaultFlag) DrawDefaultInspector();
			
			
			if(GUI.changed) EditorUtility.SetDirty(instance);
			
		}
		
		
		
		void EnableAllToweronAllPlatform(){
			PlatformTD[] platList = FindObjectsOfType(typeof(PlatformTD)) as PlatformTD[];
			for(int i=0; i<platList.Length; i++) platList[i].availableTowerIDList=new List<int>();
		}
		
	}

	
}