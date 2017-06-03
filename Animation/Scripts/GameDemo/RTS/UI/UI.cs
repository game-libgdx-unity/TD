using UnitedSolution;using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {
	
	public class UI : MonoBehaviour {
		
		public float scaleFactor=1;
		public static float GetScaleFactor(){ return instance.scaleFactor; }
		
		public enum _BuildMode{PointNBuild, DragNDrop};
		public _BuildMode buildMode=_BuildMode.PointNBuild;
		public static bool UseDragNDrop(){ return instance.buildMode==_BuildMode.PointNBuild ? false : true; }
		
		private UnitTower selectedTower;
		
		public float fastForwardTimeScale=4;
		public static float GetFFTime(){ return instance.fastForwardTimeScale; }
		
		
		public bool disableTextOverlay=false;
		public static bool DisableTextOverlay(){ return instance.disableTextOverlay; }
		
		
		public bool pauseGameInPerkMenu=true;
		public static bool PauseGameInPerkMenu(){ return instance.pauseGameInPerkMenu; }
		

		public static UI instance;
		void Awake(){
			instance=this;
		}
		
		// Use this for initialization
		void Start () {
			
		}
		
		void OnEnable(){
			GameControl.onGameOverE += OnGameOver;
			
			Unit.onDestroyedE += OnUnitDestroyed;
			
			AbilityManager.onTargetSelectModeE += OnAbilityTargetSelectMode;
			
			FPSControl.onFPSModeE += OnFPSMode;
			//FPSControl.onFPSCameraE += OnFPSCameraActive;
			
			UnitTower.onUpgradedE += SelectTower;	//called when tower is upgraded, require for upgrade which the current towerObj is destroyed so select UI can be cleared properly 
			
			BuildManager.onAddNewTowerE += OnNewTower;	//add new tower via perk
			AbilityManager.onAddNewAbilityE += OnNewAbility;	//add new ability via perk
		}
		void OnDisable(){
			GameControl.onGameOverE -= OnGameOver;
			
			Unit.onDestroyedE -= OnUnitDestroyed;
			
			AbilityManager.onTargetSelectModeE -= OnAbilityTargetSelectMode;
			
			FPSControl.onFPSModeE -= OnFPSMode;
			//FPSControl.onFPSCameraE -= OnFPSCameraActive;
			
			UnitTower.onUpgradedE -= SelectTower;
			
			BuildManager.onAddNewTowerE -= OnNewTower;
			AbilityManager.onAddNewAbilityE -= OnNewAbility;
		}
		
		void OnGameOver(bool playerWon){ StartCoroutine(_OnGameOver(playerWon)); }
		IEnumerator _OnGameOver(bool playerWon){
			UIBuildButton.Hide();
			
			yield return new WaitForSeconds(1.0f);
			UIGameOverMenu.Show(playerWon);
		}
		
		void OnUnitDestroyed(Unit unit){
			if(!unit.IsTower()) return;
			
			if(selectedTower==unit.GetUnitTower()) ClearSelectedTower();
		}
		
		private bool abilityTargetSelecting=false;
		void OnAbilityTargetSelectMode(bool flag){ StartCoroutine(_OnAbilityTargetSelectMode(flag)); }
		IEnumerator _OnAbilityTargetSelectMode(bool flag){ 
			yield return null;
			abilityTargetSelecting=flag;
		}
		
		
		// Update is called once per frame
		void Update () {
			if(GameControl.GetGameState()==_GameState.Over) return;
			
			if(FPSControl.IsOn()) return;
			
			if(abilityTargetSelecting) return;
			
			#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
				if(Input.touchCount==1){
					Touch touch=Input.touches[0];
					
					if(UIUtilities.IsCursorOnUI(touch.fingerId)) return;
					
					if(!UseDragNDrop() && !UIBuildButton.isOn) BuildManager.SetIndicator(touch.position);
					
					if(touch.phase==TouchPhase.Began) OnTouchCursorDown(touch.position);
				}
				else UpdateMouse();
			#else
				UpdateMouse();
			#endif
		}
		
		void UpdateMouse(){
			if(UIUtilities.IsCursorOnUI()) return;
				
			if(!UseDragNDrop() && !UIBuildButton.isOn) BuildManager.SetIndicator(Input.mousePosition);
			
			if(Input.GetMouseButtonDown(0)) OnTouchCursorDown(Input.mousePosition);
			if(Input.GetMouseButtonDown(1)) OnRightClickCursorDown(Input.mousePosition);
        }

        void OnRightClickCursorDown(Vector3 cursorPos)
        {
            Vector3 position = GameControl.ClickOnTerrain(cursorPos);
            if(selectedTower != null && selectedTower.IsHero())
            {
                ((UnitHero)selectedTower).agent.destination = position;
            }
        }

        void OnTouchCursorDown(Vector3 cursorPos){
			UnitTower tower=GameControl.Select(cursorPos);
					
			if(tower!=null){
				SelectTower(tower);
				UIBuildButton.Hide();
			}
			else{
				if(selectedTower!=null){
					ClearSelectedTower();
					return;
				}
				
				if(!UseDragNDrop()){
					if(BuildManager.CheckBuildPoint(cursorPos)==_TileStatus.Available){
						UIBuildButton.Show();
					}
					else{
						UIBuildButton.Hide();
					}
				}
			}
		}
		
		
		
		void SelectTower(UnitTower tower){
			selectedTower=tower;
			
			Vector3 screenPos=Camera.main.WorldToScreenPoint(selectedTower.thisT.position);
			UITowerInfo.SetScreenPos(screenPos);
			
			UITowerInfo.Show(selectedTower, true);
		}
		public static void ClearSelectedTower(){
			if(instance.selectedTower==null) return;
			instance.selectedTower=null;
			UITowerInfo.Hide();
			GameControl.ClearSelectedTower();
		}
		
		public static UnitTower GetSelectedTower(){ return instance.selectedTower; }
		
		
		
		void OnFPSMode(bool flag){
			//FPSModeCrosshairObj.SetActive(flag);
			
			#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
				UIGameMessage.DisplayMessage("FPS mode is not supported in mobile");
			#endif
			
			if(flag){
				UIBuildButton.Hide();
				UIAbilityButton.Hide();
				UIPerkMenu.Hide();
				UIFPSHUD.Show();
			}
			else{
				if(UseDragNDrop()) UIBuildButton.Show();
				if(AbilityManager.IsOn()) UIAbilityButton.Show();
				if(PerkManager.IsOn()) UIPerkMenu.Show();
				UIFPSHUD.Hide();
			}
		}
		void OnFPSCameraActive(bool flag){
			//Debug.Log(flag);
			//FPSModeCrosshairObj.SetActive(flag);
		}
		
		//public GameObject FPSModeCrosshairObj;
		public void OnFPSModeButton(){
			if(selectedTower==null) return;
			
			Vector3 pos=selectedTower.thisT.position+new Vector3(0, 7, 0);
			
			FPSControl.SetAnchorTower(selectedTower);
			ClearSelectedTower();
			
			FPSControl.Show(pos);
			//FPSModeCrosshairObj.SetActive(true);
			
		}
		
		void OnNewTower(UnitTower newTower){
			UIBuildButton.AddNewTower(newTower);
		}

		void OnNewAbility(Ability newAbility){
			Debug.Log("new abiility");
			UIAbilityButton.AddNewAbility(newAbility);
		}
	}

}