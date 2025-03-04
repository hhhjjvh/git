using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_4AnimationTrigger : EnemyAnimationTrigger
{
    public ArrowController ArrowPrefab;
    private   Vector2 finalDirection;
    private player1 player => GameObject.FindGameObjectWithTag("Player").GetComponent<player1>();
    new EnemySkeleton_4 enemy => GetComponentInParent<EnemySkeleton_4>();
    void Start()
    {
        //player = PlayerManager.instance.player;
        //enemy = GetComponentInParent<EnemyBringerofDeath>();
    }
    private void ChangePositionAnimationTrigger()
    {

        enemy.transform.position = new Vector3(player.transform.position.x - 4 * player.facingDirection, player.transform.position.y + 0.5f, player.transform.position.z);
        if (enemy.facingDirection != player.facingDirection)
        {
            enemy.Flip();
            enemy.healthBarUI.FlipUI();
        }

    }
    private void ArrowAnimationTrigger()
    {
       // ArrowController Arrow = Instantiate(ArrowPrefab,enemy.attacktionPoints[enemy.attackState.comboCounter].position, Quaternion.identity);
       //GameObject Arrow = PoolManager.instance.GetControllerFromPool("Arrow");
       // Arrow.transform.position = enemy.attacktionPoints[enemy.attackState.comboCounter].position;
       // Arrow.transform.rotation = Quaternion.identity;
      
        GameObject Arrow =PoolMgr.Instance.GetObj("Arrow",transform.position , Quaternion.identity);
        Arrow.transform.position = enemy.attacktionPoints[enemy.attackState.comboCounter].position;
        if(Arrow == null) return;
      
       
        finalDirection =  new Vector2(-AimDirection().x * enemy.arrowspeed * (enemy.attackState.comboCounter+1)
            , -AimDirection().y * enemy.arrowspeed*(enemy.attackState.comboCounter + 1));
        //Debug.Log(Arrow);
        //Arrow.GetComponent<ArrowController>().Setup(enemy.GetComponent<EnemyStats>(), finalDirection);
        Arrow.GetComponent<ArrowController>().Setup(enemy.GetComponent<EnemyStats>(), finalDirection);
    } 
    protected override void AnimationTrigger()
    {
        base.AnimationTrigger();
        
    }
    protected override void HitAnimationTrigger()
    {
        base.HitAnimationTrigger();
       
    }
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = closeEnemy(enemy.transform).position;
        Vector2 Position = enemy.attacktionPoints[enemy.attackState.comboCounter].position;
        return (Position - playerPosition).normalized;
    }
   
}
