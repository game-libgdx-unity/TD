using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	[CustomEditor(typeof(UI))]
	public class UIEditor : Editor {

		private static UI instance;
		
		
		private string[] buildModeLabel;
		private string[] buildModeTooltip;
		
		private GUIContent cont;
		private GUIContent[] contList;
		
		
		private bool showDefaultFlag=false;
		
		
		void Awake(){
			instance = (UI)target;
			
			int enumLength = Enum.GetValues(typeof(UI._BuildMode)).Length;
			buildModeLabel=new string[enumLength];
			buildModeTooltip=new string[enumLength];
			for(int i=0; i<enumLength; i++){
				buildModeLabel[i]=((UI._BuildMode)i).ToString();
				if((UI._BuildMode)i==UI._BuildMode.PointNBuild) 
					buildModeTooltip[i]="A build mode where player first select a build point and then choose which tower to build";
				if((UI._BuildMode)i==UI._BuildMode.DragNDrop) 
					buildModeTooltip[i]="A build mode where all the tower buttons are always on show. Player simply click on the button of the tower and bring the tower to the spot which it needs to be built";
			}
			
			EditorUtility.SetDirty(instance);
		}
		
		
		public override void OnInspectorGUI(){
			
			cont=new GUIContent("ScaleFactor:", "Scale factor of the UI, match this with the value specified in the CanvasScalar so that the UI element shows up in the correct position on screen");
			instance.scaleFactor=EditorGUILayout.FloatField(cont, instance.scaleFactor);
			
			int buildMode=(int)instance.buildMode;
			cont=new GUIContent("Build Mode:", "The build mode to be used. Determines the method the player uses to build a tower");
			contList=new GUIContent[buildModeLabel.Length];
			for(int i=0; i<contList.Length; i++) contList[i]=new GUIContent(buildModeLabel[i], buildModeTooltip[i]);
			buildMode = EditorGUILayout.Popup(cont, buildMode, contList);
			instance.buildMode=(UI._BuildMode)buildMode;
			
			cont=new GUIContent("Fast-forwardTimeScale:", "The fast forward time mutiplier when the fast-forward button is toggled");
			instance.fastForwardTimeScale=EditorGUILayout.FloatField(cont, instance.fastForwardTimeScale);
			
			cont=new GUIContent("DisableTextOverlay:", "Check to hide the text overlay for unit damage");
			instance.disableTextOverlay=EditorGUILayout.Toggle(cont, instance.disableTextOverlay);
			
			cont=new GUIContent("PauseGameInPerkMenu:", "Check to enable pausing the game when the perk menu is opened");
			instance.pauseGameInPerkMenu=EditorGUILayout.Toggle(cont, instance.pauseGameInPerkMenu);
			
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
			showDefaultFlag=EditorGUILayout.Foldout(showDefaultFlag, "Show default editor");
			EditorGUILayout.EndHorizontal();
			if(showDefaultFlag) DrawDefaultInspector();
			
			if(GUI.changed) EditorUtility.SetDirty(instance);
			
		}
		
		
	}

	
}