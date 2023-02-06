using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Initializing a rotation cue. User will be required to instantiate the rotationcue gameobject and assign gameobject to be child of Target.
/// Initialize its rotation direction and rotation magnitude.
/// </summary>
public class RotationCue : MonoBehaviour
{
    #region Fields
    private GameObject rotationCueArrow;
    private GameObject textObject;
    private TextMesh textComponent;

    private RotationDirection rotationDirection;
    private int rotationMagnitude;

    #endregion

    #region Properties 

    public RotationDirection RotationDirection
    {
        get { return rotationDirection; }
        set
        {
            rotationDirection = value;
            OnUpdateDirection(value);
        }
    }

    public int RotationMagnitude
    {
        get { return rotationMagnitude; }
        set
        {
            rotationMagnitude = value;
            OnUpdateTextContent(value);
        }
    }

    #endregion

    private void Awake()
    {
        rotationCueArrow = transform.Find("RotCueArrow").gameObject;
        textObject = transform.Find("CueText").gameObject;
        if (textObject != null)
        {
            textComponent = textObject.GetComponent<TextMesh>();
        }
    }

    #region Operations

    /// <summary>
    /// Updates cue's local position, rotation and color based on its direction
    /// </summary>
    /// <param name="direction"></param>
    void OnUpdateDirection(RotationDirection direction)
    {
        Vector3 cuePosition = Vector3.zero;
        Quaternion cueRotation = Quaternion.identity;
        Color cueColor = Color.clear;
        Vector3 rotCueYAxisPos = Vector3.zero; //about y axis
        Vector3 rotCueXAxisPos = Vector3.zero; //about x axis

        switch(PrimaryTaskController.Instance.TargetObjectSize)
        {
            case TargetSize.big:
                rotCueXAxisPos = new Vector3(0.26f, 0, -0.01f);
                rotCueYAxisPos = new Vector3(0, 0.26f, -0.1f);
                break;
            case TargetSize.medium:
                rotCueXAxisPos = new Vector3(0.2f, 0, -0.05f);
                rotCueYAxisPos = new Vector3(-0.01f, 0.18f, -0.05f);
                break;
            case TargetSize.small:
                rotCueXAxisPos = new Vector3(0.12f, 0, -0.01f);
                rotCueYAxisPos = new Vector3(-0.01f, 0.08f, -0.01f);
                break;
        }

        switch (direction)
        {
            case RotationDirection.left:
                cuePosition = rotCueYAxisPos;
                cueRotation = Quaternion.Euler(0, 0, 90f);
                cueColor = Color.green;
                break;
            case RotationDirection.right:
                cuePosition = rotCueYAxisPos;
                cueRotation = Quaternion.Euler(0, 0, -90f);
                cueColor = Color.green;
                break;
            case RotationDirection.up:
                cuePosition = rotCueXAxisPos;
                cueRotation = Quaternion.identity;
                cueColor = Color.red;
                break;
            case RotationDirection.down:
                cuePosition = rotCueXAxisPos;
                cueRotation = Quaternion.Euler(0, 0, -180f);
                cueColor = Color.red;
                break;
        }
        transform.localPosition = cuePosition;
        if (rotationCueArrow != null)
        {
            rotationCueArrow.transform.localRotation = cueRotation;
            rotationCueArrow.GetComponent<MeshRenderer>().material.color = cueColor;
        }
    }

    /// <summary>
    /// Update text display based on magnitude
    /// </summary>
    /// <param name="magnitude"></param>
    void OnUpdateTextContent(int magnitude)
    {
        if (magnitude > 1)
        {
            if (textObject != null)
            {
                textObject.SetActive(true);
                textComponent.text = magnitude + "x";
            }
        }
    }

    #endregion
}
