using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageTextEmitter : MonoBehaviour
{
    private float totalDamage;
    private float lastDamageTime;
    [SerializeField] private GameObject emitPos;
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private float resetDelay;
    [SerializeField] private float fadeDelay;
    public void AddDamage(float damage)
    {
        if (Time.time - lastDamageTime > resetDelay)
        {
            totalDamage = 0;
        }
        totalDamage += damage;
        lastDamageTime = Time.time;

        GameObject textInstance = Instantiate(textPrefab, emitPos.transform.position, Quaternion.identity);
        TextMeshPro textMeshPro = textInstance.GetComponent<TextMeshPro>();
        textMeshPro.text = totalDamage.ToString();

        Vector3 textScale = textInstance.transform.localScale;
        CoroutineUtil.Instance.StartPermanentCoroutine(CoroutineUtil.FadeTextTo(textMeshPro, fadeDelay, 0f, () => { Destroy(textInstance); }));
        CoroutineUtil.Instance.StartPermanentCoroutine(CoroutineUtil.ScaleTo(textMeshPro.transform, fadeDelay * 0.125f, textScale * 1.25f, () => {
            CoroutineUtil.Instance.StartPermanentCoroutine(CoroutineUtil.ScaleTo(textMeshPro.transform, fadeDelay * 0.25f, textScale));
        }));
    }
}
