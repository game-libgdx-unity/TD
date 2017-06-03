using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	[CustomEditor(typeof(PerkManager))]
	public class PerkManagerEditor : Editor {

		private static PerkManager instance;
		
		private bool showDefaultFlag=false;
		private bool showPerkList=true;
		
		private static List<Perk> perkList=new List<Perk>();
		
		private GUIContent cont;
		
		void Awake(){
			instance = (PerkManager)target;
			
			GetPerk();
			
			EditorUtility.SetDirty(instance);
		}
		
		private static void GetPerk(){
			EditorDBManager.Init();
			
			perkList=EditorDBManager.GetPerkList();
			
			if(Application.isPlaying) return;
			
			List<int> perkIDList=EditorDBManager.GetPerkIDList();
			for(int i=0; i<instance.unavailableIDList.Count; i++){
				if(!perkIDList.Contains(instance.unavailableIDList[i])){
					instance.unavailableIDList.RemoveAt(i);	i-=1;
				}
			}
		}
		
		
		
		public override void OnInspectorGUI(){
			GUI.changed = false;
			
			EditorGUILayout.Space();
			
			cont=new GUIContent("Persistant Progress", "Check to use persistantProgress\nThe progress done in this level will be carried to next\nEnable this will cause all the level to use the perk enabled in this instance, perk enabled/disable in subsequent PerkManager instance will be ignored");
			instance.persistantProgress=EditorGUILayout.Toggle(cont, instance.persistantProgress);
			
			EditorGUILayout.Space();
			
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
				showPerkList=EditorGUILayout.Foldout(showPerkList, "Show Perk List");
				EditorGUILayout.EndHorizontal();
				if(showPerkList){
					
					EditorGUILayout.BeginHorizontal();
					if(GUILayout.Button("EnableAll") && !Application.isPlaying){
						instance.unavailableIDList=new List<int>();
					}
					if(GUILayout.Button("DisableAll") && !Application.isPlaying){
						instance.purchasedIDList=new List<int>();
						
						instance.unavailableIDList=new List<int>();
						for(int i=0; i<perkList.Count; i++) instance.unavailableIDList.Add(perkList[i].ID);
					}
					EditorGUILayout.EndHorizontal ();
					
					
					for(int i=0; i<perkList.Count; i++){
						Perk perk=perkList[i];
						
						
						GUILayout.BeginHorizontal();
							
							GUILayout.Box("", GUILayout.Width(40),  GUILayout.Height(40));
							Rect rect=GUILayoutUtility.GetLastRect();
							EditorUtilities.DrawSprite(rect, perk.icon, false);
							
							GUILayout.BeginVertical();
								EditorGUILayout.Space();
								GUILayout.Label(perk.name, GUILayout.ExpandWidth(false));
						
								GUILayout.BeginHorizontal();
									bool flag=!instance.unavailableIDList.Contains(perk.ID) ? true : false;
									if(Application.isPlaying) flag=!flag;	//switch it around in runtime
									EditorGUILayout.LabelField(new GUIContent(" - enabled: ", "check to enable the perk in this level"), GUILayout.Width(70));
									flag=EditorGUILayout.Toggle(flag);
						
									if(!Application.isPlaying){
										if(flag) instance.unavailableIDList.Remove(perk.ID);
										else{
											if(!instance.unavailableIDList.Contains(perk.ID)){
												instance.unavailableIDList.Add(perk.ID);
												instance.purchasedIDList.Remove(perk.ID);
											}
										}
									}
									
									if(!instance.unavailableIDList.Contains(perk.ID)){
										flag=instance.purchasedIDList.Contains(perk.ID);
										EditorGUILayout.LabelField(new GUIContent("- purchased:", "Check to set the perk as purchased right from the start"), GUILayout.Width(75));
										flag=EditorGUILayout.Toggle(flag);
										if(!flag) instance.purchasedIDList.Remove(perk.ID);
										else if(!instance.purchasedIDList.Contains(perk.ID)) instance.purchasedIDList.Add(perk.ID);
									}
									
								GUILayout.EndHorizontal();
								
							GUILayout.EndVertical();
						
						GUILayout.EndHorizontal();
					}
				
				}
				
			EditorGUILayout.Space();
				
			if(GUILayout.Button("Open PerkEditor")){
				PerkEditorWindow.Init();
			}
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