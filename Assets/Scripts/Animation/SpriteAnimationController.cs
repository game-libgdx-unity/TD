using System;
using System.Collections;
using System.Collections.Generic;
using UnitedSolution;using UnityEngine;
using UnityEngine.AI;

namespace UnitedSolution
{
    public class SpriteAnimationController : BaseAnimationController
    {
        public float AttackYThresold = .1f;
        void PlayAnimation(string name)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(name))
            {
                animator.Play(name);
            }
        }
        public override void PlayRunAnimation()
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
        
        public override void PlayAttackAnimation()
        {
            if (animator)
            {
                float distanceY = (transform.position.z - unit.target.transform.position.z);
                if (Mathf.Abs(distanceY) < AttackYThresold)
                {
                    PlayAnimation("Attack");
                }
                else if (distanceY > AttackYThresold)
                {
                    PlayAnimation("Attack_Down");
                }
                else if (distanceY < -AttackYThresold)
                {
                    PlayAnimation("Attack_Up");
                }
            }
            else if (animation)
            {
                animation.Play("Attack");
            }
        }

        //protected override void OnAnimatorPlayDeathAnimation()
        //{
        //    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Die"))
        //    {
        //        animator.Play("Die");
        //    }
        //}

        public override void OnUnitHitted()
        {
            if (animator)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
                {
                    animator.Play("Hit");
                }
            }
            else if (animation)
            {
                animation.Play(hittedTrigger);
            }
        }

        internal override void PlayIdleAnimation()
        {
            if (animator)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    animator.Play("Idle");
                }
            }
            else if (animation)
            {
                animation.Play("Idle");
            }
        }
    }
}