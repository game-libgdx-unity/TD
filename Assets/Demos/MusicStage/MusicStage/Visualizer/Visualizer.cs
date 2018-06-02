using UnityEngine;
using System.Collections;


public class Visualizer : MonoBehaviour
{
	public Reaktion.ReaktorLink spectrum1;
	public Reaktion.ReaktorLink spectrum2;
	public Reaktion.ReaktorLink spectrum3;
	public Reaktion.ReaktorLink spectrum4;
	public Reaktion.ReaktorLink spectrum5;
	
	public Vector4 vector4;
	public Vector4 vector3;
	public float[] spectrums = new float[5];

	void Update()
	{
		vector4 = new Vector4(spectrum1.Output, spectrum2.Output, spectrum3.Output, spectrum4.Output);
		vector3 = new Vector3(spectrum1.Output, spectrum2.Output, spectrum3.Output);
		spectrums[0] = spectrum1.Output;
		spectrums[1] = spectrum2.Output;
		spectrums[2] = spectrum3.Output;
		spectrums[3] = spectrum4.Output;
		spectrums[4] = spectrum5.Output;
	}

	void OnWillRenderObject()
	{
		if (GetComponent<Renderer>() == null || GetComponent<Renderer>().sharedMaterial == null) { return; }
		Material mat = GetComponent<Renderer>().material;

		if (Vector4.Dot(vector4, vector4) <= 1.0f)
		{
			mat.SetVector("_Spectra", vector4);
		}

		Camera cam = Camera.current;
		if (cam != null) {
			Matrix4x4 view = cam.worldToCameraMatrix;
			Matrix4x4 proj = cam.projectionMatrix;
			proj[2, 0] = proj[2, 0] * 0.5f + proj[3, 0] * 0.5f;
			proj[2, 1] = proj[2, 1] * 0.5f + proj[3, 1] * 0.5f;
			proj[2, 2] = proj[2, 2] * 0.5f + proj[3, 2] * 0.5f;
			proj[2, 3] = proj[2, 3] * 0.5f + proj[3, 3] * 0.5f;
			Matrix4x4 viewprojinv = (proj * view).inverse;
			mat.SetMatrix("_ViewProjectInverse", viewprojinv);
		}
	}
}
