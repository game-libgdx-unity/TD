using UnitedSolution;using UnityEngine;
using UnityEngine.AI;

namespace UnitedSolution
{
    public class MultipleAnimationController : BaseAnimationController
    {
        public AnimationClip[] IdleClips;
        public AnimationClip[] AttackClips;
        public AnimationClip[] RunClips;
        public AnimationClip[] DeathClips;
        public AnimationClip[] HittedClips;

        protected override void Start()
        {
            base.Start();
            AddClips(IdleClips);
            AddClips(AttackClips);
            AddClips(RunClips);
            AddClips(DeathClips);
            AddClips(HittedClips);

            PlayClips(RunClips);
        }

        public override void PlayRunAnimation()
        {
            if (PlayClips(RunClips))
            {
                base.PlayRunAnimation();
            }
        }

        public override void PlayAttackAnimation()
        {
            if (PlayClips(AttackClips))
            {
                base.PlayAttackAnimation();
            }
        }

        public override void PlayDeathAnimation()
        {
            if (PlayClips(DeathClips))
            {
                base.PlayDeathAnimation();
            }
        }

        public override void OnAttackTargetStopped()
        {
            if (PlayClips(RunClips))
            {
                base.OnAttackTargetStopped();
            }
        }
        public override void OnUnitHitted()
        {
            if (animation)
            {
                animation.Play(hittedTrigger);
            }
        }

        private void AddClips(AnimationClip[] Clips)
        {
            if (Clips == null)
                return;

            if (Clips.Length > 0)
            {
                foreach (AnimationClip ac in Clips)
                    animation.AddClip(ac, ac.name);
            }
            else
            {
                Debug.LogWarning("No animation for ");
            }
        }

        private bool PlayClips(AnimationClip[] clips)
        {
            if (animation && clips.Length > 0)
            {
                int index = Random.Range(0, clips.Length);

                //if (clips.Length > 1)
                //    while (animation.IsPlaying(clips[index].name))
                //    {
                //        index = Random.Range(0, clips.Length);
                //    }

                animation.Play(clips[index].name);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}