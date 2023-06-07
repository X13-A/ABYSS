using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float maxSpawnHeight = 20;
    [SerializeField] private float maxEnnemies = 20;

    [SerializeField] List<GameObject> ennemies;
    private int ennemiesSpawned => transform.childCount;

    [SerializeField] Camera mainCamera;
    [SerializeField] private bool spawnEnabled = true;
    [SerializeField] private float spawnDelay = 1;
    private float lastSpawnTime;

    private float mapWidth => LevelData.Instance.MapWidth;
    private float mapHeight => LevelData.Instance.MapHeight;

    private void Start()
    {
        lastSpawnTime = Time.time;
    }

    private void TrySpawnMaxEnnemies()
    {
        for(int i = ennemiesSpawned; i < maxEnnemies; i++)
        {
            Vector3? location = TryFindSpawnLocation();
            if (location == null) return;
            SpawnRandomEnemy(location.Value);
        }
    }

    private Vector3? TryFindSpawnLocation()
    {
        // Try to spawn 10 times
        for (int i = 0; i < 10; i++)
        {
            Vector2 spawnStart2D = Random.insideUnitSphere * mapWidth/2 + new Vector3(mapWidth / 2, mapWidth / 2, mapWidth / 2);
            Vector3 spawnStart = new Vector3(spawnStart2D.x, maxSpawnHeight, spawnStart2D.y);

            int layerMask = 1 << LayerMask.NameToLayer("Ground");
            RaycastHit hit;
            if (Physics.Raycast(spawnStart, Vector3.down, out hit, 20, layerMask))
            {
                if (hit.distance > 3 && !IsPointVisible(hit.point))
                {
                    return hit.point;
                }
            }
        }

        return null;
    }

    private void SpawnRandomEnemy(Vector3 location)
    {
        int index = Random.Range(0, ennemies.Count);
        Instantiate(ennemies[index], location, Quaternion.identity, transform);
    }

    public bool IsPointVisible(Vector3 point)
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(point);

        return viewportPoint.x >= 0 && viewportPoint.x <= 1
            && viewportPoint.y >= 0 && viewportPoint.y <= 1
            && viewportPoint.z > 0;
    }

    private void Update()
    {
        if (!spawnEnabled) return;
        if (ennemiesSpawned >= maxEnnemies) return;

        if (Time.time - lastSpawnTime > spawnDelay)
        {
            lastSpawnTime = Time.time;

            Vector3? location = TryFindSpawnLocation();

            if (location == null) return;

            SpawnRandomEnemy(location.Value);
        }
    }
}
