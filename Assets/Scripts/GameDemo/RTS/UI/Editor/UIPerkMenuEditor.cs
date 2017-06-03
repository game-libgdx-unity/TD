using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	[CustomEditor(typeof(UIPerkMenu))]
	public class UIPerkMenuEditor : Editor {

		private static UIPerkMenu instance;
		
		private bool showDefaultFlag=false;
		
		private GUIContent cont;
		
		void Awake(){
			instance = (UIPerkMenu)target;
			
			EditorDBManager.Init();
			
			EditorUtility.SetDirty(instance);
		}
		
		
		public override void OnInspectorGUI(){
			
			cont=new GUIContent("Assign Item Manually:", "Check to manually assign the item and their associate perk");
			instance.assignItemManually=EditorGUILayout.Toggle(cont, instance.assignItemManually);
			
			if(instance.assignItemManually){
				GUILayout.BeginHorizontal();
				//EditorGUILayout.Space();
				if(GUILayout.Button("Add Item", GUILayout.MaxWidth(120))){
					//AddItem();
					instance.itemList.Add(new UIPerkMenu.PerkItem());
				}
				if(GUILayout.Button("Reduce Item", GUILayout.MaxWidth(120))){
					//RemoveItem();
					instance.itemList.RemoveAt(instance.itemList.Count-1);
				}
				GUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
				
				string[] perkNameList=EditorDBManager.GetPerkNameList();
				List<Perk> perkList=EditorDBManager.GetPerkList();
				int[] intList=new int[perkNameList.Length];
				for(int i=0; i<perkNameList.Length; i++){
					if(i==0) intList[i]=-1;
					else intList[i]=perkList[i-1].ID;
				}
				
				for(int i=0; i<instance.itemList.Count; i++){
					GUILayout.BeginHorizontal();
					
					GUILayout.Label(" - Element "+(i+1), GUILayout.Width(75));
					
					instance.itemList[i].button.rootObj=(GameObject)EditorGUILayout.ObjectField(instance.itemList[i].button.rootObj, typeof(GameObject), true);
					
					//~ if(GUILayout.Button("+", GUILayout.MaxWidth(20))){
						//~ InsertWaypoints(i);
					//~ }
					//~ if(GUILayout.Button("-", GUILayout.MaxWidth(20))){
						//~ i-=RemoveWaypoints(i);
					//~ }
					GUILayout.EndHorizontal();
					
					
					GUILayout.BeginHorizontal();
					GUILayout.Label("    Link   ", GUILayout.Width(75));
					instance.itemList[i].linkObj=(GameObject)EditorGUILayout.ObjectField(instance.itemList[i].linkObj, typeof(GameObject), true);
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					GUILayout.Label("    Label:   ", GUILayout.Width(75));
					instance.itemList[i].perkID=EditorGUILayout.IntPopup(instance.itemList[i].perkID, perkNameList, intList);
					GUILayout.EndHorizontal();
					
					EditorGUILayout.Space();
				}
			}
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
			showDefaultFlag=EditorGUILayout.Foldout(showDefaultFlag, "Show default editor");
			EditorGUILayout.EndHorizontal();
			if(showDefaultFlag) DrawDefaultInspector();
			
			if(GUI.changed) EditorUtility.SetDirty(instance);
			
		}
		
		
	}

	
}