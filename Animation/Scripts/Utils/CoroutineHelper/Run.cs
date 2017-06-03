using System;
using System.Collections;
using UnitedSolution;using UnityEngine;

namespace UnitedSolution
{
    public sealed class Run : SingletonBehaviour<Run>
    {

        public static CoroutineAction Coroutine(IEnumerator ie, Action onComplete = null)
        {
            return new CoroutineAction(Instance, Instance.StartCoroutine(DoCoroutine(ie, onComplete)), ie, onComplete);
        }

        /// <summary>
        /// Do action after wait
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="action"></param>
        public static CoroutineAction After(float seconds, Action action)
        {
            return new CoroutineAction(Instance, Instance.StartCoroutine(DoDelay(seconds, null, action)), action);
        }

        /// <summary>
        /// Do action before wait
        /// </summary>
        /// <param name="action"></param>
        /// <param name="seconds"></param>
        public static CoroutineAction Before(float seconds, Action beforeWaitAction, Action afterWaitAction)
        {
            return new CoroutineAction(Instance, Instance.StartCoroutine(DoDelay(seconds, beforeWaitAction, afterWaitAction)), afterWaitAction);
        }

        /// <summary>
        /// Do action like a progress
        /// </summary>
        /// <param name="action"></param>
        /// <param name="seconds"></param>
        public static CoroutineAction<float> Progress(float seconds, Action<float> onProgress)
        {
            return new CoroutineAction<float>(Instance, Instance.StartCoroutine(DoProgress(seconds, onProgress)), onProgress);
        }
        public static CoroutineAction Until(Func<bool> flag, Action onComplete)
        {
            return new CoroutineAction(Instance, Instance.StartCoroutine(DoUntil(flag, onComplete)), onComplete);
        }
        public static CoroutineAction Until(float delay, Func<bool> flag, Action onComplete)
        {
            return new CoroutineAction(Instance, Instance.StartCoroutine(DoUntil(delay, flag, onComplete)), onComplete);
        }
        public static CoroutineAction Every(Action action, Func<bool> stopFlag, float initDelay, float loopDelay)
        {
            return new CoroutineAction(Instance, Instance.StartCoroutine(DoEvery(action, stopFlag, initDelay, loopDelay)), action);
        }

        private static IEnumerator DoDelay(float seconds, Action before = null, Action after = null)
        {
            if (before != null)
            {
                before();
            }

            yield return new WaitForSeconds(seconds);

            if (after != null)
            {
                after();
            }
        }
        private static IEnumerator DoProgress(float seconds, Action<float> onProgress)
        {
            float step = seconds / 100f;
            float ellapsed = 0;
            for (int progress = 0; progress < 100; progress++)
            {
                yield return new WaitForSeconds(step);
                ellapsed += step;
                if (onProgress != null)
                {
                    onProgress(ellapsed / seconds);
                }
            }
            if (onProgress != null)
            {
                onProgress(1.0f);
            }
        }
        private static IEnumerator DoCoroutine(IEnumerator ie, Action onComplete = null )
        {
            yield return Instance.StartCoroutine(ie);

            if (onComplete != null)
            {
                onComplete();
            }
        } 
        private static IEnumerator DoUntil(Func<bool> flag, System.Action action)
        { 
            return DoUntil(0, flag, action);
        }
        private static IEnumerator DoUntil(float delay, Func<bool> flag, Action action)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);

            while (!flag())
            {
                yield return null;
            }

            action();
        }
        private static IEnumerator DoEvery(Action action, Func<bool> stopFlag, float initDelay, float loopDelay)
        {
            if (initDelay > 0)
            {
                yield return new WaitForSeconds(initDelay);
            }

            while (!stopFlag())
            {
                action();

                yield return new WaitForSeconds(loopDelay);
            }
        }
        private static IEnumerator DoWhenFlag(Func<bool> flagFunc, Action action)
        {
            while (!flagFunc())
            {
                yield return null;
            }

            action();
        }
    }
}