using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotionConfigurator : StateMachineBehaviour
{
    [SerializeField] private int _rootPosition = 0;
    [SerializeField] private int _rootRotation = 0;

    protected SmsGoTo _smsGoTo;
    public SmsGoTo smsGoTo { set { _smsGoTo = value; } }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_smsGoTo)
        {
            
            _smsGoTo.AddRootMotionRequest(_rootPosition, _rootRotation);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_smsGoTo)
            _smsGoTo.AddRootMotionRequest(-_rootPosition, -_rootRotation);
    }
}
