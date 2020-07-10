using System;
using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;

public class TestPlayerControllerNoRollback : MonoBehaviour
{
    private float _horizontal = 0.0f;
    private float _vertical = 0.0f;

    // Update is called once per frame
    void Update()
    {
        _horizontal = InputActionManager.GetAxis(InputActionManager.AxisType.HORIZONTAL);
        _vertical = InputActionManager.GetAxis(InputActionManager.AxisType.VERTICAL);
    }

    void FixedUpdate() {
        float angle = Mathf.Atan2(_vertical, _horizontal) * Mathf.Rad2Deg - 90.0f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
