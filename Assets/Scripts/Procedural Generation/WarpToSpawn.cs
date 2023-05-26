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
        this.spawn = FindObjectOfType<PlayerSpawn>();
        this.characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!this.spawned && this.spawn != null && this.characterController != null)
        {
            this.characterController.enabled = false;
            this.transform.position = spawn.transform.position;
            this.characterController.enabled = true;
            this.spawned = true;
            this.gameObject.SetActive(true);
            EventManager.Instance.Raise(new PlayerSpawnedEvent { });
        }
    }
}
