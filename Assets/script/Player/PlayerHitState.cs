public class PlayerHitState : PlayerState
{

    private float animspeed;

    public PlayerHitState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        animspeed = player.anim.speed;
       // player.stats.MakeisInvincible(true);
        stateTimer = 3f;
        player.CloseCounterAttackWindow();
        AudioManager.instance.PlayPlayerHitSFX();
    }

    public override void Exit()
    {
        base.Exit();
        player.anim.speed = animspeed;
        //player.stats.MakeisInvincible(false);
    }

    public override void Update()
    {
        base.Update();
        if (player.IsGroundedDetected())
        {
            player.isAnimationStop = false;
            if (player.isHitOver)
            {
                stateMachine.ChangeState(player.idleState);
                player.isHitOver = false;
            }

        }
        if (player.isAnimationStop)
        {
            player.anim.speed = 0;
        }
        else
        {
            player.anim.speed = 1;
        }

        if (stateTimer <= 0)
        {
            stateMachine.ChangeState(player.idleState);
            player.isHitOver = false;
        }
    }
}
