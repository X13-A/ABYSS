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

    }

    public async Task<GameObject> GenerateMap()
    {
        IProgress<float> progress = new Progress<float>(p =>
        {
            EventManager.Instance.Raise(new LoadingProgressUpdateEvent { progress = p, message = "Generating map" });
        });
        
        GameObject[] prefabMap = new GameObject[this.mapWidth * this.mapHeight];
        float[,] noiseMap = this.GenerateNoiseMap();


        GameObject map = new GameObject("Map");
        map.SetActive(false);
        for (int z = 0; z < this.mapHeight; z++)
        {
            for (int x = 0; x < this.mapWidth; x++)
            {
                float currentHeight = noiseMap[x, z];

                for (int i = 0; i < this.regions.Length; i++)
                {
                    if (currentHeight <= this.regions[i].height)
                    {
                        prefabMap[z * this.mapWidth + x] = this.regions[i].cubePrefab;
                        break;
                    }
                }

                for (int y = -2; y < 0; y++)
                {
                    GameObject cube = Instantiate(this.regions[2].cubePrefab);
                    cube.transform.position = new Vector3(x, (int)(this.heightCurve.Evaluate(noiseMap[x, z]) * this.heightMultiplier) + y, z);
                    cube.AddComponent<BoxCollider>();
                    cube.layer = LayerMask.NameToLayer("Ground");
                    cube.transform.SetParent(map.transform);
                }

                GameObject currentCube = Instantiate(prefabMap[z * this.mapWidth + x]);
                currentCube.transform.position = new Vector3(x, (int)(this.heightCurve.Evaluate(noiseMap[x, z]) * this.heightMultiplier), z);
                currentCube.AddComponent<BoxCollider>();
                currentCube.AddComponent<Rigidbody>();
                currentCube.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                currentCube.GetComponent<Rigidbody>().isKinematic = true;
                currentCube.layer = LayerMask.NameToLayer("Ground");
                currentCube.transform.SetParent(map.transform);

            }
            // Report progress
            progress.Report((float) z / this.mapHeight);
            // Yield control to the caller
            await Task.Yield();
        }
        map.SetActive(true);
        return map;
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
