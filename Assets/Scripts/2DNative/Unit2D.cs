using System.Collections;
using System.Collections.Generic;
using UnitedSolution;
using UnityEngine;
using System;
using System.Linq;

namespace UnitedSolution
{
    public enum State
    {
        Idle,
        Chasing,
        Attacking,
        Dying,
    }

    public enum AttackType
    {
        Physics,
        Magic,
        Siege,
        Percior
    }

    public enum AmorType
    {
        Heavy,
        Light,
        Magic,
        Fortier,
    }

    public class Unit2D : MonoBehaviour
    {
        public static Action<Unit2D> OnUnitDead;
        protected const float AttackYThresold = .4f;
        protected const float CorpseRemovalTime = 2f;
        protected const float TakeDamageDelayTime = .12f;

        public string unitName = "Unit2d";
        public float moveSpeed = 1f;
        public float attackSpeed = 1f;
        public float attackRange = 10f;
        public float detectiveRange = 100f;
        public float amor = 1f;
        public float hp = 100f;
        public float max_hp = 100f;
        public float damage = 10;
        public Unit2D target;
        public AmorType amorType = AmorType.Light;
        public bool isFaceRight = true;
        public LayerMask enemyMask;
        private WaitForSeconds scanDelay = new WaitForSeconds(.1f);
        private WaitForSeconds attackDelay;
        public bool isAlive { get { return hp > 0; } }
        public bool isDead { get { return hp <= 0; } }
        public Transform targetPosition;
        public new Collider2D collider;
        public Animator animator;
        public new SpriteRenderer renderer;
        public Rigidbody2D rigidBody2D;
        public int SortingLayer { get { return int.MaxValue - Mathf.RoundToInt((10 * transform.position.y)); } }

        public float MoveSpeed
        {
            get
            {
                return moveSpeed * slowMultiplier;// * (isFaceRight ? 1 : -1);
            }
        }

        void Start()
        {
            hp = max_hp;
            attackDelay = new WaitForSeconds(attackSpeed);
            collider = GetComponent<Collider2D>();
            animator = GetComponent<Animator>();
            renderer = GetComponent<SpriteRenderer>();
            rigidBody2D = GetComponent<Rigidbody2D>();
            renderer.sortingOrder = SortingLayer;
        }
        void FixedUpdate()
        {
            animator.SetFloat("speed", Mathf.Abs(rigidBody2D.velocity.x));
        }

        protected Vector2 direct;
        protected float distanceToTarget;
        protected virtual void Update()
        {
            if (isDead)
                return;

            velocity = Vector2.zero;
            if (target != null && target.isAlive)
            {
                direct = target.transform.position - transform.position;
                distanceToTarget = direct.magnitude;
                if (direct.x < 0)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
                rigidBody2D.velocity = Vector2.zero;
                if (distanceToTarget > attackRange)
                {
                    velocity = new Vector2(direct.normalized.x * MoveSpeed, direct.normalized.y * MoveSpeed) * Time.deltaTime;
                    rigidBody2D.velocity = velocity;
                }
                else
                {
                    float distanceY = (transform.position.y - target.transform.position.y);
                    PlayAttackAnimation(distanceY);
                    time += Time.deltaTime;
                    if (time > attackSpeed)
                    {
                        time = -attackSpeed;
                        Fire();
                    }
                }
                renderer.sortingOrder = SortingLayer;
            }
            else
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectiveRange, enemyMask.value);
                if (colliders.Length > 0)
                {
                    Debug.Log("tg: " + colliders.Length);
                    float minDis = float.MaxValue;
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        var enemy = colliders[i].GetComponent<Unit2D>();
                        if (enemy && enemy.isAlive)
                        {
                            float dis = Vector2.Distance(enemy.GetTargetTransform().position, GetTargetTransform().position);
                            if (dis < minDis)
                            {
                                minDis = dis;
                                target = enemy;
                            }
                        }
                    }
                }
                else
                {
                    StopMoving();
                    PlayAnimation("Idle");
                }
            }
        }

        protected virtual void PlayAttackAnimation(float distanceY)
        {
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

        public Transform GetTargetTransform()
        {
            return targetPosition ? targetPosition : transform;
        }

        private void StopMoving(bool kinematic = true)
        {
            //rigidBody2D.bodyType = RigidbodyType2D.Static
            rigidBody2D.isKinematic = kinematic;
            rigidBody2D.angularDrag = 0f;
            rigidBody2D.velocity = Vector2.zero;
        }

        bool isAttacking = false;

        protected void PlayAnimation(string name)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(name))
            {
                animator.Play(name);
            }
        }

        float time;
        private Vector2 velocity;
        internal float hitThreshold;
        private float slowMultiplier = 1f;

        protected virtual void Fire()
        {
            target.TakeDamage(damage, () =>
            {
                target = null;
                time = attackSpeed * .5f;
            });
            if (!target || target.isDead)
            {
                isAttacking = false;
                target = null;
            }
        }

        public void TakeDamage(float damage, Action OnTargetDead = null)
        {

            if (hp > 0)
            {
                hp -= damage;
                if (hp <= 0f)
                {
                    if (OnTargetDead != null)
                        OnTargetDead();

                    StopMoving();
                    renderer.sortingOrder = -renderer.sortingOrder;
                    animator.Play("Die");
                    StopCoroutine(TakeDamageCoroutine(damage));
                    StartCoroutine(TakeDamageCoroutine(damage));
                    return;
                }
            }
        }

        public void StopAndStartCoroutine(IEnumerator routine)
        {
            StopCoroutine(routine);
            StartCoroutine(routine);
        }
        public void TakeEffect(Slow slow)
        {
            if (this.slowMultiplier > slow.slowMultiplier)
            {
                slowRoutine.DistinctRoutine(this, SlowRoutine(slow.slowMultiplier, slow.duration)); //override with new 
            }
        }
        Coroutine slowRoutine;
        private IEnumerator SlowRoutine(float slowMultiplier, float duration)
        {
            this.slowMultiplier = slowMultiplier;
            print("Start slow: " + MoveSpeed);
            yield return new WaitForSeconds(duration);
            slowMultiplier = 1f;
            print("End slow: " + moveSpeed);
        }

        protected virtual IEnumerator TakeDamageCoroutine(float damage)
        {
            yield return new WaitForSeconds(.1f);
            renderer.sortingOrder *= -1;

            yield return new WaitForSeconds(CorpseRemovalTime);
            Destroy(gameObject);
        }

        protected void OnStateLeft(State state)
        {

        }
    }
}