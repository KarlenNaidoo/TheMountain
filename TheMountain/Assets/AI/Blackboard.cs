using UnityEngine;
using System.Collections;
using ReGoap.Core;
using System.Collections.Generic;
using System;
using Pathfinding;
using static Player.Utility;

public class Blackboard : MonoBehaviour, IBlackboard
{
    WorkingMemory _workingMemory;
    NavigationManager _navManager;
    
    public bool isCrouching { get; set; }
    public enum AIAttackType { NoWeapon, MeleeWeapon, RangeWeapon }
    public ReGoapState<string, object> worldState { get { return _workingMemory.GetWorldState();} }
    public NavigationManager navManager { get; set; }
    public Vector3 currentTarget { get; set; }
    public List<Transform> listOfTargets { get; set; }
    public Action onDoneMovement { get; set; }
    public Action onFailureMovement { get; set; }
    public bool targetReachedStatus { get; set; }
    public RichAI ai { get; private set; }
    public Vector3 localDesiredVelocity { get; private set; }
    public float angle { get; private set; }
    public bool shouldMove { get; private set; }
    public int attackID { get; private set; }
    public int attackType { get; private set; }
    public bool shouldAttack { get; private set; }
    public float aggression { get; set; }
    public float intelligence { get; set; }
    public List<HitBoxArea> hitboxes { get; set; }
    public List<HitBox> activeHitboxComponents { get; set; }

    public Animator animator { get; set; }

    public void SetMoveParameters (Vector3 localDesiredVelocity, float angle, bool shouldMove)
    {
        this.localDesiredVelocity = localDesiredVelocity;
        this.angle = angle;
        this.shouldMove = shouldMove;
    }

    public void SetAttackParameters(bool shouldAttack)
    {
        this.shouldAttack = shouldAttack;
    }

    public void SetAttackParameters(int attackID, int attackType, bool shouldAttack)
    {
        this.shouldAttack = shouldAttack;
        this.attackID = attackID;
        this.attackType = attackType;
    }

    private void Awake()
    {
        _workingMemory = GetComponent<WorkingMemory>();
        _navManager = GetComponent<NavigationManager>();
        ai = GetComponent<RichAI>();
    }

    // Use this for initialization
    void Start()
    {
        _workingMemory.GetWorldState();
        hitboxes = new List<HitBoxArea>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
