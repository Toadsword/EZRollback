using System;
using UnityEngine;

namespace EZRollback.Core.Component {
    [Serializable]
    public abstract class IRollbackBehaviour : MonoBehaviour {

        public abstract void Simulate();

        public abstract void GoToFrame(int frameNumber);

        public abstract void DeleteFrames(int fromFrame, int numFramesToDelete);

        public abstract void SaveFrame();
    }
}