using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

[System.Serializable]
public struct TerrainType
{
    public float height;
    public GameObject cubePrefab;
}

public class MapGeneration : MonoBehaviour
{

    [SerializeField] int mapWidth;
    [SerializeField] int mapHeight;

    [SerializeField] int octaves;
    [Min(0.1f)]
    [SerializeField] float noiseScale;
    [SerializeField] float persistance;
    [SerializeField] float lacunarity;

    [SerializeField] int seed;
    [SerializeField] float heightMultiplier;
    [SerializeField] AnimationCurve heightCurve;

    [SerializeField] TerrainType[] regions;

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        GameObject[] prefabMap = new GameObject[this.mapWidth * this.mapHeight];
        float[,] noiseMap = this.GenerateNoiseMap();

        for (int y = 0; y < this.mapHeight; y++)
        {
            for (int x = 0; x < this.mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];

                for (int i = 0; i < this.regions.Length; i++)
                {
                    if (currentHeight <= this.regions[i].height)
                    {
                        prefabMap[y * this.mapWidth + x] = this.regions[i].cubePrefab;
                        break;
                    }
                }
            }
        }

        for (int y = 0; y < this.mapHeight; y++)
        {
            for (int x = 0; x < this.mapWidth; x++)
            {
                GameObject currentCube = Instantiate(prefabMap[y * this.mapWidth + x]);
                currentCube.transform.position = new Vector3(x, (int)(this.heightCurve.Evaluate(noiseMap[x, y]) * this.heightMultiplier), y);
                currentCube.AddComponent<BoxCollider>();
                currentCube.transform.SetParent(gameObject.transform);
            }
        }
    }

    private float[,] GenerateNoiseMap()
    {
        float[,] noiseMap = new float[this.mapWidth, this.mapHeight];
        System.Random randomNumberGenerator = new(this.seed);
        Vector2[] octaveOffsets = new Vector2[this.octaves];

        for (int i = 0; i < this.octaves; i++)
        {
            float offsetX = randomNumberGenerator.Next(-100000, 100000);
            float offsetY = randomNumberGenerator.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = this.mapWidth / 2f;
        float halfHeight = this.mapHeight / 2f;

        for (int y = 0; y < this.mapHeight; y++)
        {
            for (int x = 0; x < this.mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < this.octaves; i++)
                {
                    float sampleX = (x - halfWidth) * frequency / this.noiseScale + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) * frequency / this.noiseScale + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    noiseHeight += perlinValue * amplitude;

                    amplitude *= this.persistance;
                    frequency *= this.lacunarity;
                }

                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < this.mapHeight; y++)
        {
            for (int x = 0; x < this.mapWidth; x++)
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
        }

        return noiseMap;
    }
}
