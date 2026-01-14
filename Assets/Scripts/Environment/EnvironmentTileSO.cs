using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Environment/Environment Tile", fileName = "New Environment Tile")]
public class EnvironmentTileSO : ScriptableObject
{
    [field: SerializeField] public GameObject TilePrefab { get; private set; }
    Bounds m_tileBounds;

    public Bounds GetBounds()
    {
        if (TilePrefab == null) return new();
        return m_tileBounds.size != Vector3.zero ? m_tileBounds : CalculateBounds();

    }

    Bounds CalculateBounds()
    {
        if (TilePrefab == null) return new();
        Renderer[] renderers = TilePrefab.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0) return new(TilePrefab.transform.position, Vector3.zero);
        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }
        return bounds;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (TilePrefab == null)
        {
            m_tileBounds = new();
            return;
        }

        m_tileBounds = CalculateBounds();
    }
#endif
}
