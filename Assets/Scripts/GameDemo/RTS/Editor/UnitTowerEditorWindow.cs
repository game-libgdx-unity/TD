using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution
{
    public enum Filter
    {
        All,
        Tower,
        Defender,
        Hero
    }
    public class UnitTowerEditorWindow : UnitEditorWindow
    {
        private const int FULL_WIDTH_TOWER_LIST = 260;
        private const int COLAPSED_WIDTH_TOWER_LIST = 60;

        public static UnitTowerEditorWindow window;

        public static Filter filter;

        public static void Init(string title = "Tower Editor")
        {
            // Get existing open window or if none, make a new one:
            window = (UnitTowerEditorWindow)EditorWindow.GetWindow(typeof(UnitTowerEditorWindow), false, title);
            //~ window.minSize=new Vector2(375, 449);
            //~ window.maxSize=new Vector2(375, 800);

            EditorDBManager.Init();

            InitLabel();
            UpdateObjectHierarchyList();

            skin = Resources.Load<GUISkin>("GUISkin");
        }

        static GUISkin skin;
        private static string[] towerTypeLabel;
        private static string[] towerTypeTooltip;
        private static string[] targetModeLabel;
        private static string[] targetModeTooltip;

        private static void InitLabel()
        {
            int enumLength = Enum.GetValues(typeof(_TowerType)).Length;
            towerTypeLabel = new string[enumLength];
            towerTypeTooltip = new string[enumLength];
            for (int i = 0; i < enumLength; i++)
            {
                towerTypeLabel[i] = ((_TowerType)i).ToString();
                if ((_TowerType)i == _TowerType.Turret) towerTypeTooltip[i] = "Typical tower, fire shootObject to damage creep";
                if ((_TowerType)i == _TowerType.AOE) towerTypeTooltip[i] = "A tower that apply its effect to all creep within it's area of effective";
                if ((_TowerType)i == _TowerType.Support) towerTypeTooltip[i] = "A tower that buff nearby friendly tower";
                if ((_TowerType)i == _TowerType.Resource) towerTypeTooltip[i] = "A tower that generate resource ovetime";
                if ((_TowerType)i == _TowerType.Mine) towerTypeTooltip[i] = "Explode and apply aoe effects when a creep wanders into it's range\nDoesn't block up path when built on a walkable platform";
                if ((_TowerType)i == _TowerType.Block) towerTypeTooltip[i] = "A tower with no any particular function other than as a structure to block up path.";
            }

            enumLength = Enum.GetValues(typeof(_TargetMode)).Length;
            targetModeLabel = new string[enumLength];
            targetModeTooltip = new string[enumLength];
            for (int i = 0; i < enumLength; i++)
            {
                targetModeLabel[i] = ((_TargetMode)i).ToString();
                if ((_TargetMode)i == _TargetMode.Hybrid) targetModeTooltip[i] = "Target both air and ground units";
                if ((_TargetMode)i == _TargetMode.Air) targetModeTooltip[i] = "Target air units only";
                if ((_TargetMode)i == _TargetMode.Ground) targetModeTooltip[i] = "Target ground units only";
            }
        }




        private static List<GameObject> objHList = new List<GameObject>();
        private static string[] objHLabelList = new string[0];
        private static void UpdateObjectHierarchyList()
        {
            List<UnitTower> towerList = filterTowers();
            if (towerList.Count == 0 || selectID >= towerList.Count) return;
            EditorUtilities.GetObjectHierarchyList(towerList[selectID].gameObject, SetObjListCallback);
        }

        private static List<UnitTower> filterTowers()
        {
            List<UnitTower> towerList = EditorDBManager.GetTowerList();
            List<UnitTower> filteredList = null;
            if (filter == Filter.All)
                filteredList = towerList;
            else
            {
                filteredList = new List<UnitTower>();
                foreach (UnitTower ut in towerList)
                {
                    if (filter == Filter.Defender && ut.IsDefender())
                    {
                        filteredList.Add(ut);
                    }
                    else if (filter == Filter.Tower && (ut.IsTower() || ut.IsCreep()))
                    {
                        filteredList.Add(ut);
                    }
                    else if (filter == Filter.Hero && ut.IsHero())
                    {
                        filteredList.Add(ut);
                    }
                }
            }

            return filteredList;
        }

        public static void SetObjListCallback(List<GameObject> objList, string[] labelList)
        {
            objHList = objList;
            objHLabelList = labelList;
        }


        public void SelectTower(int ID)
        {
            selectID = ID;
            UpdateObjectHierarchyList();
            GUI.FocusControl("");

            if (selectID * 35 < scrollPos1.y) scrollPos1.y = selectID * 35;
            if (selectID * 35 > scrollPos1.y + listVisibleRect.height - 40) scrollPos1.y = selectID * 35 - listVisibleRect.height + 40;
        }

        public void Select(string name)
        {
            int selectID = 0;
            List<UnitTower> towerList = filterTowers();
            for (int i = 0; i < towerList.Count; i++)
            {
                if (towerList[i].unitName.Trim().Equals(name.Trim()))
                {
                    selectID = i;
                }
            }
            SelectTower(selectID);
        }



        private Vector2 scrollPos1;
        private Vector2 scrollPos2;

        private static int selectID = 0;
        private float contentHeight = 0;
        private float contentWidth = 0;


        void OnGUI()
        {
            if (window == null)
            {
                Init();
            }

            GUI.skin = skin;

            List<UnitTower> towerList = filterTowers();

            if (GUI.Button(new Rect(window.position.width - 120, 5, 100, 25), "Save")) EditorDBManager.SetDirtyTower();

            EditorGUI.LabelField(new Rect(5, 7, 150, 17), "Add new tower:");
            UnitTower newTower = null;
            newTower = (UnitTower)EditorGUI.ObjectField(new Rect(100, 7, 140, 17), newTower, typeof(UnitTower), false);
            if (newTower != null)
            {
                int newSelectID = EditorDBManager.AddNewTower(newTower);
                if (newSelectID != -1) SelectTower(newSelectID);
            }


            float startX = 5;
            float startY = 50;

            if (minimiseList)
            {
                if (GUI.Button(new Rect(startX, startY - 20, 30, 18), ">")) minimiseList = false;
            }
            else
            {
                if (GUI.Button(new Rect(startX, startY - 20, 30, 18), "<")) minimiseList = true;
            }
            Vector2 v2 = DrawUnitList(startX, startY, towerList);

            startX = v2.x + 25;

            if (towerList.Count == 0) return;

            cont = new GUIContent("Tower Prefab:", "The prefab object of the tower\nClick this to highlight it in the ProjectTab");
            EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
            EditorGUI.ObjectField(new Rect(startX + 90, startY, 185, height), towerList[selectID].gameObject, typeof(GameObject), false);

            cont = new GUIContent("Disable in BuildManager:", "When checked, tower won't appear on BuildManager list and thus can't be built\nThis is to mark towers that can only be upgrade from a built tower or unlock from perk");
            EditorGUI.LabelField(new Rect(startX + 295, startY, width, height), cont);
            towerList[selectID].disableInBuildManager = EditorGUI.Toggle(new Rect(startX + 440, startY, 185, height), towerList[selectID].disableInBuildManager);

            startY += spaceY + 10;


            Rect visibleRect = new Rect(startX, startY, window.position.width - startX - 10, window.position.height - startY - 5);
            Rect contentRect = new Rect(startX, startY, contentWidth - startY, contentHeight);

            //~ GUI.color=new Color(.8f, .8f, .8f, 1f);
            //~ GUI.Box(visibleRect, "");
            //~ GUI.color=Color.white;

            scrollPos2 = GUI.BeginScrollView(visibleRect, scrollPos2, contentRect);

            v2 = DrawUnitConfigurator(startX, startY, towerList);
            contentWidth = v2.x;
            contentHeight = v2.y;

            GUI.EndScrollView();


            if (GUI.changed) EditorDBManager.SetDirtyTower();
        }






        private Rect listVisibleRect;
        private Rect listContentRect;

        private int deleteID = -1;
        private bool minimiseList = false;
        Vector2 DrawUnitList(float startX, float startY, List<UnitTower> towerList)
        {

            float width = FULL_WIDTH_TOWER_LIST;
            if (minimiseList) width = COLAPSED_WIDTH_TOWER_LIST;

            if (!minimiseList)
            {
                if (GUI.Button(new Rect(startX + 100, startY - 20, 40, 18), "up"))
                {
                    if (selectID > 0)
                    {
                        UnitTower tower = towerList[selectID];
                        towerList[selectID] = towerList[selectID - 1];
                        towerList[selectID - 1] = tower;
                        selectID -= 1;

                        if (selectID * 35 < scrollPos1.y) scrollPos1.y = selectID * 35;
                    }
                }
                if (GUI.Button(new Rect(startX + 142, startY - 20, 40, 18), "down"))
                {
                    if (selectID < towerList.Count - 1)
                    {
                        UnitTower tower = towerList[selectID];
                        towerList[selectID] = towerList[selectID + 1];
                        towerList[selectID + 1] = tower;
                        selectID += 1;

                        if (listVisibleRect.height - 35 < selectID * 35) scrollPos1.y = (selectID + 1) * 35 - listVisibleRect.height + 5;
                    }
                }
            }


            listVisibleRect = new Rect(startX, startY, width + 15, window.position.height - startY - 5);
            listContentRect = new Rect(startX, startY, width, towerList.Count * 35 + 5);

            GUI.color = new Color(.8f, .8f, .8f, 1f);
            GUI.Box(listVisibleRect, "");
            GUI.color = Color.white;

            scrollPos1 = GUI.BeginScrollView(listVisibleRect, scrollPos1, listContentRect);


            startY += 5; startX += 5;

            for (int i = 0; i < towerList.Count; i++)
            {

                EditorUtilities.DrawSprite(new Rect(startX, startY + (i * 35), 30, 30), towerList[i].iconSprite);

                if (minimiseList)
                {
                    if (selectID == i) GUI.color = new Color(0, 1f, 1f, 1f);
                    if (GUI.Button(new Rect(startX + 115, startY + (i * 35), 30, 30), "X")) SelectTower(i);
                    GUI.color = Color.white;

                    continue;
                }



                if (selectID == i) GUI.color = new Color(0, 1f, 1f, 1f);
                if (GUI.Button(new Rect(startX + 35, startY + (i * 35), 180, 30), towerList[i].unitName)) SelectTower(i);
                GUI.color = Color.white;

                if (deleteID == i)
                {

                    if (GUI.Button(new Rect(startX + 137, startY + (i * 35), 20, 20), "N")) deleteID = -1;

                    GUI.color = Color.red;
                    if (GUI.Button(new Rect(startX + 137, startY + (i * 35) + 15, 20, 20), "Y"))
                    {
                        if (selectID >= deleteID) SelectTower(Mathf.Max(0, selectID - 1));
                        EditorDBManager.RemoveTower(deleteID);
                        deleteID = -1;
                    }
                    GUI.color = Color.white;
                }
                else
                {
                    if (GUI.Button(new Rect(startX + 217, startY + (i * 35), 25, 25), "X")) deleteID = i;
                }
            }

            GUI.EndScrollView();

            return new Vector2(startX + width, startY);
        }




        Vector3 v3 = Vector3.zero;

        Vector2 DrawUnitConfigurator(float startX, float startY, List<UnitTower> towerList)
        {
            float maxWidth = 0;

            UnitTower tower = towerList[selectID];


            float cachedY = startY;
            float cachedX = startX;
            startX += 65;   //startY+=20;

            int type = (int)tower.type;
            cont = new GUIContent("Tower Type:", "Type of the tower. Each type of tower serve a different function");
            EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
            contL = new GUIContent[towerTypeLabel.Length];
            for (int i = 0; i < contL.Length; i++) contL[i] = new GUIContent(towerTypeLabel[i], towerTypeTooltip[i]);
            type = EditorGUI.Popup(new Rect(startX + 80, startY, width - 40, 15), new GUIContent(""), type, contL);
            tower.type = (_TowerType)type;
            startX = cachedX;

            v3 = DrawIconAndName(tower, startX, startY); startY = v3.y;


            startX = cachedX;
            spaceX = 110;

            cachedY = startY;
            v3 = DrawUnitDefensiveSetting(tower, startX, startY, objHList, objHLabelList);  //startY=v3.y;


            if (startX + spaceX + width > maxWidth) maxWidth = startX + spaceX + width;


            startY = cachedY;
            startX += spaceX + width + 35;



            if (TowerDealDamage(tower))
            {
                int targetMode = (int)tower.targetMode;
                cont = new GUIContent("Target Mode:", "Determine which type of unit the tower can target");
                EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
                contL = new GUIContent[targetModeLabel.Length];
                for (int i = 0; i < contL.Length; i++) contL[i] = new GUIContent(targetModeLabel[i], targetModeTooltip[i]);
                targetMode = EditorGUI.Popup(new Rect(startX + spaceX, startY, width, height), new GUIContent(""), targetMode, contL);
                tower.targetMode = (_TargetMode)targetMode;
                startY += spaceY;
            }

            v3 = DrawUnitOffensiveSetting(tower, startX, startY, objHList, objHLabelList); startY = v3.y + spaceY;


            if (startX + spaceX + width > maxWidth) maxWidth = startX + spaceX + width;



            string[] weaponNameList = EditorDBManager.GetFPSWeaponNameList();
            List<FPSWeapon> weaponList = EditorDBManager.GetFPSWeaponList();
            int weaponID = 0;
            if (tower.FPSWeaponID != -1)
            {
                for (int i = 0; i < weaponList.Count; i++)
                {
                    if (weaponList[i].prefabID == tower.FPSWeaponID) { weaponID = i + 1; break; }
                }
            }
            cont = new GUIContent("FPS Weapon:", "Weapon tied to this tower when using FPS mode");
            GUI.Label(new Rect(startX, startY, 120, height), cont);
            weaponID = EditorGUI.Popup(new Rect(startX + spaceX, startY, width, height), weaponID, weaponNameList);
            if (weaponID == 0) tower.FPSWeaponID = -1;
            else tower.FPSWeaponID = weaponList[weaponID - 1].prefabID;





            //if(startY>maxHeight) maxHeight=startY;
            float maxY = startY;
            startY = 300;

            startX = cachedX; cachedY = startY;

            string[] towerNameList = EditorDBManager.GetTowerNameList();

            cont = new GUIContent("Prev lvl Tower:", "Tower prefab which this current selected tower is upgrade from. If blank then this is the base tower (level 1). ");
            GUI.Label(new Rect(startX, startY, 120, height), cont);
            int ID = -1;
            for (int i = 0; i < towerList.Count; i++) { if (towerList[i] == tower.prevLevelTower) ID = i + 1; }
            ID = EditorGUI.Popup(new Rect(startX + spaceX, startY, 105, height), ID, towerNameList);
            if (GUI.Button(new Rect(startX + 215, startY, 20, 15), "^")) { if (tower.prevLevelTower != null) SelectTower(ID - 1); }


            cont = new GUIContent("lvl within Prefab:", "");
            GUI.Label(new Rect(startX, startY += spaceY, 120, height), cont);
            if (GUI.Button(new Rect(startX + spaceX, startY, 50, 20), "-1"))
            {
                if (tower.stats.Count > 1) tower.stats.RemoveAt(tower.stats.Count - 1);
            }
            if (GUI.Button(new Rect(startX + 165, startY, 50, 20), "+1"))
            {
                tower.stats.Add(tower.stats[tower.stats.Count - 1].Clone());
            }

            if (tower.nextLevelTowerList.Count == 0) tower.nextLevelTowerList.Add(null);

            cont = new GUIContent("Next lvl Tower 1:", "Tower prefab to be used beyond the stats level specified for this prefab");
            GUI.Label(new Rect(startX, startY += (spaceY + 10), 120, height), cont);
            ID = -1;
            for (int i = 0; i < towerList.Count; i++) { if (towerList[i] == tower.nextLevelTowerList[0]) ID = i + 1; }
            ID = EditorGUI.Popup(new Rect(startX + spaceX, startY, 105, height), ID, towerNameList);
            if (ID > 0 && towerList[ID - 1] != tower)
            {
                if (tower.nextLevelTowerList[0] != null) tower.nextLevelTowerList[0].prevLevelTower = null;

                tower.nextLevelTowerList[0] = towerList[ID - 1];
                towerList[ID - 1].prevLevelTower = tower;
            }
            else if (ID == 0) tower.nextLevelTowerList[0] = null;

            if (GUI.Button(new Rect(startX + 215, startY, 20, 15), "^")) { if (tower.nextLevelTowerList[0] != null) SelectTower(ID - 1); }

            if (tower.nextLevelTowerList[0] == null)
            {
                if (tower.nextLevelTowerList.Count > 1 && tower.nextLevelTowerList[1] != null)
                {
                    tower.nextLevelTowerList[0] = tower.nextLevelTowerList[1];
                    tower.nextLevelTowerList.RemoveAt(1);
                }
            }

            if (tower.nextLevelTowerList[0] != null)
            {
                if (tower.nextLevelTowerList.Count < 2) tower.nextLevelTowerList.Add(null);

                //startX+=295;
                cont = new GUIContent("Next lvl Tower 2:", "Tower prefab to be used beyond the stats level specified for this prefab");
                GUI.Label(new Rect(startX, startY += spaceY, 120, height), cont);
                ID = -1;
                for (int i = 0; i < towerList.Count; i++) { if (towerList[i] == tower.nextLevelTowerList[1]) ID = i + 1; }
                ID = EditorGUI.Popup(new Rect(startX + spaceX, startY, 105, height), ID, towerNameList);
                if (ID > 0 && towerList[ID - 1] != tower && !tower.nextLevelTowerList.Contains(towerList[ID - 1]))
                {
                    if (tower.nextLevelTowerList[1] != null) tower.nextLevelTowerList[1].prevLevelTower = null;

                    tower.nextLevelTowerList[1] = towerList[ID - 1];
                    towerList[ID - 1].prevLevelTower = tower;
                }
                else if (ID == 0) tower.nextLevelTowerList[1] = null;
                if (GUI.Button(new Rect(startX + 215, startY, 20, 15), "^")) { if (tower.nextLevelTowerList[1] != null) SelectTower(ID - 1); }

                //GUI.Label(new Rect(startX+270, startY, 200, height), "(for alternate upgrade path)");
                //startX=cachedX;
            }



            startY = Mathf.Max(maxY + 20, 400);
            startX = cachedX;

            float maxHeight = 0;
            float maxContentHeight = 0;

            minimiseStat = EditorGUI.Foldout(new Rect(startX, startY -= 30, width, height), minimiseStat, "Show Stats");
            if (!minimiseStat)
            {
                startY += spaceY;
                startX += 15;

                int lvl = GetStatsLevelStartOffset(tower);

                if (tower.stats.Count == 0) tower.stats.Add(new UnitStat());
                for (int i = 0; i < tower.stats.Count; i++)
                {
                    EditorGUI.LabelField(new Rect(startX, startY, width, height), "Level " + (lvl + i + 1) + " Stats");
                    v3 = DrawStat(tower.stats[i], startX, startY + spaceY - 20, statContentHeight, tower);
                    if (maxContentHeight < v3.z) maxContentHeight = v3.z;
                    //statContentHeight=v3.z;
                    startX = v3.x + 10;
                    if (startX > maxWidth) maxWidth = startX;
                    if (maxHeight < v3.y) maxHeight = v3.y;
                }
                statContentHeight = maxContentHeight;
                startY = maxHeight;
            }

            startX = cachedX;
            startY += spaceY;


            GUIStyle style = new GUIStyle("TextArea");
            style.wordWrap = true;
            cont = new GUIContent("Unit general description (to be used in runtime): ", "");
            EditorGUI.LabelField(new Rect(startX, startY += spaceY, 400, 20), cont);
            tower.desp = EditorGUI.TextArea(new Rect(startX, startY + spaceY - 3, 530, 50), tower.desp, style);

            startX = maxWidth - cachedX + 80;

            return new Vector2(startX, startY);
        }


        private bool minimiseStat = false;

        private float statContentHeight = 0;





        int GetStatsLevelStartOffset(UnitTower tower)
        {
            if (tower.prevLevelTower == null) return 0;

            int val = GetStatsLevelStartOffset(tower.prevLevelTower);
            val += tower.prevLevelTower.stats.Count;

            return val;
        }

    }

}