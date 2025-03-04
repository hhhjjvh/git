using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkeleton_4 : EnemySkeleton_1
{
    public Transform[] attacktionPoints;
    public float arrowspeed;
    public float jumpChect;
    [SerializeField] protected Transform groungsqChack2;
    [SerializeField] protected Vector2 groungsqChackSize2;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        enemyName = EnemyName.Skeleton_5;
        attackState = new Skeleton_4AttackState(this, stateMachine, this, "Attack");
        battleState = new Skeleton_4BattleState(this, stateMachine, this, "Move");
        jumpState = new Skeleton_4JumpState(this, stateMachine, this, "Jump");
        fallState = new Skeleton_4FallState(this, stateMachine, this, "Fall");
    }

    protected override void Update()
    {
        base.Update();
    }
    public bool CheckSq2Ground() => Physics2D.BoxCast(groungsqChack2.position, groungsqChackSize2, 0, Vector2.zero, 0, groundLayer);
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(groungsqChack2.position, groungsqChackSize2);
    }
}

