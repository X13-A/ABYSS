using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour, IEventHandler
{
    [SerializeField] private Slider slider;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image fill;

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
        EventManager.Instance.AddListener<DamagePlayerEvent>(this.SetHealth);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<DamagePlayerEvent>(this.SetHealth);
    }

    public void Start()
    {
        slider.maxValue = PlayerManager.Instance.Health;
        slider.value = PlayerManager.Instance.Health;
        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(DamagePlayerEvent e)
    {
        slider.value -= e.damage;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
