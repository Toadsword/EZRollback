using System.Collections;
using System.Collections.Generic;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;

namespace EZRollback.Core.Component {
public class PositionRollback : IRollbackBehaviour {
    [SerializeField] private RollbackElement<Vector3> positionRB = new RollbackElement<Vector3>();

    public override void Simulate() {
    }

    public override void GoToFrame(int frameNumber) {
        positionRB.SetValueFromFrameNumber(frameNumber);
        transform.position = positionRB.value;
    }

    public override void DeleteFrames(int numFramesToDelete, bool firstFrames) {
        positionRB.DeleteFrames(numFramesToDelete, firstFrames);
    }

    public override void SaveFrame() {
        positionRB.SetAndSaveValue(transform.position);
    }
}
}
