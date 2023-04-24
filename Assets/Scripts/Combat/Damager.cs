using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Damager : MonoBehaviour, IDamager
{
    [SerializeField] private float damage;
    private new Collider collider;
    private AttackType type = AttackType.MELEE;
    public AttackType Type => type;
    public HashSet<IDamageable> collides = new HashSet<IDamageable>();

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

    public void Update()
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY)
        {
            return;
        }

        if (PlayerManager.Instance.ActivePlayerMode != PlayerMode.PICKAXE && PlayerManager.Instance.ActivePlayerMode != PlayerMode.AXE)
        {
            return;
        }
        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit = AimUtil.Instance.Aim();
            if (hit.collider)
            {
                this.CauseDamage(hit.collider);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PlayerManager.Instance.ActivePlayerMode == PlayerMode.MELEE)
        {
            this.CauseDamage(other);
        }
    }

    public void Damage(float damage, float duration)
    {
        // Enable
        StartCoroutine(CoroutineUtil.DelayAction(duration * 0.4f, () =>
        {
            collides.Clear();
            this.damage = damage;
            collider.enabled = true;
        }));

        // Never disable (arrows, projectiles)
        if (duration <= 0)
        {
            return;
        }

        // Disable
        StartCoroutine(CoroutineUtil.DelayAction(duration, () =>
        {
            this.damage = 0;
            collider.enabled = false;
            collides.Clear();
        }));
    }

    private void CauseDamage(Collider other)
    {
        // Inflige des d�gats si l'ennemi n'a pas d�j� �t� touch�
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && !collides.Contains(damageable) && damageable.ModeGivingDamage == PlayerManager.Instance.ActivePlayerMode)
        {
            damageable.Damage(damage);
            collides.Add(damageable);
        }
    }
}
