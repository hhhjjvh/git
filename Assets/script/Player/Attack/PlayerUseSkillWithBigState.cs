using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUseSkillWithBigState : PlayerState
{
    private float gravityScale;
    public PlayerUseSkillWithBigState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        gravityScale = player.rb.gravityScale;
        player.rb.gravityScale *= 3.5f;
        stateTimer = 5;
        player.stats.MakeisInvincible(true);

    }

    public override void Exit()
    {
        base.Exit();
        player.rb.gravityScale = 4;
        player.stats.MakeisInvincible(false);
    }

    public override void Update()
    {
        base.Update();
        if (player.IsGroundedDetected() || stateTimer <= 0)
        {
            AudioManager.instance.PlaySFX(38, null);
            player.entityFX.ScreenShake(4f, 4f);
            PoolMgr.Instance?.GetObj("SkillSpecialEffect2", player.transform.position + new Vector3(0, 1.5f, 0), player.transform.rotation);
            for (int i = 1; i < 3; i++)
            {
                Vector3 position1 = player.transform.position + new Vector3(i * 4.2f, 1.5f, 0);
                Vector3 position2 = player.transform.position + new Vector3(-i * 4.2f, 1.5f, 0);
                PoolMgr.Instance?.GetObj("SkillSpecialEffect2", position1, player.transform.rotation);
                PoolMgr.Instance?.GetObj("SkillSpecialEffect2", position2, player.transform.rotation);
            }
            stateMachine.ChangeState(player.useSkillState);
        }
    }
}
