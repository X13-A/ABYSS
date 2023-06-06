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
    [SerializeField] private GameObject chestPrefab;


    private BlockType[,,] blocksMap;
    private TerrainType[][] levelArray;
    private float[,] noiseMap;
    private int[,] topBlocksHeight;

    public BlockType[,,] BlocksMap => blocksMap;
    public TerrainType[][] LevelArray => levelArray;
    public float[,] NoiseMap => noiseMap;


    private void Start()
    {
        levelArray = new TerrainType[][] { firstLevel, secondLevel, thirdLevel, fourthLevel };
    }

    private void initBlocksMap()
    {
        blocksMap = new BlockType[mapWidth, mapHeight, mapWidth];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int z = 0; z < mapWidth; z++)
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
        GenerateNoiseMap();
        initBlocksMap();
        topBlocksHeight = new int[mapWidth, mapWidth];

        // Init map object
        GameObject map = new GameObject("Map");
        map.SetActive(false);

        // Generate treasure chest
        int chestDepth = 3;
        float chestSpawnRadius = Mathf.Max(MapWidth / 2f - 2f, 0f);
        Vector2 chestPoint = new Vector2(mapWidth / 2, mapWidth / 2) + (UnityEngine.Random.insideUnitCircle) * chestSpawnRadius;
        int chestHeight = (int) (heightCurve.Evaluate(noiseMap[(int) chestPoint.x, (int) chestPoint.y]) * heightMultiplier) - chestDepth;
        chestHeight = Math.Max(chestHeight, 0); // Prevent chest from spawning under the map
        Vector3 chestPos = new Vector3((int) chestPoint.x, chestHeight, (int) chestPoint.y);
        GameObject chest = Instantiate(chestPrefab);
        chest.transform.position = chestPos;

        chest.transform.SetParent(map.transform);
        for (int z = 0; z < mapHeight; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // Read noise map
                float currentHeight = noiseMap[x, z];

                if (Vector2.Distance(new Vector2(MapWidth/2, MapWidth/2), new Vector2(x, z)) > MapWidth/2)
                {
                    continue;
                }

                // Pick block type
                GameObject currentBlockPrefab = null;
                BlockType currentBlockType = BlockType.AIR;
                for (int i = 0; i < levelArray[level].Length; i++)
                {
                    if (currentHeight <= levelArray[level][i].height)
                    {
                        currentBlockPrefab = levelArray[level][i].cubePrefab;
                        currentBlockType = levelArray[level][i].blockType;
                        break;
                    }
                }

                if (currentBlockType == BlockType.AIR) continue;

                // Fill underneath the block
                int blockHeight = (int) (heightCurve.Evaluate(noiseMap[x, z]) * heightMultiplier);
                topBlocksHeight[x, z] = blockHeight;
                for (int y = 0; y < blockHeight; y++)
                {
                    // Avoid placing on chest
                    if (new Vector3(x, y, z) == chestPos)
                    {
                        continue;
                    }
                    GameObject cube = Instantiate(levelArray[level][2].cubePrefab);
                    cube.transform.position = new Vector3(x, y, z);

                    cube.AddComponent<BoxCollider>();
                    cube.layer = LayerMask.NameToLayer("Ground");
                    cube.transform.SetParent(map.transform);
                }

                // Fill Blocks Storage
                Vector3 blockPos = new Vector3(x, blockHeight, z);
                blocksMap[(int) blockPos.x, (int) blockPos.y, (int) blockPos.z] = currentBlockType;

                // Place the top block
                GameObject currentCube = Instantiate(currentBlockPrefab);
                currentCube.transform.position = blockPos;
                currentCube.AddComponent<Rigidbody>();
                currentCube.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                currentCube.GetComponent<Rigidbody>().isKinematic = true;
                currentCube.transform.SetParent(map.transform);
            }

            // Progress
            progress.Report((float) z / mapHeight);
            await Task.Yield();
        }


        // Set player spawn
        GameObject playerSpawnObject = Instantiate(new GameObject("PlayerSpawn"));
        playerSpawnObject.AddComponent<PlayerSpawn>();
        playerSpawnObject.transform.SetParent(map.transform);
        setPlayerSpawnHeight(playerSpawnObject, noiseMap);

        // Generate portal
        Vector3 portalPos = new Vector3(50, (int) (heightCurve.Evaluate(noiseMap[50, 50]) * heightMultiplier) + portalPrefab.transform.localScale.y / 2, 50);
        GameObject portal = Instantiate(portalPrefab, portalPos, Quaternion.identity);
        portal.GetComponent<ScenePortal>().LevelGenerated = LevelManager.Instance.CurrentLevel + 1;
        portal.transform.SetParent(map.transform);

        // Save data across scenes
        LevelData.Instance.BlocksMap = blocksMap;
        LevelData.Instance.TopBlocksHeight = topBlocksHeight;
        LevelData.Instance.MapHeight = mapHeight;
        LevelData.Instance.MapWidth = mapWidth;
        LevelData.Instance.PortalPos = portalPos;
        LevelData.Instance.TreasurePos = chestPos;

        map.SetActive(true);
        return map;
    }

    private void setPlayerSpawnHeight(GameObject spawn, float[,] noiseMap)
    {
        int x = Mathf.RoundToInt(playerSpawnCoords.x);
        int z = Mathf.RoundToInt(playerSpawnCoords.z);

        float height = Mathf.Ceil(noiseMap[x, z]);
        spawn.transform.position = new Vector3
        (
            playerSpawnCoords.x,
            height,
            playerSpawnCoords.z
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
