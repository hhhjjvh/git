using UnityEngine;

public class EnemyBigHitState : EnemyState
{
    private float animspeed;
    private float time;
    protected Enemy enemy;
    public bool isCanBigHit { get; private set; }
    public EnemyBigHitState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(stateMachine, enemy, animBoolName)
    {
        this.enemy = enemy;


    }



    public override void Enter()
    {
        base.Enter();
        enemy.stats.MakeVulnerableFor(10);
        animspeed = enemy.anim.speed;
        time = 3;
        isCanBigHit = false;
        enemy.stats.MakeisInvincible(true);
        enemy.entityFX.InvokeRepeating("RedColorBlink", 0, enemy.knockbackDuration);
        stateTimer = 1.5f;
        // rb.=enemy.SetVelocity(-enemy.facingDirection * 5, 5);
        //rb.velocity=new Vector2(-enemy.facingDirection * enemy.knockback.x, enemy.knockback.y);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.anim.speed = animspeed;
        enemy.stats.MakeisInvincible(false);
        enemy.isHitOver = false;
        enemy.isAnimationStop = false;
        //enemy.entityFX.Invoke("CancelRedBlink", 0);
    }

    public override void Update()
    {
        base.Update();
        time -= Time.deltaTime;
        if (time < 0)
        {
            stateMachine.ChangeState(enemy.idleState);

        }


        enemy.SetVelocity(0, rb.velocity.y);
        if (stateTimer < 0)
        {
            enemy.isAnimationStop = false;
        }
        if (enemy.isHitOver)
        {
            stateMachine.ChangeState(enemy.idleState);
            enemy.isHitOver = false;

        }
        if (enemy.isAnimationStop)
        {
            enemy.anim.speed = 0;
        }
        else
        {
           enemy.anim.speed = animspeed;
        }
    }
    public void setBigHit(bool isBigHit)
    {
        this.isCanBigHit = isBigHit;
    }

}
