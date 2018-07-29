using Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackControl : StateMachineBehaviour {

    public float startDamage = 0.05f;
    public float endDamage = 0.9f;
    public float allowMovementAt = 0.9f;
    public int damageMultiplier;
    public string attackName;

    public bool resetAttackTrigger;
    private bool _isActive;

    public bool debug;
    private IAttackListener mFighter;
    private bool _isAttacking;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        mFighter = animator.GetComponent<IAttackListener>();
        _isAttacking = true;
        if (mFighter != null)
        {
            mFighter.OnEnableAttack();
        }
	}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime % 1 >= startDamage && stateInfo.normalizedTime % 1 <= endDamage && !_isActive)
        {
            _isActive = true;
            if (debug)
                Debug.Log(animator.name + " attack " + attackName + "enable damage in " + System.Math.Round(stateInfo.normalizedTime % 1, 2));
        }
        else if (stateInfo.normalizedTime % 1 > endDamage && _isActive)
        {
            _isActive = false;
            if (debug)
                Debug.Log(animator.name + " attack " + attackName + "disable damage in " + System.Math.Round(stateInfo.normalizedTime % 1, 2));
        }
        if(stateInfo.normalizedTime % 1 > allowMovementAt && _isAttacking)
        {
            _isAttacking = false;
            if (mFighter != null)
            {
                mFighter.OnDisableAttack();
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_isActive)
        {
            _isActive = false;
        }
        _isAttacking = false;
        if (mFighter != null)
        {
            mFighter.OnDisableAttack();
        }
        if (mFighter != null && resetAttackTrigger)
            mFighter.ResetAttackTrigger();

        if (debug)
            Debug.Log(animator.name + " attack " + attackName + " stateExit");
    }
    

}
