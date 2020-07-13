using System;
using UnityEngine;

namespace Packages.EZRollback.Runtime.Scripts {

    public class RollbackManager : MonoBehaviour {

        public static RollbackManager _instance;

        void Awake() {
            if (_instance == null) {
                _instance = this;
            } else {
                Destroy(this);
            }
        }

        public bool doRollback = false;
        public bool bufferRestriction = false;
        
        public Action prepareInputDelegate;
        
        public static Action simulateDelegate;
        public static Action saveDelegate;
        public static Action saveInputDelegate;
        public static Action<int> goToFrameDelegate;
        public static Action<int, bool> deleteFramesDelegate;
        public static Action<int, bool> deleteFramesInputDelegate;

        public InputQueue inputQueue;
        
        [SerializeField] int _maxFrameNum = 0;
        [SerializeField] int _displayedFrameNum = 0;

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
            saveInputDelegate += inputQueue.SaveFrame;
            goToFrameDelegate += inputQueue.GoToFrame;
            deleteFramesInputDelegate += inputQueue.DeleteFrames;
        }
        void OnDisable() {
            prepareInputDelegate -= inputQueue.UpdateInputStatus;
            saveInputDelegate -= inputQueue.SaveFrame;
            goToFrameDelegate -= inputQueue.GoToFrame;
            deleteFramesInputDelegate -= inputQueue.DeleteFrames;
        }
        
        public static void RegisterRollbackBehaviour(IRollbackBehaviour rbBehaviour) {
            if (rbBehaviour.registered)
                return;
            
            simulateDelegate += rbBehaviour.Simulate;
            saveDelegate += rbBehaviour.SaveFrame;
            goToFrameDelegate += rbBehaviour.SetValueFromFrameNumber;
            deleteFramesDelegate += rbBehaviour.DeleteFrames;
            
            rbBehaviour.registered = true;
        }
        
        public static void UnregisterRollbackBehaviour(IRollbackBehaviour rbBehaviour) {
            if (!rbBehaviour.registered)
                return;
            
            simulateDelegate -= rbBehaviour.Simulate;
            saveDelegate -= rbBehaviour.SaveFrame;
            goToFrameDelegate -= rbBehaviour.SetValueFromFrameNumber;
            deleteFramesDelegate -= rbBehaviour.DeleteFrames;
            
            rbBehaviour.registered = false;
        }

        // Start is called before the first frame update
        void Start() {
            _displayedFrameNum = 0;
            _maxFrameNum = 0;
        }

        // Update is called once per frame
        void FixedUpdate() {
            if(deleteFramesDelegate == null)
                return;
            
            if (doRollback) {
                GoBackInFrames(1);
            } else {
                Simulate(1);
                if (bufferRestriction) {
                    ManageBufferSize();
                }
            }
        }

        private void SetCurrentFrameAsLastRegistered(bool deleteInputs = true) {
            if (_displayedFrameNum != _maxFrameNum) {
                //Apply set
                deleteFramesDelegate.Invoke(_maxFrameNum - _displayedFrameNum, false);
                if (deleteInputs) {
                    deleteFramesInputDelegate.Invoke(_maxFrameNum - _displayedFrameNum, false);
                }
                _maxFrameNum = _displayedFrameNum;
            }
        }

        public void GoBackInFrames(int numFrames, bool deleteFrames = true, bool inputsToo = true) {
            SetValueFromFrameNumber(_displayedFrameNum - numFrames, deleteFrames, inputsToo);
        }
        
        public void SetValueFromFrameNumber(int frameNumber, bool deleteFrames = true, bool inputsToo = true) {
            if (_maxFrameNum < frameNumber || frameNumber < 0)
                return;

            //Apply Goto
            goToFrameDelegate.Invoke(frameNumber);

            _displayedFrameNum = frameNumber;
            if (deleteFrames) {
                SetCurrentFrameAsLastRegistered(inputsToo);
                _maxFrameNum = _displayedFrameNum;
            }
        }


        public void SaveCurrentFrame(bool inputsToo = true) {
            //If we try to save a frame while in restored state, we delete the first predicted future
            SetCurrentFrameAsLastRegistered(inputsToo);

            //Apply save
            saveDelegate.Invoke();
            if (inputsToo) {
                saveInputDelegate.Invoke();
            }
            
            _displayedFrameNum++;
            _maxFrameNum = _displayedFrameNum;
        }

        // From the currently loaded frame, simutate x frames by calling fixed update on all the rollbackElements
        public void Simulate(int numFrames, bool inputsToo = true) {
            SetCurrentFrameAsLastRegistered(inputsToo);

            for (int i = 0; i < numFrames; i++) {
                //Apply simulate and save for each frames
                if (inputsToo){
                    prepareInputDelegate.Invoke();
                }
                simulateDelegate.Invoke();
                SaveCurrentFrame(inputsToo);
            }
        }

        public void ReSimulate(int numFrames) {
            GoBackInFrames(numFrames, true, false);
            Simulate(numFrames, false);
        }

        private void ManageBufferSize() {
            if (_bufferSize > 0 && _maxFrameNum > _bufferSize) {
                deleteFramesDelegate.Invoke(1, true);
                deleteFramesInputDelegate.Invoke(1, true);

                _maxFrameNum = _bufferSize;
                _displayedFrameNum = _maxFrameNum;
            }
        }
    }
}
