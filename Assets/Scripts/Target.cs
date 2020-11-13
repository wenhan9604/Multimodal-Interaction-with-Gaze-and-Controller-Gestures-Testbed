﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic Target for Test Area
/// </summary>
public class Target : MonoBehaviour
{
    //Calculate Angular Size Support 
    Vector3 gazeOrigin;
    MeshRenderer meshRenderer; // access to sphere Center
    SphereCollider sphereCollider; //access to sphere Radius
    [SerializeField] float distToGazeOrigin;
    Vector3 dirToGazeOrigin;

    [SerializeField] bool debugHoriAngularSize = false;
    [SerializeField] bool debugVertAngularSize = false;

    [SerializeField] float angSizeHori;
    [SerializeField] float angSizeVert;
    //[SerializeField] float angSizeVert2;
    [SerializeField] float angSizeHori2;
    [SerializeField] float angSizeGeneric;

    public float angularSizeHorizontal
    {   
        get { return angSizeHori; } 
    }

    public float angularSizeVertical
    {
        get { return angSizeVert; }
    }

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        //Assumes that the cyclopean eye of user (center of both eyes) are at the eye gaze origin provided by Tobii
        gazeOrigin = Camera.main.gameObject.GetComponent<EyeGazeAngle>().eyeGazeOrigin;
        distToGazeOrigin = Vector3.Distance(meshRenderer.bounds.center, gazeOrigin);
        dirToGazeOrigin = (meshRenderer.bounds.center - gazeOrigin).normalized;
        CalculateAngularSize();
    }

    void CalculateAngularSize()
    {
        transform.LookAt(gazeOrigin);
        
        //Vertical AngularSize
        Vector3 positionUpperPoint = transform.up * sphereCollider.radius + meshRenderer.bounds.center;
        Vector3 positionLowerPoint = -transform.up * sphereCollider.radius + meshRenderer.bounds.center;
        Vector3 dirToUpperPoint = (positionUpperPoint - gazeOrigin).normalized;
        Vector3 dirToLowerPoint = (positionLowerPoint - gazeOrigin).normalized;
        angSizeVert = Vector3.Angle(dirToGazeOrigin, dirToUpperPoint) * 2;

        //method 2: Using Trigo - TOA
        angSizeGeneric =  Mathf.Atan(sphereCollider.radius / distToGazeOrigin) * Mathf.Rad2Deg *2;

        if(debugVertAngularSize)
        {
            Debug.DrawLine(positionUpperPoint,gazeOrigin,Color.red);
            Debug.DrawLine(positionLowerPoint, gazeOrigin, Color.red);
        }

        //Horizontal AngularSize
        Vector3 positionLeftPoint = transform.right * sphereCollider.radius + meshRenderer.bounds.center;
        Vector3 positionRightPoint = -transform.right * sphereCollider.radius + meshRenderer.bounds.center;
        Vector3 dirToLeftPoint = (positionLeftPoint - gazeOrigin).normalized;
        Vector3 dirToRightPoint = (positionRightPoint - gazeOrigin).normalized;
        angSizeHori = Vector3.Angle(dirToGazeOrigin, dirToLeftPoint) * 2;

        if (debugHoriAngularSize)
        {
            Debug.DrawLine(positionLeftPoint, gazeOrigin, Color.red);
            Debug.DrawLine(positionRightPoint, gazeOrigin, Color.red);
        }

        /*Method 2: Using cos rule to find angular size
        float distToUpperPoint = Vector3.Distance(gazeOrigin, positionUpperPoint);
        float distToLowerPoint = Vector3.Distance(gazeOrigin, positionLowerPoint);

        angSizeVert2 = Mathf.Acos( (Mathf.Pow(distToUpperPoint,2) + Mathf.Pow(distToLowerPoint,2) - Mathf.Pow((sphereCollider.radius * 2), 2)) / (2 * distToLowerPoint * distToUpperPoint)) * Mathf.Rad2Deg;
        */

        float distToLeftPoint = Vector3.Distance(gazeOrigin, positionLeftPoint);
        float distToRightPoint = Vector3.Distance(gazeOrigin, positionRightPoint);

        angSizeHori2 = Mathf.Acos((Mathf.Pow(distToLeftPoint, 2) + Mathf.Pow(distToRightPoint, 2) - Mathf.Pow((sphereCollider.radius * 2), 2)) / (2 * distToRightPoint * distToLeftPoint)) * Mathf.Rad2Deg;
    }
}
