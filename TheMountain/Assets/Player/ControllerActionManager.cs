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


