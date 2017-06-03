using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UTJ;
using DG.Tweening;

namespace RobotDemo
{
    public class RobotUnit : MonoBehaviour
    {
        public enum UnitType
        {
            Ally,
            Enemy
        }
        public UnitType type;
        public Transform targetT;
        public Transform turretT;
        public Transform[] shootingT;
        public GameObject[] undestroyedParts;
        public float bulletSpeed = 80f;
        public float range = 2.5f;
        public RobotUnit target;
        public float turretRotateSpeed = 1;
        public float speed = 3;
        public bool IsMoving;
        public float hp;
        public bool IsAlive { get { return hp > 0; } }
        private new Animation animation;
        private int enemyCount;
        private Tweener moveTweener = null;
        private bool IsAttacking;

        // Use this for initialization
        void Start()
        {
            animation = GetComponent<Animation>();
            StartCoroutine(ScanAndAttackTarget());
            StartCoroutine(Moving());
        }

        IEnumerator Moving()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(2f, 5f));
                yield return new WaitUntil(() => !IsAttacking);

                if (target)
                {
                    Vector3 direction = speed * (target.targetT.position - targetT.position).normalized;
                    moveTweener = transform.DOLocalMove(transform.position + direction, 2f)
                        .OnStart(() => { animation.Play("walkForward"); IsMoving = true; })
                        .OnComplete(() => { animation.Stop("walkForward"); IsMoving = false; })
                        .OnUpdate(() =>
                        {
                            turretT.localEulerAngles = new Vector3(turretT.localEulerAngles.x, 0, 0);
                            if (!target)
                            {
                                moveTweener.Complete(true);
                            }
                            if (IsAttacking)
                            {
                                moveTweener.Kill();
                                animation.Stop("walkForward");
                                IsMoving = false;
                            }
                        });

                    //for(int i = 0; i < 120;i++)
                    //{
                    //    transform.Translate(direction.x * .1f, 0, 0);
                    //    yield return null;
                    //}



                    //yield return moveTweener.WaitForCompletion();
                }
                else
                    yield return null;
            }
        }

        IEnumerator ScanAndAttackTarget()
        {


            while (true)
            {
                while (!target || !target.IsAlive)
                {
                    yield return null;
                    List<RobotUnit> targets = FindObjectsOfType<RobotUnit>().Where(r => r && r.IsAlive && r.type != type).ToList();
                    enemyCount = targets.Count;
                    if (enemyCount > 0)
                    {
                        int random = Random.Range(0, enemyCount);
                        this.target = targets[random];
                    }
                    else
                    {
                        target = null;
                        break;
                    }

                    Quaternion lastRotation = turretT.localRotation;
                    if (target)
                    {
                        Quaternion wantedRot = Quaternion.LookRotation(target.targetT.position - turretT.position);
                        yield return turretT.DORotateQuaternion(wantedRot, .5f).OnUpdate(() =>
                        {
                            turretT.localEulerAngles = new Vector3(turretT.localEulerAngles.x, 0, 0);
                        }).OnStart(() => IsAttacking = true).WaitForCompletion();
                    }
                }

                //if (moveTweener != null)
                //    moveTweener.Kill(true);

                //turretT.DOLookAt(target.targetT.position - targetT.position, .5f,AxisConstraint.X );
                if (target)
                {
                    IsAttacking = false;
                    yield return new WaitForSeconds(Random.Range(.6f, 3f));
                    IsAttacking = true;

                    if (!IsAlive || enemyCount == 0)
                        yield return null;

                    yield return new WaitUntil(() => !IsMoving);

                    var offsetRange = 3f;

                    for (int i = 0; i < 10; i++) //shoot 10 bullets
                    {
                        if (target)
                        {
                            var offset = new Vector3(Random.Range(-offsetRange, offsetRange),
                                                 Random.Range(-offsetRange, offsetRange),
                                                 Random.Range(-offsetRange, offsetRange));
                            Vector3 targetPostion = target.targetT.position + offset;
                            Vector3 bulletVelocity = bulletSpeed * (targetPostion - this.targetT.position).normalized;
                            Beamer.Instance.Shoot(shootingT[i % shootingT.Length].position, bulletVelocity, target, 10 /*damage*/);
                            yield return new WaitForSeconds(.07f);
                        }
                        else
                        {
                            break;
                        }
                    }

                    //Quaternion resetRotatation = lastRotation;
                    //yield return turretT.DORotateQuaternion(resetRotatation, .5f).OnUpdate(() =>
                    //{
                    //    turretT.localEulerAngles = new Vector3(turretT.localEulerAngles.x, 0, 0);
                    //}).WaitForCompletion();

                    IsAttacking = false;
                }
            }
        }

        int shieldRemainning = 15;
        internal void TakeDamage(Vector3 pos, int damage)
        {
            if (IsAlive)
            {
                if (shieldRemainning > 0)
                {
                    shieldRemainning--;
                    OpenShield(pos);
                    return;
                }
                print("Damaged");
                hp -= damage;

                HahenRenderer.Instance.Invoke(pos);
                ExplosionTest.Instance.Invoke(pos);
                if (hp <= 0)
                {
                    StopAllCoroutines();
                    Vector3 position = targetT.position;
                    ExplosionTest.Instance.Invoke(position);
                    HahenRenderer.Instance.Invoke(position);
                    StartCoroutine(transform.GetChild(0).gameObject.SplitMesh(max_triangle: 100));
                    foreach (GameObject go in undestroyedParts)
                    {
                        go.AddComponent<BoxCollider>();
                        Vector3 explosionPos = new Vector3(go.transform.position.x + Random.Range(-0.5f, 0.5f), go.transform.position.y + Random.Range(0f, 0.5f), go.transform.position.z + Random.Range(-0.5f, 0.5f));
                        go.AddComponent<Rigidbody>().AddExplosionForce(Random.Range(100, 200), explosionPos, 5);
                        GameObject.Destroy(go, 5 + Random.Range(0.0f, 5.0f));
                    }
                }
            }
        }

        // Update is called once per frame
        void OpenShield(Vector3 pos)
        {
            Shield.Instance.begin();
            var target = targetT.position;
            Shield.Instance.spawn(ref pos,
                                  ref target,
                                  Time.time,
                                  (type == UnitType.Ally ?
                                   Shield.Type.Green :
                                   Shield.Type.Red));
            Shield.Instance.end(0 /* front */);
        }
    }
}