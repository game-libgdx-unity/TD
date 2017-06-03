using UnitedSolution;using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	public class PerkDB : MonoBehaviour {

		public List<Perk> perkList=new List<Perk>();
		
		public static PerkDB LoadDB(){
			GameObject obj=Resources.Load("DB_UnitedSolution/PerkDB", typeof(GameObject)) as GameObject;
			
			#if UNITY_EDITOR
				if(obj==null) obj=CreatePrefab();
			#endif
			
			return obj.GetComponent<PerkDB>();
		}
		
		public static List<Perk> Load(){
			GameObject obj=Resources.Load("DB_UnitedSolution/PerkDB", typeof(GameObject)) as GameObject;
			
			#if UNITY_EDITOR
				if(obj==null) obj=CreatePrefab();
			#endif
			
			PerkDB instance=obj.GetComponent<PerkDB>();
			return instance.perkList;
		}
		
		public static List<Perk> LoadClone(){
			GameObject obj=Resources.Load("DB_UnitedSolution/PerkDB", typeof(GameObject)) as GameObject;
			PerkDB instance=obj.GetComponent<PerkDB>();
			
			List<Perk> newList=new List<Perk>();
			
			if(instance!=null){
				for(int i=0; i<instance.perkList.Count; i++){
					newList.Add(instance.perkList[i].Clone());
				}
			}
			
			return newList;
		}
		
		#if UNITY_EDITOR
			private static GameObject CreatePrefab(){
				GameObject obj=new GameObject();
				obj.AddComponent<PerkDB>();
				GameObject prefab=PrefabUtility.CreatePrefab("Assets/UnitedSolution/Resources/DB_UnitedSolution/PerkDB.prefab", obj, ReplacePrefabOptions.ConnectToPrefab);
				DestroyImmediate(obj);
				AssetDatabase.Refresh ();
				return prefab;
			}
		#endif
		
	}

}
