using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.InputSystem;


namespace TopDown_Template
{
    public class CharacterInputContoller : InputController
    {
        #region Variable
        [Header("CharacterStats")]
        [SerializeField] private float _moveSpeed;

        [Header("Input Setting")]
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private GameObject _mobileInput;
        [SerializeField] private Vector2 _thresholdGamePad;
        //[SerializeField] private TopDownBase _topDownInput;

        private Vector2 _currentDelta = Vector2.zero;
        private Camera _camera;
        private bool _isGamepad = false;
        private bool _mouseOverrideDone = false;
        private InputAction _movment;
        private InputAction _look;
        private InputAction _attack;
        private InputAction _reloading;
        private InputAction _switchWeapon;
        private InputAction _interact;
        private InputAction _dash;
        private Vector2 _lookInput;
        private Vector2 _moveInput;
        private Vector2 _newAim;
        private Vector2 _newAimScreenToWorld;


        //private CharacterDash _characterDash => GetComponent<CharacterDash>();
        #endregion

        #region Getters and Setters
        ////public Vector2 MoveInput { get => _moveInput;}
        public Vector2 LookDirection { get => _lookInput; }
        public Vector2 MoveDirection { get => _moveInput; }
        //public TopDownBase TopDownInput {  get =>  _topDownInput; }

        public string CurrentScheme { get; private set; } = "Keyboard&Mouse";

        public Action<string> OnSchemeChanged;
        private string _lastScheme;

        public Action OnInteractPressed;
        #endregion



        #region Unity Callback
        private void Awake()
        {
            _camera = Camera.main;
        }

        private void OnEnable()
        {
            SettingInput();
        }

        private void OnDisable()
        {
            _playerInput.onControlsChanged -= OnControlsChanged;
        }

        public void Update()
        {
            DetectMouseOverride();
            OnLook();
            OnAttack();

            //_characterDash.UpdateDash();
            LookEvent?.Invoke(_newAimScreenToWorld);
        }
        private void FixedUpdate()
        {
            PointPositionEvent?.Invoke(_newAim);
        }
        #endregion

        #region CharacterInputContoller Method
        private void SettingInput()
        {
            _playerInput.onControlsChanged += OnControlsChanged;

            var actionMap = _playerInput.actions.FindActionMap("Player");


            _movment = actionMap.FindAction("Move");
            _look = actionMap.FindAction("Look");

            _attack = actionMap.FindAction("Attack");
            _reloading = actionMap.FindAction("Reloading");
            _switchWeapon = actionMap.FindAction("SwitchWeapon");
            _interact = actionMap.FindAction("Interact");
            //_dash = actionMap.FindAction("Dash");

            _movment.Enable();
            _look.Enable();

            _attack.Enable();
            _reloading.Enable();
            _switchWeapon.Enable();
            _interact.Enable();
            //_dash.Enable();

            _interact.started += OnInteract;
            _switchWeapon.started += OnSwitchWeapon;
            _reloading.started += OnReloading;
            //_dash.started += OnDash;

            _movment.performed += OnMove;
            _movment.started += OnMove;
            _movment.canceled += OnMove;


            //_movment.performed += HandleInput;


            #region Old Code
            //_movment = _topDownInput.Player.Move;
            //_look = _topDownInput.Player.Look;

            //_attack = _topDownInput.Player.Attack;
            //_reloading = _topDownInput.Player.Reloading;
            //_switchWeapon = _topDownInput.Player.SwitchWeapon;
            //_interact = _topDownInput.Player.Interact;
            //_dash = _topDownInput.Player.Dash;

            //_movment.Enable();
            //_look.Enable();

            //_attack.Enable();
            //_reloading.Enable();
            //_switchWeapon.Enable();
            //_interact.Enable();
            //_dash.Enable();

            //_interact.started += OnInteract;
            //_switchWeapon.started += OnSwitchWeapon;
            //_reloading.started += OnReloading;
            //_dash.started += OnDash;

            //_movment.performed += OnMove;
            //_movment.started += OnMove;
            //_movment.canceled += OnMove;


            //_movment.performed += HandleInput;
            #endregion

            // Subscribe to any action (e.g., movement)
            //_input.Player.Shoot.performed += OnInputPerformed;
        }

        private Vector2 _lastAimDirection = Vector2.right; // Default aim direction

