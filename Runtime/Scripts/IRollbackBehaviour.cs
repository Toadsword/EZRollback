using System;
using UnityEngine;

[Serializable]
public abstract class IRollbackBehaviour : MonoBehaviour {
    
    public abstract void Simulate();

    public abstract void GoToFrame(int frameNumber);

    public abstract void DeleteFrames(int fromFrame, int toFrame);
    
    public abstract void SaveFrame();
}
