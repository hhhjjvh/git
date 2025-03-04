using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundState : EnemyState
{
     protected Enemy enemy;
    //protected Transform player;
    public EnemyGroundState (Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(stateMachine, enemy, animBoolName)
    {
        this.enemy = enemy;


    }



    public override void Enter()
    {
        base.Enter();
        //player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (enemy.IsGroundedDetected() && enemy.bigHitState.isCanBigHit)
        {
            enemy.entityFX.Invoke("CancelRedBlink", 0);
            stateMachine.ChangeState(enemy.bigHitState);
        }
        // Debug.Log(AttackEntity);
        if (AttackEntity != null)
        {
            if ((enemy.CheckPlayerInFront() || Vector2.Distance(AttackEntity.transform.position, enemy.transform.position) < 2 
                || enemy.CheckPlayerInCircle())&&!AttackEntity.GetComponent<CharacterStats>().isDead)
            {
                stateMachine.ChangeState(enemy.battleState);
            }
        }
    }
}