        public void OnLook()
        {
            _lookInput = _look.ReadValue<Vector2>();

            if (!_isGamepad)
            {
                // Mouse aiming
                _newAim = _lookInput;
                _newAimScreenToWorld = _camera.ScreenToWorldPoint(_newAim);
            }
            else
            {
                bool hasLookInput = _lookInput.magnitude > 0.1f;
                bool hasMoveInput = _moveInput.magnitude > 0.1f;

                if (hasLookInput)
                {
                    _lastAimDirection = _lookInput.normalized;
                }
                else if (hasMoveInput)
                {
                    _lastAimDirection = _moveInput.normalized;
                }

                _currentDelta = _lastAimDirection;

                _currentDelta.x *= _thresholdGamePad.x;
                _currentDelta.y *= _thresholdGamePad.y;

                _newAim = (Vector2)transform.position + _currentDelta;
                _newAimScreenToWorld = _newAim;
                _newAim = _camera.WorldToScreenPoint(_newAim);
            }
        }

        //public void OnLook()
        //{
        //    _lookInput = _look.ReadValue<Vector2>();

        //    if (!_isGamepad)
        //    {
        //        // Mouse aiming
        //        _newAim = _lookInput;
        //        _newAimScreenToWorld = _camera.ScreenToWorldPoint(_newAim);
        //    }
        //    else
        //    {
        //        // Only update the last known aim if the stick is being used
        //        if (_lookInput.magnitude > 0.1f)
        //        {
        //            _lastAimDirection = _lookInput.normalized;
        //        }

        //        // Always use the last valid direction
        //        _currentDelta = _lastAimDirection;

        //        _currentDelta.x *= _thresholdGamePad.x;
        //        _currentDelta.y *= _thresholdGamePad.y;

        //        _newAim = (Vector2)transform.position + _currentDelta;
        //        _newAimScreenToWorld = _newAim;
        //        _newAim = _camera.WorldToScreenPoint(_newAim);
        //    }
        //}



        private void DetectMouseOverride()
        {
            if (_mouseOverrideDone || _playerInput == null) return;

            // If the mouse moves OR user clicks something
            if (Mouse.current.delta.ReadValue() != Vector2.zero || Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (_playerInput.currentControlScheme != "Keyboard&Mouse")
                {
                    _playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
                }

                _isGamepad = false;
                CurrentScheme = "Keyboard&Mouse";
                _lastScheme = CurrentScheme;
                OnSchemeChanged?.Invoke(CurrentScheme);
                SwitchDeviceInput?.Invoke(false);

                _mouseOverrideDone = true;
            }
        }


        private void OnControlsChanged(PlayerInput input)
        {
            string scheme = input.currentControlScheme;

            if (scheme != _lastScheme)
            {
                _lastScheme = scheme;
                CurrentScheme = scheme;
                OnSchemeChanged?.Invoke(scheme);

                _isGamepad = scheme.Contains("Gamepad");
                SwitchDeviceInput?.Invoke(_isGamepad);
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = _movment.ReadValue<Vector2>().normalized * _moveSpeed;
            OnMoveEvent?.Invoke(_moveInput);

        }
        public void OnAttack()
        {
            if (_attack.ReadValue<float>() > 0.5f)
            {
                OnAttackEvent?.Invoke(true);
            }
            else
            {
                OnAttackEvent?.Invoke(false);
            }
        }

        //private void OnDash(InputAction.CallbackContext context)
        //{
        //    OnDashEvent?.Invoke();
        //    _characterDash.HandleDash(_moveInput);
        //}

        public void OnReloading(InputAction.CallbackContext context)
        {
            OnReloadingWeaponEvent?.Invoke();
        }
        public void OnSwitchWeapon(InputAction.CallbackContext context)
        {
            OnChangeWeaponEvent?.Invoke();
        }
        public void OnInteract(InputAction.CallbackContext context)
        {
            OnInteractionEvent?.Invoke();
        }
        public void ChangeDeviceInput(bool assist)
        {
            _isGamepad = assist;
            SwitchDeviceInput?.Invoke(_isGamepad);
        }
        #endregion
    }
}


    //    public class CharacterInputContoller : InputController
    //    {
    //        #region Variable
    //        [Header("CharacterStats")]
    //        [SerializeField] private float _moveSpeed;

