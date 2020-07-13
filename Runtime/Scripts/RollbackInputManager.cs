using System;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.EZRollback.Runtime.Scripts 
{

/**
 * \brief Class to allow serialization of RollbackElement of type RollbackInputBaseActions
 */
[Serializable]
public class RollbackElementRollbackInputBaseActions : RollbackElement<RollbackInputBaseActions> {}

/**
 * \brief Rollback manager that deals with input. Allow registering inputs and rewind them in time.
 */
public abstract class RollbackInputManager : MonoBehaviour
{
    public enum AxisEnum {
        HORIZONTAL,
        VERTICAL
    }
    
    [SerializeField] protected List<RollbackElementRollbackInputBaseActions> _playerInputList = new List<RollbackElementRollbackInputBaseActions>();

    void OnEnable() {
        if (_playerInputList == null) {
            _playerInputList = new List<RollbackElementRollbackInputBaseActions>();
        }
    }

    /**
     * \brief Add a player to the list of inputs.
     * \return The number of current players in the list.
     */
    public int AddPlayer() {
        _playerInputList.Add(new RollbackElementRollbackInputBaseActions());
        return _playerInputList.Count;
    }
    
    /**
     * \brief Set and save the current value of the player.
     * \param playerId ID of the player to add an input.
     */
    public void AddInput(int playerId) {
        AddInput(playerId, GetCurrentActionsValue(playerId));
    }

    /**
     * \brief Set and save the value of the player.
     * \param playerId ID of the player to add an input.
     * \param rbInputBaseActions inputs to save.
     */
    public void AddInput(int playerId, RollbackInputBaseActions rbInputBaseActions) {
        _playerInputList[playerId].SetAndSaveValue(rbInputBaseActions);
    }

    /**
     * \brief Calculate the current state of the input system. Need to be implemented accordingly to your needs with your used input system.
     * \param playerId ID of the player to add an input.
     */
    protected abstract RollbackInputBaseActions GetCurrentActionsValue(int playerId);

    /**
     * \brief Update the status of the inputs of all registered players.
     */
    public virtual void UpdateInputStatus() {
        for (int i = 0; i < _playerInputList.Count; i++) {
            RollbackInputBaseActions actionsValue = GetCurrentActionsValue(i);

            _playerInputList[i].value = actionsValue;
        }
    }
    
    /**
     * \brief Save the inputs of all the players of the current frames.
     */
    public void SaveFrame() {
        for (int i = 0; i < _playerInputList.Count; i++) {
            _playerInputList[i].SaveFrame();
        }
    }
    
    /**
     * \brief Save the inputs of all the players of the current frames.
     */
    public void SetValueFromFrameNumber(int frameNumber) {
        for (int i = 0; i < _playerInputList.Count; i++) {
            _playerInputList[i].SetValueFromFrameNumber(frameNumber);
        }
    }

    public void DeleteFrames(int numFramesToDelete, bool firstFrames) {
        for (int i = 0; i < _playerInputList.Count; i++) {
            _playerInputList[i].DeleteFrames(numFramesToDelete, firstFrames);
        }
    }

    public virtual float GetAxis(AxisEnum axis, int playerId, int frameNumber = -1) {
        if (playerId >= _playerInputList.Count)
            return 0.0f;
        
        frameNumber = CheckFrameNumber(frameNumber);
        switch (axis) {
            case AxisEnum.VERTICAL:
                return _playerInputList[playerId].GetValue(frameNumber).GetVerticalAxis();
            case AxisEnum.HORIZONTAL:
                return _playerInputList[playerId].GetValue(frameNumber).GetHorizontalAxis();
        }

        return 0.0f;
    }
    
    public virtual bool GetInput(int actionValue, int playerId, int frameNumber = -1) {
        frameNumber = CheckFrameNumber(frameNumber);
        return _playerInputList[playerId].GetValue(frameNumber).GetValueBit(actionValue);
    }

    public virtual bool GetInputDown(int actionValue, int playerId, int frameNumber = -1) {
        frameNumber = CheckFrameNumber(frameNumber);
        return !_playerInputList[playerId].GetValue(frameNumber - 1).GetValueBit(actionValue) && 
               _playerInputList[playerId].value.GetValueBit(actionValue);
    }
    
    public virtual bool GetInputUp(int actionValue, int playerId, int frameNumber = -1) {
        frameNumber = CheckFrameNumber(frameNumber);
        return _playerInputList[playerId].GetValue(frameNumber - 1).GetValueBit(actionValue) && 
               !_playerInputList[playerId].GetValue(frameNumber).GetValueBit(actionValue);
    }
    
    public virtual string GetActionName(int actionIndex) {
        return actionIndex.ToString();
    }

    public int GetNumOfControllers() {
        return _playerInputList.Count;
    }

    public void CorrectInputs(int playerId, int numFrames, RollbackInputBaseActions[] rbInputBaseActions) {
        int currentFrame = RollbackManager._instance.GetDisplayedFrameNum();
        for (int i = 0; i < numFrames; i++) {
            _playerInputList[playerId].CorrectValue(rbInputBaseActions[i],currentFrame - numFrames + i);
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
