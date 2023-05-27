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
        Walk, LookAtWall
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

    private void Update()
    {
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
        }
    }
    #endregion
    
    #region State Functions
    private void WalkState()
    {
        _body.velocity = Vector2.right * (walkSpeed * _direction);
        if (wallDetect.IsBlocked)
        {
            _currentAnimationState = AnimationState.LookAtWall;
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
    #endregion
}
