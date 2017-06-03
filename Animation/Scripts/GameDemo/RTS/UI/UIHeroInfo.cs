using UnitedSolution;using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution
{

    public class UIHeroInfo : MonoBehaviour
    {

        public UnitHero currentHero;

        public static UIHeroInfo instance;
        private GameObject thisObj;
        public Transform anchorLeft;
        public Transform anchorRight;
        public Transform frameT;

        public Text txtName;

        void Start()
        {
            instance = this;
            thisObj = gameObject;
            Hide();
        }

        void OnEnable()
        {
        }
        void OnDisable()
        {
        }

        public static void Show(UnitHero hero) { instance._Show(hero); }
        private void _Show(UnitHero hero)
        {
            thisObj.SetActive(true);
            this.currentHero = hero;
            txtName.text = hero.unitName;
        }

        public static void Hide() { instance._Hide(); }
        private void _Hide()
        {
            thisObj.SetActive(false);
        }

    }

}