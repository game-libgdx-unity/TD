using UnitedSolution;using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnitedSolution {

	public class AudioManager : MonoBehaviour {

        public AudioClip newWaveSound;
        public AudioClip waveClearedSound;
        public AudioClip gameWonSound;
        public AudioClip gameLostSound;
        public AudioClip towerConstructingSound;
        public AudioClip towerConstructedSound;
        public AudioClip towerUpgradedSound;
        public AudioClip towerSoldSound;
        public AudioClip towerDestroyedSound;
        public AudioClip abilityActivatedSound;
        public AudioClip fpsModeSound;
        public AudioClip fpsReloadSound;
        public AudioClip fpsSwitchWeaponSound;
        public AudioClip perkPurchasedSound;
        private List<AudioSource> audioSourceList=new List<AudioSource>();
		
		private static float musicVolume=.75f;
		private static float sfxVolume=.75f;
		
		public List<AudioClip> musicList;
		public bool playMusic=true;
		public bool shuffle=false;
		private int currentTrackID=0;
		private AudioSource musicSource;
		
		
		private static AudioManager instance;
		private GameObject thisObj;
		private Transform thisT;
		
		
		public static void Init(){
			if(instance!=null) return;
			GameObject obj=new GameObject();
			obj.name="AudioManager";
			obj.AddComponent<AudioManager>();
		}
		
		
		void Awake(){
			if(instance!=null){
				Destroy(gameObject);
				return;
			}
			
			instance=this;
			
			thisObj=gameObject;
			thisT=transform;
			
			DontDestroyOnLoad(thisObj);
			
			if(playMusic && musicList!=null && musicList.Count>0){
				musicSource=thisObj.AddComponent<AudioSource>();
				musicSource.loop=false;
				musicSource.playOnAwake=false;
				musicSource.volume=musicVolume;
				
				musicSource.ignoreListenerVolume=true;
				
				if(shuffle) currentTrackID=Random.Range(0, musicList.Count);
				musicSource.clip=musicList[currentTrackID];
				musicSource.Play();
			}
			
			audioSourceList=new List<AudioSource>();
			for(int i=0; i<10; i++){
				GameObject obj=new GameObject();
				obj.name="AudioSource"+(i+1);
				
				AudioSource src=obj.AddComponent<AudioSource>();
				src.playOnAwake=false;
				src.loop=false;
				
				obj.transform.parent=thisT;
				obj.transform.localPosition=Vector3.zero;
				
				audioSourceList.Add(src);
			}
			
			AudioListener.volume=sfxVolume;
		}
		
		
		void Update(){
			if(musicSource!=null && !musicSource.isPlaying){
				if(shuffle) musicSource.clip=musicList[Random.Range(0, musicList.Count)];
				else{
					musicSource.clip=musicList[currentTrackID];
					currentTrackID+=1;
					if(currentTrackID==musicList.Count) currentTrackID=0;
				}
				
				musicSource.Play();
			}
		}
		
		
		void OnEnable(){
			SpawnManager.onNewWaveE += OnNewWave;
			SpawnManager.onWaveClearedE += OnWaveCleared;
			
			GameControl.onGameOverE += OnGameOver;
			
			Unit.onDestroyedE += OnUnitDestroyed;
			UnitCreep.onDestinationE += OnCreepDestination;
			
			UnitTower.onSoldE += OnTowerSold;
			UnitTower.onUpgradedE += OnTowerUpgraded;
			UnitTower.onConstructionStartE += OnTowerConstructing;
			UnitTower.onConstructionCompleteE += OnTowerConstructed;
			
			AbilityManager.onAbilityActivatedE += OnAbilityActivated;
			
			FPSControl.onFPSModeE += OnFPSMode;
			FPSControl.onFPSReloadE += OnFPSReload;
			FPSControl.onSwitchWeaponE += OnFPSSwitchWeapon;
			
			PerkManager.onPerkPurchasedE += OnPerkPurchased;
		}
		
		void OnDisable(){
			SpawnManager.onNewWaveE -= OnNewWave;
			SpawnManager.onWaveClearedE -= OnWaveCleared;
			
			GameControl.onGameOverE -= OnGameOver;
			
			Unit.onDestroyedE -= OnUnitDestroyed;
			UnitCreep.onDestinationE -= OnCreepDestination;
			
			UnitTower.onSoldE -= OnTowerSold;
			UnitTower.onUpgradedE -= OnTowerUpgraded;
			UnitTower.onConstructionStartE -= OnTowerConstructing;
			UnitTower.onConstructionCompleteE -= OnTowerConstructed;
			
			AbilityManager.onAbilityActivatedE -= OnAbilityActivated;
			
			FPSControl.onFPSModeE -= OnFPSMode;
			FPSControl.onFPSReloadE -= OnFPSReload;
			FPSControl.onSwitchWeaponE -= OnFPSSwitchWeapon;
			
			PerkManager.onPerkPurchasedE -= OnPerkPurchased;
		}
		
		
		void OnNewWave(int waveID){ if(newWaveSound!=null) _PlaySound(newWaveSound); }
		void OnWaveCleared(int waveID){ if(waveClearedSound!=null) _PlaySound(waveClearedSound); }
		
		void OnGameOver(bool playerWon){ 
			if(playerWon){ if(gameWonSound!=null) _PlaySound(gameWonSound);  }
			else{ if(gameLostSound!=null) _PlaySound(gameLostSound);  }
		}
		
		//void OnUnitDestroyed(Unit unit){ if(newWaveSound!=null) _PlaySound(newWaveSound); }
		void OnCreepDestination(UnitCreep creep){ if(newWaveSound!=null) _PlaySound(newWaveSound); }
			
		void OnTowerSold(UnitTower tower){ if(towerSoldSound!=null) _PlaySound(towerSoldSound); }
		void OnTowerUpgraded(UnitTower tower){ if(towerUpgradedSound!=null) _PlaySound(towerUpgradedSound); }
		void OnTowerConstructing(UnitTower tower){ if(towerConstructingSound!=null) _PlaySound(towerConstructingSound); }
		void OnTowerConstructed(UnitTower tower){ if(towerConstructedSound!=null) _PlaySound(towerConstructedSound); }
		void OnUnitDestroyed(Unit unit){
			if(unit.IsTower() && towerDestroyedSound!=null) _PlaySound(towerDestroyedSound); 
		}
			
		void OnAbilityActivated(Ability ab){ if(abilityActivatedSound!=null) _PlaySound(abilityActivatedSound); }
			
		void OnFPSMode(bool flag){ if(fpsModeSound!=null) _PlaySound(fpsModeSound); }
		void OnFPSReload(bool flag){ if(flag && fpsReloadSound!=null) _PlaySound(fpsReloadSound); }
		void OnFPSSwitchWeapon(){ if(fpsSwitchWeaponSound!=null) _PlaySound(fpsSwitchWeaponSound); }
			
		void OnPerkPurchased(Perk perk){ if(perkPurchasedSound!=null) _PlaySound(perkPurchasedSound); }
		
		//check for the next free, unused audioObject
		private int GetUnusedAudioSourceID(){
			for(int i=0; i<audioSourceList.Count; i++){
				if(!audioSourceList[i].isPlaying) return i;
			}
			return 0;	//if everything is used up, use item number zero
		}
		
		
		//call to play a specific clip
		public static void PlaySound(AudioClip clip){ 
			if(instance==null) Init();
			instance._PlaySound(clip);
		}
		public void _PlaySound(AudioClip clip){ 
			int ID=GetUnusedAudioSourceID();
			
			audioSourceList[ID].clip=clip;
			audioSourceList[ID].Play();
		}
		
		
		public static void SetSFXVolume(float val){
			sfxVolume=val;
			AudioListener.volume=val;
		}
		
		public static void SetMusicVolume(float val){
			musicVolume=val;
			if(instance && instance.musicSource) instance.musicSource.volume=val;
		}
		
		public static float GetMusicVolume(){ return musicVolume; }
		public static float GetSFXVolume(){ return sfxVolume; }
	}
}