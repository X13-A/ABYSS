using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, Color[] colorMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData(width, height);
        int vertexIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);
                if (x < width - 1 && y < height - 1)
                {
                    /*meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);*/
                    GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    newCube.transform.position = new Vector3(topLeftX + x, (int) (heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier), topLeftZ - y);
                    newCube.GetComponent<Renderer>().material.color = colorMap[y * heightMap.GetLength(0) + x];
                }
                vertexIndex++;
            }
        }
        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        this.vertices = new Vector3[meshWidth * meshHeight];
        this.triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        this.uvs = new Vector2[meshWidth * meshHeight];
    }

    public void AddTriangle(int a, int b, int c)
    {
        this.triangles[triangleIndex] = a;
        this.triangles[triangleIndex + 1] = b;
        this.triangles[triangleIndex + 2] = c;
        this.triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = this.vertices;
        mesh.triangles = this.triangles;
        mesh.uv = this.uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}