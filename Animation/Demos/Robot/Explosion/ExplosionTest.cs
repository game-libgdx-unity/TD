/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;
using UnitedSolution;
using UTJ;

namespace RobotDemo
{

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ExplosionTest : SingletonBehaviour<ExplosionTest>
    {

        public Camera camera_;
        public Material material_;
        private bool ready_ = false;

        public void Invoke(Vector3 pos)
        {
            ready_ = true;
            GetComponent<MeshRenderer>().sharedMaterial = material_;
            Explosion.Instance.begin();
            Explosion.Instance.spawn(ref pos, Time.time);
            //Explosion.Instance.end(0 /* front */);
        }

        protected override void Awake()
        {
            base.Awake();
            Explosion.Instance.init(material_);
        }

        void Update()
        {
            if (!ready_)
            {
                return;
            }
            Explosion.Instance.render(0 /* front */, camera_, Time.time);
            var mesh = Explosion.Instance.getMesh();
            GetComponent<MeshFilter>().sharedMesh = mesh;
            var material = Explosion.Instance.getMaterial();
            GetComponent<MeshRenderer>().material = material;
        }
    }

} // namespace UTJ {

/*
 * End of ExplosionTest.cs
 */
