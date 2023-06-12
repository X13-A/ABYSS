using UnityEngine;
using SDD.Events;

public class BossAnimationController : MonoBehaviour, IEventHandler
{
    private Animator m_Animator;

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
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

    private void Update()
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY) return;
        bool isAwake = m_Animator.GetBool("isAwake");
    }
}
