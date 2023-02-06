using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.XR; 

public class DebugGazeAndHeadVector : MonoBehaviour
{

    [SerializeField] private GameObject mainCameraGO;
    private Vector3 camOrigin;
    private Vector3 headGazeDir;
    private float countKeyCodeSpace = 1;
    [SerializeField] private float rayLength = 8f;
    [SerializeField] private bool CalculateGazeAndHeadAngleOnSpaceBar;
    public Vector3 eyeGazeOrigin;


    private void Start()
    {
        if(mainCameraGO == null)
            mainCameraGO = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);

        //Draw Head Gaze Ray
        if (mainCameraGO != null)
        {
            camOrigin = mainCameraGO.transform.position;
            headGazeDir = mainCameraGO.transform.forward;
            Vector3 headGazeDirWithMag = headGazeDir * rayLength;
            Debug.DrawRay(camOrigin, headGazeDirWithMag, Color.white);
        }

        //Draw Eye Gaze Ray
        if (eyeTrackingData.GazeRay.IsValid)
        {
            eyeGazeOrigin = eyeTrackingData.GazeRay.Origin;
            var eyeGazeDir = eyeTrackingData.GazeRay.Direction;

            Vector3 eyeGazeDirWithMag = eyeGazeDir * rayLength;

            //Draw Ray from Camera's position in the direction of Eye Gaze
            Debug.DrawRay(camOrigin, eyeGazeDirWithMag, Color.yellow);

            //get angle between head gaze and eye gaze
            if(Input.GetKeyDown(KeyCode.Space) && CalculateGazeAndHeadAngleOnSpaceBar)
            {
                Debug.Log("Space Key Count: " + countKeyCodeSpace);
                countKeyCodeSpace++;

                Debug.Log("Head Gaze Dir " + headGazeDir.ToString("F4"));
                Debug.Log("Eye Gaze Dir " + eyeGazeDir.ToString("F4"));

                //get angle about y axis 
                float angleAboutY = AngleOffAroundAxis(eyeGazeDir,headGazeDir, Vector3.up);
                Debug.Log("Angle Rotated about Y: " + angleAboutY);

                //get angle about x axis 
                float angleAboutX = AngleOffAroundAxis(eyeGazeDir,headGazeDir, Vector3.right, true);
                Debug.Log("Angle Rotated about X: " + angleAboutX);

                //get the angular difference of direct rotation 
                float angDiff = Vector3.Angle(headGazeDir, eyeGazeDir);
                Debug.Log("Angular Diff of Direct Rotation: " + angDiff);
            }
        }
    }

    /// <summary>
    /// Find some projected angle measure off some forward around some axis.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="forward"></param>
    /// <param name="axis"></param>
    /// <returns>Angle in degrees</returns>
    public static float AngleOffAroundAxis(Vector3 v, Vector3 forward, Vector3 axis, bool clockwise = false)
    {
        Vector3 right;
        if (clockwise)
        {
            right = Vector3.Cross(forward, axis);
            forward = Vector3.Cross(axis, right);
        }
        else
        {
            right = Vector3.Cross(axis, forward);
            forward = Vector3.Cross(right, axis);
        }
        return Mathf.Atan2(Vector3.Dot(v, right), Vector3.Dot(v, forward)) * Mathf.Rad2Deg;
    }
}
