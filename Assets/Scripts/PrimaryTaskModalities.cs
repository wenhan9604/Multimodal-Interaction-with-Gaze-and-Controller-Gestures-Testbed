using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Tobii.XR;
using Valve.VR;
using ViveHandTracking;

public enum PrimaryTaskInput
{
    ControllerClick,
    GazeDwell,
    GazeClick,
    GazeGesture
}

/// <summary>
/// Enables participants to complete Primary task using different modalities: 
/// Controller + Click. 
/// Gaze + Dwell Time
/// Gaze + Click 
/// Gaze + Gesture
/// </summary>
public class PrimaryTaskModalities : MonoBehaviour
{
    #region Fields 
    //EyeGazeSupport 
    private Vector3 eyeGazeDir;
    private Vector3 eyeGazeOrigin;
    private GameObject previousFrameGameObject;
    private GameObject gazeFocusedGameObject;
    
    //Support for Target Selection
    private GameObject selectedTarget;
    private SphericalTarget targetScript;
    private float targetDistFromEyes;

    //Support for GazeDwellTime
    private bool gazeDwellTimerStatus = false;
    private float gazeDwellTimer = 0;
    [SerializeField] private float dwellTime; //Optimal Dwell time 300 - 600ms

    //Support for ButtonClick
    public SteamVR_Action_Boolean triggerPressAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("TriggerPress");

    //Support for Gesture
    [SerializeField] private float pinchConfidence = 0.5f;
    [SerializeField] private float pinchGestureInterval = 0.3f;
    [SerializeField] private GameObject RightHandRenderer;
    [SerializeField] private GestureProvider gestureProviderScript;
    private bool gestureDwellTimeStatus = false;
    private float gestureTimer = 0;

    //Support for ControllerClick
    private float defaultControllerRayLength = 10.0f;
    [SerializeField] private GameObject controllerGO;
    private GameObject pointerGO;
    private GameObject dotGO;
    private LineRenderer controllerRay;
    private float targetDistFromController;
    private GameObject previousHitTarget;

    #endregion

    private void Start()
    {
        //Initialize Controller variables
        if(controllerGO != null)
        {
            pointerGO = controllerGO.transform.Find("Pointer").gameObject;
            if (pointerGO != null)
            { 
                controllerRay = pointerGO.GetComponent<LineRenderer>();
                dotGO = pointerGO.transform.Find("Dot").gameObject;
            }
        }

        InitializeModalityStartSettings(ExperimentController.Instance.PrimaryTaskInput);
    }

    // Update is called once per frame
    void Update()
    {
        if (ExperimentController.Instance.PrimaryTaskInput == PrimaryTaskInput.ControllerClick)
        {
            ControllerClick();
        }
        else
        {
            bool isGazeDataValid = UpdateGazeData();

            switch (ExperimentController.Instance.PrimaryTaskInput)
            {
                case PrimaryTaskInput.GazeDwell:
                    GazeDwellTime();
                    break;
                case PrimaryTaskInput.GazeClick:
                    GazeClick();
                    break;
                case PrimaryTaskInput.GazeGesture:
                    GazeGesture();
                    break;
            }
            if (isGazeDataValid)
            {
                //Target Dragging with Gaze
                if (selectedTarget != null)
                {
                    Vector3 eyeGazeDirWithMag = eyeGazeDir.normalized * targetDistFromEyes;
                    selectedTarget.transform.position = eyeGazeDirWithMag + eyeGazeOrigin;
                }
            }
            else
            {
                Debug.Log("Gaze Data is not valid. Unable to update target dragging with gaze");
            }
        }
    }

    #region Interactions


    /// <summary>
    /// Updates Gaze Data. Track whether gaze is fixating on a GazeInteractable Object.
    /// Return if eyetrackingdata is valid 
    /// </summary>
    /// <returns></returns>
    private bool UpdateGazeData()
    {
        var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
        eyeGazeDir = eyeTrackingData.GazeRay.Direction;
        eyeGazeOrigin = eyeTrackingData.GazeRay.Origin;

        if (TobiiXR.FocusedObjects.Count > 0)
        {
            gazeFocusedGameObject = TobiiXR.FocusedObjects[0].GameObject;
        }
        else
        {
            gazeFocusedGameObject = null;
        }

        return eyeTrackingData.GazeRay.IsValid;
    }

