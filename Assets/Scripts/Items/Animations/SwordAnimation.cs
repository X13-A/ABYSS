using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAnimation : MonoBehaviour
{
    [SerializeField] private ItemId itemId;
    [SerializeField] private ParticleSystem swordTrail;

    private void OnEnable()
    {
        SubscribeEvents();
        swordTrail.Stop(true);
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
        swordTrail.Stop(true);
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<AnimateItemEvent>(Animate);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<AnimateItemEvent>(Animate);
    }

    private void Animate(AnimateItemEvent e)
    {
        if (e.itemId != this.itemId) return;

        foreach (KeyValuePair<string, float> animation in e.animations)
        {
            string name = animation.Key;
            float delay = animation.Value;

            if (name == "startTrail")
            {
                StartCoroutine(CoroutineUtil.DelayAction(delay, () =>
                {
                    this.swordTrail.Stop(true);
                    this.swordTrail.Play(true);
                }));
            }
            else if (name == "stopTrail")
            {
                StartCoroutine(CoroutineUtil.DelayAction(delay, () =>
                {
                    this.swordTrail.Stop(true);
                }));
            }
        }
    }
}
