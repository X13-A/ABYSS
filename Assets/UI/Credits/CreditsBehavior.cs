using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsBehavior : MonoBehaviour
{
    [SerializeField] private float scrollSpeed;
    private RectTransform rectTransform;
    private bool finished = false;

    private void Awake()
    {
       rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (finished) return;
        float screenTop = Screen.height;
        float creditsBottom = rectTransform.position.y - (rectTransform.rect.height / 2);
        if (creditsBottom < screenTop)
        {
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y + this.scrollSpeed * Time.deltaTime,
                transform.position.z
            );
        }
        else
        {
            finished = true;
            EventManager.Instance.Raise(new SceneAboutToChangeEvent { targetScene = "Main Menu", levelGenerated = 0 });
        }
    }
}
