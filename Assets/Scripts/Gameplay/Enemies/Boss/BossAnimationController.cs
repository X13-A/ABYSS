using UnityEngine;
using SDD.Events;

public class BossAnimationController : MonoBehaviour, IEventHandler
{
    private Animator m_Animator;
    private float spawnTime;
    public float TimeSinceSpawn => Time.time - spawnTime;

    private void Start()
    {
        spawnTime = Time.time;
        m_Animator = GetComponent<Animator>();
    }

    public void SubscribeEvents()
    {
    }

    public void UnsubscribeEvents()
    {
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void Update()
    {
        bool isIdling = m_Animator.GetBool("IsIdling");
        bool isAwake = m_Animator.GetBool("IsAwake");

        if (GameManager.Instance.State != GAMESTATE.PLAY)
        {
            if (!isIdling)
            {
                m_Animator.SetBool("IsAwake", false);
                m_Animator.SetBool("IsIdling", true);
            }
            return;
        }

        if (TimeSinceSpawn > 5)
        {
            m_Animator.SetBool("IsAwake", true);
        }
    }
}
