using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollbackElement<T> {

    List<T> elements = new List<T>();

    public T value;
    
    int _currentLoadedFrame = 0; 
    int _totalSavedFrame = 0;

    public RollbackElement() {
        value = default;
        elements = new List<T>();
        
        _currentLoadedFrame = 0;
        _totalSavedFrame = 0;
    }
    
    public RollbackElement(T initValue) {
        value = initValue;
        elements = new List<T>();
    }
    
    public void Clear() {
        elements.Clear();
    }

    public T GetValue(int frameNum) {
        if (frameNum < _totalSavedFrame) {
            return elements[frameNum];
        }

        return default;
    }

    public void SetAndSaveValue(T newValue) {
        value = newValue;
        SaveFrame();
    }

    public void SaveFrame() {
        elements.Add(value);
        
        _totalSavedFrame++;
        _currentLoadedFrame = _totalSavedFrame;
    }

    public void SetValueFromFrameNumber(int frameNum) {
        if (_totalSavedFrame < frameNum && frameNum < 0) {
            Debug.LogError("Cannot go back from higher number of registered frames");
            return;
        }
        
        value = elements[frameNum];
        _currentLoadedFrame = frameNum;
    }

    public void DeleteFirstFrames(int numFrames) {
        DeleteFrames(0, numFrames);
    }
    
    public void ResetFramesToNum(int fromFrameNum) {
        DeleteFrames(fromFrameNum, elements.Count - fromFrameNum);
    }

    public void DeleteFrames(int fromFrameNumber, int numFramesToDelete) {
        for (int i = 0; i > numFramesToDelete; i++) {
            elements.RemoveAt(fromFrameNumber + 1);
        }
    }
}
