using UnityEngine;

public class PlayerParryState : PlayerState
{
    public PlayerParryState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        player.isParry = true;

    }

    public override void Exit()
    {
        base.Exit();
        player.isParry = false;
     
        player.isJumped = true;
    }

    public override void Update()
    {
        base.Update();
        if (InputManager.Instance.moveInput.y==0)
        {
            stateMachine.ChangeState(player.idleState);
        }
        player.SetVelocity(0, rb.velocity.y);
        if (InputManager.Instance.canAttack && player.GetComponent<PlayerStats>().mana >= 5)
        {
            InputManager.Instance.canAttack = false;
            player.GetComponent<PlayerStats>().mana -= 5;
            stateMachine.ChangeState(player.dashAttackState);
        }
        if (InputManager.Instance.canDistanceAttack&& player.GetComponent<PlayerStats>().mana >= 7)
        {
            InputManager.Instance.canDistanceAttack = false;
            AudioManager.instance.PlaySFX(30, null);
           GameObject pulse = PoolMgr.Instance.GetObj("pulse", player.transform.position, player.transform.rotation);
            pulse.GetComponent<Rigidbody2D>().velocity = new Vector2(10 * player.facingDirection, 0);
           // PoolMgr.Instance.Release(pulse, 5f);
            player.GetComponent<PlayerStats>().mana -= 7;
            //Debug.Log("use crystal");
            //player.skillManager.crystal.CanUseSkill();
        }
        if (!player.IsGroundedDetected())
        {
            stateMachine.ChangeState(player.fallState);
        }
        if ((yInput >= 0.8 && player.IsLadderDetected()))
        {

            stateMachine.ChangeState(player.climbState);
        }
    }
}