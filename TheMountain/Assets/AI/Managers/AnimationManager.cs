using UnityEngine;
using System.Collections;

public class AnimationManager : MonoBehaviour
{
    Animator _anim;
    Blackboard _blackboard;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _blackboard = GetComponent<Blackboard>();
    }

    private void Update()
    {
        UpdateMoveAnimations(_blackboard.localDesiredVelocity, _blackboard.angle, _blackboard.shouldMove);
        UpdateAttackAnimations(_blackboard.attackID, _blackboard.attackType, _blackboard.shouldAttack);
        
    }
    private void UpdateMoveAnimations(Vector3 localDesiredVelocity, float angle, bool shouldMove)
    {
        _anim.SetBool("Move", shouldMove);
        _anim.SetFloat("TurnAngle", angle);
        _anim.SetFloat("Speed", localDesiredVelocity.magnitude);
    }

    private void UpdateAttackAnimations(int attackID, int attackType, bool shouldAttack)
    {
        if (shouldAttack)
        {
            _anim.SetTrigger("Attack");
        }
        if (!shouldAttack)
        {
            _anim.ResetTrigger("Attack");
        }
        _anim.SetInteger("AttackID", attackID);
        _anim.SetInteger("AttackType", attackType);
    }
}
