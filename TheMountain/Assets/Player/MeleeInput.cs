using UnityEngine;
using System.Collections;
using Player.PlayerController;
using System;

namespace Player.PlayerController
{
    // TODO: Simplify combat inputs
    public class MeleeInput : PlayerInput, IHitboxResponder
    {
    
        //Cooldown time between attacks (in seconds)
        [Range(0,1f)] public float attackCooldown = 0.6f;
        
        //Max time before combo ends (in seconds)
        [Range(0, 2f)] public float maxTimeBetweenAttacks = 1.6f;
        //Max number of attacks in combo
        [Range(0, 3)] public int maxCombo = 3;
        //Time of last attack
        float lastTime;
        bool _lightAttack, _heavyAttack;
        float timeBetweenAttacks;
        float oldTime;
        float[] inputBuffer;
        int inputBufferCount = 0;
        int nextAttack = 0;
        [Range(0, 1)] public float inputResponseTime;
        HitBox hitbox;
        AnimEvents animEvents;
        // Use this for initialization
        protected override void Start()
        {
            inputBuffer = new float[4];
            hitbox = GetComponentInChildren<HitBox>();
            Debug.Log(hitbox.gameObject.name + " hitbox being used. Make sure it is the right one");
            animEvents = GetComponent<AnimEvents>();
            //Starts the looping coroutine
            StartCoroutine(CheckForAttack());
            base.Start();
          
            
            
        }
        
        
        protected virtual IEnumerator CheckForAttack()
        {

            //Constantly loops so you only have to call it once
            while (true)
            {
                
                if (playerInputController != null && (playerInputController.IsSprinting || playerInputController.IsRunning))
                {
                    CheckForRunningAttack();
                }
                
                //Checks if attacking and then starts of the combo
                if (playerActions.LightAttack.IsPressed)
                {
                    Attack();
                    //Debug.Log("Attack pressed");
                    if (!(playerInputController.AttackID == Utility.AttackCategory.running_heavy
                        || playerInputController.AttackID == Utility.AttackCategory.running_light))
                        playerInputController.AttackID = Utility.AttackCategory.light;

                    ComboIncrease(Utility.AttackCategory.light);

                    //Combo loop that ends the combo if you reach the maxTime between attacks, or reach the end of the combo
                    while ((Time.time - lastTime) < maxTimeBetweenAttacks && playerInputController.combo < maxCombo)
                    {

                        //Attacks if your cooldown has reset
                        if (playerActions.LightAttack.IsPressed && (Time.time - lastTime) > attackCooldown)
                        {
                            if (!(playerInputController.AttackID == Utility.AttackCategory.running_heavy 
                                || playerInputController.AttackID == Utility.AttackCategory.running_light))
                                playerInputController.AttackID = Utility.AttackCategory.light;
                            ComboIncrease(Utility.AttackCategory.light);
                        }
                        else if (playerActions.LightAttack.IsPressed && (Time.time - lastTime) > inputResponseTime && (Time.time - lastTime) < attackCooldown)
                        {
                            playerInputController.CanLightAttackAgain = true;
                            //Debug.Log("Player can attack again");

                        }
                        //Attacks if your cooldown has reset
                        if (playerActions.HeavyAttack.IsPressed && (Time.time - lastTime) > attackCooldown)
                        {
                            if (!(playerInputController.AttackID == Utility.AttackCategory.running_heavy 
                                || playerInputController.AttackID == Utility.AttackCategory.running_light))
                                playerInputController.AttackID = Utility.AttackCategory.heavy;
                            ComboIncrease(Utility.AttackCategory.heavy);
                        }
                        else if (playerActions.HeavyAttack.IsPressed && (Time.time - lastTime) > inputResponseTime && (Time.time - lastTime) < attackCooldown)
                        {
                            playerInputController.CanHeavyAttackAgain = true;
                            Debug.Log("Player can attack again");

                        }
                        yield return null;
                    }
                    //Resets combo and waits the remaining amount of cooldown time before you can attack again to restart the combo
                    playerInputController.combo = 0;
                    playerInputController.AttackID = Utility.AttackCategory.none;
                    yield return new WaitForSeconds(attackCooldown - (Time.time - lastTime));
                }
                //Checks if attacking and then starts of the combo
                if (playerActions.HeavyAttack.IsPressed)
                {
                    if (!(playerInputController.AttackID == Utility.AttackCategory.running_heavy 
                        || playerInputController.AttackID == Utility.AttackCategory.running_light))
                        playerInputController.AttackID = Utility.AttackCategory.heavy;
                    ComboIncrease(Utility.AttackCategory.heavy);
                    //Combo loop that ends the combo if you reach the maxTime between attacks, or reach the end of the combo
                    while ((Time.time - lastTime) < maxTimeBetweenAttacks && playerInputController.combo < maxCombo)
                    {
                        //Attacks if your cooldown has reset
                        if (playerActions.LightAttack.IsPressed && (Time.time - lastTime) > attackCooldown)
                        {
                            if (!(playerInputController.AttackID == Utility.AttackCategory.running_heavy 
                                || playerInputController.AttackID == Utility.AttackCategory.running_light))
                                playerInputController.AttackID = Utility.AttackCategory.light;

                            ComboIncrease(Utility.AttackCategory.light);
                        }
                        else if (playerActions.LightAttack.IsPressed && (Time.time - lastTime) > inputResponseTime && (Time.time - lastTime) < attackCooldown)
                        {
                            playerInputController.CanLightAttackAgain = true;
                            Debug.Log("Player can attack again");
                        }

                        //Attacks if your cooldown has reset
                        if (playerActions.HeavyAttack.IsPressed && (Time.time - lastTime) > attackCooldown)
                        {
                            if (!(playerInputController.AttackID == Utility.AttackCategory.running_heavy 
                                || playerInputController.AttackID == Utility.AttackCategory.running_light))
                                playerInputController.AttackID = Utility.AttackCategory.heavy;

                            ComboIncrease(Utility.AttackCategory.heavy);
                        }
                        else if (playerActions.HeavyAttack.IsPressed && (Time.time - lastTime) > inputResponseTime && (Time.time - lastTime) < attackCooldown)
                        {
                            playerInputController.CanHeavyAttackAgain = true;
                            Debug.Log("Player can attack again");

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


        private void CheckForRunningAttack()
        {
            if (playerActions.LightAttack.IsPressed)
            {
                playerInputController.AttackID = Utility.AttackCategory.running_light;
            }
            if (playerActions.HeavyAttack.IsPressed)
            {
                playerInputController.AttackID = Utility.AttackCategory.running_heavy;
            }
        }

        private void ComboIncrease(Utility.AttackCategory attackType)
        {
            if(playerInputController.combo == 3) //TODO: get rid of magic numbers
            {
                playerInputController.combo = 1;
            }
            else
            {
                playerInputController.combo++;
            }
            lastTime = Time.time;
            if (attackType == Utility.AttackCategory.light)
            {

                playerInputController.LightAttack();
            }
            else
            {
                playerInputController.HeavyAttack();
            }
        }

        public void CollidedWith(Collider collider)
        {
            Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
            if (animEvents.OpenHitBox())
            {
                Damage attackDamage = new Damage(15);
                hurtbox?.ReceiveDamage(attackDamage);
            }
        }


        private void Attack()
        {
            hitbox.SetResponder(this);
        }
    }

    
}
