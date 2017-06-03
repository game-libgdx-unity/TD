//using UnityEngine;
//using UnityEditor;

//using System;

//using System.Collections;
//using System.Collections.Generic;

//using UnitedSolution;

//namespace UnitedSolution {

//	public class FPSWeaponEditorWindow : UnitEditorWindow {
		
//		private static FPSWeaponEditorWindow window;
		
		

//		public static void Init () {
//			// Get existing open window or if none, make a new one:
//			window = (FPSWeaponEditorWindow)EditorWindow.GetWindow(typeof (FPSWeaponEditorWindow));
//			//~ window.minSize=new Vector2(375, 449);
//			//~ window.maxSize=new Vector2(375, 800);
			
//			EditorDBManager.Init();
			
//			UpdateObjectHierarchyList();
//		}
		
		
		
		
//		private static List<GameObject> objHList=new List<GameObject>();
//		private static string[] objHLabelList=new string[0];
//		private static void UpdateObjectHierarchyList(){
//			List<FPSWeapon> weaponList=EditorDBManager.GetFPSWeaponList();
//			if(weaponList.Count==0 || selectID>=weaponList.Count) return;
//			EditorUtilities.GetObjectHierarchyList(weaponList[selectID].gameObject, SetObjListCallback);
//		}
//		public static void SetObjListCallback(List<GameObject> objList, string[] labelList){
//			objHList=objList;
//			objHLabelList=labelList;
//		}
		
		
		
//		void SelectWeapon(int ID){
//			selectID=ID;
//			UpdateObjectHierarchyList();
//			GUI.FocusControl ("");
			
//			if(selectID*35<scrollPos1.y) scrollPos1.y=selectID*35;
//			if(selectID*35>scrollPos1.y+listVisibleRect.height-40) scrollPos1.y=selectID*35-listVisibleRect.height+40;
//		}
		
		
		
		
		
//		private Vector2 scrollPos1;
//		private Vector2 scrollPos2;
		
//		private static int selectID=0;
//		private float contentHeight=0;
//		private float contentWidth=0;
		
		
//		void OnGUI () {
//			if(window==null) Init();
			
//			List<FPSWeapon> weaponList=EditorDBManager.GetFPSWeaponList();
			
//			if(GUI.Button(new Rect(window.position.width-120, 5, 100, 25), "Save")) EditorDBManager.SetDirtyFPSWeapon();
			
//			EditorGUI.LabelField(new Rect(5, 7, 200, 17), "Add new weapon:");
//			FPSWeapon newWeapon=null;
//			newWeapon=(FPSWeapon)EditorGUI.ObjectField(new Rect(115, 7, 140, 17), newWeapon, typeof(FPSWeapon), false);
//			if(newWeapon!=null){
//				int newSelectID=EditorDBManager.AddNewFPSWeapon(newWeapon);
//				if(newSelectID!=-1) SelectWeapon(newSelectID);
//			}
			
			
//			float startX=5;
//			float startY=50;
			
//			if(minimiseList){
//				if(GUI.Button(new Rect(startX, startY-20, 30, 18), ">>")) minimiseList=false;
//			}
//			else{
//				if(GUI.Button(new Rect(startX, startY-20, 30, 18), "<<")) minimiseList=true;
//			}
//			Vector2 v2=DrawWeaponList(startX, startY, weaponList);
			
//			startX=v2.x+25;
			
//			if(weaponList.Count==0) return;
			
//			cont=new GUIContent("Weapon Prefab:", "The prefab object of the weapon\nClick this to highlight it in the ProjectTab");
//			EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
//			EditorGUI.ObjectField(new Rect(startX+100, startY, 175, height), weaponList[selectID].gameObject, typeof(GameObject), false);
//			startY+=spaceY+10;
			
			
//			Rect visibleRect=new Rect(startX, startY, window.position.width-startX-10, window.position.height-startY-5);
//			Rect contentRect=new Rect(startX, startY, contentWidth-startY, contentHeight);
			
