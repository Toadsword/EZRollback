using System;
using System.IO;
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
                EditorGUILayout.Separator();
                DisplayInformations();
                
                EditorGUILayout.Separator();
                DisplaySimulateOptions();
                
                EditorGUILayout.Separator();
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
                    break;
            }
        }

        private void DisplayRollbackEditionButtons() {
            EditorGUILayout.BeginHorizontal();
            
            
            if (GUILayout.Button("<=", GUILayout.Width(30), GUILayout.Height(20))) {
                _rollbackManager.GoToFrame(1, false);
                UnityEditor.EditorApplication.isPaused = true;
            }
            
            if (GUILayout.Button("<", GUILayout.Width(30), GUILayout.Height(20))) {
                _rollbackManager.GoToFrame(_rollbackManager.GetDisplayedFrameNum() - 1, false);
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
                    _rollbackManager.GoToFrame(_rollbackManager.GetDisplayedFrameNum(), true);
                    UnityEditor.EditorApplication.isPaused = false;
                }
            } else {
                if (GUILayout.Button("Pause", GUILayout.Width(100), GUILayout.Height(20))) {
                    UnityEditor.EditorApplication.isPaused = true;
                }
            }

            if (GUILayout.Button(">", GUILayout.Width(30), GUILayout.Height(20))) {
                _rollbackManager.GoToFrame(_rollbackManager.GetDisplayedFrameNum() + 1, false);
            }
            
            if (GUILayout.Button("=>", GUILayout.Width(30), GUILayout.Height(20))) {
                _rollbackManager.GoToFrame(_rollbackManager.GetMaxFramesNum(), false);
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DisplayInformations() {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("CurrentFrame", GUILayout.Width(100));
            int newFrameNum = (int) GUILayout.HorizontalSlider(_rollbackManager.GetDisplayedFrameNum(), 1,
                (_rollbackManager.GetMaxFramesNum() - 1));

            if (newFrameNum != _rollbackManager.GetDisplayedFrameNum()) {
                _rollbackManager.GoToFrame(newFrameNum, false);
                UnityEditor.EditorApplication.isPaused = true;
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Label("Current frame number : " + (_rollbackManager.GetDisplayedFrameNum() - 1) + " / " + (_rollbackManager.GetMaxFramesNum() - 1));
        }

        private void DisplaySimulateOptions() {
            
            EditorGUILayout.BeginHorizontal();
            _numFramesToSimulate = EditorGUILayout.IntField("Num frames to simulate : ", _numFramesToSimulate);
            if (_numFramesToSimulate < 0)
                _numFramesToSimulate = 0;
            if (GUILayout.Button("Simulate !")) {
                if (_rollbackManager != null) {
                    _rollbackManager.Simulate(_numFramesToSimulate);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DisplaySimulateInput() {
            
            int oldnumOfInputs = _numOfInputs;
            _numOfInputs = EditorGUILayout.IntField("NumOfInputs : ", _numOfInputs);

            if (_numOfInputs != oldnumOfInputs) {
                _rbBaseInput = new RollbackInputBaseActions(1 + _numOfInputs / 8);
            }

            EditorGUILayout.IntSlider("ControllerId : ", _controllerId, 0, _rollbackManager.GetMaxFramesNum());
            
            //Vertical input
            float verticalValue = RollbackManager.inputQueue.TransformSByteToAxisValue(_rbBaseInput.verticalValue);
            verticalValue = EditorGUILayout.Slider("Vertical", verticalValue, -1f, 1f);
            _rbBaseInput.verticalValue = RollbackManager.inputQueue.TransformAxisValueToSByte(verticalValue);
            
            //Vertical input
            float horizontalValue = RollbackManager.inputQueue.TransformSByteToAxisValue(_rbBaseInput.horizontalValue);
            horizontalValue = EditorGUILayout.Slider("Horizontal", horizontalValue, -1f, 1f);
            _rbBaseInput.horizontalValue = RollbackManager.inputQueue.TransformAxisValueToSByte(horizontalValue);

            for (int i = 0; i < _numOfInputs; i++) {
                EditorGUILayout.BeginHorizontal();
                bool initValue = _rbBaseInput.GetValueBit(i);
                initValue = EditorGUILayout.Toggle(RollbackManager.inputQueue.GetActionName(i), initValue);
                _rbBaseInput.SetOrClearBit(i, initValue);
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Input to queue")) {
                if (_rollbackManager != null) {
                    RollbackManager.inputQueue.AddInput(_controllerId);
                }
            }
            
        }
    }
}
