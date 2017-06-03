using UnitedSolution;using UnityEngine;
using System.Collections;
using UnitedSolution;

public class SelfActivator : MonoBehaviour
{

    public float delay = 2f;
    public GameObject ActivationEffect;
    public GameObject ActivationObject;
    private Unit unit; 

    void Awake()
    {
        unit = GetComponent<Unit>();
        //unit.enabled = false;
        ActivationObject.SetActive(false);

        StartCoroutine(ActivateRoutine());
    }

    IEnumerator ActivateRoutine()
    {
        yield return new WaitForSeconds(.1f);
        GameObject gameObject = ObjectPoolManager.Spawn(ActivationEffect, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(delay);
        ObjectPoolManager.Unspawn(gameObject);
        ActivationObject.SetActive(true);
        //unit.enabled = true;
        //this.enabled = false;
    }

    private void SetActiveComponents(bool enabled)
    {
        //foreach (Renderer ren in renderers)
        //    ren.enabled = enabled;
        //unit.enabled = enabled;
    }
}
