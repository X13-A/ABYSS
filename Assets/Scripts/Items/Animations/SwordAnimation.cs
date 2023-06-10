using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAnimation : MonoBehaviour
{
    [SerializeField] private ParticleSystem swordTrail;

    private void OnEnable()
    {
        swordTrail.Stop(true);
    }

    private void OnDisable()
    {
        swordTrail.Stop(true);
    }

    public void Animate(float delay)
    {
        StartCoroutine(CoroutineUtil.DelayAction(0f, () =>
        {
            swordTrail.Stop(true);
            swordTrail.Play(true);
        }));
        StartCoroutine(CoroutineUtil.DelayAction(delay, () =>
        {
            swordTrail.Stop(true);
        }));
    }
}
