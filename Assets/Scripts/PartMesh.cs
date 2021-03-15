using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartMesh
{
    private List<Vector3> verticies = new List<Vector3>();
    private List<Vector3> normals = new List<Vector3>();
    private List<List<int>> triangles = new List<List<int>>();
    private List<Vector2> uvs = new List<Vector2>();

    public void AddTriangle(int submesh, Vector3 vert1, Vector3 vert2, Vector3 vert3, Vector3 normal1, Vector3 normal2, Vector3 normal3, 
        Vector3 uv1, Vector3 uv2, Vector3 uv3)
    {
        triangles[submesh].Add(verticies.Count);
        verticies.Add(vert1);
        triangles[submesh].Add(verticies.Count);
        verticies.Add(vert2);
        triangles[submesh].Add(verticies.Count);
        verticies.Add(vert3);
        normals.Add(normal1);
        normals.Add(normal2);
        normals.Add(normal3);
        uvs.Add(uv1);
        uvs.Add(uv2);
        uvs.Add(uv3);
    }

    public void MakeGameobject()
    {
        GameObject createdObject = new GameObject();

        var mesh = new Mesh();
        mesh.vertices = verticies.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        for(int i = 0; i < triangles.Count; i++)
        {
            mesh.SetTriangles(triangles[i], i, true);
        }
        Bounds bounds = mesh.bounds;

        var renderer = createdObject.AddComponent<MeshRenderer>();
        
    }
}
