using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private PlayerCharacteristic characteristic;

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
        EventManager.Instance.AddListener<EnemyAttackEvent>(this.SetHealth);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<EnemyAttackEvent>(this.SetHealth);
    }

    public void Start()
    {
        slider.maxValue = characteristic.maxHealth;
        slider.value = characteristic.maxHealth;
    }

    public void SetHealth(EnemyAttackEvent e)
    {
        slider.value -= 10;
    }
}
