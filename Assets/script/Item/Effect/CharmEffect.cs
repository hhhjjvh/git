using UnityEngine;

[CreateAssetMenu(fileName = "FreezeEnemiesEffect", menuName = "Data/Item effect/CharmEffect")]
public class CharmEffect : ItemEffect
{
    [SerializeField] private float duration;

    public override void ExecuteEffect(Transform enemyPosition)
    {

        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        EnemyStats enemy = enemyPosition.GetComponent<EnemyStats>();
        if (enemy == null) return;

        if (enemy.isDead && enemy.GetComponent<Enemy>().GetAttackLayerName() == "Player")
        {
            // Debug.Log("Enemy is dead");
            EnemyName enemyname = enemyPosition.GetComponent<Enemy>().enemyName;

           GameObject obj = EnemyFactory.Instance.GetCharmEnemy(enemyname,
           Random.Range(playerStats.GetComponent<PlayerStats>().GetLevel(), playerStats.GetComponent<PlayerStats>().GetLevel()+3), enemyPosition.transform.position);

            var bounds = GameObject.FindGameObjectWithTag("Bounds");
            if (bounds != null)
            {
                obj.transform.SetParent(bounds.transform);
            }
            obj.transform.SetParent(enemyPosition.parent);
            PoolMgr.Instance.Release(enemyPosition.gameObject);
            //Destroy(enemyPosition.gameObject);
            // Destroy(obj, duration);
            PoolMgr.Instance.Release(obj, 10f);

        }

    }
}
