using UnitedSolution;using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	public class DemoMenu : MonoBehaviour {
		
		public RectTransform frame;
		
		public List<string> displayedName=new List<string>();
		public List<string> levelName=new List<string>();
		public List<UnityButton> buttonList=new List<UnityButton>();
		
		// Use this for initialization
		void Start (){
			for(int i=0; i<levelName.Count; i++){
				if(i==0) buttonList[0].Init();
				else if(i>0){
					buttonList.Add(buttonList[0].Clone("ButtonStart"+(i+1), new Vector3(0, -i*40, 0)));
				}
				
				buttonList[i].label.text=displayedName[i];
			}
			
			frame.sizeDelta=new Vector2(200, 30+levelName.Count*40);
		}
		
		// Update is called once per frame
		void Update () {
		
		}
		
		public void OnStartButton(GameObject butObj){
			for(int i=0; i<buttonList.Count; i++){
				if(buttonList[i].rootObj==butObj){
					Application.LoadLevel(levelName[i]);
				}
			}
		}
		
	}

}