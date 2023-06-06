using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogDamage : MonoBehaviour
{
    [SerializeField] float dps = 10;
    private bool damaging = false;
    private float lastDamageTime;

    private void Start()
    {
        lastDamageTime = Time.time;
    }

    private void OnTriggerEnter(Collider other)
    {
        IPlayerCollider player = other.GetComponent<IPlayerCollider>();
        if (player != null)
        {
            damaging = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IPlayerCollider player = other.GetComponent<IPlayerCollider>();
        if (player != null)
        {
            damaging = false;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY) return;
        if (damaging && Time.time - lastDamageTime >= 1f)
        {
            EventManager.Instance.Raise(new DamagePlayerEvent { damage = dps });
            lastDamageTime = Time.time;
        }
    }
}
