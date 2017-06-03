using UnitedSolution;using UnityEngine;
using System.Collections;

public class LaserFade : MonoBehaviour {

	private LineRenderer lineR;
	//~ private float width=0;
	
	void Awake(){
		lineR=gameObject.GetComponent<LineRenderer>();
		//~ width=lineR.width;
	}
	
	void OnEnable(){
		StartCoroutine(Fade());
	}
	
	IEnumerator Fade(){
		float duration=0;
		lineR.materials[0].SetColor("_TintColor", new Color(.5f, .5f, .5f, .5f));
		while(duration<1){
			lineR.materials[0].SetColor("_TintColor", new Color(.5f, .5f, .5f, (1f-duration)/2));
			duration+=Time.fixedDeltaTime*1.5f;
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
		lineR.materials[0].SetColor("_TintColor", new Color(.5f, .5f, .5f, 0));
	}
	
	
	
	public Vector2 uvAnimationRate = new Vector2( 1.0f, 0.0f );
    Vector2 uvOffset = Vector2.zero;

    void Update(){
        uvOffset += ( uvAnimationRate * Time.deltaTime );
		lineR.materials[ 0 ].SetTextureOffset("_MainTex", uvOffset );
	}
}
