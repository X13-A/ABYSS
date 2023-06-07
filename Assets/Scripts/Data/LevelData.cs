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

    public float MapRadius { get; set; }

    public int MapWidth { get; set; }

    public int MapHeight { get; set; }

    public Vector3 PortalPos { get; set; }

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

