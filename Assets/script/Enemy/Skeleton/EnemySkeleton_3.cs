using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkeleton_3 : Enemy_Skeleton
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        enemyName = EnemyName.Skeleton_3;
        attackState = new Skeleton_3AttackState(this, stateMachine, this, "Attack");
    }

    protected override void Update()
    {
        base.Update();
    }
    public override void Rest()
    {
        canMove = true;
        isKnocked = false;
        facingDirection = -1;
        facingRight = false;
    }
}

