using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsBehavior : MonoBehaviour
{
    [SerializeField] private float scrollSpeed;
    private float scrollSpeedScale = 5;
    private RectTransform rectTransform;
    private bool finished = false;

    private void Awake()
    {
       rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (finished) return;
        float scrollSpeedChange = Input.mouseScrollDelta.y;
        scrollSpeedScale += scrollSpeedChange;
        scrollSpeedScale = Mathf.Clamp(scrollSpeedScale, -10f, 10f);

        float creditsBottom = rectTransform.position.y - (rectTransform.rect.height / 2);
        if (creditsBottom < Screen.height)
        {
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y + scrollSpeed * scrollSpeedScale * Time.deltaTime,
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
