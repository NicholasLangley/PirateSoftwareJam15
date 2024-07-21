using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{

    protected PlayerController _player;
    protected StateMachine _playerStateMachine;

    public State(PlayerController player, StateMachine stateMachine)
    {
        _player = player;
        _playerStateMachine = stateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }

    //public virtual void AnimationTriggerEvent(PlayerController.ANIMATION_TRIGGER_TYPE animationTriggerType) { }

    public virtual void HandleTriggerCollision(Collider2D collision) { }

}