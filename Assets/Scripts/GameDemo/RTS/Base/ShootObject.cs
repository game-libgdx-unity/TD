using UnitedSolution;using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnitedSolution; 

namespace UnitedSolution
{
    public enum _ShootObjectType { Projectile, Missile, Beam, Effect, Bullet, FPSProjectile, FPSBeam, FPSEffect }

    public class ShootObject : MonoBehaviour
    {

        public AbilityBehavior Ab_Start_Holder;
        public AbilityBehavior Ab_End_Holder;

        public _ShootObjectType type;

        public float speed_cached;
        public float speed = 5;
        public float beamDuration = .5f;


        private bool hit = false;

        public bool autoSearchLineRenderer = true;
        public List<LineRenderer> lineList = new List<LineRenderer>();
        private List<TrailRenderer> trailList = new List<TrailRenderer>();

        private Transform shootPoint;
        private AttackInstance attInstance;
        public GameObject shootEffect;
        public GameObject hitEffect;

        public bool penetratable;
        public int penetration_max_targets;
        public float penetration_distance;

        public bool multiShoot;
        public float multishot_delay = .1f;
        public int multishot_max_targets = 3;

        public bool bouncing;
        public float bouncing_radius = 5;
        public int bouncing_max_targets_bk = 3;
        public int bouncing_max_targets = 3;
        public float bouncingDelay = .2f;


        private GameObject thisObj;
        private Transform thisT;
        public EffectSettings effectSetting;


        void Awake()
        {
            speed_cached = speed;
            bouncing_max_targets_bk = bouncing_max_targets;
            thisObj = gameObject;
            thisT = transform;

            thisObj.layer = LayerManager.LayerShootObject();

            if (autoSearchLineRenderer)
            {
                LineRenderer[] lines = thisObj.GetComponentsInChildren<LineRenderer>(true);
                for (int i = 0; i < lines.Length; i++) lineList.Add(lines[i]);
            }


            TrailRenderer[] trails = thisObj.GetComponentsInChildren<TrailRenderer>(true);
            for (int i = 0; i < trails.Length; i++) trailList.Add(trails[i]);

            if (type == _ShootObjectType.FPSProjectile)
            {
                SphereCollider sphereCol = GetComponent<SphereCollider>();
                if (sphereCol == null)
                {
                    sphereCol = thisObj.AddComponent<SphereCollider>();
                    sphereCol.radius = 0.15f;
                }
                hitRadius = sphereCol.radius;
            }

            if (shootEffect != null) ObjectPoolManager.New(shootEffect);
            if (hitEffect != null) ObjectPoolManager.New(hitEffect);
        }

        //void Start()
        //{
        //    ObjectPoolManager.New(gameObject);
        //}

        void OnEnable()
        {
            for (int i = 0; i < trailList.Count; i++) StartCoroutine(ClearTrail(trailList[i]));
            bouncing_max_targets = bouncing_max_targets_bk;
            speed = speed_cached;
        }
        void OnDisable()
        {
        }

        public void Shoot(AttackInstance attInst = null, Transform sp = null)
        {
            if (attInst.tgtUnit == null || attInst.tgtUnit.GetTargetT() == null)
            {
                ObjectPoolManager.Unspawn(thisObj);
                return;
            }

            attInstance = attInst;
            target = attInstance.tgtUnit;
            //print(attInstance.srcUnit.unitName + " attacks " + attInstance.tgtUnit.unitName + " with " + attInstance.damage + " damage");

            targetPos = target.GetTargetT().position;
#if Game_2D
            hitThreshold = Mathf.Max(.01f, target.hitThreshold);
#else
            hitThreshold = Mathf.Max(.1f, target.hitThreshold);
#endif
            shootPoint = sp;

            if (shootPoint != null) thisT.rotation = shootPoint.rotation;
            if (shootEffect != null && !target.dead)
            {
                GameObject shootEffectInstance = ObjectPoolManager.Spawn(shootEffect, thisT.position, thisT.rotation);
                if (shootEffectInstance)
                {
                    effectSetting = shootEffectInstance.GetComponent<EffectSettings>();
                    if (effectSetting == null)
                    {
                        effectSetting = shootEffectInstance.GetComponentInChildren<EffectSettings>();
                    }
                    if (effectSetting)
                    {
                        if (effectSetting.EffectType == EffectSettings.EffectTypeEnum.Other)
                        {
                            Vector3 direction = (targetPos - thisT.position).normalized;
                            shootEffectInstance.transform.rotation = Quaternion.LookRotation(direction);
                        }

                        effectSetting.Target = target.GetTargetT().gameObject;
                        SelfDeactivator selfDeactivator = shootEffectInstance.GetComponent<SelfDeactivator>();
                        if (selfDeactivator == null)
                        {
                            selfDeactivator = shootEffectInstance.AddComponent<SelfDeactivator>();
                        }
                        selfDeactivator.duration = attInstance.srcUnit.CurrentStat.cooldown;
                    }
                }
                Unit.onDestroyedE += OnTargetDestroy;
            }
            hit = false;


            if (Ab_Start_Holder)
            {
                AbilityManager.instance.ActivateAbility(Ab_Start_Holder.ability, attInstance.srcUnit, targetPos, target);
            }

            if (type == _ShootObjectType.Projectile) StartCoroutine(ProjectileRoutine());
            else if (type == _ShootObjectType.Beam) StartCoroutine(BeamRoutine());
            else if (type == _ShootObjectType.Missile) StartCoroutine(MissileRoutine());
            else if (type == _ShootObjectType.Effect && EffectDelay > 0) StartCoroutine(EffectRoutine()); else Hit();
        }

