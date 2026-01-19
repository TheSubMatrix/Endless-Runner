using System;
using System.Collections.Generic;
using System.Linq;
using MatrixUtils.Attributes;
using UnityEditor;
using UnityEngine;

public class EnvironmentDataAggregator : MonoBehaviour
{
    [field: SerializeField, ReadOnly] public Bounds PrefabBounds { get; private set; }
    [ReadOnly] public Rigidbody2D Rigidbody2D;
    [ContextMenu("Recalculate Bounds", true)]
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
