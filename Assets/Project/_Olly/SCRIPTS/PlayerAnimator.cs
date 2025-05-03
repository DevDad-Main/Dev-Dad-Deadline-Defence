using System.Collections;
using System.Collections.Generic;
using TopDown_Template;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Movment m_PlayerMovement;

    private Animator _animator => GetComponent<Animator>(); 

    void Update()
    {
        _animator.SetBool(Utils.movingAnimation, m_PlayerMovement.CurrentVelocity != Vector2.zero);
    }
}
