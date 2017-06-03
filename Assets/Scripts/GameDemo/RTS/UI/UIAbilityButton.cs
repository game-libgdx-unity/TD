using UnitedSolution;using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution
{

    public class UIAbilityButton : MonoBehaviour
    {
        public List<UnityButton> buttonList = new List<UnityButton>();

        public RectTransform energyRect;
        public Text txtEnergy;

        public GameObject tooltipObj;
        public Text txtTooltipName;
        public Text txtTooltipCost;
        public Text txtTooltipDesp;

        public static UIAbilityButton instance;
        private GameObject thisObj;

        void Awake()
        {
            instance = this;
            thisObj = gameObject;
        }

        // Use this for initialization
        void Start()
        {
            tooltipObj.SetActive(false);

            if (!AbilityManager.IsOn())
            {
                Hide();
                return;
            }
            BuildAbilityButtons(AbilityManager.GetAbilityList());
        }

        private void BuildAbilityButtons(List<Ability> abList)
        {
            //buttonList.Clear();
            if (abList.Count == 0)
            {
                Hide();
                return;
            }

            EventTrigger.Entry entryRequireTargetSelect = SetupTriggerEntry(true);
            EventTrigger.Entry entryDontRequireTargetSelect = SetupTriggerEntry(false);

            for (int i = 0; i < abList.Count; i++)
            {
                if (i == 0) buttonList[0].Init();
                else if (i > 0)
                {
                    buttonList.Add(buttonList[0].Clone("button" + (i + 1), new Vector3(i * 55, 0, 0)));
                }
                abList[i].abilityButton = buttonList[i];
                buttonList[i].imageIcon.sprite = abList[i].icon;
                buttonList[i].label.text = "";

                EventTrigger trigger = buttonList[i].rootObj.GetComponent<EventTrigger>();
                if (abList[i].requireTargetSelection) trigger.triggers.Add(entryRequireTargetSelect);
                else trigger.triggers.Add(entryDontRequireTargetSelect);
            }

            float offset = 0.5f * (buttonList.Count - 1) * 55;
            for (int i = 0; i < buttonList.Count; i++)
            {
                buttonList[i].rootT.localPosition -= new Vector3(offset, 0, 0);
            }
        }

        private EventTrigger.Entry SetupTriggerEntry(bool requireTargetSelection)
        {
            UnityEngine.Events.UnityAction<BaseEventData> call = new UnityEngine.Events.UnityAction<BaseEventData>(OnAbilityButton);
            EventTriggerType eventID = EventTriggerType.PointerClick;
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
				if(requireTargetSelection) eventID=EventTriggerType.PointerDown;
#endif

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = eventID;
            entry.callback = new EventTrigger.TriggerEvent();
            entry.callback.AddListener(call);

            return entry;
        }


        void OnEnable()
        {
            AbilityManager.onAbilityActivatedE += OnAbilityActivated;
        }

        void OnDisable()
        {
            AbilityManager.onAbilityActivatedE -= OnAbilityActivated;
            //GameControl.onTowerSelected -= GameControl_onTowerSelected;
        }


        public static void AddNewAbility(Ability newAbility) { instance._AddNewAbility(newAbility); }
        public void _AddNewAbility(Ability newAbility)
        {
            int count = buttonList.Count;
            buttonList.Add(buttonList[count - 1].Clone("button" + (count), new Vector3(60, 0, 0)));
            buttonList[count].imageIcon.sprite = newAbility.icon;
            newAbility.abilityButton = buttonList[count];

            EventTrigger trigger = buttonList[count].rootObj.GetComponent<EventTrigger>();
            if (newAbility.requireTargetSelection)
            {
                EventTrigger.Entry entryRequireTargetSelect = SetupTriggerEntry(true);
                trigger.triggers.Add(entryRequireTargetSelect);
            }
            else
            {
                EventTrigger.Entry entryDontRequireTargetSelect = SetupTriggerEntry(false);
                trigger.triggers.Add(entryDontRequireTargetSelect);
            }
        }

        public static void AlignButtonBegin()
        {
            float offset = 0.5f * (instance.buttonList.Count - 1) * 55;
            for (int i = 0; i < instance.buttonList.Count; i++)
            {
                if (instance.buttonList[i].rootObj.activeSelf)
                    instance.buttonList[i].rootT.localPosition += new Vector3(offset, 0, 0);
            }
        }

        public static void AlignButtonEnd()
        {
            float offset = 0.5f * (instance.buttonList.Count - 1) * 55;
            for (int i = 0; i < instance.buttonList.Count; i++)
            {
                if (instance.buttonList[i].rootObj.activeSelf)
                    instance.buttonList[i].rootT.localPosition -= new Vector3(offset, 0, 0);
            }
        }

        public void OnAbilityButton(UnityEngine.EventSystems.BaseEventData baseEvent)
        {
            OnAbilityButton(baseEvent.selectedObject);
        }
        public void OnAbilityButton(GameObject butObj)
        {
            //if(FPSControl.IsOn()) return;

            //in drag and drop mode, player could be hitting the button while having an active tower selected
            //if that's the case, clear the selectedTower first. and we can show the tooltip properly
            if (UI.UseDragNDrop() && GameControl.GetSelectedTower() != null)
            {
                UI.ClearSelectedTower();
                return;
            }

            UI.ClearSelectedTower();

            int ID = GetButtonID(butObj);

            string exception = AbilityManager.SelectAbility(ID);
            if (exception != "") UIGameMessage.DisplayMessage(exception);

            //Hide();
        }
        public void OnHoverAbilityButton(GameObject butObj)
        {
            //if(FPSControl.IsOn()) return;


            if (GameControl.GetSelectedTower() != null) return;

            int ID = GetButtonID(butObj);
            Ability ability = AbilityManager.GetAbilityList()[ID];

            txtTooltipName.text = ability.name;
            txtTooltipCost.text = "Cost:" + ability.cost;
            txtTooltipDesp.text = ability.GetDesp();

            tooltipObj.SetActive(true);
        }
        public void OnExitHoverAbilityButton(GameObject butObj)
        {
            tooltipObj.SetActive(false);
        }
        int GetButtonID(GameObject butObj)
        {
            for (int i = 0; i < buttonList.Count; i++)
            {
                if (buttonList[i].rootObj == butObj) return i;
            }
            return 0;
        }

        void OnAbilityActivated(Ability ab) { StartCoroutine(ButtonCDROutine(ab)); }
        IEnumerator ButtonCDROutine(Ability ab)
        {
            int ID = ab.ID;
            buttonList[ID].imageIcon.color = new Color(.125f, .125f, .125f, 1);

            if (ab.usedCount == ab.maxUseCount)
            {
                buttonList[ID].label.text = "Used";
                yield break;
            }


            while (true)
            {
                string text = "";
                float duration = ab.currentCD;
                if (duration <= 0) break;

                if (duration > 60) text = Mathf.Floor(duration / 60).ToString("F0") + "m";
                else text = (Mathf.Ceil(duration)).ToString("F0") + "s";
                buttonList[ID].label.text = text;
                yield return new WaitForSeconds(0.1f);
            }
            buttonList[ID].imageIcon.color = new Color(1, 1, 1, 1);
            buttonList[ID].label.text = "";
        }

        // Update is called once per frame
        void Update()
        {
            txtEnergy.text = AbilityManager.GetEnergy().ToString("f0") + "/" + AbilityManager.GetEnergyFull().ToString("f0");
            float valueX = Mathf.Clamp(AbilityManager.GetEnergy() / AbilityManager.GetEnergyFull() * 200, 4, 200);
            float valueY = Mathf.Min(valueX, 8);
            energyRect.sizeDelta = new Vector2(valueX, valueY);
        }

        public static bool isOn = true;
        public static void Show() { instance._Show(); }
        public void _Show()
        {
            isOn = true;
            thisObj.SetActive(isOn);
        }
        public static void Hide() { instance._Hide(); }
        public void _Hide()
        {
            isOn = false;
            thisObj.SetActive(isOn);
        }




    }

}