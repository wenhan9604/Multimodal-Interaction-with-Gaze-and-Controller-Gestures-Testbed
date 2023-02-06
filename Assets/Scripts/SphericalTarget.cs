using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.G2OM;
using System;

/// <summary>
/// Spherical Target containing cube inside. Spherical bound will be highlighted when hovered / selected.
/// </summary>
public class SphericalTarget : MonoBehaviour , IGazeFocusable
{
    #region Fields
    
    //Calculate Angular Size Support
    Vector3 gazeOrigin;
    MeshRenderer meshRenderer; // access to sphere Center
    SphereCollider sphereCollider; //access to sphere Radius
    float distToGazeOrigin;
    Vector3 dirToGazeOrigin;
    [SerializeField] float angularSize;

    //Target Interaction Support
    private bool isTargetHoveredOn = false;
    private bool isTargetSelected = false;
    private bool isCollidingWithDestination = false;

    //TargetChange Color Support 
    [SerializeField] Color targetHoveredOnColor = new Color(0, .3f, .6f, 1f);
    [SerializeField] Color targetSelectedColor = new Color(0, 0.9f, 0.5f);

    #endregion

    #region Properties

    public float AngularSize
    {
        get { return angularSize; }
    }

    public bool IsTargetHoveredOn
    {
        get { return isTargetHoveredOn; }
        set
        {
            if (value != isTargetHoveredOn)
            {
                ChangeColorOnHovered(value);
                isTargetHoveredOn = value;
            }
        }
    }

    public bool IsTargetSelected
    {
        get { return isTargetSelected; }
        set
        {
            TargetSelected(value);
            isTargetSelected = value;
        }
    }

    public bool IsCollidingWithDestination
    {
        get { return isCollidingWithDestination; }
        private set
        {
            isCollidingWithDestination = value;
        }
    }

    #endregion

    #region Events 
    public static Action OnTargetSelection;

    #endregion

    /*
    [SerializeField] bool debugHoriAngularSize = false;
    [SerializeField] bool debugVertAngularSize = false;

    [SerializeField] float angSizeHori;
    [SerializeField] float angSizeVert;
    [SerializeField] float angSizeVert2;
    [SerializeField] float angSizeHori2;

    public float angularSizeHorizontal
    {   
        get { return angSizeHori; } 
    }

    public float angularSizeVertical
    {
        get { return angSizeVert; }
    }*/

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    void Update()
    {
        /*
        //Assumes that the cyclopean eye of user (center of both eyes) are at the eye gaze origin provided by Tobii
        gazeOrigin = Camera.main.gameObject.GetComponent<EyeGazeAngle>().eyeGazeOrigin;
        distToGazeOrigin = Vector3.Distance(meshRenderer.bounds.center, gazeOrigin);
        dirToGazeOrigin = (meshRenderer.bounds.center - gazeOrigin).normalized;
        CalculateAngularSize();
        */
    }

    #region Interactions

    /// <summary>
    /// When Gaze hovers on object.
    /// </summary>
    /// <param name="hasFocus"></param>
    void IGazeFocusable.GazeFocusChanged(bool hasFocus)
    {
        if(ExperimentController.Instance.PrimaryTaskInput != PrimaryTaskInput.ControllerClick)
            IsTargetHoveredOn = hasFocus;
    }

    /// <summary>
    /// Function called when target is selected
    /// </summary>
    /// <param name="isTargetSelected"></param>
    void TargetSelected(bool isTargetSelected)
    {
        if (IsTargetHoveredOn)
            IsTargetHoveredOn = false;

        ChangeColorOnSelection(isTargetSelected);

        //Log Wrong Deselection error only if deselected and is not colliding with destination.
        //Can set a destination limit to adjust sensitive of this error as well. 
        if (!IsCollidingWithDestination && !isTargetSelected)
        {
            DataCollectionManager.Instance.UpdateWrongTargetDeselection();
        }

        if (isTargetSelected)
            OnTargetSelection();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Destination")
        {
            IsCollidingWithDestination = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Destination")
        {
            IsCollidingWithDestination = false;
        }
    }

    #endregion

    #region Operations

    void CalculateAngularSize()
    {
        /*
        transform.LookAt(gazeOrigin);
        
        //Method 1: Vertical AngularSize using Vector3.Angle
        Vector3 positionUpperPoint = transform.up * sphereCollider.radius + meshRenderer.bounds.center;
        Vector3 positionLowerPoint = -transform.up * sphereCollider.radius + meshRenderer.bounds.center;
        Vector3 dirToUpperPoint = (positionUpperPoint - gazeOrigin).normalized;
        Vector3 dirToLowerPoint = (positionLowerPoint - gazeOrigin).normalized;
        angSizeVert = Vector3.Angle(dirToGazeOrigin, dirToUpperPoint) * 2;

        if(debugVertAngularSize)
        {
            Debug.DrawLine(positionUpperPoint,gazeOrigin,Color.red);
            Debug.DrawLine(positionLowerPoint, gazeOrigin, Color.red);
        }

        //Horizontal AngularSize using Vector3.Angle
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

        //Method 2: Using cos rule 
        float distToUpperPoint = Vector3.Distance(gazeOrigin, positionUpperPoint);
        float distToLowerPoint = Vector3.Distance(gazeOrigin, positionLowerPoint);
        angSizeVert2 = Mathf.Acos( (Mathf.Pow(distToUpperPoint,2) + Mathf.Pow(distToLowerPoint,2) - Mathf.Pow((sphereCollider.radius * 2), 2)) / (2 * distToLowerPoint * distToUpperPoint)) * Mathf.Rad2Deg;
        
        float distToLeftPoint = Vector3.Distance(gazeOrigin, positionLeftPoint);
        float distToRightPoint = Vector3.Distance(gazeOrigin, positionRightPoint);
        angSizeHori2 = Mathf.Acos((Mathf.Pow(distToLeftPoint, 2) + Mathf.Pow(distToRightPoint, 2) - Mathf.Pow((sphereCollider.radius * 2), 2)) / (2 * distToRightPoint * distToLeftPoint)) * Mathf.Rad2Deg;
        */

        //method 3: Using Trigo - Toa Cah Soh
        angularSize = Mathf.Atan(sphereCollider.radius / distToGazeOrigin) * Mathf.Rad2Deg * 2;
    }

    void ChangeColorOnHovered(bool isHoveredOn)
    {
        if (isHoveredOn)
        {
            meshRenderer.material.SetColor("_OutlineColor", targetHoveredOnColor);
        }
        else
        {
            meshRenderer.material.SetColor("_OutlineColor", Color.clear);
        }
    }

    void ChangeColorOnSelection(bool isSelected)
    {
        if (isSelected)
        {
            meshRenderer.material.SetColor("_OutlineColor", targetSelectedColor);
        }
        else
        {
            meshRenderer.material.SetColor("_OutlineColor", targetHoveredOnColor);
        }
    }

    #endregion
}
