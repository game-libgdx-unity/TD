using System.Collections;
using System.Collections.Generic;
using UnitedSolution;using UnityEngine;
using System;
using DG.Tweening;

namespace UnitedSolution
{
    public class SimpleSpawner : MonoBehaviour {

        public float delay = .5f;
        public Spawner[] prefabs;

        // Use this for initialization
        void Start() {

            foreach (Spawner spawner in prefabs)
            {
                ObjectPoolManager.New(spawner.prefab);
            }

            DOVirtual.DelayedCall(delay, () =>
            {
                foreach (Spawner spawner in prefabs)
                {
                    for(int i = 0; i < spawner.quatity; i++ )
                    {
                        ObjectPoolManager.Spawn(spawner.prefab, spawner.StartingPosition);
                    }
                }
            });
        }

        // Update is called once per frame
        void Update() {

        }
    }
    [Serializable]
    public class Spawner
    {
        public GameObject prefab;
        public int quatity = 1;
        public Transform StartingPosition;
    }
}