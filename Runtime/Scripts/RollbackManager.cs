using UnityEngine;

namespace EZRollback.Core {
    public class RollbackManager : MonoBehaviour {
        public bool doRollback = false;

        [SerializeField] RollbackComponent[] rollbackElements;
        int maxFrameNum = 0;
        int currentFrameNum = 0;

        public int GetCurrentFrameNum() {
            return currentFrameNum;
        }

        public int GetMaxFramesNum() {
            return maxFrameNum;
        }

        // Start is called before the first frame update
        void Start() {
            rollbackElements = GameObject.FindObjectsOfType<RollbackComponent>();
            currentFrameNum = 0;
        }

        // Update is called once per frame
        void FixedUpdate() {
            if (doRollback) {
                GoToFrame(currentFrameNum - 1);
            } else {
                SaveCurrentFrame();
            }
        }

        private void SetCurrentFrameAsLastRegistered() {
            if (currentFrameNum != maxFrameNum) {
                foreach (RollbackComponent rollbackElement in rollbackElements) {
                    rollbackElement.GoToFrame(currentFrameNum, true);
                }
            }
        }
        
        public void GoToFrame(int frameNumber, bool deleteFrames = true) {
            if (maxFrameNum < frameNumber)
                return;

            foreach (RollbackComponent rollbackElement in rollbackElements) {
                rollbackElement.GoToFrame(currentFrameNum, deleteFrames);
            }

            currentFrameNum = frameNumber;
            if (deleteFrames) {
                maxFrameNum = currentFrameNum;
            }
        }


        public void SaveCurrentFrame() {
            //If we try to save a frame while in restored state, we delete the first predicted future
            SetCurrentFrameAsLastRegistered();

            foreach (RollbackComponent rollbackElement in rollbackElements) {
                rollbackElement.SaveCurrentFrame();
            }

            currentFrameNum++;
            maxFrameNum = currentFrameNum;
        }

        // From the currently loaded frame, simutate x frames by calling fixed update on all the rollbackElements
        public void Simulate(int numFrames) {
            
            Debug.Log("Simulate !");
            Debug.Log("numFrames : " + numFrames.ToString());
            SetCurrentFrameAsLastRegistered();

            for (int i = 0; i < numFrames; i++) {
                Debug.Log(i);
                foreach (RollbackComponent rollbackElement in rollbackElements) {
                    rollbackElement.Simulate();
                    rollbackElement.SaveCurrentFrame();
                }
            }
            Debug.Log("End Simulate !");
        }
    }
}
