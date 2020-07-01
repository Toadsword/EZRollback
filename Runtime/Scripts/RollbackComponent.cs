using System;
using System.Collections.Generic;
using UnityEngine;

namespace EZRollback.Core {
[Serializable]
    public abstract class RollbackComponent
    {
        [SerializeField] public List<string> rollbackedComponentsName = new List<string>();
        [SerializeField] public List<bool> doRollbackComponents = new List<bool>();
        
        public void RegisterNetRollbackElement() {
            
        }
    }
}
