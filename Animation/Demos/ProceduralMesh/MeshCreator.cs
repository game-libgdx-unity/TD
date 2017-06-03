using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshCreator : MonoBehaviour
{

    public int width = 50;
    public int height = 50;
    // Use this for initialization
    void Start()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        Vector3[] vertices =
        {
            new Vector3(0,0,0), new Vector3(0,width,0),new Vector3(0,height,0),new Vector3(width,height,0)
        };
        Vector3[] normals =
        {
            -Vector3.forward, -Vector3.forward,-Vector3.forward,-Vector3.forward
        };
        int[] triangles = { 0, 2, 1, 2, 3, 1 };
        Vector2[] uv = { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        mf.mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
