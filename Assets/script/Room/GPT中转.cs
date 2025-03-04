using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolMgr2 : SingletonMonoGlobal<PoolMgr>
{
    // <summary>
    // 一个类型对应一个对象池  顺序是预制体顺序
    // </summary>
    public Dictionary<string, ObjectPool<GameObject>> poolDic = new();
    public Dictionary<string, Transform> parents = new();
    public Dictionary<string, PoolData> prePoolDic = new();
    //引用类型
    private Dictionary<Type, object> objPoolDic = new();

    // <summary>
    // 保存预设体 方便复用
    // </summary>
    private Dictionary<string, GameObject> prefabs = new();


    // <summary>
    //  用于预创建对象,将自动扩容，事实上音乐播放不需要特殊处理,  所以废弃
    // </summary>
    // <param name="objName"></param>
    // <param name="size"></param>
    // public void CreatePool(string objName, int size = 10)
    // {
    //     GameObject prefab = GetPrefab(objName);

    //     PoolData pool;
    //     if (prePoolDic.ContainsKey(objName))
    //     {
    //         pool = prePoolDic[objName];
    //     }
    //     else
    //     {
    //         pool = new PoolData(prefab, gameObject);
    //         prePoolDic.Add(objName, pool);
    //     }

    //     for (int i = 0; i < size; i++)
    //     {
    //         GameObject newObj = Instantiate(prefab);
    //         newObj.name = objName;
    //         pool.Push(newObj);
    //     }

    //     return;
    // }


    // <summary>
    //  获取对象
    // </summary>
    public GameObject GetObj(string objName, Vector3 position = default, Quaternion rotation = default, Action<GameObject> preCallback = null)
    {
        // 对象池不存在则创建
        if (poolDic.ContainsKey(objName) && poolDic[objName].CountAll > 0)
        {
            //Debug.Log("GetObj:" + objName);

            //池子有东西，可拿  
            GameObject result = poolDic[objName].Get();

            result.transform.SetPositionAndRotation(position, rotation);

            // Debug.Log("GetObj:" + objName);
            return result;
        }
        else
        {
            // Debug.Log("GetObj:" + objName + " 没有对象池");
            //池子没有东西，创建
            GameObject prefab = GetPrefab(objName);
            // Debug.Log("GetPrefab:" + objName);

            if (position != default) prefab.transform.position = position;
            if (rotation != default) prefab.transform.rotation = rotation;

            var newPool = CreatePool(prefab, objName);
            poolDic.Add(objName, newPool);
            return newPool.Get();
        }
    }
    private ObjectPool<GameObject> CreatePool(GameObject prefab, string objName)
    {
        Transform parent = new GameObject(prefab.name).transform;
        parent.SetParent(transform);
        parents.Add(objName, parent);
        var newPool = new ObjectPool<GameObject>(
            () =>
            {
                var newObj = Instantiate(prefab, parent);
                newObj.name = objName;
                return newObj;
            },
            e =>
            {
                e.SetActive(true);
            },
            e => e.SetActive(false),
            e => Destroy(e)
        // defaultCapacity: 10
        );
        return newPool;
    }
    private ObjectPool<GameObject> CreateOrGetPool(string objName, GameObject prefab, int initialCapacity = 10, int maxCapacity = 100, int preloadCount = 0)
    {
        if (poolDic.ContainsKey(objName))
        {
            return poolDic[objName];
        }

        // 创建父对象容器
        Transform parent = new GameObject($"Pool_{objName}").transform;
        parent.SetParent(transform);
        parents[objName] = parent;

        // 创建对象池
        var newPool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                var newObj = Instantiate(prefab, parent);
                newObj.name = objName;
                return newObj;
            },
            actionOnGet: obj =>
            {
                obj.SetActive(true);
            },
            actionOnRelease: obj =>
            {
                obj.SetActive(false);
                obj.transform.SetParent(parent);
            },
            actionOnDestroy: obj =>
            {
                Destroy(obj);
            },
            defaultCapacity: initialCapacity,
            maxSize: maxCapacity
        );

        // 预加载指定数量的对象
        for (int i = 0; i < preloadCount; i++)
        {
            var obj = newPool.Get();
            newPool.Release(obj);
        }

        poolDic.Add(objName, newPool);
        return newPool;
    }
    public void Preload(string objName, int preloadCount, int initialCapacity = 10, int maxCapacity = 100)
    {
        // 确保预制体已加载
        var prefab = GetPrefab(objName);
        if (prefab == null)
        {
            Debug.LogError($"Prefab {objName} not found.");
            return;
        }

        // 创建或获取池并预加载
        CreateOrGetPool(objName, prefab, initialCapacity, maxCapacity, preloadCount);
    }


    private GameObject GetPrefab(string objName)
    {
        GameObject prefab = null;
        //如果已经加载过该预设体
        if (prefabs.ContainsKey(objName))
        {
            prefab = prefabs[objName];
        }
        else     //如果没有加载过该预设体
        {
            //加载预设体
            // prefab = Resources.Load<GameObject>(objName);
            try
            {
                prefab = AAMgr.LoadAsset<GameObject>(objName);
                prefab.name = objName;
                //更新字典
                prefabs.Add(objName, prefab);
                // prefab.SetActive(false);
                prefab.gameObject?.SetActive(false);

            }
            catch
            {
                Debug.LogError("加载预设体失败：" + objName);
            }
        }
        return prefab;
    }

    #region  引用类型对象池
    // <summary>
    // 回收直接用clear
    // </summary>
    // <typeparam name="T"></typeparam>
    // <returns></returns>
    // public T GetObj<T>() where T : class
    // {
    //     if (objPoolDic.ContainsKey(typeof(T)))
    //     {
    //         Debug.Log("对象数量" + ((ObjectPool<T>)objPoolDic[typeof(T)]).CountAll);

    //         return ((ObjectPool<T>)objPoolDic[typeof(T)]).Get();
    //     }
    //     else
    //     {
    //         var newPool = CreateObjPool<T>();
    //         objPoolDic.Add(typeof(T), newPool);
    //         return newPool.Get() as T;
    //     }
    // }
    #endregion


    #region 销毁与获取
    public void Clear()
    {
        foreach (var pool in poolDic)
        {
            pool.Value.Clear();
        }

        foreach (var pool in prePoolDic)
        {
            pool.Value.Clear();
        }

        // foreach (var pool in objPoolDic)
        // {
        //     var clearMethod = pool.GetType().GetMethod("Clear");
        //     clearMethod?.Invoke(pool, null);
        // }
        poolDic.Clear();
        prePoolDic.Clear();
        // objPoolDic.Clear();
    }

    public void Release(GameObject obj)
    {
        if (poolDic.ContainsKey(obj.name))
        {
            //Debug.Log("对象数量" + poolDic[obj.name].CountAll);
            if (obj.activeSelf == false)
            {
                string objName = obj.name;
                Debug.LogWarning($"对象'{objName}' 已经回收.");
                return;
            }
            //Transform parent = new GameObject(obj.name).transform;
            //parent.SetParent(transform);
            obj.transform.SetParent(parents[obj.name]);
            poolDic[obj.name].Release(obj);
        }
        else if (prePoolDic.ContainsKey(obj.name))
        {
            //Debug.Log("对象数量2213" + prePoolDic[obj.name]);
            //Transform parent = new GameObject(obj.name).transform;
            //parent.SetParent(transform);
            prePoolDic[obj.name].Release(obj);
        }
        else
        {
            Destroy(obj);
        }
    }
    // private IEnumerator ReleaseAsync(ObjectPool<GameObject> objPool, GameObject obj)
    // {
    //     yield return new WaitForSeconds(1.5f);
    //     objPool.Release(obj);
    // }

    private ObjectPool<T> CreateObjPool<T>() where T : class
    {
        var newPool = new ObjectPool<T>(
            () => Activator.CreateInstance<T>(),
            null,
            null
        );
        return newPool;
    }


    #endregion
}

