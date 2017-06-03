using System;
using System.Collections;
using System.Collections.Generic;
using UnitedSolution;using UnityEngine;

namespace UnitedSolution
{

    public class DissolveOut : MonoBehaviour
    {
        public float opacity = -0.2f;
        public float delayFadeOut = 1f;
        Material material;
        Unit unit;

        void Start()
        {
            material = GetComponentInChildren<Renderer>().sharedMaterial;
            unit = GetComponent<Unit>();
        }

        void OnEnable()
        {
            opacity = -0.2f;
            StartCoroutine(FadeIn());
            UnitCreep.onDestroyedE += UnitCreep_onDestroyedE;
        }

        private void UnitCreep_onDestroyedE(Unit unit)
        {
            if (this.unit == unit)
            {
                StartCoroutine(FadeOut());
            }
        }

        void Disable()
        {
            UnitCreep.onDestroyedE -= UnitCreep_onDestroyedE;
        }

        public float Opacity { get { return opacity; } set { opacity = value; material.SetFloat(Shader.PropertyToID("_Visible"), opacity); } }

        private IEnumerator FadeIn()
        {
            while (Opacity < 1.25f)
            {
                Opacity += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            } 
        }

        private IEnumerator FadeOut()
        {
            yield return new WaitForSeconds(delayFadeOut);
            while (Opacity > -.2f)
            {
                Opacity -= Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }
        }
    }

}