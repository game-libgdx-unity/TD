using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace UnitedSolution
{
    public static class Extensions
    {
        public static void DelayedRoutine(this Coroutine coroutine, MonoBehaviour mono, float delay, Action call)
        {
            DistinctRoutine(coroutine, mono, DelayRoutine(delay, call));
        }

        public static void Stop(this Coroutine coroutine, MonoBehaviour mono)
        {
            mono.StopCoroutine(coroutine);
        }

        public static void DistinctRoutine(this Coroutine coroutine, MonoBehaviour mono, IEnumerator routine)
        {
            if (coroutine != null)
            {
                mono.StopCoroutine(coroutine);
            }
            coroutine = mono.StartCoroutine(routine);
        }
        public static IEnumerator DelayRoutine(float delay, Action delayedCall)
        {
            yield return new WaitForSeconds(delay);
            if (delayedCall != null)
            {
                delayedCall();
            }
        }
        public static void DelayedCall(this Tween tweenDelay, float delay, Action start, Action delayedCall, bool callCallback = true)
        {
            if (tweenDelay != null)
            {
                tweenDelay.Complete(callCallback);
                tweenDelay = null;
            }
            if (start != null)
            {
                start();
            }

            tweenDelay = DOVirtual.DelayedCall(delay, null).OnComplete(() => delayedCall());
        }

        public static void SimulateUpdate(this Tween tweenSimulateUpdate, float updateFrequently, float duration = -1f,
               Action onStarted = null, Action onUpdated = null, Action onFinished = null)
        {
            if (tweenSimulateUpdate != null)
            {
                tweenSimulateUpdate.Kill();
                tweenSimulateUpdate = null;
            }

            if (onUpdated == null)
            {
                return;
            }

            if (onStarted != null)
            {
                onStarted();
            }

            if (duration <= 0)
            {
                tweenSimulateUpdate = DOVirtual.DelayedCall(updateFrequently, () =>
                {
                    onUpdated();
                }).SetLoops(-1).OnComplete(() => { if (onFinished != null) onFinished(); });
            }
            else
            {
                int loop = (int)(duration / updateFrequently);
                tweenSimulateUpdate = DOVirtual.DelayedCall(updateFrequently, () =>
                {
                    onUpdated();
                }).SetLoops(loop).OnComplete(() => { if (onFinished != null) onFinished(); });
            }
        }

        public static double NextDouble(this System.Random rand, double max)
        {
            return rand.NextDouble() * max;
        }

        public static double NextDouble(this System.Random rand, double min, double max)
        {
            return min + (rand.NextDouble() * (max - min));
        }

        public static Vector2[] ToVector2(this Vector3[] vec3)
        {
            Vector2[] output = new Vector2[vec3.Length];
            for (int i = 0; i < vec3.Length; i++)
            {
                output[i] = new Vector2(vec3[i].x, vec3[i].z);
            }
            return output;
        }

        public static Vector3 Average(this Vector3 vec3, Vector3 invec3)
        {
            return (vec3 + invec3) / 2f;
        }

        public static float MidX(this Rect rect)
        {
            return (rect.xMin + rect.width / 2f);
        }
        public static float MidY(this Rect rect)
        {
            return (rect.yMin + rect.height / 2);
        }
    }

}