    //        [Header("Input Setting")]
    //        [SerializeField] private PlayerInput _playerInput;
    //        [SerializeField] private Vector2 _thresholdGamePad;
    //        [Tooltip("Seconds to \"Hold Onto\" aim after stick input ends")]
    //        [SerializeField] private float _lookInputGracePeriod = 0.3f; // seconds to "hold onto" aim after stick input ends


    //        private Vector2 _currentDelta = Vector2.zero;
    //        private float _lastActiveLookTime;
    //        private Camera _camera;
    //        private bool _mouseOverrideDone = false;
    //        private string _lastScheme;

    //        private InputAction _movment;
    //        private InputAction _look;
    //        private InputAction _attack;
    //        private InputAction _reloading;
    //        private InputAction _switchWeapon;
    //        private InputAction _interact;
    //        private Vector2 _lookInput;
    //        private Vector2 _moveInput;
    //        private Vector2 _newAim;
    //        private Vector2 _newAimScreenToWorld;

    //        private string _currentControlScheme;
    //        private bool _isGamepad = false;
    //        #endregion
    //        #region Unity Callback
    //        private void Awake()
    //        {
    //            _camera = Camera.main;
    //        }

    //        private void Start()
    //        {
    //            //if (_playerInput != null)
    //            //{
    //            //    _playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
    //            //}
    //        }
    //        private void OnEnable()
    //        {
    //            SettingInput();
    //        }
    //        private void OnDisable()
    //        {
    //            _playerInput.onControlsChanged -= OnControlsChanged;
    //        }

    //        public void Update()
    //        {
    //            DetectMouseOverride();
    //            OnLook();
    //            OnAttack();

    //            LookEvent?.Invoke(_newAimScreenToWorld);
    //        }
    //        private void FixedUpdate()
    //        {
    //            PointPositionEvent?.Invoke(_newAim);
    //        }
    //        #endregion

    //        #region CharacterInputContoller Method
    //        private void SettingInput()
    //        {
    //            _playerInput.onControlsChanged += OnControlsChanged;

    //            _movment = _playerInput.actions["Move"];
    //            _look = _playerInput.actions["Look"];

    //            _attack = _playerInput.actions["Attack"];
    //            _reloading = _playerInput.actions["Reloading"];
    //            _switchWeapon = _playerInput.actions["SwitchWeapon"];
    //            _interact = _playerInput.actions["Interact"];


    //            //_movment = _topDownInput.Player.Move;
    //            //_look = _topDownInput.Player.Look;

    //            //_attack = _topDownInput.Player.Attack;
    //            //_reloading = _topDownInput.Player.Reloading;
    //            //_switchWeapon = _topDownInput.Player.SwitchWeapon;
    //            //_interact = _topDownInput.Player.Interact;


    //            _movment.Enable();
    //            _look.Enable();

    //            _attack.Enable();
    //            _reloading.Enable();
    //            _switchWeapon.Enable();
    //            _interact.Enable();

    //            _interact.started += OnInteract;
    //            _switchWeapon.started += OnSwitchWeapon;
    //            _reloading.started += OnReloading;

    //            _movment.performed += OnMove;
    //            _movment.performed += HandleInput;

    //            _movment.started += OnMove;
    //            _movment.canceled += OnMove;
    //        }

    //        private void OnControlsChanged(PlayerInput input)
    //        {
    //            string scheme = input.currentControlScheme;

    //            if (scheme != _lastScheme)
    //            {
    //                _lastScheme = scheme;
    //                _currentControlScheme = scheme;
    //                //OnSchemeChanged?.Invoke(scheme);

    //                _isGamepad = scheme.Contains("Gamepad");
    //                SwitchDeviceInput?.Invoke(_isGamepad);
    //            }
    //        }


    //        private void DetectMouseOverride()
    //        {
    //            if (_playerInput == null) return;

    //            // Allow override again after switching to gamepad
    //            if (_isGamepad)
    //                _mouseOverrideDone = false;

    //            if (_mouseOverrideDone) return;

    //            if (Mouse.current.delta.ReadValue() != Vector2.zero || Mouse.current.leftButton.wasPressedThisFrame)
    //            {
    //                if (_playerInput.currentControlScheme != "Keyboard&Mouse")
    //                {
    //                    _playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
    //                }

    //                _isGamepad = false;
    //                _currentControlScheme = "Keyboard&Mouse";
    //                _lastScheme = _currentControlScheme;
    //                SwitchDeviceInput?.Invoke(false);

