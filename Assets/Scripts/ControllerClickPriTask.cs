using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

/// <summary>
/// - Line Rendered
/// - Select object with trigger button 
/// - Deselect object with Trigger button
/// </summary>
public class ControllerClickPriTask : MonoBehaviour
{
    //Line Rendering Support
    [SerializeField] private float defaultLength = 5.0f;
    [SerializeField] private GameObject dot;
    private LineRenderer lineRenderer;

    //TargetSelection Support
    public SteamVR_Action_Boolean triggerPressAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("TriggerPress");
    private float targetDistFromController;
    private GameObject selectedTarget;
    private bool isTargetSelected;
    [SerializeField] private Color targetDefaultColor = new Color(0, 0.7f, 1f);
    [SerializeField] private Color targetSelectedColor = new Color(0, 0.9f, 0.5f);

    public bool IsTargetSelected
    {
        get
        {
            return isTargetSelected;
        }

        set
        {
            if(value) // When Target is Selected
            {
                targetDistFromController = Vector3.Distance(transform.position, selectedTarget.transform.position);
                dot.gameObject.SetActive(false);
                selectedTarget.gameObject.GetComponent<MeshRenderer>().material.SetColor("_FirstOutlineColor", targetSelectedColor);
            }
            else //When Target is not selected
            {
                dot.gameObject.SetActive(true);
                selectedTarget.gameObject.GetComponent<MeshRenderer>().material.SetColor("_FirstOutlineColor", targetDefaultColor);
            }

            isTargetSelected = value;
        }
    }

    private void Awake()
    {
        lineRenderer = transform.Find("Pointer").GetComponent<LineRenderer>();
    }

    private void Update()
    {
        //Localization and line rendering when no target is selected
        if(!IsTargetSelected)
        {
            RaycastHit hit = CreateRayCast(defaultLength);
            DefaultLineRendering(hit);

            //Selection with controller click
            if (triggerPressAction.GetStateDown(SteamVR_Input_Sources.Any))
            {
                if (hit.transform.CompareTag("Target"))
                {
                    selectedTarget = hit.transform.gameObject;
                    IsTargetSelected = true;
                }
            }
        }
        else //there is a selected target
        {
            //Set target relative to Controller
            selectedTarget.transform.position = transform.position + (transform.forward * targetDistFromController);

            //Update Line renderer to set to selected target too
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, selectedTarget.transform.position);

            if (triggerPressAction.GetStateDown(SteamVR_Input_Sources.Any))
            {
                IsTargetSelected = false;
                selectedTarget = null;
            }
        }
    }

    /// <summary>
    /// Update rendering line based on End point 
    /// </summary>
    /// <param name="hit"></param>
    private void DefaultLineRendering(RaycastHit hit)
    {
        //End position of line based on Default length
        Vector3 lineEndPos = transform.position + (transform.forward * defaultLength);

        //End position of line based on hit position
        if (hit.collider != null)
            lineEndPos = hit.point;

        //Set Position of dot 
        dot.transform.position = lineEndPos;

        //Set Line Renderer
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, lineEndPos);
    }

    private RaycastHit CreateRayCast(float length)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, length);

        return hit;
    }

}
