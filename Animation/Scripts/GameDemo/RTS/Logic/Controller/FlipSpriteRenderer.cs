using System.Collections;
using System.Collections.Generic;
using UnitedSolution;using UnityEngine;
using UnityEngine.AI;

namespace UnitedSolution {
    public class FlipSpriteRenderer : MonoBehaviour {

        Unit unit;
        SpriteRenderer spriteRenderer;

        void Start()
        {
            unit = transform.parent.GetComponent<Unit>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            if (unit.target)
            {
                spriteRenderer.flipX = unit.transform.position.x - unit.target.transform.position.x > 0;
                spriteRenderer.sortingOrder = int.MaxValue - (int)(1000 * unit.transform.position.z);
            }
        }
    }
}