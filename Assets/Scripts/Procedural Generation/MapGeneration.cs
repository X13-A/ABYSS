using SDD.Events;
using System;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public struct TerrainType
{
    public float height;
    public GameObject cubePrefab;
}

public class MapGeneration : MonoBehaviour
{

    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;

    [SerializeField] private int octaves;
    [Min(0.1f)]
    [SerializeField] private float noiseScale;
    [SerializeField] private float persistance;
    [SerializeField] private float lacunarity;

    [SerializeField] private int seed;
    [SerializeField] private float heightMultiplier;
    [SerializeField] private AnimationCurve heightCurve;

    [SerializeField] private TerrainType[] regions;

    private void Start()
    {

    }

    public async Task<GameObject> GenerateMap()
    {
        IProgress<float> progress = new Progress<float>(p =>
        {
            EventManager.Instance.Raise(new LoadingProgressUpdateEvent { progress = p, message = "Generating map" });
        });
        
        GameObject[] prefabMap = new GameObject[mapWidth * mapHeight];
        float[,] noiseMap = GenerateNoiseMap();


        GameObject map = new GameObject("Map");
        map.SetActive(false);
        for (int z = 0; z < mapHeight; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, z];

                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        prefabMap[z * mapWidth + x] = regions[i].cubePrefab;
                        break;
                    }
                }

                for (int y = -2; y < 0; y++)
                {
                    GameObject cube = Instantiate(regions[2].cubePrefab);
                    cube.transform.position = new Vector3(x, (int)(heightCurve.Evaluate(noiseMap[x, z]) * heightMultiplier) + y, z);
                    cube.AddComponent<BoxCollider>();
                    cube.layer = LayerMask.NameToLayer("Ground");
                    cube.transform.SetParent(map.transform);
                }

                GameObject currentCube = Instantiate(prefabMap[z * mapWidth + x]);
                currentCube.transform.position = new Vector3(x, (int)(heightCurve.Evaluate(noiseMap[x, z]) * heightMultiplier), z);
                currentCube.AddComponent<BoxCollider>();
                currentCube.AddComponent<Rigidbody>();
                currentCube.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                currentCube.GetComponent<Rigidbody>().isKinematic = true;
                currentCube.layer = LayerMask.NameToLayer("Ground");
                currentCube.transform.SetParent(map.transform);

            }
            // Report progress
            progress.Report((float) z / mapHeight);
            // Yield control to the caller
            await Task.Yield();
        }
        map.SetActive(true);
        return map;
    }

    private float[,] GenerateNoiseMap()
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];
        System.Random randomNumberGenerator = new(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = randomNumberGenerator.Next(-100000, 100000);
            float offsetY = randomNumberGenerator.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) * frequency / noiseScale + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) * frequency / noiseScale + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }

                if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }
        return noiseMap;
    }
}
