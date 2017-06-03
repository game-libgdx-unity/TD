using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	public class ResourceDBEditor : EditorWindow {
		
		static private ResourceDBEditor window;
		
		public static void Init () {
			// Get existing open window or if none, make a new one:
			window = (ResourceDBEditor)EditorWindow.GetWindow(typeof (ResourceDBEditor));
			window.minSize=new Vector2(355, 455);
			//~ window.maxSize=window.minSize;
			
			EditorDBManager.Init();
		}
		
		int delete=-1;
		private Vector2 scrollPos;
		
		void OnGUI () {
			if(window==null) Init();
			
			if(GUI.Button(new Rect(window.position.width-110, 10, 100, 30), "Save")) EditorDBManager.SetDirtyRsc();
			
			if(GUI.Button(new Rect(10, 10, 100, 30), "New Resource")) EditorDBManager.AddNewRsc();
			
			List<Rsc> rscList=EditorDBManager.GetRscList();
			
			if(rscList.Count>0){
				GUI.Box(new Rect(5, 50, 50, 20), "ID");
				GUI.Box(new Rect(5+50-1, 50, 70+1, 20), "Texture");
				GUI.Box(new Rect(5+120-1, 50, 150+2, 20), "Name");
				GUI.Box(new Rect(5+270, 50, window.position.width-280, 20), "");
			}
			
			int row=0;
			for(int i=0; i<rscList.Count; i++){
				if(i%2==0) GUI.color=new Color(.8f, .8f, .8f, 1);
				else GUI.color=Color.white;
				GUI.Box(new Rect(5, 75+i*49, window.position.width-10, 50), "");
				GUI.color=Color.white;
				
				GUI.Label(new Rect(22, 15+75+i*49, 50, 20), rscList[i].ID.ToString());
				
				
				EditorUtilities.DrawSprite(new Rect(12+50, 3+75+i*49, 44, 44), rscList[i].icon, true);
				
				
				//rscList[i].name=EditorGUI.TextField(new Rect(5+120, 15+75+i*49, 150, 20), rscList[i].name);
				rscList[i].name=EditorGUI.TextField(new Rect(5+120, 5+75+i*49, 150, 18), rscList[i].name);
				GUI.Label(new Rect(5+120, 25+75+i*49, 120, 18), "Icon: ");
				rscList[i].icon=(Sprite)EditorGUI.ObjectField(new Rect(45+120, 25+75+i*49, 110, 18), rscList[i].icon, typeof(Sprite), false);
				
				if(delete!=i){
					if(GUI.Button(new Rect(window.position.width-35, 12+75+i*49, 25, 25), "X")){
						delete=i;
					}
				}
				else{
					GUI.color = Color.red;
					if(GUI.Button(new Rect(window.position.width-65, 12+75+i*49, 25, 25), "X")){
						EditorDBManager.RemoveRsc(i);
						delete=-1;
					}
					GUI.color = Color.green;
					if(GUI.Button(new Rect(window.position.width-35, 12+75+i*49, 25, 25), "-")){
						delete=-1;
					}
					GUI.color = Color.white;
				}
				
				row+=1;
			}
			
			
			if(GUI.changed) EditorDBManager.SetDirtyRsc();
		}
		
		private int currentSwapID=-1;
		void SwapResource(int ID){
			List<Rsc> rscList=EditorDBManager.GetRscList();
			
			Rsc rsc=rscList[currentSwapID];
			rscList[currentSwapID]=rscList[ID];
			rscList[ID]=rsc;
			
			currentSwapID=-1;
			
			EditorDBManager.SetDirtyRsc();
		}
	}

}