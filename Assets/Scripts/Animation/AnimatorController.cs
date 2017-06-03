using System.Collections;
using System.Collections.Generic;
using UnitedSolution;using UnityEngine;
using UnityEngine.AI;

namespace UnitedSolution
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    public class AnimatorController : BaseAnimationController
    {
        public int AttackTypeCount = 1;
        private readonly int AttackTypeHash = Animator.StringToHash("AttackType");
        private readonly int ForwardHash = Animator.StringToHash("Forward");
        void Update()
        {
            UpdateAnimator();
        }

        public override void PlayAttackAnimation()
        {
            if (AttackTypeCount > 1)
            {
                animator.SetInteger(AttackTypeHash, Random.Range(0, AttackTypeCount));
                animator.SetFloat(ForwardHash, 0f, 0.1f, Time.deltaTime);
                rigidBody.velocity = Vector3.zero;
            }

            base.PlayAttackAnimation();
        }

        void UpdateAnimator()
        {
            // update the animator parameters
            float forward = 0f;

            if  (agent.remainingDistance > agent.stoppingDistance)
            {
                forward = Mathf.InverseLerp(0, agent.velocity.magnitude, agent.desiredVelocity.magnitude);
            }

            animator.SetFloat(ForwardHash, forward, 0.1f, Time.deltaTime);
        }
    }
}