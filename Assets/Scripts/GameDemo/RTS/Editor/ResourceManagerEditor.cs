using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	[CustomEditor(typeof(ResourceManager))]
	public class ResourceManagerEditor : Editor {

		
		private static ResourceManager instance;
		
		public static List<Rsc> rscList=new  List<Rsc>();
		
		void Awake(){
			instance = (ResourceManager)target;
			
			rscList=ResourceDB.Load();
			
			//VerifyingList();
			instance.Init();
			EditorUtility.SetDirty(instance);
		}
		
		bool CheckMatch(){
			if(rscList.Count!=instance.rscList.Count) return true;
			for(int i=0; i<rscList.Count; i++){
				if(rscList[i].IsMatch(instance.rscList[i])){
					return true;
				}
			}
			return true;
		}
		
		
		
		GUIContent cont;
		private static bool showDefaultFlag=false;
		
		public override void OnInspectorGUI(){
			
			GUI.changed = false;
			
			EditorGUILayout.Space();
			
			/*
			GUILayout.BeginHorizontal();
				cont=new GUIContent("UseValueFromPreviousLevel:", "check to enable value from previous level, current setting will be overriden"); 
				EditorGUILayout.LabelField(cont, GUILayout.Width(170));
				instance.useValueFromPrevLevel=EditorGUILayout.Toggle(instance.useValueFromPrevLevel);
			GUILayout.EndHorizontal();
			
			if(instance.useValueFromPrevLevel){
				GUILayout.BeginHorizontal();
					cont=new GUIContent("AccumulateValue:", "check to add the value from previous level to the starting value of the current one"); 
					EditorGUILayout.LabelField(cont, GUILayout.Width(170));
					instance.accumulateValueFromPrevLevel=EditorGUILayout.Toggle(instance.accumulateValueFromPrevLevel);
				GUILayout.EndHorizontal();
			}
			*/
			
			
			if(!CheckMatch()) instance.Init();
			
			cont=new GUIContent("CarryFromLastScene:", "Check to carry the resource value from last scene. If this is the first scene, the specified value is used");
			instance.carryFromLastScene=EditorGUILayout.Toggle(cont, instance.carryFromLastScene);
			
			cont=new GUIContent("Generate Overtime:", "Check to have the resource generate overtime. Value specified is value per second");
			instance.enableRscGen=EditorGUILayout.Toggle(cont, instance.enableRscGen);
			
			if(instance.enableRscGen){
				while(instance.rscGenRateList.Count<rscList.Count) instance.rscGenRateList.Add(0);
				while(instance.rscGenRateList.Count>rscList.Count) instance.rscGenRateList.RemoveAt(instance.rscGenRateList.Count-1);
			}
			
			EditorGUILayout.Space();
			
			for(int i=0; i<instance.rscList.Count; i++){
				if(instance.rscList[i]!=null){
					GUILayout.BeginHorizontal();
					GUILayout.Label(instance.rscList[i].icon.texture, GUILayout.Width(40),  GUILayout.Height(40));
					
						GUILayout.BeginVertical();
							//~ GUILayout.Label(instance.rscList[i].name, GUILayout.Width(70));
					
								GUILayout.BeginHorizontal();
								EditorGUILayout.LabelField("Start Value: ", GUILayout.Width(70));
								instance.rscList[i].value=EditorGUILayout.IntField(instance.rscList[i].value);
								GUILayout.EndHorizontal();
					
								if(instance.enableRscGen){
									GUILayout.BeginHorizontal();
									EditorGUILayout.LabelField("Regen Rate: ", GUILayout.Width(70));
									instance.rscGenRateList[i]=EditorGUILayout.FloatField(instance.rscGenRateList[i]);
									GUILayout.EndHorizontal();
								}
								else EditorGUILayout.LabelField("Regen Rate: -");
							
					
						GUILayout.EndVertical();
					
					GUILayout.EndHorizontal();
				}
				EditorGUILayout.Space();
			}
			
			
			
			showDefaultFlag=EditorGUILayout.Foldout(showDefaultFlag, "Show default editor");
			if(showDefaultFlag) DrawDefaultInspector();
			
			if(GUI.changed) EditorUtility.SetDirty(instance);
			
		}
		
		
	}

}