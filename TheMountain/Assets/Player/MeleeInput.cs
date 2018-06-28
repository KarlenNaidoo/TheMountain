using UnityEngine;
using System.Collections;
using Player.PlayerController;

namespace Player.PlayerController
{

    public class MeleeInput : PlayerInput
    {
    
        //Cooldown time between attacks (in seconds)
        [Range(0,1f)] public float attackCooldown = 0.6f;

        //Max time before combo ends (in seconds)
        [Range(0, 2f)] public float maxTimeBetweenAttacks = 1.6f;
        //Max number of attacks in combo
        [Range(0, 3)] public int maxCombo = 3;
       
        //Time of last attack
        float lastTime;
        // Use this for initialization
        protected override void Start()
        {
            //Starts the looping coroutine
            StartCoroutine(CheckForAttack());

            // Need an if statement or else the base class will be called twice
            //if (!playerInputController.IsAttacking)
            //{
                base.Start();
            //}
            
            
        }

        protected override void HandleInput()
        {
            if (!playerInputController.IsAttacking)
                base.HandleInput();
        }

        protected virtual IEnumerator CheckForAttack()
        {
            //Constantly loops so you only have to call it once
            while (true)
            {
                //Checks if attacking and then starts of the combo
                if (Input.GetMouseButtonDown(0))
                {
                    ComboIncrease(Utility.AttackCategory.light);

                    //Combo loop that ends the combo if you reach the maxTime between attacks, or reach the end of the combo
                    while ((Time.time - lastTime) < maxTimeBetweenAttacks && playerInputController.combo < maxCombo)
                    {
                        //Attacks if your cooldown has reset
                        if ((Input.GetMouseButtonDown(0)) && (Time.time - lastTime) > attackCooldown)
                        {
                            ComboIncrease(Utility.AttackCategory.light);
                        }
                        //Attacks if your cooldown has reset
                        if ((Input.GetMouseButtonDown(1)) && (Time.time - lastTime) > attackCooldown)
                        {
                            ComboIncrease(Utility.AttackCategory.heavy);
                        }
                        yield return null;
                    }
                    //Resets combo and waits the remaining amount of cooldown time before you can attack again to restart the combo
                    playerInputController.combo = 0;
                    yield return new WaitForSeconds(attackCooldown - (Time.time - lastTime));
                }
                //Checks if attacking and then starts of the combo
                if (Input.GetMouseButtonDown(1))
                {
                    ComboIncrease(Utility.AttackCategory.heavy);
                    //Combo loop that ends the combo if you reach the maxTime between attacks, or reach the end of the combo
                    while ((Time.time - lastTime) < maxTimeBetweenAttacks && playerInputController.combo < maxCombo)
                    {
                        //Attacks if your cooldown has reset
                        if (Input.GetMouseButtonDown(0) && (Time.time - lastTime) > attackCooldown)
                        {
                            ComboIncrease(Utility.AttackCategory.light);
                        }

                        //Attacks if your cooldown has reset
                        if (Input.GetMouseButtonDown(1) && (Time.time - lastTime) > attackCooldown)
                        {
                            ComboIncrease(Utility.AttackCategory.heavy);
                        }
                        yield return null;
                    }
                    //Resets combo and waits the remaining amount of cooldown time before you can attack again to restart the combo
                    playerInputController.combo = 0;
                    yield return new WaitForSeconds(attackCooldown - (Time.time - lastTime));
                }
                yield return null;
            }
        }

        private void ComboIncrease(Utility.AttackCategory attackType)
        {
            if(playerInputController.combo == 3)
            {
                playerInputController.combo = 1;
            }
            else
            {
                playerInputController.combo++;
            }
            lastTime = Time.time;
            Debug.Log("Attack " + playerInputController.combo);
            if (attackType == Utility.AttackCategory.light)
            {

                playerInputController.LightAttack();
            }
            else
            {
                playerInputController.HeavyAttack();
            }
        }
    }

    
}
