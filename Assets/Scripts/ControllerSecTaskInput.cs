using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using System;

/// <summary>
/// Behaviour for registering controller rotation input. Rotation input will be mapped to controller's trackpad.
/// </summary>
public class ControllerSecTaskInput : MonoBehaviour
{
    //Design 1
    //public SteamVR_Action_Vector2 trackPadXYSwipeAction;

    //Design 2
    //[SerializeField] private SteamVR_Action_Vector2 trackpadXSwipeAction;
    [SerializeField] private SteamVR_Action_Boolean[] dPadClickActions = new SteamVR_Action_Boolean[4];
    private Dictionary<RotationDirection, bool> inputDirectionFlags = new Dictionary<RotationDirection, bool>();

    public static Action<RotationDirection> OnControllerSecTaskInput;

    private void Start()
    {
        inputDirectionFlags = ExperimentController.Instance.SecTaskInputDirectionFlag;
    }

    // Update is called once per frame
    void Update()
    {
        /*Design 1 XY Rotation with Swipe gestures in XY direction on trackpad
        Vector2 trackPadXYSwipeValue = trackPadXYSwipeAction.GetAxis(SteamVR_Input_Sources.Any);

        if (trackPadXYSwipeValue.magnitude > 1.5f)
        {
            print("Swipe value " + trackPadXYSwipeValue);
            trackPadXYSwipeValue = trackPadXYSwipeValue.normalized; //normalize to get only the direction of rotation
            trackPadXYSwipeValue = new Vector2(trackPadXYSwipeValue.normalized.y, trackPadXYSwipeValue.normalized.x); // swap x and y value in SwipeValue Vector2 coord
            targetScript.RotateTarget(trackPadXYSwipeValue);
        }
        */
        /*
        //Design 2 - Z Rotation with swipes on X dir on trackpad.
        Vector2 trackPadXSwipeValue = trackpadXSwipeAction.GetAxis(SteamVR_Input_Sources.Any);

        if (trackPadXSwipeValue != Vector2.zero) // ZRotation with swiping action in the left / right direction
        {
            if (inputDirectionFlags[RotationDirection.zClockwise]) 
            {
                if (trackPadXSwipeValue.y < 0)
                {
                    if (OnControllerSecTaskInput != null)
                        OnControllerSecTaskInput(RotationDirection.zClockwise);
                }
            }
            else if (inputDirectionFlags[RotationDirection.zCounterClockwise]) 
            {
                if (trackPadXSwipeValue.y > 0)
                {
                    if (OnControllerSecTaskInput != null)
                        OnControllerSecTaskInput(RotationDirection.zCounterClockwise);
                }
            }
        }
        */

        //X/Y Rotation with button click in the up/down/left/right dir
        if (inputDirectionFlags[RotationDirection.up]) 
        {
            if (dPadClickActions[0].GetStateDown(SteamVR_Input_Sources.Any))
            {
                if (OnControllerSecTaskInput != null)
                    OnControllerSecTaskInput(RotationDirection.up);
            }
        }
        if (inputDirectionFlags[RotationDirection.left]) 
        {
            if (dPadClickActions[1].GetStateDown(SteamVR_Input_Sources.Any))
            {
                if (OnControllerSecTaskInput != null)
                { 
                    OnControllerSecTaskInput(RotationDirection.left);
                    Debug.Log("Controller Left pad clicked! ");
                }
            }
        }
        if (inputDirectionFlags[RotationDirection.down]) 
        {
            if (dPadClickActions[2].GetStateDown(SteamVR_Input_Sources.Any))
            {
                if (OnControllerSecTaskInput != null)
                    OnControllerSecTaskInput(RotationDirection.down);
            }
        }
        if (inputDirectionFlags[RotationDirection.right]) 
        {
            if (dPadClickActions[3].GetStateDown(SteamVR_Input_Sources.Any))
            {
                if (OnControllerSecTaskInput != null)
                {
                    OnControllerSecTaskInput(RotationDirection.right);
                    Debug.Log("Controller Right pad clicked! ");
                }
            }
        }
    }
}
