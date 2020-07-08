﻿using System;
using UnityEngine;

namespace Packages.EZRollback.Runtime.Scripts {

    public class RollbackManager : MonoBehaviour {
        public bool doRollback = false;
        public bool bufferRestriction = false;
        
        public Action prepareInputDelegate;
        
        public Action simulateDelegate;
        public Action saveDelegate;
        public Action<int> goToFrameDelegate;
        public Action<int, bool> deleteFramesDelegate;

        public static InputQueue inputQueue;
        
        [SerializeField] int _maxFrameNum = 0;
        [SerializeField] int _displayedFrameNum = 0;

        [SerializeField] int bufferSize = -1;

        public int GetDisplayedFrameNum() {
            return _displayedFrameNum;
        }

        public int GetMaxFramesNum() {
            return _maxFrameNum;
        }


        IRollbackBehaviour[] _rbRegisteredBehaviours;
        void OnEnable() {
            inputQueue = GetComponent<InputQueue>();
            
            _rbRegisteredBehaviours = GameObject.FindObjectsOfType<IRollbackBehaviour>();

            foreach (IRollbackBehaviour rbBehaviour in _rbRegisteredBehaviours) {
                simulateDelegate += rbBehaviour.Simulate;
                saveDelegate += rbBehaviour.SaveFrame;
                goToFrameDelegate += rbBehaviour.GoToFrame;
                deleteFramesDelegate += rbBehaviour.DeleteFrames;
            }
            
            prepareInputDelegate += inputQueue.PrepareInput;
            saveDelegate += inputQueue.SaveFrame;
            goToFrameDelegate += inputQueue.GoToFrame;
            deleteFramesDelegate += inputQueue.DeleteFrames;
        }

        void OnDisable() {
            foreach (IRollbackBehaviour rbBehaviour in _rbRegisteredBehaviours) {
                simulateDelegate -= rbBehaviour.Simulate;
                saveDelegate -= rbBehaviour.SaveFrame;
                goToFrameDelegate -= rbBehaviour.GoToFrame;
                deleteFramesDelegate -= rbBehaviour.DeleteFrames;
            }
            _rbRegisteredBehaviours = new IRollbackBehaviour[]{};
            
            prepareInputDelegate -= inputQueue.PrepareInput;
            saveDelegate -= inputQueue.SaveFrame;
            goToFrameDelegate -= inputQueue.GoToFrame;
            deleteFramesDelegate -= inputQueue.DeleteFrames;
        }

        // Start is called before the first frame update
        void Start() {
            
            _displayedFrameNum = 0;
            _maxFrameNum = 0;
        }

        // Update is called once per frame
        void FixedUpdate() {
            if (doRollback) {
                GoToFrame(_displayedFrameNum - 1);
            } else {
                Simulate(1);
                if (bufferRestriction) {
                    ManageBufferSize();
                }
            }
        }

        private void SetCurrentFrameAsLastRegistered() {
            if (_displayedFrameNum != _maxFrameNum) {
                //Apply set
                deleteFramesDelegate.Invoke(_maxFrameNum - _displayedFrameNum, false);
                _maxFrameNum = _displayedFrameNum;
            }
        }
        
        public void GoToFrame(int frameNumber, bool deleteFrames = true) {
            
            if (_maxFrameNum < frameNumber || frameNumber < 0)
                return;
            
            //Apply Goto
            goToFrameDelegate.Invoke(frameNumber);

            _displayedFrameNum = frameNumber;
            if (deleteFrames) {
                SetCurrentFrameAsLastRegistered();
                _maxFrameNum = _displayedFrameNum;
            }
        }


        public void SaveCurrentFrame() {
            //If we try to save a frame while in restored state, we delete the first predicted future
            SetCurrentFrameAsLastRegistered();

            //Apply save
            saveDelegate.Invoke();
            
            _displayedFrameNum++;
            _maxFrameNum = _displayedFrameNum;
        }

        // From the currently loaded frame, simutate x frames by calling fixed update on all the rollbackElements
        public void Simulate(int numFrames) {
            SetCurrentFrameAsLastRegistered();

            for (int i = 0; i < numFrames; i++) {
                //Apply simulate and save for each frames
                prepareInputDelegate.Invoke();
                simulateDelegate.Invoke();
                SaveCurrentFrame();
            }
        }

        private void ManageBufferSize() {
            if (bufferSize > 0 && _maxFrameNum > bufferSize) {
                deleteFramesDelegate.Invoke(1, true);

                _maxFrameNum = bufferSize;
                _displayedFrameNum = _maxFrameNum;
            }
        }
    }
}
