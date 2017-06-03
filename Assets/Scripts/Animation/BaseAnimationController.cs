using System;
using System.Collections;
using System.Collections.Generic;
using UnitedSolution;using UnityEngine;
using UnityEngine.AI;

namespace UnitedSolution
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Unit))]
    public class BaseAnimationController : MonoBehaviour
    {
        private int hashAttackTrigger;
        public string attackTrigger = "Attack";
        private int hashDeadTrigger;
        public string deadTrigger = "die";
        private int hashHittedTrigger;
        public string hittedTrigger = "hit";
        public string animIdle = "Idle";
        public float removalDelay = 3f;
        public float attackDelay = 0f;

        public Animator animator;
        protected NavMeshAgent agent;
        protected Rigidbody rigidBody;
        protected Unit unit;
        protected new Animation animation;

        protected virtual void Start()
        {
            unit = GetComponent<Unit>();
            animator = FindComponent<Animator>();
            hashAttackTrigger = Animator.StringToHash(attackTrigger);
            hashDeadTrigger = Animator.StringToHash(deadTrigger);
            hashHittedTrigger = Animator.StringToHash(hittedTrigger);
            rigidBody = GetComponent<Rigidbody>();
            agent = GetComponent<NavMeshAgent>();

            PlayRunAnimation();
        }

        private T FindComponent<T>() where T : Component
        {
            T com = GetComponent<T>();
            if (!com)
            {
                com = FindComponentInChildren<T>(transform);
            }
            return com;
        }

        protected T FindComponentInChildren<T>(Transform tramsform) where T:Component
        {
            T component = tramsform.GetComponentInChildren<T>();
            if (!component)
            {
               foreach(Transform child in transform)
                {
                    component = FindComponentInChildren<T>(child);
                    if (component)
                        break;
                }
            }
            return component;
        }
        public virtual void PlayRunAnimation()
        {
            if (animator)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                {
                    animator.Play("Run");
                }
            }
            else if (animation)
            {
                animation.Play("Run");
            }
        }

        public virtual void PlayAttackAnimation()
        {

            if (animator)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    animator.Play("Attack");
                }
            }
            else if (animation)
            {
                animation.Play("Attack");
            }
            else if (animator)
            {
                animator.SetTrigger(hashAttackTrigger);
                //print(unit.unitName + "attacking");
            }
        }

        IEnumerator RemoveAnimation()
        {
            yield return new WaitForSeconds(.85f);
            this.animation.Stop();
            this.animation.enabled = false;
        }

        public virtual void PlayDeathAnimation()
        {
            //print(unit.unitName + " died");
            if (animator)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Die"))
                {
                    animator.Play("Die");
                }
            }
            else
           if (animation)
            {
                animation.Play(deadTrigger);
                StartCoroutine(RemoveAnimation());
            }
        }

        public virtual void OnAttackTargetStopped()
        {
            if (animation)
            {
            }
        }

        public virtual void OnUnitHitted()
        {
            if (animation)
            {
                animation.Play(hittedTrigger);
            }
            else
            if (animator)
            {
                animator.SetTrigger(hashHittedTrigger);
            }
        }

        public virtual void stopAnimation()
        {
            if (animation)
                animation.Stop();
        }

        internal virtual void PlayIdleAnimation()
        {
            if (animator)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    animator.Play("Idle");
                }
            }
            else
           if (animation)
            {
                animation.Play("Idle");
            }
        }
    }
}