using UnityEngine;
using System.Collections;
using Player.PlayerController;
using System;

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
        bool _lightAttack, _heavyAttack;
        float timeBetweenAttacks;
        float oldTime;
        float[] inputBuffer;
        int inputBufferCount = 0;
        int nextAttack = 0;
        [Range(0, 1)] public float inputResponseTime;

        // Use this for initialization
        protected override void Start()
        {
            inputBuffer = new float[4];

            //Starts the looping coroutine
            StartCoroutine(CheckForAttack());
            base.Start();
          
            
            
        }

        protected virtual void CreateInputBuffer()
        {


            if (Input.GetMouseButtonDown(0))
            {
                inputBuffer[inputBufferCount] = (int) Utility.AttackCategory.light;
                timeBetweenAttacks = Time.time - lastTime;
                inputBuffer[inputBufferCount + 1] = timeBetweenAttacks;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                inputBuffer[inputBufferCount] = (int) Utility.AttackCategory.heavy;
                timeBetweenAttacks = Time.time - lastTime;
                inputBuffer[inputBufferCount + 1] = timeBetweenAttacks;
            }
            inputBufferCount = (inputBufferCount >= 3) ? 0 : inputBufferCount++;
        }

        protected override void Update()
        {

            CreateInputBuffer();
            base.Update();
        }

        protected virtual void LoopAttackBuffer()
        {
            for (int i = 0; i < inputBuffer.Length; i++)
            {
                Debug.Log("The value of i is " + inputBuffer[i]);
                if (inputBuffer[i] == (int)Utility.AttackCategory.light)
                {
                    Debug.Log("This is a light attack");

                }
                if (inputBuffer[i] == (int)Utility.AttackCategory.heavy)
                {
                    Debug.Log("This is a heavy attack");
                }
                // Increment i again as we are saving the time in the next slot
                i++;
            }

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
                if (Input.GetMouseButtonDown(0))
                {
                    playerInputController.AttackID = Utility.AttackCategory.light;

                    ComboIncrease(Utility.AttackCategory.light);

                    //Combo loop that ends the combo if you reach the maxTime between attacks, or reach the end of the combo
                    while ((Time.time - lastTime) < maxTimeBetweenAttacks && playerInputController.combo < maxCombo)
                    {
                        
                        //Attacks if your cooldown has reset
                        if (Input.GetMouseButtonDown(0) && (Time.time - lastTime) > attackCooldown)
                        {
                            playerInputController.AttackID = Utility.AttackCategory.light;
                            ComboIncrease(Utility.AttackCategory.light);
                        }
                        else if (Input.GetMouseButtonDown(0) && (Time.time - lastTime) > inputResponseTime && (Time.time - lastTime) < attackCooldown)
                        {
                            playerInputController.CanLightAttackAgain = true;
                            Debug.Log("Player can attack again");

                        }
                        //Attacks if your cooldown has reset
                        if (Input.GetMouseButtonDown(1) && (Time.time - lastTime) > attackCooldown)
                        {
                            if (!(playerInputController.AttackID == Utility.AttackCategory.run))
                                playerInputController.AttackID = Utility.AttackCategory.heavy;
                            ComboIncrease(Utility.AttackCategory.heavy);
                        }
                        else if (Input.GetMouseButtonDown(1) && (Time.time - lastTime) > inputResponseTime && (Time.time - lastTime) < attackCooldown)
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
                if (Input.GetMouseButtonDown(1))
                {
                    if(!(playerInputController.AttackID == Utility.AttackCategory.run))
                        playerInputController.AttackID = Utility.AttackCategory.heavy;
                    ComboIncrease(Utility.AttackCategory.heavy);
                    //Combo loop that ends the combo if you reach the maxTime between attacks, or reach the end of the combo
                    while ((Time.time - lastTime) < maxTimeBetweenAttacks && playerInputController.combo < maxCombo)
                    {
                        //Attacks if your cooldown has reset
                        if (Input.GetMouseButtonDown(0) && (Time.time - lastTime) > attackCooldown)
                        {
                            playerInputController.AttackID = Utility.AttackCategory.light;

                            ComboIncrease(Utility.AttackCategory.light);
                        }
                        else if (Input.GetMouseButtonDown(0) && (Time.time - lastTime) > inputResponseTime && (Time.time - lastTime) < attackCooldown)
                        {
                            playerInputController.CanLightAttackAgain = true;
                            Debug.Log("Player can attack again");
                        }

                        //Attacks if your cooldown has reset
                        if (Input.GetMouseButtonDown(1) && (Time.time - lastTime) > attackCooldown)
                        {
                            if (!(playerInputController.AttackID == Utility.AttackCategory.run))
                                playerInputController.AttackID = Utility.AttackCategory.heavy;

                            ComboIncrease(Utility.AttackCategory.heavy);
                        }
                        else if (Input.GetMouseButtonDown(1) && (Time.time - lastTime) > inputResponseTime && (Time.time - lastTime) < attackCooldown)
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
            if (Input.GetMouseButtonDown(1))
            {
                playerInputController.AttackID = Utility.AttackCategory.run;
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
    }

    
}
