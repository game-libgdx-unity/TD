using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	public class SpawnEditorWindow : EditorWindow {
		
		public static SpawnManager instance;
		public static SpawnEditorWindow window;
		
		private static string[] spawnLimitLabel=new string[0];
		private static string[] spawnLimitTooltip=new string[0];
		private static string[] spawnModeLabel=new string[0];
		private static string[] spawnModeTooltip=new string[0];
		
		private static string[] creepNameList=new string[0];
		
		public static List<UnitCreep> creepList=new List<UnitCreep>();
		public static List<Rsc> rscList=new List<Rsc>();
		
		private List<bool> waveFoldList=new List<bool>();
		
		public static void Init(){
			// Get existing open window or if none, make a new one:
			window = (SpawnEditorWindow)EditorWindow.GetWindow(typeof (SpawnEditorWindow),false, "Spawn Editor");
			window.minSize=new Vector2(480, 620);
			
			
			int enumLength = Enum.GetValues(typeof(SpawnManager._SpawnLimit)).Length;
			spawnLimitLabel=new string[enumLength];
			spawnLimitTooltip=new string[enumLength];
			for(int i=0; i<enumLength; i++){
				spawnLimitLabel[i]=((SpawnManager._SpawnLimit)i).ToString();
				if((SpawnManager._SpawnLimit)i==SpawnManager._SpawnLimit.Finite) spawnLimitTooltip[i]="Finite number of waves";
				if((SpawnManager._SpawnLimit)i==SpawnManager._SpawnLimit.Infinite) spawnLimitTooltip[i]="Infinite number of waves (for survival or endless mode)";
			}
			
			
			enumLength = Enum.GetValues(typeof(SpawnManager._SpawnMode)).Length;
			spawnModeLabel=new string[enumLength];
			spawnModeTooltip=new string[enumLength];
			for(int i=0; i<enumLength; i++){
				spawnModeLabel[i]=((SpawnManager._SpawnMode)i).ToString();
				if((SpawnManager._SpawnMode)i==SpawnManager._SpawnMode.Continous) 
					spawnModeTooltip[i]="A new wave is spawn upon every wave duration countdown (with option to skip the timer)";
				if((SpawnManager._SpawnMode)i==SpawnManager._SpawnMode.WaveCleared) 
					spawnModeTooltip[i]="A new wave is spawned when the current wave is cleared (with option to spawn next wave in advance)";
				if((SpawnManager._SpawnMode)i==SpawnManager._SpawnMode.Round) 
					spawnModeTooltip[i]="Each wave is treated like a round. a new wave can only take place when the previous wave is cleared. Each round require initiation from user";
			}
			
			
			creepList=CreepDB.Load();
			rscList=ResourceDB.Load();
			GetSpawnManager();
		}
		
		private static void ResetCreepNameList(){
			creepNameList=new string[creepList.Count];
			for(int i=0; i<creepList.Count; i++) creepNameList[i]=creepList[i].unitName;
		}
		
		private static void GetSpawnManager(){
			instance=(SpawnManager)FindObjectOfType(typeof(SpawnManager));

			if(instance!=null){
				window.waveFoldList=new List<bool>();
				for(int i=0; i<instance.waveList.Count; i++) window.waveFoldList.Add(true);
				
				//if path is empty, get a path
				if(instance.defaultPath==null) instance.defaultPath=(PathTD)FindObjectOfType(typeof(PathTD));
				
				//verify and setup the procedural wave generation unit list
				List<ProceduralUnitSetting> unitSettingList=instance.waveGenerator.unitSettingList;
				List<ProceduralUnitSetting> newSettingList=new List<ProceduralUnitSetting>();
				for(int i=0; i<creepList.Count; i++){
					bool match=false;
					for(int n=0; n<unitSettingList.Count; n++){
						if(unitSettingList[n].unit==creepList[i].gameObject){
							newSettingList.Add(unitSettingList[n]);
							match=true;
							break;
						}
					}
					if(!match){
						ProceduralUnitSetting unitSetting=new ProceduralUnitSetting();
						unitSetting.unit=creepList[i].gameObject;
						newSettingList.Add(unitSetting);
					}
				}
				instance.waveGenerator.unitSettingList=newSettingList;
				
				instance.waveGenerator.CheckPathList();
				if(instance.defaultPath!=null && instance.waveGenerator.pathList.Count==0)
					instance.waveGenerator.pathList.Add(instance.defaultPath);
				
				List<ProceduralVariable> rscSettingList=instance.waveGenerator.rscSettingList;
				while(rscList.Count>rscSettingList.Count) rscSettingList.Add(new ProceduralVariable(10, 500));
				while(rscList.Count<rscSettingList.Count) rscSettingList.RemoveAt(rscSettingList.Count-1);
				
				EditorUtility.SetDirty(instance);
			}
		}
		
		
		
		
		
		private Vector2 scrollPos;
		private int deleteID=-1;
		
		private GUIContent cont;
		private GUIContent[] contList;
		
		
		Vector2 WaveConfigurator(float startX, float startY){
			
			for(int i=0; i<instance.waveList.Count; i++){
				Wave wave=instance.waveList[i];
				
				float foldOffset=25;
				if(deleteID==i){
					if(GUI.Button(new Rect(startX, startY-3, 60, 20), "Cancel")){
						deleteID=-1;
					}
					GUI.color=Color.red;
					if(GUI.Button(new Rect(startX+65, startY-3, 20, 20), "X")){
						instance.waveList.RemoveAt(i);	i-=1;
						deleteID=-1;
					}
					GUI.color=Color.white;
					
					foldOffset+=65;
				}
				else{
					cont=new GUIContent("X", "Delete wave");
					if(GUI.Button(new Rect(startX, startY-3, 20, 20), cont)){
						deleteID=i;
					}
				}
				
				waveFoldList[i] = EditorGUI.Foldout(new Rect(startX+foldOffset, startY, 60, 15), waveFoldList[i], "wave "+(i+1).ToString());
				if(!waveFoldList[i]) {
					
					
					PreviewSubWave(wave, startX+foldOffset, startY, spaceX, width, spaceY, height);
					
					startY+=35;
					continue;
				}
				
				startX+=20;
				
				
				
				cont=new GUIContent("SubWave Size: "+wave.subWaveList.Count, "Number of subwave in this wave, each subwave can be made of different creep with different spawn configuration");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, 15), cont);
				if(GUI.Button(new Rect(startX+spaceX+10, startY-1, 40, 15), "-1")){
					if(wave.subWaveList.Count>1) wave.subWaveList.RemoveAt(wave.subWaveList.Count-1);
				}
				if(GUI.Button(new Rect(startX+spaceX+60, startY-1, 40, 15), "+1")){
					wave.subWaveList.Add(wave.subWaveList[wave.subWaveList.Count-1].Clone());
				}
				
				
				if(instance.spawnMode==SpawnManager._SpawnMode.Continous){
					cont=new GUIContent("Duration: ", "Time until next wave");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
					wave.duration=EditorGUI.FloatField(new Rect(startX+spaceX-20, startY, 40, 15), wave.duration);
					
					float reqDuration=wave.CalculateSpawnDuration();
					EditorGUI.LabelField(new Rect(startX+spaceX+30, startY, 800, height), "(Time required for all units in this wave to be spawned: "+reqDuration.ToString("f1")+"s)");
				}
				
				
				float cachedX=startX;
				startY+=3;
				
				Vector2 v2=SubWaveConfigurator(wave, startX, startY, spaceX, width, spaceY, height);
					
				startY=v2.y+5;
				startX=cachedX;
				
				
				cont=new GUIContent("Life Gain: ", "The amount of life player will gain when surviving the wave");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				wave.lifeGain=EditorGUI.IntField(new Rect(startX+spaceX-20, startY, 40, 15), wave.lifeGain);
				
				//~ startX+=spaceX+75;
				cont=new GUIContent("Energy Gain: ", "The amount of energy (for abilities) player will gain when surviving the wave");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY-2, width, height), cont);
				wave.energyGain=EditorGUI.IntField(new Rect(startX+spaceX-20, startY, 40, 15), wave.energyGain);
				
				//~ startX=cachedX;	startY+=3;
				
				
				
				cont=new GUIContent("Resource Gain:", "The amount of resource player will gain when surviving the wave");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, 300, 15), cont);
				
				if(wave.rscGainList.Count<rscList.Count) wave.rscGainList.Add(0);
				if(wave.rscGainList.Count>rscList.Count) wave.rscGainList.RemoveAt(wave.rscGainList.Count-1);
				
				cachedX=startX;
				startX+=spaceX;
				for(int n=0; n<rscList.Count; n++){
					EditorUtilities.DrawSprite(new Rect(startX, startY-2, 20, 20), rscList[n].icon);
					wave.rscGainList[n]=EditorGUI.IntField(new Rect(startX+21, startY, 40, height-2), wave.rscGainList[n]);
					startX+=75;
				}
				startX=cachedX;
				
				
				startY+=50;
				startX-=20;
			}
			
			return new Vector2(startX, startY);
		}
		
		Vector2 SubWaveConfigurator(Wave wave, float startX, float startY, float spaceX, float width, float spaceY, float height){
			float cachedY=startY;
			
			startX+=5;
			
			float width2=width*.6f;
			
			for(int i=0; i<wave.subWaveList.Count; i++){
				SubWave subWave=wave.subWaveList[i];
				
				float cachedX=startX;
				
				GUI.Box(new Rect(startX-5, startY+spaceY-5, spaceX+width2+10, 8*spaceY+47), "");
				
				int unitID=-1;
				for(int n=0; n<creepList.Count; n++){ if(subWave.unit==creepList[n].gameObject) unitID=n;	}
			
				Sprite icon=unitID==-1 ? null : creepList[unitID].iconSprite;
				EditorUtilities.DrawSprite(new Rect(startX, startY+spaceY, 30, 30), icon);
				
				cont=new GUIContent("Creep Prefab:", "The creep prefab to be spawned");
				EditorGUI.LabelField(new Rect(startX+32, startY+=spaceY-2, width, height), cont);
				unitID=EditorGUI.Popup(new Rect(startX+32, startY+=spaceY-5, width+20, height), unitID, creepNameList);
				if(unitID>=0) subWave.unit=creepList[unitID].gameObject;
				
				//subWave.unit=DrawCreepSelect(startX, startY+=spaceY, subWave.unit); startY+=14;
				
				
				startX=cachedX;
				
								cont=new GUIContent("Number of Unit:", "Number of unit to be spawned");
								EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
								subWave.count=EditorGUI.IntField(new Rect(startX+spaceX, startY, width2, height), subWave.count);
				
								cont=new GUIContent("Start Delay:", "Time delay before the first creep of this subwave start spawn");
								EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
								subWave.delay=EditorGUI.FloatField(new Rect(startX+spaceX, startY, width2, height), subWave.delay);
								
								cont=new GUIContent("Spawn Interval:", "The time interval in second between each single individual spawned");
								EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
								subWave.interval=EditorGUI.FloatField(new Rect(startX+spaceX, startY, width2, height), subWave.interval);
								
								cont=new GUIContent("Alternate Path:", "The path to use for this subwave, if it's different from the default path. Optional and can be left blank");
								EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
								subWave.path=(PathTD)EditorGUI.ObjectField(new Rect(startX+spaceX, startY, width2, height), subWave.path, typeof(PathTD), true);
				
				
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY+5, width, height), "Override:");
				
						GUI.color=new Color(.5f, .5f, .5f, 1f);
						if(subWave.overrideHP>=0) GUI.color=Color.white;
				
								cont=new GUIContent(" - HP:", "Override the value of default HP set in CreepEditor. Only valid if value is set to >0");
								EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
								subWave.overrideHP=EditorGUI.FloatField(new Rect(startX+spaceX, startY, width2, height), subWave.overrideHP);
								
						GUI.color=new Color(.5f, .5f, .5f, 1f);
						if(subWave.overrideShield>=0) GUI.color=Color.white;
								
								cont=new GUIContent(" - Shield:", "Override the value of default shield set in CreepEditor. Only valid if value is set to >0");
								EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
								subWave.overrideShield=EditorGUI.FloatField(new Rect(startX+spaceX, startY, width2, height), subWave.overrideShield);
								
						GUI.color=new Color(.5f, .5f, .5f, 1f);
						if(subWave.overrideMoveSpd>=0) GUI.color=Color.white;
								
								cont=new GUIContent(" - Move Speed:", "Override the value of default MoveSpeed set in CreepEditor. Only valid if value is set to >0");
								EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
								subWave.overrideMoveSpd=EditorGUI.FloatField(new Rect(startX+spaceX, startY, width2, height), subWave.overrideMoveSpd);
								
				GUI.color=Color.white;
							
				
				if(i<wave.subWaveList.Count-1) startY=cachedY;
				startX+=spaceX+width*0.85f;
				
				if(contentWidth<startX) contentWidth=startX;
			}
			return new Vector2(startX, startY+3);
		}
		
		
		void PreviewSubWave(Wave wave, float startX, float startY, float spaceX, float width, float spaceY, float height){
			startY-=5;
			for(int i=0; i<wave.subWaveList.Count; i++){
				SubWave subWave=wave.subWaveList[i];
				startX+=70;
				
				int unitID=-1;
				for(int n=0; n<creepList.Count; n++){ if(subWave.unit==creepList[n].gameObject) unitID=n;	}
				
				if(unitID>=0 && creepList[unitID].iconSprite!=null){
					EditorUtilities.DrawSprite(new Rect(startX, startY, 30, 30), creepList[unitID].iconSprite, false, false);
					EditorGUI.LabelField(new Rect(startX+33, startY+7, 30, 30), "x"+subWave.count);
				}
			}
		}
		
		
		
		
		
		
		private bool showProceduralPathList=true;
		Vector2 WaveGeneratorSetting(float startX, float startY){
			float cachedX=startX;
			
			WaveGenerator waveGen=instance.waveGenerator;
			
			Vector2 v2;
			
			cont=new GUIContent("Sub Wave Count:");
			v2=DrawProceduralVariable(startX, startY, waveGen.subWaveCount, cont);		startX+=v2.x+20;
			cont=new GUIContent("Total Creep Count:");
			v2=DrawProceduralVariable(startX, startY, waveGen.unitCount, cont);		startX+=v2.x+20;	//startY=v2.y;
			
			startX+=140;
			showProceduralPathList=EditorGUI.Foldout(new Rect(startX, startY, 60, height), showProceduralPathList, "Path List "+(showProceduralPathList ? "" : "("+waveGen.pathList.Count+")"));
			if(showProceduralPathList){
				int count=waveGen.pathList.Count;
				count=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height-2), count);
				while(count<waveGen.pathList.Count) waveGen.pathList.RemoveAt(waveGen.pathList.Count-1);
				while(count>waveGen.pathList.Count) waveGen.pathList.Add(null);
				
				for(int i=0; i<waveGen.pathList.Count; i++){
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), "    - Element "+(i+1));
					waveGen.pathList[i]=(PathTD)EditorGUI.ObjectField(new Rect(startX+spaceX, startY, width, 15), waveGen.pathList[i], typeof(PathTD), true);
				}
			}
			
			
			if(startY<v2.y) startY=v2.y;
			
			
			startX=cachedX;
			startY+=spaceY+15;
			
			for(int i=0; i<waveGen.rscSettingList.Count; i++){
				EditorUtilities.DrawSprite(new Rect(startX, startY-5, 22, 22), rscList[i].icon);
				cont=new GUIContent("      "+rscList[i].name+":");
				v2=DrawProceduralVariable(startX, startY, waveGen.rscSettingList[i], cont);		startX+=v2.x+20;	//startY=v2.y;
			}
			
			startX=cachedX;
			startY=v2.y+spaceY+15;
			
			for(int i=0; i<waveGen.unitSettingList.Count; i++){
				ProceduralUnitSetting unitSetting=waveGen.unitSettingList[i];
				v2=DrawProceduralUnitSetting(startX, startY, unitSetting);		startY=v2.y+spaceY+15;
			}
			
			contentWidth=185*4+25;
			
			return new Vector2(185*4, startY);
		}
		
		Vector2 DrawProceduralUnitSetting(float startX, float startY, ProceduralUnitSetting unitSetting){
			
			if(unitSetting.enabled) GUI.Box(new Rect(startX, startY, 185*4, 32+5*17+12), "");
			else GUI.Box(new Rect(startX, startY, 185*4, 32+10), "");
			
			startX+=5; 	startY+=5;
			
			Vector2 v2=default(Vector2);
			float cachedX=startX;
			
			int unitID=-1;
			for(int n=0; n<creepList.Count; n++){ if(unitSetting.unit==creepList[n].gameObject) unitID=n;	}
			EditorUtilities.DrawSprite(new Rect(startX, startY, 30, 30), creepList[unitID].iconSprite);
			
			EditorGUI.LabelField(new Rect(startX+32, startY+=-2, width, height), creepList[unitID].unitName);
			
			cont=new GUIContent("enabled: ", "Check to enable unit in the procedural generation otherwise unit will not be considered at all");
			EditorGUI.LabelField(new Rect(startX+32, startY+=spaceY-5, width, height), cont);
			unitSetting.enabled=EditorGUI.Toggle(new Rect(startX+32+60, startY, width, height), unitSetting.enabled);
			
			if(!unitSetting.enabled) return new Vector2(cachedX, startY);
			
			cont=new GUIContent("Min Wave:", "The minimum wave in which the creep will start appear in");
			EditorGUI.LabelField(new Rect(startX+=185, startY-8, width, height), cont);
			unitSetting.minWave=EditorGUI.IntField(new Rect(startX+70, startY-8, 40, height), unitSetting.minWave);
			
			EditorGUI.LabelField(new Rect(cachedX, startY+7, 185*4-10, height), "______________________________________________________________________________________________________________________");
			
			
			startY+=spaceY;
			startX=cachedX;
			
			
			cont=new GUIContent("HitPoint (HP):");
			v2=DrawProceduralVariable(startX, startY, unitSetting.HP, cont);			startX+=v2.x+20;
			
			cont=new GUIContent("Shield:");
			v2=DrawProceduralVariable(startX, startY, unitSetting.shield, cont);		startX+=v2.x+20;
			
			cont=new GUIContent("Move Speed:");
			v2=DrawProceduralVariable(startX, startY, unitSetting.speed, cont);	startX+=v2.x+20;
			
			cont=new GUIContent("Spawn Interval:");
			v2=DrawProceduralVariable(startX, startY, unitSetting.interval, cont);	startX+=v2.x+20;
				
			return v2;
		}
		
		Vector2 DrawProceduralVariable(float startX, float startY, ProceduralVariable variable, GUIContent cont=null){
			float spaceY=17; float height=16; float spaceX=85;
			
			startX+=2; 	startY+=2;
			
			if(cont==null) cont=new GUIContent("");
			EditorGUI.LabelField(new Rect(startX, startY, width, height), cont); startY-=1;
			
			cont=new GUIContent(" - Start Value:", "Override the value of default MoveSpeed set in CreepEditor. Only valid if value is set to >0");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			variable.startValue=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), variable.startValue);
			
			cont=new GUIContent(" - Increment:", "Override the value of default MoveSpeed set in CreepEditor. Only valid if value is set to >0");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			variable.incMultiplier=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), variable.incMultiplier);
			
			cont=new GUIContent(" - Deviation:", "Override the value of default MoveSpeed set in CreepEditor. Only valid if value is set to >0");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			variable.devMultiplier=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), variable.devMultiplier);
			
			cont=new GUIContent(" - Min/Max:", "Override the value of default MoveSpeed set in CreepEditor. Only valid if value is set to >0");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			variable.minValue=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), variable.minValue);
			variable.maxValue=EditorGUI.FloatField(new Rect(startX+spaceX+40, startY, 40, height), variable.maxValue);
			
			return new Vector2(spaceX+80, startY);
		}
		
		private float width=120;
		private float spaceX=100;
		private float height=18;
		private float spaceY=20;
		
		private bool configureAutoGen=false;
		void OnGUI(){
			if(window==null) Init();
			if(instance==null){
				EditorGUI.LabelField(new Rect(5, 5, 350, 18), "No SpawnManager in current scene");
				GetSpawnManager();
				return;
			}
			
			if(creepList.Count!=creepNameList.Length) ResetCreepNameList();
			
			
			if(instance.spawnLimit==SpawnManager._SpawnLimit.Finite && !instance.procedurallyGenerateWave){
				if(GUI.Button(new Rect(window.position.width-130, 5, 125, 25), configureAutoGen ? "Wave List" : "Configuration")){
					configureAutoGen=!configureAutoGen;
				}
				
				if(!configureAutoGen){
					GUI.color=new Color(0, 1, 1, 1);
					cont=new GUIContent("Auto Generate", "Procedurally generate all the waves\nCAUTION: overwirte all existing wave!");
					if(GUI.Button(new Rect(window.position.width-130, 35, 125, 25), cont)){
						for(int i=0; i<instance.waveList.Count; i++) instance.waveList[i]=instance.waveGenerator.Generate(i);
					}
					GUI.color=Color.white;
				}
			}
			
			if(GUI.Button(new Rect(window.position.width-130, 90, 125, 25), "Creep Editor")){
				UnitCreepEditorWindow.Init();
			}
			
			
			float startX=5;
			float startY=5;
			
			
			int spawnMode=(int)instance.spawnMode;
			cont=new GUIContent("Spawn Mode:", "Spawn mode in this level");
			EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
			cont=new GUIContent("", "");
			contList=new GUIContent[spawnModeLabel.Length];
			for(int i=0; i<contList.Length; i++) contList[i]=new GUIContent(spawnModeLabel[i], spawnModeTooltip[i]);
			spawnMode = EditorGUI.Popup(new Rect(startX+spaceX, startY, width, 15), cont, spawnMode, contList);
			instance.spawnMode=(SpawnManager._SpawnMode)spawnMode;
			
			if(instance.spawnMode!=SpawnManager._SpawnMode.Round){
				cont=new GUIContent("Allow Skip:", "Allow player to skip ahead and spawn the next wave");
				EditorGUI.LabelField(new Rect(startX+spaceX+width+20, startY, width, height), cont);
				instance.allowSkip=GUI.Toggle(new Rect(startX+spaceX+width+90, startY, 15, 15), instance.allowSkip, "");
			}
			
			
			int spawnLimit=(int)instance.spawnLimit;
			cont=new GUIContent("Spawn Count:", "Spawn count in this level. Infinite (endless mode) must use procedural wave generation");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			cont=new GUIContent("", "");
			contList=new GUIContent[spawnLimitLabel.Length];
			for(int i=0; i<contList.Length; i++) contList[i]=new GUIContent(spawnLimitLabel[i], spawnLimitTooltip[i]);
			spawnLimit = EditorGUI.Popup(new Rect(startX+spaceX, startY, width, 15), cont, spawnLimit, contList);
			instance.spawnLimit=(SpawnManager._SpawnLimit)spawnLimit;
			
			cont=new GUIContent("Auto Start: ", "Check to have the spawning start on a fixed timer. Rather than waiting for player initiation");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width+50, height), cont);
			instance.autoStart=EditorGUI.Toggle(new Rect(startX+spaceX, startY, width, height), instance.autoStart);
			
			if(instance.autoStart){
				cont=new GUIContent("Timer: ", "The duration to wait in second before the spawning start");
				EditorGUI.LabelField(new Rect(startX+spaceX+30, startY, width+50, height), cont);
				instance.autoStartDelay=EditorGUI.FloatField(new Rect(startX+spaceX+70, startY, width-70, height), instance.autoStartDelay);
			}
			
			if(instance.spawnLimit==SpawnManager._SpawnLimit.Finite){	
				cont=new GUIContent("Auto Generate", "Check to have the SpawnManager automatically generate the wave in runtime as opposed to using preset data");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width+50, height), cont);
				instance.procedurallyGenerateWave=EditorGUI.Toggle(new Rect(startX+spaceX, startY, width, height), instance.procedurallyGenerateWave);
				
				cont=new GUIContent("Default Path:", "The primary path to be used. Every creep will follow this path unless an alternate path is specified in a sub-wave");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				instance.defaultPath=(PathTD)EditorGUI.ObjectField(new Rect(startX+spaceX, startY, width, 15), instance.defaultPath, typeof(PathTD), true);
				
				cont=new GUIContent("Waves Size: "+instance.waveList.Count, "Number of waves in the level");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, 15), cont);
				if(GUI.Button(new Rect(startX+spaceX, startY-1, 40, 15), "-1")){
					if(instance.waveList.Count>1){
						instance.waveList.RemoveAt(instance.waveList.Count-1);
						waveFoldList.RemoveAt(waveFoldList.Count-1);
					}
				}
				if(GUI.Button(new Rect(startX+spaceX+50, startY-1, 40, 15), "+1")){
					if(instance.waveList.Count>0) instance.waveList.Add(instance.waveList[instance.waveList.Count-1].Clone());
					else{
						Wave wave=new Wave();
						SubWave subWave=new SubWave();
						wave.subWaveList.Add(subWave);
						List<Rsc> rscList=EditorDBManager.GetRscList();
						wave.rscGainList=new List<int>();
						for(int i=0; i<rscList.Count; i++) wave.rscGainList.Add(0);
						instance.waveList.Add(wave);
					}
					waveFoldList.Add(true);
				}
			}
			else configureAutoGen=false;
			
			if(instance.spawnLimit==SpawnManager._SpawnLimit.Infinite || configureAutoGen){
				EditorGUI.LabelField(new Rect(startX, startY+30, 400, height), "Procedural Wave Generation Parameters");
				startY+=10;
			}
			
			startY+=35;
			float waveConfigStartY=startY;
			
			Rect visibleRect=new Rect(startX, startY, window.position.width-10, window.position.height-startY-5);
			Rect contentRect=new Rect(startX, startY, contentWidth-25, contentHeight);
			
			contentWidth=0;
				GUI.color=new Color(.8f, .8f, .8f, 1f);
				GUI.Box(visibleRect, "");
				GUI.color=Color.white;
			
			scrollPos = GUI.BeginScrollView(visibleRect, scrollPos, contentRect);
			
				startX+=5;
				startY+=10;
			
				float cachedX=startX;
				Vector2 v2=Vector2.zero;
			
				if(instance.spawnLimit==SpawnManager._SpawnLimit.Infinite || configureAutoGen || instance.procedurallyGenerateWave){
					v2=WaveGeneratorSetting(startX+5, startY);
				}
				else{
					v2=WaveConfigurator(startX, startY);
				}
				
				startX=cachedX;
				startY=v2.y;
			
			GUI.EndScrollView();
			
			contentHeight=startY-waveConfigStartY;
			
			if(GUI.changed) EditorUtility.SetDirty(instance);
		}
		
		private float contentHeight=0;
		private float contentWidth=0;
		
	}

}