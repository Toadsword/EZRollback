using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;

namespace Packages.EZRollback.Tests.Runtime.InputDelayComparer.Scripts {

public class SampleRollbackInputManager : RollbackInputManager
{
    public enum ActionsCode : int{
        UP = 0,
        RIGHT,
        DOWN,
        LEFT,
        
        SHOOT
    }
    
    
    protected  override RollbackInputBaseActions GetCurrentActionsValue(int controllerId){
        
        RollbackInputBaseActions actionsValue = new RollbackInputBaseActions(5);
        
        SetBitFromKeycode((int)ActionsCode.UP, KeyCode.W, ref actionsValue);
        SetBitFromKeycode((int)ActionsCode.RIGHT, KeyCode.D, ref actionsValue);
        SetBitFromKeycode((int)ActionsCode.DOWN, KeyCode.S, ref actionsValue);
        SetBitFromKeycode((int)ActionsCode.LEFT, KeyCode.A, ref actionsValue);
        actionsValue.SetOrClearBit((int)ActionsCode.SHOOT, Input.GetMouseButton(1));

        actionsValue.SetHorizontalAxis(Input.GetAxisRaw("Horizontal"));
        actionsValue.SetVerticalAxis(Input.GetAxisRaw("Vertical"));

        return actionsValue;
    }

    public override int AddPlayer() {
        Debug.Log("AddPlayer");
        return base.AddPlayer();
    }

    void SetBitFromKeycode(int inputIndex, KeyCode keyCode, ref RollbackInputBaseActions actionsValue) {
        actionsValue.SetOrClearBit(inputIndex, Input.GetKey(keyCode));
    }

    public override string GetActionName(int actionIndex) {
        switch (actionIndex) {
            case 0 : return "Up";
            case 1 : return "Right";
            case 2 : return "Down";
            case 3 : return "Left";
            case 4 : return "Left mouse button";
        }

        return "out";
    } 
}
}
