using UnityEngine;
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
        blackboard.comboList = GetComboList();
        IsOneHandedOrTwoHanded();
        MapControllerAtkActions();
    }

    public void MapControllerAtkActions()
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
                    a.combos = weaponList.oneHandedSwordActions[i].combos;
                }
                break;
            case WeaponStatus.TwoHanded:
                EmptyAllSlots();
                for (int i = 0; i < weaponList.twoHandedSwordActions.Count; i++)
                {
                    WeaponAction a = GetAction(weaponList.twoHandedSwordActions[i].inputButton);
                    a.targetAnim = weaponList.twoHandedSwordActions[i].targetAnim;
                    a.combos = weaponList.twoHandedSwordActions[i].combos;
                }
                break;
            default:
                EmptyAllSlots();
                for (int i = 0; i < weaponList.oneHandedSwordActions.Count; i++)
                {
                    WeaponAction a = GetAction(weaponList.oneHandedSwordActions[i].inputButton);
                    a.targetAnim = weaponList.oneHandedSwordActions[i].targetAnim;
                    a.combos = weaponList.oneHandedSwordActions[i].combos;
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

    public List<ComboAction> GetComboList()
    {
        WeaponAction a_action = GetActionSlot();
        return a_action.combos;

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
        if (playerActions.Crouch.IsPressed)
            return ControllerActionInput.L1;
        if (playerActions.Crouch.IsPressed)
            return ControllerActionInput.L2;

        return ControllerActionInput.None;
    }

    public void IsOneHandedOrTwoHanded()
    {
        if (playerActions.OneHanded.IsPressed)
            blackboard.currentWeapon = WeaponStatus.OneHanded;
        if (playerActions.TwoHanded.IsPressed)
            blackboard.currentWeapon = WeaponStatus.TwoHanded;
    }

    public void CanAttack()
    {
        Debug.Log("Opening can attack");
        blackboard.canAttack = true;
        blackboard.doOnce = false;
    }

    public void CloseCanAttack()
    {

        //Debug.Log("Closing can attack");
        //blackboard.canAttack = false;
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
public class ComboAction
{
    public ControllerActionInput inputButton;
    public string targetAnim;
}

[System.Serializable]
public class WeaponAction
{
    public ControllerActionInput inputButton;
    public string targetAnim;
    public List<ComboAction> combos;
}

[System.Serializable]
public class ItemAction
{
    public string targetAnim;
    public string itemID;
}


