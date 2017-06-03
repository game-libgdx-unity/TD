using System;
using System.Collections;
using UnitedSolution;using UnityEngine;
using DG.Tweening;

namespace UnitedSolution
{
    public class LazyBehaviour : MonoBehaviour
    {

        [HideInInspector, NonSerialized]
        private Animation _animation;

        /// <summary>
        /// Gets the Animation attached to the object.
        /// </summary>
        public new Animation animation { get { return _animation ? _animation : (_animation = GetComponent<Animation>()); } }

        [HideInInspector, NonSerialized]
        private Collider _collider;

        /// <summary>
        /// Gets the Collider attached to the object.
        /// </summary>
        public new Collider collider { get { return _collider ? _collider : (_collider = GetComponent<Collider>()); } }
        
        [HideInInspector, NonSerialized]
        private ParticleSystem _particleSystem;

        /// <summary>
        /// Gets the ParticleSystem attached to the object.
        /// </summary>
        public new ParticleSystem particleSystem { get { return _particleSystem ? _particleSystem : (_particleSystem = GetComponent<ParticleSystem>()); } }

        [HideInInspector, NonSerialized]
        private Renderer _renderer;

        /// <summary>
        /// Gets the Renderer attached to the object.
        /// </summary>
        public new Renderer renderer { get { return _renderer ? _renderer : (_renderer = GetComponent<Renderer>()); } }

        [HideInInspector, NonSerialized]
        private Rigidbody _rigidbody;

        /// <summary>
        /// Gets the Rigidbody attached to the object.
        /// </summary>
        public new Rigidbody rigidbody { get { return _rigidbody ? _rigidbody : (_rigidbody = GetComponent<Rigidbody>()); } }

        /// <summary>
        /// Start a call after a delay
        /// </summary>
        /// <param name="time"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        protected Coroutine StartCoroutine(float time, Action action = null)
        {
            return StartCoroutine(RunAfterCoroutine(time, action));
        }
        private IEnumerator RunAfterCoroutine(float time, Action delayCall = null)
        {
            if(delayCall == null)
            {
                yield break;
            }
            if (time != 0)
            {
                yield return new WaitForSeconds(time);
            }
            delayCall();
        }
        /// <summary>
        /// Start a call after a delay
        /// </summary>
        /// <param name="time"></param>
        /// <param name="delayCall"></param>
        protected void DelayCall(float time, TweenCallback delayCall)
        {
            if (delayCall == null)
            {
                return;
            }
            if (time == 0)
            {
                delayCall();
            }
            DOVirtual.DelayedCall(time, delayCall);
        }
    }
}
