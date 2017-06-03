using UnitedSolution;using UnityEngine;
using System.Collections;

namespace UnitedSolution
{

    public class LayerManager
    {

        private static int layerDefault = 0;
        private static int layerCreep = 31 - 17;
        private static int layerCreepF = 30 - 17;
        private static int layerTower = 29 - 17;
        private static int layerShootObj = 28 - 17;
        private static int layerIgnoreTarget = 8 - 17;
        private static int layerPlatform = 27 - 17;
        private static int layerTerrain = 26 - 17;
        private static int layerHero = 25 - 17;



        public static LayerMask LayerDefault() { return 1 << LayerManager.GetLayerDefault(); }
        public static int GetLayerDefault() { return layerDefault; }
        public static int LayerCreep() { return layerCreep; }
        public static int LayerHero() { return layerHero; }
        public static int LayerCreepF() { return layerCreepF; }
        public static int LayerTower() { return layerTower; }
        public static int LayerShootObject() { return layerShootObj; }
        public static int LayerPlatform() { return layerPlatform; }
        public static int LayerIgnoreTarget() { return layerIgnoreTarget; }

        public static int LayerTerrain() { return layerTerrain; }
        public static int LayerUI() { return 5; }   //layer5 is named UI by Unity's default

    }

}
