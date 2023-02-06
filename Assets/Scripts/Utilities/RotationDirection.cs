using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum RotationDirection
{ up = 0, left = 1, right = 2, down = 3, zClockwise = 4, zCounterClockwise = 5 }

public enum RotationDirectionPair
{
    leftAndRight,
    upAndDown,
    clockwiseAndAntiClockwise
}

public static class RotationDirectionUtility
{
    public static List<T> GetAllEnumValues<T>() 
    {
        T[] array = (T[])Enum.GetValues(typeof(T));
        List<T> returnable = new List<T>(array);
        return returnable; 
    }

    /// <summary>
    /// Returns the oppsite direction of the input argument
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static RotationDirection GetOppositeDirection(RotationDirection direction)
    {
        RotationDirection oppDirection = RotationDirection.up;
        switch(direction)
        {
            case RotationDirection.up:
                oppDirection = RotationDirection.down;
                break;
            case RotationDirection.down:
                oppDirection = RotationDirection.up;
                break;
            case RotationDirection.left:
                oppDirection = RotationDirection.right;
                break;
            case RotationDirection.right:
                oppDirection = RotationDirection.left;
                break;
            case RotationDirection.zClockwise:
                oppDirection = RotationDirection.zCounterClockwise;
                break;
            case RotationDirection.zCounterClockwise:
                oppDirection = RotationDirection.zClockwise;
                break;
        }
        return oppDirection;
    }

    /// <summary>
    /// Returns a Vector3 that is the rotational axis for the rotation direction.
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static Vector3 ConvertRotationDirectionToVector3(RotationDirection direction)
    {
        switch (direction)
        {
            case RotationDirection.up:
                return Vector3.right;

            case RotationDirection.left:
                return Vector3.up;

            case RotationDirection.right:
                return Vector3.down;

            case RotationDirection.down:
                return Vector3.left;

            case RotationDirection.zClockwise:
                return Vector3.forward;

            case RotationDirection.zCounterClockwise:
                return Vector3.back;

            default:
                return Vector3.zero;
        }
    }

    public static List<RotationDirection> GetRotationDirectionsFromRotDirPairs(List<RotationDirectionPair> pairs)
    {
        List<RotationDirection> returnable = new List<RotationDirection>();

        foreach (var pair in pairs)
        {
            switch (pair)
            {
                case RotationDirectionPair.leftAndRight:
                    returnable.Add(RotationDirection.left);
                    returnable.Add(RotationDirection.right);
                    break;
                case RotationDirectionPair.upAndDown:
                    returnable.Add(RotationDirection.up);
                    returnable.Add(RotationDirection.down);
                    break;
                case RotationDirectionPair.clockwiseAndAntiClockwise:
                    returnable.Add(RotationDirection.zClockwise);
                    returnable.Add(RotationDirection.zCounterClockwise);
                    break;
            }
        }

        return returnable;
    }

    public static RotationDirectionPair GetRotationDirectionPair(RotationDirection rotationDirection)
    {
        if(rotationDirection == RotationDirection.left || rotationDirection == RotationDirection.right)
        {
            return RotationDirectionPair.leftAndRight;
        }
        else if (rotationDirection == RotationDirection.up || rotationDirection == RotationDirection.down)
        {
            return RotationDirectionPair.upAndDown;
        }
        else 
        {
            return RotationDirectionPair.clockwiseAndAntiClockwise;
        }

    }

    public static List<RotationDirection> GetListOfRotationDirectionPair(RotationDirection rotationDirection)
    {
        List<RotationDirection> returnable = new List<RotationDirection>();

        if (rotationDirection == RotationDirection.left || rotationDirection == RotationDirection.right)
        {
            returnable.Add(RotationDirection.left);
            returnable.Add(RotationDirection.right);
        }
        else if (rotationDirection == RotationDirection.up || rotationDirection == RotationDirection.down)
        {
            returnable.Add(RotationDirection.up);
            returnable.Add(RotationDirection.down);
        }
        else if (rotationDirection == RotationDirection.zClockwise || rotationDirection == RotationDirection.zCounterClockwise)
        {
            returnable.Add(RotationDirection.zClockwise);
            returnable.Add(RotationDirection.zCounterClockwise);
        }
        return returnable;
    }

}
