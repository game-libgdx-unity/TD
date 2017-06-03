using UnitedSolution;using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	public class ResourceDB : MonoBehaviour {

		public List<Rsc> rscList=new List<Rsc>();
		
		public static ResourceDB LoadDB(){
			GameObject obj=Resources.Load("DB_UnitedSolution/ResourceDB", typeof(GameObject)) as GameObject;
			
			#if UNITY_EDITOR
				if(obj==null) obj=CreatePrefab();
			#endif
			
			return obj.GetComponent<ResourceDB>();
		}
		
		public static List<Rsc> Load(){
			GameObject obj=Resources.Load("DB_UnitedSolution/ResourceDB", typeof(GameObject)) as GameObject;
			
			#if UNITY_EDITOR
				if(obj==null) obj=CreatePrefab();
			#endif
			
			ResourceDB instance=obj.GetComponent<ResourceDB>();
			return instance.rscList;
		}
		
		public static List<Rsc> LoadClone(){
			GameObject obj=Resources.Load("DB_UnitedSolution/ResourceDB", typeof(GameObject)) as GameObject;
			ResourceDB instance=obj.GetComponent<ResourceDB>();
			
			List<Rsc> newList=new List<Rsc>();
			
			if(instance!=null){
				for(int i=0; i<instance.rscList.Count; i++){
					newList.Add(instance.rscList[i].Clone());
				}
			}
			
			return newList;
		}
		
		#if UNITY_EDITOR
			private static GameObject CreatePrefab(){
				GameObject obj=new GameObject();
				obj.AddComponent<ResourceDB>();
				GameObject prefab=PrefabUtility.CreatePrefab("Assets/UnitedSolution/Resources/DB_UnitedSolution/ResourceDB.prefab", obj, ReplacePrefabOptions.ConnectToPrefab);
				DestroyImmediate(obj);
				AssetDatabase.Refresh ();
				return prefab;
			}
		#endif
		
	}

}
