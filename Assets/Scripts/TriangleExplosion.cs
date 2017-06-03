
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public static class TriangleExplosion
{
    public static IEnumerator SplitMesh(this GameObject obj, bool destroy = true, int max_triangle = 1000)
    {
        if (obj.GetComponent<MeshFilter>() == null || obj.GetComponent<SkinnedMeshRenderer>() == null)
        {
            yield return null;
        }

        if (obj.GetComponent<Collider>())
        {
            obj.GetComponent<Collider>().enabled = false;
        }

        Mesh M = new Mesh();
        if (obj.GetComponent<MeshFilter>())
        {
            M = obj.GetComponent<MeshFilter>().mesh;
        }
        else if (obj.GetComponent<SkinnedMeshRenderer>())
        {
            M = obj.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        }

        Material[] materials = new Material[0];
        if (obj.GetComponent<MeshRenderer>())
        {
            materials = obj.GetComponent<MeshRenderer>().materials;
        }
        else if (obj.GetComponent<SkinnedMeshRenderer>())
        {
            materials = obj.GetComponent<SkinnedMeshRenderer>().materials;
        }

        Vector3[] verts = M.vertices;
        Vector3[] normals = M.normals;
        Vector2[] uvs = M.uv;
        int triangle = 0;
        for (int submesh = 0; submesh < M.subMeshCount; submesh++)
        {
            int[] indices = M.GetTriangles(submesh);

            for (int i = 0; i < indices.Length; i += 4)
            {
                Vector3[] newVerts = new Vector3[4];
                Vector3[] newNormals = new Vector3[4];
                Vector2[] newUvs = new Vector2[4];
                for (int n = 0; n < 4; n++)
                {
                    int index = indices[i + n];
                    newVerts[n] = verts[index];
                    newUvs[n] = uvs[index];
                    newNormals[n] = normals[index];
                }

                Mesh mesh = new Mesh();
                mesh.vertices = newVerts;
                mesh.normals = newNormals;
                mesh.uv = newUvs;

                mesh.triangles = new int[] { 0, 1, 2, 2, 1, 0, 1,2,3,3,2,1 };

                GameObject GO = new GameObject("Quad " + (i / 4));
                //GO.layer = LayerMask.NameToLayer("Particle");
                GO.transform.position = obj.transform.position;
                GO.transform.rotation = obj.transform.rotation;
                GO.AddComponent<MeshRenderer>().material = materials[submesh];
                GO.AddComponent<MeshFilter>().mesh = mesh;
                GO.AddComponent<BoxCollider>();
                Vector3 explosionPos = new Vector3(obj.transform.position.x + Random.Range(-0.5f, 0.5f), obj.transform.position.y + Random.Range(0f, 0.5f), obj.transform.position.z + Random.Range(-0.5f, 0.5f));
                GO.AddComponent<Rigidbody>().AddExplosionForce(Random.Range(300, 500), explosionPos, 5);
                GameObject.Destroy(GO, 5 + Random.Range(0.0f, 5.0f));

                triangle++;
                if(triangle > max_triangle)
                {
                    break;
                }
            }
            if (triangle > max_triangle)
            {
                break;
            }
        }

        yield return null;

        if (destroy == true)
        {
            GameObject.Destroy(obj);
        }

    }
}