//using UnitedSolution;using UnityEngine;
//using System;
//using DG.Tweening;

//namespace UnitedSolution
//{
//    public static class TweenHelper
//    {

//        public static Tween SimulateUpdate(this MonoBehaviour behaviour, float frequently = .08f, float delay = 0f, int numberOfUpdate = int.MaxValue, Action OnStarted = null, Action OnUpdate = null, Action OnFinished = null)
//        {
//            return DOVirtual.DelayedCall(delay, () =>
//{

//}).OnStart(() => { if (OnStarted != null) OnStarted(); });
//        }

//        public static Tween SimulateUpdate(this ShootObject2D behaviour, float frequently = .08f, float delay = 0f, int numberOfUpdate = int.MaxValue, Action OnStarted = null, Action OnUpdate = null, Action OnFinished = null)
//        {
//            return DOVirtual.DelayedCall(delay, () =>
//            {

//            }).OnStart(() => { if (OnStarted != null) OnStarted(); });
//        }
//    }
//}
