using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    #region Getters and Setters
    public static Player Instance { get; private set; }
    public PlayerInput PlayerInput { get => _playerInput; }

    #endregion


    #region Private Variables
    private PlayerInput _playerInput;
    #endregion  
    private void Awake()
    {
        Instance = this;
    
        _playerInput = GetComponent<PlayerInput>();
    }
}
