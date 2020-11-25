using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTarget : Target
{
    
    [SerializeField] Vector3 pivotPoint;
    [SerializeField] float distFromPivotPoint;
    [SerializeField] float angularVelocity;
        
    // Update is called once per frame
    void Update()
    {
        transform.position = (transform.position - pivotPoint).normalized * distFromPivotPoint + pivotPoint;
        transform.RotateAround(pivotPoint, Vector3.up, angularVelocity * Time.deltaTime);
    }
}
