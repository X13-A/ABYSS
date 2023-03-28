using Newtonsoft.Json;
using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SettingsManager : MonoBehaviour, IEventHandler
{
    private static SettingsManager m_Instance;
    public static SettingsManager Instance => m_Instance;

    private SettingsStore m_SettingsStore;
    private SettingsStore m_SettingsBeforeChanges;
    private SettingsStore m_SettingsAfterChanges;
    private string savePath;

    public float ResolutionScale
    {
        get { return m_SettingsStore.ResolutionScale; }
        set 
        { 
            m_SettingsStore.ResolutionScale = value;
            EventManager.Instance.Raise(new ResolutionScaleUpdateEvent());
        }
    }

    private void Awake()
    {
        if (!m_Instance)
        {
            m_Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        savePath = Application.dataPath + "/settings.json";
        m_SettingsStore = LoadSettingsFromFile(savePath);
        m_SettingsBeforeChanges = new SettingsStore(m_SettingsStore);
        m_SettingsAfterChanges = new SettingsStore(m_SettingsStore);
    }

    public void SaveSettingsToFile(SettingsStore settings, string filePath)
    {
        string jsonData = JsonConvert.SerializeObject(settings);
        File.WriteAllText(filePath, jsonData);
    }

    public SettingsStore LoadSettingsFromFile(string filePath)
    {
        string jsonData = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<SettingsStore>(jsonData);
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<SettingsButtonClickedEvent>(StartEditMode);
        EventManager.Instance.AddListener<CancelSettingsButtonClickedEvent>(DiscardChanges);
        EventManager.Instance.AddListener<SaveSettingsButtonClickedEvent>(ApplyChanges);
        EventManager.Instance.AddListener<ResolutionScaleSliderChangeEvent>(SetResolutionScaleEditMode);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<SettingsButtonClickedEvent>(StartEditMode);
        EventManager.Instance.RemoveListener<CancelSettingsButtonClickedEvent>(DiscardChanges);
        EventManager.Instance.RemoveListener<SaveSettingsButtonClickedEvent>(ApplyChanges);
        EventManager.Instance.RemoveListener<ResolutionScaleSliderChangeEvent>(SetResolutionScaleEditMode);
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    public void SetResolutionScaleEditMode(ResolutionScaleSliderChangeEvent e)
    {
        m_SettingsAfterChanges.ResolutionScale = e.value;
    }

    public void ResolutionScaleSliderChange(float value)
    {
        EventManager.Instance.Raise(new ResolutionScaleSliderChangeEvent { value = value });
    }

    public void StartEditMode(SettingsButtonClickedEvent e)
    {
        m_SettingsAfterChanges = new SettingsStore(m_SettingsStore);
        m_SettingsBeforeChanges = new SettingsStore(m_SettingsStore);
    }

    public void DiscardChanges(CancelSettingsButtonClickedEvent e)
    {
        ResolutionScale = m_SettingsBeforeChanges.ResolutionScale;
        m_SettingsBeforeChanges = null;
        m_SettingsAfterChanges = null;
    }

    public void ApplyChanges(SaveSettingsButtonClickedEvent e)
    {
        if (ResolutionScale != m_SettingsAfterChanges.ResolutionScale)
        {
            ResolutionScale = m_SettingsAfterChanges.ResolutionScale;
        }

        m_SettingsBeforeChanges = new SettingsStore(m_SettingsStore);
        SaveSettingsToFile(m_SettingsStore, savePath);
    }
}
