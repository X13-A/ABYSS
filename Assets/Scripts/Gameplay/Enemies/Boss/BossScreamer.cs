using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossScreamer : MonoBehaviour
{
    [SerializeField] private new GameObject light;

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
        //float elapsedTime = 0f;
        //float flashDuration = 2f;

        Vector3 initPos = transform.position;
        transform.position = PlayerManager.Instance.PlayerReference.transform.position + new Vector3(0f, 0f, 3.2f);
        light.transform.position = new Vector3(transform.position.x, light.transform.position.y, transform.position.z);
        light.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        light.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        light.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        transform.position = initPos;
        light.SetActive(false);

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
