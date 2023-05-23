using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour, IDamager
{
    [SerializeField] private float damage;
    [SerializeField] private AttackType type;
    private Collider collider;
    public AttackType Type => type;
    public HashSet<IDamageable> collides = new HashSet<IDamageable>();

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

    public void Update()
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY) return;

        if (PlayerManager.Instance.ActivePlayerMode != PlayerMode.PICKAXE && PlayerManager.Instance.ActivePlayerMode != PlayerMode.AXE) return;
        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit = AimUtil.Instance.Aim(~(1 << LayerMask.NameToLayer("Aim")));
            if (hit.collider) this.CauseDamage(hit.collider.GetComponent<IDamageable>());
        }
    }

    public void Damage(float damage, float duration)
    {
        // Enable
        StartCoroutine(CoroutineUtil.DelayAction(duration * 0.4f, () =>
        {
            this.collides.Clear();
            this.damage = damage;
            this.collider.enabled = true;
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
            this.collider.enabled = false;
            this.collides.Clear();
        }));
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable == null) return;
        if (this.collides.Contains(damageable)) return;
        if (!damageable.DamagerTypes.Contains(this.type)) return;

        this.CauseDamage(damageable);
    }

    private void CauseDamage(IDamageable damageable)
    {
        damageable.Damage(this.damage, this.type);
        this.collides.Add(damageable);
    }
}
