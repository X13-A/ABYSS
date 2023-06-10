using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScreamerPositionPlayer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            EventManager.Instance.Raise(new StartBossScreamerEvent { });
            Destroy(gameObject);
        }
    }
}
