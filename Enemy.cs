using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Vector3 _direction = Vector3.right;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rb;
    private Animator _animator;
    private float _acceleration = 10.0f;
    private float _speed = 3.0f;
    private bool _idling = true;
    [SerializeField]private float _range = 1.5f;
    [SerializeField]private float _damage;
    private bool _attacking;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        PlayerMovment.Singleton.OnShift += Shift;
        PlayerMovment.Singleton.OnReturn += Return;
    }

    private void Shift()
    {
        _spriteRenderer.color = Color.red;
    }

    private void Return()
    {
        _spriteRenderer.color = Color.white;
    }

    private void Update()
    {
        CheckPlayer();
    }

    private void CheckPlayer()
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position + (_spriteRenderer.bounds.size.x / 2 * Vector3.right * _direction.x), _direction, 10, LayerMask.GetMask("Player"));
        if (raycastHit2D.transform != null)
        {
            _idling = false;
        }
    }

    private void FixedUpdate()
    {
        if (!_idling)
        {
            if (Vector3.Distance(transform.position, PlayerMovment.Singleton.transform.position) <= _range + (_spriteRenderer.bounds.size.x / 2 * _direction.x))
            {
                Attack();
            }
            else
            {
                _direction = new Vector3(PlayerMovment.Singleton.transform.position.x - transform.position.x, 0, 0);
                Move();
            }
            return;
        }
        CheckWall();
        CheckEdge();
        Move();
    }

    private void Attack()
    {
        if (!_attacking)
        {
            _animator.SetTrigger("Attack");
            _attacking = true;
        }
    }

    private void OnAttackAnimFinished()
    {
        Vector3 topRight = transform.position + (_spriteRenderer.bounds.size.y / 2 * Vector3.up * _direction.x) + (_spriteRenderer.bounds.size.x * Vector3.right * _direction.x);
        Vector3 bottomLeft = transform.position + (_spriteRenderer.bounds.size.y / 2 * Vector3.down * _direction.x) + (_range * Vector3.right * _direction.x);
        Collider2D hit = Physics2D.OverlapArea(topRight, bottomLeft, LayerMask.GetMask("Player"));
        if (hit.transform != null)
        {
            if (hit.GetComponent<HealthController>() != null)
            {
                hit.GetComponent<HealthController>().TakeDamage(_damage);
            }
        }
    }

    private void Move()
    {
         float targetX = _direction.x * _speed;
        float newX = Mathf.MoveTowards(_rb.linearVelocity.x, targetX, _acceleration * Time.fixedDeltaTime);
        _rb.linearVelocity = new Vector2(newX, _rb.linearVelocity.y);
    }

    private void CheckEdge()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + (_spriteRenderer.bounds.size.x / 2 * Vector3.right * _direction.x) + (_spriteRenderer.bounds.size.y / 2 * _direction.x * Vector3.right), Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
        if (hit.collider == null)
        {
            _direction = -_direction;
        }
    }

    private void CheckWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + (_spriteRenderer.bounds.size.x / 2 * Vector3.right * _direction.x), _direction, 0.5f, LayerMask.GetMask("Ground"));
        if (hit.collider == null)
        {
            _direction = -_direction;
        }
    }
}