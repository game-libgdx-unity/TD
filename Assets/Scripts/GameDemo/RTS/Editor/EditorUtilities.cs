using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

public class EditorUtilities : EditorWindow
{
    static List<string> layers;
    static string[] layerNames;

    public static LayerMask LayerMaskField(Rect rect, string label, LayerMask selected)
    {

        if (layers == null)
        {
            layers = new List<string>();
            layerNames = new string[4];
        }
        else
        {
            layers.Clear();
        }

        int emptyLayers = 0;
        for (int i = 0; i < 32; i++)
        {
            string layerName = LayerMask.LayerToName(i);

            if (layerName != "")
            {

                for (; emptyLayers > 0; emptyLayers--)
                    layers.Add("Layer " + (i - emptyLayers));

                layers.Add(layerName);
            }
            else
            {
                emptyLayers++;
            }
        }

        if (layerNames.Length != layers.Count)
        {
            layerNames = new string[layers.Count];
        }
        for (int i = 0; i < layerNames.Length; i++) layerNames[i] = layers[i];


        selected.value = EditorGUI.MaskField(rect, label, selected.value, layerNames);

        return selected;
    }

    public static Vector2 DrawButton(Rect rect, Vector2 Space, String label, Action onClicked)
    {
        if (GUI.Button(rect, label))
        {
            if(onClicked!=null)
            {
                onClicked();
            }
        }

        return new Vector2(rect.x + rect.width + Space.x, rect.y + rect.height + Space.y);
    }

public static bool DrawSprite(Rect rect, Sprite sprite, bool addXButton=false, bool drawBox=true){
		if(drawBox) GUI.Box(rect, "");
		
		if(sprite!=null){
			Texture t = sprite.texture;
			Rect tr = sprite.textureRect;
			Rect r = new Rect(tr.x / t.width, tr.y / t.height, tr.width / t.width, tr.height / t.height );
			
			rect.x+=2;
			rect.y+=2;
			rect.width-=4;
			rect.height-=4;
			GUI.DrawTextureWithTexCoords(rect, t, r);
		}
		
		if(addXButton){
			rect.width=12;	rect.height=12;
			bool flag=GUI.Button(rect, "X", GetXButtonStyle());
			return flag;
		}
		
		return false;
	}
	
	//a guiStyle used to draw the button to delete sprite icon on TowerDB, CreepDB and ResourceDB Editor
	private static GUIStyle xButtonStyle;
	public static GUIStyle GetXButtonStyle(){
		if(xButtonStyle==null){
			xButtonStyle=new GUIStyle("Button");
			xButtonStyle.alignment=TextAnchor.MiddleCenter;
			xButtonStyle.padding=new RectOffset(0, 0, 0, 0);
		}
		return xButtonStyle;
	}
	
	public delegate void SetObjListCallback(List<GameObject> objHList, string[] objHLabelList);
	
	public static void GetObjectHierarchyList(GameObject obj, SetObjListCallback callback){
		List<GameObject> objHList=new List<GameObject>();
		List<string> tempLabelList=new List<string>();
		
		HierarchyList hList=GetTransformInHierarchy(obj.transform, 0);
		
		objHList.Add(null);
		tempLabelList.Add(" - ");
		
		for(int i=0; i<hList.ListT.Count; i++){
			objHList.Add(hList.ListT[i].gameObject);
		}
		for(int i=0; i<hList.ListName.Count; i++){
			while(tempLabelList.Contains(hList.ListName[i])) hList.ListName[i]+=".";
			tempLabelList.Add(hList.ListName[i]);
		}
		
		string[] objHLabelList=new string[tempLabelList.Count];
		for(int i=0; i<tempLabelList.Count; i++) objHLabelList[i]=tempLabelList[i];
		
		callback(objHList, objHLabelList);
	}
	
	
	private static HierarchyList GetTransformInHierarchy(Transform transform, int depth){
		HierarchyList hl=new HierarchyList();
		
		hl=GetTransformInHierarchyRecursively(transform, depth);
		
		hl.ListT.Insert(0, transform);
		hl.ListName.Insert(0, "-"+transform.name);
		
		return hl;
	}
	private static HierarchyList GetTransformInHierarchyRecursively(Transform transform, int depth){
		HierarchyList hList=new HierarchyList();
		depth+=1;
		foreach(Transform t in transform){
			string label="";
			for(int i=0; i<depth; i++) label+="   ";
			
			hList.ListT.Add(t);
			hList.ListName.Add(label+"-"+t.name);
			
			HierarchyList tempHL=GetTransformInHierarchyRecursively(t, depth);
			foreach(Transform tt in tempHL.ListT){
				hList.ListT.Add(tt);
			}
			foreach(string ll in tempHL.ListName){
				hList.ListName.Add(ll);
			}
		}
		return hList;
	}
	
	private class HierarchyList{
		public List<Transform> ListT=new List<Transform>();
		public List<string> ListName=new List<string>();
	}
	
	
}
