using UnityEngine;
using SDD.Events;

public class BossAnimationController : MonoBehaviour, IEventHandler
{
    [SerializeField] private Animator m_Animator;
    [SerializeField] private BossSword m_Sword;

    private void Start()
    {
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

    public void StartMeleeDamage()
    {
        m_Sword.StartDamage();
    }

    public void EndMeleeDamage()
    {
        m_Sword.EndDamage();
    }
}
