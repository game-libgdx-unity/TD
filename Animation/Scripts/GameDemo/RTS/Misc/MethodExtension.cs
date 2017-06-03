using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitedSolution;using UnityEngine;

namespace UnitedSolution
{
    public static class MethodExtension
    {
        public static bool IsLayerDefault(this LayerMask layer)
        {
            return layer == 1 << LayerManager.GetLayerDefault();
        }

        public static void Log(this MonoBehaviour layer)
        {
           
        }
        public static Collider[] GetColliders(this RaycastHit[] hits)
        {
            Collider[] output = new Collider[hits.Length];
            for (int i = 0; i < output.Length; i++)
                output[i] = hits[i].collider;

            return output;
        }

    }
}
