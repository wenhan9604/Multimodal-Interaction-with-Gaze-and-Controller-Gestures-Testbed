﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.G2OM;

public class ToolTipOnGaze : MonoBehaviour , IGazeFocusable
{
    [SerializeField] private GameObject toolTip;
    [SerializeField] private float animationTime = 0.3f;
    private bool isToolTipActive;

    public void GazeFocusChanged(bool hasFocus)
    {
        isToolTipActive = hasFocus;
    }

    // Update is called once per frame
    void Update()
    {
        if (isToolTipActive)
            toolTip.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(toolTip.GetComponent<CanvasGroup>().alpha, 1, Time.deltaTime * (1 / animationTime));
        else
            toolTip.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(toolTip.GetComponent<CanvasGroup>().alpha, 0, Time.deltaTime * (1 / animationTime));
    } 
}
