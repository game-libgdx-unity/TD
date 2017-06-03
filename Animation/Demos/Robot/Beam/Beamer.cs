/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace RobotDemo
{
    public class Beamer : MonoBehaviour
    {
        public Material material_;
        static Beamer instance_;
        public static Beamer Instance { get { return instance_ ?? (instance_ = GameObject.Find("BeamRenderer").GetComponent<Beamer>()); } }

        struct Bullet
        {
            public bool alive_;
            public Vector3 position_;
            public Vector3 velocity_;
            public int id_;
            public int cnt_;
            public int damage;
            public RobotUnit target;
        }
        private Bullet[] pool_;
        private int pool_idx_;
        private bool moving;
        IEnumerator loop()
        {
            yield return new WaitForSeconds(0.5f);

            pool_ = new Bullet[16000];
            for (var i = 0; i < pool_.Length; ++i)
            {
                pool_[i].alive_ = false;
                pool_[i].cnt_ = 0;
            }
            pool_idx_ = 0;

            while (true)
            {
                // update
                for (var i = 0; i < pool_.Length; ++i)
                {
                    if (!pool_[i].alive_) continue;

                    pool_[i].position_ += pool_[i].velocity_ * (1f / 60f);

                    if (pool_[i].target)
                    {
                        if (Vector3.Distance(pool_[i].position_, pool_[i].target.targetT.position) < 3f)
                        {
                            pool_[i].target.TakeDamage(pool_[i].position_, pool_[i].damage);
                            pool_[i].alive_ = false;
                            Beam.Instance.destroy(pool_[i].id_);
                        }
                    }


                    --pool_[i].cnt_;
                    if (pool_[i].cnt_ <= 0)
                    {
                        pool_[i].alive_ = false;
                        Beam.Instance.destroy(pool_[i].id_);
                    }

                }

                // render
                Beam.Instance.begin(0 /* front */);
                for (var i = 0; i < pool_.Length; ++i)
                {
                    if (!pool_[i].alive_) continue;

                    var tail = pool_[i].position_ - (pool_[i].velocity_ * 0.1f);
                    Beam.Instance.renderUpdate(0 /* front */,
                                               pool_[i].id_,
                                               ref pool_[i].position_,
                                               ref tail);
                }
                Beam.Instance.end();

                yield return null;
            }
        }

        public void Shoot(Vector3 position, Vector3 velocity, RobotUnit target, int damage)
        {
            while (pool_[pool_idx_].alive_)
            {
                ++pool_idx_;
                if (pool_idx_ >= pool_.Length)
                {
                    pool_idx_ = 0;
                }
            }
            pool_[pool_idx_].alive_ = true;
            pool_[pool_idx_].target = target;
            pool_[pool_idx_].damage = damage;
            pool_[pool_idx_].position_ = position;
            pool_[pool_idx_].velocity_ = velocity;
            if (target.type == RobotUnit.UnitType.Ally)
                pool_[pool_idx_].id_ = Beam.Instance.spawn(0.8f /* width */, Beam.Type.LightBall);
            else
                pool_[pool_idx_].id_ = Beam.Instance.spawn(0.8f /* width */, Beam.Type.Bullet);
            pool_[pool_idx_].cnt_ = 100;
        }

        void Start()
        {
            Beam.Instance.init(material_);
            BeamRenderer.Instance.init(Beam.Instance);
            StartCoroutine(loop());
        }
        void LateUpdate()
        {
            Beam.Instance.render(0);
        }

    }

} // namespace UTJ {

/*
 * End of BeamTest.cs
 */
