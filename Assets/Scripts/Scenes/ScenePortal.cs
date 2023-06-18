using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using SDD.Events;

public class ScenePortal : MonoBehaviour
{
    [SerializeField] private string targetScene;
    [SerializeField] private int levelGenerated;

    public int LevelGenerated
    {
        get => levelGenerated;
        set => levelGenerated = value;
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        IPlayerCollider player = other.gameObject.GetComponent(typeof(IPlayerCollider)) as IPlayerCollider;
        if (player == null) return;
        if (levelGenerated == 5)
        {
            EventManager.Instance.Raise(new SceneAboutToChangeEvent
            {
                levelGenerated = 0,
                displayLoading = false,
                targetScene = "Boss Scene"
            });
        }
        else
            EventManager.Instance.Raise(new SceneAboutToChangeEvent { targetScene = targetScene, levelGenerated = levelGenerated });;
    }
}
