using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusAnimationController : MonoBehaviour
{
    private Animator m_Animator;

    [SerializeField] private AnimationClip walk;
    [SerializeField] private AnimationClip run;


    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
