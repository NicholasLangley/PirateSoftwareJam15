using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : State
{
    public PlayerIdleState(PlayerController player, StateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        _player.changeAnimation(PlayerController.PLAYER_ANIMATION.Idle);
    }

    public override void ExitState()
    {

    }

    public override void FrameUpdate()
    {
        if (Input.GetAxisRaw("Horizontal") != 0) { _playerStateMachine.changeState(_player._playerWalkingState); }
        else if (!_player._isGrounded)
        {
            _playerStateMachine.changeState(_player._playerAirborneState);
        }
        else if (Input.GetButtonDown("Jump"))
        {
            _playerStateMachine.changeState(_player._playerJumpingState);
        }
        _player.Move();
    }

   /* public override void AnimationTriggerEvent(PlayerController.ANIMATION_TRIGGER_TYPE animationTriggerType)
    {

    }*/

    public override void HandleTriggerCollision(Collider2D collision)
    {
        
    }
}