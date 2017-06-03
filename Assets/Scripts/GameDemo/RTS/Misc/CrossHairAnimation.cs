using UnitedSolution;using UnityEngine;
using System.Collections;

public class CrossHairAnimation : MonoBehaviour {

	public RectTransform rectT;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float size=150+15*Mathf.Sin(Time.time*5);
		rectT.sizeDelta = new Vector2(size, size);
	}
}
