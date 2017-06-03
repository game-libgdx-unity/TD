using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitedSolution;

public class UnitStat : MonoBehaviour
{
    public float hitPoint = 100;
    public float max_hitPoint = 100;

    public bool allowWandering = false;
    public float idleDuration = 1f;
    public float speed = 1f;
    public float distanceToScan = 10f;
    public float distanceToAttack = 1.2f;
    public bool allowScanForEnemy = false;
    public LayerMask enemyMark;
    public GameObject target;
    private Animator animator;

    public bool IsAlive() { return hitPoint > 0; }
    public bool IsDead() { return hitPoint <= 0; }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnIdleStateEnter()
    {
        Run.After(idleDuration, () => animator.SetFloat("speed", 1f));
        Run.After(3f, () => animator.SetFloat("speed", 0f));
    }
    public void OnIdleStateUpdate()
    {
        if (animator.GetFloat("speed") > 0f)
        {
            transform.Translate(Time.deltaTime * speed, 0, 0);
        }
    }

    public void OnIdleStateExit()
    {

    }
}