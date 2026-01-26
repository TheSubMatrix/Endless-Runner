using System;
using System.Linq;
using JetBrains.Annotations;
using MatrixUtils.DependencyInjection;
using MatrixUtils.GenericDatatypes;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class RunnerCharacter : MonoBehaviour
{
    [SerializeField] float m_jumpHeight = 2f;
    [SerializeField] float m_maxGroundAngle = 45f;
    [SerializeField] float m_gravity = -20f;
    public UnityEvent m_onJump = new();
    public Observer<bool> m_isGrounded = new(false);
    bool m_desiresJump;
    float m_minGroundDotProduct;
    Rigidbody2D m_rigidBody;
    public Observer<Vector2> m_velocity = new(new());
    
    [Inject, UsedImplicitly]
    void SetupInputs(IInputHandler handler)
    {
        handler.Jump.AddListener(Jump);
        handler.Roll.AddListener(Roll);
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
        m_velocity.Value = Vector2.zero;
        OnValidate();
    }

    void FixedUpdate()
    {
        HandleJump();
        ApplyGravity();
        m_rigidBody.linearVelocity = m_velocity;
        m_isGrounded.Value = false;
    }

    void ApplyGravity()
    {
        if (m_isGrounded) return;
        m_velocity.Value += new Vector2(0,m_gravity * Time.fixedDeltaTime);
    }

    void HandleJump()
    {
        if (!m_desiresJump) return;
        if (m_isGrounded)
        {
            float jumpSpeed = Mathf.Sqrt(-2f * m_gravity * m_jumpHeight);
            if (m_velocity.Value.y > 0)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - m_velocity.Value.y, 0f);
            }
            m_velocity.Value += new Vector2(0,jumpSpeed);
            m_onJump.Invoke();
        }
        m_desiresJump = false;
    }

    void Roll()
    {
        if (!m_isGrounded)
        {
            
        }
        
        
    }

    void Jump()
    {
        if (m_desiresJump) return;
        m_desiresJump = true;
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
        m_isGrounded.Value = false;
    }

    void EvaluateCollision(Collision2D collision)
    {
        if (!collision.contacts.Any(contact => contact.normal.y >= m_minGroundDotProduct)) return;
        m_isGrounded.Value = true;
        if (m_velocity.Value.y < 0)
        {
            m_velocity.Value = new(m_velocity.Value.x, 0);
        }
    }
}