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
        StartCoroutine(CoroutineUtil.DelayAction(0f, () =>
        {
            wandCast.Stop(true);
            wandTrail.Stop(true);
            wandTrail.Play(true);
        }));
        StartCoroutine(CoroutineUtil.DelayAction(delay, () =>
        {
            wandTrail.Stop(true);
        }));
        StartCoroutine(CoroutineUtil.DelayAction(delay, () =>
        {
            wandCast.Stop(true);
            wandCast.Play(true);
        }));
    }
}
