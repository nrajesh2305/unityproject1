using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public partial class BaseEnemy : MonoBehaviour, ICharacter
{
    [SerializeField]
    private float health;
    [SerializeField]
    private float moveSpeed = 4f;

    private Rigidbody2D rb;
    public float Health
    {
        get => health;
        set => health = Mathf.Max(value, 0);
    }
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = Mathf.Max(value, 0);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Attack()
    {
        throw new System.NotImplementedException();
    }

    public void Move(Vector2 direction)
    {
        Vector2 move = direction.normalized * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + move);
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        // Kill the enemy and add to the score manager ? Invoke death event??
    }
}
