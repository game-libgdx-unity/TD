using UnitedSolution;using UnityEngine;
using System.Collections;

using TBTK2;

namespace TBTK2{

	public class TextureScroll : MonoBehaviour {

		//~ private LineRenderer lineR;
		//~ private float width=0;
		
		public Material mat;
		
		void Awake(){
			//~ lineR=gameObject.GetComponent<LineRenderer>();
			//~ width=lineR.width;
			mat=transform.GetComponent<Renderer>().material;
		}
		
		void OnEnable(){
			//~ StartCoroutine(Fade());
		}
		
		//~ IEnumerator Fade(){
			//~ float duration=0;
			//~ lineR.materials[0].SetColor("_TintColor", new Color(.5f, .5f, .5f, .5f));
			//~ while(duration<1){
				//~ lineR.materials[0].SetColor("_TintColor", new Color(.5f, .5f, .5f, (1f-duration)/2));
				//~ duration+=Time.fixedDeltaTime*4;
				//~ yield return new WaitForSeconds(Time.fixedDeltaTime);
			//~ }
			//~ lineR.materials[0].SetColor("_TintColor", new Color(.5f, .5f, .5f, 0));
		//~ }
		
		
		
		public Vector2 uvAnimationRate = new Vector2( 1.0f, 0.0f );
		Vector2 uvOffset = Vector2.zero;

		void Update(){
			uvOffset += ( uvAnimationRate * Time.deltaTime );
			mat.SetTextureOffset("_MainTex", uvOffset );
		}
		
	}

}
