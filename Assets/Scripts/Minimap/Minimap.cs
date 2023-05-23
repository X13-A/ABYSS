using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    [SerializeField] private float exploreRadius;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private Vector2 mapSize;
    [SerializeField] private GameObject player;
    private Vector2 playerPos;
    private bool[,] exploredMap;

    private Texture2D imageTexture;

    private void Start()
    {
        imageTexture = new Texture2D((int) mapSize.x, (int) mapSize.y);
        rawImage.texture = imageTexture;
        exploredMap = new bool[(int) mapSize.x, (int) mapSize.y];
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

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector2 pixelPos = new Vector2(x, y);
                float dist = Vector2.Distance(playerPos, pixelPos);
                if (dist < exploreRadius)
                {
                    if (exploredMap[x, y] == false)
                    {
                        exploredMap[x, y] = true;
                    }
                }
            }
        }
    }

    private void UpdatePixels()
    {
        Color32[] pixelColors = imageTexture.GetPixels32();

        for (int y = 0; y < imageTexture.height; y++)
        {
            for (int x = 0; x < imageTexture.width; x++)
            {
                if (exploredMap[x, y] == true)
                {
                    pixelColors[y * imageTexture.width + x] = Color.clear;
                }
                else
                {
                    pixelColors[y * imageTexture.width + x] = Color.black;
                }
            }
        }

        imageTexture.SetPixels32(pixelColors);
        imageTexture.Apply();
    }

    private void Update()
    {
        ExploreMinimap();
        UpdatePixels();
    }
}