        private void OnTargetDestroy(Unit unit)
        {
            Unit.onDestroyedE -= OnTargetDestroy;

            if (unit == target)
            {
                StopAllCoroutines();
                CancelInvoke();
                ObjectPoolManager.Unspawn(shootEffect);

            }
            else if (unit == attInstance.srcUnit)
            {
                StopAllCoroutines();
                CancelInvoke();
                if (effectSetting)
                    effectSetting.Deactivate();
                ObjectPoolManager.Unspawn(shootEffect);
                if (target)
                    target.PurifyEffect();
            }

        }

        public void ShootFPS(AttackInstance attInst = null, Transform sp = null)
        {
            shootPoint = sp;
            if (shootPoint != null) thisT.rotation = shootPoint.rotation;

            if (shootEffect != null) ObjectPoolManager.Spawn(shootEffect, thisT.position, thisT.rotation);

            hit = false;
            attInstance = attInst;
            if (type == _ShootObjectType.FPSProjectile) StartCoroutine(FPSProjectileRoutine());
            if (type == _ShootObjectType.FPSBeam) StartCoroutine(FPSBeamRoutine(sp));
            if (type == _ShootObjectType.FPSEffect) StartCoroutine(FPSEffectRoutine());
        }

        public float hitRadius = .1f;


        private Unit target;
        private Vector3 targetPos;
        public float maxShootAngle = 30f;
        public float maxShootRange = 0.5f;
        private float hitThreshold = 0.15f;
        public float GetMaxShootRange()
        {
            if (type == _ShootObjectType.Projectile || type == _ShootObjectType.Missile) return maxShootRange;
            return 1;
        }
        public float GetMaxShootAngle()
        {
            if (type == _ShootObjectType.Projectile || type == _ShootObjectType.Missile) return maxShootAngle;
            return 0;
        }

