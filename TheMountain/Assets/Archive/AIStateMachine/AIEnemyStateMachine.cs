using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEnemyStateMachine : AIStateMachine {

    [SerializeField] [Range(10f, 360f)] float _fov = 50f;
    [SerializeField] [Range(0f, 1f)] float _sight = 0.5f;
    [SerializeField] [Range(0f, 1f)] float _hearing = 1f;
    [SerializeField] [Range(0, 1f)] float _aggression = 0.5f;
    [SerializeField] [Range(0, 100)] int _health = 100;
    [SerializeField] [Range(0,1f)] float _intelligence = 0.5f;

    private int _seeking = 0;
    private bool _crouching = false;
    private int _attackType = 0;

    private int _magnitudeHash = Animator.StringToHash("Magnitude");
    private int _crouchingHash = Animator.StringToHash("IsCrouching");
    private int _seekingHash = Animator.StringToHash("Seeking");
    private int _attackIDHash = Animator.StringToHash("AttackID");

    public float fov { get { return _fov; } }
    public float sight { get { return _sight; } }
    public float hearing { get { return _hearing; } }
    public float agression { get { return _aggression; } set { _aggression = value; } }
    public int health { get { return _health; } set { _health = value; } }
    public float intelligence { get { return _intelligence; } }
    public int seeking { get { return _seeking; } set { _seeking = value; } }
    public bool crouching { get { return _crouching; } set { _crouching = value; } }
    public int attackType { get { return _attackType; } set { _attackType = value; } }
    public float speed
    {
        get {
            return _navAgent != null ? _navAgent.speed : 0f;

        }
        set { if (_navAgent != null) _navAgent.speed = value; }
    }

    protected override void Update()
    {
        base.Update();
        if (_animator != null)
        {
            _animator.SetFloat(_magnitudeHash, _navAgent.speed);
            _animator.SetBool(_crouchingHash, _crouching);
            _animator.SetInteger(_seekingHash, _seeking);
            _animator.SetInteger(_attackIDHash, _attackType);
        }
    }
}
