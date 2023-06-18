using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Score : MonoBehaviour, IEventHandler
{
    private TextMeshProUGUI textMeshPro;
    private void OnEnable()
    {
        SubscribeEvents();
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    public void OnDisable()
    {
        UnsubscribeEvents();
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<UpdateScoreEvent>(SetScore);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<UpdateScoreEvent>(SetScore);
    }

    public void SetScore(UpdateScoreEvent e)
    {
        textMeshPro.text = "Score : " + e.updatedScore;
    }
}
