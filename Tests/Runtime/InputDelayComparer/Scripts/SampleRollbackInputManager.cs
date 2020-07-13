using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;

namespace Packages.EZRollback.Tests.Runtime.InputDelayComparer.Scripts {

public class SampleRollbackInputManager : RollbackInputManager
{
    protected  override RollbackInputBaseActions GetCurrentActionsValue(int controllerId){
        
        RollbackInputBaseActions actionsValue = new RollbackInputBaseActions(5);
        
        SetBitFromAction(0, KeyCode.W, ref actionsValue);
        SetBitFromAction(1, KeyCode.D, ref actionsValue);
        SetBitFromAction(2, KeyCode.S, ref actionsValue);
        SetBitFromAction(3, KeyCode.A, ref actionsValue);

        actionsValue.SetHorizontalAxis(Input.GetAxisRaw("Horizontal"));
        actionsValue.SetVerticalAxis(Input.GetAxisRaw("Vertical"));

        return actionsValue;
    }

    void SetBitFromAction(int inputIndex, KeyCode keyCode, ref RollbackInputBaseActions actionsValue) {
        actionsValue.SetOrClearBit(inputIndex, Input.GetKey(keyCode));
    }

    public override string GetActionName(int actionIndex) {
        switch (actionIndex) {
            case 0 : return "Up";
            case 1 : return "Right";
            case 2 : return "Down";
            case 3 : return "Left";
        }

        return "out";
    } 
}
}
