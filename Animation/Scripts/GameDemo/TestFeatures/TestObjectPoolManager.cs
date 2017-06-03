//using System.Collections;
//using System.Collections.Generic;
//using UnitedSolution;
//using UnitedSolution;using UnityEngine;

//public class TestObjectPoolManager : LazyBehaviour
//{
//    public GameObject testPrefab;
//    private const int OBJECT_COUNT = 10;

//    // Use this for initialization
//    void Start () {

//        GameObject[] dummyObjects = new GameObject[OBJECT_COUNT]; //an initial array of dummy objects
//        ObjectPoolManager.New(testPrefab); //cached the prefab in OPM

//        // spawn 10 dummy objects
//        DelayCall(1f, () => { 
//            for(int i = 0; i < OBJECT_COUNT; i++)
//            {
//                //dummyObjects[i] = ObjectPoolManager.Spawn(testPrefab, transform);
//                //dummyObjects[i].name = Random.Range(0, 1000).ToString(); //random name
//            }
//        });


//        // unspawn 10 dummy objects
//        DelayCall(3f, () => {
//            for (int i = 0; i < OBJECT_COUNT; i++)
//            {
//                ObjectPoolManager.Unspawn(dummyObjects[i]);
//            }
//        });
        
//        // spawn 10 dummy objects
//        DelayCall(5f, () => {
//            for (int i = 0; i < OBJECT_COUNT; i++)
//            {
//                //dummyObjects[i] = ObjectPoolManager.Spawn(testPrefab, transform);
//            }
//        });

//    }
//}
