using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZRollback.Core.Component {
public class TransformRollback : IRollbackBehaviour {
    [SerializeField] private RollbackElement<Vector3> positionRB = new RollbackElement<Vector3>();

    public override void Simulate() {
    }

    public override void GoToFrame(int frameNumber) {
        positionRB.SetValueFromFrameNumber(frameNumber);
        transform.position = positionRB.value;
    }

    public override void DeleteFrames(int fromFrame, int numFramesToDelete) {
        positionRB.DeleteFrames(fromFrame, numFramesToDelete);
    }

    public override void SaveFrame() {
        positionRB.SetAndSaveValue(transform.position);
    }
}
}
