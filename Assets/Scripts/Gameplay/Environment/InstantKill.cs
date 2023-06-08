using SDD.Events;
using UnityEngine;

public class InstantKill : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        IPlayerCollider player = other.GetComponent<IPlayerCollider>();
        if (player != null)
        {
            Kill(player);
            return;
        }

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Kill(damageable);
            return;
        }

        // Destroy anything else
        Destroy(other.gameObject);
    }

    private void Kill(IPlayerCollider player)
    {
        EventManager.Instance.Raise(new DamagePlayerEvent { damage = Mathf.Infinity });
    }

    private void Kill(IDamageable damageable)
    {
        damageable.Damage(Mathf.Infinity, AttackType.MAGIC);
    }
}
