using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Task/Conditions/KillCondition")]
public class KillCondition : TaskCondition
{
   public EnemyName enemyName;

    public override void RegisterListeners()
    {
        EnemyEventManager.OnEnemyDiedBatch += HandleEnemyDeathBatch; // �������¼�
    }

    public override void UnregisterListeners()
    {
        EnemyEventManager.OnEnemyDiedBatch -= HandleEnemyDeathBatch;
    }

    // ���������������
    private void HandleEnemyDeathBatch(EnemyName enemyName, int count)
    {
        if (enemyName == this.enemyName)
        {
            currentAmount += count; // һ������������
            
            OnProgressUpdated?.Invoke();
        }
    }

    

    public override bool IsConditionMet() => currentAmount >= requiredAmount;
}