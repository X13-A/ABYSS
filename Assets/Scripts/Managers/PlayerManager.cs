using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class PlayerManager : MonoBehaviour, IEventHandler
{
    public static PlayerManager Instance;

    [SerializeField] private PlayerMode activePlayerMode;
    [SerializeField] private PlayerLook activePlayerLook;
    public PlayerMode ActivePlayerMode => this.activePlayerMode;
    public PlayerLook ActivePlayerLook => this.activePlayerLook;
    private void OnEnable()
    {
        this.SubscribeEvents();
    }

    private void OnDisable()
    {
        this.UnsubscribeEvents();
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<PlayerSwitchModeEvent>(this.SetPlayerMode);
        EventManager.Instance.AddListener<PlayerSwitchLookModeEvent>(this.SetPlayerLook);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayerSwitchModeEvent>(this.SetPlayerMode);
        EventManager.Instance.RemoveListener<PlayerSwitchLookModeEvent>(this.SetPlayerLook);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

    }

    private void SetPlayerMode(PlayerSwitchModeEvent e)
    {
        this.activePlayerMode = e.mode;
    }

    private void SetPlayerLook(PlayerSwitchLookModeEvent e)
    {
        this.activePlayerLook = e.lookMode;
    }
}
