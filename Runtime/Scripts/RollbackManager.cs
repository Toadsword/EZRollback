using System;
using UnityEngine;

namespace Packages.EZRollback.Runtime.Scripts {

/**
 * \brief The RollbackManager is main rollback system present in the scene. It is required to allow your scripts to rewind in time.
 */
    public class RollbackManager : MonoBehaviour {

    /**
     * \brief _instance is a static variable, allowing any component to access it when needed without the need of a reference
     */
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
        
        /** ----------------- STATICS -------------------- **/
        /**
         * Delegates are created to make a callback to all registered functions when rewinding in time or going forward.
         */
        public Action prepareInputDelegate;
        
        public static Action simulateDelegate;
        public static Action saveDelegate;
        public static Action saveInputDelegate;
        public static Action<int> goToFrameDelegate;
        public static Action<int, bool> deleteFramesDelegate;
        public static Action<int, bool> deleteFramesInputDelegate;

        public static RollbackInputManager rbInputManager;
        
        [SerializeField] int _maxFrameNum = 0;
        [SerializeField] int _displayedFrameNum = 0;

        [SerializeField] public int _bufferSize = -1;

        /* ----- Getter and Setters ------ */
        public RollbackInputManager GetRBInputManager() {
            return rbInputManager;
        }
        
        public int GetDisplayedFrameNum() {
            return _displayedFrameNum;
        }

        public int GetMaxFramesNum() {
            return _maxFrameNum;
        }

        IRollbackBehaviour[] _rbRegisteredBehaviours;
        void OnEnable() {
            rbInputManager = GetComponent<RollbackInputManager>();

            //Register the inputs callbacks
            prepareInputDelegate += rbInputManager.UpdateInputStatus;
            saveInputDelegate += rbInputManager.SaveFrame;
            goToFrameDelegate += rbInputManager.SetValueFromFrameNumber;
            deleteFramesInputDelegate += rbInputManager.DeleteFrames;
        }
        void OnDisable() {
            //Unregister the inputs callbacks
            prepareInputDelegate -= rbInputManager.UpdateInputStatus;
            saveInputDelegate -= rbInputManager.SaveFrame;
            goToFrameDelegate -= rbInputManager.SetValueFromFrameNumber;
            deleteFramesInputDelegate -= rbInputManager.DeleteFrames;
        }

        /**
         * \brief Register an IRollbackBehaviour to the manager's rollback callback
         * \param rbBehaviour IRollbackBehaviour to register
         */
        public static void RegisterRollbackBehaviour(IRollbackBehaviour rbBehaviour) {
            if (rbBehaviour.registered)
                return;
            
            simulateDelegate += rbBehaviour.Simulate;
            saveDelegate += rbBehaviour.SaveFrame;
            goToFrameDelegate += rbBehaviour.SetValueFromFrameNumber;
            deleteFramesDelegate += rbBehaviour.DeleteFrames;
            
            rbBehaviour.registered = true;
        }
        
        /**
         * \brief Unregister an IRollbackBehaviour from the manager's rollback callback.
         * \param rbBehaviour IRollbackBehaviour to unregister
         */
        public static void UnregisterRollbackBehaviour(IRollbackBehaviour rbBehaviour) {
            if (!rbBehaviour.registered)
                return;
            
            simulateDelegate -= rbBehaviour.Simulate;
            saveDelegate -= rbBehaviour.SaveFrame;
            goToFrameDelegate -= rbBehaviour.SetValueFromFrameNumber;
            deleteFramesDelegate -= rbBehaviour.DeleteFrames;
            
            rbBehaviour.registered = false;
        }

        void Start() {
            _displayedFrameNum = 0;
            _maxFrameNum = 0;
        }

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

        /**
         * \brief Setting the current frame as last registered, and delete all future frames currently registered
         * \param deleteInputs Also delete input frames if true, doesn't otherwise
         */
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

        /**
         * \brief Get back in x frames
         * \param numFrames Number of frames to rollback
         * \param deleteFrames true if we want to delete the frames we rewind.
         * \param inputsToo along with deleting frames, delete the input frames if true.
         */
        public void GoBackInFrames(int numFrames, bool deleteFrames = true, bool inputsToo = true) {
            SetValueFromFrameNumber(_displayedFrameNum - numFrames, deleteFrames, inputsToo);
        }
        
        /**
         * \brief Get back to a specific frame number
         * \param frameNumber Frame number wanted
         * \param deleteFrames true if we want to delete the frames we rewind.
         * \param inputsToo along with deleting frames, delete the input frames if true.
         */
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

        /**
         * \brief Save the value of the current frame
         * \param inputsToo True to save the current state of the inputs, false otherwise. False is used when rewinding frames with calculated new inputs
         */
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

        /**
         * \brief Simulate a certain number of frames
         * \param numFrames Number of frames to simulate
         * \param inputsToo True to save the current state of the inputs, false otherwise. False is used when rewinding frames with calculated new inputs
         */
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
        
        /**
         * \brief Simulate a certain number of frames, while taking in count the already defined inputs of the players
         * \param numFrames Number of frames to resimulate
         */
        public void ReSimulate(int numFrames) {
            GoBackInFrames(numFrames, true, false);
            Simulate(numFrames, false);
        }

        /**
         * \brief Resize the frame buffer if it exceeds its size
         */
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
