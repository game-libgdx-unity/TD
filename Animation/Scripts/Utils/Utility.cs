using UnitedSolution;using UnityEngine;
using System.Collections;
//using System.Collections.Generic;
public class CV
{
    public static Vector3 Vector3Zero = Vector3.zero;
    public static Vector3 Vector3One = Vector3.one;
    public static Vector3 Vector3Forward = Vector3.forward;
    public static Vector3 Vector3Back = Vector3.back;
    public static Vector3 Vector3Left = Vector3.left;
    public static Vector3 Vector3Right = Vector3.right;
    public static Vector3 Vector3Up = Vector3.up;
    public static Vector3 Vector3Down = Vector3.down;
    public static Quaternion QuaternionIdentity = Quaternion.identity;
    public static Quaternion Quaternion180Y = Quaternion.Euler(0f, 180f, 0f);

    public static Vector3[] Vector3ArrayEmpty = new Vector3[0];
    public static Vector2[] Vector2ArrayEmpty = new Vector2[0];
    public static int[] IntArrayEmpty = new int[0];
}

public class Utility : MonoBehaviour { 

    public static Vector3 GetWorldScale(Transform transform){
		Vector3 worldScale = transform.localScale;
		Transform parent = transform.parent;
		
		while (parent != null){
			worldScale = Vector3.Scale(worldScale,parent.localScale);
			parent = parent.parent;
		}
		
		return worldScale;
	}
	
	
	
	
	public static void DestroyColliderRecursively(Transform root){
		foreach(Transform child in root) {
			if(child.GetComponent<Collider>()!=null) {
				Destroy(child.GetComponent<Collider>());
			}
			DestroyColliderRecursively(child);
		}
	}
	
	public static void DisableColliderRecursively(Transform root){
		foreach(Transform child in root) {
			if(child.gameObject.GetComponent<Collider>()!=null)  child.gameObject.GetComponent<Collider>().enabled=false;
			DisableColliderRecursively(child);
		}
	}
	
	
	
	public static void SetMatRecursively(Transform root, string materialName){
		foreach(Transform child in root) {
			if(child.GetComponent<Renderer>()!=null){
				foreach(Material mat in child.GetComponent<Renderer>().materials)
					mat.shader=Shader.Find(materialName);
			}
			SetMatRecursively(child, materialName);
		}
	}
	
	public static void SetMatColorRecursively(Transform root, string colorName, Color color){
		foreach(Transform child in root) {
			if(child.GetComponent<Renderer>()!=null){
				foreach(Material mat in child.GetComponent<Renderer>().materials)  
					mat.SetColor(colorName, color);
			}
			SetMatColorRecursively(child, colorName, color);
		}
	}

	
}

