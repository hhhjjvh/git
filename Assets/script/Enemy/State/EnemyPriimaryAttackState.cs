using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPriimaryAttackState : EnemyAttackBase
{
    
    public EnemyPriimaryAttackState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(enemybase, stateMachine, enemy, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
       
    }

    public override void Exit()
    {
        base.Exit();
       
    }

    public override void Update()
    {
        base.Update();
        

    }
}
