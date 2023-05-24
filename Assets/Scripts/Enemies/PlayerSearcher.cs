using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSearcher : MonoBehaviour
{
    [SerializeField] private float detectionRadius;
    [SerializeField] private float velocity;
    [SerializeField] private Transform playerReference;


    private float distanceToPlayer;

    private void Start()
    {
    }

    private void Update()
    {
        SearchPlayer();
    }

    private void SearchPlayer()
    {
        if (playerReference == null)
        {
            return;
        }
        distanceToPlayer = Vector3.Distance(transform.position, playerReference.position);
    }

    private void FixedUpdate()
            {
        if (distanceToPlayer <= detectionRadius)
        {
            Vector3 directionToPlayer = (playerReference.position - transform.position).normalized;
            transform.LookAt(playerReference.transform);
            transform.position += directionToPlayer * velocity;
        }
    }
}
