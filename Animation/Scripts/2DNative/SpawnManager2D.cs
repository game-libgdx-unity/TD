using System;
using System.Collections.Generic;
using UnitedSolution;using UnityEngine;
using DG.Tweening;
using System.Collections;

namespace UnitedSolution
{
    public class SpawnManager2D : MonoBehaviour
    {

        public Unit2D prefab;
        public int quatity = 1;
        public int column = 0;
        public int screenHeight = 20;

        public List<Unit2D> units;

        public static Action<Unit2D> OnUnit2DSpawned;
        public static Action<Unit2D> OnUnit2DUnspawned;

        // Use this for initialization
        void Start()
        {
            DOVirtual.DelayedCall(.5f, () =>
            {
                int offsetY = quatity / 2;
                Vector2 offsetPos = new Vector2(20, screenHeight * offsetY);
                for (int i = 0; i < quatity; i++)
                {
                    Unit2D unit = Instantiate(prefab);
                    unit.transform.ScreenPlacement(ScreenPosition.Left, offsetPos);
                    unit.transform.parent = transform;
                    offsetPos -= new Vector2(0, screenHeight);
                    units.Add(unit);
                    if (OnUnit2DSpawned != null)
                    {
                        OnUnit2DSpawned(unit);
                    }
                }
            });

            //StartCoroutine(CheckRenderSortingOrder());
        }
    }
}