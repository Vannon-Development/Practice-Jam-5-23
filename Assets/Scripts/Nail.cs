using System;
using BCLib;
using UnityEngine;

[AddComponentMenu("Escape Hammer/Nail Character")]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
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
    [Header("Sounds")]
    [SerializeField] private AudioClip climbSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip lookAtWallSound;
    #endregion
    
    #region Components
    private Animator _ani;
    private Rigidbody2D _body;
    private AudioSource _audio;
    #endregion

    #region Animation Control Items
    private enum AnimationState
    {
        Walk, LookAtWall, ClimbOnLedge, Jump, Fall, Land
    }

    private static readonly int AniStateHash = Animator.StringToHash("State");

    private AnimationState _currentAnimationState;
    private bool _stateDone;

    private void AnimationEnded()
    {
        _stateDone = true;
    }
    #endregion
    
    #region Audio Control Items
    private bool _allowAudioTrigger = true;
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
        _audio = GetComponent<AudioSource>();
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
                case AnimationState.Land:
                    LandStateEnded();
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
            case AnimationState.Land:
                LandState();
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        CalculateContacts(other);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        CalculateContacts(other);
    }
    #endregion
    
    #region Physics Functions

    private void CalculateContacts(Collision2D other)
    {
        var horiz = false;
        var vert = false;
        foreach (var c in other.contacts)
        {
            if (!c.normal.x.NearZero())
                horiz = true;
            if (!c.normal.y.NearZero())
                vert = true;
        }

        if (!ledgeDetect.IsBlocked && wallDetect.IsBlocked && horiz)
        {
            _currentAnimationState = AnimationState.ClimbOnLedge;
        }

        if (_currentAnimationState == AnimationState.Fall && vert)
        {
            _currentAnimationState = AnimationState.Land;
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
        if (_allowAudioTrigger)
        {
            _audio.PlayOneShot(lookAtWallSound);
            _allowAudioTrigger = false;
        }
        _body.velocity = Vector2.zero;
        _body.angularVelocity = 0;
    }

    private void LookAtWallStateEnd()
    {
        _allowAudioTrigger = true;
        _direction *= -1;
        flipControl.transform.localScale = new Vector3(_direction, 1, 1);
        _currentAnimationState = AnimationState.Walk;
        _ani.SetInteger(AniStateHash, (int)_currentAnimationState);
        wallDetect.ForceReset();
        _stateDone = false;
    }

    private void ClimbOnLedgeState()
    {
        if (_allowAudioTrigger)
        {
            _audio.PlayOneShot(climbSound);
            _allowAudioTrigger = false;
        }
        _body.velocity = Vector2.zero;
        _body.angularVelocity = 0;
        _body.simulated = false;
        groundCollider.enabled = false;
    }

    private void ClimbOnLedgeStateEnd()
    {
        _allowAudioTrigger = true;
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
        if (_allowAudioTrigger)
        {
            _audio.PlayOneShot(jumpSound);
            _allowAudioTrigger = false;
        }
        if (_applyJumpVelocity)
        {
            _body.velocity = new Vector2(0.6f / 0.5f * _direction, _body.velocity.y);
        }
    }

    private void JumpStateEnded()
    {
        _allowAudioTrigger = true;
        _applyJumpVelocity = false;
        _body.velocity = new Vector2(0, _body.velocity.y);
        _currentAnimationState = AnimationState.Fall;
        _stateDone = false;
    }

    private void FallState()
    {
        _body.velocity = new Vector2(0, _body.velocity.y);
    }

    private void LandState()
    {
        if (_allowAudioTrigger)
        {
            _audio.PlayOneShot(landSound);
            _allowAudioTrigger = false;
        }

        _body.velocity = new Vector2(0, _body.velocity.y);
    }

    private void LandStateEnded()
    {
        _allowAudioTrigger = true;
        _currentAnimationState = AnimationState.Walk;
        _stateDone = false;
    }
    #endregion
}
