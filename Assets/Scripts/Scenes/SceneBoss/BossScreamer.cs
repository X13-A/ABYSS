using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossScreamer : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject light;
    [SerializeField] private Material floorMaterial;

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
        EventManager.Instance.AddListener<StartBossScreamerEvent>(BossAnimation);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<StartBossScreamerEvent>(BossAnimation);
    }

    private void BossAnimation(StartBossScreamerEvent e)
    {
        StartCoroutine(ScreamerCoroutine());
        
    }

    private IEnumerator ScreamerCoroutine()
    {
        float elapsedTime = 0f;
        float flashDuration = 2f;

        Vector3 initPos = transform.position;
        transform.position = playerTransform.position + new Vector3(0f, 0f, 3.2f);
        light.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        transform.position = initPos;


        EventManager.Instance.Raise(new EndBossScreamerEvent { });

        yield return null;

        //while (elapsedTime < flashDuration)
        //{
        //    light.SetActive(true);
        //    yield return new WaitForSeconds(0.8f);
        //    light.SetActive(false);

        //    elapsedTime++;
        //}

        //light.SetActive(false);
        //transform.position = initPos;
    }
}
