using System;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;

namespace Packages.EZRollback.Runtime.Scripts.RollbackBehaviours {

[Serializable]
public class RollbackElementVector3 : RollbackElement<Vector3> { }

public class PositionRollback : IRollbackBehaviour {
    [SerializeField] private RollbackElementVector3 positionRB = new RollbackElementVector3();

    public override void Simulate() {
    }

    public override void SetValueFromFrameNumber(int frameNumber) {
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
