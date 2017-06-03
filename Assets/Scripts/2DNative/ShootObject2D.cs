using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

namespace UnitedSolution
{
    public interface ICloneable<T>
    {
        T Clone();
    }
    [System.Serializable]
    public class SpellEffect : ICloneable<SpellEffect>
    {
        public float damage;
        public Critical crit;
        public Stun stun;
        public Slow slow;
        public Dot dot;             //damage over time
        public InstantKill instantKill;
        public Buff buff;               //for support tower

        public SpellEffect()
        {
            slow = new Slow(.5f, 10f);
        }

        public SpellEffect Clone()
        {
            SpellEffect spellEffect = new SpellEffect();
            spellEffect.damage = damage;

            return spellEffect;
        }
    }
    [System.Serializable]
    public class Spell
    {
        public LayerMask customMask = -1;
        public float effectRange;

        public List<SpellEffect> effects = new List<SpellEffect>();

        public string spellName;
    }

    public class ShootObject2D : MonoBehaviour
    {
        public AttackInstance2D attInstance;
        public GameObject shootEffect;
        public GameObject hitEffect;
        public _ShootObjectType type;
        public bool updateTargetPosition = true;
        public Unit2D target;
        public float speed = 30f;
        public float shootDelay = .1f;
        public float effectDelay;

        private Vector3 targetPos;
        private float hitThreshold;
        private bool hit;
        private Transform shootPoint;
        private float speed_cached;
        private new SpriteRenderer renderer;
        public List<LineRenderer> lineList = new List<LineRenderer>();
        private List<TrailRenderer> trailList = new List<TrailRenderer>();

        public Spell spell;

        void Awake()
        {
            if (shootEffect != null) ObjectPoolManager.New(shootEffect);
            if (hitEffect != null) ObjectPoolManager.New(hitEffect); 

            renderer = GetComponent<SpriteRenderer>();

            if (type == _ShootObjectType.Beam)
            {
                LineRenderer[] lines = GetComponentsInChildren<LineRenderer>(true);
                for (int i = 0; i < lines.Length; i++) lineList.Add(lines[i]);
            }

            TrailRenderer[] trails = GetComponentsInChildren<TrailRenderer>(true);
            for (int i = 0; i < trails.Length; i++) trailList.Add(trails[i]);

            speed_cached = speed;
        }

        IEnumerator ClearTrail(TrailRenderer trail)
        {
            if (trail == null) yield break;
            float trailDuration = trail.time;
            trail.time = -1;
            yield return null;
            trail.time = trailDuration;
        }

        void OnEnable()
        {
            for (int i = 0; i < trailList.Count; i++) StartCoroutine(ClearTrail(trailList[i]));
            speed = speed_cached;
            transform.rotation = Quaternion.identity;
        }
        public void Shoot(AttackInstance2D attInst = null, Transform sp = null)
        {
            if (attInst.tgtUnit == null || attInst.tgtUnit.GetTargetTransform() == null)
            {
                ObjectPoolManager.Unspawn(gameObject);
                return;
            }

            attInstance = attInst;
            target = attInstance.tgtUnit;
            targetPos = target.GetTargetTransform().position;
            hitThreshold = Mathf.Max(.1f, target.hitThreshold);
            shootPoint = sp;

            if (type != _ShootObjectType.Beam)
            {
                if (shootPoint.position.x > targetPos.x)
                {
                    renderer.flipX = true;
                }
                else
                {
                    renderer.flipX = false;
                }
            }

            //if (shootPoint != null) transform.rotation = shootPoint.rotation;
            if (shootEffect != null && target.isAlive)
            {
                ObjectPoolManager.Spawn(shootEffect, transform.position, transform.rotation);
            }
            hit = false;

            print("Shoot");
            if (type == _ShootObjectType.Projectile) StartCoroutine(ProjectileRoutine());
            else if (type == _ShootObjectType.Beam) StartCoroutine(BeamRoutine());
            else if (type == _ShootObjectType.Bullet) StartCoroutine(BulletRoutine());
            else if (type == _ShootObjectType.Missile) StartCoroutine(MissileRoutine());
            else if (type == _ShootObjectType.Effect && effectDelay > 0) StartCoroutine(EffectRoutine()); else Hit();
        }