//			scrollPos2 = GUI.BeginScrollView(visibleRect, scrollPos2, contentRect);
			
//				v2=DrawWeaponConfigurator(startX, startY, weaponList[selectID]);
//				contentWidth=v2.x;
//				contentHeight=v2.y;
			
//			GUI.EndScrollView();
			
			
//			if(GUI.changed) EditorDBManager.SetDirtyFPSWeapon();
//		}
		
		

		
		
		
//		private Rect listVisibleRect;
//		private Rect listContentRect;
		
//		private int deleteID=-1;
//		private bool minimiseList=false;
//		Vector2 DrawWeaponList(float startX, float startY, List<FPSWeapon> weaponList){
			
//			float width=260;
//			if(minimiseList) width=60;
			
			
//			if(!minimiseList){
//				if(GUI.Button(new Rect(startX+180, startY-20, 40, 18), "up")){
//					if(selectID>0){
//						FPSWeapon weapon=weaponList[selectID];
//						weaponList[selectID]=weaponList[selectID-1];
//						weaponList[selectID-1]=weapon;
//						selectID-=1;
						
//						if(selectID*35<scrollPos1.y) scrollPos1.y=selectID*35;
//					}
//				}
//				if(GUI.Button(new Rect(startX+222, startY-20, 40, 18), "down")){
//					if(selectID<weaponList.Count-1){
//						FPSWeapon weapon=weaponList[selectID];
//						weaponList[selectID]=weaponList[selectID+1];
//						weaponList[selectID+1]=weapon;
//						selectID+=1;
						
//						if(listVisibleRect.height-35<selectID*35) scrollPos1.y=(selectID+1)*35-listVisibleRect.height+5;
//					}
//				}
//			}
			
			
//			listVisibleRect=new Rect(startX, startY, width+15, window.position.height-startY-5);
//			listContentRect=new Rect(startX, startY, width, weaponList.Count*35+5);
			
//			GUI.color=new Color(.8f, .8f, .8f, 1f);
//			GUI.Box(listVisibleRect, "");
//			GUI.color=Color.white;
			
//			scrollPos1 = GUI.BeginScrollView(listVisibleRect, scrollPos1, listContentRect);
			
			
//				startY+=5;	startX+=5;
			
//				for(int i=0; i<weaponList.Count; i++){
					
//					EditorUtilities.DrawSprite(new Rect(startX, startY+(i*35), 30, 30), weaponList[i].icon);
					
//					if(minimiseList){
//						if(selectID==i) GUI.color = new Color(0, 1f, 1f, 1f);
//						if(GUI.Button(new Rect(startX+35, startY+(i*35), 30, 30), "")) SelectWeapon(i);
//						GUI.color = Color.white;
						
//						continue;
//					}
					
					
					
//					if(selectID==i) GUI.color = new Color(0, 1f, 1f, 1f);
//					if(GUI.Button(new Rect(startX+35, startY+(i*35), 150, 30), weaponList[i].weaponName)) SelectWeapon(i);
//					GUI.color = Color.white;
					
//					if(deleteID==i){
						
//						if(GUI.Button(new Rect(startX+190, startY+(i*35), 60, 15), "cancel")) deleteID=-1;
						
//						GUI.color = Color.red;
//						if(GUI.Button(new Rect(startX+190, startY+(i*35)+15, 60, 15), "confirm")){
//							if(selectID>=deleteID) SelectWeapon(Mathf.Max(0, selectID-1));
//							EditorDBManager.RemoveWeapon(deleteID);
//							deleteID=-1;
//						}
//						GUI.color = Color.white;
//					}
//					else{
//						if(GUI.Button(new Rect(startX+190, startY+(i*35), 60, 15), "remove")) deleteID=i;
//					}
//				}
			
//			GUI.EndScrollView();
			
