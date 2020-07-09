using System;
using UnityEngine;

namespace Packages.EZRollback.Runtime.Scripts {
[Serializable]
public class RollbackElement<T> {
    const int DEFAULT_SIZE = 51;
    
    //[SerializeField] List<T> elements = new List<T>();
    [SerializeField] T[] elements;

    [SerializeField] public T value;
    
    [SerializeField] int _head = 0;
    [SerializeField] int _tail = 0;
    [SerializeField] int _size = 0;
    public RollbackElement(T initValue = default, int baseSize = DEFAULT_SIZE) {
        InitializeRollbackElement(initValue, baseSize);
    }

    private void InitializeRollbackElement(T initValue, int baseSize) {
        value = initValue;
        elements = new T[baseSize];

        Clear();
    }
    
    public void Clear() {
        _head = 0;
        _tail = 0;
        _size = 0;
    }

    public int GetCurrentFrameNumberValue() {
        return _size;
    }
    
    public T GetValue(int frameNum) {
        if (frameNum < _size) {
            return elements[(_tail + frameNum) % elements.Length];
        }

        return default;
    }

    public void SetAndSaveValue(T newValue) {
        value = newValue;
        SaveFrame();
    }

    //Used for inputs and corrections of data
    public void CorrectValue(T correctedValue, int frameNum) {
        elements[(_tail + frameNum) % elements.Length] = correctedValue;
    }

    public void SaveFrame() {
        elements[_head] = value;

        _head++;
        _size++;
        
        CheckArraySize();
    }

    public void SetValueFromFrameNumber(int frameNum) {
        if (-1 > frameNum && frameNum > _size) {
            Debug.LogError("Cannot go back from higher number of registered frames");
            return;
        }
        
        value = elements[(_tail + frameNum) % elements.Length];
    }
    
    public void DeleteFrames(int numFramesToDelete, bool firstFrames) {
        if (firstFrames) {
            _tail += numFramesToDelete;
            _tail = _tail % elements.Length;
        } else {
            _head -= numFramesToDelete;
            if (_head < 0) {
                _head += elements.Length;
            }
        }

        _size -= numFramesToDelete;

        //That means we deleted all the frames
        if (_size <= 0) {
            _size = 0;
            _head = 0;
            _tail = 0;
        }
    }

    private void CheckArraySize() {
        _head = _head % elements.Length;

        if (_head == _tail && _size == elements.Length) {
            Resize(elements.Length * 2);
        }
    }

    private void Resize(int newSize) {
        int currentSize = elements.Length;
        
        T[] oldElements = elements;
        elements = new T[newSize];

        //For each element of the previous buffer
        for (int i = 0; i < _size; i++) {
            elements[i] = oldElements[( _tail + i) % currentSize];
        }

        _tail = 0;
        _head = currentSize;
        _size = currentSize;
    }
}
}
