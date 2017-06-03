using UnitedSolution;using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	public class CreepDB : MonoBehaviour {

		public List<UnitCreep> creepList=new List<UnitCreep>();
	
		public static CreepDB LoadDB(){
			GameObject obj=Resources.Load("DB_UnitedSolution/CreepDB", typeof(GameObject)) as GameObject;
			
			#if UNITY_EDITOR
				if(obj==null) obj=CreatePrefab();
			#endif
			
			return obj.GetComponent<CreepDB>();
		}
		
		public static List<UnitCreep> Load(){
			GameObject obj=Resources.Load("DB_UnitedSolution/CreepDB", typeof(GameObject)) as GameObject;
			
			#if UNITY_EDITOR
				if(obj==null) obj=CreatePrefab();
			#endif
			
			CreepDB instance=obj.GetComponent<CreepDB>();
			return instance.creepList;
		}
		
		#if UNITY_EDITOR
			private static GameObject CreatePrefab(){
				GameObject obj=new GameObject();
				obj.AddComponent<CreepDB>();
				GameObject prefab=PrefabUtility.CreatePrefab("Assets/UnitedSolution/Resources/DB_UnitedSolution/CreepDB.prefab", obj, ReplacePrefabOptions.ConnectToPrefab);
				DestroyImmediate(obj);
				AssetDatabase.Refresh ();
				return prefab;
			}
		#endif
		
		
		//for filling up  empty unit of spawnManager
		public static UnitCreep GetFirstPrefab(){
			GameObject obj=Resources.Load("DB_UnitedSolution/CreepDB", typeof(GameObject)) as GameObject;
			
			#if UNITY_EDITOR
				if(obj==null) obj=CreatePrefab();
			#endif
			
			CreepDB instance=obj.GetComponent<CreepDB>();
			return instance.creepList.Count==0 ? null : instance.creepList[0];
		}

	}
	
}