//			return new Vector2(startX+width, startY);
//		}
		
		
		
		
//		Vector3 v3=Vector3.zero;
		
//		Vector2 DrawWeaponConfigurator(float startX, float startY, FPSWeapon weapon){
//			float maxWidth=0;
			
//			float cachedY=startY;
//			float cachedX=startX;
			
			
//			EditorUtilities.DrawSprite(new Rect(startX, startY, 60, 60), weapon.icon);
//			startX+=65;
			
//			cont=new GUIContent("Name:", "The unit name to be displayed in game");
//			EditorGUI.LabelField(new Rect(startX, startY+=spaceY-8, width, height), cont);
//			weapon.weaponName=EditorGUI.TextField(new Rect(startX+spaceX-65, startY, width-5, height), weapon.weaponName);
			
//			cont=new GUIContent("Icon:", "The unit icon to be displayed in game, must be a sprite");
//			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//			weapon.icon=(Sprite)EditorGUI.ObjectField(new Rect(startX+spaceX-65, startY, width-5, height), weapon.icon, typeof(Sprite), false);
			
			
//			startX=cachedX;
//			startY+=spaceY+8;
			
			
//			string[] damageTypeLabel=EditorDBManager.GetDamageTypeLabel();
//			cont=new GUIContent("Damage Type:", "The damage type of the weapon\nDamage type can be configured in Damage Armor Table Editor");
//			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//			weapon.damageType=EditorGUI.Popup(new Rect(startX+spaceX, startY, width, height), weapon.damageType, damageTypeLabel);
			
//			cont=new GUIContent("Recoil:", "The recoil force of the weapon");
//			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//			weapon.recoil=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), weapon.recoil);
			
//			cont=new GUIContent("ShootPoint:", "The transform which indicate the position where the shootObject will be fired from (Optional)\nEach shootPoint assigned will fire a shootObject instance in each attack\nIf left empty, the unit transform itself will be use as the shootPoint\nThe orientation of the shootPoint matter as they dictate the orientation of the shootObject starting orientation.\n");
//			shootPointFoldout=EditorGUI.Foldout(new Rect(startX, startY+=spaceY, spaceX, height), shootPointFoldout, cont);
//			int shootPointCount=weapon.shootPoints.Count;
//			shootPointCount=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), shootPointCount);
			
//			if(shootPointCount!=weapon.shootPoints.Count){
//				while(weapon.shootPoints.Count<shootPointCount) weapon.shootPoints.Add(null);
//				while(weapon.shootPoints.Count>shootPointCount) weapon.shootPoints.RemoveAt(weapon.shootPoints.Count-1);
//			}
				
//			if(shootPointFoldout){
//				for(int i=0; i<weapon.shootPoints.Count; i++){
//					int objID=GetObjectIDFromHList(weapon.shootPoints[i], objHList);
//					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), "    - Element "+(i+1));
//					objID = EditorGUI.Popup(new Rect(startX+spaceX, startY, width, height), objID, objHLabelList);
//					weapon.shootPoints[i] = (objHList[objID]==null) ? null : objHList[objID].transform;
//				}
//			}
			
			
//			startX=cachedX;
//			startY+=10;
			
//			GUIStyle style=new GUIStyle("TextArea");
//			style.wordWrap=true;
//			cont=new GUIContent("Weapon description: ", "");
//			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, 400, 20), cont);
//			weapon.desp=EditorGUI.TextArea(new Rect(startX, startY+spaceY-3, 280, 200), weapon.desp, style);
			
			
			
//			startX+=330;
//			startY=cachedY;
			
//			if(weapon.stats.Count==0) weapon.stats.Add(new UnitStat());
//			for(int i=0; i<weapon.stats.Count; i++){
//				v3=DrawWeaponStat(weapon.stats[i], startX, startY, statContentHeight);
//				statContentHeight=v3.z;
//				startX=v3.x+10;
//				if(startX>maxWidth) maxWidth=startX;
//			}
//			startY=v3.y+spaceY-cachedY;
			
			
			
