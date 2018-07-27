using UnityEngine;
using System.Collections;
using ReGoap.Core;
using System.Collections.Generic;
using System;
using Pathfinding;

public class Blackboard : MonoBehaviour
{
    WorkingMemory _workingMemory;
    NavigationManager _navManager;
    
    public ReGoapState<string, object> worldState { get { return _workingMemory.GetWorldState();} }
    public NavigationManager navManager { get; set; }
    public Transform currentTarget { get; set; }
    public List<Transform> listOfTargets { get; set; }
    public Action onDoneMovement { get; set; }
    public Action onFailureMovement { get; set; }
    public bool targetReachedStatus { get; set; }
    public RichAI ai { get; private set; }
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
    }

    // Update is called once per frame
    void Update()
    {

    }
}
