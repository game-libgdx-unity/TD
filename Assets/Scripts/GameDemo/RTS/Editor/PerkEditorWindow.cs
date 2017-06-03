using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	public class PerkEditorWindow : EditorWindow {
		
		private static PerkEditorWindow window;
		
		public static void Init () {
			// Get existing open window or if none, make a new one:
			window = (PerkEditorWindow)EditorWindow.GetWindow(typeof (PerkEditorWindow));
			//~ window.minSize=new Vector2(375, 449);
			//~ window.maxSize=new Vector2(375, 800);
			
			EditorDBManager.Init();
			
			InitLabel();
		}

		private static string[] perkTypeLabel;
		private static string[] perkTypeTooltip;
		
		private static void InitLabel(){
			int enumLength = Enum.GetValues(typeof(_PerkType)).Length;
			perkTypeLabel=new string[enumLength];
			perkTypeTooltip=new string[enumLength];
			for(int i=0; i<enumLength; i++){
				perkTypeLabel[i]=((_PerkType)i).ToString();
				if((_PerkType)i==_PerkType.NewTower) 	perkTypeTooltip[i]="";
				if((_PerkType)i==_PerkType.NewAbility) 	perkTypeTooltip[i]="";
				if((_PerkType)i==_PerkType.NewFPSWeapon) 	perkTypeTooltip[i]="";
				
				if((_PerkType)i==_PerkType.GainLife) 	perkTypeTooltip[i]="";
				if((_PerkType)i==_PerkType.LifeCap) 	perkTypeTooltip[i]="";
				if((_PerkType)i==_PerkType.LifeRegen) 	perkTypeTooltip[i]="";
				if((_PerkType)i==_PerkType.LifeWaveClearedBonus) 	perkTypeTooltip[i]="";
			}
		}
		
		
		
		void SelectPerk(int ID){
			selectID=ID;
			GUI.FocusControl ("");
			
			if(selectID*35<scrollPos1.y) scrollPos1.y=selectID*35;
			if(selectID*35>scrollPos1.y+listVisibleRect.height-40) scrollPos1.y=selectID*35-listVisibleRect.height+40;
		}
		
		
		private int selectID=0;
		
		private Vector2 scrollPos1;
		private Vector2 scrollPos2;
		
		private GUIContent cont;
		private GUIContent[] contL;
		
		private float contentHeight=0;
		private float contentWidth=0;
		
		private float spaceX=120;
		private float spaceY=20;
		private float width=150;
		private float height=18;
		
		void OnGUI () {
			if(window==null) Init();
			
			List<Perk> perkList=EditorDBManager.GetPerkList();
			
			if(GUI.Button(new Rect(window.position.width-120, 5, 100, 25), "Save")) EditorDBManager.SetDirtyPerk();
			
			
			if(GUI.Button(new Rect(5, 5, 120, 25), "Create New")){
				int newSelectID=EditorDBManager.AddNewPerk();
				if(newSelectID!=-1) SelectPerk(newSelectID);
			}
			if(perkList.Count>0 && GUI.Button(new Rect(130, 5, 100, 25), "Clone Selected")){
				int newSelectID=EditorDBManager.ClonePerk(selectID);
				if(newSelectID!=-1) SelectPerk(newSelectID);
			}
			
			
			float startX=5;
			float startY=55;
			
			
			if(minimiseList){
				if(GUI.Button(new Rect(startX, startY-20, 30, 18), ">>")) minimiseList=false;
			}
			else{
				if(GUI.Button(new Rect(startX, startY-20, 30, 18), "<<")) minimiseList=true;
			}
			Vector2 v2=DrawPerkList(startX, startY, perkList);	
			
			startX=v2.x+25;
			
			if(perkList.Count==0) return;
			
			
			Rect visibleRect=new Rect(startX, startY, window.position.width-startX-10, window.position.height-startY-5);
			Rect contentRect=new Rect(startX, startY, contentWidth-startY, contentHeight);
			
			scrollPos2 = GUI.BeginScrollView(visibleRect, scrollPos2, contentRect);
			
				//float cachedX=startX;
				v2=DrawPerkConfigurator(startX, startY, perkList[selectID]);
				contentWidth=v2.x+50;
				contentHeight=v2.y-55;
			
			GUI.EndScrollView();
			
			
			if(GUI.changed) EditorDBManager.SetDirtyPerk();
		}
		
		int GetListIDFromPerkID(int ID){
			List<Perk> perkList=EditorDBManager.GetPerkList();
			for(int i=0; i<perkList.Count; i++){ if(perkList[i].ID==ID) return i;}
			return 0;
		}
		int GetPerkIDFromListID(int ID){ return EditorDBManager.GetPerkList()[ID].ID; }
		
		
		Vector2 DrawPerkConfigurator(float startX, float startY, Perk perk){
			
			float cachedX=startX;
			float cachedY=startY;
			
			EditorUtilities.DrawSprite(new Rect(startX, startY, 60, 60), perk.icon);
			startX+=65;
			
			cont=new GUIContent("Name:", "The ability name to be displayed in game");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY/2, width, height), cont);
			perk.name=EditorGUI.TextField(new Rect(startX+spaceX-65, startY, width-5, height), perk.name);
			
			cont=new GUIContent("Icon:", "The ability icon to be displayed in game, must be a sprite");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.icon=(Sprite)EditorGUI.ObjectField(new Rect(startX+spaceX-65, startY, width-5, height), perk.icon, typeof(Sprite), false);
			
			startX-=65;
			startY+=10+spaceY/2;	cachedY=startY;
			
			cont=new GUIContent("Repeatable:", "Check if the ability can be repeatably purchase. For perk that offer straight, one off bonus such as life and resource");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.repeatable=EditorGUI.Toggle(new Rect(startX+spaceX, startY, 40, height), perk.repeatable);
			
			
			cont=new GUIContent("Prerequisite Perk:", "Perks that needs to be purchased before this perk is unlocked and become available");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			
			int listID=0;
			string[] perkNameList=EditorDBManager.GetPerkNameList();
			for(int i=0; i<perk.prereq.Count; i++){
				listID=GetListIDFromPerkID(perk.prereq[i])+1;
				listID=EditorGUI.Popup(new Rect(startX+spaceX, startY, width, height), listID, perkNameList);
				if(listID>0){
					int ID=GetPerkIDFromListID(listID-1);
					if(ID!=perk.ID && !perk.prereq.Contains(ID)) perk.prereq[i]=ID;
				}
				else{
					perk.prereq.RemoveAt(i);
					i-=1;
				}
				startY+=spaceY;
			}
			listID=0;
			listID = EditorGUI.Popup(new Rect(startX+spaceX, startY, width, height), listID, perkNameList);
			if(listID>0){
				int ID=GetPerkIDFromListID(listID-1);
				if(ID!=perk.ID && !perk.prereq.Contains(ID)) perk.prereq.Add(ID);
			}
			
			
			cont=new GUIContent("Min level required:", "Minimum level to reach before the perk becoming available. (level are specified in GameControl of each scene)");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.minLevel=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), perk.minLevel);
			
			cont=new GUIContent("Min wave required:", "Minimum wave to reach before the perk becoming available");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.minWave=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), perk.minWave);
			
			cont=new GUIContent("Min PerkPoint req:", "Minimum perk point to have before the perk becoming available");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.minPerkPoint=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), perk.minPerkPoint);
			
			
			List<Rsc> rscList=EditorDBManager.GetRscList();
			
			while(perk.cost.Count<rscList.Count) perk.cost.Add(0);
			while(perk.cost.Count>rscList.Count) perk.cost.RemoveAt(perk.cost.Count-1);
			
				cont=new GUIContent("Purchase Cost:", "The resource required to build/upgrade to this level");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY+3, width, height), cont);
				int count=0;	startX+=spaceX;		float cachedX2=startX;
				for(int i=0; i<perk.cost.Count; i++){
					EditorUtilities.DrawSprite(new Rect(startX, startY-1, 20, 20), rscList[i].icon);
					perk.cost[i]=EditorGUI.IntField(new Rect(startX+20, startY, 40, height), perk.cost[i]);
					count+=1; 	startX+=75;
					if(count==2){ startY+=spaceY; startX=cachedX2; }
				}
			
			//startX=cachedX;	//startY+=4;
			
			float temp=cachedY;
			cachedY=startY+15;	startY=temp;	
			startX=cachedX+310;
			
			Vector2 v2=DrawPerkType(startX, startY, perk);  float maxHeight=v2.y+40;
			
			
			startX=cachedX;	startY=cachedY;
			
			GUIStyle style=new GUIStyle("TextArea");
			style.wordWrap=true;
			cont=new GUIContent("Perk description (to be used in runtime): ", "");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, 400, 20), cont);
			perk.desp=EditorGUI.TextArea(new Rect(startX, startY+spaceY-3, 270, 150), perk.desp, style);
				
				
			return new Vector2(startX+280, Mathf.Max(maxHeight, startY+170));
		}
		
		
		Vector2 DrawItemIDTower(float startX, float startY, Perk perk, int limit=1){
			string[] towerNameList=EditorDBManager.GetTowerNameList();
			List<UnitTower> towerList=EditorDBManager.GetTowerList();
			
			if(perk.itemIDList.Count==0) perk.itemIDList.Add(-1);
			while(perk.itemIDList.Count>limit) perk.itemIDList.RemoveAt(perk.itemIDList.Count-1);
			
			for(int i=0; i<perk.itemIDList.Count; i++){
				int ID=perk.itemIDList[i];
				
				if(ID>=0){
					for(int n=0; n<towerList.Count; n++){ 
						if(towerList[n].prefabID==ID){ ID=n+1;	break; }
					}
				}
				
				cont=new GUIContent(" - Tower:", "The tower to add to game when the perk is unlocked");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				ID = EditorGUI.Popup(new Rect(startX+spaceX-30, startY, width, 15), ID, towerNameList);
				if(ID>0 && !perk.itemIDList.Contains(towerList[ID-1].prefabID)) perk.itemIDList[i]=towerList[ID-1].prefabID;
				else if(ID==0) perk.itemIDList[i]=-1;
				
				//if the list is full, extend it
				if(i==perk.itemIDList.Count-1 && ID>=0 && perk.itemIDList.Count<limit) perk.itemIDList.Add(-1);
				
				//if one of the element in the list is empty, shrink it
				if(i<perk.itemIDList.Count-1 && perk.itemIDList[i]==-1){ perk.itemIDList.RemoveAt(i); i-=1; }
			}
			
			return new Vector2(startX, startY);
		}
		
		Vector2 DrawItemIDAbility(float startX, float startY, Perk perk, int limit=1){
			string[] abilityNameList=EditorDBManager.GetAbilityNameList();
			List<Ability> abilityList=EditorDBManager.GetAbilityList();
			
			if(perk.itemIDList.Count==0) perk.itemIDList.Add(-1);
			while(perk.itemIDList.Count>limit) perk.itemIDList.RemoveAt(perk.itemIDList.Count-1);
			
			for(int i=0; i<perk.itemIDList.Count; i++){
				int ID=perk.itemIDList[i];
				
				if(ID>=0){
					for(int n=0; n<abilityList.Count; n++){ 
						if(abilityList[n].ID==ID){ ID=n+1;	break; }
					}
				}
				
				cont=new GUIContent(" - Ability:", "The ability to add to game when the perk is unlocked");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				ID = EditorGUI.Popup(new Rect(startX+spaceX-30, startY, width, 15), ID, abilityNameList);
				if(ID>0 && !perk.itemIDList.Contains(abilityList[ID-1].ID)) perk.itemIDList[i]=abilityList[ID-1].ID;
				else if(ID==0) perk.itemIDList[i]=-1;
				
				//if the list is full, extend it
				if(i==perk.itemIDList.Count-1 && ID>=0 && perk.itemIDList.Count<limit) perk.itemIDList.Add(-1);
				
				//if one of the element in the list is empty, shrink it
				if(i<perk.itemIDList.Count-1 && perk.itemIDList[i]==-1){ perk.itemIDList.RemoveAt(i); i-=1; }
			}
			
			return new Vector2(startX, startY);
		}
		
		Vector2 DrawItemIDFPSWeapon(float startX, float startY, Perk perk, int limit=1){
			string[] fpsWeaponNameList=EditorDBManager.GetFPSWeaponNameList();
			List<FPSWeapon> fpsWeaponList=EditorDBManager.GetFPSWeaponList();
			
			if(perk.itemIDList.Count==0) perk.itemIDList.Add(-1);
			while(perk.itemIDList.Count>limit) perk.itemIDList.RemoveAt(perk.itemIDList.Count-1);
			
			for(int i=0; i<perk.itemIDList.Count; i++){
				int ID=perk.itemIDList[i];
				
				if(ID>=0){
					for(int n=0; n<fpsWeaponList.Count; n++){ 
						if(fpsWeaponList[n].prefabID==ID){ ID=n+1;	break; }
					}
				}
				
				cont=new GUIContent(" - Ability:", "The ability to add to game when the perk is unlocked");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				ID = EditorGUI.Popup(new Rect(startX+spaceX-30, startY, width, 15), ID, fpsWeaponNameList);
				if(ID>0 && !perk.itemIDList.Contains(fpsWeaponList[ID-1].prefabID)) perk.itemIDList[i]=fpsWeaponList[ID-1].prefabID;
				else if(ID==0) perk.itemIDList[i]=-1;
				
				//if the list is full, extend it
				if(i==perk.itemIDList.Count-1 && ID>=0 && perk.itemIDList.Count<limit) perk.itemIDList.Add(-1);
				
				//if one of the element in the list is empty, shrink it
				if(i<perk.itemIDList.Count-1 && perk.itemIDList[i]==-1){ perk.itemIDList.RemoveAt(i); i-=1; }
			}
			
			return new Vector2(startX, startY);
		}
		
		void DrawValue(float startX, float startY, Perk perk, GUIContent cont=null){
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.value=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.value);
		}
		
		void DrawValueMinMax(float startX, float startY, Perk perk, GUIContent cont1=null, GUIContent cont2=null){
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont1);
			perk.value=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.value);
				
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont2);
			perk.valueAlt=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.valueAlt);
		}
		
		
		
		
		Vector2 DrawPerkType(float startX, float startY, Perk perk){
			int type=(int)perk.type;
			cont=new GUIContent("Perk Type:", "What the perk does");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			contL=new GUIContent[perkTypeLabel.Length];
			for(int i=0; i<contL.Length; i++) contL[i]=new GUIContent(perkTypeLabel[i], perkTypeTooltip[i]);
			type = EditorGUI.Popup(new Rect(startX+spaceX-20, startY, width, 15), new GUIContent(""), type, contL);
			perk.type=(_PerkType)type;
			
			_PerkType perkType=perk.type;
			
			
			startX+=10;
			
			
			if(perkType==_PerkType.NewTower){
				DrawItemIDTower(startX, startY, perk);
			}
			else if(perkType==_PerkType.NewAbility){
				DrawItemIDAbility(startX, startY, perk);
			}
			else if(perkType==_PerkType.NewFPSWeapon){
				DrawItemIDFPSWeapon(startX, startY, perk);
			}
			
			
			else if(perkType==_PerkType.GainLife){
				GUIContent cont1=new GUIContent(" - Min value:", "Minimum value");
				GUIContent cont2=new GUIContent(" - Max value:", "Maximum value");
				DrawValueMinMax(startX, startY, perk, cont1, cont2);
			}
			else if(perkType==_PerkType.LifeCap){
				cont=new GUIContent(" - Increase Value:", "value used to modify the existing maximum life capacity");
				DrawValue(startX, startY, perk, cont);
			}
			else if(perkType==_PerkType.LifeRegen){
				cont=new GUIContent(" - Increase Value:", "value used to modify the existing life regeneration rate");
				DrawValue(startX, startY, perk, cont);
			}
			else if(perkType==_PerkType.LifeWaveClearedBonus){
				GUIContent cont1=new GUIContent(" - Min value:", "Minimum value");
				GUIContent cont2=new GUIContent(" - Max value:", "Maximum value");
				DrawValueMinMax(startX, startY, perk, cont1, cont2);
			}
			
			
			
			
			else if(IsPerkTypeUsesRsc(perkType)){
				if(perkType==_PerkType.GainRsc) cont=new GUIContent(" - Gain:", "The resource to be gain upon purchasing this perk");
				else if(perkType==_PerkType.RscRegen) cont=new GUIContent(" - Rate modifier:", "The resource to be gain upon purchasing this perk");
				else if(perkType==_PerkType.RscGain) cont=new GUIContent(" - Gain multiplier:", "The resource to be gain upon purchasing this perk");
				else if(perkType==_PerkType.RscCreepKilledGain) cont=new GUIContent(" - Gain multiplier:", "The resource to be gain upon purchasing this perk");
				else if(perkType==_PerkType.RscWaveClearedGain) cont=new GUIContent(" - Gain multiplier:", "The resource to be gain upon purchasing this perk");
				else if(perkType==_PerkType.RscResourceTowerGain) cont=new GUIContent(" - Gain multiplier:", "The resource to be gain upon purchasing this perk");
				
				List<Rsc> rscList=EditorDBManager.GetRscList();
				while(perk.valueRscList.Count<rscList.Count) perk.valueRscList.Add(0);
				while(perk.valueRscList.Count<rscList.Count) perk.valueRscList.RemoveAt(perk.valueRscList.Count-1);
				
				//cont=new GUIContent("Gain:", "The resource to be gain upon purchasing this perk");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				int count=0;	startY+=spaceY; 	float cachedX=startX;
				for(int i=0; i<rscList.Count; i++){
					EditorUtilities.DrawSprite(new Rect(startX+15, startY-1, 20, 20), rscList[i].icon);
					perk.valueRscList[i]=EditorGUI.FloatField(new Rect(startX+35, startY, 40, height), perk.valueRscList[i]);
					count+=1; 	startX+=75;
					if(count==3){ startY+=spaceY; startX=cachedX; }
				}
				startX=cachedX;	startY+=5;
			}
			
			
			
			
			//~ if(IsPerkTypeUsesUnitStats(perkType)){
			if(perkType==_PerkType.Tower || perkType==_PerkType.TowerSpecific){
				Vector2 v2;
				if(perkType==_PerkType.TowerSpecific){
					v2=DrawItemIDTower(startX, startY, perk, 5);	startY=v2.y;
				}
				v2=DrawTowerStat(startX, startY, perk);		startY=v2.y;
			}
			
			if(perkType==_PerkType.Ability || perkType==_PerkType.AbilitySpecific){
				if(perkType==_PerkType.AbilitySpecific){
					DrawItemIDAbility(startX, startY, perk);	startY+=spaceY;
				}
				Vector2 v2=DrawAbilityStat(startX, startY, perk);		startY=v2.y;
			}
			
			if(perkType==_PerkType.FPSWeapon || perkType==_PerkType.FPSWeaponSpecific){
				if(perkType==_PerkType.FPSWeaponSpecific){
					DrawItemIDFPSWeapon(startX, startY, perk);	startY+=spaceY;
				}
				Vector2 v2=DrawFPSWeaponStat(startX, startY, perk);		startY=v2.y;
			}
			
			
			
			else if(perkType==_PerkType.EnergyRegen){
				cont=new GUIContent(" - Increase Value:", "value used to modify the existing energy regeneration rate");
				DrawValue(startX, startY, perk, cont);
			}
			else if(perkType==_PerkType.EnergyIncreaseCap){
				cont=new GUIContent(" - Increase Value:", "value used to modify the existing maximum energy capacity");
				DrawValue(startX, startY, perk, cont);
			}
			else if(perkType==_PerkType.EnergyCreepKilledBonus){
				GUIContent cont1=new GUIContent(" - Min value:", "Minimum value");
				GUIContent cont2=new GUIContent(" - Max value:", "Maximum value");
				DrawValueMinMax(startX, startY, perk, cont1, cont2);
			}
			else if(perkType==_PerkType.EnergyWaveClearedBonus){
				GUIContent cont1=new GUIContent(" - Min value:", "Minimum value");
				GUIContent cont2=new GUIContent(" - Max value:", "Maximum value");
				DrawValueMinMax(startX, startY, perk, cont1, cont2);
			}
			
			return new Vector2(startX, startY);
		}
		
		
		private bool foldGeneralParameter=true;
		Vector2 DrawTowerStat(float startX, float startY, Perk perk){
			startY+=5;
			
			foldGeneralParameter=EditorGUI.Foldout(new Rect(startX, startY+=spaceY, width, height), foldGeneralParameter, "Show General Stats");
			
			if(foldGeneralParameter){
				startX+=15;
				
				cont=new GUIContent("HP:", "HP multiplier of the tower. Takes value from 0 and above with 0.2 being 20% increment");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				perk.HP=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.HP);
				cont=new GUIContent("HP Regen:", "HP rgeneration multiplier of the tower. Takes value from 0 and above with 0.2 being 20% increment");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				perk.HPRegen=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.HPRegen);
				cont=new GUIContent("HP Stagger:", "HP stagger duration multiplier of the tower. Takes value from 0-1 with 0.2 being 20% decrease in duration");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				perk.HPStagger=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.HPStagger);
				
				cont=new GUIContent("Shield:", "Shield multiplier of the tower. Takes value from 0 and above with 0.2 being 20% increment");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				perk.shield=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.shield);
				cont=new GUIContent("Shield Regen:", "Shield rgeneration multiplier of the tower. Takes value from 0 and above with 0.2 being 20% increment");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				perk.shieldRegen=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.shieldRegen);
				cont=new GUIContent("Shield Stagger:", "Shield stagger duration multiplier of the tower. Takes value from 0-1 with 0.2 being 20% decrease in duration");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				perk.shieldStagger=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.shieldStagger);
				
				cont=new GUIContent("Build Cost:", "Build cost multiplier of the tower. Takes value from 0-1 with 0.2 being 20% decrease in cost");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				perk.buildCost=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.buildCost);
				cont=new GUIContent("Upgrade Cost:", "Upgrade cost multiplier of the tower. Takes value from 0-1 with 0.2 being 20% decrease in cost");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				perk.upgradeCost=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.upgradeCost);
				
				startX-=15;
			}
			
			Vector2 v2=DrawUnitStat(startX, startY+5, perk.stats, false);
			startY=v2.y;
			
			return new Vector2(startX, startY);
		}
		
		Vector2 DrawAbilityStat(float startX, float startY, Perk perk){
			startY+=5;
			
			//~ if(perk.itemIDList[0]==-1) return;
			
			//~ Ability ability=null; 
			//~ List<Ability> abilityList=EditorDBManager.GetAbilityList();
			//~ for(int i=0; i<abilityList.Count; i++){ if(abilityList[i].ID==perk.itemIDList[0]) ability=abilityList[i]; }
			
			cont=new GUIContent("Cost:", "Multiplier to the ability energy cost. Takes value from 0-1 with 0.3 being decrease energy cost by 30%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.abCost=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.abCost);
			cont=new GUIContent("Cooldown:", "Multiplier to the ability cooldown duration. Takes value from 0-1 with 0.3 being decrease cooldown duration by 30%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.abCooldown=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.abCooldown);
			cont=new GUIContent("AOE Radius:", "Multiplier to the ability AOE radius. Takes value from 0 and above with 0.3 being increment of 30%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.abAOERadius=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.abAOERadius);
			
			
			startY+=5;
			
			cont=new GUIContent("Duration:", "Duration multiplier. Takes value from 0 and above with 0.3 being increase duration by 30%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.effects.duration=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.effects.duration);
			perk.effects.dot.duration=perk.effects.duration;
			perk.effects.slow.duration=perk.effects.duration;

			startY+=5;
			
			cont=new GUIContent("Damage:", "Damage multiplier. Takes value from 0 and above with 0.3 being increase existing effect damage by 30%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.effects.damageMin=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.effects.damageMin);
			
			cont=new GUIContent("Stun Chance:", "Duration modifier. Takes value from 0 and above with 0.3 being increase stun chance by 30%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.effects.stunChance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.effects.stunChance);
			
			startY+=5;
			
			cont=new GUIContent("Slow", "");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);	startY-=spaceY;
			
			cont=new GUIContent("         - Multiplier:", "Move speed multiplier. Takes value from 0-1 with with 0.3 being decrese default speed by 30%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.effects.slow.slowMultiplier=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.effects.slow.slowMultiplier);
			
			
			startY+=5;
			
			cont=new GUIContent("Dot", "Damage over time");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);	startY-=spaceY;
			
			cont=new GUIContent("        - Damage:", "Damage multiplier to DOT. Takes value from 0 and above with with 0.3 being increase the tick damage by 30%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.effects.dot.value=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.effects.dot.value);
			
			startY+=5;
			
			cont=new GUIContent("DamageBuff:", "Damage buff modifer. Takes value from 0 and above with 0.3 being increase existing damage by 30%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.effects.damageBuff=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.effects.damageBuff);
			
			cont=new GUIContent("RangeBuff:", "Range buff modifer. Takes value from 0 and above with 0.3 being increase existing range by 30%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.effects.rangeBuff=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.effects.rangeBuff);
			
			cont=new GUIContent("CDBuff:", "Cooldown buff modifer. Takes value from 0 and above with 0.3 being reduce existing cooldown by 30%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.effects.cooldownBuff=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.effects.cooldownBuff);
			
			cont=new GUIContent("HPGain:", "HP Gain multiplier. Takes value from 0 and above with 0.3 being increase existing effect HP gain value by 30%.");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.effects.HPGainMin=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.effects.HPGainMin);
			
			
			return new Vector2(startX, startY);
		}
		
		Vector2 DrawFPSWeaponStat(float startX, float startY, Perk perk){
			startY+=5;
			
			Vector2 v2=DrawUnitStat(startX, startY+5, perk.stats, true);
			//startY=v2.x+spaceY;
			
			return new Vector2(startX, v2.y);
		}
		
		
		private bool foldOffenseParameter=true;
		private bool foldSupportParameter=true;
		private bool foldRscParameter=true;
		Vector2 DrawUnitStat(float startX, float startY, UnitStat stats, bool isWeapon){
			float fWidth=40;
			
			if(!isWeapon) foldOffenseParameter=EditorGUI.Foldout(new Rect(startX, startY+=spaceY, width, height), foldOffenseParameter, "Show Offensive Stats");
			if(isWeapon || foldOffenseParameter){
				startX+=15;
				
				cont=new GUIContent("Damage:", "");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				stats.damageMin=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.damageMin);
				
				cont=new GUIContent("Cooldown:", "");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				stats.cooldown=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.cooldown);
				
				if(isWeapon){
					cont=new GUIContent("Clip Size:", "");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
					stats.clipSize=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.clipSize);
					
					cont=new GUIContent("Reload Duration:", "");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
					stats.reloadDuration=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.reloadDuration);
				}
					
				if(!isWeapon){
					cont=new GUIContent("Range:", "");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
					stats.attackRange=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.attackRange);
				}
				
				cont=new GUIContent("AOE Radius:", "");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				stats.aoeRadius=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.aoeRadius);
				
				if(!isWeapon){
					cont=new GUIContent("Hit Modifier:", "");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
					stats.hit=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.hit);
					
					cont=new GUIContent("Dodge Modifier:", "");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
					stats.dodge=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.dodge);
				}
				
				cont=new GUIContent("Stun", "");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);	startY-=spaceY;
					
					cont=new GUIContent("        - Chance:", "Chance to stun the target in each successful attack. Takes value from 0-1 with 0 being 0% and 1 being 100%");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
					stats.stun.chance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.stun.chance);
					
					cont=new GUIContent("        - Duration:", "The stun duration in second");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
					stats.stun.duration=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.stun.duration);
				
				cont=new GUIContent("Critical", "");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);	startY-=spaceY;
					
					cont=new GUIContent("            - Chance:", "Chance to score critical hit in attack. Takes value from 0-1 with 0 being 0% and 1 being 100%");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
					stats.crit.chance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.crit.chance);
					
					cont=new GUIContent("            - Multiplier:", "Damage multiplier for successful critical hit. Takes value from 0 and above with with 0.5 being 50% of normal damage as bonus");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
					stats.crit.dmgMultiplier=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.crit.dmgMultiplier);
				
				
				cont=new GUIContent("Slow", "");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);	startY-=spaceY;
					
					cont=new GUIContent("         - Duration:", "The effect duration in second");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
					stats.slow.duration=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.slow.duration);
					
					cont=new GUIContent("         - Multiplier:", "Move speed multiplier. Takes value from 0-1 with with 0.7 being decrese default speed by 30%");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
					stats.slow.slowMultiplier=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.slow.slowMultiplier);
					
					
					
				cont=new GUIContent("Dot", "Damage over time");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);	startY-=spaceY;
					
					cont=new GUIContent("        - Duration:", "The effect duration in second");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
					stats.dot.duration=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.dot.duration);
					
					cont=new GUIContent("        - Interval:", "Duration between each tick. Damage is applied at each tick.");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
					stats.dot.interval=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.dot.interval);
					
					cont=new GUIContent("        - Damage:", "Damage applied at each tick");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
					stats.dot.value=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.dot.value);
					
					
					
				cont=new GUIContent("InstantKill", "");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);	startY-=spaceY;
					
					cont=new GUIContent("               - Chance:", "The chance to instant kill the target. Takes value from 0-1 with 0 being 0% and 1 being 100%");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
					stats.instantKill.chance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.instantKill.chance);
					
					cont=new GUIContent("       - HP Threshold:", "The HP threshold of the target in order for the instantKill to become valid. Take value from 0-1 with 0.3 being 30% of the fullHP.");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
					stats.instantKill.HPThreshold=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.instantKill.HPThreshold);
					
					
					cont=new GUIContent("Shield Break:", "The chance of the unit's attack to damage target's shield and disable shield regen permenantly\nTakes value from 0-1 with 0 being 0% and 1 being 100%");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
					stats.shieldBreak=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.shieldBreak);
					
					cont=new GUIContent("Shield Pierce:", "The chance of the unit's attack to bypass target's shield and damage HP directly\nTakes value from 0-1 with 0 being 0% and 1 being 100%");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
					stats.shieldPierce=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.shieldPierce);
					
				startX-=15;
			}
			
			if(!isWeapon) foldSupportParameter=EditorGUI.Foldout(new Rect(startX, startY+=spaceY, width, height), foldSupportParameter, "Show Support Stats");
			if(!isWeapon && foldSupportParameter){
				startX+=15;
				
				cont=new GUIContent("Buff:", "Note: Buffs from multple tower doesnt stack, however when there's difference in the buff strength, the stronger buff applies. A tower can gain maximum dmage buff from one source and maximum range buff from another");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);	startY-=spaceY;
				
				cont=new GUIContent("        - Damage:", "Damage buff multiplier. Takes value from 0 and above with 0.5 being 50% increase in damage");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				stats.buff.damageBuff=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.buff.damageBuff);
				
				cont=new GUIContent("        - Cooldown:", "Dooldown buff multiplier. Takes value from 0-1 with 0.2 being reduce cooldown by 20%");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				stats.buff.cooldownBuff=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.buff.cooldownBuff);
				
				cont=new GUIContent("        - Range:", "Range buff multiplier. Takes value from 0 and above with 0.5 being 50% increase in range");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				stats.buff.rangeBuff=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.buff.rangeBuff);
				
				cont=new GUIContent("        - Critical:", "Critical hit chance buff modifier. Takes value from 0 and above with 0.25 being 25% increase in critical hit chance");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				stats.buff.criticalBuff=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.buff.criticalBuff);
				
				cont=new GUIContent("        - Hit:", "Hit chance buff modifier. Takes value from 0 and above with .2 being 20% increase in hit chance");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				stats.buff.hitBuff=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.buff.hitBuff);
				
				cont=new GUIContent("        - Dodge:", "Dodge chance buff modifier. Takes value from 0 and above with 0.15 being 15% increase in dodge chance");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				stats.buff.dodgeBuff=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.buff.dodgeBuff);
				
				cont=new GUIContent("        - HP Regen:", "HP Regeneration Buff. Takes value from 0 and above with 2 being gain 2HP second ");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				stats.buff.regenHP=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stats.buff.regenHP);
				
				startX-=15;
			}
			
			
			if(!isWeapon) foldRscParameter=EditorGUI.Foldout(new Rect(startX, startY+=spaceY, width, height), foldRscParameter, "Show RscGain");
			if(!isWeapon && foldRscParameter){
				startX+=15;
				
				List<Rsc> rscList=EditorDBManager.GetRscList();
				if(stats.rscGain.Count!=rscList.Count){
					while(stats.rscGain.Count>rscList.Count) stats.rscGain.RemoveAt(stats.rscGain.Count-1);
					while(stats.rscGain.Count<rscList.Count) stats.rscGain.Add(0);
				}
				cont=new GUIContent("Resource Gain:", "The resource gain by unit at each cooldown interval\nOnly applicable to ResourceTower");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				int count=0;	startY+=spaceY; 	float cachedX=startX;
				for(int i=0; i<rscList.Count; i++){
					EditorUtilities.DrawSprite(new Rect(startX+10, startY-1, 20, 20), rscList[i].icon);
					stats.rscGain[i]=EditorGUI.IntField(new Rect(startX+30, startY, fWidth, height), stats.rscGain[i]);
					count+=1; 	startX+=65;
					if(count==3){ startY+=spaceY; startX=cachedX; }
				}
				
				startX-=15;
			}
			
			return new Vector2(startX, startY);
		}
		
		
		bool IsPerkTypeUsesRsc(_PerkType type){
			if(type==_PerkType.GainRsc) return true;
			else if(type==_PerkType.RscRegen) return true;
			else if(type==_PerkType.RscGain) return true;
			else if(type==_PerkType.RscCreepKilledGain) return true;
			else if(type==_PerkType.RscWaveClearedGain) return true;
			else if(type==_PerkType.RscResourceTowerGain) return true;
			return false;
		}
		
		bool IsPerkTypeUsesUnitStats(_PerkType type){
			if(type==_PerkType.Tower) return true;
			else if(type==_PerkType.TowerSpecific) return true;
			else if(type==_PerkType.Ability) return true;
			else if(type==_PerkType.AbilitySpecific) return true;
			else if(type==_PerkType.FPSWeapon) return true;
			else if(type==_PerkType.FPSWeaponSpecific) return true;
			return false;
		}
		
		
		
		
		
		
		
		
		private Rect listVisibleRect;
		private Rect listContentRect;
		
		private int deleteID=-1;
		private bool minimiseList=false;
		Vector2 DrawPerkList(float startX, float startY, List<Perk> perkList){
			
			float width=260;
			if(minimiseList) width=60;
			
			
			if(!minimiseList){
				if(GUI.Button(new Rect(startX+180, startY-20, 40, 18), "up")){
					if(selectID>0){
						Perk perk=perkList[selectID];
						perkList[selectID]=perkList[selectID-1];
						perkList[selectID-1]=perk;
						selectID-=1;
						
						if(selectID*35<scrollPos1.y) scrollPos1.y=selectID*35;
					}
				}
				if(GUI.Button(new Rect(startX+222, startY-20, 40, 18), "down")){
					if(selectID<perkList.Count-1){
						Perk perk=perkList[selectID];
						perkList[selectID]=perkList[selectID+1];
						perkList[selectID+1]=perk;
						selectID+=1;
						
						if(listVisibleRect.height-35<selectID*35) scrollPos1.y=(selectID+1)*35-listVisibleRect.height+5;
					}
				}
			}
			
			
			listVisibleRect=new Rect(startX, startY, width+15, window.position.height-startY-5);
			listContentRect=new Rect(startX, startY, width, perkList.Count*35+5);
			
			GUI.color=new Color(.8f, .8f, .8f, 1f);
			GUI.Box(listVisibleRect, "");
			GUI.color=Color.white;
			
			scrollPos1 = GUI.BeginScrollView(listVisibleRect, scrollPos1, listContentRect);
			
			
				startY+=5;	startX+=5;
			
				for(int i=0; i<perkList.Count; i++){
					
					EditorUtilities.DrawSprite(new Rect(startX, startY+(i*35), 30, 30), perkList[i].icon);
					
					if(minimiseList){
						if(selectID==i) GUI.color = new Color(0, 1f, 1f, 1f);
						if(GUI.Button(new Rect(startX+35, startY+(i*35), 30, 30), "")) SelectPerk(i);
						GUI.color = Color.white;
						
						continue;
					}
					
					
					
					if(selectID==i) GUI.color = new Color(0, 1f, 1f, 1f);
					if(GUI.Button(new Rect(startX+35, startY+(i*35), 150, 30), perkList[i].name)) SelectPerk(i);
					GUI.color = Color.white;
					
					if(deleteID==i){
						
						if(GUI.Button(new Rect(startX+190, startY+(i*35), 60, 15), "cancel")) deleteID=-1;
						
						GUI.color = Color.red;
						if(GUI.Button(new Rect(startX+190, startY+(i*35)+15, 60, 15), "confirm")){
							if(selectID>=deleteID) SelectPerk(Mathf.Max(0, selectID-1));
							perkList.RemoveAt(deleteID);
							deleteID=-1;
						}
						GUI.color = Color.white;
					}
					else{
						if(GUI.Button(new Rect(startX+190, startY+(i*35), 60, 15), "remove")) deleteID=i;
					}
				}
			
			GUI.EndScrollView();
			
			return new Vector2(startX+width, startY);
		}
		
	}

}