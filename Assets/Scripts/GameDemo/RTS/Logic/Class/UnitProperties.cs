using UnitedSolution;using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnitedSolution {

	//Use in BuildManager, the status return when CheckBuildPoint() is called
	public enum _TileStatus{
		NoPlatform, 	//no platform at detected
		Available, 		//there's a valid build point
		Unavailable, 	//the build point is invalid (occupied)
		Blocked			//building on the spot will block the only available path
	}
	
	//Use in BuildManager, contain all the infomation of the specific select build spot
	[System.Serializable]
	public class BuildInfo{
		public Vector3 position=Vector3.zero;		//the position of the build point
		public PlatformTD platform;					//the platform the build point belongs to
		
		//the prefabIDs of the towers available to be build
		public List<int> availableTowerIDList=new List<int>();	
	}
	
	
	
	
	[System.Serializable]
	public class UnitedSolutionItem{
		public int ID=0;
		public string name="";
		
		public Sprite icon;
		//public Texture icon;
		//public string iconName;
	}
	
	
	[System.Serializable]
	public class Rsc : UnitedSolutionItem{
		public int value;
		
		public Rsc Clone(){
			Rsc rsc=new Rsc();
			rsc.ID=ID;
			rsc.name=name;
			rsc.icon=icon;
			//rsc.iconName=iconName;
			rsc.value=value;
			return rsc;
		}
		
		public bool IsMatch(Rsc rsc){
			if(rsc.ID!=ID) return false;
			if(rsc.name!=name) return false;
			if(rsc.icon!=icon) return false;
			return true;
		}
	}
	
	
	
	[System.Serializable]
	public class DAType : UnitedSolutionItem{
		public string desp="";
	}
	[System.Serializable]
	public class DamageType : DAType{
		
	}
	[System.Serializable]
	public class ArmorType : DAType{
		public List<float> modifiers=new List<float>();
	}
	
	
	
}