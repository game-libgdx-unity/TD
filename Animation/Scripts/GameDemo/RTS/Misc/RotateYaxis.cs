using UnitedSolution;using UnityEngine;
using System.Collections;

public class RotateYaxis : MonoBehaviour {

	public float speed=5;
	
	private Transform thisT;
	
	// Use this for initialization
	void Start () {
		thisT=transform;
	}
	
	// Update is called once per frame
	void Update () {
	
		thisT.Rotate(Vector3.up*speed*Time.deltaTime*35);
		
	}
}
