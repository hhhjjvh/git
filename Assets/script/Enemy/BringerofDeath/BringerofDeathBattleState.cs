using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringerofDeathBattleState : EnemyBattleState
{
     EnemyBringerofDeath enemy;
    private float Timer;
    public BringerofDeathBattleState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(enemybase, stateMachine, enemy, animBoolName)
    {
        this.enemy = enemy as EnemyBringerofDeath;  
    }

    public override void Enter()
    {
        base.Enter();
        if(Random.Range(0, 99)<enemy.Teleportamount)
        {
            enemy.canTelepor = true;
        }
        Timer = 1;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        Timer -= Time.deltaTime;
        if (Timer < 0)
        {
            
           // Debug.Log(enemy.Teleportamount);
            Timer = 0.8f;
            if (Random.Range(0, 99) < enemy.Teleportamount)
            {
                enemy.canTelepor = true;
            }
        }
        if (enemy.canTelepor)
        {
            stateMachine.ChangeState(enemy.teleportState);
        }

        base.Update();
       

        
    }
}
