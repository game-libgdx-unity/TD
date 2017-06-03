using UnitedSolution;
using UnityEngine;
using System.Collections;
using UnitedSolution;

public class SelfDeactivator : MonoBehaviour
{

    public bool useObjectPool = true;
    public float duration = 2;

    public bool resetParticleEffects = true;
    ParticleSystem[] particles;
    void Awake()
    {
        particles = GetComponentsInChildren<ParticleSystem>();

        AbilityBehavior ab = GetComponent<AbilityBehavior>();
        if (ab && ab.ability.effect.duration > .1f)
        {
            duration = ab.ability.effect.duration;
        }
    }

    void OnEnable()
    {
        StartCoroutine(DeactivateRoutine());
        if (particles.Length > 0)
        {
            foreach (ParticleSystem ps in particles)
            {
                if (ps)
                    ps.Play(true);
            }
        }
    }

    void OnDisable()
    {
        if (particles.Length > 0)
            foreach (ParticleSystem ps in particles)
            {
                if (ps)
                {
                    ps.Stop();
                    ps.Clear();
                }
            }
    }

    IEnumerator DeactivateRoutine()
    {
        yield return new WaitForSeconds(duration);
        if (useObjectPool) ObjectPoolManager.Unspawn(gameObject);
        else Destroy(gameObject);
    }

}
