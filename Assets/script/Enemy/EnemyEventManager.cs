using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Concurrent;

public class EnemyEventManager : MonoBehaviour
{
    private static EnemyEventManager _instance;
    // 新增事件，参数为敌人类型和数量
    public static event System.Action<EnemyName, int> OnEnemyDiedBatch;
    //private Dictionary<EnemyName, int> deathCounts = new(); // 按敌人类型计数
    private ConcurrentDictionary<EnemyName, int> deathCounts = new();
    [SerializeField] private int batchSize = 10; // 累积阈值

    private static readonly Lazy<EnemyEventManager> _lazyInstance =
     new Lazy<EnemyEventManager>(() =>
     {
         var instance = new GameObject("EnemyEventManager").AddComponent<EnemyEventManager>();
         DontDestroyOnLoad(instance.gameObject);
         return instance;
     });

    public static EnemyEventManager Instance => _lazyInstance.Value;



    // 入队时直接计数
    public void EnqueueDeathEvent(Enemy enemy, EnemyName name)
    {
        if (deathCounts.ContainsKey(name))
            deathCounts[name]++;
        else
            deathCounts[name] = 1;

        int currentCount = deathCounts[name];
        if (currentCount >= batchSize)
        {
            TriggerBatchEvent(name, currentCount);
            deathCounts[name] -= currentCount; // 仅减去已触发的数量
        }
    }
    void Start()
    {
        StartCoroutine(ProcessBatchEvents());
    }

    //private void Update()
    //{
    //    if (Time.frameCount % 10 == 0&& deathCounts.Count > 0)
    //    {
    //        // 使用临时列表避免枚举器失效
    //        var keys = new List<EnemyName>(deathCounts.Keys);
    //        foreach (var name in keys)
    //        {
    //            if (deathCounts.TryGetValue(name, out int count) && count > 0)
    //            {
    //                TriggerBatchEvent(name, count);
    //                deathCounts[name] = 0;
    //            }
    //        }
    //    }
    //}
    private IEnumerator ProcessBatchEvents()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f); // 每 0.5 秒处理一次
            var keys = new List<EnemyName>(deathCounts.Keys);
            foreach (var name in keys)
            {
                if (deathCounts.TryGetValue(name, out int count) && count > 0)
                {
                    TriggerBatchEvent(name, count);
                    deathCounts[name] = 0;
                }
            }
        }
    }
    public void TriggerBatchEvent(EnemyName name, int count)
    {
        OnEnemyDiedBatch?.Invoke(name, count);
    }
}
