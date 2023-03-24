using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class PlayerManager : MonoBehaviour, IEventHandler
{
    public static PlayerManager Instance;

    [SerializeField] private PlayerMode activePlayerMode;
    public PlayerMode ActivePlayerMode { get { return this.activePlayerMode; } }
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
        EventManager.Instance.AddListener<PlayerSwitchModeEvent>(SetPlayerMode);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayerSwitchModeEvent>(SetPlayerMode);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        
    }

    private void SetPlayerMode(PlayerSwitchModeEvent e)
    {
        this.activePlayerMode = e.mode;
    }
}
