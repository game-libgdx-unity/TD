using UnitedSolution;using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using System;

public class BeastAI : MonoBehaviour
{
    #region Fields 
    public float attackRange = 2;
    public int hitpoint = 100;
    public GameObject enemy;

    private Animator animator;
    private UnityEngine.AI.NavMeshAgent agent;
    CapsuleCollider collider;
    public int damage = 30;
    public UnityEvent OnDead;

    #endregion

    #region Initialization
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider>(); 
        if (enemy)
        {
            agent.destination = enemy.transform.position; //find a target
        }

    }
    #endregion

    #region Private Members

    private void Damage(int damage)
    {
        print("Hit zombie");
        if (hitpoint > 0)
        {
            hitpoint -= damage;
            if (hitpoint <= 0)
            {
                GoDie();
            }
        }
    }

    private void GoDie()
    {
        animator.Play("Die");
        collider.enabled = false;
        StartCoroutine(Remove());
        if (OnDead != null) OnDead.Invoke();
    }

    IEnumerator Remove()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    bool attacking = false;
    IEnumerator Attack()
    {
        attacking = true;
        yield return new WaitForSeconds(.5f);
        if (hitpoint > 0 && enemy != null)
        {
            enemy.SendMessage("Damage", damage);
        }
        yield return new WaitForSeconds(.5f);

        attacking = false;
    }

    private void Update()
    {
        if (hitpoint <= 0)
            return;

        if (agent.isOnNavMesh && enemy != null)
        {
            agent.destination = enemy.transform.position;
            float dis = Vector3.Distance(transform.position, enemy.transform.position);
            if (dis < attackRange)
            {
                print("attack");
                animator.SetTrigger("bite");
                if (!attacking)
                    StartCoroutine(Attack());
            }
        }
    }

    #endregion
}