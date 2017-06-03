using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	[CustomEditor(typeof(FPSControl))]
	public class FPSControlEditor : Editor {

		static private FPSControl instance;
		
		private static bool showWeaponList=true;
		private static bool showDefaultFlag=false;
		private static bool showPivotTransform=false;
		
		private static List<FPSWeapon> weaponList=new List<FPSWeapon>();
		
		private GUIContent cont;
		private GUIContent[] contL;
		
		private string[] recoilModeLabel=new string[3];
		private string[] recoilModeTooltip=new string[3];
		
		void Awake(){
			instance = (FPSControl)target;
			
			GetWeapon();
			
			//EditorUtility.SetDirty(instance);
			
			recoilModeLabel=new string[3];
			recoilModeTooltip=new string[3];
			
			recoilModeLabel[0]="None";
			recoilModeLabel[1]="Mode1 (simulated spread)";
			recoilModeLabel[2]="Mode2 (actual recoil)";
			
			recoilModeTooltip[0]="No recoil";
			recoilModeTooltip[1]="The aims remains in place but the bullet is stray from the aim direction. The bullet spreading will auto-correct back to the aim.";
			recoilModeTooltip[2]="The aims recoil and require user input to manually correct the aim";
		}
		
		
		private static void GetWeapon(){
			EditorDBManager.Init();
			
			weaponList=EditorDBManager.GetFPSWeaponList();
			
			if(Application.isPlaying) return;
			
			List<int> weaponIDList=EditorDBManager.GetFPSWeaponIDList();
			for(int i=0; i<instance.unavailableIDList.Count; i++){
				if(!weaponIDList.Contains(instance.unavailableIDList[i])){
					instance.unavailableIDList.RemoveAt(i);	i-=1;
				}
			}
		}
		
		
		
		
		
		public override void OnInspectorGUI(){
			GUI.changed = false;
			
			EditorGUILayout.Space();
			cont=new GUIContent("Use Tower Weapon:", "Check to have use the assigned weapon of the selected tower. When this is enabled, player cannot switch weapon.");
			instance.useTowerWeapon=EditorGUILayout.Toggle(cont, instance.useTowerWeapon);
			
			
			cont=new GUIContent("Aim Sensitivity:", "Mouse sensitivity when aiming in fps mode");
			instance.aimSensitivity=EditorGUILayout.FloatField(cont, instance.aimSensitivity);
			
			cont=new GUIContent("Recoil Mode:", "The way shoot recoil works");
			contL=new GUIContent[recoilModeLabel.Length];
			for(int i=0; i<contL.Length; i++) contL[i]=new GUIContent(recoilModeLabel[i], recoilModeTooltip[i]);
			instance.recoilMode = EditorGUILayout.Popup(cont, instance.recoilMode, contL);
			
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
			showPivotTransform=EditorGUILayout.Foldout(showPivotTransform, "Show Pivot Transforms & Camera");
			EditorGUILayout.EndHorizontal();
			if(showPivotTransform){
				cont=new GUIContent("Weapon Pivot:", "The pivot transform of the weapon object.");
				instance.weaponPivot=(Transform)EditorGUILayout.ObjectField(cont, instance.weaponPivot, typeof(Transform), true);
				
				cont=new GUIContent("Camera Pivot:", "The pivot transform of the camera");
				instance.cameraPivot=(Transform)EditorGUILayout.ObjectField(cont, instance.cameraPivot, typeof(Transform), true);
				
				cont=new GUIContent("Camera Transform:", "The transform which contains the fps camera component");
				instance.camT=(Transform)EditorGUILayout.ObjectField(cont, instance.camT, typeof(Transform), true);
			}
			
			
			EditorGUILayout.Space();
			
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
			showWeaponList=EditorGUILayout.Foldout(showWeaponList, "Show Weapon List");
			EditorGUILayout.EndHorizontal();
			if(showWeaponList){
				
				EditorGUILayout.BeginHorizontal();
				if(GUILayout.Button("EnableAll") && !Application.isPlaying){
					instance.unavailableIDList=new List<int>();
				}
				if(GUILayout.Button("DisableAll") && !Application.isPlaying){
					instance.unavailableIDList=new List<int>();
					for(int i=0; i<weaponList.Count; i++) instance.unavailableIDList.Add(weaponList[i].prefabID);
				}
				EditorGUILayout.EndHorizontal ();
				
				//scrollPosition = GUILayout.BeginScrollView (scrollPosition);
				
				for(int i=0; i<weaponList.Count; i++){
					FPSWeapon weapon=weaponList[i];
					
					GUILayout.BeginHorizontal();
						
						GUILayout.Box("", GUILayout.Width(40),  GUILayout.Height(40));
						Rect rect=GUILayoutUtility.GetLastRect();
						EditorUtilities.DrawSprite(rect, weapon.icon, false);
						
						GUILayout.BeginVertical();
							EditorGUILayout.Space();
							GUILayout.Label(weapon.name, GUILayout.ExpandWidth(false));
							
							bool flag=!instance.unavailableIDList.Contains(weapon.prefabID) ? true : false;
							if(Application.isPlaying) flag=!flag;	//switch it around in runtime
							flag=EditorGUILayout.Toggle(new GUIContent(" - enabled: ", "check to enable the weapon in this level"), flag);
							
							if(!Application.isPlaying){
								if(flag) instance.unavailableIDList.Remove(weapon.prefabID);
								else{
									if(!instance.unavailableIDList.Contains(weapon.prefabID)) 
										instance.unavailableIDList.Add(weapon.prefabID);
								}
							}
					
						GUILayout.EndVertical();
					
					GUILayout.EndHorizontal();
				}

			}
			
			
			EditorGUILayout.Space();
			
			
			if(GUILayout.Button("Open FPSWeaponEditor")){
				//FPSWeaponEditorWindow.Init();
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