//			startX=maxWidth-cachedX+80;
			
			
//			return new Vector2(startX, startY);
//		}
		
		
//		private float statContentHeight=0;
		
		
		
		
		
		
		
//		public static Vector3 DrawWeaponStat(UnitStat stat, float startX, float startY, float statContentHeight){
			
//			float width=150;
//			float fWidth=35;
//			float spaceX=130;
//			float height=18;
//			float spaceY=height+2;
			
//			//startY-=spaceY;
			
//			GUI.Box(new Rect(startX, startY, 220, statContentHeight-startY), "");
			
//			startX+=10;	startY+=10;
			
			
			
//				cont=new GUIContent("ShootObject:", "The shootObject used by the unit.\nUnit that intended to shoot at the target will not function correctly if this is left unassigned.");
//				EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
//				stat.ShootObject=(ShootObject)EditorGUI.ObjectField(new Rect(startX+spaceX-50, startY, 4*fWidth-20, height), stat.ShootObject, typeof(ShootObject), false);
//				startY+=10;
				
			
//				cont=new GUIContent("Damage(Min/Max):", "");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.damageMin=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.damageMin);
//				stat.damageMax=EditorGUI.FloatField(new Rect(startX+spaceX+fWidth, startY, fWidth, height), stat.damageMax);
				
//				cont=new GUIContent("Cooldown:", "Duration between each attack");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.cooldown=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.cooldown);
				
				
//				cont=new GUIContent("Clip Size:", "The amount of attack the unit can do before the unit needs to reload\nWhen set to -1 the unit will never need any reload");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.clipSize=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.clipSize);
//				stat.clipSize=Mathf.Round(stat.clipSize);
				
//				cont=new GUIContent("Reload Duration:", "");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.reloadDuration=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.reloadDuration);
				
				
//				startY+=10;
				
				
//				cont=new GUIContent("AOE Radius:", "Area-of-Effective radius. When the shootObject hits it's target, any other hostile unit within the area from the impact position will suffer the same target as the target.\nSet value to >0 to enable. ");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.aoeRadius=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.aoeRadius);
				
				
				
//				cont=new GUIContent("Stun", "");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);	startY-=spaceY;
				
//				cont=new GUIContent("        - Chance:", "Chance to stun the target in each successful attack. Takes value from 0-1 with 0 being 0% and 1 being 100%");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.stun.chance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.stun.chance);
				
//				cont=new GUIContent("        - Duration:", "The stun duration in second");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.stun.duration=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.stun.duration);
				
				
				
//				cont=new GUIContent("Critical", "");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);	startY-=spaceY;
				
//				cont=new GUIContent("            - Chance:", "Chance to score critical hit in attack. Takes value from 0-1 with 0 being 0% and 1 being 100%");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.crit.chance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.crit.chance);
				
//				cont=new GUIContent("            - Multiplier:", "Damage multiplier for successful critical hit. Takes value from 0 and above with with 0.5 being 50% of normal damage as bonus");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.crit.dmgMultiplier=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.crit.dmgMultiplier);
				
				
				
//				cont=new GUIContent("Slow", "");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);	startY-=spaceY;
				
//				cont=new GUIContent("         - Duration:", "The effect duration in second");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.slow.duration=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.slow.duration);
				
//				cont=new GUIContent("         - Multiplier:", "Move speed multiplier. Takes value from 0-1 with with 0.7 being decrese default speed by 30%");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.slow.slowMultiplier=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.slow.slowMultiplier);
				
				
				
//				cont=new GUIContent("Dot", "Damage over time");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);	startY-=spaceY;
				
//				cont=new GUIContent("        - Duration:", "The effect duration in second");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.dot.duration=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.dot.duration);
				
//				cont=new GUIContent("        - Interval:", "Duration between each tick. Damage is applied at each tick.");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.dot.interval=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.dot.interval);
				
