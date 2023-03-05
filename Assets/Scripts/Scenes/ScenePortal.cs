using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using SDD.Events;

public class ScenePortal : MonoBehaviour
{
    [SerializeField] string targetScene;


    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        IPlayerCollider player = other.gameObject.GetComponent(typeof(IPlayerCollider)) as IPlayerCollider;
        if (player == null) return;

        EventManager.Instance.Raise(new SceneAboutToChangeEvent { targetScene = targetScene });
    }
}
