using System;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Packages.EZRollback.Runtime.Scripts.RollbackBehaviours {

public class ActiveRollback : IRollbackBehaviour {
    [SerializeField] RollbackElement<bool> _isActiveRb = new RollbackElement<bool>();

    public override void Simulate() { }

    public override void SetValueFromFrameNumber(int frameNumber) {
        _isActiveRb.SetValueFromFrameNumber(frameNumber);
        gameObject.SetActive(_isActiveRb.value);
    }

    public override void DeleteFrames(int numFramesToDelete, bool firstFrames) {
        _isActiveRb.DeleteFrames(numFramesToDelete, firstFrames);
    }

    public override void SaveFrame() {
        _isActiveRb.SetAndSaveValue(gameObject.activeSelf);
    }
}
}