using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorInputUtility 
{
    /// <summary>
    /// Get input direction of color
    /// </summary>
    /// <param name="inputColor"></param>
    /// <param name="directionInput">Input only primary colors: Red, Blue, Green, Yellow </param>
    /// <returns></returns>
    public static bool TryGetColorDirectionInput(Color inputColor, out RotationDirection directionInput)
    {
        bool hasRotationDirection = false;

        if (inputColor == Color.red)
        {
            directionInput = RotationDirection.up;
            hasRotationDirection = true;
        }
        else if (inputColor == Color.green)
        {
            directionInput = RotationDirection.left;
            hasRotationDirection = true;
        }
        else if (inputColor == Color.blue)
        {
            directionInput = RotationDirection.down;
            hasRotationDirection = true;
        }
        else
        {
            directionInput = RotationDirection.right;
            hasRotationDirection = true;
        }
        return hasRotationDirection;
    }

    /// <summary>
    /// Get name of color in string format.
    /// </summary>
    /// <param name="inputColor">Input only primary colors: Red, Blue, Green, Yellow </param>
    /// <returns></returns>
    public static bool TryGetColorName(Color inputColor, out string colorName)
    {
        bool isColorNameTrue = false;

        if (inputColor == Color.red)
        {
            colorName = "Red";
            isColorNameTrue = true;
        }
        else if (inputColor == Color.green)
        {
            colorName = "Green";
            isColorNameTrue = true;
        }
        else if (inputColor == Color.blue)
        {
            colorName = "Blue";
            isColorNameTrue = true;
        }
        else if (inputColor == Color.yellow)
        {
            colorName = "Yellow";
            isColorNameTrue = true;
        }
        else
        {
            Debug.Log("Invalid color input");
            isColorNameTrue = false;
            colorName = "Black";
        }
        return isColorNameTrue;
    }

}
