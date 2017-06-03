using System;
using System.Collections;
using System.Linq;
using UnitedSolution;using UnityEngine;

namespace UnitedSolution
{
    public abstract class BaseCoroutineAction
    {
        public float Progress { get; internal set; }

        protected Run run;
        protected Coroutine coroutine;
        protected Func<IEnumerator> routiner;
        
        internal BaseCoroutineAction(Run run, Coroutine coroutine)
        {
            this.run = run;
            this.coroutine = coroutine;
        }

        public abstract void Stop(bool invokeComplete);
        public void Start(IEnumerator routine)
        {
            routiner = () => routine;
            coroutine = run.StartCoroutine(routine);
        }
        public void Start(string MethodName)
        {
            coroutine = run.StartCoroutine(MethodName);
        }
        public void Start(string MethodName, object data)
        {
            coroutine = run.StartCoroutine(MethodName, data);
        }
        public bool Restart(bool InvokeComplete = false)
        {
            if (routiner != null)
            {
                Stop(InvokeComplete);
                Start(routiner());
                return true;
            }
            return false;
        }
    }

    public class CoroutineAction : BaseCoroutineAction
    {
        public Action OnComplete;

        internal CoroutineAction(Run run, Coroutine coroutine, Action OnComplete) : base(run, coroutine)
        {
            this.OnComplete = OnComplete;
        }

        internal CoroutineAction(Run run, Coroutine coroutine, IEnumerator routine, Action OnComplete) : base(run, coroutine)
        {
            this.routiner = () => routine;
            this.OnComplete = OnComplete;
        }

        public override void Stop(bool invokeComplete)
        {
            run.StopCoroutine(coroutine);

            if (invokeComplete)
            {
                if (OnComplete != null)
                {
                    OnComplete();
                }
            }
        }
    }

    public class CoroutineAction<T> : BaseCoroutineAction
    {
        public Action<T> OnComplete;

        internal CoroutineAction(Run run, Coroutine coroutine, Action<T> OnComplete) : base(run, coroutine)
        {
            this.OnComplete = OnComplete;
        }

        public override void Stop(bool invokeComplete)
        {
            run.StopCoroutine(coroutine);

            if (invokeComplete)
            {
                if (OnComplete != null)
                {
                    OnComplete(default(T));
                }
            }
        }
    }
}
