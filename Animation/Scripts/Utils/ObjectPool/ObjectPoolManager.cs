using UnitedSolution;using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace UnitedSolution
{
    public class ObjectPoolManager : SingletonBehaviour<ObjectPoolManager>
    {
        #region Variables

        public GameObject[] prefabs;
        public List<Pool> poolList = new List<Pool>();

        #endregion

        #region Public members

        public static ShootObject Spawn(ShootObject so, Vector3 pos, Quaternion rot)
        {
            GameObject go = Spawn(so.gameObject, pos, rot);
            return go.GetComponent<ShootObject>();
        }

        public static Transform Spawn(Transform objT)
        {
            return Spawn(objT.gameObject, Vector3.zero, Quaternion.identity).transform;
        }
        public static Transform Spawn(Transform objT, Vector3 pos, Quaternion rot)
        {
            return Instance._Spawn(objT.gameObject, pos, rot).transform;
        }
        public static GameObject Spawn(GameObject obj)
        {
            return Spawn(obj, Vector3.zero, Quaternion.identity);
        }
        public static GameObject Spawn(GameObject obj, Transform parentTransform)
        {
            GameObject go = Spawn(obj);
            go.transform.parent = parentTransform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            return go;
        }
        public static Mesh Spawn(Mesh mesh)
        {
            throw new NotImplementedException();
        }
        public static GameObject Spawn(GameObject obj, Vector3 pos, Quaternion rot)
        {
            return Instance._Spawn(obj, pos, Quaternion.identity);
        }
        public static void ClearAll()
        {
            for (int i = 0; i < Instance.poolList.Count; i++) Instance.poolList[i].Clear();
            Instance.poolList = new List<Pool>();
        }
        public static Transform GetOPMTransform()
        {
            return Instance.transform;
        }
        public static void Unspawn(GameObject objT, float delay) { Instance.StartCoroutine(RemovalDelay(objT, delay)); }
        public static void Unspawn(Transform objT) { Instance._Unspawn(objT.gameObject); }
        public static void Unspawn(GameObject obj) { Instance._Unspawn(obj); }
        public static int New(Transform objT, int count = 5) { return Instance._New(objT.gameObject, count); }
        public static int New(GameObject obj, int count = 5) { return Instance._New(obj, count); }

        #endregion

        #region Protected members

        protected override void Awake()
        {
            base.Awake();

            if (prefabs != null)
            {
                foreach (GameObject prefab in prefabs)
                {
                    New(prefab); //cache prefabs
                }
            }
        }

        #endregion

        #region Private members

        private static IEnumerator RemovalDelay(GameObject go, float delay) { yield return new WaitForSeconds(delay); Unspawn(go); }

        private int _New(GameObject obj, int count = 5)
        {
            int ID = GetPoolID(obj);
            if (ID != -1)
            {
                poolList[ID].MatchObjectCount(count);
            }
            else
            {
                Pool pool = new Pool()
                {
                    prefab = obj
                };
                pool.MatchObjectCount(count);
                poolList.Add(pool);
                ID = poolList.Count - 1;
            }
            return ID;
        }
        private void _Unspawn(GameObject obj)
        {
            if (obj == null || !obj.activeSelf)
            {
                return;
            }

            for (int i = 0; i < poolList.Count; i++)
            {
                if (poolList[i].Unspawn(obj)) return;
            }

            int ID = GetPoolID(obj);
            if (ID > -1)
            {
                obj.SetActive(false);
                poolList[ID].inactiveList.Add(obj);
                Debug.LogWarning("Cannot unspawn the object properly, using force to unspawn");
            }
            else
            {
                Destroy(obj);
                Debug.LogWarning("Cannot force to unspawn the object, using force to destroy");
            }
        }
        private GameObject _Spawn(GameObject obj, Vector3 pos, Quaternion rot)
        {
            if (obj == null)
            {
                Debug.Log("NullReferenceException: obj unspecified");
                return null;
            }

            int ID = GetPoolID(obj);

            if (ID == -1) ID = _New(obj);

            return poolList[ID].Spawn(pos, rot);
        }
        private int GetPoolID(GameObject obj)
        {
            for (int i = 0; i < poolList.Count; i++)
            {
                if (poolList[i].prefab == obj) return i;
            }
            return -1;
        }

        #endregion
    }
}