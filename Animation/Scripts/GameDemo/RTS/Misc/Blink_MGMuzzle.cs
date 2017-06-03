using UnitedSolution;using UnityEngine;
using System.Collections;

public class Blink_MGMuzzle : MonoBehaviour {

	private float posOffset=0.025f;
	private Renderer ren;
	private Transform thisT;
	
	// Use this for initialization
	void Start () {
		thisT=transform;
		ren=transform.GetComponent<Renderer>();
	}
	
	void OnEnable(){
		if(ren!=null) ren.enabled=true;
		StartCoroutine(Blinking());
	}
	
	IEnumerator Blinking(){
		while(true){
			while(ren==null) yield return null;
			float x=Random.Range(-posOffset, posOffset);
			float y=Random.Range(-posOffset, posOffset);
			float z=Random.Range(-posOffset, posOffset);
			thisT.localPosition=new Vector3(0, 0, 0.5f)+new Vector3(x, y, z);
			ren.enabled=!ren.enabled;
			yield return new WaitForSeconds(0.05f);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
