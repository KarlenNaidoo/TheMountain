using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotionConfigurator : StateMachineBehaviour
{
    [SerializeField] private int _rootPosition = 0;
    [SerializeField] private int _rootRotation = 0;

    protected NavigationManager _navManager;
    public NavigationManager navManager { set { _navManager = value; } }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_navManager)
        {
            
            _navManager.AddRootMotionRequest(_rootPosition, _rootRotation);
            //Debug.Log("Root rotation enter: " + _rootRotation);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_navManager)
        {

            _navManager.AddRootMotionRequest(-_rootPosition, -_rootRotation);
            //Debug.Log("Root rotation exit: " + _rootRotation);
        }
    }
}
