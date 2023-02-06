using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

public class Destination : MonoBehaviour
{
    #region Fields 
    [SerializeField] private Color defaultColor = Color.red;
    [SerializeField] private Color hoveredColor = Color.blue;
    [SerializeField] private Color selectionColor = Color.green;
    private MeshRenderer meshRenderer;
    SerializedObject halo;
    private bool isHoveredOn = false;
    
    private bool isDestinationSelectedOnPreviousFrame;

    #endregion

    #region Properties

    public bool IsHoveredOn
    {
        get { return isHoveredOn; }
    }

    #endregion

    #region Events
    public static Action<bool> OnDestinationHovered;
    public static Action OnDestinationSelected;

    #endregion 

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        halo = new SerializedObject(GetComponent("Halo"));
        ChangeColor(defaultColor);
    }

    #region Interactions

    private void OnTriggerEnter(Collider other) //Double check if trigger stay will be activated on object instantiation.
    {
        if(other.tag == "Target")
        {
            OnDestinationHoveredOn(true);
        }
    }

    private void OnTriggerStay(Collider other) 
    {
        if(other.tag == "Target")
        {
            if (!other.gameObject.GetComponent<SphericalTarget>().IsTargetSelected) //When Target is released on destination
            {
                DestinationSelected();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Target")
            OnDestinationHoveredOn(false);
    }

    #endregion

    #region Operations 

    private void OnDestinationHoveredOn(bool isHovering)
    {
        isHoveredOn = isHovering;
        if(isHovering)
            ChangeColor(hoveredColor);
        else
            ChangeColor(defaultColor);

        if (OnDestinationHovered != null)
            OnDestinationHovered(isHovering);
    }

    /// <summary>
    /// When destination is selected
    /// </summary>
    private void DestinationSelected()
    {
        if (!isDestinationSelectedOnPreviousFrame)
        {
            if (OnDestinationSelected != null)
                OnDestinationSelected();

            ChangeColor(selectionColor);
            isDestinationSelectedOnPreviousFrame = true;
        }
    }

    private void ChangeColor(Color color)
    {
        meshRenderer.material.color = color;
        meshRenderer.material.SetColor("_EmissionColor", color);
        halo.FindProperty("m_Color").colorValue = color;
        halo.ApplyModifiedProperties();
    }

    #endregion
}
