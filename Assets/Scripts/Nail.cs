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
        Walk, LookAtWall, ClimbOnLedge,
    }

    private static readonly int AniStateHash = Animator.StringToHash("State");

    private AnimationState _currentAnimationState;
    private Vector2 _velocityBoost;
    private bool _stateDone;

    private void AnimationEnded()
    {
        _stateDone = true;
    }

    private void VelocityBoost(float amount)
    {
        _velocityBoost = Vector2.right * amount;
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
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!ledgeDetect.IsBlocked && wallDetect.IsBlocked)
        {
            _currentAnimationState = AnimationState.ClimbOnLedge;
        }
    }

    #endregion
    
    #region State Functions
    private void WalkState()
    {
        _body.velocity = new Vector2(walkSpeed * _direction, _body.velocity.y);
        _body.angularVelocity = 0;
        if (wallDetect.IsBlocked)
        {
            if (ledgeDetect.IsBlocked)
            {
                _currentAnimationState = AnimationState.LookAtWall;
            }
        }
    }

    private void LookAtWallState()
    {
        if (_stateDone)
        {
            _direction *= -1;
            flipControl.transform.localScale = new Vector3(_direction, 1, 1);
            _currentAnimationState = AnimationState.Walk;
            _stateDone = false;
        }
    }

    private void ClimbOnLedgeState()
    {
        _body.simulated = false;
        groundCollider.enabled = false;
        if (_stateDone)
        {
            _stateDone = false;
            transform.localPosition += new Vector3(0.4883f * _direction, 1, 0);
            _body.simulated = true;
            groundCollider.enabled = true;
            _body.velocity = Vector2.zero;
            _body.angularVelocity = 0;
            _currentAnimationState = AnimationState.Walk;
        }
    }
    #endregion
}
