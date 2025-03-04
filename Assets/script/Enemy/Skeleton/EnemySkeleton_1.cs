using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkeleton_1 : Enemy_Skeleton
{
    public float jumpForce;

    [SerializeField]protected Transform groungsqChack;
   [SerializeField]protected Vector2 groungsqChackSize;

    public virtual SkeletonJumpState jumpState { get; protected set; }
    public virtual EnemyFallState fallState { get; protected set; }

    protected override void Awake()
    {
        base.Awake();
       
    }

    protected override void Start()
    {
        base.Start();
        enemyName = EnemyName.Skeleton_1;
        attackState = new SkeletonPriimaryAttackState(this, stateMachine, this, "Attack");
        battleState = new SkeletonBattleState(this, stateMachine, this, "Move");
        jumpState = new SkeletonJumpState(this, stateMachine, this, "Jump");
        fallState = new EnemyFallState(this, stateMachine, this, "Fall");
    }

    protected override void Update()
    {
        base.Update();
    }
    public bool CheckSqGround()=> Physics2D.BoxCast(groungsqChack.position, groungsqChackSize, 0,Vector2.zero,0 ,groundLayer);
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(groungsqChack.position, groungsqChackSize);
    }
}
