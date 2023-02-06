using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveHandTracking;
using System;
using System.Linq;

/// <summary>
/// Behaviour for registering rotational input from user's hand gesture.
/// Upon recognizing finger gesture, creates 6 points around the tip of index finger.
/// When tip of index finger collides with points, will trigger a discrete output to rotate the target by 90deg.
/// </summary>
public class GestureSecTaskInput : MonoBehaviour
{
    #region Fields
    private Vector3 indexFingerTip;
    private Vector3 IndexFingerTipDist;
    [SerializeField] private float activationDistance = 0.04f;

    //Flag to support inputdirection
    private Dictionary<RotationDirection, bool> inputDirectionFlags = new Dictionary<RotationDirection, bool>();

    //flag to rotate target target for one frame 
    private bool rotateTargetOnPreviousFrame = false;

    private List<GameObject> gestureOrbs = new List<GameObject>();

    #endregion 

    public static Action<RotationDirection> OnGestureSecTaskInput; 

    private void Start()
    {
        /*Design 1: GesturePoints
        //initialize callback function in each gesture point
        foreach(Transform child in transform)
        {
            GesturePointCallbackFunc childScript = child.GetComponent<GesturePointCallbackFunc>();
            if (childScript != null)
                childScript.Callback = OnGesturePointTrigger;
        }
        */

        inputDirectionFlags = ExperimentController.Instance.SecTaskInputDirectionFlag;
        InitializeGestureOrbs();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateFingerRelativeTipPosition();
        RotateTargetWithFingerTip(IndexFingerTipDist);
    }

    #region Interactions

    public void OnStateChanged(int state)
    {
        gameObject.SetActive(state == 1);
    }

    /// <summary>
    /// Set gesture points to appear around the finger tip's position
    /// </summary>
    private void OnEnable() 
    { 
        var Camera = GestureProvider.Current.transform;
        Vector3 indexFingerTip = Vector3.zero;
        if (GestureProvider.RightHand != null)
            indexFingerTip = GestureProvider.RightHand.points[GestureProvider.Mode == GestureMode.Skeleton ? 8 : 0];
        if (indexFingerTip != Vector3.zero)                
        {
            //gameObject.SetActive(true);
            transform.position = indexFingerTip;
            transform.rotation = Quaternion.Euler(0, Camera.rotation.eulerAngles.y, 0);
        }
    }

    /*Design 1: GesturePoints
    /// <summary>
    /// Callback function for the points when collided with fingertip.
    /// </summary>
    public void OnGesturePointTrigger(RotationDirection dir)
    {
        //rotate object 
        StartCoroutine(targetScript.RotateTargetOnce(dir));
        //set this object to be inactive 
        gameObject.SetActive(false);
    }
    */

    #endregion

    #region Operations 

    /// <summary>
    /// Set gesture orbs active based on available rotation direction inputs
    /// </summary>
    private void InitializeGestureOrbs()
    {
        foreach (Transform child in transform)
        {
            gestureOrbs.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }

        List<string> availRotationDirectionInput = new List<string>();
        //SetActive/Inactive based on flags in rotationdirectionFlags 
        foreach(KeyValuePair<RotationDirection,bool> pair in inputDirectionFlags)
        {
            if(pair.Value)
            {
                availRotationDirectionInput.Add(pair.Key.ToString());
            }
        }

        foreach(var orb in gestureOrbs)
        {
            foreach(var rotDir in availRotationDirectionInput)
            {
                if(StringExtension.Contains(orb.name, rotDir, StringComparison.OrdinalIgnoreCase))
                {
                    orb.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// Returns finger tip position relative to its origin
    /// </summary>
    private void UpdateFingerRelativeTipPosition()
    {
        if (GestureProvider.RightHand != null)
            indexFingerTip = GestureProvider.RightHand.points[GestureProvider.Mode == GestureMode.Skeleton ? 8 : 0];
        if (indexFingerTip != Vector3.zero)
            IndexFingerTipDist = indexFingerTip - transform.position;
    }

    /// <summary>
    /// Trigger event to rotate target when finger tip's distance from center in one of the direction up/down/left/right/back/front is above threshold.
    /// Mechanism for registering finger tip's movement in the x,y,z direction: 
    /// Have an empty object follow the tip of index finger. Get relative distance between empty object and this GO. 
    /// if any of the its magnitude > 0.05f, activate rotation. make this game object false
    /// </summary>
    /// <param name="IndexFingerTipDist"></param> Distance of finger tip from the center of rotation board.
    void RotateTargetWithFingerTip(Vector3 IndexFingerTipDist)
    {
        //Only rotate when rotate wasnt called in previous frame
        if (IndexFingerTipDist.x > activationDistance)
        {
            if (!rotateTargetOnPreviousFrame)
            {
                if (inputDirectionFlags[RotationDirection.right])
                {
                    if (OnGestureSecTaskInput != null)
                    {
                        OnGestureSecTaskInput(RotationDirection.right);
                        rotateTargetOnPreviousFrame = true;
                    }
                }
            }
        }
        else if (IndexFingerTipDist.x < -activationDistance)
        {
            if (!rotateTargetOnPreviousFrame)
            {
                if (inputDirectionFlags[RotationDirection.left])
                {
                    if (OnGestureSecTaskInput != null)
                    {
                        OnGestureSecTaskInput(RotationDirection.left);
                        rotateTargetOnPreviousFrame = true;
                    }
                }
            }
        }
        else if (IndexFingerTipDist.y > activationDistance)
        {
            if (!rotateTargetOnPreviousFrame)
            {
                if (inputDirectionFlags[RotationDirection.up])
                {
                    if (OnGestureSecTaskInput != null)
                    {
                        OnGestureSecTaskInput(RotationDirection.up);
                        rotateTargetOnPreviousFrame = true;
                    }
                }
            }
        }
        else if (IndexFingerTipDist.y < -activationDistance)
        {
            if (!rotateTargetOnPreviousFrame)
            {
                if (inputDirectionFlags[RotationDirection.down])
                {
                    if (OnGestureSecTaskInput != null)
                    {
                        OnGestureSecTaskInput(RotationDirection.down);
                        rotateTargetOnPreviousFrame = true;
                    }
                }
            }
        }
        else if (IndexFingerTipDist.z < -activationDistance) //z front
        {
            if (!rotateTargetOnPreviousFrame)
            {
                if (inputDirectionFlags[RotationDirection.zClockwise])
                {
                    if (OnGestureSecTaskInput != null)
                    {
                        OnGestureSecTaskInput(RotationDirection.zClockwise);
                        rotateTargetOnPreviousFrame = true;
                    }
                }
            }
        }
        else if(IndexFingerTipDist.z > activationDistance) //z back
        {
            if (!rotateTargetOnPreviousFrame)
            {
                if (inputDirectionFlags[RotationDirection.zCounterClockwise])
                {
                    if (OnGestureSecTaskInput != null)
                    {
                        OnGestureSecTaskInput(RotationDirection.zCounterClockwise);
                        rotateTargetOnPreviousFrame = true;
                    }
                }
            }
        }
        else //when finger tip is within sphere
        {
            rotateTargetOnPreviousFrame = false;
        }
    }

    #endregion
}
