using System;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.EZRollback.Runtime.Scripts 
{

//Created to let the Unity Editor serialize the element.
[Serializable]
public class RollbackElementRollbackInputBaseActions : RollbackElement<RollbackInputBaseActions> {}

public abstract class InputQueue : MonoBehaviour
{
    public enum AxisEnum {
        HORIZONTAL,
        VERTICAL
    }
    
    [SerializeField] protected List<RollbackElementRollbackInputBaseActions> _baseActions = new List<RollbackElementRollbackInputBaseActions>();

    void OnEnable() {
        if (_baseActions == null) {
            _baseActions = new List<RollbackElementRollbackInputBaseActions>();
        }
    }

    public int AddController() {
        _baseActions.Add(new RollbackElementRollbackInputBaseActions());
        return _baseActions.Count;
    }
    
    public void AddInput(int controllerId) {
        AddInput(controllerId, GetCurrentActionsValue(controllerId));
    }

    public void AddInput(int controllerId, RollbackInputBaseActions rbInputBaseActions) {
        _baseActions[controllerId].SetAndSaveValue(rbInputBaseActions);
    }

    protected abstract RollbackInputBaseActions GetCurrentActionsValue(int controllerId);

    public virtual void UpdateInputStatus() {
        for (int i = 0; i < _baseActions.Count; i++) {
            RollbackInputBaseActions actionsValue = GetCurrentActionsValue(i);

            _baseActions[i].value = actionsValue;
        }
    }
    
    public void SaveFrame() {
        for (int i = 0; i < _baseActions.Count; i++) {
            _baseActions[i].SaveFrame();
        }
    }
    
    public void GoToFrame(int frameNumber) {
        for (int i = 0; i < _baseActions.Count; i++) {
            _baseActions[i].SetValueFromFrameNumber(frameNumber);
        }
    }

    public void DeleteFrames(int numFramesToDelete, bool firstFrames) {
        for (int i = 0; i < _baseActions.Count; i++) {
            _baseActions[i].DeleteFrames(numFramesToDelete, firstFrames);
        }
    }

    public virtual float GetAxis(AxisEnum axis, int controllerId, int frameNumber = -1) {
        if (controllerId >= _baseActions.Count)
            return 0.0f;
        
        frameNumber = CheckFrameNumber(frameNumber);
        switch (axis) {
            case AxisEnum.VERTICAL:
                return TransformSByteToAxisValue(_baseActions[controllerId].GetValue(frameNumber).verticalValue);
            case AxisEnum.HORIZONTAL:
                return TransformSByteToAxisValue(_baseActions[controllerId].GetValue(frameNumber).horizontalValue);
        }

        return 0.0f;
    }
    
    public virtual bool GetInput(int actionValue, int controllerId, int frameNumber = -1) {
        frameNumber = CheckFrameNumber(frameNumber);
        return _baseActions[controllerId].GetValue(frameNumber).GetValueBit(actionValue);
    }

    public virtual bool GetInputDown(int actionValue, int controllerId, int frameNumber = -1) {
        frameNumber = CheckFrameNumber(frameNumber);
        return !_baseActions[controllerId].GetValue(frameNumber - 1).GetValueBit(actionValue) && 
               _baseActions[controllerId].value.GetValueBit(actionValue);
    }
    
    public virtual bool GetInputUp(int actionValue, int controllerId, int frameNumber = -1) {
        frameNumber = CheckFrameNumber(frameNumber);
        return _baseActions[controllerId].GetValue(frameNumber - 1).GetValueBit(actionValue) && 
               !_baseActions[controllerId].GetValue(frameNumber).GetValueBit(actionValue);
    }
    
    public virtual string GetActionName(int actionIndex) {
        return actionIndex.ToString();
    } 
    
    public sbyte TransformAxisValueToSByte(float value) {
        return (sbyte) (value * 127f);
    }
    public float TransformSByteToAxisValue(sbyte value) {
        return value / 127.0f;
    }

    public int GetNumOfControllers() {
        return _baseActions.Count;
    }

    public void CorrectInputs(int controllerId, int numFrames, RollbackInputBaseActions[] rbInputBaseActions) {
        int currentFrame = RollbackManager._instance.GetDisplayedFrameNum();
        for (int i = 0; i < numFrames; i++) {
            _baseActions[controllerId].CorrectValue(rbInputBaseActions[i],currentFrame - numFrames + i);
        }
    }
    
    private int CheckFrameNumber(int frameNumber) {
        if (frameNumber < 0 || frameNumber > RollbackManager._instance.GetDisplayedFrameNum()) {
            frameNumber = RollbackManager._instance.GetDisplayedFrameNum();
        }

        return frameNumber;
    }
}
}
