using System;
using Packages.EZRollback.Runtime.Scripts;
using UnityEditor;
using UnityEngine;

namespace Packages.EZRollback.Editor {
    [Serializable]
    public class RollbackTool : EditorWindow {
        public static RollbackInformation rollbackInformation = new RollbackInformation();

        RollbackManager _rollbackManager;
        
        bool openedObjectList = false;
        int currentWantedSize = 0;

        int numFramesToSimulate = 0;
        
        [MenuItem("RollbackTool/Information")]
        public static void ShowWindow() {
            GetWindow(typeof(RollbackTool));
        }
        
        RollbackTool() {
            EditorApplication.playModeStateChanged += LogPlayModeState;
        }

        void OnGUI() {
            DisplayRollbackEditionButtons();

            DisplayInformations();

            DisplaySimulateOptions();
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
            if (UnityEditor.EditorApplication.isPlaying && _rollbackManager != null) {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("CurrentFrame", GUILayout.Width(100));
                int newFrameNum = (int)GUILayout.HorizontalSlider(_rollbackManager.GetDisplayedFrameNum(), 1, (_rollbackManager.GetMaxFramesNum() - 1));

                if (newFrameNum != _rollbackManager.GetDisplayedFrameNum()) {
                    _rollbackManager.GoToFrame(newFrameNum, false);
                    UnityEditor.EditorApplication.isPaused = true;
                }

                EditorGUILayout.EndHorizontal();
            
                GUILayout.Label("Current frame number : " + (_rollbackManager.GetDisplayedFrameNum() - 1) + " / " + (_rollbackManager.GetMaxFramesNum() - 1));
            }
        }

        private void DisplaySimulateOptions() {
            
            EditorGUILayout.BeginHorizontal();
            numFramesToSimulate = EditorGUILayout.IntField("Num frames to simulate : ", numFramesToSimulate);
            if (numFramesToSimulate < 0)
                numFramesToSimulate = 0;
            if (GUILayout.Button("Simulate !")) {
                if (_rollbackManager != null) {
                    _rollbackManager.Simulate(numFramesToSimulate);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
