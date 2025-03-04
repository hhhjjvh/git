using UnityEngine;

public class EnemyBattleState : EnemyState
{
    Enemy enemy;

    public EnemyBattleState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(stateMachine, enemy, animBoolName)
    {
        this.enemy = enemy;


    }



    public override void Enter()
    {
        base.Enter();

        //enemy.moveSpeed *= 3;

    }

    public override void Exit()
    {
        base.Exit();
       // AttackEntity = null;
        //enemy.moveSpeed /= 3;
    }

    public override void Update()
    {
        base.Update();
        if (AttackEntity == null) return;
        if (AttackEntity.GetComponent<CharacterStats>().isDead||AttackEntity.gameObject.activeSelf==false)
        {
            //Debug.Log("目标死亡");
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        if (AttackEntity.position.x < enemy.transform.position.x)
        {
            moveDirection = -1;
        }
        else if (AttackEntity.position.x > enemy.transform.position.x)
        {
            moveDirection = 1;
        }

        float rand = Random.Range(0.7f, 1.3f);
        if (enemy.CheckPlayerInFront())
        {
            stateTimer = enemy.battleTime;
            if (enemy.CheckPlayerInFront().distance < enemy.attackDistance * .9f)
            {
                enemy.SetVelocity(0, rb.velocity.y);
                if (CanAttack())
                {
                    stateMachine.ChangeState(enemy.attackState);
                }
            }

        }
        else
        {
            if (Vector2.Distance(AttackEntity.transform.position, enemy.transform.position) > enemy.chaseDistance)
            {
                stateMachine.ChangeState(enemy.idleState);
            }
        }

        if (enemy.CheckPlayerInFront() && enemy.CheckPlayerInFront().distance < enemy.attackDistance * .9f) return;
        //绝对值
        if (AttackEntity.transform.position.x- enemy.transform.position.x>.5f|| AttackEntity.transform.position.x - enemy.transform.position.x <-0.5f)
        {
            if (Vector2.Distance(AttackEntity.transform.position, enemy.transform.position) < enemy.attackDistance * .1f)
            {
                enemy.rb.velocity = new Vector2(enemy.moveSpeed * -moveDirection * rand, rb.velocity.y);
                //enemy.SetVelocity(enemy.moveSpeed * -moveDirection * rand, rb.velocity.y);
            }
            else if (Vector2.Distance(AttackEntity.transform.position, enemy.transform.position) > enemy.attackDistance * .8f )
            {
                enemy.SetVelocity(enemy.moveSpeed * moveDirection * rand, rb.velocity.y);
            }
            else
            {
                enemy.SetVelocity(enemy.moveSpeed * moveDirection * rand*0.3f, rb.velocity.y);
            }
        }
        else
        {
            enemy.SetVelocity(0, rb.velocity.y);
        }

        if (enemy.IsGroundedDetected() && enemy.bigHitState.isCanBigHit)
        {
            enemy.entityFX.Invoke("CancelRedBlink", 0);
            stateMachine.ChangeState(enemy.bigHitState);
        }
    }
    protected bool CanAttack()
    {
        if (Time.time >= enemy.lastTimeAttacked + enemy.attackCooldown)
        {
            return true;
        }
        return false;
    }
}
