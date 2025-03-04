using UnityEngine;

public class Skeleton_4AttackState : EnemyPriimaryAttackState
{
    new protected EnemySkeleton_4 enemy;


    public bool canAttack;

    protected float comboWindow = 0.5f;

    public Skeleton_4AttackState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(enemybase, stateMachine, enemy, animBoolName)
    {
        this.enemy = enemy as EnemySkeleton_4;
    }

    public override void Enter()
    {
        base.Enter();
        canAttack = true;
        if (comboCounter > 1 || Time.time - enemy.lastTimeAttacked > comboWindow)
        {
            comboCounter = 0;
        }
        if (enemy.transform.position.y > AttackEntity.position.y + 2.5f && canAttack)
        {
            canAttack = false;
            comboCounter = 1;
        }
        enemy.anim.SetInteger("ComboCounter", comboCounter);
        if (comboCounter == 1)
        {

            enemy.rb.velocity = new Vector2(-2 * enemy.facingDirection, 2f);
        }
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();
        if (comboCounter == 1)
        {
            enemy.rb.velocity = new Vector2(-2 * enemy.facingDirection, 5f);
        }

        if (triggerCalled)
        {
            if (comboCounter == 1)
            {
                comboCounter = 0;
                stateMachine.ChangeState(enemy.fallState);
            }
            else
            {
                stateMachine.ChangeState(enemy.battleState);

            }
        }
    }
}
