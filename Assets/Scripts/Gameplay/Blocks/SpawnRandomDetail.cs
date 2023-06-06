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

    private float detailCenterOffset;
    private float maximumDetailDistance;


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

        Debug.Log(maximumDetailDistance);

        for (int i = 0; i < maximumDetails; i++)
        {
            if (Random.value <= probability)
            {
                Side randomSide = GetRandomSide();
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
        return rotation;
    }
}