        private void Hit()
        {
            hit = true;

            if (hitEffect != null)
            {
                print("show hit effect  "); ObjectPoolManager.Spawn(hitEffect, targetPos, Quaternion.identity);
            }

            if (spell.effects.Count > 0)
            {
                Debug.Log("Cast spell effects");
                CastSpell(spell, targetPos, target);
            }
            ObjectPoolManager.Unspawn(gameObject);
        }
        public void CastSpell(Spell spell, Vector3 pos = default(Vector3), Unit2D target = null)
        {
            
            if (spell.effectRange > 0)
            {
                print("AOE effect!!!");
                Collider2D[] cols = Physics2D.OverlapCircleAll(pos, spell.effectRange, spell.customMask.value);
                if (cols.Length > 0)
                {
                    for (int i = 0; i < cols.Length; i++)
                    {
                        Unit2D unit = cols[i].gameObject.GetComponent<Unit2D>();
                        if (unit && unit.isAlive)
                        {
                            foreach (var effect in spell.effects)
                            {
                                print(unit.unitName + " takes effect " + spell.spellName);

                                StartCoroutine(ApplySpellEffect(effect, unit));
                            }
                        }
                    }
                }
            }
            else //single unit is affected
            {
                foreach (var effect in spell.effects)
                {
                    StartCoroutine(ApplySpellEffect(effect, target));
                }
            }
        }

        private IEnumerator ApplySpellEffect(SpellEffect effect, Unit2D target)
        {
            target.TakeDamage(effect.damage);
            if(effect.slow.slowMultiplier > 0f && effect.slow.duration > 0f)
            {
                target.TakeEffect(effect.slow);
            }
            yield return null;

        }

        private IEnumerator BulletRoutine()
        {
            print("BulletRoutine");

            if (shootEffect != null) ObjectPoolManager.Spawn(shootEffect, transform.position, transform.rotation);
            yield return new WaitForSeconds(shootDelay);
            //while the shootObject havent hit the target  
            while (!hit)
            {
                if (target != null) targetPos = target.GetTargetTransform().position;
                //calculating distance to targetPos
                Vector3 curPos = transform.position;
                //curPos.y = y;
                float currentDist = Vector2.Distance(curPos, targetPos);
                //if the target is close enough, trigger a hit
                if (currentDist < hitThreshold && !hit)
                {
                    Hit();
                    break;
                }

                transform.LookAt(targetPos);
                //move forward
                transform.Translate(Vector3.forward * Mathf.Min(speed * Time.deltaTime, currentDist));
                //transform.rotation = Quaternion.Euler(0, 0, 0);
                transform.rotation = Quaternion.Euler(0, 0, -transform.rotation.eulerAngles.x);
                speed += accel * Time.deltaTime;
                yield return null;
            }
        }

        public float smoothly = .5f;
        public float maxShootRange = 10;
        public float maxShootAngle = 45;

        private IEnumerator ProjectileRoutine()
        {

            if (shootEffect != null) ObjectPoolManager.Spawn(shootEffect, transform.position, transform.rotation);
            yield return new WaitForSeconds(shootDelay);

            //make sure the shootObject is facing the target and adjust the projectile angle
            transform.LookAt(targetPos);
            float angle = Mathf.Min(1, Vector3.Distance(transform.position, targetPos) / maxShootRange) * maxShootAngle;
            //clamp the angle magnitude to be less than 45 or less the dist ratio will be off
            transform.rotation = transform.rotation * Quaternion.Euler(-angle, 0, 0);

            Vector3 startPos = transform.position;
            float iniRotX = transform.rotation.eulerAngles.x;
            float y = Mathf.Min(targetPos.y, startPos.y);
            float totalDist = Vector3.Distance(startPos, targetPos);
            float timeShot = Time.time;

            //while the shootObject havent been hitting the target
            while (!hit)
            {
                if (target != null && updateTargetPosition) targetPos = target.GetTargetTransform().position;
                //calculating distance to targetPos
                Vector3 curPos = transform.position;
                //curPos.y = y;
                float currentDist = Vector2.Distance(curPos, targetPos);
                //if the target is close enough, trigger a hit
                if (currentDist < hitThreshold && !hit)
                {
                    Hit();
                    break;
                }

                if (Time.time - timeShot < 3f)
                {
                    //calculate ratio of distance covered to total distance
                    float invR = 1 - Mathf.Min(.5f, currentDist * smoothly / totalDist);

                    //use the distance information to set the rotation, 
                    //as the projectile approach target, it will aim straight at the target
                    Vector3 wantedDir = targetPos - transform.position;
                    if (wantedDir != Vector3.zero)
                    {
                        Quaternion wantedRotation = Quaternion.LookRotation(wantedDir);
                        float rotX = Mathf.LerpAngle(iniRotX, wantedRotation.eulerAngles.x, invR);

                        //make y-rotation always face target
                        transform.rotation = Quaternion.Euler(rotX, wantedRotation.eulerAngles.y, wantedRotation.eulerAngles.z);
                    }
                }
                else
                {
                    //this shoot time exceed 3 sec, abort the trajectory and just head to the target
                    transform.LookAt(targetPos);
                }
                //move forward
                //transform.LookAt(targetPos);
                transform.Translate(Vector3.forward * Mathf.Min(speed * Time.deltaTime, currentDist));
                transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.x);
                yield return null;
            }
        }
        public float shootAngleY = 20;
        private float missileSpeedModifier = 1;
        public float accel = 2;

