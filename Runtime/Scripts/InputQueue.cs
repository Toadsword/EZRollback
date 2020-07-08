using System;
using UnityEngine;

namespace Packages.EZRollback.Runtime.Scripts 
{

//Created to let the Unity Editor serialize the element.
[Serializable]
public class RollbackElementRollbackInputBaseActions : RollbackElement<RollbackInputBaseActions> {}

public class InputQueue : MonoBehaviour
{
    public enum AxisEnum {
        HORIZONTAL,
        VERTICAL
    }
    
    [SerializeField] protected RollbackElementRollbackInputBaseActions _baseActions = new RollbackElementRollbackInputBaseActions();

    public int GetCurrentFrameNumberValue() {
        return _baseActions.GetCurrentFrameNumberValue();
    }
    
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

    private int CheckFrameNumber(int frameNumber) {
        if (frameNumber < 0 || frameNumber > _baseActions.GetCurrentFrameNumberValue()) {
            frameNumber = _baseActions.GetCurrentFrameNumberValue();
        }

        return frameNumber;
    }
    
    public virtual float GetAxis(AxisEnum axis, int frameNumber = -1) {
        frameNumber = CheckFrameNumber(frameNumber);
        switch (axis) {
            case AxisEnum.VERTICAL:
                return TransformSByteToAxisValue(_baseActions.GetValue(frameNumber).verticalValue);
            case AxisEnum.HORIZONTAL:
                return TransformSByteToAxisValue(_baseActions.GetValue(frameNumber).horizontalValue);
        }

        return 0;
    }
    
    public virtual bool GetInput(int actionValue, int frameNumber = -1) {
        frameNumber = CheckFrameNumber(frameNumber);
        return _baseActions.GetValue(frameNumber).GetValueBit(actionValue);
    }

    public virtual bool GetInputDown(int actionValue, int frameNumber = -1) {
        frameNumber = CheckFrameNumber(frameNumber);
        return !_baseActions.GetValue(_baseActions.GetCurrentFrameNumberValue()).GetValueBit(actionValue) && 
               _baseActions.value.GetValueBit(actionValue);
    }
    
    public virtual bool GetInputUp(int actionValue, int frameNumber = -1) {
        frameNumber = CheckFrameNumber(frameNumber);
        return _baseActions.GetValue( frameNumber - 1).GetValueBit(actionValue) && 
               !_baseActions.GetValue(frameNumber).GetValueBit(actionValue);
    }

    protected sbyte TransformAxisValueToSByte(float value) {
        return (sbyte) (value * 127f);
    }
    private float TransformSByteToAxisValue(sbyte value) {
        return value / 127f;
    }
}
}
