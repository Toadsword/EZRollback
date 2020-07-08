using System;
using UnityEngine;

namespace Packages.EZRollback.Runtime.Scripts 
{

//Created to let the Unity Editor serialize the element.
[Serializable]
public class RollbackElementRollbackInputBaseActions : RollbackElement<RollbackInputBaseActions> {}

public class InputQueue : MonoBehaviour
{
    [SerializeField] protected RollbackElementRollbackInputBaseActions _baseActions = new RollbackElementRollbackInputBaseActions();

    public virtual void PrepareInput(){}

    public void SaveFrame() {
        _baseActions.SaveFrame();
    }
    
    public void GoToFrame(int frameNumber) {
        _baseActions.SetValueFromFrameNumber(frameNumber);
    }

    public void DeleteFrames(int numFramesToDelete, bool firstFrames) {
        _baseActions.DeleteFrames(numFramesToDelete, firstFrames);
    }
}
}
