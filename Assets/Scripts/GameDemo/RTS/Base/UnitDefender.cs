using UnitedSolution;using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnitedSolution;
using UnityEngine.AI;

namespace UnitedSolution
{
    public class UnitDefender : UnitTower
    {
        public int moveSpeed = 2;
        public float rotateSpd = 10;
        public bool allowWandering = false;
        public float timeToNextMove = 5f;
        public float maxDistance = 2f;
        public float evasionRange = 2;
        private Animator m_Animator;
        private NavMeshAgent agent;
        private Rigidbody m_Rigidbody;
        private float timer;
        protected override void Wandering()
        {
            if (!dead && !stunned && allowWandering)
            {
                timer += Time.deltaTime;

                if (timer >= timeToNextMove)
                {
                    Vector3 newPos = RandomNavSphere(transform.position, maxDistance, -1);
                    agent.SetDestination(newPos);
                    timer = 0;
                }
            }
            else
            {
                if (dead)
                {
                    animController.PlayDeathAnimation();
                }
                else if (behaviour == Behaviour.Stational)
                    animController.PlayIdleAnimation();
                else
                    animController.PlayRunAnimation();
            }
        }
        public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
        {
            Vector3 randDirection = Random.insideUnitSphere * dist;
            randDirection += origin;
            NavMeshHit navHit;
            NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
            return navHit.position;
        }

        public override void InitTower(int ID)
        {
            base.InitTower(ID);
            realAttackRange = 0.5f;
            InitNavMesh();
            if(behaviour == Behaviour.Stational)
            {
                allowWandering = false;
            }
            else if (behaviour == Behaviour.TacticallyMove)
            {
                targetPriority = _TargetPriority.Nearest;
                if (evasionRange <= 0)
                    evasionRange = GetAttackRange() * .5f;
            }
            if (useNavMesh)
            {
                agent.speed = moveSpeed;
                agent.angularSpeed = rotateSpd;
            }
            //if (needInverseRotation)
            //    transform.localRotation = Quaternion.Euler(0, 180, 0);
        }

        protected override void rotateTurretBack()
        {
            //turretObject.rotation = Quaternion.Slerp(turretObject.rotation, Quaternion.Euler(-90, 0, 0), turretRotateSpeed * Time.deltaTime * 0.25f);
        } // rotateTurretBack
        protected override IEnumerator AttackTarget(Unit unit)
        {
            float distance = Vector3.Distance(GetTargetT().position, target.GetTargetT().position);
            if (useNavMesh && distance <= GetAttackRange())
                yield return base.AttackTarget(unit);
            else
                yield return null;
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            float attackRange = GetAttackRange();
            float realStopDistance = attackRange * attackRange < 2f ? .65f : .90f;
            if (target && !stunned && !IsInConstruction() && behaviour != Behaviour.Stational)
            {
                if (runningAway)
                {
                    animController.PlayRunAnimation();

                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        StopRunningAway();
                    }
                }
                else if (behaviour == Behaviour.TacticallyMove)
                {
                    float distance = Vector3.Distance(GetTargetT().position, target.GetTargetT().position);
                    if (distance < evasionRange && !target.dead)
                    {
                        StartRunningAway();
                    }
                }
                else if (behaviour == Behaviour.Berserk)
                {
                    if (useNavMesh)
                    {
                        if (dirtySlow)
                            agent.speed = GetMoveSpeed();

                        if (target != null && !target.dead)
                        {
                            agent.stoppingDistance = realStopDistance;
                            agent.destination = target.GetTargetT().position;
                        }
                        else
                        {
                            agent.destination = thisT.position;
                            agent.stoppingDistance = .1f;
                        }
                    }
                    else if (target)
                    {
                        MoveToPoint(target.transform.position);
                    }
                }
            }
        }

        private void StartRunningAway()
        {
            Vector3 direction = target.GetTargetT().position - GetTargetT().position;
            agent.destination = GetTargetT().position - direction.normalized * evasionRange;
            agent.stoppingDistance = .1f;
            runningAway = true;
            animController.PlayRunAnimation();
        }

        private void StopRunningAway()
        {
            animController.PlayRunAnimation();
            target = ScanForTarget(CurrentStat.customMask, GetAttackRange());
            if (target)
            {
                agent.destination = target.GetTargetT().position;
                agent.stoppingDistance = stats[0].attackRange;
            }
            StartCoroutine(StopRunAnim());
        }
        IEnumerator StopRunAnim()
        {
            yield return new WaitForSeconds(.1f);
            runningAway = false;
        }
        public void MoveToPoint(Vector3 point)
        {
            float dist = Vector3.Distance(point, thisT.position);

            //if the unit have reached the point specified
            if (dist < 0.1f) return;

            //rotate towards destination
            if (moveSpeed > 0)
            {
                Quaternion wantedRot = Quaternion.LookRotation(point - thisT.position);
                thisT.rotation = Quaternion.Slerp(thisT.rotation, wantedRot, rotateSpd * Time.deltaTime);
            }

            //move, with speed take distance into accrount so the unit wont over shoot
            Vector3 dir = (point - thisT.position).normalized;
            thisT.Translate(dir * Mathf.Min(dist, moveSpeed * slowMultiplier * Time.fixedDeltaTime), Space.World);
        }

        public void InitNavMesh()
        {
            //Init();
            realAttackRange = 0f;
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody>();
            agent = thisObj.GetComponent<NavMeshAgent>();
            if (agent)
            {
                useNavMesh = true;
                agent.speed = GetMoveSpeed();
                agent.destination = thisT.position;
            }
        }
        public float GetMoveSpeed() { return moveSpeed * slowMultiplier; }
        bool useNavMesh;
    }
}
