/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ
{

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ShieldRenderer : MonoBehaviour
    {
        public Material material_;
        private bool ready_ = false;
        private MeshFilter mf_;
        private MeshRenderer mr_;

        void Awake()
        {   
            Shield.Instance.init(material_);
        }

        void Start()
        {
            mf_ = GetComponent<MeshFilter>();
            mr_ = GetComponent<MeshRenderer>();
        }

        void Update()
        {
            //if (!ready_)
            //{
            //    return;
            //}
            Shield.Instance.render(0 /* front */, Time.time);
            mf_.sharedMesh = Shield.Instance.getMesh();
            mr_.material = Shield.Instance.getMaterial();
            mr_.SetPropertyBlock(Shield.Instance.getMaterialPropertyBlock());
        }
    }

} // namespace UTJ {

/*
 * End of Shield.Instance.Test.cs
 */
