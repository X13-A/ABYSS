using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSlider : MonoBehaviour, IEventHandler
{
    [SerializeField]

    TextMeshProUGUI text;

    void OnEnable()
    {
        SubscribeEvents();
        Init();
    }

    void OnDisable()
    {
        UnsubscribeEvents();
    }
    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<WindowResizeEvent>(UpdateValue);
        EventManager.Instance.AddListener<ResolutionScaleSliderChangeEvent>(UpdateText);

    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<WindowResizeEvent>(UpdateValue);
        EventManager.Instance.RemoveListener<ResolutionScaleSliderChangeEvent>(UpdateText);
    }

    void Init()
    {
        if (SettingsManager.Instance == null) return;
        GetComponent<Slider>().value = SettingsManager.Instance.ResolutionScale;
        EventManager.Instance.Raise(new ResolutionScaleSliderChangeEvent() { value = SettingsManager.Instance.ResolutionScale });
    }

    void UpdateText(ResolutionScaleSliderChangeEvent e)
    {
        text.text = (e.value * 100).ToString("F0") + "%";
    }

    void UpdateValue(WindowResizeEvent e)
    {
        GetComponent<Slider>().value = e.resolutionScale;
        EventManager.Instance.Raise(new ResolutionScaleSliderChangeEvent() { value = e.resolutionScale });
    }
}