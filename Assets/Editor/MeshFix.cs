using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System.Linq;
using System.IO;

public class MeshFix : Editor
{
    [MenuItem("Mesh Fix/Remove Combined Mesh")]
    public static void RemoveCombinedMesh()
    {
        Dictionary<string, Mesh> cachedMeshes = new Dictionary<string, Mesh>();
        Dictionary<string, int> duplicates = new Dictionary<string, int>();

        foreach (MeshFilter filter in FindObjectsOfType<MeshFilter>())
        {
            if (filter == null || filter.sharedMesh == null)
            {
                continue;
            }
            
            if (filter.gameObject.isStatic && filter.sharedMesh.name.StartsWith("Combined Mesh (root scene)"))
            {
                MeshCollider collider = filter.GetComponent<MeshCollider>();

                if (collider != null)
                {
                    filter.sharedMesh = collider.sharedMesh;
                    continue;
                }

                if (!filter.sharedMesh.isReadable)
                {
                    File.WriteAllText(Application.dataPath + "/" + AssetDatabase.GetAssetPath(filter.sharedMesh).Replace("Assets", ""), File.ReadAllText(AssetDatabase.GetAssetPath(filter.sharedMesh)).Replace("m_IsReadable: 0", "m_IsReadable: 1"));
                    AssetDatabase.Refresh();
                }

                filter.sharedMesh = ExtractSubmesh(filter.transform, filter.name, filter.sharedMesh, filter.GetComponent<MeshRenderer>().subMeshStartIndex);

                if (cachedMeshes.ContainsKey(filter.name))
                {
                    if (!MeshesAreTheSame(filter.sharedMesh, cachedMeshes[filter.name]))
                    {
                        if (!duplicates.ContainsKey(filter.name))
                        {
                            duplicates.Add(filter.name, 0);
                        }

                        cachedMeshes.Add(filter.name + "_" + duplicates[filter.name], filter.sharedMesh);
                        duplicates[filter.name]++;
                    }
                    else
                    {
                        filter.sharedMesh = cachedMeshes[filter.name];
                    }
                }
                else
                {
                    cachedMeshes.Add(filter.name, filter.sharedMesh);
                }
            }
        }

        if (!Directory.Exists(Application.dataPath + "/SplitSubmeshes"))
        {
            Directory.CreateDirectory(Application.dataPath + "/SplitSubmeshes");
        }

        if (!Directory.Exists(Application.dataPath + "/SplitSubmeshes/" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name))
        {
            Directory.CreateDirectory(Application.dataPath + "/SplitSubmeshes/" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        foreach (KeyValuePair<string, Mesh> cachedMesh in cachedMeshes)
        {
            AssetDatabase.CreateAsset(cachedMesh.Value, "Assets/SplitSubmeshes/" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "/" + cachedMesh.Key + ".asset");
        }
    }

    private static bool MeshesAreTheSame(Mesh x, Mesh y)
    {
        if (x.vertices.Length != y.vertices.Length)
        {
            return false;
        }

        for (int i = 0; i < x.vertices.Length; i++)
        {
            if (x.vertices[i] != y.vertices[i] || x.normals[i] != y.normals[i] || x.uv[i] != y.uv[i])
            {
                return false;
            }
        }

        return true;
    }

    private static Mesh ExtractSubmesh(Transform transform, string name, Mesh original, int subMeshIndex)
    {
        SubMeshDescriptor subMesh = original.GetSubMesh(subMeshIndex);

        Mesh mesh = new Mesh
        {
            vertices = RepositionVertices(transform, original.vertices.ToList().GetRange(subMesh.firstVertex, subMesh.vertexCount).ToArray()),
            normals = CheckMeshPart(subMesh, original, original.normals),
            tangents = CheckMeshPart(subMesh, original, original.tangents),
            boneWeights = CheckMeshPart(subMesh, original, original.boneWeights),
            uv = CheckMeshPart(subMesh, original, original.uv),
            uv2 = CheckMeshPart(subMesh, original, original.uv2),
            uv3 = CheckMeshPart(subMesh, original, original.uv3),
            uv4 = CheckMeshPart(subMesh, original, original.uv4),
            uv5 = CheckMeshPart(subMesh, original, original.uv5),
            uv6 = CheckMeshPart(subMesh, original, original.uv6),
            uv7 = CheckMeshPart(subMesh, original, original.uv7),
            uv8 = CheckMeshPart(subMesh, original, original.uv8),
            colors = CheckMeshPart(subMesh, original, original.colors),
            colors32 = CheckMeshPart(subMesh, original, original.colors32),
        };
        
        int[] triangles = original.triangles.ToList().GetRange(subMesh.indexStart, subMesh.indexCount).ToArray();

        for (int i = 0; i < triangles.Length; i++)
        {
            triangles[i] -= subMesh.firstVertex;
        }

        mesh.triangles = triangles;

        if (mesh.normals == null || mesh.normals.Length == 0)
        {
            mesh.RecalculateNormals();
        }

        mesh.Optimize();
        mesh.OptimizeIndexBuffers();

        mesh.RecalculateBounds();
        mesh.name = name;

        return mesh;
    }

    private static Vector3[] RepositionVertices(Transform transform, Vector3[] vertices)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            if (float.IsNaN(vertices[i].x) || float.IsInfinity(vertices[i].x))
            {
                vertices[i].x = 0f;
            }

            if (float.IsNaN(vertices[i].x) || float.IsInfinity(vertices[i].x))
            {
                vertices[i].y = 0f;
            }

            if (float.IsNaN(vertices[i].x) || float.IsInfinity(vertices[i].x))
            {
                vertices[i].z = 0f;
            }

            vertices[i] -= transform.position;
            vertices[i] = Quaternion.Inverse(transform.rotation) * vertices[i];

            vertices[i] = new Vector3(vertices[i].x / transform.localScale.x, vertices[i].y / transform.localScale.y, vertices[i].z / transform.localScale.z);
        }

        return vertices;
    }

    private static T[] CheckMeshPart<T>(SubMeshDescriptor subMesh, Mesh original, T[] originalPart)
    {
        if (originalPart == null || originalPart.Length != original.vertices.Length)
        {
            return null;
        }

        return originalPart.ToList().GetRange(subMesh.firstVertex, subMesh.vertexCount).ToArray();
    }
}