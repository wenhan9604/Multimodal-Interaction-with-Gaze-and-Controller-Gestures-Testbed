using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public enum TargetDestinationAngDist
{
    close = 20,
    mid = 30,
    far = 40
}

public enum TargetSize
{
    big,
    medium,
    small
}

public enum PathCurvature
{
    up,
    down
}

public static class TargetUtils
{
    /// <summary>
    /// Get TargetSize
    /// </summary>
    /// <param name="targetSize"></param>
    /// <returns></returns>
    public static float GetTargetSize(TargetSize targetSize)
    {
        switch (targetSize)
        {
            case TargetSize.big:
                return 1f;
            case TargetSize.medium:
                return 0.5f;
            case TargetSize.small:
                return 0.25f;
            default:
                Debug.Log("TargetSize is invalid, couldnt change targetSize");
                return 1f;
        }
    }

    /// <summary>
    /// Converts from string to TargetSize enum
    /// </summary>
    /// <param name="targetSize">Input only "big", "medium", "small"</param>
    /// <returns> TargetSize enum: big, medium , small</returns>
    public static TargetSize ConvertToTargetSize(string targetSize)
    {
        TargetSize returnable = TargetSize.big;
        switch(targetSize)
        {
            case "big":
                returnable = TargetSize.big;
                break;
            case "medium":
                returnable = TargetSize.medium;
                break;
            case "small":
                returnable = TargetSize.small;
                break;
            default:
                Debug.Log("Errorneous input, no such targetsize");
                break;
        }
        return returnable;
    }

    /// <summary>
    /// Converts from input to TargetDestinationAngDist enum
    /// </summary>
    /// <param name="targetSize">Input only "far", "mid", "close"</param>
    /// <returns> TargetDestinationAngularDistance enum: far, mid or close</returns>
    public static TargetDestinationAngDist ConvertToTargetDestAngularDistance(string input)
    {
        TargetDestinationAngDist returnable = TargetDestinationAngDist.far;
        switch (input)
        {
            case "far":
                returnable = TargetDestinationAngDist.far;
                break;
            case "mid":
                returnable = TargetDestinationAngDist.mid;
                break;
            case "close":
                returnable = TargetDestinationAngDist.close;
                break;
            default:
                Debug.Log("Errorneous input, no such TargetDestinationAngDist");
                break;
        }
        return returnable;
    }

    /// <summary>
    /// Converts from input to PathCurvature enum
    /// </summary>
    /// <param name="targetSize">Input only "up" or "down" </param>
    /// <returns> PathCurvature enum: up or down</returns>
    public static PathCurvature ConvertToPathCurvatureDirection(string input)
    {
        PathCurvature returnable = PathCurvature.up;
        switch (input)
        {
            case "up":
                returnable = PathCurvature.up;
                break;
            case "down":
                returnable = PathCurvature.down;
                break;
            default:
                Debug.Log("Errorneous input, no such PathCurvature");
                break;
        }
        return returnable;
    }

    /// <summary>
    /// Get a list of equal number of true and false
    /// </summary>
    /// <param name="listCount">Must be an even number</param>
    /// <returns></returns>
    public static List<bool> GetEqualTrueFalseList(int listCount)
    {
        List<bool> returnable = new List<bool>(listCount);

        if (listCount % 2 == 1)
        {
            Debug.Log("Error: ListCount is not an even number.");
            return returnable;
        }
        else
        {
            for (int i = 0; i < listCount / 2; i++)
            {
                returnable[i] = false;
            }
            for (int ii = listCount / 2; ii < listCount; ii++)
            {
                returnable[ii] = true;
            }
            return returnable;
        }
    }

}