    /// <summary>
    ///Gaze + Dwell Time Modality 
    ///target will be charged up. Outline = blue. Timer starts
    ///if Timer reaches indicated dwell time, object is selected. If object loses focuses, timer restarts.
    /// </summary>
    private void GazeDwellTime()
    {
        //Target Selection with timer
        if (selectedTarget == null)
        {
            //When localized on target using gaze
            if (gazeFocusedGameObject != null)
            {
                //Debug.Log("gaze timer is true. No target selected, gaze focusing on object");
                gazeDwellTimerStatus = true;
                //Initialize flag
                if (previousFrameGameObject == null)
                {
                    previousFrameGameObject = gazeFocusedGameObject;
                }
            }

            //When User loses focus on current gaze object or loses focus on any game object
            if (previousFrameGameObject != gazeFocusedGameObject || gazeFocusedGameObject == null)
            {
                gazeDwellTimerStatus = false;
                gazeDwellTimer = 0;
                previousFrameGameObject = null;
            }

        }
        else //TargetDeselection: When target is selected and target is colliding with destination.
        {
            if(targetScript != null)
            {
                if(targetScript.IsCollidingWithDestination)
                {
                    //Debug.Log("gaze timer is true. Target selected, gaze focusing destination");
                    gazeDwellTimerStatus = true;
                }
                else
                {
                    gazeDwellTimerStatus = false;
                    gazeDwellTimer = 0;
                }
            }
        }

        //Activate Selection/Deselection
        if (gazeDwellTimer > dwellTime)
        {
            gazeDwellTimerStatus = false;
            gazeDwellTimer = 0;
            previousFrameGameObject = null;

            if(selectedTarget == null) //When there is no target
            {
                TargetSelection(gazeFocusedGameObject);
            }
            else //when there is a target already.
            { 
                TargetDeselection();
            }
        }

        //Timer
        if (gazeDwellTimerStatus)
        {
            gazeDwellTimer += Time.deltaTime;
        }
    }  
    
    /// <summary>
    /// Gaze + trigger click
    /// Able to Select / Deselect Target
    /// </summary>
    private void GazeClick()
    {
        //When object is selected while being gazed on 
        if (triggerPressAction.GetStateDown(SteamVR_Input_Sources.Any))
        {
            if(gazeFocusedGameObject != null)
            {
                //Activate Selection
                if(selectedTarget == null)
                {
                    TargetSelection(gazeFocusedGameObject);
                }
                else
                {
                    TargetDeselection();
                }
            }
        }
    }

    /// <summary>
    /// Gaze + Pinch Gesture to select target. Able to select/deselect Target
    /// </summary>
    private void GazeGesture()
    {
        if (!RightHandRenderer.activeSelf)
            RightHandRenderer.SetActive(true);

        if(GestureProvider.RightHand != null)
        {  
            if(GestureProvider.RightHand.pinch.pinchLevel > 0.05)
            {
                Debug.Log("Is pinching. Pinch Value: " + GestureProvider.RightHand.pinch.pinchLevel);
            }
            if(GestureProvider.RightHand.pinch.pinchLevel > pinchConfidence)
            {
                if (gazeFocusedGameObject != null)
                {
                    // Pinching gesture selects target either on first try or after an interval
                    if (!gestureDwellTimeStatus || gestureTimer > pinchGestureInterval)
                    {
                        //select target in this frame 
                        if(selectedTarget == null)
                        {
                            TargetSelection(gazeFocusedGameObject);
                        }
                        else
                        {
                            TargetDeselection();
                        }
                        gestureTimer = 0;
                        gestureDwellTimeStatus = true;
                    }
                }
            }
        }
        
        if(gestureDwellTimeStatus)
        {
            gestureTimer += Time.deltaTime;

            if(gestureTimer > 3f)
            {
                gestureDwellTimeStatus = false;
                gestureTimer = 0;
            }
        }
    }

