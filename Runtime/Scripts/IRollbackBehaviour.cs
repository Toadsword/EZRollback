using System;
using UnityEngine;

namespace Packages.EZRollback.Runtime.Scripts {
    [Serializable]
    public abstract class IRollbackBehaviour : MonoBehaviour {

        public bool registered = false;

        public void Start() {
            RollbackManager.RegisterRollbackBehaviour(this);
        }

        public void OnDestroy() {
            RollbackManager.UnregisterRollbackBehaviour(this);
        }
        
        //Work instead of FixedUpdate
        public abstract void Simulate();

        public abstract void GoToFrame(int frameNumber);

        public abstract void DeleteFrames(int numFramesToDelete, bool firstFrames);

        public abstract void SaveFrame();
    }
}