        IEnumerator MissileRoutine()
        {
            StartCoroutine(MissileSpeedRoutine());

            float angleX = Random.Range(maxShootAngle / 2, maxShootAngle);
            float angleY = Random.Range(shootAngleY / 2, maxShootAngle);
            if (Random.Range(0f, 1f) > 0.5f) angleY *= -1;
            transform.LookAt(targetPos);
            transform.rotation = transform.rotation;
            Quaternion wantedRotation = transform.rotation * Quaternion.Euler(-angleX, angleY, angleX);
            float rand = Random.Range(4f, 10f);

            float totalDist = Vector3.Distance(transform.position, targetPos);

            float estimateTime = totalDist / speed;
            float shootTime = Time.time;

            Vector3 startPos = transform.position;

            while (!hit)
            {
                if (target != null && updateTargetPosition) targetPos = target.GetTargetTransform().position;
                float currentDist = Vector3.Distance(transform.position, targetPos);

                float delta = totalDist - Vector3.Distance(startPos, targetPos);
                float eTime = estimateTime - delta / speed;

                transform.rotation = lastRotation;

                if (Time.time - shootTime > eTime)
                {
                    Vector3 wantedDir = targetPos - transform.position;
                    if (wantedDir != Vector3.zero)
                    {
                        wantedRotation = Quaternion.LookRotation(wantedDir);
                        float val1 = (Time.time - shootTime) - (eTime);
                        transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, val1 / (eTime * currentDist));
                    }
                }
                else transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, Time.fixedDeltaTime * rand);

                if (currentDist < hitThreshold)
                {
                    Hit();
                    break;
                }

                transform.Translate(Vector3.forward * Mathf.Min(speed * Time.fixedDeltaTime * missileSpeedModifier, currentDist));
                lastRotation = transform.rotation;
                transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.x);
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
        }
        IEnumerator MissileSpeedRoutine()
        {
            missileSpeedModifier = .05f;
            float duration = 0;
            while (duration < 1)
            {
                missileSpeedModifier = Mathf.Sin(Mathf.Sin(duration * Mathf.PI / 2) * Mathf.PI / 2);
                duration += Time.deltaTime * 1f;
                yield return null;
            }
            missileSpeedModifier = 1;
        }
        Quaternion lastRotation;

        public float beamDuration = 0.5f;
        private IEnumerator BeamRoutine()
        {
            float timeShot = Time.time;

            while (!hit)
            {
                if (target != null) targetPos = target.GetTargetTransform().position;

                float dist = Vector3.Distance(shootPoint.position, targetPos);
                Ray ray = new Ray(shootPoint.position, (targetPos - shootPoint.position));
                Vector3 targetPosition = ray.GetPoint(dist - hitThreshold);

                for (int i = 0; i < lineList.Count; i++)
                {
                    lineList[i].SetPosition(0, shootPoint.position);
                    lineList[i].SetPosition(1, targetPos);
                }

                if (Time.time - timeShot > beamDuration)
                {
                    Hit();
                    break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        public float EffectDelay = 0.125f;

        IEnumerator EffectRoutine()
        {
            yield return new WaitForSeconds(EffectDelay);
            Hit();
        }
    }
}