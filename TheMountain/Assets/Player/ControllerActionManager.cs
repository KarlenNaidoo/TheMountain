using UnityEngine;
using System.Collections.Generic;
using static Player.Utility;
using Player.PlayerController;

public class ControllerActionManager : PlayerInput
{ 
    [SerializeField] List<ControllerAction> actionSlots = new List<ControllerAction>();
    
    protected ControllerActionManager()
    {
        for (int i = 0; i < 4; i++)
        {
            ControllerAction a = new ControllerAction();
            a.inputButton = (ActionInput)i;
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
        Weapon weaponList = blackboard.weaponList;
        switch (blackboard.currentWeapon)
        {
            case WeaponStatus.OneHanded:
                for (int i = 0; i < weaponList.oneHandedSwordActions.Count; i++)
                {
                    ControllerAction a = GetAction(weaponList.oneHandedSwordActions[i].inputButton);
                    a.targetAnim = weaponList.oneHandedSwordActions[i].targetAnim;
                }
                break;
            case WeaponStatus.TwoHanded:
                for (int i = 0; i < weaponList.twoHandedSwordActions.Count; i++)
                {
                    ControllerAction a = GetAction(weaponList.twoHandedSwordActions[i].inputButton);
                    a.targetAnim = weaponList.twoHandedSwordActions[i].targetAnim;
                }
                break;
            default:
                for (int i = 0; i < weaponList.oneHandedSwordActions.Count; i++)
                {
                    ControllerAction a = GetAction(weaponList.oneHandedSwordActions[i].inputButton);
                    a.targetAnim = weaponList.oneHandedSwordActions[i].targetAnim;
                }
                break;
        }
    }

    public ControllerAction GetActionSlot ()
    {
        ActionInput a_input = GetActionInput();
        return GetAction(a_input);
    }

    public ActionInput GetActionInput ()
    {
        if (playerActions.LightAttack.IsPressed)
            return ActionInput.R1;            
        if (playerActions.HeavyAttack.IsPressed)
            return ActionInput.R2;
        if (playerActions.Crouch)
            return ActionInput.L1;
        if (playerActions.Crouch)
            return ActionInput.L2;

        return ActionInput.None;
    }

    public void CheckCurrentWeapon()
    {
        if (playerActions.OneHanded.IsPressed)
            blackboard.currentWeapon = WeaponStatus.OneHanded;
        if (playerActions.TwoHanded.IsPressed)
            blackboard.currentWeapon = WeaponStatus.TwoHanded;
    }

    ControllerAction GetAction(ActionInput input)
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
public class ControllerAction
{
    public ActionInput inputButton;
    public string targetAnim;
}


