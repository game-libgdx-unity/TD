using UnitedSolution;using UnityEngine;
using System.Collections;

public class Hover : MonoBehaviour {

	public float offset;
	
	// Use this for initialization
	void Start () {
		offset=Random.Range(-5f, 5f);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Vector3.up*0.018f*Mathf.Sin(6.5f*Time.time+offset)*Time.deltaTime);
	}
}
