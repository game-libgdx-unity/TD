using UnitedSolution;using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	public class UITowerInfo : MonoBehaviour {
		
		public UnitTower currentTower;
		
		public static UITowerInfo instance;
		private GameObject thisObj;
	
		public Transform anchorLeft;
		public Transform anchorRight;
		public Transform frameT;
		
		public Text txtName;
		public Text txtLvl;
		public Text txtDesp1;
		public Text txtDesp2;
		
		private GameObject rscObj;
		public List<UnityButton> rscObjList=new List<UnityButton>();
		
		public GameObject floatingButtons;
		public GameObject butUpgrade;
		public GameObject butSell;
		
		public GameObject butUpgradeAlt1;
		public GameObject butUpgradeAlt2;
		
		private int upgradeOption=1;	//a number recording the upgrade option available for the tower, update everytime tower.ReadyToBeUpgrade() is called
													//0 - not upgradable
													//1 - can be upgraded
													//2 - 2 upgrade path, shows 2 upgrade buttons
		
		public GameObject rscTooltipObj;
		public List<UnityButton> rscTooltipObjList=new List<UnityButton>();
		
		public GameObject controlObj;
		public GameObject priorityObj;
		public Text txtTargetPriority;
		
		public GameObject directionObj;
		public Slider sliderDirection;
		
		public GameObject fpsButtonObj;
		
		void Start(){
			instance=this;
			thisObj=gameObject;
			
			//for build cost
			List<Rsc> rscList=ResourceManager.GetResourceList();
			for(int i=0; i<rscList.Count; i++){
				if(i==0) rscObjList[i].Init();
				else rscObjList.Add(rscObjList[0].Clone("RscObj"+i, new Vector3(i*80, 0, 0)));
				rscObjList[i].imageIcon.sprite=rscList[i].icon;
			}
			rscObj=rscObjList[0].rootT.parent.gameObject;
			
			
			//for upgrade/selling cost
			for(int i=0; i<rscList.Count; i++){
				if(i==0) rscTooltipObjList[i].Init();
				else rscTooltipObjList.Add(rscTooltipObjList[0].Clone("RscObj"+i, new Vector3(i*50, 0, 0)));
				rscTooltipObjList[i].imageIcon.sprite=rscList[i].icon;
			}
			float offset=0.5f * (rscList.Count-1) * 50;
			for(int i=0; i<rscList.Count; i++)  rscTooltipObjList[i].rootT.localPosition+=new Vector3(-offset, 0, 0);
			rscTooltipObj.GetComponent<RectTransform>().sizeDelta+=new Vector2((rscList.Count-1)*50, 0);
			
			rscTooltipObj.SetActive(false);
			
			Hide();
		}

		// Use this for initialization
		//~ void Start () {
			
		//~ }
		
		void OnEnable(){
			GameControl.onGameOverE += OnGameOver;
			UnitTower.onConstructionCompleteE += OnConstructionComplete;
			
			Unit.onDestroyedE += OnUnitDestroyed;
		}
		void OnDisable(){
			GameControl.onGameOverE += OnGameOver;
			UnitTower.onConstructionCompleteE -= OnConstructionComplete;
			
			Unit.onDestroyedE -= OnUnitDestroyed;
		}
		
		void OnGameOver(bool flag){ Hide(); }
		
		void OnTowerUpgraded(UnitTower tower){
			Show(tower, true);
		}
		
		void OnConstructionComplete(UnitTower tower){
			if(tower!=currentTower) return;
			
			//upgradeOption=tower.ReadyToBeUpgrade();
			//if(upgradeOption>0) butUpgrade.SetActive(true);
			//else butUpgrade.SetActive(false);
			
			upgradeOption=currentTower.ReadyToBeUpgrade();
			if(upgradeOption>0){
				butUpgrade.SetActive(true);
				if(upgradeOption>1) butUpgradeAlt1.SetActive(true);
				else butUpgradeAlt1.SetActive(false);
			}
			else{
				butUpgrade.SetActive(false);
				butUpgradeAlt1.SetActive(false);
			}
			
			//butUpgradeAlt1.SetActive(false);
			//butUpgradeAlt2.SetActive(false);
		}
		
		void OnUnitDestroyed(Unit unit){
			if(currentTower==null) return;
			if(!unit.IsTower() || unit.GetUnitTower()!=currentTower) return;
			Hide();
		}
		
		// Update is called once per frame
		void Update () {
			if(!isOn) return;
			
			Vector3 screenPos = Camera.main.WorldToScreenPoint(currentTower.thisT.position);
			floatingButtons.transform.localPosition=screenPos/UI.GetScaleFactor();
			
			//force the frame to go left when the tower is off screen (specifically for dragNdrop button hover)
			if(currentTower.IsSampleTower()){
				if(screenPos.x<0 || screenPos.x>Screen.width || screenPos.y<0 || screenPos.y>Screen.height){
					screenPos.x=Screen.width;
					currentX=0;
				}
			}
			
			if(currentX<Screen.width/2 && screenPos.x>Screen.width/2) _SetScreenPos(screenPos);
			else if(currentX>Screen.width/2 && screenPos.x<Screen.width/2) _SetScreenPos(screenPos);
			
		}
		
		
		
		
		public void OnUpgradeButton(){ UpgradeTower(); }
		public void OnUpgradeButtonAlt(){ UpgradeTower(1); }
		public void UpgradeTower(int upgradePath=0){
			string exception=currentTower.Upgrade(upgradePath);
			if(exception==""){
				upgradeOption=0;
				floatingButtons.SetActive(false);
			}
			else UIGameMessage.DisplayMessage(exception);
		}
		public void OnHoverUpgradeButton(){ OnHoverUpgradeSellButton(currentTower.GetCost()); }
		public void OnHoverUpgradeButtonAlt(){ OnHoverUpgradeSellButton(currentTower.GetCost(1)); }
		
		
		public void OnSellButton(){

			currentTower.Sell();
			UI.ClearSelectedTower();
		}
		public void OnHoverSellButton(){ OnHoverUpgradeSellButton(currentTower.GetValue()); }
		
		public void OnHoverUpgradeSellButton(List<int> cost){
			if(cost.Count!=rscTooltipObjList.Count) return;
			for(int i=0; i<rscTooltipObjList.Count; i++){
				rscTooltipObjList[i].label.text=cost[i].ToString();
			}
			rscTooltipObj.SetActive(true);
		}
		public void OnExitUpgradeSellButton(){ rscTooltipObj.SetActive(false); }
		
		
		public void OnTargetPrioritySwitch(){
			currentTower.SwitchToNextTargetPriority();
			txtTargetPriority.text=currentTower.targetPriority.ToString();
		}
		public void OnSliderDirectionChanged(){
			if(currentTower!=null) currentTower.ChangeScanAngle((int)sliderDirection.value);
		}
		
		
		private float currentX=0;
		public static void SetScreenPos(Vector3 pos){ instance._SetScreenPos(pos); }
		public void _SetScreenPos(Vector3 pos){
			if(pos.x<Screen.width/2){
				frameT.SetParent(anchorRight);
				frameT.localPosition=new Vector3(-170, 0, 0);
			}
			else{
				frameT.SetParent(anchorLeft);
				frameT.localPosition=new Vector3(170, 0, 0);
			}
			
			currentX=pos.x;
		}
		
		
		IEnumerator WaitForConstruction(){
			while(currentTower!=null && currentTower.IsInConstruction()) yield return null;
			if(currentTower!=null){
				Update();
				floatingButtons.SetActive(true);
			}
		}
		
		
		public static bool isOn=true;
		public static void Show(UnitTower tower, bool showControl=false){ instance._Show(tower, showControl); }
		public void _Show(UnitTower tower, bool showControl=false){

            if (!tower.IsTower())
                return;

			isOn=true;
			
			thisObj.SetActive(showControl);
			floatingButtons.SetActive(showControl);
			controlObj.SetActive(showControl);
			rscObj.SetActive(!showControl);
			
			currentTower=tower;
			
			if(showControl){
				if(currentTower.IsInConstruction()){
					StartCoroutine(WaitForConstruction());
					floatingButtons.SetActive(false);
				}
				
				upgradeOption=currentTower.ReadyToBeUpgrade();
				if(upgradeOption>0){
					butUpgrade.SetActive(true);
					if(upgradeOption>1) butUpgradeAlt1.SetActive(true);
					else butUpgradeAlt1.SetActive(false);
				}
				else{
					butUpgrade.SetActive(false);
					butUpgradeAlt1.SetActive(false);
				}
				
				txtTargetPriority.text=tower.targetPriority.ToString();
				if(tower.directionalTargeting){
					directionObj.SetActive(true);
					sliderDirection.value=tower.dirScanAngle;
				}
				else directionObj.SetActive(false);
				
				if(FPSControl.ActiveInScene()){
					if(FPSControl.UseTowerWeapon() && currentTower.FPSWeaponID==-1) 
						fpsButtonObj.SetActive(false);
					else fpsButtonObj.SetActive(true);
				}
				else fpsButtonObj.SetActive(false);
			}
			else{
				List<int> cost=currentTower.GetCost();
				for(int i=0; i<rscObjList.Count; i++){
					rscObjList[i].label.text=cost[i].ToString();
				}
			}
			
			Update();
			
			txtName.text=tower.unitName;
			txtLvl.text="lvl"+tower.GetLevel();
			txtDesp1.text=tower.GetDespStats();
			txtDesp2.text=tower.GetDespGeneral();
			
			thisObj.SetActive(isOn);
		}
		public static void Hide(){ instance._Hide(); }
		public void _Hide(){
			//currentTower=null;
			isOn=false;
			thisObj.SetActive(isOn);
		}
		
	}

}