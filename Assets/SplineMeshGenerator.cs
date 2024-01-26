using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Splines;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SplineMeshGenerator : MonoBehaviour
{
    public SplineContainer splineContainer;
    public float roadWidth = 2f;
    public int resolution = 10;

    private void Start()
    {
        GenerateMesh();
    }

    [ContextMenu("GenerateMesh")]
    private void GenerateMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        int numPoints = resolution;
        int numVertices = numPoints * 2;
        int numTriangles = (numPoints - 1) * 2;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numTriangles * 3];

        int vertexIndex = 0;
        int triangleIndex = 0;

        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            Vector3 pointOnSpline = splineContainer.Spline.EvaluatePosition(t);

            // Calculate a perpendicular direction without using tangent
            Vector3 perpendicularDir = new Vector3(0, 0, 1); // Adjust this based on your spline orientation
            Vector3 offset = Vector3.Cross(perpendicularDir, Vector3.up).normalized * roadWidth;

            vertices[vertexIndex] = pointOnSpline + offset;
            vertices[vertexIndex + 1] = pointOnSpline - offset;

            GameObject go1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go1.transform.position = pointOnSpline + offset;
            GameObject go2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go2.transform.position = pointOnSpline - offset;

            if (i < numPoints - 1)
            {
                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + 1;
                triangles[triangleIndex + 2] = vertexIndex + 2;

                triangles[triangleIndex + 3] = vertexIndex + 1;
                triangles[triangleIndex + 4] = vertexIndex + 3;
                triangles[triangleIndex + 5] = vertexIndex + 2;

                vertexIndex += 2;
                triangleIndex += 6;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Optional: You may want to recalculate normals and set other mesh properties here.
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}