using System;
using System.Linq;
using JetBrains.Annotations;
using MatrixUtils.DependencyInjection;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RunnerCharacter : MonoBehaviour
{
    [SerializeField] float m_jumpHeight = 2f;
    [SerializeField] float m_maxGroundAngle = 45f;
    [SerializeField] float m_gravity = -20f;
    
    bool m_desiresJump;
    bool m_isGrounded;
    float m_minGroundDotProduct;
    Rigidbody2D m_rigidBody;
    Vector2 m_velocity;
    
    [Inject, UsedImplicitly]
    void SetupInputs(IInputHandler handler)
    {
        handler.Jump.AddListener(Jump);
        handler.Crouch.AddListener(Crouch);
    }

    void OnValidate()
    {
        m_minGroundDotProduct = Mathf.Cos(m_maxGroundAngle * Mathf.Deg2Rad);
    }

    void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_rigidBody.bodyType = RigidbodyType2D.Dynamic;
        m_rigidBody.gravityScale = 0f;
        m_velocity = Vector2.zero;
        OnValidate();
    }

    void FixedUpdate()
    {
        HandleJump();
        ApplyGravity();
        m_rigidBody.linearVelocity = m_velocity;
        m_isGrounded = false;
    }

    void ApplyGravity()
    {
        if (m_isGrounded) return;
        m_velocity.y += m_gravity * Time.fixedDeltaTime;
    }

    void HandleJump()
    {
        if (!m_desiresJump) return;
        if (m_isGrounded)
        {
            float jumpSpeed = Mathf.Sqrt(-2f * m_gravity * m_jumpHeight);
            if (m_velocity.y > 0)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - m_velocity.y, 0f);
            }
            m_velocity.y += jumpSpeed;
        }
        m_desiresJump = false;
    }

    void Crouch()
    {
        
    }

    void Jump()
    {
        if (m_desiresJump) return;
        m_desiresJump = true;
    }

    public void Swing()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        EvaluateCollision(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        EvaluateCollision(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        m_isGrounded = false;
    }

    void EvaluateCollision(Collision2D collision)
    {
        if (!collision.contacts.Any(contact => contact.normal.y >= m_minGroundDotProduct)) return;
        m_isGrounded = true;
        if (m_velocity.y < 0)
        {
            m_velocity.y = 0;
        }
    }
}