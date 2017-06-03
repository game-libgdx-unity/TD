using UnitedSolution;using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace UnitedSolution
{
    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;
        public List<GameObject> inactiveList = new List<GameObject>();
        public List<GameObject> activeList = new List<GameObject>();
        public int cap = 1000;
        public GameObject Spawn(Vector3 pos, Quaternion rot)
        {
            GameObject obj = null;
            if (inactiveList.Count == 0)
            {
                obj = (GameObject)MonoBehaviour.Instantiate(prefab, pos, rot);
                activeList.Add(obj);
            }
            else
            {
                bool successSpawn = false;
                for (int index = 0; index < inactiveList.Count; index++)
                {
                    obj = inactiveList[index];
                    if (obj)
                    {
                        obj.transform.parent = null;
                        obj.transform.position = pos;
                        obj.transform.rotation = Quaternion.identity;
                        obj.SetActive(true);
                        inactiveList.RemoveAt(index);
                        activeList.Add(obj);
                        successSpawn = true;
                        break;
                    }
                    else
                    {
                        inactiveList.RemoveAt(index);
                        index--;
                    }
                }

                if (!successSpawn)
                {
                    obj = (GameObject)MonoBehaviour.Instantiate(prefab, pos, rot);
                    activeList.Add(obj);
                }
            }
            return obj;
        }

        public bool Unspawn(GameObject obj)
        {
            if (activeList.Contains(obj))
            {
                obj.transform.parent = ObjectPoolManager.GetOPMTransform();
                activeList.Remove(obj);
                inactiveList.Add(obj);
                obj.SetActive(false);
                return true;
            }
            return false;
        }


        public void MatchObjectCount(int count)
        {
            if (count > cap) return;
            int currentCount = GetTotalObjectCount();
            for (int i = currentCount; i < count; i++)
            {
                GameObject obj = (GameObject)MonoBehaviour.Instantiate(prefab);
                obj.SetActive(false);
                obj.transform.parent = ObjectPoolManager.GetOPMTransform();
                inactiveList.Add(obj);
            }
        }

        public int GetTotalObjectCount()
        {
            return inactiveList.Count + activeList.Count;
        }

        public void Clear()
        {
            for (int i = 0; i < inactiveList.Count; i++)
            {
                if (inactiveList[i] != null) MonoBehaviour.Destroy(inactiveList[i]);
            }
            for (int i = 0; i < activeList.Count; i++)
            {
                if (activeList[i] != null) MonoBehaviour.Destroy(inactiveList[i]);
            }
        }
    }
}
