using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShieldBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fill;
    [SerializeField] private GameObject shieldBar;

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
        EventManager.Instance.AddListener<UpdateShieldPlayerHealthEvent>(SetShield);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<UpdateShieldPlayerHealthEvent>(SetShield);
    }
    public void Start()
    {
        slider.maxValue = PlayerManager.Instance.MaxShield;
        slider.value = PlayerManager.Instance.Shield;

        if (PlayerManager.Instance.Shield == 0) shieldBar.SetActive(false);
        else shieldBar.SetActive(true);
    }

    private void SetShield(UpdateShieldPlayerHealthEvent e)
    {
        if (e.newShieldHealth == 0) shieldBar.SetActive(false);
        else shieldBar.SetActive(true);
        slider.value = e.newShieldHealth;
    }
}
