using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EZRollback.Core.Component {
public class RotationRollback : IRollbackBehaviour {
    [SerializeField] private RollbackElement<Quaternion> rotationRB = new RollbackElement<Quaternion>();

    public override void Simulate() {
    }

    public override void GoToFrame(int frameNumber) {
        rotationRB.SetValueFromFrameNumber(frameNumber);
        transform.rotation = rotationRB.value;
    }

    public override void DeleteFrames(int fromFrame, int numFramesToDelete) {
        rotationRB.DeleteFrames(fromFrame, numFramesToDelete);
    }

    public override void SaveFrame() {
        rotationRB.SetAndSaveValue(transform.rotation);
    }
}
}