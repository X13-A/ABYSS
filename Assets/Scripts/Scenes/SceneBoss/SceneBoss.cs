using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBoss : MonoBehaviour
{
    [SerializeField] GameObject door;
    [SerializeField] GameObject wallDoor;

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
        EventManager.Instance.AddListener<PlayerDetectorEvent>(WallEnable);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayerDetectorEvent>(WallEnable);
    }

    private void WallEnable(PlayerDetectorEvent e)
    {
        door.SetActive(false);
        wallDoor.SetActive(true);
    }
}
