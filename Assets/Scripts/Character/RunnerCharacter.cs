using System;
using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using MatrixUtils.DependencyInjection;
using MatrixUtils.GenericDatatypes;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class RunnerCharacter : MonoBehaviour
{
    [SerializeField] float m_jumpHeight = 2f;
    [SerializeField] float m_maxGroundAngle = 45f;
    [SerializeField] float m_gravity = -20f;
    [SerializeField] float m_fastFallGravityMultiplier = 2f;
    float m_currentGravity;
    public UnityEvent m_onJump = new();
    public UnityEvent m_onRoll = new();
    public Observer<bool> m_isGrounded = new(false);
    bool m_desiresJump;
    bool m_rolling;
    float m_minGroundDotProduct;
    Vector2 m_originalColliderSize;
    Vector2 m_originalColliderOffset;
    Rigidbody2D m_rigidBody;
    BoxCollider2D m_collider;
    Coroutine m_rollRoutine;
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
        m_collider = GetComponent<BoxCollider2D>();
        m_rigidBody.bodyType = RigidbodyType2D.Dynamic;
        m_rigidBody.gravityScale = 0f;
        m_originalColliderSize = m_collider.size;
        m_originalColliderOffset = m_collider.offset;
        m_currentGravity = m_gravity;
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
        m_velocity.Value += new Vector2(0,m_currentGravity * Time.fixedDeltaTime);
    }

    void HandleJump()
    {
        if (!m_desiresJump) return;
        if(m_rolling) CancelRoll();
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
        if(m_rolling) return;
        if (m_isGrounded)
        {
            m_onRoll.Invoke();
            m_rollRoutine = StartCoroutine(RollRoutine());
        }
        else
        {
            m_rollRoutine = StartCoroutine(DropAndRollRoutine());
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

    public void RollComplete()
    {
        m_rolling = false;
    }

    public void RollStarted()
    {
        m_rolling = true;
    }

    IEnumerator DropAndRollRoutine()
    {
        m_currentGravity = m_gravity * m_fastFallGravityMultiplier;
        yield return new WaitUntil(() => m_isGrounded.Value);
        m_onRoll.Invoke();
        m_currentGravity = m_gravity;
        yield return RollRoutine();
    }
    IEnumerator RollRoutine()
    {
        yield return new WaitUntil(() => m_rolling);
        float heightDifference = m_originalColliderSize.y - m_originalColliderSize.y / 2;
        m_collider.size = new(m_originalColliderSize.x, m_originalColliderSize.y - heightDifference);
        m_collider.offset = new(m_originalColliderOffset.x, m_originalColliderOffset.y - heightDifference / 2f);
        yield return new WaitUntil(() => !m_rolling);
        CancelRoll();
    }
    void CancelRoll()
    {
        m_rolling = false;
        StopCoroutine(m_rollRoutine);
        m_collider.size = m_originalColliderSize;
        m_collider.offset = m_originalColliderOffset;
    }
}