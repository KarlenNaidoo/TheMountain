﻿using UnityEngine;
using System.Collections.Generic;
using static Player.Utility;
using Player.PlayerController;

public class ControllerActionManager : PlayerInput
{ 
    [SerializeField] List<WeaponAction> actionSlots = new List<WeaponAction>();
    const int CONTROLLER_INPUT_BUTTONS = 4;

    protected ControllerActionManager()
    {
        for (int i = 0; i < CONTROLLER_INPUT_BUTTONS; i++) 
        {
            WeaponAction a = new WeaponAction();
            a.inputButton = (ControllerActionInput)i;
            actionSlots.Add(a);
        }
    }

    protected override void HandleInput()
    {
        base.HandleInput();
        blackboard.actionSlot = GetActionSlot();
        CheckCurrentWeapon();
        SwitchWeaponActions();
    }

    public void SwitchWeaponActions()
    {
        WeaponList weaponList = blackboard.weaponList;
        switch (blackboard.currentWeapon)
        {
            case WeaponStatus.OneHanded:
                EmptyAllSlots();
                for (int i = 0; i < weaponList.oneHandedSwordActions.Count; i++)
                {
                    WeaponAction a = GetAction(weaponList.oneHandedSwordActions[i].inputButton);
                    a.targetAnim = weaponList.oneHandedSwordActions[i].targetAnim;
                }
                break;
            case WeaponStatus.TwoHanded:
                EmptyAllSlots();
                for (int i = 0; i < weaponList.twoHandedSwordActions.Count; i++)
                {
                    WeaponAction a = GetAction(weaponList.twoHandedSwordActions[i].inputButton);
                    a.targetAnim = weaponList.twoHandedSwordActions[i].targetAnim;
                }
                break;
            default:
                EmptyAllSlots();
                for (int i = 0; i < weaponList.oneHandedSwordActions.Count; i++)
                {
                    WeaponAction a = GetAction(weaponList.oneHandedSwordActions[i].inputButton);
                    a.targetAnim = weaponList.oneHandedSwordActions[i].targetAnim;
                }
                break;
        }
    }

    void EmptyAllSlots()
    {
        for (int i = 0; i < CONTROLLER_INPUT_BUTTONS; i++)
        {
            WeaponAction a = GetAction((ControllerActionInput)i);
            a.targetAnim = null;

        }
    }
    public WeaponAction GetActionSlot ()
    {
        ControllerActionInput a_input = GetActionInput();
        return GetAction(a_input);
    }

    public ControllerActionInput GetActionInput ()
    {
        if (playerActions.LightAttack.IsPressed)
            return ControllerActionInput.R1;            
        if (playerActions.HeavyAttack.IsPressed)
            return ControllerActionInput.R2;
        if (playerActions.Crouch)
            return ControllerActionInput.L1;
        if (playerActions.Crouch)
            return ControllerActionInput.L2;

        return ControllerActionInput.None;
    }

    public void CheckCurrentWeapon()
    {
        if (playerActions.OneHanded.IsPressed)
            blackboard.currentWeapon = WeaponStatus.OneHanded;
        if (playerActions.TwoHanded.IsPressed)
            blackboard.currentWeapon = WeaponStatus.TwoHanded;
    }

    WeaponAction GetAction(ControllerActionInput input)
    {
        for (int i = 0; i < actionSlots.Count; i++)
        {
            if (actionSlots[i].inputButton == input)
            {
                return actionSlots[i];
            }
        }

        return null;
    }
}

[System.Serializable]
public class WeaponAction
{
    public ControllerActionInput inputButton;
    public string targetAnim;
}

[System.Serializable]
public class ItemAction
{
    public string targetAnim;
    public string itemID;
}


