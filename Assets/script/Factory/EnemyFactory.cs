using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class EnemyFactory
{
    private static EnemyFactory instance;
    public static EnemyFactory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EnemyFactory();
            }
            return instance;
        }
    }
    public GameObject GetCharmEnemy(EnemyName type, int level, Vector2 pos)
    {

        GameObject obj =PoolMgr.Instance.GetObj(type.ToString(), pos);
           // ProxyResourceFactory.Instance.Factory.GetEnemy(type);
        obj.layer = LayerMask.NameToLayer("Player");

        Enemy enemy = obj.GetComponent<Enemy>();
        enemy.Rest();
        enemy.SetAttackLayerName("Enemy");
        enemy.GetComponentInChildren<SpriteRenderer>().color = new Color(190f / 255f, 7f / 255, 201f / 255f);

        EnemyStats stats = enemy.GetComponent<EnemyStats>();
        stats.SetLevel(level);
        obj.transform.position = pos;
       // enemy.stateMachine.Initialize(enemy.idleState);
        //enemy.stateMachine.ChangeState(enemy.idleState);
        return obj;

    }
    public GameObject GetEnemy(EnemyName type, int level, Vector2 pos)
    {

        GameObject obj = PoolMgr.Instance.GetObj(type.ToString(), pos);
        // ProxyResourceFactory.Instance.Factory.GetEnemy(type);
        Enemy enemy = obj.GetComponent<Enemy>();
        obj.layer = LayerMask.NameToLayer("Enemy");

        enemy.SetAttackLayerName("Player");
        //°×É«
        enemy.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        enemy.Rest();
        EnemyStats stats =obj.GetComponent<EnemyStats>();
       
        stats.SetLevel(level);
        obj.transform.position = pos;
       // enemy.stateMachine.Initialize(enemy.idleState);
        //enemy.stateMachine.ChangeState(enemy.idleState);
        return obj;

    }
}
