using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CoroutineBossPath : MonoBehaviour
{
    [SerializeField] private GameObject path;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private GameObject boss;

    private Vector3[] coordinates;

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
        EventManager.Instance.AddListener<StartCoroutineBossPathEvent>(StartCoroutineBossPath);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<StartCoroutineBossPathEvent>(StartCoroutineBossPath);
    }

    void Start()
    {
        if (path != null)
        {
            Transform[] childrenTransforms = path.GetComponentsInChildren<Transform>();
            int numChildren = childrenTransforms.Length - 1;
            coordinates = new Vector3[numChildren];

            for (int i = 1; i < childrenTransforms.Length; i++)
            {
                coordinates[i - 1] = childrenTransforms[i].localPosition;
            }
        }
    }

    private void StartCoroutineBossPath(StartCoroutineBossPathEvent e)
    {
        StartCoroutine(BossPath());
    }

    private IEnumerator BossPath()
    {
        foreach (Vector3 pos in coordinates)
        {
            float elapsedTime = 0f;
            Vector3 startingPosition = boss.transform.position;
            float duration = movementSpeed;

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                boss.transform.position = Vector3.Lerp(startingPosition, pos, t);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            boss.transform.position = pos;

            EventManager.Instance.Raise(new IsArrivedToPointEvent { });

            yield return new WaitForSeconds(2);
        }
    }
}
