using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageableType { Player, Enemy, Decor, Block };


public interface IDamageable
{


    public float Health { get; }

    DamageableType DamageableType { get; }

    public List<AttackType> DamagerTypes { get; }

    public List<float> DamagerTypesFactors { get; }

    public void Damage(float damage, AttackType type);

    public void Die();
}
