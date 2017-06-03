using UnitedSolution;using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnitedSolution {

	[System.Serializable]
	public class SubWave{
		public GameObject unit;
		public int count=1;
		public float interval=1;
		public float delay;
		public PathTD path;
		public float overrideHP=-1;
		public float overrideShield=-1;
		public float overrideMoveSpd=-1;
		public int overrideLifeCost=-1;
		public int overrideScoreCost=-1;
		public int[] overrideValue=new int[0];
		public List<int> overrideValueMin=new List<int>();
		public List<int> overrideValueMax=new List<int>();
		
		public SubWave Clone(){
			SubWave subWave=new SubWave();
			subWave.unit=unit;
			subWave.count=count;
			subWave.interval=interval;
			subWave.delay=delay;
			subWave.path=path;
			subWave.overrideHP=overrideHP;
			subWave.overrideShield=overrideShield;
			subWave.overrideMoveSpd=overrideMoveSpd;
			subWave.overrideLifeCost=overrideLifeCost;
			subWave.overrideScoreCost=overrideScoreCost;
			//subWave.overrideValue=overrideValue;
			subWave.overrideValueMin=new List<int>(overrideValueMin);
			subWave.overrideValueMax=new List<int>(overrideValueMax);
			return subWave;
		}
		
	}

	[System.Serializable]
	public class Wave{
		[HideInInspector] public int waveID=-1;
		public List<SubWave> subWaveList=new List<SubWave>();
		
		public int lifeGain=0;
		public int energyGain=0;
		public int scoreGain=100;
		public List<int> rscGainList=new List<int>();
		
		public int activeUnitCount=0;	//only used in runtime
		
		[HideInInspector] public bool spawned=false; //flag indicating weather all unit in the wave have been spawn, only used in runtime
		[HideInInspector] public bool cleared=false; //flag indicating weather the wave has been cleared, only used in runtime
		
		public float duration=10;						//duration until next wave
		public int subWaveSpawnedCount=0;		//the number of subwave that finish spawning, used to check if all the spawning is done, only used in runtime
		
		//calculate the time require to spawn this wave
		public float CalculateSpawnDuration(){
			float duration=0;
			for(int i=0; i<subWaveList.Count; i++){
				SubWave subWave=subWaveList[i];
				float thisDuration=(subWave.count-1)*subWave.interval+subWave.delay;
				if(thisDuration>duration){
					duration=thisDuration;
				}
			}
			return duration;
		}
		
		public Wave Clone(){
			Wave wave=new Wave();
			wave.duration=duration;
			wave.scoreGain=scoreGain;
			
			for(int i=0; i<subWaveList.Count; i++) wave.subWaveList.Add(subWaveList[i].Clone());
			
			//for(int i=0; i<rscGainList.Count; i++) wave.rscGainList.Add(rscGainList[i]);
			wave.rscGainList=new List<int>(rscGainList);
			
			return wave;
		}
	}
	
}
