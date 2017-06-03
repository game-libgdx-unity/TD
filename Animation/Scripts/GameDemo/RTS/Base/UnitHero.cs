using System.Collections;
using System.Collections.Generic;
using UnitedSolution;using UnityEngine;
using UnityEngine.AI;

namespace UnitedSolution
{

    public class UnitHero : UnitTower
    {
        public bool controlable = true;
        public LayerMask customMask;

        public int moveSpeed = 2;
        public float rotateSpd = 10;

        private Animator m_Animator;
        public NavMeshAgent agent;
        private Rigidbody m_Rigidbody;

        protected override void _Awake()
        {
            base._Awake();

            if (stats.Count == 0)
            {
                stats.Add(new UnitStat());
            }

            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody>();
            agent = thisObj.GetComponent<NavMeshAgent>();
        }

        public override void InitTower(int ID)
        {
            base.InitTower(ID);

            subClass = _UnitSubClass.Hero;
            gameObject.layer = LayerManager.LayerTower();
        }

        void MoveToPoint(Vector3 target)
        {
            agent.destination = target;
            agent.stoppingDistance = .02f;
        }

        void MoveToAttack(Transform target, UnitStat stat)
        {
            agent.destination = target.position;
            agent.stoppingDistance = stat.attackRange * .9f;
        }

        protected override void ActivateRoutine()
        {
            if (controlable)
            {

            }
            else
            {
                base.ActivateRoutine();
            }
        }
    }
}