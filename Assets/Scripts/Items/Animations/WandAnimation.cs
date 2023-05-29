using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandAnimation : MonoBehaviour, IEventHandler
{
    [SerializeField] private string itemId;
    [SerializeField] private ParticleSystem wandCast;
    [SerializeField] private ParticleSystem wandTrail;

    private void OnEnable()
    {
        SubscribeEvents();
        wandTrail.Stop(true);
        wandCast.Stop(true);
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
        wandTrail.Stop(true);
        wandCast.Stop(true);
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
                    this.wandTrail.Stop(true);
                    this.wandTrail.Play(true);
                }));
            }
            else if (name == "stopTrail")
            {
                StartCoroutine(CoroutineUtil.DelayAction(delay, () =>
                {
                    this.wandTrail.Stop(true);
                }));
            }
            else if (name == "startWandCast")
            {
                StartCoroutine(CoroutineUtil.DelayAction(delay, () =>
                {
                    this.wandCast.Stop(true);
                    this.wandCast.Play(true);
                }));
            }
        }
    }
}
