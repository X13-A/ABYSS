using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BestScore : MonoBehaviour, IEventHandler
{
    private TextMeshProUGUI textMeshPro;
    private int bestScore;

    private void OnEnable()
    {
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.text = "Best Score : " + bestScore;
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<SaveScoreEvent>(SaveScore);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<SaveScoreEvent>(SaveScore);
    }

    public void SaveScore(SaveScoreEvent e)
    {
        if (PlayerManager.Instance.Score > bestScore)
        {
            PlayerPrefs.SetInt("BestScore", PlayerManager.Instance.Score);
            PlayerPrefs.Save();
        }
    }
}
