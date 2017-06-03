using UnitedSolution;using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	public class UIPerkMenu : MonoBehaviour {

		public enum _ItemState{Selected, Normal, Unavailable, Purchased}
		
		[System.Serializable]
		public class PerkItem{
			//[HideInInspector] public Perk perk;
			public int perkID=-1;		//the ID of the perk in PerkManager's perkList
			public int perkIndex=0;	//the index of the perk in PerkManager's perkList
			public UnityButton button=new UnityButton();
			public GameObject linkObj;
			public _ItemState state=_ItemState.Normal;
		}
		
		
		public GameObject perkButtonObj;
		public GameObject menuObj;
		
		
		public Text lbPerkPoint;
		public RectTransform scrollViewContent;
		
		
		public bool assignItemManually=false;
		[HideInInspector] public List<PerkItem> itemList=new List<PerkItem>();
		public GameObject backupItemObj;
		
		public Transform selectHighlightT;
		
		public int selectedID=-1;
		
		public Text lbName;
		public Text lbDesp;
		public Text lbReq;
		public Text lbPurchased;
		public GameObject butPurchaseObj;
		
		private GameObject rscObj;
		public List<UnityButton> rscObjList=new List<UnityButton>();
		
		
		private static UIPerkMenu instance;
		
		void Awake(){
			instance=this;
		}
		
		// Use this for initialization
		void Start() {
			if(!PerkManager.IsOn()){
				gameObject.SetActive(false);
				return;
			}
			
			if(!assignItemManually){
				List<Perk> perkList=PerkManager.GetPerkList();
				
				itemList=new List<PerkItem>();
				PerkItem item0=new PerkItem();
				item0.button.rootObj=backupItemObj;
				itemList.Add(item0);
				
				int count=0; float y=0;
				for(int i=0; i<perkList.Count; i++){
					if(i==0) itemList[0].button.Init();
					else if(i>0){
						PerkItem item=new PerkItem();
						item.button=itemList[0].button.Clone("ItemButton"+(count+1), new Vector3(count*60, y, 0));
						itemList.Add(item);
					}
					
					count+=1; 
					if(count==8){ count=0; y-=60; }
					
					itemList[i].button.imageIcon.sprite=perkList[i].icon;
					itemList[i].perkID=perkList[i].ID;
					itemList[i].perkIndex=i;
					
					if(perkList[i].purchased) SetToPurchased(itemList[i]);
					else if(perkList[i].IsAvailable()!="") SetToUnavailable(itemList[i]);
					else SetToNormal(itemList[i]);
					
				}
				
				scrollViewContent.sizeDelta=new Vector2(500, -y+60+Mathf.Abs(itemList[0].button.rootT.localPosition.y));
			}
			else{
				List<Perk> perkList=PerkManager.GetPerkList();
				for(int i=0; i<itemList.Count; i++){
					if(itemList[i].button.rootObj!=null){
						itemList[i].button.Init();
						
						if(itemList[i].perkID>=0){
							for(int n=0; n<perkList.Count; n++){
								if(perkList[n].ID==itemList[i].perkID){
									itemList[i].button.imageIcon.sprite=perkList[n].icon;
									itemList[i].perkIndex=n;
									
									if(perkList[n].purchased) SetToPurchased(itemList[i]);
									else if(perkList[n].IsAvailable()!="") SetToUnavailable(itemList[i]);
									else SetToNormal(itemList[i]);
									
									break;
								}
							}
						}
						
					}
				}
			}
			
			
			List<Rsc> rscList=ResourceManager.GetResourceList();
			for(int i=0; i<rscList.Count; i++){
				if(i==0) rscObjList[i].Init();
				else rscObjList.Add(rscObjList[0].Clone("RscObj"+i, new Vector3(i*80, 0, 0)));
				rscObjList[i].imageIcon.sprite=rscList[i].icon;
			}
			rscObj=rscObjList[0].rootT.parent.gameObject;
			
			UpdatePerkPoint(PerkManager.GetPerkPoint());
			
			//select the first item
			OnItemButton(itemList[0].button.rootObj);
			
			menuObj.transform.localPosition=new Vector3(0, 10, 0);
			
			_Hide(false);
		}
		
		
		void OnEnable(){
			PerkManager.onPerkPointE += UpdatePerkPoint;
			
			SpawnManager.onNewWaveE += UpdatePerkItem;
		}
		void OnDisable(){
			PerkManager.onPerkPointE -= UpdatePerkPoint;
			
			SpawnManager.onNewWaveE -= UpdatePerkItem;
		}
		
		void UpdatePerkPoint(int point){ 
			lbPerkPoint.text="Points: "+PerkManager.GetPerkPoint();
		}
		
		
		//on each new wave, some unavailble perk may become available
		void UpdatePerkItem(int waveID=0){
			List<Perk> perkList=PerkManager.GetPerkList();
			
			for(int i=0; i<itemList.Count; i++){
				if(itemList[i].state==_ItemState.Unavailable || itemList[i].state==_ItemState.Selected){
					if(perkList[itemList[i].perkIndex].IsAvailable()==""){
						SetToNormal(itemList[i]);
						if(selectedID==i) OnItemButton(itemList[i].button.rootObj);
					}
				}
			}
		}
		
		
		public void OnItemButton(GameObject butObj){
			for(int i=0; i<itemList.Count; i++){
				if(butObj==itemList[i].button.rootObj){
					ClearSelection();
					selectedID=i;
					break;
				}
			}
			
			SetToSelected(itemList[selectedID]);
			
			UpdateDisplay();
		}
		
		void UpdateDisplay(){
			if(selectedID==-1){
				lbName.text="";
				lbDesp.text="";
				lbReq.text="";
				lbPurchased.enabled=false;
				
				rscObj.SetActive(false);
				butPurchaseObj.SetActive(false);
				
				return;
			}
			
			Perk perk=PerkManager.GetPerk(itemList[selectedID].perkID);
			
			lbName.text=perk.name;
			lbDesp.text=perk.desp;
			lbPurchased.enabled=false;
			
			if(perk.purchased){
				lbName.text+=" (Purchased)";
				
				lbReq.text="";
				lbPurchased.enabled=true;
				rscObj.SetActive(false);
				butPurchaseObj.SetActive(false);
				
				return;
			}
			
			string text=perk.IsAvailable();
			if(text==""){
				List<int> cost=perk.GetCost();
				for(int i=0; i<rscObjList.Count; i++){
					rscObjList[i].label.text=cost[i].ToString();
				}
				
				lbReq.text="";
				rscObj.SetActive(true);
				butPurchaseObj.SetActive(true);
			}
			else{
				lbReq.text=text;
				rscObj.SetActive(false);
				butPurchaseObj.SetActive(false);
			}
		}
		
		
		
		void ClearSelection(){
			if(selectedID<0) return;
			
			if(PerkManager.IsPerkPurchased(itemList[selectedID].perkID)) SetToPurchased(itemList[selectedID]);
			else if(PerkManager.IsPerkAvailable(itemList[selectedID].perkID)!="") SetToUnavailable(itemList[selectedID]);
			else SetToNormal(itemList[selectedID]);
			
			selectedID=-1;
		}
		
		
		public void OnPurchaseButton(){
			string text=PerkManager.PurchasePerk(itemList[selectedID].perkID);
			
			if(text!=""){
				UIGameMessage.DisplayMessage(text);
				return;
			}
			
			for(int i=0; i<itemList.Count; i++){
				if(PerkManager.IsPerkPurchased(itemList[i].perkID)) SetToPurchased(itemList[i]);
				else if(PerkManager.IsPerkAvailable(itemList[i].perkID)!="") SetToUnavailable(itemList[i]);
				else SetToNormal(itemList[i]);
			}
			
			OnItemButton(itemList[selectedID].button.rootObj);
			
			UpdateDisplay();
		}
		
		public void OnCloseButton(){ _Hide(false); }
		
		public void OnPerkButton(){
			if(isOn) Hide(false);
			else Show(true);
		}
		
		
		public static bool isOn=true;
		public static void Show(bool showPerkMenu=false){ instance._Show(showPerkMenu); }
		public void _Show(bool showPerkMenu=false){
			UpdatePerkPoint(PerkManager.GetPerkPoint());
			
			UpdatePerkItem();
			
			if(showPerkMenu){
				if(UI.PauseGameInPerkMenu()){
					Time.timeScale=0;
					UIHUD.NormalTime();
				}
				
				isOn=true;
				menuObj.SetActive(isOn);
			}
			perkButtonObj.SetActive(true);
		}
		public static void Hide(bool hidePerkButton=true){ instance._Hide(hidePerkButton); }
		public void _Hide(bool hidePerkButton=true){
			if(UI.PauseGameInPerkMenu()){
				Time.timeScale=1;
				UIHUD.NormalTime();
			}
			
			isOn=false;
			menuObj.SetActive(isOn);
			if(hidePerkButton) perkButtonObj.SetActive(isOn);
		}
		
		
		
		
		
		private Color colorSelected=new Color(1f, 75f/255f, 75f/255f, 1f);
		private Color colorNormal=new Color(0.5f, 0.5f, 0.5f, 196f/255f);
		private Color colorUnlocked=new Color(1f, 150f/255f, 0, 1f);
		private Color colorUnavailable=new Color(.15f, .15f, .15f, 1);
		
		public void SetToSelected(PerkItem perkItem=null){
			ColorBlock colors=perkItem.button.button.colors;
			colors.normalColor = colorSelected;
			perkItem.button.button.colors=colors;
			perkItem.state=_ItemState.Selected;
			
			selectHighlightT.localPosition=perkItem.button.rootT.localPosition;
		}
		public void SetToNormal(PerkItem perkItem){
			ColorBlock colors=perkItem.button.button.colors;
			colors.normalColor = colorNormal;
			perkItem.button.button.colors=colors;
			
			perkItem.button.imageIcon.color=Color.white;
			
			perkItem.state=_ItemState.Normal;
			
			if(perkItem.linkObj!=null) perkItem.linkObj.SetActive(false);
		}
		public void SetToUnavailable(PerkItem perkItem){
			ColorBlock colors=perkItem.button.button.colors;
			colors.normalColor = colorNormal;
			perkItem.button.button.colors=colors;
			
			perkItem.button.imageIcon.color=colorUnavailable;
			
			perkItem.state=_ItemState.Unavailable;
			
			if(perkItem.linkObj!=null) perkItem.linkObj.SetActive(false);
		}
		public void SetToPurchased(PerkItem perkItem){
			ColorBlock colors=perkItem.button.button.colors;
			colors.normalColor = colorUnlocked;
			perkItem.button.button.colors=colors;
			
			perkItem.button.imageIcon.color=Color.white;
			
			perkItem.state=_ItemState.Purchased;
			
			if(perkItem.linkObj!=null) perkItem.linkObj.SetActive(true);
		}
	}

}