using UnityEngine;
using UnityEditor;

using System.Collections;

using UnitedSolution;

namespace UnitedSolution
{

    public class MenuExtension : EditorWindow
    {

        [MenuItem("Tools/New Scene - Fixed Path", false, -100)]
        private static void NewScene()
        {
            EditorApplication.NewScene();
            GameObject camObj = Camera.main.gameObject; DestroyImmediate(camObj);

            GameObject obj = (GameObject)Instantiate(Resources.Load("ScenePrefab/UnitedSolution_FixedPath", typeof(GameObject)));
            obj.name = "UnitedSolution_FixedPath";

            SpawnManager spawnManager = (SpawnManager)FindObjectOfType(typeof(SpawnManager));
            if (spawnManager.waveList[0].subWaveList[0].unit == null)
                spawnManager.waveList[0].subWaveList[0].unit = CreepDB.GetFirstPrefab().gameObject;
        }

        [MenuItem("Tools/New Scene - Open Path", false, -100)]
        static void New2()
        {
            EditorApplication.NewScene();
            GameObject camObj = Camera.main.gameObject; DestroyImmediate(camObj);

            GameObject obj = (GameObject)Instantiate(Resources.Load("ScenePrefab/UnitedSolution_OpenPath", typeof(GameObject)));
            obj.name = "UnitedSolution_OpenPath";

            SpawnManager spawnManager = (SpawnManager)FindObjectOfType(typeof(SpawnManager));
            if (spawnManager.waveList[0].subWaveList[0].unit == null)
                spawnManager.waveList[0].subWaveList[0].unit = CreepDB.GetFirstPrefab().gameObject;
        }

        [MenuItem("Tools/CreepEditor", false, 10)]
        public static void OpenCreepEditor()
        {
            UnitCreepEditorWindow.Init();
        }

        [MenuItem("Tools/TowerEditor", false, 10)]
        public static void OpenTowerEditor()
        {
            UnitTowerEditorWindow.filter = Filter.Tower;
            UnitTowerEditorWindow.Init();
        }

        [MenuItem("Tools/Hero Editor", false, 10)]
        public static void OpenHeroEditor()
        {
            UnitTowerEditorWindow.filter = Filter.Hero;
            UnitTowerEditorWindow.Init("Hero Editor");
        }

        [MenuItem("Tools/Defender Editor", false, 10)]
        public static void OpenDefenderEditor()
        {
            UnitTowerEditorWindow.filter = Filter.Defender;
            UnitTowerEditorWindow.Init("Defender Editor");
        }


        [MenuItem("Tools/SpawnEditor", false, 10)]
        public static void OpenSpawnEditor()
        {
            SpawnEditorWindow.Init();
        }

      

        [MenuItem("Tools/FPSWeaponEditor", false, 10)]
        public static void OpenAbilityEditor()
        {
            //FPSWeaponEditorWindow.Init();
        }

        [MenuItem("Tools/AbilityEditor", false, 10)]
        public static void OpenFPSWeaponEditor()
        {
            AbilityEditorWindow.Init();
        }

        [MenuItem("Tools/PerkEditor", false, 10)]
        public static void OpenPerkEditor()
        {
            PerkEditorWindow.Init();
        }

        [MenuItem("Tools/ResourceDBEditor", false, 10)]
        public static void OpenResourceEditor()
        {
            ResourceDBEditor.Init();
        }

        [MenuItem("Tools/DamageArmorTable", false, 10)]
        public static void OpenDamageTable()
        {
            DamageArmorDBEditor.Init();
        }
    }


}