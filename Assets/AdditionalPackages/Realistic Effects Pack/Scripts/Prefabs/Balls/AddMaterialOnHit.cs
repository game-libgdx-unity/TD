using System.Linq;
using UnitedSolution;using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class AddMaterialOnHit : MonoBehaviour
{
    public float RemoveAfterTime = 3;
    public bool RemoveWhenDisable;
    public EffectSettings EffectSettings;
    public Material Material;
    public bool UsePointMatrixTransform;
    public Vector3 TransformScale = Vector3.one;

    private FadeInOutShaderColor[] fadeInOutShaderColor;
    private FadeInOutShaderFloat[] fadeInOutShaderFloat;
    private UVTextureAnimator uvTextureAnimator;
    private Renderer[] renderParent;
    private Material instanceMat;
    private int materialQueue = -1;
    private bool waitRemove;
    private float timeToDelete;

    //void Start()
    //{
    //    StartCoroutine(Removal());
    //}

    void Update()
    {
        if (EffectSettings == null) return;
        if (EffectSettings.IsVisible)
        {
            timeToDelete = 0;
        }
        else
        {
            timeToDelete += Time.deltaTime;
            if (timeToDelete > RemoveAfterTime) ObjectPoolManager.Unspawn(gameObject);
        }
    }

    public void UpdateMaterial(RaycastHit hit)
    {
        var hitGO = hit.transform;
        if (hitGO != null)
        {
            AddMaterialObject(hit);
        }
    }

    IEnumerator AddAndRemove(Renderer render)
    {
        var materials = render.sharedMaterials;
        var length = materials.Length + 1;
        var newMaterials = new Material[length];

        materials.CopyTo(newMaterials, 0);
        render.material = Material;
        instanceMat = render.material;
        newMaterials[length - 1] = instanceMat;
        render.sharedMaterials = newMaterials;

        yield return new WaitForSeconds(RemoveAfterTime);

        if (render)
        {
            List<Material> Materials = render.sharedMaterials.ToList();
            Materials.RemoveAll(m => m == instanceMat);
            render.sharedMaterials = Materials.ToArray();
        }
    }

    private void AddMaterialObject(RaycastHit hit)
    {
        if (!RemoveWhenDisable) ObjectPoolManager.Unspawn(gameObject, RemoveAfterTime);
        fadeInOutShaderColor = GetComponents<FadeInOutShaderColor>();
        fadeInOutShaderFloat = GetComponents<FadeInOutShaderFloat>();
        uvTextureAnimator = GetComponent<UVTextureAnimator>();
        renderParent = transform.parent.GetComponentsInChildren<Renderer>();

        foreach (Renderer render in renderParent)
        {

            StartCoroutine( AddAndRemove(render));

            if (UsePointMatrixTransform)
            {
                var m = Matrix4x4.TRS(hit.transform.InverseTransformPoint(hit.point), Quaternion.Euler(180, 180, 0f), TransformScale);
                //m *= transform.localToWorldMatrix;
                instanceMat.SetMatrix("_DecalMatr", m);
            }
            if (materialQueue != -1)
                instanceMat.renderQueue = materialQueue;

            if (fadeInOutShaderColor != null)
            {
                foreach (var inOutShaderColor in fadeInOutShaderColor)
                {
                    inOutShaderColor.UpdateMaterial(instanceMat);
                }
            }

            if (fadeInOutShaderFloat != null)
            {
                foreach (var inOutShaderFloat in fadeInOutShaderFloat)
                {
                    inOutShaderFloat.UpdateMaterial(instanceMat);
                }
            }
            if (uvTextureAnimator != null)
                uvTextureAnimator.SetInstanceMaterial(instanceMat, hit.textureCoord);
        }
    }


    public void UpdateMaterial(Transform transformTarget)
    {
        if (transformTarget != null)
        {
            if (!RemoveWhenDisable) ObjectPoolManager.Unspawn(gameObject, RemoveAfterTime);
            fadeInOutShaderColor = GetComponents<FadeInOutShaderColor>();
            fadeInOutShaderFloat = GetComponents<FadeInOutShaderFloat>();
            uvTextureAnimator = GetComponent<UVTextureAnimator>();
            renderParent = transform.parent.GetComponentsInChildren<MeshRenderer>();

            //for(int i = 0; i < renderParent.Length; i++)
            //{
            //    if(renderParent[i].tag.Equals("IgnoreAddMaterial"))
            //    {

            //    }
            //}
            foreach (Renderer render in renderParent)
            {
                StartCoroutine(AddAndRemove(render));

                if (materialQueue != -1)
                    instanceMat.renderQueue = materialQueue;

                if (fadeInOutShaderColor != null)
                {
                    foreach (var inOutShaderColor in fadeInOutShaderColor)
                    {
                        inOutShaderColor.UpdateMaterial(instanceMat);
                    }
                }

                if (fadeInOutShaderFloat != null)
                {
                    foreach (var inOutShaderFloat in fadeInOutShaderFloat)
                    {
                        inOutShaderFloat.UpdateMaterial(instanceMat);
                    }
                }
                if (uvTextureAnimator != null)
                    uvTextureAnimator.SetInstanceMaterial(instanceMat, Vector2.zero);
            }
        }
    }

    public void SetMaterialQueue(int matlQueue)
    {
        materialQueue = matlQueue;
    }

    public int GetDefaultMaterialQueue()
    {
        return instanceMat.renderQueue;
    }

    private void OnDestroy()
    {
        RemoveNewMaterial();
    }

    public void RemoveNewMaterial()
    {
        if (renderParent == null)
            return;

        foreach (Renderer render in renderParent)
        {
            if (render)
            {
                var materials = render.sharedMaterials.ToList();
                materials.RemoveAll(m => m == instanceMat);
                render.sharedMaterials = materials.ToArray();
            }
        }
    }
}
