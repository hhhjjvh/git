using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Task/Conditions/KillCondition")]
public class KillCondition : TaskCondition
{
   public EnemyName enemyName;

    public override void RegisterListeners()
    {
        EnemyEventManager.OnEnemyDiedBatch += HandleEnemyDeathBatch; // 订阅新事件
    }

    public override void UnregisterListeners()
    {
        EnemyEventManager.OnEnemyDiedBatch -= HandleEnemyDeathBatch;
    }

    // 批量处理敌人死亡
    private void HandleEnemyDeathBatch(EnemyName enemyName, int count)
    {
        if (enemyName == this.enemyName)
        {
            currentAmount += count; // 一次性增加数量
            
            OnProgressUpdated?.Invoke();
        }
    }

    

    public override bool IsConditionMet() => currentAmount >= requiredAmount;
}