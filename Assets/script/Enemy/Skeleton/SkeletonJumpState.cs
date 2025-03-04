using UnityEngine;

public class SkeletonJumpState : EnemyBattleState
{
    public float facingDirection;
    protected EnemySkeleton_1 enemy;
    public SkeletonJumpState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(enemybase, stateMachine, enemy, animBoolName)
    {

        this.enemy = (EnemySkeleton_1)enemy;


    }




    public override void Enter()
    {
        base.Enter();
        rb.velocity = new Vector2(rb.velocity.x, enemy.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();
        
        if (rb.velocity.y < -0.01f)
        {
            stateMachine.ChangeState(enemy.fallState);
        }
    }
}