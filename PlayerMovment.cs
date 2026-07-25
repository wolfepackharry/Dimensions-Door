using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerMovment : MonoBehaviour
{
    private Rigidbody2D _rb;

    [SerializeField] private float _speed = 5.0f;
    private float _jumpForce = 8f;
    private SpriteRenderer _sr;
    [SerializeField] private float _acceleration = 40f;
    private bool _canDash = true;
    private bool _dashing;
    private bool _dashPressed;
    private bool _jumpPressed;
    private bool _jumpHeld;
    private float _gracePeriod = 0.5f;
    private bool _grace = true;
    public static PlayerMovment Singleton;
    private Action _onShift;
    private Action _onDie;
    private Action _onReturn;
    private Vector3 _positionBeforeDeath;
    public Action OnShift {get {return _onShift;} set{_onShift = value;}}
    public Action OnReturn {get {return _onReturn;} set{_onReturn = value;}}
    public Action OnDie {get {return _onDie;} set {_onDie = value;}}

    private KeyControl _jumpKey;
    private KeyControl _rightKey;
    private KeyControl _leftKey;
    private KeyControl _dashKey;
    private KeyControl _shiftKey;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Singleton == null){Singleton = this;}
        else if (Singleton != this){Destroy(gameObject);}
        _jumpKey = Keyboard.current.wKey;
        _rightKey = Keyboard.current.dKey;
        _leftKey = Keyboard.current.aKey;
        _dashKey = Keyboard.current.leftShiftKey;
        _shiftKey = Keyboard.current.spaceKey;
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;
        _dashPressed = _dashKey.isPressed;
        _jumpPressed = _jumpKey.wasPressedThisFrame;
        _jumpHeld = _jumpKey.isPressed;
        if (CheckTargetable()){CheckWallJump();}
        CheckShift();
    }

    private void CheckSquish()
    {
        if (!CheckTargetable())
        {
            print("NO");
            return;
        }

        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.2f, LayerMask.GetMask("Ground"));
        //print("getting squish == " + hit.transform);
        if (hit != null)
        {
            print("DIE");
            OnDie?.Invoke();
            Time.timeScale = 1;
            transform.gameObject.layer = LayerMask.NameToLayer("Player");
            transform.position = RespawnManager.Singleton.GetCurrentRespawnPoint(_positionBeforeDeath);
            _rb.linearVelocity = Vector2.zero;

        }
    }

    private void OnBecameInvisible()
    {
        OnDie?.Invoke();
        Time.timeScale = 1;
        transform.gameObject.layer = LayerMask.NameToLayer("Player");
        transform.position = RespawnManager.Singleton.GetCurrentRespawnPoint(_positionBeforeDeath);
        _rb.linearVelocity = Vector2.zero;
    }

    private void CheckShift()
    {
        if (_shiftKey.wasPressedThisFrame)
        {
            if (CheckTargetable()){OnShift?.Invoke();}
            else{OnReturn?.Invoke();}
            Time.timeScale = gameObject.layer == LayerMask.NameToLayer("Player") ? 0.7f : 1f;
            gameObject.layer = gameObject.layer == LayerMask.NameToLayer("Player") ? LayerMask.NameToLayer("Untargetable") : LayerMask.NameToLayer("Player");
        }
    }

    private bool CheckTargetable()
    {
        return gameObject.layer == LayerMask.NameToLayer("Player");
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.timeScale == 0) return;
        print(CheckGrounded());
        if (RespawnManager.Singleton.GetCurrentRespawnPoint(transform.position) != Vector3.positiveInfinity){_positionBeforeDeath = transform.position;}
        if (CheckGrounded() && !_dashing){_canDash = true;}
        CheckSquish();
        if (_dashing){return;}
        CheckMovement();
        if (CheckTargetable()) {CheckJump();}
        CheckDash();
    }

    private void CheckDash()
    {
        if (_dashPressed && _canDash && !CheckGrounded())
        {
            StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        _rb.gravityScale = 0f;
        _rb.AddForce(Vector3.right * 5 * (_sr.flipX ? -1 : 1), ForceMode2D.Impulse);
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);
        _canDash = false;
        _dashing = true;
        yield return new WaitForSeconds(0.2f);
        _dashing = false;
        _rb.gravityScale = 2f;
    }
    private void CheckWallJump()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position + (_sr.bounds.size.x / 2 * Vector3.left), Vector2.left, 0.1f, LayerMask.GetMask("Ground"));
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position + (_sr.bounds.size.x / 2 * Vector3.right), Vector2.right, 0.1f, LayerMask.GetMask("Ground"));
        
        print(hitLeft.transform);
        if (hitRight.transform != null && hitLeft.transform != null)
        {
            OnDie?.Invoke();
            Time.timeScale = 1;
            transform.gameObject.layer = LayerMask.NameToLayer("Player");
            transform.position = RespawnManager.Singleton.GetCurrentRespawnPoint(_positionBeforeDeath);
            _rb.linearVelocity = Vector2.zero;
        }
        if (hitRight.transform != null || hitLeft.transform != null)
        {
            _sr.color = Color.red;
            if (!_rightKey.isPressed && _grace && hitRight && !CheckGrounded()) //TODO: Make grace period where the player doesnt have to be holding move key but will still move at wall for friction
            {
                if (_gracePeriod > 0)
                {
                    _rb.linearVelocity = new Vector2(_speed, 0);
                    _gracePeriod -= Time.deltaTime;
                }
                else
                {
                    _gracePeriod = 0.5f;
                    _grace = false;
                }
            }
            else if (!_leftKey.isPressed && _grace && hitLeft && !CheckGrounded())
            {
                if (_gracePeriod > 0)
                {
                    _rb.linearVelocity = new Vector2(-_speed, 0);
                    _gracePeriod -= Time.deltaTime;
                }
                else
                {
                    _gracePeriod = 0.5f;
                    _grace = false;
                }
            }
        }
        else
        {
            _sr.color = Color.white;
        }
        if (_jumpPressed && ((_rightKey.isPressed && hitLeft.transform != null && !_leftKey.isPressed) || (hitRight.transform != null && _leftKey.isPressed && !_rightKey.isPressed)))
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
            _canDash = true;
            _grace = false;
        }
        print(_jumpPressed);
    }

    private void CheckJump()
    {
        if (_jumpHeld && CheckGrounded())
        {
            _jumpForce = Mathf.Clamp(_jumpForce + Time.deltaTime * 20, 8, 10);
        }
        if ((_jumpForce > 8f && !_jumpPressed) || _jumpForce == 10)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
            _jumpForce = 8f;
        }
    }

    private bool CheckGrounded()
    {
        if (!CheckTargetable()){return false;}
        RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.down * (_sr.bounds.size.y / 2), Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
        Debug.DrawRay(transform.position + (Vector3.down * _sr.bounds.size.y / 2), Vector3.down , Color.red);
        return hit.transform != null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spikes"))
        {
            OnDie?.Invoke();
            Time.timeScale = 1;
            transform.gameObject.layer = LayerMask.NameToLayer("Player");
            transform.position = RespawnManager.Singleton.GetCurrentRespawnPoint(_positionBeforeDeath);
            _rb.linearVelocity = Vector2.zero;
        }
    }
    private void CheckMovement()
    {
        float moveX = 0f;
        if (Keyboard.current != null)
        {
            moveX = (_rightKey.isPressed ? 1 : 0) - (_leftKey.isPressed ? 1 : 0);
        }
        if (moveX != 0f){_sr.flipX = moveX < 0;}
        float targetX = moveX * _speed;
        float newX = Mathf.MoveTowards(_rb.linearVelocity.x, targetX, _acceleration * Time.fixedDeltaTime);
        _rb.linearVelocity = new Vector2(newX, _rb.linearVelocity.y);
    }

    public void ChangeKey(PopUpManager.Key keyToReplace, KeyControl keyValue)
    {
        switch (keyToReplace)
        {
            case PopUpManager.Key.Shift:
                _shiftKey = keyValue;
                break;
            case PopUpManager.Key.A:
                _leftKey = keyValue;
                break;
            case PopUpManager.Key.D:
                _rightKey = keyValue;
                break;
            case PopUpManager.Key.W:
                _jumpKey = keyValue;
                break;
            case PopUpManager.Key.Dash:
                _dashKey = keyValue;
                break;
        }
    }


    public bool CheckKey(KeyControl key)
    {
        return key != _shiftKey && key != _leftKey && key != _rightKey && key != _dashKey && key != _jumpKey;
    }

    public KeyControl GetKey(PopUpManager.Key currentKey)
    {
        switch (currentKey)
        {
            case PopUpManager.Key.Shift:
                return _shiftKey;
            case PopUpManager.Key.A:
                return _leftKey;
            case PopUpManager.Key.D:
                return _rightKey;
            case PopUpManager.Key.W:
                return _jumpKey;
            case PopUpManager.Key.Dash:
                return _dashKey;
        }
        return null;
    }
}