    private void ControllerClick()
    {
        if(!pointerGO.activeSelf)
            pointerGO.SetActive(true);

        //Localizing and controller ray rendering
        if(selectedTarget == null)
        {
            RaycastHit hit = CreateControllerRayCast(defaultControllerRayLength);
            DefaultControllerRayRendering(hit);
            
            //TargetHovering
            if (hit.collider != null)
            {
                GameObject hitGameObject = hit.collider.gameObject;
                //Check if ray is hovering on Target
                if (hitGameObject.tag == "Target") //Ray hits Target
                {
                    hitGameObject.GetComponent<SphericalTarget>().IsTargetHoveredOn = true;
                    previousHitTarget = hitGameObject;
                }
                else // Ray hits other gameobject which is not a Target
                {
                    if (previousHitTarget != null)
                    {
                        previousHitTarget.GetComponent<SphericalTarget>().IsTargetHoveredOn = false;
                        previousHitTarget = null;
                    }
                }
            }
            else //Ray hits nothing (no game object) 
            {
                if(previousHitTarget != null)
                {
                    previousHitTarget.GetComponent<SphericalTarget>().IsTargetHoveredOn = false;
                    previousHitTarget = null;
                }
            }

            //Selection with Controller Click
            if (triggerPressAction.GetStateDown(SteamVR_Input_Sources.Any))
            {
                if(hit.transform != null && hit.transform.CompareTag("Target"))
                {
                    TargetSelection(hit.transform.gameObject);
                }
            }
        }
        else // Target Dragging with controller
        {
            if (selectedTarget != null)
            {
                selectedTarget.transform.position = controllerGO.transform.position +
                  (controllerGO.transform.forward * targetDistFromController);

                controllerRay.SetPosition(0, controllerGO.transform.position);
                controllerRay.SetPosition(1, selectedTarget.transform.position);

                if (triggerPressAction.GetStateDown(SteamVR_Input_Sources.Any))
                {
                    TargetDeselection();
                }
            }
        }

    }

    /// <summary>
    /// targetselected = gameobject 
    /// Get Distance of target from camera/ controller 
    /// </summary>
    /// <param name="value">Target to be selected</param>
    private void TargetSelection(GameObject targetToBeSelect)
    {
        selectedTarget = targetToBeSelect;
        targetScript = selectedTarget.GetComponent<SphericalTarget>();
        targetScript.IsTargetSelected = true;

        if (ExperimentController.Instance.PrimaryTaskInput == PrimaryTaskInput.ControllerClick)
        {
            targetDistFromController = Vector3.Distance(controllerGO.transform.position, selectedTarget.transform.position);
            dotGO.SetActive(false);
        }
        else
        {
            targetDistFromEyes = Vector3.Distance(selectedTarget.transform.position, eyeGazeOrigin);
        }
    }

    /// <summary>
    /// Deselect Target.
    /// </summary>
    private void TargetDeselection()
    {
        //when target is deselected
        if (ExperimentController.Instance.PrimaryTaskInput == PrimaryTaskInput.ControllerClick)
        {
            dotGO.SetActive(true);
        }
        targetScript.IsTargetSelected = false;
        targetScript = null;
        selectedTarget = null;
    }

    #endregion

    #region Operation

    /// <summary>
    /// Set the needed variables/settings to true for the indicated modality. Note that this setting will only be changed from the start. 
    /// </summary>
    /// <param name="primaryTaskInput"></param>
    private void InitializeModalityStartSettings(PrimaryTaskInput primaryTaskInput)
    {
        pointerGO.SetActive(false);
        gestureProviderScript.enabled = false;

        switch (primaryTaskInput)
        {
            case PrimaryTaskInput.ControllerClick:
                pointerGO.SetActive(true);
                break;
            case PrimaryTaskInput.GazeGesture:
                gestureProviderScript.enabled = true;
                break;
            default:
                break;
        }
    }

    private RaycastHit CreateControllerRayCast(float length)
    {
        RaycastHit hit;
        Ray ray = new Ray(controllerGO.transform.position, controllerGO.transform.forward);
        Physics.Raycast(ray, out hit, length);
        return hit;
    }

    private void DefaultControllerRayRendering(RaycastHit hit)
    {
        Vector3 lineEndPos = controllerGO.transform.position + controllerGO.transform.forward * defaultControllerRayLength;

        if (hit.collider != null)
            lineEndPos = hit.point;

        if (dotGO != null)
            dotGO.transform.position = lineEndPos;

        controllerRay.SetPosition(0, controllerGO.transform.position);
        controllerRay.SetPosition(1, lineEndPos);
    }
    #endregion
}
