using System.Collections;
using System.Collections.Generic;
using UnitedSolution;
using UnityEngine;
using DG.Tweening;

namespace UnitedSolution
{
    public class RangedUnit2D : Unit2D
    {
        public GameObject shootObject;
        public Transform shootPosition;
        public float meleeAttackDistance = .8f;
        void Awake()
        {
            if (shootObject) ObjectPoolManager.New(shootObject.gameObject);
        }

        protected override void PlayAttackAnimation(float distanceY)
        {
            if (distanceToTarget > meleeAttackDistance)
            {
                base.PlayAttackAnimation(distanceY);
            }
            else
            {
                if (Mathf.Abs(distanceY) < AttackYThresold)
                {
                    PlayAnimation("Attack_Melee");
                }
                else if (distanceY > AttackYThresold)
                {
                    PlayAnimation("Attack_Melee_Down");
                }
                else if (distanceY < -AttackYThresold)
                {
                    PlayAnimation("Attack_Melee_Up");
                }
            }
        }

        protected override void Fire()
        {
            if (distanceToTarget < meleeAttackDistance)
            {
                base.Fire();
            }
            else
            {
                StartCoroutine(FireCoroutine());
            }
        }

        private IEnumerator FireCoroutine()
        {
            //yield return new WaitForSeconds(.1f);
            ShootObject2D arrow = ObjectPoolManager.Spawn(shootObject).GetComponent<ShootObject2D>();
            arrow.transform.position = shootPosition.position;
            print("arrow.Shoot");
            AttackInstance2D attInstance = new AttackInstance2D();
            attInstance.srcUnit = this;
            attInstance.tgtUnit = target;
            attInstance.Process();
            arrow.Shoot(attInstance, shootPosition);
            yield return null;
            //arrow.transform.DOLocalMove(target.targetPosition.position, .5f).OnComplete(() => { base.Fire(); Destroy(arrow.gameObject); });
        }
    }
}