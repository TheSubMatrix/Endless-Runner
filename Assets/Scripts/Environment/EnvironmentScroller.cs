using System;
using System.Collections.Generic;
using MatrixUtils.GenericDatatypes;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

public class EnvironmentScroller : MonoBehaviour
{
    [SerializeField] float m_edgePadding = 2f;
    [SerializeField] List<EnvironmentTilePool> m_tilePools;
    [SerializeField] Observer<float> m_distanceTraveled;
    [SerializeField] Observer<float> m_scrollSpeed = new(0);
    readonly LinkedList<ActiveTile> m_activeTiles = new();
    float m_cameraRightBound;
    float m_cameraLeftBound;
    public void SetScrollSpeed(float speed) => m_scrollSpeed.Value = speed;
    void Start()
    {
        m_scrollSpeed.Notify();
        if (Camera.main == null)return;
        
        m_cameraLeftBound = Camera.main.ViewportToWorldPoint(new(0, 0.5f, Camera.main.nearClipPlane)).x;
        m_cameraRightBound = Camera.main.ViewportToWorldPoint(new(1, 0.5f, Camera.main.nearClipPlane)).x;
        foreach (EnvironmentTilePool pool in m_tilePools)
        {
            pool.Initialize();
        }

        float spawnX = m_cameraLeftBound;
        while (spawnX < m_cameraRightBound)
        {
            EnvironmentTilePool pool = m_tilePools[UnityEngine.Random.Range(0, m_tilePools.Count)];
            ActiveTile activeTile = pool.Get();
            activeTile.Tile.transform.position = new(spawnX + pool.TileBounds.extents.x, transform.position.y, 0);
            m_activeTiles.AddLast(activeTile);
            spawnX += pool.TileBounds.size.x;
        }
    }

    void FixedUpdate()
    {
        float scrollAmount = m_scrollSpeed * Time.fixedDeltaTime;
        foreach (ActiveTile activeTile in m_activeTiles)
        {
            activeTile.Rigidbody.MovePosition(activeTile.Rigidbody.position + Vector2.left * scrollAmount);
        }

        if (m_activeTiles.Count == 0)
        {
            return;
        }

        ActiveTile leftmost = m_activeTiles.First.Value;
        float leftmostRightEdge = leftmost.Tile.transform.position.x + leftmost.Pool.TileBounds.extents.x;
        if (leftmostRightEdge < m_cameraLeftBound)
        {
            leftmost.Pool.Release(leftmost);
            m_activeTiles.RemoveFirst();
        }

        ActiveTile rightmost = m_activeTiles.Last.Value;
        float rightmostRightEdge = rightmost.Tile.transform.position.x + rightmost.Pool.TileBounds.extents.x;

        while (rightmostRightEdge < m_cameraRightBound + m_edgePadding)
        {
            EnvironmentTilePool pool = m_tilePools[UnityEngine.Random.Range(0, m_tilePools.Count)];
            ActiveTile activeTile = pool.Get();
            Vector3 newPos = new(rightmostRightEdge + pool.TileBounds.extents.x - scrollAmount, transform.position.y, 0);
            activeTile.Tile.transform.position = newPos;
            m_activeTiles.AddLast(activeTile);
            rightmost = activeTile;
            rightmostRightEdge = rightmost.Tile.transform.position.x + rightmost.Pool.TileBounds.extents.x;
        }
        m_distanceTraveled.Value += scrollAmount;
    }
}

public class ActiveTile
{
    public ActiveTile(GameObject tile, EnvironmentTilePool pool)
    {
        Tile = tile;
        Pool = pool;
        Rigidbody = tile.GetComponent<Rigidbody2D>();
    }
    
    public readonly GameObject Tile;
    public readonly Rigidbody2D Rigidbody;
    public readonly EnvironmentTilePool Pool;
}

[Serializable]
public class EnvironmentTilePool : IObjectPool<ActiveTile>
{
    [SerializeField] EnvironmentDataAggregator m_tilePrefab;
    public Bounds TileBounds => m_tilePrefab.PrefabBounds;
    ObjectPool<ActiveTile> m_pool;
    
    public void Initialize()
    {
        m_pool = new(
            createFunc: () =>
            {
                GameObject obj = Object.Instantiate(m_tilePrefab.gameObject);
                return new(obj, this);
            },
            actionOnGet: activeTile => activeTile.Tile.SetActive(true),
            actionOnRelease: activeTile => activeTile.Tile.SetActive(false),
            actionOnDestroy: activeTile => Object.Destroy(activeTile.Tile),
            defaultCapacity: 10,
            maxSize: 100
        );
    }
    
    public ActiveTile Get()
    {
        return m_pool.Get();
    }
    
    public PooledObject<ActiveTile> Get(out ActiveTile v)
    {
        return m_pool.Get(out v);
    }
    
    public void Release(ActiveTile element)
    {
        m_pool.Release(element);
    }
    
    public void Clear()
    {
        m_pool.Clear();
    }
    
    public int CountInactive => m_pool.CountInactive;
}