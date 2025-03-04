using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkeleton_2 : EnemySkeleton_1
{
    public float parryDistance = 2;
    public EnemyParryState parryState {  get; private set; }
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        enemyName = EnemyName.Skeleton_2;
        parryState = new EnemyParryState(this, stateMachine, this, "Parry");
        battleState = new Skeleton_2BattleState(this, stateMachine, this, "Move");
        hitState=new Skeleton_2HitState(this, stateMachine, this, "Hit");
    }

    protected override void Update()
    {
        base.Update();
    }
}

