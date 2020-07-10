using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerControllerNoRollback : MonoBehaviour
{
    [SerializeField] private float _horizontal = 0.0f;
    [SerializeField] private float _vertical = 0.0f;

    // Update is called once per frame
    void Update() {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate() {
        float angle = Mathf.Atan2(_vertical, _horizontal) * Mathf.Rad2Deg - 90.0f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
