//using UnitedSolution;using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//using UnitedSolution;


//namespace UnitedSolution
//{

//    [RequireComponent(typeof(UnitCreep))]
//    public class UnitCreepAnimation : MonoBehaviour
//    {

//        private UnitCreep unitCreep;

//        public enum _AniType { None, Mecanim, Legacy }
//        public _AniType type = _AniType.None;

//        public GameObject aniRootObj;

//        //common for both mecanim and legacy
//        public AnimationClip clipSpawn;
//        public AnimationClip clipMove;
//        public AnimationClip clipHit;
//        public AnimationClip clipDead;
//        public AnimationClip clipDestination;


//        //for mecanim
//        [HideInInspector]
//        public Animator anim;

//        //for legacy
//        [HideInInspector]
//        public Animation aniInstance;
//        public float moveSpeedMultiplier = 1.0f;



//        void Awake()
//        {
//            if (type == _AniType.None) return;

//            unitCreep = gameObject.GetComponent<UnitCreep>();

//            if (type == _AniType.Legacy)
//            {
//                aniInstance = aniRootObj.GetComponent<Animation>();
//                if (aniInstance != null)
//                {
//                    InitAnimation();
//                    unitCreep.SetAnimationComponent(this);
//                }
//            }

//            if (type == _AniType.Mecanim)
//            {
//                if (anim == null) anim = aniRootObj.GetComponent<Animator>();
//                if (anim != null) unitCreep.SetAnimationComponent(this);

//                AnimatorOverrideController overrideController = new AnimatorOverrideController();
//                overrideController.runtimeAnimatorController = anim.runtimeAnimatorController;

//                //overrideController["Assigned Animation Clip Name In The Controller"] = New Clip To Be Assigned;
//                //overrideController["DummySpawn"] = clipSpawn!=null ? clipSpawn : null;
//                overrideController["DummyMove"] = clipMove != null ? clipMove : null;
//                overrideController["DummyDestination"] = clipDestination != null ? clipDestination : null;
//                overrideController["DummyDestroyed"] = clipDead != null ? clipDead : null;

//                //if no spawn animation has been assigned, use move animation instead otherwise there will be an delay, bug maybe?
//                AnimationClip spawn = clipSpawn != null ? clipSpawn : clipMove;
//                overrideController["DummySpawn"] = spawn != null ? spawn : null;

//                anim.runtimeAnimatorController = overrideController;
//            }
//        }



//        void Update()
//        {
//            if (type == _AniType.None) return;

//            if (type == _AniType.Legacy && aniInstance != null)
//            {
//                if (unitCreep.stunned)
//                {
//                    aniInstance.GetComponent<Animation>()[clipMove.name].speed = 0;
//                }
//                else
//                {
//                    aniInstance.GetComponent<Animation>()[clipMove.name].speed = unitCreep.GetMoveSpeed() * moveSpeedMultiplier;
//                }
//            }

//            if (type == _AniType.Mecanim && anim != null)
//            {
//                anim.SetFloat("Speed", unitCreep.GetMoveSpeed());
//            }
//        }

//        void OnEnable()
//        {
//            if (type == _AniType.None) return;

//            PlaySpawn();

//            if (type == _AniType.Legacy)
//            {
//                if (aniInstance != null && clipMove != null)
//                {
//                    aniInstance.Play(clipMove.name);
//                }
//            }
//        }




//        public float PlaySpawn()
//        {
//            if (type == _AniType.Mecanim) return PlaySpawnMecanim();
//            if (type == _AniType.Legacy) return PlaySpawnLegacy();
//            return 0;
//        }
//        public void PlayHit()
//        {
//            if (type == _AniType.Mecanim) PlayHitMecanim();
//            if (type == _AniType.Legacy) PlayHitLegacy();
//            return;
//        }
//        public float PlayDead()
//        {
//            if (type == _AniType.Mecanim) return PlayDeadMecanim();
//            if (type == _AniType.Legacy) return PlayDeadLegacy();
//            return 0;
//        }
//        public float PlayDestination()
//        {
//            if (type == _AniType.Mecanim) return PlayDestinationMecanim();
//            if (type == _AniType.Legacy) return PlayDestinationLegacy();
//            return 0;
//        }



//        public float PlaySpawnLegacy()
//        {
//            float duration = 0;

//            if (aniInstance != null && clipSpawn != null)
//            {
//                aniInstance.CrossFade(clipSpawn.name);
//                duration = clipSpawn.length;
//            }

//            return duration;
//        }
//        public void PlayHitLegacy()
//        {
//            if (aniInstance != null && clipHit != null) aniInstance.CrossFade(/*clipHit*/.name);
//        }
//        public float PlayDeadLegacy()
//        {
//            aniInstance.Stop();
//            float duration = 0;
//            if (aniInstance != null && clipDead != null)
//            {
//                aniInstance.CrossFade(clipDead.name);
//                duration = clipDead.length;
//            }
//            return duration;
//        }
//        public float PlayDestinationLegacy()
//        {
//            aniInstance.Stop();
//            float duration = 0;
//            if (aniInstance != null && clipDestination != null)
//            {
//                aniInstance.CrossFade(clipDestination.name);
//                duration = clipDestination.length;
//            }
//            return duration;
//        }






//        public float PlaySpawnMecanim()
//        {
//            anim.SetTrigger("Spawn");
//            return clipSpawn != null ? clipSpawn.length : 0;
//        }
//        public void PlayHitMecanim()
//        {
//            anim.SetTrigger("Hit");
//        }
//        public float PlayDeadMecanim()
//        {
//            anim.SetTrigger("Dead");
//            //return anim.GetNextAnimatorStateInfo(0).length;
//            return clipDead != null ? clipDead.length : 0;
//        }
//        public float PlayDestinationMecanim()
//        {
//            anim.SetTrigger("Destination");
//            //return anim.GetNextAnimatorStateInfo(0).length;
//            return clipDestination != null ? clipDestination.length : 0;
//        }






//        private void InitAnimation()
//        {
//            if (aniInstance == null) return;

//            if (clipSpawn != null)
//            {
//                aniInstance.AddClip(clipSpawn, clipSpawn.name);
//                aniInstance.GetComponent<Animation>()[clipSpawn.name].layer = 1;
//                aniInstance.GetComponent<Animation>()[clipSpawn.name].wrapMode = WrapMode.Once;
//            }

//            if (clipMove != null)
//            {
//                aniInstance.AddClip(clipMove, clipMove.name);
//                aniInstance.GetComponent<Animation>()[clipMove.name].layer = 0;
//                aniInstance.GetComponent<Animation>()[clipMove.name].wrapMode = WrapMode.Loop;
//            }

//            if (clipHit != null)
//            {
//                aniInstance.AddClip(clipHit, clipHit.name);
//                aniInstance.GetComponent<Animation>()[clipHit.name].layer = 3;
//                aniInstance.GetComponent<Animation>()[clipHit.name].wrapMode = WrapMode.Once;
//            }

//            if (clipDead != null)
//            {
//                aniInstance.AddClip(clipDead, clipDead.name);
//                aniInstance.GetComponent<Animation>()[clipDead.name].layer = 3;
//                aniInstance.GetComponent<Animation>()[clipDead.name].wrapMode = WrapMode.Once;
//            }

//            if (clipDestination != null)
//            {
//                aniInstance.AddClip(clipDestination, clipDestination.name);
//                aniInstance.GetComponent<Animation>()[clipDestination.name].layer = 3;
//                aniInstance.GetComponent<Animation>()[clipDestination.name].wrapMode = WrapMode.Once;
//            }

//        }



//    }

//}
