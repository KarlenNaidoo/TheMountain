using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Player.PlayerController;
using System;
using static Player.Utility;

namespace Player.PlayerController
{
    // TODO: Simplify combat inputs
    public class WeaponActions : PlayerInput, IHitboxResponder
    {
    
        HitBox _hitbox;
        HitBoxController _hitboxController;
        Animator _anim;
        bool _canCombo;
        List<AttackCategory> attackLogger;
        Dictionary<AttackCategory[], string>[] combos;
        protected virtual void Start()
        {
            _anim = GetComponent<Animator>();
            _hitbox = GetComponentInChildren<HitBox>();
            _hitboxController = GetComponent<HitBoxController>();
            attackLogger = new List<AttackCategory>();
            
        }

        protected override void Update()
        {
            base.Update();
            if (playerActions.LightAttack.IsPressed)
            {
                Debug.Log("Checking for combo");
                CheckForCombo();
                _anim.Play("LightAtk1");
                attackLogger.Add(AttackCategory.light);

            }
        }
        protected void ComboWindowStart()
        {
            Debug.Log("Start combo window");
            _canCombo = true;
            Debug.Log("Can combo: " + _canCombo);
        }

        protected void CheckForCombo()
        {
            if (playerActions.LightAttack.IsPressed && _canCombo)
            {
                Debug.Log("Player pressed light attack and can combo");
                _anim.Play("LightAtk2");
            }
            else
            {
                _anim.SetBool("InCombo", false);
            }
        }

        protected void ComboWindowEnd()
        {
            Debug.Log("End combo window");
            _canCombo = false;
        }
        
        public void CollidedWith(Collider collider)
        {
            Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
            IHealthController hurtBoxController = hurtbox.GetComponentInParent<IHealthController>(); // the parent gameobject will implement the health and damage
            Damage attackDamage = new Damage(15);
            hurtBoxController?.ReceiveDamage(attackDamage);
        }


        private void SetResponderToHitbox()
        {
            _hitbox.SetResponder(this);
        }
    }

    
}
