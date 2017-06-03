using UnitedSolution;using UnityEngine;
using System.Collections;

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]

public class CombineMesh : MonoBehaviour {

	
	void Start(){
		Quaternion startRotation=transform.localRotation;

		foreach(Transform child in transform)
			child.position += transform.position;
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
	   
		MeshFilter[] meshFilters = (MeshFilter[]) GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length-1];
		int index = 0;
		for (int i = 0; i < meshFilters.Length; i++)
		{
			if (meshFilters[i].sharedMesh == null) continue;
			combine[index].mesh = meshFilters[i].sharedMesh;
			combine[index++].transform = meshFilters[i].transform.localToWorldMatrix;
			meshFilters[i].GetComponent<Renderer>().enabled = false;
		}
		GetComponent<MeshFilter>().mesh = new Mesh();
		GetComponent<MeshFilter>().mesh.CombineMeshes (combine);
		GetComponent<Renderer>().material = meshFilters[1].GetComponent<Renderer>().sharedMaterial;
		
		transform.localRotation=startRotation;
	}

}
