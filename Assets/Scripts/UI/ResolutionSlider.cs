using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSlider : MonoBehaviour, IEventHandler
{
    [SerializeField]
    private TextMeshProUGUI text;

    private void OnEnable()
    {
        SubscribeEvents();
        Init();
    }

    private void OnDisable()
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

    private void Init()
    {
        if (SettingsManager.Instance == null)
        {
            return;
        }

        GetComponent<Slider>().value = SettingsManager.Instance.ResolutionScale;
        EventManager.Instance.Raise(new ResolutionScaleSliderChangeEvent() { value = SettingsManager.Instance.ResolutionScale });
    }

    private void UpdateText(ResolutionScaleSliderChangeEvent e)
    {
        float aspectRatio = (float) Screen.width / Screen.height;

        int height = (int) (RenderManager.Instance.MaxHeight * e.value);
        int width = Mathf.RoundToInt(height * aspectRatio);

        text.text = $"({width}x{height})";
    }

    private void UpdateValue(WindowResizeEvent e)
    {
        GetComponent<Slider>().value = e.resolutionScale;
        EventManager.Instance.Raise(new ResolutionScaleSliderChangeEvent() { value = e.resolutionScale });
    }
}
