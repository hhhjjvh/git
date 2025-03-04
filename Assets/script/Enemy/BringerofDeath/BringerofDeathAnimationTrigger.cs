
using UnityEngine;



public class BringerofDeathAnimationTrigger : EnemyAnimationTrigger
{
    //public CastAnimationTrigger CastPrefab;
    private player1 player => GameObject.FindGameObjectWithTag("Player").GetComponent<player1>();
    new EnemyBringerofDeath enemy=> GetComponentInParent<EnemyBringerofDeath>();
    void Start()
    {
        //player = PlayerManager.instance.player;
        //enemy = GetComponentInParent<EnemyBringerofDeath>();
    }
    private void ChangePositionAnimationTrigger()
    {
        Transform transform = closeEnemy(enemy.transform);
        int x = 1;
        if ( transform.GetComponent<entity>().IsBehindWallDetected())
        {
            x = -1;
        }

        enemy.transform.position = new Vector3(transform.position.x-4*x*transform.GetComponent<entity>().facingDirection,
            transform.position.y+0.5f, transform.position.z);
       if( enemy.facingDirection != transform.GetComponent<entity>().facingDirection)
        {
            enemy.SetVelocity(1f * player.facingDirection, enemy.rb.velocity.y);
           
        }
       
    }
    private void CastAnimationTrigger()
    {
        Transform transform = closeEnemy(enemy.transform);
        // CastAnimationTrigger Cast = Instantiate(CastPrefab,new Vector2(transform.position.x, transform.position.y+2f), Quaternion.identity);
        //GameObject Cast = PoolManager.instance.GetControllerFromPool("BringerofDeathMagicAttack");
        //Cast.transform.position = new Vector2(transform.position.x, transform.position.y + 2f);
        GameObject Cast = PoolMgr.Instance.GetObj("BringerofDeathMagicAttack", new Vector2(transform.position.x, transform.position.y + 2f), Quaternion.identity);
        Cast.GetComponent<CastAnimationTrigger>().Setup(enemy.GetComponent<CharacterStats>());
        
    }
    protected override void AnimationTrigger()
    {
        base.AnimationTrigger();
        enemy.Teleportamount += 5;
        enemy.castamount += 1;
       
    }
    protected override void HitAnimationTrigger()
    {
        base.HitAnimationTrigger();
        enemy.Teleportamount += 3;
        enemy.castamount += 1;
    
    }

}
