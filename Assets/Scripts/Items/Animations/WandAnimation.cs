using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandAnimation : MonoBehaviour
{
    [SerializeField] private ParticleSystem wandCast;
    [SerializeField] private ParticleSystem wandTrail;

    private void OnEnable()
    {
        wandTrail.Stop(true);
        wandCast.Stop(true);
    }

    private void OnDisable()
    {
        wandTrail.Stop(true);
        wandCast.Stop(true);
    }

    public void Animate(float delay)
    {
        Debug.Log(delay);
        StartCoroutine(CoroutineUtil.DelayAction(0f, () =>
        {
            this.wandCast.Stop(true);
            this.wandTrail.Stop(true);
            this.wandTrail.Play(true);
        }));
        StartCoroutine(CoroutineUtil.DelayAction(delay, () =>
        {
            this.wandTrail.Stop(true);
        }));
        StartCoroutine(CoroutineUtil.DelayAction(delay, () =>
        {
            this.wandCast.Stop(true);
            this.wandCast.Play(true);
        }));
    }
}
