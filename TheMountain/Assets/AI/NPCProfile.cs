using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Blackboard))]
public class NPCProfile : Character {

    [SerializeField][Range(0,10)] float _aggression;
    [SerializeField] [Range(0, 10)] float _intelligence;
    Blackboard _blackboard;

    private void Awake()
    {
        _blackboard = GetComponent<Blackboard>();
    }

    private void Start()
    {
        _blackboard.aggression = _aggression;
        _blackboard.intelligence = _intelligence;
    }

}
