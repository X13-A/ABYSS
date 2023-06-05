using SDD.Events;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour, IEventHandler
{
    [SerializeField] private float exploreRadius;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerMarker;
    [SerializeField] private GameObject portalMarker;
    [SerializeField] private GameObject treasureMarker;
    [SerializeField] private GameObject display;
    [SerializeField] private Transform mapCenter;

    private bool exploring = false;

    private Vector2 playerPos;
    private bool[,] exploredMap;
    private RectTransform rectTransform;

    private RectTransform playerMarkerRect;
    private RectTransform portalMarkerRect;
    private RectTransform treasureMarkerRect;

    private Texture2D imageTexture;
    private bool portalDiscovered = false;

    public bool Visibility
    {
        get
        {
            return display.activeSelf;
        }
        set
        {
            display.SetActive(value);
        }
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<PlayerSpawnedEvent>(StartExploration);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayerSpawnedEvent>(StartExploration);
    }

    private void StartExploration(PlayerSpawnedEvent e)
    {
        exploring = true;
    }

    private void Start()
    {
        imageTexture = new Texture2D(LevelData.Instance.MapWidth, LevelData.Instance.MapHeight);
        imageTexture.filterMode = FilterMode.Point;
        rawImage.texture = imageTexture;
        exploredMap = new bool[LevelData.Instance.MapWidth, LevelData.Instance.MapWidth];
        rectTransform = GetComponent<RectTransform>();
        if (playerMarker != null )
        {
            playerMarkerRect = playerMarker.GetComponent<RectTransform>();
        }
        if (portalMarker != null )
        {
            portalMarkerRect = portalMarker.GetComponent<RectTransform>();
            portalMarkerRect.anchoredPosition = new Vector3
            (
                (LevelData.Instance.PortalPos.x / LevelData.Instance.MapWidth) * rectTransform.rect.width,
                (LevelData.Instance.PortalPos.z / LevelData.Instance.MapHeight) * rectTransform.rect.height,
                0
            );
        }
        if (treasureMarker != null)
        {
            treasureMarkerRect = portalMarker.GetComponent<RectTransform>();
            treasureMarkerRect.anchoredPosition = new Vector3
            (
                (LevelData.Instance.TreasurePos.x / LevelData.Instance.MapWidth) * rectTransform.rect.width,
                (LevelData.Instance.TreasurePos.z / LevelData.Instance.MapHeight) * rectTransform.rect.height,
                0
            );
        }
    }

    private void ExploreMinimap()
    {
        if (player == null) return;
        Vector2 newPlayerPos = new Vector2
        (
            (int) player.transform.position.x,
            (int) player.transform.position.z
        );
        if (playerPos == newPlayerPos) return;
        playerPos = newPlayerPos;

        playerMarkerRect.anchoredPosition = new Vector3
        (
            (newPlayerPos.x / LevelData.Instance.MapWidth) * rectTransform.rect.width,
            (newPlayerPos.y / LevelData.Instance.MapHeight) * rectTransform.rect.height,
            0
        );

        for (int x = 0; x < LevelData.Instance.MapWidth; x++)
        {
            for (int y = 0; y < LevelData.Instance.MapHeight; y++)
            {
                Vector2 pixelPos = new Vector2(x, y);
                float dist = Vector2.Distance(playerPos, pixelPos);

                if (dist < exploreRadius)
                {
                    if (exploredMap[x, y] == false)
                    {
                        exploredMap[x, y] = true;
                        if (LevelData.Instance.PortalPos != null && portalDiscovered == false && x == (int) LevelData.Instance.PortalPos.x && y == (int) LevelData.Instance.PortalPos.z)
                        {
                            OnPortalDiscover();
                        }
                    }
                }
            }
        }
    }

    private void OnPortalDiscover()
    {
        EventManager.Instance.Raise(new PortalDiscoveredEvent { });
        portalDiscovered = true;
        portalMarker.SetActive(true);
    }

    private Color FindPixelColor(int x, int z)
    {
        // Get height of block at x, z
        BlockType[,,] topBlocksHeight = LevelData.Instance.BlocksMap;
        if (topBlocksHeight == null) return Color.clear;
        int y = LevelData.Instance.TopBlocksHeight[x, z];

        // Get block type
        BlockType[,,] blocksMap = LevelData.Instance.BlocksMap;
        if (blocksMap == null) return Color.clear;

        BlockType type = LevelData.Instance.BlocksMap[x, y, z];
        Color color = EnumConverter.ColorFromBlockType(type);

        // Set alpha depending on depth
        float alpha = 1f - Mathf.Clamp(0.2f + ((float)y / 5f)*0.8f, 0f, 1f);
        color = new Color (color.r, color.g, color.b, alpha);

        return color;
    }

    private void UpdatePixels()
    {
        Color32[] pixelColors = imageTexture.GetPixels32();

        for (int z = 0; z < imageTexture.height; z++)
        {
            for (int x = 0; x < imageTexture.width; x++)
            {

                if (exploredMap[x, z] == true)
                {
                    Color color = FindPixelColor(x, z);
                    pixelColors[z * imageTexture.width + x] = color;
                }
                else
                {
                    pixelColors[z * imageTexture.width + x] = Color.black;
                }
            }
        }

        imageTexture.SetPixels32(pixelColors);
        imageTexture.Apply();
    }

    private void Update()
    {
        if (exploring == true)
        {
            ExploreMinimap();
            UpdatePixels();
        }
    }
}
