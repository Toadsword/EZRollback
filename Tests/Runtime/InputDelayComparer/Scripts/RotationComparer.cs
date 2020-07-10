using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationComparer : MonoBehaviour {

    [SerializeField] Transform _transform1;
    [SerializeField] Transform _transform2;

    float time;

    bool recording = false;

    void Start() {
        recording = false;
        time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (recording) {
            if (_transform1.rotation.Equals(_transform2.rotation)) {
                Debug.Log("----------- FINISHED RECORDING ------------");
                Debug.Log("Time elapsed : " + (Time.time - time).ToString());
                recording = false;
            }
        } else {
            if (!_transform1.rotation.Equals(_transform2.rotation)) {
                time = Time.time;
                recording = true;
            }
        }
    }
}
