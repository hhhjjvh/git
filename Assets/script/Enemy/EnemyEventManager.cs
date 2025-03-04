using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Concurrent;

public class EnemyEventManager : MonoBehaviour
{
    private static EnemyEventManager _instance;
    // �����¼�������Ϊ�������ͺ�����
    public static event System.Action<EnemyName, int> OnEnemyDiedBatch;
    //private Dictionary<EnemyName, int> deathCounts = new(); // ���������ͼ���
    private ConcurrentDictionary<EnemyName, int> deathCounts = new();
    [SerializeField] private int batchSize = 10; // �ۻ���ֵ

    private static readonly Lazy<EnemyEventManager> _lazyInstance =
     new Lazy<EnemyEventManager>(() =>
     {
         var instance = new GameObject("EnemyEventManager").AddComponent<EnemyEventManager>();
         DontDestroyOnLoad(instance.gameObject);
         return instance;
     });

    public static EnemyEventManager Instance => _lazyInstance.Value;



    // ���ʱֱ�Ӽ���
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
            deathCounts[name] -= currentCount; // ����ȥ�Ѵ���������
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
    //        // ʹ����ʱ�б����ö����ʧЧ
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
            yield return new WaitForSeconds(0.5f); // ÿ 0.5 �봦��һ��
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
