using System;
using UnityEngine;

namespace Packages.EZRollback.Runtime.Scripts.RollbackBehaviours {


[Serializable]
public class RollbackElementQuaternion : RollbackElement<Quaternion> { }

public class RotationRollback : IRollbackBehaviour {
    [SerializeField] RollbackElementQuaternion rotationRB = new RollbackElementQuaternion();
    
    public override void Simulate() {
    }

    public override void GoToFrame(int frameNumber) {
        rotationRB.SetValueFromFrameNumber(frameNumber);
        transform.rotation = rotationRB.value;
    }

    public override void DeleteFrames(int numFramesToDelete, bool firstFrames) {
        rotationRB.DeleteFrames(numFramesToDelete, firstFrames);
    }

    public override void SaveFrame() {
        rotationRB.SetAndSaveValue(transform.rotation);
    }
}
}