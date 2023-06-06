using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomDetail : MonoBehaviour
{
    private enum Side { Left, Right, Front, Back, Top, Bottom };

    [SerializeField] private GameObject detail;
    [SerializeField] private float probability;
    [SerializeField] private int maximumDetails;

    [SerializeField] private List<Side> spawnSides;
    [SerializeField] private bool pivotIsCenter = false; // Use if the pivot of the detail object is at the center instead of bottom
    [SerializeField] private bool drawRayCasts = false;

    private float detailCenterOffset;
    private float maximumDetailDistance;

    [SerializeField] private float heightNeeded = 1;

    private void Start()
    {
        if (spawnSides.Count == 0) return;
        if (detail == null) return;
        if (maximumDetails <= 0) return;
        if (probability <= 0) return;

        // Calculate detail size for offset
        detailCenterOffset = 0;
        MeshRenderer meshRenderer = detail.GetComponentInChildren<MeshRenderer>(); // Get the mesh from the MeshFilter component
        Bounds bounds = meshRenderer.bounds; // Retrieve the bounding box
        Vector3 detailSize = bounds.size; // Size of the bounding box
        maximumDetailDistance = Mathf.Max(0, (1 - detailSize.y / 2) / 2);

        detailCenterOffset = 0;
        if (pivotIsCenter)
        {
            detailCenterOffset = detailSize.y / 2;
        }

        for (int i = 0; i < maximumDetails; i++)
        {
            if (Random.value <= probability)
            {
                Side randomSide = GetRandomSide();
                if (CheckForObstruction(randomSide) == true) return;

                Vector3 spawnPosition = GetRandomSpawnLocalPosition(randomSide);
                Quaternion spawnRotation = GetSpawnRotation(randomSide);
                GameObject spawnedDetail = Instantiate(detail, spawnPosition, spawnRotation, transform);
                spawnedDetail.transform.localPosition = spawnPosition;
            }
        }
    }

    private Side GetRandomSide()
    {
        int randomIndex = Random.Range(0, spawnSides.Count);
        return spawnSides[randomIndex];
    }


    private Vector3 GetRandomSpawnLocalPosition(Side side)
    {
        Vector3 localPosition = Vector3.zero;
        switch (side)
        {
            case Side.Left:
                localPosition = new Vector3(-0.5f - detailCenterOffset, Random.Range(-maximumDetailDistance, maximumDetailDistance), Random.Range(-maximumDetailDistance, maximumDetailDistance));
                break;
            case Side.Right:
                localPosition = new Vector3(0.5f + detailCenterOffset, Random.Range(-maximumDetailDistance, maximumDetailDistance), Random.Range(-maximumDetailDistance, maximumDetailDistance));
                break;
            case Side.Front:
                localPosition = new Vector3(Random.Range(-maximumDetailDistance, maximumDetailDistance), Random.Range(-maximumDetailDistance, maximumDetailDistance), -0.5f - detailCenterOffset);
                break;
            case Side.Back:
                localPosition = new Vector3(Random.Range(-maximumDetailDistance, maximumDetailDistance), Random.Range(-maximumDetailDistance, maximumDetailDistance), 0.5f + detailCenterOffset);
                break;
            case Side.Top:
                localPosition = new Vector3(Random.Range(-maximumDetailDistance, maximumDetailDistance), 0.5f + detailCenterOffset, Random.Range(-maximumDetailDistance, maximumDetailDistance));
                break;
            case Side.Bottom:
                localPosition = new Vector3(Random.Range(-maximumDetailDistance, maximumDetailDistance), -0.5f - detailCenterOffset, Random.Range(-maximumDetailDistance, maximumDetailDistance));
                break;
        }
        return localPosition;
    }
    private Quaternion GetSpawnRotation(Side side)
    {
        Quaternion rotation;
        float randomRotation = Random.Range(-180, 180);
        switch (side)
        {
            case Side.Left:
                rotation = Quaternion.Euler(0f, 0f, 90f);
                break;
            case Side.Right:
                rotation = Quaternion.Euler(0f, 0f, -90f);
                break;
            case Side.Front:
                rotation = Quaternion.Euler(-90f, 0f, 0f);
                break;
            case Side.Back:
                rotation = Quaternion.Euler(90f, 0f, 0f);
                break;
            case Side.Top:
                rotation = Quaternion.Euler(0f, 0f, 0f);
                break;
            case Side.Bottom:
                rotation = Quaternion.Euler(-180f, 0f, 0f);
                break;
            default:
                rotation = Quaternion.identity;
                break;
        }
        return rotation * Quaternion.Euler(0, randomRotation, 0f);
    }

    private bool CheckForObstruction(Side side)
    {
        Vector3 raycastDirection = GetRaycastDirection(side);
        List<Vector3> raycastOrigins = GetRaycastOrigins(side);

        foreach (Vector3 origin in raycastOrigins)
        {
            RaycastHit hit;
            if (Physics.Raycast(origin, raycastDirection, out hit, heightNeeded))
            {
                return true;
            }
        }
        return false;
    }
    private List<Vector3> GetRaycastOrigins(Side side)
    {
        List<Vector3> positions = new List<Vector3>();
        float offset = 0.5f;
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

        if (side == Side.Left)
        {
            positions.Add(new Vector3(x - offset, y - 1, z - 1));
            positions.Add(new Vector3(x - offset, y - 1, z + 1));
            positions.Add(new Vector3(x - offset, y - 1, z    ));

            positions.Add(new Vector3(x - offset, y    , z - 1));
            positions.Add(new Vector3(x - offset, y    , z + 1));
            positions.Add(new Vector3(x - offset, y    , z    ));

            positions.Add(new Vector3(x - offset, y + 1, z - 1));
            positions.Add(new Vector3(x - offset, y + 1, z + 1));
            positions.Add(new Vector3(x - offset, y + 1, z    ));
        }
        else if (side == Side.Right)
        {
            positions.Add(new Vector3(x + offset, y - 1, z - 1));
            positions.Add(new Vector3(x + offset, y - 1, z + 1));
            positions.Add(new Vector3(x + offset, y - 1, z    ));

            positions.Add(new Vector3(x + offset, y    , z - 1));
            positions.Add(new Vector3(x + offset, y    , z + 1));
            positions.Add(new Vector3(x + offset, y    , z    ));

            positions.Add(new Vector3(x + offset, y + 1, z - 1));
            positions.Add(new Vector3(x + offset, y + 1, z + 1));
            positions.Add(new Vector3(x + offset, y + 1, z    ));
        }
        else if (side == Side.Front)
        {
            positions.Add(new Vector3(x - 1, y - 1, z + offset));
            positions.Add(new Vector3(x + 1, y - 1, z + offset));
            positions.Add(new Vector3(x    , y - 1, z + offset));

            positions.Add(new Vector3(x - 1, y    , z + offset));
            positions.Add(new Vector3(x + 1, y    , z + offset));
            positions.Add(new Vector3(x    , y    , z + offset));

            positions.Add(new Vector3(x - 1, y + 1, z + offset));
            positions.Add(new Vector3(x + 1, y + 1, z + offset));
            positions.Add(new Vector3(x    , y + 1, z + offset));
        }
        else if (side == Side.Back)
        {
            positions.Add(new Vector3(x - 1, y - 1, z - offset));
            positions.Add(new Vector3(x + 1, y - 1, z - offset));
            positions.Add(new Vector3(x    , y - 1, z - offset));

            positions.Add(new Vector3(x - 1, y    , z - offset));
            positions.Add(new Vector3(x + 1, y    , z - offset));
            positions.Add(new Vector3(x    , y    , z - offset));

            positions.Add(new Vector3(x - 1, y + 1, z - offset));
            positions.Add(new Vector3(x + 1, y + 1, z - offset));
            positions.Add(new Vector3(x    , y + 1, z - offset));
        }
        else if (side == Side.Top)
        {
            positions.Add(new Vector3(x - 1, y + offset, z - 1));
            positions.Add(new Vector3(x + 1, y + offset, z - 1));
            positions.Add(new Vector3(x    , y + offset, z - 1));

            positions.Add(new Vector3(x - 1, y + offset, z + 1));
            positions.Add(new Vector3(x + 1, y + offset, z + 1));
            positions.Add(new Vector3(x    , y + offset, z + 1));

            positions.Add(new Vector3(x - 1, y + offset, z    ));
            positions.Add(new Vector3(x + 1, y + offset, z    ));
            positions.Add(new Vector3(x    , y + offset, z    ));
        }
        else if (side == Side.Bottom)
        {
            positions.Add(new Vector3(x - 1, y - offset, z - 1));
            positions.Add(new Vector3(x + 1, y - offset, z - 1));
            positions.Add(new Vector3(x    , y - offset, z - 1));

            positions.Add(new Vector3(x - 1, y - offset, z + 1));
            positions.Add(new Vector3(x + 1, y - offset, z + 1));
            positions.Add(new Vector3(x    , y - offset, z + 1));

            positions.Add(new Vector3(x - 1, y - offset, z    ));
            positions.Add(new Vector3(x + 1, y - offset, z    ));
            positions.Add(new Vector3(x    , y - offset, z    ));
        }

        return positions;
    }

    private void OnDrawGizmos()
    {
        if (!drawRayCasts) return;
        if (spawnSides.Count == 0) return;
        if (detail == null) return;

        foreach (Side side in spawnSides)
        {
            List<Vector3> raycastOrigins = GetRaycastOrigins(side);
            Vector3 raycastDirection = GetRaycastDirection(side);
            foreach(Vector3 origin in raycastOrigins)
            {
                Debug.DrawRay(origin, raycastDirection * heightNeeded, Color.red);
            }
        }
    }

    private Vector3 GetRaycastDirection(Side side)
    {
        Vector3 direction = Vector3.zero;
        switch (side)
        {
            case Side.Left:
                direction = -transform.right;
                break;
            case Side.Right:
                direction = transform.right;
                break;
            case Side.Front:
                direction = transform.forward;
                break;
            case Side.Back:
                direction = -transform.forward;
                break;
            case Side.Top:
                direction = transform.up;
                break;
            case Side.Bottom:
                direction = -transform.up;
                break;
        }
        return direction;
    }
}
