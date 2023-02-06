using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles cube rotation logic. 
/// </summary>
public class TargetRotation : MonoBehaviour
{
    #region Fields
    [SerializeField] private float rotationSpeedPerSec = 540f;
    [SerializeField] private float rotationIncrementValue = 90f;

    //support for queuing coroutines
    private Queue<IEnumerator> coroutineQueue = new Queue<IEnumerator>();

    #endregion 

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(CoroutineCoordinator());
        GestureSecTaskInput.OnGestureSecTaskInput += RotateTarget;
        ControllerSecTaskInput.OnControllerSecTaskInput += RotateTarget;
    }

    private void OnDisable()
    {
        GestureSecTaskInput.OnGestureSecTaskInput -= RotateTarget;
        ControllerSecTaskInput.OnControllerSecTaskInput -= RotateTarget;
        coroutineQueue.Clear();
    }

    void Update()
    {
        //WASDInputModule();
    }

    #region Interactions 

    private void RotateTarget(RotationDirection dir)
    {
        Vector3 rotationalAxis = RotationDirectionUtility.ConvertRotationDirectionToVector3(dir);
        coroutineQueue.Enqueue(RotateTargetByNinetyDeg(rotationalAxis));
    }
    private void RotateTarget(Vector3 rotationalAxis)
    {
        coroutineQueue.Enqueue(RotateTargetByNinetyDeg(rotationalAxis));
    }

    /*
    /// <summary>
    /// Uses transform.Rotate() for rotation
    /// Will need to input the incremental value, and not absolute value(Lerp) of rotation.
    /// Also, foresee a problem where current rotation will overshoot target rotation.  
    /// Accuracy down to 0.01deg because cannot set absolute value for target rotation.
    /// </summary>
    IEnumerator RotateWithIncrement(Vector3 rotationalAxis)
    {
        float currentAngleValue = 0;
        float totalAngleChange = 90f;
        Quaternion initialRotation = transform.rotation;

        while (currentAngleValue < totalAngleChange)
        {
            transform.Rotate(rotationalAxis, Time.deltaTime * rotationSpeedPerSec, Space.World);
            currentAngleValue += Time.deltaTime * rotationSpeedPerSec;
            yield return null;
        }
        //Attempt to set euler angle to fixed value. 
        transform.rotation = initialRotation * Quaternion.Euler(rotationalAxis * totalAngleChange);
    }

    /// <summary>
    /// Rotate by rotating object's forward vector in either the top/bottom/left/right vector of the object.
    /// Too complicated, how to identify which face of the cube  is the arrow currently at now? 
    /// </summary>
    /// <returns></returns>
    IEnumerator RotateWithForwardVector()
    {
        yield return null;
    }
    */

    /// <summary>
    /// Test Module to test target rotation with WASD keyboard input.
    /// </summary>
    private void WASDInputModule()
    {
        if (Input.GetKeyDown("w"))
        {
            RotateTarget(Vector3.right);
        }
        if (Input.GetKeyDown("s"))
        {
            RotateTarget(Vector3.left);
        }
        if (Input.GetKeyDown("a"))
        {
            RotateTarget(Vector3.up);
        }
        if (Input.GetKeyDown("d"))
        {
            RotateTarget(Vector3.down);
        }
        if (Input.GetKeyDown("q"))
        {
            RotateTarget(Vector3.forward);
        }
        if (Input.GetKeyDown("e"))
        {
            RotateTarget(Vector3.back);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            print("coroutineQueue count: " + coroutineQueue.Count);
        }
    }

    #endregion

    #region Operations 

    /// <summary>
    /// Method responsible for queuing the coroutines
    /// </summary>
    /// <returns></returns>
    IEnumerator CoroutineCoordinator()
    {
        while (true)
        {
            while (coroutineQueue.Count > 0)
                yield return StartCoroutine(coroutineQueue.Dequeue());
            yield return null;
        }
    }

    /// <summary>
    /// Have to use Coroutine to perform queuing effect for the coroutines
    /// Uses Quaternion to perform rotation. 
    /// Accuracy of Rotation: 100%, doesnt overshoot target rotation.
    /// One input in the x,y,z axis will result in a 90deg rotation about the x,y,z axis
    /// </summary>
    /// <returns></returns>
    private IEnumerator RotateTargetByNinetyDeg(Vector3 rotationalAxis)
    {
        Quaternion initialRotation = transform.rotation;
        Vector3 vec = transform.InverseTransformDirection(rotationalAxis); // keep rot axis at global axis
        Quaternion targetRotation = initialRotation * Quaternion.Euler(vec * rotationIncrementValue); //*operator = summing both angles tgt in Quaternion 

        while (transform.rotation.eulerAngles.magnitude != targetRotation.eulerAngles.magnitude)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeedPerSec * Time.deltaTime);
            yield return null;
        }
    }

    #endregion
}
