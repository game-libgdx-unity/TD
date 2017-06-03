using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	public class DamageArmorDBEditor : EditorWindow {

		private static DamageArmorDBEditor window;
		
	   public static void Init () {
			// Get existing open window or if none, make a new one:
			window = (DamageArmorDBEditor)EditorWindow.GetWindow(typeof (DamageArmorDBEditor));
			//~ window.minSize=new Vector2(340, 170);
			window.minSize=new Vector2(470, 300);
			//~ window.maxSize=new Vector2(471, 301);
			
			EditorDBManager.Init();
			
		}
		
		
		
		Vector2 scrollPos;

		private enum _Tab{Armor, Damage}
		private _Tab tab=_Tab.Armor;
		private int selectedID;
		bool delete=false;

		void OnGUI(){
			if(window==null) Init();
			
			
			int startX=0;
			int startY=0;
			
			if(GUI.Button(new Rect(10, 10, 100, 30), "New Armor")) EditorDBManager.AddNewArmorType();
			if(GUI.Button(new Rect(120, 10, 100, 30), "New Damage")) EditorDBManager.AddNewDamageType();
			
			
			List<DamageType> damageTypeList=EditorDBManager.GetDamageTypeList();
			List<ArmorType> armorTypeList=EditorDBManager.GetArmorTypeList();
			
			
			Rect visibleRect=new Rect(10, 50, window.position.width-20, 185);
			Rect contentRect=new Rect(10, 50, 118+damageTypeList.Count*105, 5+(armorTypeList.Count+1)*25);
			
			GUI.Box(visibleRect, "");
			scrollPos = GUI.BeginScrollView(visibleRect, scrollPos, contentRect);
			
				startY=60;
				startX=20;
				for(int i=0; i<damageTypeList.Count; i++){
					DamageType dmgType=damageTypeList[i];
					if(selectedID==i && tab==_Tab.Damage) GUI.color=new Color(0, 1, 1, 1);
					if(GUI.Button(new Rect(startX+=105, startY, 100, 20), dmgType.name)){
						selectedID=i; 	tab=_Tab.Damage;
						delete=false;
						GUI.FocusControl ("");
					}
					GUI.color=Color.white;
				}
				
				
				
				startY=60;
				for(int i=0; i<armorTypeList.Count; i++){
					startX=20;
					
					ArmorType armorType=armorTypeList[i];
					if(selectedID==i && tab==_Tab.Armor) GUI.color=new Color(0, 1, 1, 1);
					if(GUI.Button(new Rect(startX, startY+=25, 100, 20), armorType.name)){
						selectedID=i; 	tab=_Tab.Armor;
						delete=false;
						GUI.FocusControl ("");
					}
					GUI.color=Color.white;
					
					if(armorType.modifiers.Count!=damageTypeList.Count){
						while(armorType.modifiers.Count<damageTypeList.Count) armorType.modifiers.Add(1);
						while(armorType.modifiers.Count>damageTypeList.Count) armorType.modifiers.RemoveAt(armorType.modifiers.Count-1);
						EditorDBManager.SetDirtyDamageArmor();
					}
					
					startX+=110;
					for(int j=0; j<damageTypeList.Count; j++){
						armorType.modifiers[j]=EditorGUI.FloatField(new Rect(startX, startY, 90, 20), armorType.modifiers[j]);
						startX+=105;
					}
				}
				
			
			
			GUI.EndScrollView();
			
			
			
			
			
			startX=10;
			startY=250;
			
			
			
			if(selectedID>=0){
				DAType daInstance=null;
				if(tab==_Tab.Damage){
					selectedID=Mathf.Min(selectedID, damageTypeList.Count-1);
					daInstance=damageTypeList[selectedID];
				}
				if(tab==_Tab.Armor){
					selectedID=Mathf.Min(selectedID, armorTypeList.Count-1);
					daInstance=armorTypeList[selectedID];
				}
			
				GUI.Label(new Rect(startX, startY, 200, 17), "Name:");
				daInstance.name=EditorGUI.TextField(new Rect(startX+80, startY, 150, 17), daInstance.name);
				
				
				GUIStyle styleL=new GUIStyle(GUI.skin.textArea);
						styleL.wordWrap=true;
						styleL.clipping=TextClipping.Clip;
						styleL.alignment=TextAnchor.UpperLeft;
				EditorGUI.LabelField(new Rect(startX, startY+=25, 150, 17), "Description: ");
				daInstance.desp=EditorGUI.TextArea(new Rect(startX, startY+=17, window.position.width-20, 50), daInstance.desp, styleL);
				
				
				string label="";
				if(tab==_Tab.Damage) {
					for(int i=0; i<armorTypeList.Count; i++){
						label+=" - cause "+(armorTypeList[i].modifiers[selectedID]*100).ToString("f0")+"% damage to "+armorTypeList[i].name+"\n";
					}
				}
				if(tab==_Tab.Armor){
					for(int i=0; i<damageTypeList.Count; i++){
						label+=" - take "+(armorTypeList[selectedID].modifiers[i]*100).ToString("f0")+"% damage from "+damageTypeList[i].name+"\n";
					}
				}
				GUI.Label(new Rect(startX, startY+=60, window.position.width-20, 100), label);
				
				
				startX=300;
				startY=250;
				if(!delete){
					if(GUI.Button(new Rect(startX, startY, 80, 20), "delete")) delete=true;
				}
				else if(delete){
					if(GUI.Button(new Rect(startX, startY, 80, 20), "cancel")) delete=false;
					GUI.color=Color.red;
					if(GUI.Button(new Rect(startX+=90, startY, 80, 20), "confirm")){
						if(tab==_Tab.Damage) EditorDBManager.RemoveDamageType(selectedID);
						if(tab==_Tab.Armor) EditorDBManager.RemoveArmorType(selectedID);
						selectedID=-1;
					}
					GUI.color=Color.white;
				}
			}
			
			
			
			if(GUI.changed) EditorDBManager.SetDirtyDamageArmor();
			
		}

	}


}