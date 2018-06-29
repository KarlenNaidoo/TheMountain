using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;
using Player.PlayerController;

namespace Player.Melee
{
    public class MeleeStateControl : StateMachineBehaviour
    {

        //public vAttackType meleeAttackType = vAttackType.Unarmed;


        [SerializeField] bool endOfCombo;
        private bool isActive = false;
        private PlayerInputController melee;
        public int combo;
        private int oldAttack;
        private float start, end;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            melee = animator.GetComponent<PlayerInputController>();
            if (melee != null)
            {
                melee.OnEnableAttack();
            }
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (melee != null)
            {

                if (melee.CanLightAttackAgain)
                {
                    if(stateInfo.normalizedTime >= 0.7f)
                    {

                        animator.SetBool("CanLightAttackAgain", melee.CanLightAttackAgain);
                    }
                }

                if (melee.CanHeavyAttackAgain)
                {
                    if (stateInfo.normalizedTime >= 0.7f)
                    {

                        animator.SetBool("CanHeavyAttackAgain", melee.CanHeavyAttackAgain);
                    }
                }
                //if(stateInfo.normalizedTime >= 0.7f)
                //{

                    //animator.SetInteger("AttackID", (int)melee.AttackID);
                //}
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (melee != null)
            {
                melee.OnDisableAttack();
                if(endOfCombo)
                    melee.AttackID = 0;

                animator.SetBool("CanLightAttackAgain", false);
                animator.SetBool("CanHeavyAttackAgain", false);
                melee.CanHeavyAttackAgain = false;
                melee.CanLightAttackAgain = false;


            }

        }

    }
}

