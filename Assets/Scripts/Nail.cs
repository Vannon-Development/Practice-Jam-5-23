using System;
using UnityEngine;

[AddComponentMenu("Escape Hammer/Nail Character")]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Nail : MonoBehaviour
{
    #region Inspector Items
    [SerializeField] private float walkSpeed;
    [SerializeField] private Transform flipControl;
    [SerializeField] private Collider2D groundCollider;
    [Header("Detection")]
    [SerializeField] private Detector wallDetect;
    [SerializeField] private Detector ledgeDetect;
    [SerializeField] private Detector dropDetect;
    #endregion
    
    #region Components
    private Animator _ani;
    private Rigidbody2D _body;
    #endregion

    #region Animation Control Items
    private enum AnimationState
    {
        Walk, LookAtWall, ClimbOnLedge, Jump, Fall
    }

    private static readonly int AniStateHash = Animator.StringToHash("State");

    private AnimationState _currentAnimationState;
    private bool _stateDone;

    private void AnimationEnded()
    {
        _stateDone = true;
    }
    #endregion
    
    #region Jump Control Items
    private bool _applyJumpVelocity;

    private void JumpStart()
    {
        _applyJumpVelocity = true;
    }
    #endregion

    #region Motion State Items
    private float _direction = 1.0f;
    #endregion

    #region Unity MonoBehavior
    private void Start()
    {
        _ani = GetComponent<Animator>();
        _body = GetComponent<Rigidbody2D>();
        _currentAnimationState = AnimationState.Walk;
        dropDetect.ForceBlock();
    }

    private void Update()
    {
        if (_stateDone)
        {
            switch (_currentAnimationState)
            {
                case AnimationState.ClimbOnLedge:
                    ClimbOnLedgeStateEnd();
                    break;
                case AnimationState.LookAtWall:
                    LookAtWallStateEnd();
                    break;
                case AnimationState.Jump:
                    JumpStateEnded();
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        _ani.SetInteger(AniStateHash, (int)_currentAnimationState);
        
        switch (_currentAnimationState)
        {
            case AnimationState.Walk:
                WalkState();
                break;
            case AnimationState.LookAtWall:
                LookAtWallState();
                break;
            case AnimationState.ClimbOnLedge:
                ClimbOnLedgeState();
                break;
            case AnimationState.Jump:
                JumpState();
                break;
            case AnimationState.Fall:
                FallState();
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!ledgeDetect.IsBlocked && wallDetect.IsBlocked)
        {
            _currentAnimationState = AnimationState.ClimbOnLedge;
        }

        if (_currentAnimationState == AnimationState.Fall)
        {
            _currentAnimationState = AnimationState.Walk;
        }
    }

    #endregion
    
    #region State Functions
    private void WalkState()
    {
        _body.velocity = new Vector2(walkSpeed * _direction, _body.velocity.y);
        _body.angularVelocity = 0;
        
        if (wallDetect.IsBlocked && ledgeDetect.IsBlocked)
        {
            _body.velocity = new Vector2(0, _body.velocity.y);
            _currentAnimationState = AnimationState.LookAtWall;
        }
        else if (!dropDetect.IsBlocked)
        {
            _body.velocity = Vector2.zero;
            _currentAnimationState = AnimationState.Jump;
        }
    }

    private void LookAtWallState()
    {
        _body.velocity = Vector2.zero;
        _body.angularVelocity = 0;
    }

    private void LookAtWallStateEnd()
    {
        _direction *= -1;
        flipControl.transform.localScale = new Vector3(_direction, 1, 1);
        _currentAnimationState = AnimationState.Walk;
        _ani.SetInteger(AniStateHash, (int)_currentAnimationState);
        wallDetect.ForceReset();
        _stateDone = false;
    }

    private void ClimbOnLedgeState()
    {
        _body.velocity = Vector2.zero;
        _body.angularVelocity = 0;
        _body.simulated = false;
        groundCollider.enabled = false;
    }

    private void ClimbOnLedgeStateEnd()
    {
        transform.localPosition += new Vector3(0.2883f * _direction, 1, 0);
        _body.simulated = true;
        groundCollider.enabled = true;
        wallDetect.ForceReset();
        _body.velocity = Vector2.zero;
        _body.angularVelocity = 0;
        _currentAnimationState = AnimationState.Walk;
        _ani.SetInteger(AniStateHash, (int)_currentAnimationState);
        _stateDone = false;
    }

    private void JumpState()
    {
        if (_applyJumpVelocity)
        {
            _body.velocity = new Vector2(0.6f / 0.5f * _direction, _body.velocity.y);
        }
    }

    private void JumpStateEnded()
    {
        _applyJumpVelocity = false;
        _body.velocity = new Vector2(0, _body.velocity.y);
        _currentAnimationState = AnimationState.Fall;
        _stateDone = false;
    }

    private void FallState()
    {
        _body.velocity = new Vector2(0, _body.velocity.y);
    }
    #endregion
}
