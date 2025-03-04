using Unity.Burst.CompilerServices;
using UnityEngine;

public class EnemyState
{
    protected Rigidbody2D rb;

    protected EnemyStateMachine stateMachine;
    protected Enemy enemyBase;

    private string animBoolName;


    protected float stateTimer;
    protected bool triggerCalled;
    protected Transform AttackEntity;
    protected int moveDirection;
    public EnemyState(EnemyStateMachine enemyStateMachine, Enemy enemyBase, string animBoolName)
    {
        stateMachine = enemyStateMachine;

        this.enemyBase = enemyBase;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        //AttackEntity = GameObject.FindGameObjectWithTag("Player").transform;

        FindAttackEnity();

        enemyBase.anim.SetBool(animBoolName, true);
        rb = enemyBase.rb;
        triggerCalled = false;
    }

    protected void FindAttackEnity()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemyBase.transform.position, 50);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        foreach (Collider2D hit in colliders)
        {
            if (hit.GetComponent<entity>() != null)
            {
                if (hit.gameObject.layer == LayerMask.NameToLayer(enemyBase.GetAttackLayerName()) && !hit.GetComponent<CharacterStats>().isDead)
                {


                    float distance = Vector2.Distance(enemyBase.transform.position, hit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = hit.transform;
                    }
                }
            }
        }
        if (closestEnemy == null&& PlayerManager.instance!= null) closestEnemy = PlayerManager.instance.player.transform;

        AttackEntity = closestEnemy;
    }

    public virtual void Exit()
    {
        enemyBase.anim.SetBool(animBoolName, false);
        enemyBase.AssigLastAnimName(animBoolName);
        FindAttackEnity();
    }
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
        if (AttackEntity==null)
        {
            AttackEntity = PlayerManager.instance.player.transform;
        }

    }
    public virtual void AnimationTrigger()
    {
        triggerCalled = true;
    }
    protected virtual Transform closeEnemy(Transform checktransform)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checktransform.position, 50f);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && !hit.GetComponent<CharacterStats>().isDead)
            {
                float distance = Vector2.Distance(checktransform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hit.transform;
                }
            }
        }
        if (closestEnemy == null)
        {
            closestEnemy = checktransform;
        }
        return closestEnemy;
    }
}
