using JetBrains.Annotations;
using MatrixUtils.DependencyInjection;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class RunnerCharacter : MonoBehaviour
{
    [Inject, UsedImplicitly]
    void SetupInputs(IInputHandler handler)
    {
        handler.Jump.AddListener(Jump);
        handler.Crouch.AddListener(Crouch);
    }

    Rigidbody2D m_rigidBody;
    private void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
    }
    public void Crouch()
    {
        Debug.Log("Crouch");
    }

    public void Jump()
    {
        m_rigidBody.AddForce(Vector2.up* 10, ForceMode2D.Impulse);
    }

    public void Swing()
    {
        
    }
}
