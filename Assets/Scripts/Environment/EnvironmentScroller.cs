using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

public class EnvironmentScroller : MonoBehaviour
{
    [SerializeField] float m_scrollSpeed = 1f;
    [SerializeField] List<EnvironmentTilePool> m_tilePools;
    float m_currentSpawnX;
    
    void Start()
    {
        if (Camera.main == null){return;}
        float cameraLeftBound = Camera.main.ViewportToWorldPoint(new(0, 0.5f, Camera.main.nearClipPlane)).x;
        float cameraRightBound = Camera.main.ViewportToWorldPoint(new(1, 0.5f, Camera.main.nearClipPlane)).x;
        foreach (EnvironmentTilePool pool in m_tilePools)
        {
            pool.Initialize(); 
        }
        m_currentSpawnX = cameraLeftBound;
        while (m_currentSpawnX < cameraRightBound)
        {
            EnvironmentTilePool pool = m_tilePools[UnityEngine.Random.Range(0, m_tilePools.Count)];
            GameObject x = pool.Get();
            x.transform.position = new (m_currentSpawnX + pool.TileBounds.extents.x, 0, 0);
            m_currentSpawnX += pool.TileBounds.size.x;
        }
    }
}

[Serializable]
public class EnvironmentTilePool : IObjectPool<GameObject>
{
    [SerializeField] EnvironmentTileSO m_tileScriptableObject;
    public Bounds TileBounds => m_tileScriptableObject.GetBounds();
    ObjectPool<GameObject> m_pool;
    
    public void Initialize()
    {
        m_pool = new(
            createFunc: () =>
            {
                GameObject obj = Object.Instantiate(m_tileScriptableObject.TilePrefab);
                return obj;
            },
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: Object.Destroy,
            defaultCapacity: 10,
            maxSize: 100
        );
    }
    
    public GameObject Get()
    {
        return m_pool.Get();
    }
    
    public PooledObject<GameObject> Get(out GameObject v)
    {
        return m_pool.Get(out v);
    }
    
    public void Release(GameObject element)
    {
        m_pool.Release(element);
    }
    
    public void Clear()
    {
        m_pool.Clear();
    }
    
    public int CountInactive => m_pool.CountInactive;
}