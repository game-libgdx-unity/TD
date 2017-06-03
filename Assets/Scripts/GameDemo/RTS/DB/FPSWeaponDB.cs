using UnitedSolution;using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	public class FPSWeaponDB : MonoBehaviour {

		public List<FPSWeapon> weaponList=new List<FPSWeapon>();
	
		public static FPSWeaponDB LoadDB(){
			GameObject obj=Resources.Load("DB_UnitedSolution/FPSWeaponDB", typeof(GameObject)) as GameObject;
			
			#if UNITY_EDITOR
				if(obj==null) obj=CreatePrefab();
			#endif
			
			return obj.GetComponent<FPSWeaponDB>();
		}
		
		public static List<FPSWeapon> Load(){
			GameObject obj=Resources.Load("DB_UnitedSolution/FPSWeaponDB", typeof(GameObject)) as GameObject;
			
			#if UNITY_EDITOR
				if(obj==null) obj=CreatePrefab();
			#endif
			
			FPSWeaponDB instance=obj.GetComponent<FPSWeaponDB>();
			return instance.weaponList;
		}
		
		#if UNITY_EDITOR
			private static GameObject CreatePrefab(){
				GameObject obj=new GameObject();
				obj.AddComponent<FPSWeaponDB>();
				GameObject prefab=PrefabUtility.CreatePrefab("Assets/UnitedSolution/Resources/DB_UnitedSolution/FPSWeaponDB.prefab", obj, ReplacePrefabOptions.ConnectToPrefab);
				DestroyImmediate(obj);
				AssetDatabase.Refresh ();
				return prefab;
			}
		#endif

	}
	
}
