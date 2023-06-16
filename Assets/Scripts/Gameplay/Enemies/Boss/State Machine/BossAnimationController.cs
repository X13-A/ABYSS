using UnityEngine;
using SDD.Events;

public class BossAnimationController : MonoBehaviour, IEventHandler
{
    [SerializeField] private Animator m_Animator;
    [SerializeField] private BossSword m_Sword;
    [SerializeField] private BossWand m_Wand;

    private void Start()
    {
        m_Animator.speed = BossManager.Instance.BossSpeedScale;
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<EndBossScreamerEvent>(StartAnimating);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<EndBossScreamerEvent>(StartAnimating);
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void StartAnimating(EndBossScreamerEvent e)
    {
        m_Animator.SetBool("isAwake", true);
    }

    public void StartMagicAttackAnimation()
    {
        m_Animator.SetTrigger("MagicAttack");
    }

    public void StartMeleeDamage()
    {
        m_Sword.StartDamage();
    }

    public void EndMeleeDamage()
    {
        m_Sword.EndDamage();
    }

    // HACK: not really in the right place but deadline is in 48 hours
    public void FireMagicAttack()
    {
        m_Wand.FireProjectile();
    }
}
