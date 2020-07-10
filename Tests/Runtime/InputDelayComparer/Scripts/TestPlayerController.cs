using System.Collections;
using System.Collections.Generic;
using Packages.EZRollback.Runtime.Scripts;
using UnityEditor;
using UnityEngine;

public class TestPlayerController : IRollbackBehaviour
{
    [SerializeField] private float _horizontal = 0.0f;
    [SerializeField] private float _vertical = 0.0f;

    new void Start() {
        base.Start();
        RollbackManager._instance.inputQueue.AddController();
    }
    // Update is called once per frame
    void Update() {
        Simulate();
    }
    
    public override void Simulate() {
        _horizontal = RollbackManager._instance.inputQueue.GetAxis(InputQueue.AxisEnum.HORIZONTAL, 0);
        _vertical = RollbackManager._instance.inputQueue.GetAxis(InputQueue.AxisEnum.VERTICAL, 0);

        float angle = Mathf.Atan2(_vertical, _horizontal) * Mathf.Rad2Deg - 90.0f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public override void GoToFrame(int frameNumber) {
    }

    public override void DeleteFrames(int numFramesToDelete, bool firstFrames) {
    }

    public override void SaveFrame() {
    }

}
