using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class EnvironmentDataAggregator : MonoBehaviour
{
    [field: SerializeField] public Bounds PrefabBounds { get; private set; }
    public Rigidbody2D Rigidbody2D;
    [FormerlySerializedAs("OnResetForPool")] public UnityEvent OnCleanup = new();
    [FormerlySerializedAs("OnSpawnedForPool")] public UnityEvent OnInitialize = new();
    [ContextMenu("Recalculate Bounds")]
    void Reset()
    {
        Bounds localBounds = new(Vector3.zero, Vector3.zero);
        foreach (Collider2D foundCollider in GetComponentsInChildren<Collider2D>())
        {
            Vector3 localCenter = transform.InverseTransformPoint(foundCollider.bounds.center);
            Vector3 localSize = foundCollider.bounds.size;
            localBounds.Encapsulate(new Bounds(localCenter, localSize));
        }
        PrefabBounds = localBounds;
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }
}
