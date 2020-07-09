using System;
using UnityEngine;

namespace Packages.EZRollback.Runtime.Scripts {

    public class RollbackManager : MonoBehaviour {
        public bool doRollback = false;
        public bool bufferRestriction = false;
        
        public Action prepareInputDelegate;
        
        public static Action simulateDelegate;
        public static Action saveDelegate;
        public static Action<int> goToFrameDelegate;
        public static Action<int, bool> deleteFramesDelegate;

        public static InputQueue inputQueue;
        
        [SerializeField] int _maxFrameNum = 0;
        [SerializeField] int _displayedFrameNum = -1;

        [SerializeField] int _bufferSize = -1;

        public int GetDisplayedFrameNum() {
            return _displayedFrameNum;
        }

        public int GetMaxFramesNum() {
            return _maxFrameNum;
        }

        public int GetBufferSize(int newValue) {
            return _bufferSize;
        }
        
        public void SetBufferSize(int newValue) {
            _bufferSize = newValue;
        }

        IRollbackBehaviour[] _rbRegisteredBehaviours;
        void OnEnable() {
            inputQueue = GetComponent<InputQueue>();

            prepareInputDelegate += inputQueue.UpdateInputStatus;
            saveDelegate += inputQueue.SaveFrame;
            goToFrameDelegate += inputQueue.GoToFrame;
            deleteFramesDelegate += inputQueue.DeleteFrames;
        }
        void OnDisable() {
            prepareInputDelegate -= inputQueue.UpdateInputStatus;
            saveDelegate -= inputQueue.SaveFrame;
            goToFrameDelegate -= inputQueue.GoToFrame;
            deleteFramesDelegate -= inputQueue.DeleteFrames;
        }
        
        public static void RegisterRollbackBehaviour(IRollbackBehaviour rbBehaviour) {
            if (rbBehaviour.registered)
                return;
            
            simulateDelegate += rbBehaviour.Simulate;
            saveDelegate += rbBehaviour.SaveFrame;
            goToFrameDelegate += rbBehaviour.GoToFrame;
            deleteFramesDelegate += rbBehaviour.DeleteFrames;
            
            Debug.Log("Registered : " + rbBehaviour.name);
            rbBehaviour.registered = true;
        }
        
        public static void UnregisterRollbackBehaviour(IRollbackBehaviour rbBehaviour) {
            if (!rbBehaviour.registered)
                return;
            simulateDelegate -= rbBehaviour.Simulate;
            saveDelegate -= rbBehaviour.SaveFrame;
            goToFrameDelegate -= rbBehaviour.GoToFrame;
            deleteFramesDelegate -= rbBehaviour.DeleteFrames;
            
            Debug.Log("UNREGISTERED : " + rbBehaviour.name);
            rbBehaviour.registered = false;
        }

        // Start is called before the first frame update
        void Start() {
            _displayedFrameNum = -1;
            _maxFrameNum = 0;
        }

        // Update is called once per frame
        void FixedUpdate() {
            if (doRollback) {
                GoBackInFrames(1);
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

        public void GoBackInFrames(int numFrames, bool deleteFrames = true) {
            GoToFrame(_displayedFrameNum - numFrames, deleteFrames);
        }
        
        public void GoToFrame(int frameNumber, bool deleteFrames = true) {
            if (_maxFrameNum <= frameNumber || frameNumber < 0)
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
            if (_bufferSize > 0 && _maxFrameNum > _bufferSize) {
                deleteFramesDelegate.Invoke(1, true);

                _maxFrameNum = _bufferSize;
                _displayedFrameNum = _maxFrameNum;
            }
        }
    }
}
