//using UnitedSolution;using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//using UnitedSolution;
//using UnityEngine.AI;

//namespace UnitedSolution
//{

//    public class UnitDefenderComponent : UnitTower
//    {
//        [SerializeField]
//        public Movement movement;

//        protected override void Wandering()
//        {
//            if (!dead && !stunned)
//            {
//                movement.Wandering();
//            }
//            else
//            {
//                base.Wandering();
//            }
//        }

//        public override void InitTower(int ID)
//        {
//            movement = GetComponent<Movement>();
//            base.InitTower(ID);
//            realAttackRange = 0.5f;

//            //if (needInverseRotation)
//            //    transform.localRotation = Quaternion.Euler(0, 180, 0);
//        }

//        protected override void rotateTurretBack()
//        {
//            //turretObject.rotation = Quaternion.Slerp(turretObject.rotation, Quaternion.Euler(-90, 0, 0), turretRotateSpeed * Time.deltaTime * 0.25f);
//        }

//        public override void FixedUpdate()
//        {
//            base.FixedUpdate();

//            if (target && !stunned && !IsInConstruction())
//            {
//                movement.DoUpdate();
//            }
//        } 

//        private void StartRunningAway()
//        {
//            movement.StartRunningAway();
//            runningAway = true;
//            animController.PlayRunAnimation();
//        }



//        private void StopRunningAway()
//        {
//            animController.PlayRunAnimation();
//            target = ScanForTarget(CurrentStat.customMask, GetAttackRange());
//            if (target)
//            {
//              movement.StopRunningAway();
//            }
//            StartCoroutine(StopRunAnim());
//        }

//        IEnumerator StopRunAnim()
//        {
//            yield return new WaitForSeconds(.1f);
//            runningAway = false;
//        }

//    }
//}
