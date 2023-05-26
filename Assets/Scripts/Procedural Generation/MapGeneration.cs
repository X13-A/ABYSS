using SDD.Events;
using System;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public struct TerrainType
{
    public float height;
    public GameObject cubePrefab;
    public BlockType blockType;
}

public class MapGeneration : MonoBehaviour
{

    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;

    public int MapHeight => mapHeight;
    public int MapWidth => mapHeight;

    [SerializeField] private int octaves;
    [Min(0.1f)]
    [SerializeField] private float noiseScale;
    [SerializeField] private float persistance;
    [SerializeField] private float lacunarity;

    [SerializeField] private float heightMultiplier;
    [SerializeField] private AnimationCurve heightCurve;

    [SerializeField] private TerrainType[] firstLevel;
    [SerializeField] private TerrainType[] secondLevel;
    [SerializeField] private TerrainType[] thirdLevel;
    [SerializeField] private TerrainType[] fourthLevel;

    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private Vector3 playerSpawnCoords;
    
    private BlockType[,,] blocksMap;
    private TerrainType[][] levelArray;
    private float[,] noiseMap;
    private int[,] topBlocksHeight;

    public BlockType[,,] BlocksMap => blocksMap;
    public TerrainType[][] LevelArray => levelArray;
    public float[,] NoiseMap => noiseMap;


    private void Start()
    {
        this.levelArray = new TerrainType[][] { firstLevel, secondLevel, thirdLevel, fourthLevel };
    }

    private void initBlocksMap()
    {
        this.blocksMap = new BlockType[this.mapWidth, this.mapHeight, this.mapWidth];
        for (int x = 0; x < this.mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int z = 0; z < this.mapWidth; z++)
                {
                    blocksMap[x, y, z] = BlockType.AIR;
                }
            }
        }
    }

    public async Task<GameObject> GenerateMap(int level)
    {
        // Set progress
        IProgress<float> progress = new Progress<float>(p =>
        {
            EventManager.Instance.Raise(new LoadingProgressUpdateEvent { progress = p, message = "Generating map" });
        });

        // Init NoiseMap and Blocks Storage
        this.GenerateNoiseMap();
        this.initBlocksMap();
        this.topBlocksHeight = new int[this.mapWidth, this.mapWidth];

        // Init map object
        GameObject map = new GameObject("Map");
        map.SetActive(false);

        for (int z = 0; z < this.mapHeight; z++)
        {
            for (int x = 0; x < this.mapWidth; x++)
            {
                // Read noise map
                float currentHeight = this.noiseMap[x, z];

                // Pick block type
                GameObject currentBlockPrefab = null;
                BlockType currentBlockType = BlockType.AIR;
                for (int i = 0; i < this.levelArray[level].Length; i++)
                {
                    if (currentHeight <= this.levelArray[level][i].height)
                    {
                        currentBlockPrefab = this.levelArray[level][i].cubePrefab;
                        currentBlockType = this.levelArray[level][i].blockType;
                        break;
                    }
                }

                // Fill underneath the block
                int blockHeight = (int) (this.heightCurve.Evaluate(this.noiseMap[x, z]) * this.heightMultiplier);
                this.topBlocksHeight[x, z] = blockHeight;
                for (int y = 0; y < blockHeight; y++)
                {
                    GameObject cube = Instantiate(this.levelArray[level][2].cubePrefab);
                    cube.transform.position = new Vector3(x, y, z);
                    
                    cube.AddComponent<BoxCollider>();
                    cube.layer = LayerMask.NameToLayer("Ground");
                    cube.transform.SetParent(map.transform);
                }

                // Fill Blocks Storage
                Vector3 blockPos = new Vector3(x, blockHeight, z);
                this.blocksMap[(int) blockPos.x, (int) blockPos.y, (int) blockPos.z] = currentBlockType;

                // Place the top block
                GameObject currentCube = Instantiate(currentBlockPrefab);
                currentCube.transform.position = blockPos;
                currentCube.AddComponent<Rigidbody>();
                currentCube.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                currentCube.GetComponent<Rigidbody>().isKinematic = true;
                currentCube.transform.SetParent(map.transform);
            }

            // Progress
            progress.Report((float) z / this.mapHeight);
            await Task.Yield();
        }

        // Save data across scenes
        LevelData.Instance.BlocksMap = this.blocksMap;
        LevelData.Instance.TopBlocksHeight = this.topBlocksHeight;
        LevelData.Instance.MapHeight = this.mapHeight;
        LevelData.Instance.MapWidth = this.mapWidth;

        // Set player spawn
        GameObject playerSpawnObject = Instantiate(new GameObject("PlayerSpawn"));
        playerSpawnObject.AddComponent<PlayerSpawn>();
        playerSpawnObject.transform.SetParent(map.transform);
        this.setPlayerSpawnHeight(playerSpawnObject, this.noiseMap);

        // Generate portal
        GameObject portal = Instantiate(this.portalPrefab, new Vector3(50, (int) (this.heightCurve.Evaluate(this.noiseMap[50, 50]) * this.heightMultiplier) + this.portalPrefab.transform.localScale.y / 2, 50), Quaternion.identity);
        portal.GetComponent<ScenePortal>().LevelGenerated = LevelManager.Instance.CurrentLevel + 1;
        portal.transform.SetParent(map.transform);
        map.SetActive(true);
        return map; 
    }

    private void setPlayerSpawnHeight(GameObject spawn, float[,] noiseMap)
    {
        int x = Mathf.RoundToInt(this.playerSpawnCoords.x);
        int z = Mathf.RoundToInt(this.playerSpawnCoords.z);

        float height = Mathf.Ceil(noiseMap[x, z]);
        spawn.transform.position = new Vector3
        (
            this.playerSpawnCoords.x,
            height,
            this.playerSpawnCoords.z
        );
    }

    private void GenerateNoiseMap()
    {
        System.Random rnd = new();
        float[,] noiseMap = new float[mapWidth, mapHeight];
        System.Random randomNumberGenerator = new(rnd.Next(-100000, 100000));
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
        this.noiseMap = noiseMap;
    }
}
