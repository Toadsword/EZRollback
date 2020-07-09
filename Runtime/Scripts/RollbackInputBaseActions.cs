using System;
using UnityEngine;

[Serializable]
public struct RollbackInputBaseActions {
    public sbyte horizontalValue;
    public sbyte verticalValue;
    public byte[] bits;
 
    public RollbackInputBaseActions(int defaultSize = 1) {
        bits = new byte[1 + defaultSize/8];
        horizontalValue = 0;
        verticalValue = 0;
    }

    public bool GetValueBit(int i) { return (bits[i/8] & (1 << (i%8))) != 0; }
        
    public void SetOrClearBit(int i, bool setIt) {
        if (setIt) 
            SetBit(i);
        else 
            ClearBit(i);
    }
    public void SetBit(int i) { bits[i/8] |= (byte)(1 << (i%8)); }

    public void ClearBit(int i) { bits[i/8] &= (byte)~(1 << (i%8)); }
}
