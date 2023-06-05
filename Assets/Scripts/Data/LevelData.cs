using UnityEngine;

public class LevelData : MonoBehaviour
{
    public static LevelData m_Instance;
    public static LevelData Instance => m_Instance;

    // HACK: encapsulation is not safe
    private BlockType[,,] blocksMap;
    public BlockType[,,] BlocksMap { get { return blocksMap; } set { blocksMap = value; } }

    private int[,] topBlocksHeight;
    public int[,] TopBlocksHeight { get { return topBlocksHeight; } set { topBlocksHeight = value; } }

    private int mapWidth;
    public int MapWidth { get; set; }

    private int mapHeight;
    public int MapHeight { get; set; }

    private Vector3 portalPos { get; set; }
    public Vector3 PortalPos { get; set; }

    private Vector3 treasurePos { get; set; }
    public Vector3 TreasurePos { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        m_Instance = this;
        gameObject.transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }
}

