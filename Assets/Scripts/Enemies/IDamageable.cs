using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public float Health { get; }

    public PlayerMode ModeGivingDamage { get;  }
    public void Damage(float damage);
    public void Die();
}
