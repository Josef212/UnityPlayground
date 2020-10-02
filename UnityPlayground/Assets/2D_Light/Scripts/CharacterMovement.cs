using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private void Awake()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        var dx = Input.GetAxis("Horizontal");
        var dy = Input.GetAxis("Vertical");
        
        var direction = new Vector2(dx, dy).normalized * m_movementSpeed;
        m_velocity = direction;
    }

    private void FixedUpdate()
    {
        m_rigidbody2D.position += m_velocity * Time.fixedDeltaTime;
    }

    [SerializeField] private float m_movementSpeed = 5.0f;

    private Rigidbody2D m_rigidbody2D = null;
    private Vector2 m_velocity = Vector2.zero;
}