    //                _mouseOverrideDone = true;
    //            }
    //        }
    //        public void OnLook()
    //        {
    //            _lookInput = _look.ReadValue<Vector2>();
    //            if (!_isGamepad)
    //            {
    //                _newAim = _lookInput;
    //                _newAimScreenToWorld = _camera.ScreenToWorldPoint(_newAim);
    //            }
    //            if (_isGamepad && (_look.ReadValue<Vector2>().magnitude > 0.9f || (_look.ReadValue<Vector2>().magnitude < 0.5f && _moveInput.magnitude > 0.5f)))
    //            {
    //                if (_look.ReadValue<Vector2>().magnitude > 0.9f)
    //                {
    //                    _currentDelta = _lookInput.normalized;
    //                }
    //                else if (_look.ReadValue<Vector2>().magnitude < 0.5f && _moveInput.magnitude > 0.5f)
    //                {
    //                    _currentDelta = _moveInput.normalized;
    //                }

    //                _currentDelta.x *= _thresholdGamePad.x;
    //                _currentDelta.y *= _thresholdGamePad.y;
    //                _newAim = (Vector2)transform.position + _currentDelta;

    //                _newAimScreenToWorld = _newAim;
    //                _newAim = _camera.WorldToScreenPoint(_newAim);
    //            }
    //        }

    //        void HandleInput(InputAction.CallbackContext ctx)
    //        {
    //            var device = ctx.control.device;

    //            if (device is Gamepad)
    //            {
    //                var name = device.displayName.ToLower();
    //                _currentControlScheme = "Gamepad";
    //            }
    //            else if (device is Keyboard || device is Mouse)
    //            {
    //                _currentControlScheme = "Keyboard&Mouse";
    //            }

    //            if (_currentControlScheme != _lastScheme)
    //            {
    //                _lastScheme = _currentControlScheme;
    //                //OnSchemeChanged?.Invoke(_currentControlScheme);
    //            }
    //        }





    //        //public void OnLook()
    //        //{
    //        //    _lookInput = _look.ReadValue<Vector2>();

    //        //    if (!_isGamepad || _lookInput == Vector2.zero)
    //        //    {
    //        //        // Mouse aiming
    //        //        _newAim = _lookInput;
    //        //        _newAimScreenToWorld = _camera.ScreenToWorldPoint(_newAim);
    //        //    }
    //        //    else if (_lookInput.magnitude > 0.1f || _moveInput.magnitude > 0.5f)
    //        //    {
    //        //        // Controller aiming logic
    //        //        if (_lookInput.magnitude > 0.9f)
    //        //        {
    //        //            _currentDelta = _lookInput.normalized;
    //        //        }
    //        //        else
    //        //        {
    //        //            _currentDelta = _moveInput.normalized;
    //        //        }

    //        //        _currentDelta.x *= _thresholdGamePad.x;
    //        //        _currentDelta.y *= _thresholdGamePad.y;
    //        //        _newAim = (Vector2)transform.position + _currentDelta;

    //        //        _newAimScreenToWorld = _newAim;
    //        //        _newAim = _camera.WorldToScreenPoint(_newAim);
    //        //    }
    //        //}

    //        public void OnMove(InputAction.CallbackContext context)
    //        {
    //            _moveInput = _movment.ReadValue<Vector2>().normalized * _moveSpeed;
    //            OnMoveEvent?.Invoke(_moveInput);

    //        }
    //        public void OnAttack()
    //        {
    //            if (_attack.ReadValue<float>() > 0.5f)
    //            {
    //                OnAttackEvent?.Invoke(true);
    //            }
    //            else
    //            {
    //                OnAttackEvent?.Invoke(false);
    //            }
    //        }
    //        public void OnReloading(InputAction.CallbackContext context)
    //        {
    //            OnReloadingWeaponEvent?.Invoke();
    //        }
    //        public void OnSwitchWeapon(InputAction.CallbackContext context)
    //        {
    //            OnChangeWeaponEvent?.Invoke();
    //        }
    //        public void OnInteract(InputAction.CallbackContext context)
    //        {
    //            OnInteractionEvent?.Invoke();
    //        }
    //        public void ChangeDeviceInput(bool assist)
    //        {
    //            _isGamepad = assist;
    //            SwitchDeviceInput?.Invoke(_isGamepad);
    //        }
    //        #endregion
    //    }