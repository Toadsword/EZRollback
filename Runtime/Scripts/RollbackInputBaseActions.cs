using System;
using UnityEngine;

/**
 * \brief Utility struct that accept all button press inputs, stored inputs in a single bit per actions.
 * It includes two axis : Horizontal and Vertical.
 */
[Serializable]
public struct RollbackInputBaseActions {

    const float AxisChangeRatio = 64.0f;
    
    [SerializeField] private sbyte horizontalValue;
    [SerializeField] private sbyte verticalValue;
    [SerializeField] private byte[] bits;
    
    /**
     * \brief Constructor of the class.
     * \param defaultSize Number of inputs the BaseActions should be able to handle.
     */
    public RollbackInputBaseActions(int defaultSize = 1) {
        bits = new byte[1 + defaultSize/8];
        horizontalValue = 0;
        verticalValue = 0;
    }

    /**
     * \brief Get the value of the input at the requested index.
     * \param i Index of the input.
     * \return true if input is pressed, false otherwise.
     */
    public bool GetValueBit(int i) {
        return (bits[i/8] & (1 << (i%8))) != 0;
    }

    /**
     * \brief Utility function to choose between setting or clearing a bit.
     * \param i Index of the input.
     * \param setIt true if input is pressed, false otherwise.
     */
    public void SetOrClearBit(int i, bool setIt) {
        if (setIt) 
            SetBit(i);
        else 
            ClearBit(i);
    }
    
    /**
     * \brief Set the value of the input at the requested index. (Set to 1)
     * \param i Index of the input.
     */
    public void SetBit(int i) {
        bits[i/8] |= (byte)(1 << (i%8));
    }
    
    /**
     * \brief Clear the value of the input at the requested index. (Set to 0)
     * \param i Index of the input.
     */
    public void ClearBit(int i) {
        bits[i/8] &= (byte)~(1 << (i%8));
    }
    
    /**
     * \brief Set the verticalAxis from a float value
     * \param value
     */
    public void SetVerticalAxis(float value) {
        verticalValue = TransformAxisValueToSByte(value);
    }
    
    /**
     * \brief Get the verticalAxis value in float
     * \return value
     */
    public float GetVerticalAxis() {
        return TransformSByteToAxisValue(verticalValue);
    }
    
    /**
     * \brief Get the Horizontal Axis value in float
     * \param value
     */
    public void SetHorizontalAxis(float value) {
        horizontalValue = TransformAxisValueToSByte(value);
    }
    
    /**
     * \brief Get the Horizontal axis value in float
     * \return value
     */
    public float GetHorizontalAxis() {
        return TransformSByteToAxisValue(horizontalValue);
    }

    /**
     * \brief Transform a float value (axis) to an sbyte
     * \return value
     */
    private sbyte TransformAxisValueToSByte(float value) {
        return (sbyte) (value * AxisChangeRatio);
    }
    
    /**
     * \brief Transform a sbyte value to a float (axis)
     * \param value Value to transform
     * \return value Result
     */
    private float TransformSByteToAxisValue(sbyte value) {
        return value / AxisChangeRatio;
    }
}
