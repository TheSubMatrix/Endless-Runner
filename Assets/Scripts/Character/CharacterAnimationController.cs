using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    static readonly int s_grounded = Animator.StringToHash("Grounded");
    static readonly int s_airSpeedY = Animator.StringToHash("AirSpeedY");
    static readonly int s_animState = Animator.StringToHash("AnimState");
    static readonly int s_runSpeed = Animator.StringToHash("RunSpeed");
    Animator m_animator;
    void Awake()
    {
        m_animator = GetComponent<Animator>();
    }
    public void ChangeGroundedState(bool isGrounded)
    {
        m_animator.SetBool(s_grounded, isGrounded);
    }

    public void UpdateVelocityY(Vector2 velocity)
    {
        m_animator.SetFloat(s_airSpeedY, velocity.y);
    }

    public void UpdateMovementState(float speed)
    {
        m_animator.SetInteger(s_animState, speed > 0 ? 1 : 0);
    }
    public void UpdateMovementSpeed(float speed)
    {
        m_animator.SetFloat(s_runSpeed,speed / 2);
    }
}
