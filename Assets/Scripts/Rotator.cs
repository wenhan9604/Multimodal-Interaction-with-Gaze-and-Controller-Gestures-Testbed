using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    Vector3 pivotPoint;
    float distFromPivotPoint;
    float linearVelocity;

    Rigidbody rigidBody;
    [SerializeField] Vector3 angVelocityPerFrame;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Quaternion deltaRotation = Quaternion.Euler(angVelocityPerFrame * Time.deltaTime);
        rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);
    }
}
