using System;
using System.Collections;
using System.Collections.Generic;
using UnitedSolution;
using UnityEngine;

public class EffectController : MonoBehaviour {

    public float removalDelay = -1f;

    public Animator animator;
	// Use this for initialization
	void Start () {

        if (!animator)
            animator = GetComponent<Animator>();

        if(removalDelay < 0)
        {
            removalDelay = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        }

        StartCoroutine(RemovalRoutine());
    }

    private IEnumerator RemovalRoutine()
    {
        yield return new WaitForSeconds(removalDelay);
        ObjectPoolManager.Unspawn(gameObject);
    }
}
