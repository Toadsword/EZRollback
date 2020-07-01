using System;
using System.Collections.Generic;
using UnityEngine;

namespace EZRollback.Core {
    public class RollbackManager : MonoBehaviour {
        public bool doRollback = false;
        public bool bufferRestriction = false;
        
        public Action simulateDelegate;
        public Action saveDelegate;
        public Action<int> goToFrameDelegate;
        public Action<int, int> deleteFramesDelegate;

        [SerializeField] int maxFrameNum = 0;
        [SerializeField] int currentFrameNum = 0;

        [SerializeField] int bufferSize = -1;
        
        public int GetCurrentFrameNum() {
            return currentFrameNum;
        }

        public int GetMaxFramesNum() {
            return maxFrameNum;
        }

        // Start is called before the first frame update
        void Start() {
            IRollbackBehaviour[] rbBehaviours = GameObject.FindObjectsOfType<IRollbackBehaviour>();

            foreach (IRollbackBehaviour rbBehaviour in rbBehaviours) {
                RegisterNewRollbakcBehaviour(rbBehaviour);
            }
            
            currentFrameNum = 0;
            maxFrameNum = 0;
        }

        public void RegisterNewRollbakcBehaviour(IRollbackBehaviour rbBehaviour) {
            simulateDelegate += rbBehaviour.Simulate;
            saveDelegate += rbBehaviour.SaveFrame;
            goToFrameDelegate += rbBehaviour.GoToFrame;
            deleteFramesDelegate += rbBehaviour.DeleteFrames;
        }

        // Update is called once per frame
        void FixedUpdate() {
            if (doRollback) {
                GoToFrame(currentFrameNum - 1);
            } else {
                Simulate(1);
                if (bufferRestriction) {
                    ManageBufferSize();
                }
            }
        }

        private void SetCurrentFrameAsLastRegistered() {
            if (currentFrameNum != maxFrameNum) {
                //Apply set
                deleteFramesDelegate.Invoke(currentFrameNum, maxFrameNum);
                maxFrameNum = currentFrameNum;
            }
        }
        
        public void GoToFrame(int frameNumber, bool deleteFrames = true) {
            
            if (maxFrameNum < frameNumber || frameNumber < 0)
                return;
            
            //Apply Goto
            goToFrameDelegate.Invoke(frameNumber);

            currentFrameNum = frameNumber;
            if (deleteFrames) {
                maxFrameNum = currentFrameNum;
            }
        }


        public void SaveCurrentFrame() {
            //If we try to save a frame while in restored state, we delete the first predicted future
            SetCurrentFrameAsLastRegistered();

            //Apply save
            saveDelegate.Invoke();
            
            currentFrameNum++;
            maxFrameNum = currentFrameNum;
        }

        // From the currently loaded frame, simutate x frames by calling fixed update on all the rollbackElements
        public void Simulate(int numFrames) {
            SetCurrentFrameAsLastRegistered();

            for (int i = 0; i < numFrames; i++) {
                //Apply simulate and save for each frames
                simulateDelegate.Invoke();
                SaveCurrentFrame();
            }
        }

        private void ManageBufferSize() {
            if (bufferSize > 0 && maxFrameNum > bufferSize) {
                deleteFramesDelegate.Invoke(0, maxFrameNum - bufferSize);

                maxFrameNum = bufferSize;
                currentFrameNum = maxFrameNum;
            }
        }
    }
}
