using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCroushState : PlayerState
{
    private Vector2 colliderOffset;
    private Vector2 colliderSize;
 
    public PlayerCroushState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
      

        colliderOffset = player.cd.offset;
        colliderSize = player.cd.size;

        player.cd.offset = new Vector2(0.05f, -0.8f);
        player.cd.size = new Vector2(0.99f, 0.99f);

    }

    public override void Exit()
    {
        base.Exit();
      
        player.cd.offset = colliderOffset;
        player.cd.size = colliderSize;
      //  CameraControl.Instance.confiner.enabled = false;
        Vector3 pos = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
        //CameraControl.Instance.transform.position;
        player.gameObject.layer = LayerMask.NameToLayer("Player");
        CameraControl.Instance.SwitchLayer(pos);
        //MonoBehaviour.StartCoroutine(returnLayer(0.5f));

    }

    public override void Update()
    {
        base.Update();
        player.SetVelocity(0, rb.velocity.y);
        if (InputManager.Instance.moveInput.y == 0&&!player.IsWallDetected3())
        {
            stateMachine.ChangeState(player.idleState);
        }
        if (InputManager.Instance.canJump && player.IsOneWayPlatformDetected())
        {
            InputManager.Instance.canJump = false;
            // stateMachine.ChangeState(player.idleState);
          //  CameraControl.Instance.confiner.enabled = false;
            Vector3 pos = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
            //CameraControl.Instance.transform.position;
            player.gameObject.layer = LayerMask.NameToLayer("Deadly");
            CameraControl.Instance.SwitchLayer(pos);
            stateTimer = 0.5f;
        }
        if (!player.IsGroundedDetected()&&stateTimer<=0)
        {
            stateMachine.ChangeState(player.fallState);
        }
        if (InputManager.Instance.canJump && player.IsGroundedDetected())
        {
            InputManager.Instance.canJump = false;
            stateMachine.ChangeState(player.slideState);
        }
        if (InputManager.Instance.canAttack && player.GetComponent<PlayerStats>().mana >= 5)
        {
            InputManager.Instance.canAttack = false;
            player.GetComponent<PlayerStats>().mana -= 5;
            stateMachine.ChangeState(player.dashAttackState);
        }
        if (InputManager.Instance.canDistanceAttack && player.GetComponent<PlayerStats>().mana >= 7)
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
        if ((yInput <= -0.8 && player.IsLadderDetected2()))
        {

            stateMachine.ChangeState(player.climbState);
        }
    }
  
}
