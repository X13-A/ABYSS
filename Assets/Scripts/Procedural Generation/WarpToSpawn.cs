using SDD.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpToSpawn : MonoBehaviour
{
    private PlayerSpawn spawn;
    private CharacterController characterController;

    private bool spawned = false;
    private void Start()
    {
        spawn = FindObjectOfType<PlayerSpawn>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!spawned && spawn != null && characterController != null)
        {
            characterController.enabled = false;
            transform.position = spawn.transform.position;
            characterController.enabled = true;
            spawned = true;
            gameObject.SetActive(true);
            EventManager.Instance.Raise(new PlayerSpawnedEvent { });
        }
    }
}