//				cont=new GUIContent("        - Damage:", "Damage applied at each tick");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.dot.value=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.dot.value);
				
				
				
//				cont=new GUIContent("InstantKill", "");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);	startY-=spaceY;
				
//				cont=new GUIContent("                - Chance:", "The chance to instant kill the target. Takes value from 0-1 with 0 being 0% and 1 being 100%");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.instantKill.chance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.instantKill.chance);
				
//				cont=new GUIContent("        - HP Threshold:", "The HP threshold of the target in order for the instantKill to become valid. Take value from 0-1 with 0.3 being 30% of the fullHP.");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.instantKill.HPThreshold=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.instantKill.HPThreshold);
				
				
//				startY+=10;
				
				
//				cont=new GUIContent("Damage Shield Only:", "When checked, unit will only inflict shield damage");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.damageShieldOnly=EditorGUI.Toggle(new Rect(startX+spaceX, startY, fWidth, height), stat.damageShieldOnly);
				
//				cont=new GUIContent("Shield Break:", "The chance of the unit's attack to damage target's shield and disable shield regen permenantly\nTakes value from 0-1 with 0 being 0% and 1 being 100%");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.shieldBreak=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.shieldBreak);
				
//				cont=new GUIContent("Shield Pierce:", "The chance of the unit's attack to bypass target's shield and damage HP directly\nTakes value from 0-1 with 0 being 0% and 1 being 100%");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.shieldPierce=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.shieldPierce);
			
			
			
//			/*
//				cont=new GUIContent("Buff:", "Note: Buffs from multple tower doesnt stack, however when there's difference in the buff strength, the stronger buff applies. A tower can gain maximum dmage buff from one source and maximum range buff from another");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);	startY-=spaceY;
				
//				cont=new GUIContent("        - Damage:", "Damage buff multiplier. Takes value from 0 and above with 0.5 being 50% increase in damage");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.buff.damageBuff=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.buff.damageBuff);
				
//				cont=new GUIContent("        - Cooldown:", "Dooldown buff multiplier. Takes value from 0-1 with 0.2 being reduce cooldown by 20%");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.buff.cooldownBuff=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.buff.cooldownBuff);
				
//				cont=new GUIContent("        - Range:", "Range buff multiplier. Takes value from 0 and above with 0.5 being 50% increase in range");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.buff.rangeBuff=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.buff.rangeBuff);
				
//				cont=new GUIContent("        - Critical:", "Critical hit chance buff modifier. Takes value from 0 and above with 0.25 being 25% increase in critical hit chance");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.buff.criticalBuff=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.buff.criticalBuff);
				
//				cont=new GUIContent("        - Hit:", "Hit chance buff modifier. Takes value from 0 and above with .2 being 20% increase in hit chance");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.buff.hitBuff=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.buff.hitBuff);
				
//				cont=new GUIContent("        - Dodge:", "Dodge chance buff modifier. Takes value from 0 and above with 0.15 being 15% increase in dodge chance");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.buff.dodgeBuff=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.buff.dodgeBuff);
				
//				cont=new GUIContent("        - HP Regen:", "HP Regeneration Buff. Takes value from 0 and above with 2 being gain 2HP second ");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
//				stat.buff.regenHP=EditorGUI.FloatField(new Rect(startX+spaceX, startY, fWidth, height), stat.buff.regenHP);
//			*/
			
			
			
//				startY+=10;
//				GUIStyle style=new GUIStyle("TextArea");
//				style.wordWrap=true;
//				cont=new GUIContent("Description(to be used in runtime): ", "");
//				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, 200, 20), cont);
//				stat.desp=EditorGUI.TextArea(new Rect(startX, startY+spaceY-3, 200, 90), stat.desp, style);
				
//				startY+=90;
			
			
//			statContentHeight=startY+spaceY+5;
			
//			return new Vector3(startX+220, startY, statContentHeight);
//		}

		
//	}

//}