﻿using System;
using System.Collections;
using System.Collections.Generic;
using Packages.EZRollback.Runtime.Scripts;
using UnityEditor;
using UnityEngine;

public class TestPlayerController : IRollbackBehaviour
{
    [SerializeField] private float _horizontal = 0.0f;
    [SerializeField] private float _vertical = 0.0f;

    [SerializeField] Color _baseColor = Color.white;
    SpriteRenderer _spriteRenderer;

    RollbackElement<Color> _colors = new RollbackElement<Color>();
    
    new void Start() {
        base.Start();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        RollbackManager._instance.inputQueue.AddController();
    }
    // Update is called once per frame
    void Update() {
        Simulate();
    }
    
    public override void Simulate() {
        _horizontal = RollbackManager._instance.inputQueue.GetAxis(InputQueue.AxisEnum.HORIZONTAL, 0);
        _vertical = RollbackManager._instance.inputQueue.GetAxis(InputQueue.AxisEnum.VERTICAL, 0);
        
        _spriteRenderer.color = _baseColor;
        if (RollbackManager._instance.inputQueue.GetInput(1, 0)) {
            Debug.Log("Input");
            _spriteRenderer.color = Color.cyan; 
        }
        if (RollbackManager._instance.inputQueue.GetInputDown(1, 0)) {
            Debug.Log("InputDown");
            _spriteRenderer.color = Color.blue; 
        }
        if (RollbackManager._instance.inputQueue.GetInputUp(1, 0)) {
            Debug.Log("InputUp");
            _spriteRenderer.color = Color.green; 
        }
        
        float angle = Mathf.Atan2(_vertical, _horizontal) * Mathf.Rad2Deg - 90.0f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        _colors.value = _spriteRenderer.color;
    }

    public override void SetValueFromFrameNumber(int frameNumber) {
        _colors.SetValueFromFrameNumber(frameNumber);
        _spriteRenderer.color = _colors.value;
    }

    public override void DeleteFrames(int numFramesToDelete, bool firstFrames) {
        _colors.DeleteFrames(numFramesToDelete, firstFrames);
    }

    public override void SaveFrame() {
        _colors.SaveFrame();
    }

}
