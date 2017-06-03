using UnitedSolution;using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution
{

    public class UIGameMessage : MonoBehaviour
    {

        public GameObject txtGameMessage;
        private Transform txtGameMessageT;

        public static UIGameMessage instance;

        private List<GameObject> msgList = new List<GameObject>();

        // Use this for initialization
        void Awake()
        {
            CanvasGroup cg = GetComponent<CanvasGroup>();
            cg.alpha = 1f;

            instance = this;
            txtGameMessageT = txtGameMessage.transform;
            txtGameMessage.SetActive(false);
        }


        void OnEnable()
        {
            GameControl.onGameMessageE += _DisplayMessage;
        }
        void OnDisabe()
        {
            GameControl.onGameMessageE -= _DisplayMessage;
        }


        public static void DisplayMessage(string msg) { instance._DisplayMessage(msg); }
        void _DisplayMessage(string msg)
        {
            if (txtGameMessage == null) return;

            int counter = msgList.Count;
            foreach (GameObject msgObj in msgList)
            {
                Vector3 pos = txtGameMessageT.localPosition + new Vector3(0, counter * 20, 0);
                TweenPosition(msgObj, .15f, pos);
                counter -= 1;
            }

            GameObject obj = (GameObject)Instantiate(txtGameMessage);
            obj.transform.SetParent(txtGameMessageT.parent);
            obj.transform.localPosition = txtGameMessageT.localPosition;
            obj.transform.localScale = txtGameMessageT.localScale;
            obj.GetComponent<Text>().text = msg;
            obj.SetActive(true);

            msgList.Add(obj);
            StartCoroutine(DestroyMessage(obj));
        }


        IEnumerator DestroyMessage(GameObject obj)
        {
            float dur = 0;
            while (dur < 1.25f) { dur += Time.unscaledDeltaTime; yield return null; }

            TweenScale(obj, 0.5f, new Vector3(0.01f, 0.01f, 0.01f));

            dur = 0;
            while (dur < 0.75f) { dur += Time.unscaledDeltaTime; yield return null; }

            msgList.RemoveAt(0);
            Destroy(obj);
        }



        void TweenPosition(GameObject obj, float duration, Vector3 targetPos)
        {
            StartCoroutine(_TweenPosition(obj, duration, targetPos));
        }
        IEnumerator _TweenPosition(GameObject obj, float duration, Vector3 targetPos)
        {
            Transform objT = obj.transform;
            Vector3 startPos = objT.localPosition;
            float dur = 0;
            while (dur < duration)
            {
                if (objT == null) yield break;
                objT.localPosition = Vector3.Lerp(startPos, targetPos, dur / duration);
                dur += Time.unscaledDeltaTime;
                yield return null;
            }
            if (objT != null) objT.localPosition = targetPos;
        }


        void TweenScale(GameObject obj, float duration, Vector3 targetScale)
        {
            StartCoroutine(_TweenScale(obj, duration, targetScale));
        }
        IEnumerator _TweenScale(GameObject obj, float duration, Vector3 targetScale)
        {
            Transform objT = obj.transform;
            Vector3 startScale = objT.localScale;
            float dur = 0;
            while (dur < duration)
            {
                if (objT == null) yield break;
                objT.localScale = Vector3.Lerp(startScale, targetScale, dur / duration);
                dur += Time.unscaledDeltaTime;
                yield return null;
            }
            if (objT != null) objT.localScale = targetScale;
        }



    }

}