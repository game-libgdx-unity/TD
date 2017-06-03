/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;
using UnitedSolution;
using UTJ;

namespace RobotDemo
{

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HahenRenderer : SingletonBehaviour<HahenRenderer>
    {
        public Material material_;
        private bool ready_ = false; 

        public void Invoke(Vector3 pos)
        {
            ready_ = true;

            GetComponent<MeshRenderer>().sharedMaterial = material_;
            Hahen.Instance.begin();
            Hahen.Instance.spawn(ref pos, Time.time);
            //Hahen.Instance.end(0 /* front */);
        }

        void Start()
        {
            Hahen.Instance.init(material_);
        }

        void Update()
        {
            if (!ready_)
            {
                return;
            }
            Hahen.Instance.render(0 /* front */, Time.time);
            var mesh = Hahen.Instance.getMesh();
            GetComponent<MeshFilter>().sharedMesh = mesh;
            var material = Hahen.Instance.getMaterial();
            GetComponent<MeshRenderer>().material = material;
        }
    }

} // namespace UTJ {

/*
 * End of HahenTest.cs
 */
