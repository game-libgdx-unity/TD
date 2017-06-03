using UnitedSolution;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution
{

    public class UIBuildButton : MonoBehaviour
    {

        public List<UnityButton> buttonList = new List<UnityButton>();  //all button list
        private List<UnityButton> activeButtonList = new List<UnityButton>();   //current active button list, subject to tower availability on platform

        private BuildInfo buildInfo;

        public static UIBuildButton instance;
        private GameObject thisObj;

        void Awake()
        {
            instance = this;
            thisObj = gameObject;

            isOn = false;
        }

        // Use this for initialization
        void Start()
        {
            List<UnitTower> towerList = BuildManager.GetTowerList();

            EventTrigger.Entry entry = SetupTriggerEntry();

            for (int i = 0; i < towerList.Count; i++)
            {
                if (i == 0) buttonList[0].Init();
                else if (i > 0)
                {
                    buttonList.Add(buttonList[0].Clone("button" + (i + 1), new Vector3(i * 55, 0, 0)));
                }

                buttonList[i].imageIcon.sprite = towerList[i].iconSprite;

                EventTrigger trigger = buttonList[i].rootObj.GetComponent<EventTrigger>();
                trigger.triggers.Add(entry);
            }

            Hide();
        }

        private EventTrigger.Entry SetupTriggerEntry()
        {
            UnityEngine.Events.UnityAction<BaseEventData> call = new UnityEngine.Events.UnityAction<BaseEventData>(OnTowerButton);
            EventTriggerType eventID = EventTriggerType.PointerClick;
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
				if(UI.UseDragNDrop()) eventID=EventTriggerType.PointerDown;
#endif

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = eventID;
            entry.callback = new EventTrigger.TriggerEvent();
            entry.callback.AddListener(call);

            return entry;
        }



        public static void AddNewTower(UnitTower newTower) { instance._AddNewTower(newTower); }
        public void _AddNewTower(UnitTower newTower)
        {
            int count = buttonList.Count;
            buttonList.Add(buttonList[count - 1].Clone("button" + (count), new Vector3(55, 0, 0)));
            buttonList[count].imageIcon.sprite = newTower.iconSprite;

            EventTrigger trigger = buttonList[count].rootObj.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = SetupTriggerEntry();
            trigger.triggers.Add(entry);
        }



        public void OnTowerButton(UnityEngine.EventSystems.BaseEventData baseEvent)
        {
            OnTowerButton(baseEvent.selectedObject);
        }
        public void OnTowerButton(GameObject butObj)
        {
            //in drag and drop mode, player could be hitting the button while having an active tower selected
            //if that's the case, clear the selectedTower first. and we can show the tooltip properly
            if (UI.UseDragNDrop() && GameControl.GetSelectedTower() != null)
            {
                UI.ClearSelectedTower();
                return;
            }

            int ID = GetButtonID(butObj);

            List<UnitTower> towerList = BuildManager.GetTowerList();

            string exception = "";
            if (!UI.UseDragNDrop()) exception = BuildManager.BuildTower(towerList[ID]);
            else exception = BuildManager.BuildTowerDragNDrop(towerList[ID]);

            if (exception != "") UIGameMessage.DisplayMessage(exception);

            if (!UI.UseDragNDrop()) OnExitHoverButton(butObj);

            Hide();
        }
        public void OnHoverTowerButton(GameObject butObj)
        {
            if (GameControl.GetSelectedTower() != null) return;

            int ID = GetButtonID(butObj);

            UnitTower tower = BuildManager.GetSampleTower(ID);

            if (!UI.UseDragNDrop()) { BuildManager.ShowSampleTower(ID); } //clear sample

            //show tooltip
            UITowerInfo.Show(tower);
        }
        public void OnExitHoverButton(GameObject butObj)
        {
            if (GameControl.GetSelectedTower() != null && !GameControl.GetSelectedTower().IsSampleTower()) return;

            BuildManager.ClearSampleTower();

            //clear tooltip
            UITowerInfo.Hide();
        }
        int GetButtonID(GameObject butObj)
        {
            for (int i = 0; i < buttonList.Count; i++)
            {
                if (buttonList[i].rootObj == butObj) return i;
            }
            return 0;
        }


        // Update is called once per frame
        void Update()
        {
            if (!UI.UseDragNDrop())
            {
                float cutoff = 120;

                Vector3 screenPos = Camera.main.WorldToScreenPoint(buildInfo.position);
                List<Vector3> pos = GetPieMenuPos(activeButtonList.Count, screenPos, cutoff, 45);

                for (int i = 0; i < activeButtonList.Count; i++)
                {
                    if (i < pos.Count)
                    {
                        activeButtonList[i].rootT.localPosition = pos[i];
                    }
                    else
                    {
                        activeButtonList[i].rootT.localPosition = new Vector3(0, 9999, 0);
                    }
                }
            }
        }


        void UpdateActiveBuildButtonList()
        {
            if (buildInfo == null) return;
            //Debug.Log("UpdateActiveBuildButtonList");

            activeButtonList = new List<UnityButton>();
            for (int i = 0; i < buildInfo.availableTowerIDList.Count; i++)
            {
                //Debug.Log(buildInfo.availableTowerIDList[i]);
                activeButtonList.Add(buttonList[buildInfo.availableTowerIDList[i]]);
            }

            for (int i = 0; i < buttonList.Count; i++)
            {
                if (activeButtonList.Contains(buttonList[i])) buttonList[i].rootObj.SetActive(true);
                else buttonList[i].rootObj.SetActive(false);
            }
        }


        public static bool isOn = true;
        public static void Show() { instance._Show(); }
        public void _Show()
        {
            buildInfo = BuildManager.GetBuildInfo();
            UpdateActiveBuildButtonList();
            Update();

            isOn = true;
            thisObj.SetActive(isOn);
        }
        public static void Hide() { instance._Hide(); }
        public void _Hide()
        {
            if (UI.UseDragNDrop() && !FPSControl.IsOn()) return;

            BuildManager.ClearBuildPoint();

            isOn = false;
            thisObj.SetActive(isOn);
        }



        public static Transform piePosDummyT;
        public static List<Vector3> GetPieMenuPos(float num, Vector3 screenPos, float cutoff, int size)
        {
            List<Vector3> points = new List<Vector3>();

            if (num == 1)
            {
                points.Add(screenPos + new Vector3(0, 50, 0));
                return points;
            }

            //if there's only two button to be displayed, then normal calculation doesnt apply
            if (num <= 2)
            {
                points.Add(screenPos + new Vector3(50, 10, 0));
                points.Add(screenPos + new Vector3(-50, 10, 0));
                return points;
            }

            //create a dummy transform which we will use to do the calculation
            if (piePosDummyT == null) piePosDummyT = new GameObject().transform;

            int cutoffOffset = cutoff > 0 ? 1 : 0;

            //calculate the spacing of angle and distance of button from center
            float spacing = (float)((360f - cutoff) / (num - cutoffOffset));
            //float dist=Mathf.Max((num+1)*10, 50);
            float dist = 0.35f * num * size * UI.GetScaleFactor();

            piePosDummyT.rotation = Quaternion.Euler(0, 0, cutoff / 2);
            piePosDummyT.position = screenPos;//Vector3.zero;

            //rotate the dummy transform using the spacing interval, then sample the end point
            //these end point will be our button position
            for (int i = 0; i < num; i++)
            {
                Vector3 pos = piePosDummyT.TransformPoint(new Vector3(0, -dist, 0));
                points.Add(pos / UI.GetScaleFactor());
                piePosDummyT.Rotate(Vector3.forward * spacing);
            }

            //clear the dummy object
            //Destroy(piePosDummyT.gameObject);

            return points;
        }
    }

}