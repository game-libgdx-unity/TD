using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	[CustomEditor(typeof(GameControl))]
	public class GameControlEditor : Editor {

		private static GameControl instance;

		void Awake(){
			instance = (GameControl)target;
		}
		
		private bool indicatorFlag=false;
		
		private static bool showDefaultFlag=false;
		private GUIContent cont;
		
		
		public override void OnInspectorGUI(){
			
			GUI.changed = false;
			EditorGUILayout.Space();
			
			
			cont=new GUIContent("Level ID:", "Indicate what level this scene is. This is used to determined if any perk should become available");
			instance.levelID=EditorGUILayout.IntField(cont, instance.levelID, GUILayout.ExpandWidth(true));
			
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
			cont=new GUIContent("Player Life (capped):", "The amount of life the player has. Under certain setting player might be able to gain life, check to have the player life capped");
			instance.playerLife=EditorGUILayout.IntField(cont, instance.playerLife, GUILayout.ExpandWidth(true));
			instance.capLife=EditorGUILayout.Toggle(instance.capLife, GUILayout.MaxWidth(20));
			EditorGUILayout.EndHorizontal();
			
			if(instance.capLife){
				cont=new GUIContent("Player Life Max:", "Maximum amount of life the player can have");
				instance.playerLifeCap=EditorGUILayout.IntField(cont, instance.playerLifeCap);
			}
			
			EditorGUILayout.BeginHorizontal();
			cont=new GUIContent("Enable Life Regen:", "Check to have the player life regenerate overtime");
			instance.enableLifeGen=EditorGUILayout.Toggle(cont, instance.enableLifeGen);
			
			if(instance.enableLifeGen){
				cont=new GUIContent("  Rate:", "The rate at which the player life regenerate (per second)");
				EditorGUILayout.LabelField(cont, GUILayout.MaxWidth(45));
				instance.lifeRegenRate=EditorGUILayout.IntField(instance.lifeRegenRate);
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Space();
			
			cont=new GUIContent("Tower Refund Ratio:", "The ratio of the total tower value that the player will receive when they sell a tower. The value takes into account the cost to build the tower as well as the resources spent to upgrade it.");
			instance.sellTowerRefundRatio=EditorGUILayout.FloatField(cont, instance.sellTowerRefundRatio);
			
			cont=new GUIContent("Reset TargetOn Each Shot:", "Check to have the turret tower's target reset the target after each shot, forcing them to acquire a new target.\nThis would be useful in some case to highlight the target priority mode use by the tower");
			instance.resetTargetAfterShoot=EditorGUILayout.Toggle(cont, instance.resetTargetAfterShoot);
			
			EditorGUILayout.Space();
			
			cont=new GUIContent("MainMenu Name:", "Scene's name of the main menu to be loaded when return to menu on UI is called");
			instance.mainMenu=EditorGUILayout.TextField(cont, instance.mainMenu);
			cont=new GUIContent("NextScene Name:", "Scene's name to be loaded when this level is completed");
			instance.nextScene=EditorGUILayout.TextField(cont, instance.nextScene);
			
			
			cont=new GUIContent("Load AudioManager:", "Check to load and create an AudioManager instance. AudioManager only needs to be loaded once (initially) and will remain through all subsequent scene");
			instance.loadAudioManager=EditorGUILayout.Toggle(cont, instance.loadAudioManager);
			
			
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
			indicatorFlag = EditorGUILayout.Foldout(indicatorFlag, "Range Indicators ");
			EditorGUILayout.EndHorizontal();
			
			if(indicatorFlag){
				cont=new GUIContent("     Range Indicator (Circular):", "The circular range indicator prefab to be instantiated and used in game");
				EditorGUILayout.LabelField(cont);
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("      - ", GUILayout.MaxWidth(40));
				instance.rangeIndicator=(Transform)EditorGUILayout.ObjectField( instance.rangeIndicator, typeof(Transform), false);
				EditorGUILayout.EndHorizontal();
				
				cont=new GUIContent("     Range Indicator (Cone):", "The cone shape range indicator prefab to be instantiated and used in game");
				EditorGUILayout.LabelField(cont);
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("      - ", GUILayout.MaxWidth(40));
				instance.rangeIndicatorCone=(Transform)EditorGUILayout.ObjectField(instance.rangeIndicatorCone, typeof(Transform), false);
				EditorGUILayout.EndHorizontal();
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