using UnityEngine;

namespace Packages.EZRollback.Runtime.Scripts.RollbackBehaviours {
public class RotationRollback : IRollbackBehaviour {
    [SerializeField] private RollbackElement<Quaternion> rotationRB = new RollbackElement<Quaternion>();

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