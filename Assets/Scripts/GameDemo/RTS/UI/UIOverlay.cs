using UnitedSolution;using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {
	
	
	public class TextOverlay{
		public delegate void TextOverlayHandler(TextOverlay textO); 
		public static event TextOverlayHandler onTextOverlayE;
	
		public Vector3 pos;
		public string msg;
		public Color color;
		public bool useColor=false;
		
		public TextOverlay(Vector3 p, string m){
			float rand=.25f;
			Vector3 posR=new Vector3(Random.Range(-rand, rand), Random.Range(-rand, rand), Random.Range(-rand, rand));
			pos=p+posR;
			msg=m;
			if(onTextOverlayE!=null) onTextOverlayE(this);
		}
		public TextOverlay(Vector3 p, string m, Color col){
			float rand=.25f;
			Vector3 posR=new Vector3(Random.Range(-rand, rand), Random.Range(-rand, rand), Random.Range(-rand, rand));
			pos=p+posR;
			msg=m;
			color=col;
			useColor=true;
			
			if(onTextOverlayE!=null) onTextOverlayE(this);
		}
	}
	
	
	
	public class UIOverlay : MonoBehaviour {
		
		[HideInInspector] public Camera mainCam;
		public void OnRefreshMainCamera(){ mainCam=Camera.main; }
		
		public List<Slider> buildingBarList=new List<Slider>();
		public List<UnitOverlay> overlayList=new List<UnitOverlay>();
		public List<Text> textOverlayList=new List<Text>();
		
		public List<Unit> overlayedUnitList=new List<Unit>();
		
		
		
		public static void NewUnit(Unit unit){ instance.StartCoroutine(instance._UnitOverlay(unit)); }
		IEnumerator _UnitOverlay(Unit unit){
			if(overlayedUnitList.Contains(unit)) yield break;
			
			overlayedUnitList.Add(unit);
			
			UnitOverlay overlay=GetUnusedOverlay();
			overlay.rootObj.SetActive(true);
			
			if(unit.defaultShield>0) overlay.barShield.gameObject.SetActive(true);
			else overlay.barShield.gameObject.SetActive(false);
			
			while(unit!=null && !unit.dead && unit.thisObj.activeInHierarchy){
				overlay.barHP.value=unit.HP/unit.fullHP;
				if(unit.defaultShield>0) overlay.barShield.value=unit.shield/unit.fullShield;
				
				Vector3 screenPos = mainCam.WorldToScreenPoint(unit.thisT.position+new Vector3(0, 0, 0));
				overlay.rootT.localPosition=(screenPos+new Vector3(0, 20, 0))/UI.GetScaleFactor();
				
				if(overlay.barHP.value==1 && overlay.barShield.value==1) break;
				
				yield return null;
			}
			
			overlay.rootObj.SetActive(false);
			overlayedUnitList.Remove(unit);
		}
		
		public static UIOverlay instance;
		
		void Awake(){
			instance=this;
			
			for(int i=0; i<3; i++){
				if(i>0){
					GameObject obj=(GameObject)Instantiate(buildingBarList[0].gameObject);
					buildingBarList.Add(obj.GetComponent<Slider>());
					buildingBarList[i].transform.SetParent(buildingBarList[0].transform.parent);
				}
				buildingBarList[i].gameObject.SetActive(false);
			}
			
			for(int i=0; i<10; i++){
				if(i>0){
					GameObject obj=(GameObject)Instantiate(textOverlayList[0].gameObject);
					textOverlayList.Add(obj.GetComponent<Text>());
					textOverlayList[i].transform.SetParent(textOverlayList[0].transform.parent);
				}
				textOverlayList[i].text="";
				textOverlayList[i].gameObject.SetActive(false);
			}
			
			for(int i=0; i<15; i++){
				if(i==0) overlayList[i].Init();
				else overlayList.Add(overlayList[0].Clone());
				overlayList[i].rootObj.SetActive(false);
			}
			
			mainCam=Camera.main;
		}
		
		
		void OnEnable(){
			Unit.onDamagedE += NewUnit;
			
			UnitTower.onConstructionStartE += Building;
			
			TextOverlay.onTextOverlayE += OnTextOverlay;
			
			FPSControl.onFPSCameraE += OnRefreshMainCamera;
		}
		void OnDisable(){
			Unit.onDamagedE -= NewUnit;
			
			UnitTower.onConstructionStartE -= Building;
			
			TextOverlay.onTextOverlayE -= OnTextOverlay;
			
			FPSControl.onFPSCameraE -= OnRefreshMainCamera;
		}
		
		void OnTextOverlay(TextOverlay overlayInstance){
			if(UI.DisableTextOverlay()) return;
			
			Text txt=GetUnusedTextOverlay();
			
			txt.text=overlayInstance.msg;
			if(overlayInstance.useColor) txt.color=overlayInstance.color;
			else txt.color=new Color(1f, 150/255f, 0, 1f);
			
			Vector3 screenPos = mainCam.WorldToScreenPoint(overlayInstance.pos);
			txt.transform.localPosition=screenPos/UI.GetScaleFactor();
			
			txt.gameObject.SetActive(true);
			
			StartCoroutine(TextOverlayRoutine(txt));
		}
		IEnumerator TextOverlayRoutine(Text txt){
			Transform txtT=txt.transform;
			float duration=0;
			while(duration<1){
				txtT.localPosition+=new Vector3(0, 30*Time.deltaTime, 0);
				Color color=txt.color;
				color.a=1-duration;
				//~ if(duration>0.5f) color.a=1-duration*2;
				txt.color=color;
				
				duration+=Time.deltaTime*1.5f;
				yield return null;
			}
			//txt.text="";
			txt.gameObject.SetActive(false);
		}
		
		
		
		public static void Building(UnitTower tower){ instance.StartCoroutine(instance._Building(tower)); }
		IEnumerator _Building(UnitTower tower){
			Slider bar=GetUnusedBuildingBar();
			Transform barT=bar.transform;
			while(tower!=null && tower.IsInConstruction()){
				bar.value=tower.GetBuildProgress();
				
				if(mainCam==null){
					mainCam=Camera.main;
					continue;
				}
				
				Vector3 screenPos = mainCam.WorldToScreenPoint(tower.thisT.position+new Vector3(0, 0, 0));
				barT.localPosition=(screenPos+new Vector3(0, -20, 0))/UI.GetScaleFactor();
				bar.gameObject.SetActive(true);
				
				yield return null;
			}
			bar.gameObject.SetActive(false);
		}
		
		Slider GetUnusedBuildingBar(){
			for(int i=0; i<buildingBarList.Count; i++){
				if(!buildingBarList[i].gameObject.activeInHierarchy) return buildingBarList[i];
			}
			GameObject obj=(GameObject)Instantiate(buildingBarList[0].gameObject);
			obj.transform.parent=buildingBarList[0].transform.parent;
			obj.transform.localScale=buildingBarList[0].transform.localScale;
			Slider slider=obj.GetComponent<Slider>();
			buildingBarList.Add(slider);
			return slider;
		}
		
		UnitOverlay GetUnusedOverlay(){
			for(int i=0; i<overlayList.Count; i++){
				if(!overlayList[i].rootObj.activeInHierarchy) return overlayList[i];
			}
			UnitOverlay overlay=overlayList[0].Clone();
			overlayList.Add(overlay);
			return overlay;
		}
		
		Text GetUnusedTextOverlay(){
			for(int i=0; i<textOverlayList.Count; i++){
				if(textOverlayList[i].text=="") return textOverlayList[i];
			}
			
			GameObject obj=(GameObject)Instantiate(textOverlayList[0].gameObject);
			obj.transform.SetParent(textOverlayList[0].transform.parent);
			obj.transform.localScale=textOverlayList[0].transform.localScale;
			Text txt=obj.GetComponent<Text>();
			textOverlayList.Add(txt);
			return txt;
		}

		
		// Update is called once per frame
		//~ void Update () {
			//~ if(Input.GetMouseButtonDown(0)){
				//~ Vector3 pos=new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
				//~ new TextOverlay(pos, Random.Range(1, 999).ToString(), Color.white);
			//~ }
		//~ }
		
		
	}

}