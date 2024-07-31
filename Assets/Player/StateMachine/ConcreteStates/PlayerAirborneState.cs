using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirborneState : State
{
    public PlayerAirborneState(PlayerController player, StateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {

    }

    public override void ExitState()
    {
        _player.UnpauseAnimation();
    }

    public override void FrameUpdate()
    {
        //coyote time
        _player._coyoteTimer += Time.deltaTime;
        if (Input.GetButtonDown("Jump") && !_player._isJumping && _player._coyoteTimer <= _player._coyoteTime) { _playerStateMachine.changeState(_player._playerJumpingState); return; }

        if (_player._currentYSpeed <= 0f)
        {
            _player.changeAnimation(PlayerController.PLAYER_ANIMATION.Falling);
            _player._isJumping = false;
        }
        else 
        {
            _player.CheckForHeadbonk();
            /* _player.changeAnimation(PlayerController.PLAYER_ANIMATION.Jumping); */
        }

        if (_player._currentYSpeed == 0 && _player._isGrounded)
        {
            _playerStateMachine.changeState(_player._currentXSpeed != 0 ? _player._playerWalkingState : _player._playerIdleState);
        }
        else { AirControl(); }
    }

   /* public override void AnimationTriggerEvent(PlayerController.ANIMATION_TRIGGER_TYPE animationTriggerType)
    {

    }*/

    public override void HandleTriggerCollision(Collider2D collision)
    {
        
    }

    private void AirControl()
    {
        float accelertionAmount = _player._xAcceleration * _player._airAccelrationFactor * Time.deltaTime;
        float horizontalAccel = Input.GetAxisRaw("Horizontal") * accelertionAmount;
        //accelerate
        if (horizontalAccel != 0 && !_player.isDead) { _player._currentXSpeed = Mathf.Clamp(_player._currentXSpeed += horizontalAccel, -_player._maxXSpeed, _player._maxXSpeed); }

        _player.checkDirectionToFace(horizontalAccel);
        _player.Move();
    }
}