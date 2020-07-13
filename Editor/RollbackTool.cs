using System;
using System.IO;
using Packages.EZRollback.Editor.Utils;
using Packages.EZRollback.Runtime.Scripts;
using UnityEditor;
using UnityEngine;

namespace Packages.EZRollback.Editor {
        
    [Serializable]
    public class RollbackTool : EditorWindow {
        public static RollbackInformation rollbackInformation = new RollbackInformation();

        RollbackManager _rollbackManager;
        RollbackInputBaseActions _rbBaseInput;

        int _numOfInputs = 1;
        int _numFramesToSimulate = 0;
        
        int _controllerId = 0;
        
        [MenuItem("RollbackTool/Information")]
        public static void ShowWindow() {
            GetWindow(typeof(RollbackTool));
        }
        
        RollbackTool() {
            EditorApplication.playModeStateChanged += LogPlayModeState;
        }

        void OnGUI() {
            DisplayRollbackEditionButtons();

            if (UnityEditor.EditorApplication.isPlaying && _rollbackManager != null) {
                GUIUtils.GuiLine(3);
                DisplayRollbackInformations();

                GUIUtils.GuiLine(3);
                DisplaySimulateOptions();
            
                GUIUtils.GuiLine(3);
                DisplaySimulateInput();
            }
        }

        private void LogPlayModeState(PlayModeStateChange playModeStateChange) {
            switch (playModeStateChange) {
                case PlayModeStateChange.EnteredPlayMode:
                    _rollbackManager = GameObject.FindObjectOfType<RollbackManager>();
                    if (_rollbackManager == null) {
                        _rollbackManager = Instantiate(Resources.Load("RollbackManagerPrefab") as GameObject, Vector3.zero, Quaternion.identity).GetComponent<RollbackManager>();
                    }
                    
                    _rbBaseInput = new RollbackInputBaseActions(1 + _numOfInputs / 8);
                    break;
            }
        }

        private void DisplayRollbackEditionButtons() {
            
            
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("<=", GUILayout.Width(30), GUILayout.Height(20))) {
                _rollbackManager.SetValueFromFrameNumber(0, false);
                UnityEditor.EditorApplication.isPaused = true;
            }
            
            if (GUILayout.Button("<", GUILayout.Width(30), GUILayout.Height(20))) {
                _rollbackManager.SetValueFromFrameNumber(_rollbackManager.GetDisplayedFrameNum() - 1, false);
                UnityEditor.EditorApplication.isPaused = true;
            }

            if (UnityEditor.EditorApplication.isPlaying){
                if (GUILayout.Button("Stop", GUILayout.Width(100), GUILayout.Height(20))) {
                    UnityEditor.EditorApplication.isPlaying = false;
                }
            } else {
                if (GUILayout.Button("Play", GUILayout.Width(100), GUILayout.Height(20))) {
                    UnityEditor.EditorApplication.isPlaying = true;
                }
            }

            if (UnityEditor.EditorApplication.isPaused) {
                if (GUILayout.Button("Resume", GUILayout.Width(100), GUILayout.Height(20))) {
                    _rollbackManager.SetValueFromFrameNumber(_rollbackManager.GetDisplayedFrameNum(), true);
                    UnityEditor.EditorApplication.isPaused = false;
                }
            } else {
                if (GUILayout.Button("Pause", GUILayout.Width(100), GUILayout.Height(20))) {
                    UnityEditor.EditorApplication.isPaused = true;
                }
            }

            if (GUILayout.Button(">", GUILayout.Width(30), GUILayout.Height(20))) {
                _rollbackManager.SetValueFromFrameNumber(_rollbackManager.GetDisplayedFrameNum() + 1, false);
            }
            
            if (GUILayout.Button("=>", GUILayout.Width(30), GUILayout.Height(20))) {
                _rollbackManager.SetValueFromFrameNumber(_rollbackManager.GetMaxFramesNum() - 1, false);
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DisplayRollbackInformations() {
            GUILayout.Label("Rollback options", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Current Frame", GUILayout.Width(100));
            int newFrameNum = (int) GUILayout.HorizontalSlider(_rollbackManager.GetDisplayedFrameNum(), 0,
                (_rollbackManager.GetMaxFramesNum()));

            if (newFrameNum != _rollbackManager.GetDisplayedFrameNum()) {
                _rollbackManager.SetValueFromFrameNumber(newFrameNum, false);
                UnityEditor.EditorApplication.isPaused = true;
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Label("Current frame number : " + (_rollbackManager.GetDisplayedFrameNum()) + " / " + (_rollbackManager.GetMaxFramesNum()));
        }

        private void DisplaySimulateOptions() {
            GUILayout.Label("Simulations options", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            _numFramesToSimulate = EditorGUILayout.IntField("Num frames to simulate : ", _numFramesToSimulate);
            if (_numFramesToSimulate < 0)
                _numFramesToSimulate = 0;
            if (GUILayout.Button("Simulate !")) {
                if (_rollbackManager != null) {
                    _rollbackManager.Simulate(_numFramesToSimulate);
                }
            }
            
            if (GUILayout.Button("Go back x frames")) {
                if (_rollbackManager != null) {
                    _rollbackManager.GoBackInFrames(_numFramesToSimulate, false, false);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DisplaySimulateInput() {
            
            GUILayout.Label("Input simulations options", EditorStyles.boldLabel);

            EditorGUILayout.IntField("ControllerId : ", _controllerId);
            
            //Vertical axis input
            float verticalValue = _rollbackManager.inputQueue.TransformSByteToAxisValue(_rbBaseInput.verticalValue);
            verticalValue = EditorGUILayout.Slider("Vertical axis", verticalValue, -1f, 1f);
            _rbBaseInput.verticalValue = _rollbackManager.inputQueue.TransformAxisValueToSByte(verticalValue);
            
            //Vertical axis input
            float horizontalValue = _rollbackManager.inputQueue.TransformSByteToAxisValue(_rbBaseInput.horizontalValue);
            horizontalValue = EditorGUILayout.Slider("Horizontal axis", horizontalValue, -1f, 1f);
            _rbBaseInput.horizontalValue = _rollbackManager.inputQueue.TransformAxisValueToSByte(horizontalValue);

            //Button inputs
            GUILayout.Label("Buttons press options : ", EditorStyles.boldLabel);
            int oldNumOfInputs = _numOfInputs;
            _numOfInputs = EditorGUILayout.IntField("NumOfInputs : ", _numOfInputs);
            
            if (_numOfInputs != oldNumOfInputs) {
                _rbBaseInput = new RollbackInputBaseActions(1 + _numOfInputs / 8);
            }
            
            for (int i = 0; i < _numOfInputs; i++) {
                EditorGUILayout.BeginHorizontal();
                bool initValue = _rbBaseInput.GetValueBit(i);
                initValue = EditorGUILayout.Toggle(_rollbackManager.inputQueue.GetActionName(i), initValue);
                _rbBaseInput.SetOrClearBit(i, initValue);
                EditorGUILayout.EndHorizontal();
            }
            
            //Correction of inputs
            if (GUILayout.Button("Correct Inputs")) {
                if (_rollbackManager != null) {
                    
                    RollbackInputBaseActions[] rbInputs = new RollbackInputBaseActions[_numFramesToSimulate];
                    for (int i = 0; i < _numFramesToSimulate; i++) {
                        rbInputs[i] = _rbBaseInput;
                    }
                    
                    _rollbackManager.inputQueue.CorrectInputs(_controllerId, _numFramesToSimulate, rbInputs);
                    _rollbackManager.ReSimulate(_numFramesToSimulate);
                }
            }
        }
    }
}
