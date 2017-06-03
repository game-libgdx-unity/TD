//using UnityEngine;
//using UnityEditor;

//using System;

//using System.Collections;

//using UnitedSolution;

//namespace UnitedSolution {

//	[CustomEditor(typeof(UnitCreepAnimation))]
//	public class UnitCreepAnimationEditor : Editor {
		
//		private static string[] typeLabel=new string[4];
//		private static string[] typeTooltip=new string[4];
//		private static bool init=false;
		
		
//		private static void Init(){
//			init=true;
			
//			int enumLength = Enum.GetValues(typeof(UnitCreepAnimation._AniType)).Length;
//			typeLabel=new string[enumLength];
//			typeTooltip=new string[enumLength];
//			for(int i=0; i<enumLength; i++){
//				typeLabel[i]=((UnitCreepAnimation._AniType)i).ToString();
//				if((UnitCreepAnimation._AniType)i==UnitCreepAnimation._AniType.Mecanim) typeTooltip[i]="";
//				if((UnitCreepAnimation._AniType)i==UnitCreepAnimation._AniType.Legacy) typeTooltip[i]="";
//			}
//		}
		
//		void Awake(){
//			instance = (UnitCreepAnimation)target;
			
//			if(!init) Init();
			
//			EditorUtility.SetDirty(instance);
//		}
		
		
//		private static bool showDefaultFlag=false;
		
//		private GUIContent cont;
//		private GUIContent[] contList;
		
//		public override void OnInspectorGUI(){
//			GUI.changed = false;
			
//			EditorGUILayout.Space();
			
//			int type=(int)instance.type;
//			cont=new GUIContent("Type:", "Type of the animation to use");
//			contList=new GUIContent[typeLabel.Length];
//			for(int i=0; i<contList.Length; i++) contList[i]=new GUIContent(typeLabel[i], typeTooltip[i]);
//			type = EditorGUILayout.Popup(cont, type, contList);
//			instance.type=(UnitCreepAnimation._AniType)type;
			
//			EditorGUILayout.Space();
			
//			if(instance.type==UnitCreepAnimation._AniType.Legacy){
//				cont=new GUIContent("AniRootObject:", "The Animation component that runs the animation on the unit");
//				instance.aniInstance=(Animation)EditorGUILayout.ObjectField(cont, instance.aniInstance, typeof(Animation), true);
				
//				cont=new GUIContent("MoveSpeedMultiplier:", "The multiplier used to match the move animation speed to the unit's move speed");
//				instance.moveSpeedMultiplier=EditorGUILayout.FloatField(cont, instance.moveSpeedMultiplier);
				
//				EditorGUILayout.Space();
//			}
			
//			if(instance.type==UnitCreepAnimation._AniType.Mecanim){
//				cont=new GUIContent("Animator:", "The Animator component to be controlled by the script. This is optional if the AnimatorRootObject has been assigned in CreepEditor");
//				instance.anim=(Animator)EditorGUILayout.ObjectField(cont, instance.anim, typeof(Animator), true);
				
//				EditorGUILayout.Space();
//			}
			
//			cont=new GUIContent("Clip Spawn:", "The animation clip to be played when the unit is moving");
//			instance.clipSpawn=(AnimationClip)EditorGUILayout.ObjectField(cont, instance.clipSpawn, typeof(AnimationClip), true);
			
//			cont=new GUIContent("Clip Move:", "The animation clip to be played when the unit is moving");
//			instance.clipMove=(AnimationClip)EditorGUILayout.ObjectField(cont, instance.clipMove, typeof(AnimationClip), true);
			
//			cont=new GUIContent("Clip Dead:", "The animation clip to be played when the unit is destroyed");
//			instance.clipDead=(AnimationClip)EditorGUILayout.ObjectField(cont, instance.clipDead, typeof(AnimationClip), true);
			
//			cont=new GUIContent("Clip Destination:", "The animation clip to be played when the unit reach its destination");
//			instance.clipDestination=(AnimationClip)EditorGUILayout.ObjectField(cont, instance.clipDestination, typeof(AnimationClip), true);
			
			
//			EditorGUILayout.Space();
			
			
//			EditorGUILayout.BeginHorizontal();
//			EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
//			showDefaultFlag=EditorGUILayout.Foldout(showDefaultFlag, "Show default editor");
//			EditorGUILayout.EndHorizontal();
//			if(showDefaultFlag) DrawDefaultInspector();
			
			
//			if(GUI.changed) EditorUtility.SetDirty(instance);
			
//		}
//	}

//}