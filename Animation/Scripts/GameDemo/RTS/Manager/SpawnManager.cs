using UnitedSolution;using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	public class SpawnManager : MonoBehaviour {
		
		public delegate void NewWaveHandler(int waveID);
		public static event NewWaveHandler onNewWaveE;
		
		public delegate void WaveSpawnedHandler(int time);
		public static event WaveSpawnedHandler onWaveSpawnedE;	//listen by UnitedSolution
		
		public delegate void WaveClearedHandler(int time);
		public static event WaveClearedHandler onWaveClearedE;			//listen by UnitedSolution
		
		public delegate void EnableSpawnHandler();
		public static event EnableSpawnHandler onEnableSpawnE;	//call to indicate it's ready to spawn next wave (when no longer in the process of actively spawning a wave)
		
		public delegate void SpawnTimerHandler(float time);
		public static event SpawnTimerHandler onSpawnTimerE;		//call to indicate timer refresh for continous spawn
		
		
		
		public enum _SpawnMode{Continous, WaveCleared, Round}
		public _SpawnMode spawnMode;
		
		public enum _SpawnLimit{Finite, Infinite}
		public _SpawnLimit spawnLimit=_SpawnLimit.Finite;
		
		public bool allowSkip=false;
		
		public bool autoStart=false;
		public float autoStartDelay=5;
		public static bool AutoStart(){ return instance.autoStart; }
		public static float GetAutoStartDelay(){ return instance.autoStartDelay; }
		
		public bool procedurallyGenerateWave=false;	//used in finite mode, when enabled, all wave is generate procedurally
		
		public PathTD defaultPath;
		private int currentWaveID=-1;			//start at -1, switch to 0 as soon as first wave start, always indicate latest spawned wave's ID
		public bool spawning=false;
		
		public int activeUnitCount=0;	//for wave-cleared mode checking
		public int totalSpawnCount=0;	//for creep instanceID
		public int waveClearedCount=0; 	//for quick checking how many wave has been cleared
		
		public List<Wave> waveList=new List<Wave>();	//in endless mode, this is use to store temporary wave
		
		public WaveGenerator waveGenerator;
		
		public static SpawnManager instance;
		
		void Awake(){
			instance=this;
		}
		
		// Use this for initialization
		void Start () {
			if(defaultPath==null){
				Debug.Log("DefaultPath on SpawnManager not assigned, auto search for one");
				defaultPath=(PathTD)FindObjectOfType(typeof(PathTD));
			}
			
			if(spawnLimit==_SpawnLimit.Infinite || procedurallyGenerateWave){
				waveGenerator.CheckPathList();
				if(defaultPath!=null && waveGenerator.pathList.Count==0) waveGenerator.pathList.Add(defaultPath);
			}
			
			//waveGenerator.Generate(i);
			if(spawnLimit==_SpawnLimit.Finite && procedurallyGenerateWave){
				for(int i=0; i<waveList.Count; i++){
					waveList[i]=waveGenerator.Generate(i);
				}
			}
			
			if(spawnLimit==_SpawnLimit.Infinite) waveList=new List<Wave>();
			if(spawnLimit==_SpawnLimit.Finite){
				for(int i=0; i<waveList.Count; i++) waveList[i].waveID=i;
			}
			
			if(autoStart) StartCoroutine(AutoStartRoutine());
		}
		
		
		IEnumerator AutoStartRoutine(){
			yield return new WaitForSeconds(autoStartDelay);
			_Spawn();
		}
		
		
		void OnEnable(){
			Unit.onDestroyedE += OnUnitDestroyed;
			UnitCreep.onDestinationE += OnUnitReachDestination;
		}
		void OnDisable(){
			Unit.onDestroyedE -= OnUnitDestroyed;
			UnitCreep.onDestinationE -= OnUnitReachDestination;
		}
		
		
		void OnUnitDestroyed(Unit unit){
			if(!unit.IsCreep()) return;
			
			UnitCreep creep=unit.GetUnitCreep();
			OnUnitCleared(creep);
		}
		void OnUnitReachDestination(UnitCreep creep){
			//only execute if creep is dead 
			//when using path-looping the creep would be still active and wouldnt set it's dead flag to true
			if(creep.dead) OnUnitCleared(creep);
		}
		void OnUnitCleared(UnitCreep creep){
			int waveID=creep.waveID;
			
			activeUnitCount-=1;
			
			Wave wave=null;
			if(spawnLimit==_SpawnLimit.Finite) wave=waveList[waveID];
			else if(spawnLimit==_SpawnLimit.Infinite){
				for(int i=0; i<waveList.Count; i++){
					if(waveList[i].waveID==waveID){
						wave=waveList[i];
						break;
					}
				}
				
				if(wave==null){
					Debug.Log("error!");
					return;
				}
			}
			
			
			wave.activeUnitCount-=1;
			if(wave.spawned && wave.activeUnitCount==0){
				wave.cleared=true;
				waveClearedCount+=1;
				//Debug.Log("wave"+(waveID+1)+ " is cleared");
				
				ResourceManager.GainResource(wave.rscGainList, PerkManager.GetRscWaveKilled());
				GameControl.GainLife(wave.lifeGain+PerkManager.GetLifeWaveClearedModifier());
				AbilityManager.GainEnergy(wave.energyGain+(int)PerkManager.GetEnergyWaveClearedModifier());
				
				if(spawnLimit==_SpawnLimit.Infinite) waveList.Remove(wave);
				
				if(IsAllWaveCleared()){
					GameControl.GameWon();
				}
				else{
					if(spawnMode==_SpawnMode.Round && onEnableSpawnE!=null) onEnableSpawnE();
				}
			}
			
			
			if(!IsAllWaveCleared() && activeUnitCount==0 && !spawning){
				if(spawnMode==_SpawnMode.WaveCleared) SpawnWaveFinite();
			}
			
		}
		
		
		public static int AddDestroyedSpawn(UnitCreep unit){ return instance._AddDestroyedSpawn(unit); }
		public int _AddDestroyedSpawn(UnitCreep unit){
			activeUnitCount+=1;
			
			if(spawnLimit==_SpawnLimit.Finite) waveList[unit.waveID].activeUnitCount+=1;
			else if(spawnLimit==_SpawnLimit.Infinite){
				for(int i=0; i<waveList.Count; i++){
					if(waveList[i].waveID==unit.waveID){
						waveList[i].activeUnitCount+=1;
						break;
					}
				}
			}
			
			return totalSpawnCount+=1;
		}
		
		
		
		
		public static void Spawn(){ instance._Spawn(); }
		public void _Spawn(){
			if(GameControl.IsGameOver()) return;
			
			if(IsSpawningStarted()){
				if(spawnMode==_SpawnMode.Round){
					if(spawnLimit==_SpawnLimit.Infinite){
						if(waveList.Count!=0) return;
					}
					else{
						if(!waveList[currentWaveID].cleared) return;
					}
				}
				else if(!allowSkip) return;
				
				spawnCD=SpawnWaveFinite();
				
				return;
			}
			
			if(spawnMode!=_SpawnMode.Continous) SpawnWaveFinite();
			else StartCoroutine(ContinousSpawnRoutine());
			
			//spawningStarted=true;
			GameControl.StartGame();
		}
		
		
		private float spawnCD=0;
		IEnumerator ContinousSpawnRoutine(){
			while(true){
				if(GameControl.IsGameOver()) yield break;
				
				//float duration=SpawnWaveFinite();
				//if(spawnLimit==_SpawnLimit.Finite && currentWaveID>=waveList.Count) break;
				//yield return new WaitForSeconds(duration);
				
				spawnCD=SpawnWaveFinite();
				if(spawnLimit==_SpawnLimit.Finite && currentWaveID>=waveList.Count) break;
				while(spawnCD>0){
					spawnCD-=Time.deltaTime;
					yield return null;
				}
			}
		}
		
		
		private float SpawnWaveFinite(){
			if(spawning) return 0;
			
			spawning=true;
			currentWaveID+=1;
			
			if(spawnLimit==_SpawnLimit.Finite && currentWaveID>=waveList.Count) return 0;
			
			//Debug.Log("spawning wave"+(currentWaveID+1));
			if(onNewWaveE!=null) onNewWaveE(currentWaveID+1);
			
			Wave wave=null;
			if(spawnLimit==_SpawnLimit.Finite) wave=waveList[currentWaveID];
			else if(spawnLimit==_SpawnLimit.Infinite){
				wave=waveGenerator.Generate(currentWaveID);
				wave.waveID=currentWaveID;
				waveList.Add(wave);
			}
			
			if(spawnMode==_SpawnMode.Continous){
				if(spawnLimit==_SpawnLimit.Finite && currentWaveID<waveList.Count-1){
					if(onSpawnTimerE!=null) onSpawnTimerE(wave.duration);
				}
				else if(spawnLimit==_SpawnLimit.Infinite){
					if(onSpawnTimerE!=null) onSpawnTimerE(wave.duration);
				}
			}
			
			for(int i=0; i<wave.subWaveList.Count; i++){
				StartCoroutine(SpawnSubWave(wave.subWaveList[i], wave));
			}
			
			return wave.duration;
		}
		IEnumerator SpawnSubWave(SubWave subWave, Wave parentWave){
			yield return new WaitForSeconds(subWave.delay);
			
			PathTD path=defaultPath;
			if(subWave.path!=null) path=subWave.path;
				
			Vector3 pos=path.GetSpawnPoint().position;
			Quaternion rot=path.GetSpawnPoint().rotation;
			
			int spawnCount=0;
			
			while(spawnCount<subWave.count){
				GameObject obj=ObjectPoolManager.Spawn(subWave.unit, pos, rot);
				UnitCreep unit=obj.GetComponent<UnitCreep>();

				if(subWave.overrideShield>0) unit.defaultShield=subWave.overrideShield;
				if(subWave.overrideMoveSpd>0) unit.moveSpeed=subWave.overrideMoveSpd;
				
				unit.Init(path, totalSpawnCount, parentWave.waveID);
				
				totalSpawnCount+=1;
				activeUnitCount+=1;
				
				parentWave.activeUnitCount+=1;
				
				spawnCount+=1;
				if(spawnCount==subWave.count) break;
				
				yield return new WaitForSeconds(subWave.interval);
			}
			
			parentWave.subWaveSpawnedCount+=1;
			if(parentWave.subWaveSpawnedCount==parentWave.subWaveList.Count){
				parentWave.spawned=true;
				spawning=false;
				//Debug.Log("wave "+(parentWave.waveID+1)+" has done spawning");
				
				yield return new WaitForSeconds(0.5f);
				
				if(currentWaveID<=waveList.Count-2){
					//for UI to show spawn button again
					if(spawnMode==_SpawnMode.Continous && allowSkip && onEnableSpawnE!=null) onEnableSpawnE();
					if(spawnMode==_SpawnMode.WaveCleared && allowSkip && onEnableSpawnE!=null) onEnableSpawnE();
				}
			}
		}
		
		
		
		
		public bool IsSpawningStarted(){
			return currentWaveID>=0 ? true : false;
		}
		
		public static bool IsAllWaveCleared(){
			if(instance.spawnLimit==_SpawnLimit.Infinite) return false;
			
			//Debug.Log("check all wave cleared   "+instance.waveClearedCount+"   "+instance.waveList.Count);
			if(instance.waveClearedCount>=instance.waveList.Count) return true;
			return false;
		}
		
		public static int GetTotalWaveCount(){
			if(instance.spawnLimit==_SpawnLimit.Infinite) return -1;
			return instance.waveList.Count;
		}
		
		public static int GetCurrentWaveID(){ return instance.currentWaveID; }
	}

}