        public float EffectDelay = 0.125f;
        IEnumerator EffectRoutine()
        {
            var delay = attInstance.srcUnit.animController;
            yield return new WaitForSeconds(Mathf.Max(delay ? delay.attackDelay : 0f, EffectDelay));
            Hit();
        }
        IEnumerator BeamRoutine()
        {
            float timeShot = Time.time;

            while (!hit)
            {
                if (effectSetting == null)
                {
                    if (target != null) targetPos = target.GetTargetT().position;

                    float dist = Vector3.Distance(shootPoint.position, targetPos);
                    Ray ray = new Ray(shootPoint.position, (targetPos - shootPoint.position));
                    Vector3 targetPosition = ray.GetPoint(dist - hitThreshold);

                    for (int i = 0; i < lineList.Count; i++)
                    {
                        lineList[i].SetPosition(0, shootPoint.position);
                        lineList[i].SetPosition(1, targetPos);
                    }
                }

                if (Time.time - timeShot > beamDuration)
                {
                    Hit();
                    break;
                }

                yield return new WaitForEndOfFrame();
            }
        }
        public float shootDelay = .5f;
        IEnumerator ProjectileRoutine()
        {
            if (shootEffect != null) ObjectPoolManager.Spawn(shootEffect, thisT.position, thisT.rotation);

            yield return new WaitForSeconds(shootDelay);

            float timeShot = Time.time;
            targetPos = target.GetTargetT().position;

            //make sure the shootObject is facing the target and adjust the projectile angle
            thisT.LookAt(targetPos);
            float angle = Mathf.Min(1, Vector3.Distance(thisT.position, targetPos) / maxShootRange) * maxShootAngle;
            //clamp the angle magnitude to be less than 45 or less the dist ratio will be off
            thisT.rotation = thisT.rotation * Quaternion.Euler(-angle, 0, 0);

            Vector3 startPos = thisT.position;
            float iniRotX = thisT.rotation.eulerAngles.x;

            float y = Mathf.Min(targetPos.y, startPos.y);
            float totalDist = Vector3.Distance(startPos, targetPos);

            //while the ball havent hit the target
            while (!hit)
            {
                if (target != null) targetPos = target.GetTargetT().position;

                //calculating distance to targetPos
                Vector3 curPos = thisT.position;
                //curPos.y = y;
                float currentDist = Vector3.Distance(curPos, targetPos);
                //if the target is close enough, trigger a hit
                if (currentDist < hitThreshold && !hit)
                {
                    Hit();
                    break;
                }

                if (Time.time - timeShot < 3f)
                {
                    //calculate ratio of distance covered to total distance
                    float invR = 1 - Mathf.Min(1, currentDist / totalDist);

                    //use the distance information to set the rotation, 
                    //as the projectile approach target, it will aim straight at the target
                    Vector3 wantedDir = targetPos - thisT.position;
                    if (wantedDir != Vector3.zero)
                    {
                        Quaternion wantedRotation = Quaternion.LookRotation(wantedDir);
                        float rotX = Mathf.LerpAngle(iniRotX, wantedRotation.eulerAngles.x, invR);

                        //make y-rotation always face target
                        thisT.rotation = Quaternion.Euler(rotX, wantedRotation.eulerAngles.y, wantedRotation.eulerAngles.z);
                    }
                }
                else
                {
                    //this shoot time exceed 3 sec, abort the trajectory and just head to the target
                    thisT.LookAt(targetPos);
                }

                //move forward
                thisT.Translate(Vector3.forward * Mathf.Min(speed * Time.deltaTime, currentDist));
                speed += accel * Time.fixedDeltaTime;

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
            thisT.LookAt(targetPos);
            thisT.rotation = thisT.rotation;
            Quaternion wantedRotation = thisT.rotation * Quaternion.Euler(-angleX, angleY, 0);
            float rand = Random.Range(4f, 10f);

            float totalDist = Vector3.Distance(thisT.position, targetPos);

            float estimateTime = totalDist / speed;
            float shootTime = Time.time;

            Vector3 startPos = thisT.position;

            while (!hit)
            {
                if (target != null) targetPos = target.GetTargetT().position;
                float currentDist = Vector3.Distance(thisT.position, targetPos);

                float delta = totalDist - Vector3.Distance(startPos, targetPos);
                float eTime = estimateTime - delta / speed;

                if (Time.time - shootTime > eTime)
                {
                    Vector3 wantedDir = targetPos - thisT.position;
                    if (wantedDir != Vector3.zero)
                    {
                        wantedRotation = Quaternion.LookRotation(wantedDir);
                        float val1 = (Time.time - shootTime) - (eTime);
                        thisT.rotation = Quaternion.Slerp(thisT.rotation, wantedRotation, val1 / (eTime * currentDist));
                    }
                }
                else thisT.rotation = Quaternion.Slerp(thisT.rotation, wantedRotation, Time.fixedDeltaTime * rand);

                if (currentDist < hitThreshold)
                {
                    Hit();
                    break;
                }

                thisT.Translate(Vector3.forward * Mathf.Min(speed * Time.fixedDeltaTime * missileSpeedModifier, currentDist));
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

        public bool isArrow = false;

        void Hit()
        {
            hit = true;

            if (isArrow) //leave an arrow in the ground effect
            {
                targetPos = new Vector3(targetPos.x + Random.value - .5f, 0.05f, targetPos.z + Random.value - .5f);
            }
            if (hitEffect != null) ObjectPoolManager.Spawn(hitEffect, targetPos, isArrow ? Quaternion.Euler(-180, Random.Range(0, 30), Random.Range(0, 30)) : Quaternion.identity);

            //apply ability
            if (Ab_End_Holder)
            {
                AbilityManager.instance.ActivateAbility(Ab_End_Holder.ability, attInstance.srcUnit, targetPos, target);
            }
            thisT.position = targetPos;
            attInstance.impactPoint = thisT.position;
            LayerMask mask = attInstance.srcUnit.TargetMask;

            if (attInstance.srcUnit.GetAOERadius() > 0)
            {
                Collider[] cols = Physics.OverlapSphere(thisT.position, attInstance.srcUnit.GetAOERadius(), mask);
                AttackTargets(cols);
                print("Splash attack kill more " + cols.Length);
            }
            else
            {
                if (target != null)
                {
                    target.ApplyEffect(attInstance);
                    if (target.alarmWhenGotAttacked && !target.isAlarming && target.GetAttackRange() < 2 && attInstance.damage > 0)
                    {
                        target.isAlarming = true;
                        target.attacker = attInstance.srcUnit;
                        target.StartCoroutine(SetAlarmOff(target, 10));
                    }

                    if (penetratable && penetration_distance > 0) //penetratable attack
                    {
                        Vector3 position = shootPoint.position;
                        Vector3 direction = target.GetTargetT().position - position;
                        RaycastHit[] hits = Physics.RaycastAll(position, direction.normalized, penetration_distance);
                        AttackTargets(hits.GetColliders());
                        print("Penetrate attack kill more " + hits.Length);
                    }
                }
            }
            //DestroyImmediate(thisObj);
            if (!bouncing)
            {
                ObjectPoolManager.Unspawn(thisObj);
            }
            else
            {
                Collider[] cols = Physics.OverlapSphere(thisT.position, bouncing_radius, mask);
                if (cols.Length > 0)
                {
                    print("Bonce");
                    List<Unit> tgtList = new List<Unit>();
                    for (int i = 0; i < cols.Length; i++)
                    {
                        Unit unit = cols[i].gameObject.GetComponent<Unit>();
                        if (unit && !unit.dead && unit != attInstance.tgtUnit) tgtList.Add(unit);
                    }
                    if (tgtList.Count > 0 && bouncing_max_targets >= 0)
                    {
                        bouncing_max_targets--;
                        attInstance.tgtUnit = tgtList[0];
                        Shoot(attInstance, thisT);
                    }
                }
                else
                {
                    ObjectPoolManager.Unspawn(thisObj);
                }
            }
        }

        IEnumerator SetAlarmOff(Unit unit, float timeout)
        {
            yield return new WaitForSeconds(timeout);
            unit.isAlarming = false;
        }

        private void AttackTargets(Collider[] targets)
        {
            if (targets.Length > 0)
            {
                List<Unit> tgtList = new List<Unit>();
                for (int i = 0; i < targets.Length; i++)
                {
                    Unit unit = targets[i].gameObject.GetComponent<Unit>();
                    if (unit && !unit.dead) tgtList.Add(unit);
                }
                if (tgtList.Count > 0)
                {
                    for (int i = 0; i < tgtList.Count; i++)
                    {
                        if (tgtList[i] == target)
                        {
                            target.ApplyEffect(attInstance);
                            print(attInstance.srcUnit.unitName + " attacks " + target.unitName + " with " + attInstance.damage + " damage");
                        }
                        else
                        {
                            AttackInstance attInst = attInstance.Clone();
                            attInst.srcUnit = attInstance.srcUnit;
                            attInst.tgtUnit = tgtList[i];
                            attInst.Process();
                            tgtList[i].ApplyEffect(attInst);

                            print(attInst.srcUnit.unitName + " attacks " + attInst.tgtUnit.unitName + " with " + attInst.damage + " damage");
                        }
                    }
                }
            }
        }
        IEnumerator ClearTrail(TrailRenderer trail)
        {
            if (trail == null) yield break;
            float trailDuration = trail.time;
            trail.time = -1;
            yield return null;
            trail.time = trailDuration;
        }

        #region Legacy

        IEnumerator FPSEffectRoutine()
        {
            yield return new WaitForSeconds(0.05f);

            RaycastHit raycastHit;
            Vector3 dir = thisT.TransformDirection(new Vector3(0, 0, 1));
            if (Physics.SphereCast(thisT.position, hitRadius / 2, dir, out raycastHit))
            {
                Unit unit = raycastHit.transform.GetComponent<Unit>();
                FPSHit(unit, raycastHit.point);

                if (hitEffect != null) ObjectPoolManager.Spawn(hitEffect, raycastHit.point, Quaternion.identity);
            }

            yield return new WaitForSeconds(0.1f);
            ObjectPoolManager.Unspawn(thisObj);
        }
        IEnumerator FPSBeamRoutine(Transform sp)
        {
            thisT.parent = sp;
            float duration = 0;
            while (duration < beamDuration)
            {
                RaycastHit raycastHit;
                Vector3 dir = thisT.TransformDirection(new Vector3(0, 0, 1));
                bool hitCollider = Physics.SphereCast(thisT.position, hitRadius, dir, out raycastHit);
                if (hitCollider)
                {
                    if (!hit)
                    {
                        hit = true;
                        Unit unit = raycastHit.transform.GetComponent<Unit>();
                        FPSHit(unit, raycastHit.point);

                        if (hitEffect != null) ObjectPoolManager.Spawn(hitEffect, raycastHit.point, Quaternion.identity);
                    }
                }

                float lineDist = raycastHit.distance == 0 ? 9999 : raycastHit.distance;
                for (int i = 0; i < lineList.Count; i++) lineList[i].SetPosition(1, new Vector3(0, 0, lineDist));

                duration += Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            thisT.parent = null;
            ObjectPoolManager.Unspawn(thisObj);
        }
        IEnumerator FPSProjectileRoutine()
        {
            float timeShot = Time.time;
            while (true)
            {
                RaycastHit raycastHit;
                Vector3 dir = thisT.TransformDirection(new Vector3(0, 0, 1));
                float travelDist = speed * Time.fixedDeltaTime;
                bool hitCollider = Physics.SphereCast(thisT.position, hitRadius, dir, out raycastHit, travelDist);
                if (hitCollider) travelDist = raycastHit.distance + hitRadius;

                thisT.Translate(Vector3.forward * travelDist);
                if (Time.time - timeShot > 5) break;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            ObjectPoolManager.Unspawn(thisObj);
            yield return null;
        }
        void FPSHit(Unit hitUnit, Vector3 hitPoint)
        {
            if (attInstance.srcWeapon.GetAOERange() > 0)
            {
                LayerMask mask1 = 1 << LayerManager.LayerCreep();
                LayerMask mask2 = 1 << LayerManager.LayerCreepF();
                LayerMask mask = mask1 | mask2;

                Collider[] cols = Physics.OverlapSphere(hitPoint, attInstance.srcWeapon.GetAOERange(), mask);
                if (cols.Length > 0)
                {
                    List<Unit> tgtList = new List<Unit>();
                    for (int i = 0; i < cols.Length; i++)
                    {
                        Unit unit = cols[i].gameObject.GetComponent<Unit>();
                        if (!unit.dead) tgtList.Add(unit);
                    }
                    if (tgtList.Count > 0)
                    {
                        for (int i = 0; i < tgtList.Count; i++)
                        {
                            AttackInstance attInst = new AttackInstance();
                            attInst.srcWeapon = attInstance.srcWeapon;
                            attInst.tgtUnit = tgtList[i];
                            tgtList[i].ApplyEffect(attInst);
                        }
                    }
                }
            }
            else
            {
                if (hitUnit != null && hitUnit.IsCreep())
                {
                    attInstance.tgtUnit = hitUnit;
                    hitUnit.ApplyEffect(attInstance);
                }
            }
        }
        //void OnTriggerEnter(Collider collider)
        //{
        //if (!hit && type != _ShootObjectType.FPSProjectile) return;

        //hit = true;

        ////if (hitEffect != null) ObjectPoolManager.Spawn(hitEffect, thisT.position, Quaternion.Euler(-180, 0, 0));

        //attInstance.impactPoint = thisT.position;

        //Unit unit = collider.gameObject.GetComponent<Unit>();
        //FPSHit(unit, thisT.position);

        //if (hitEffect != null) ObjectPoolManager.Spawn(hitEffect, thisT.position, thisT.rotation);

        //ObjectPoolManager.Unspawn(thisObj);
        //}

        #endregion
    }

}