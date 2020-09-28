using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.XR;
using Tobii.XR.Examples;

[RequireComponent(typeof(ControllerVisualizer))]
public class HUDPopUp : MonoBehaviour
{
    private ControllerManager controllerManager;
    private ControllerVisualizer controllerVisualizer;
    private GameObject controller;
    private GameObject HUDCanvas;

    public string targetName;
    private Vector3 targetToolTipPosition;
    private Quaternion targetToolTipOrientation;

    // Start is called before the first frame update
    void Start()
    {
        controllerManager = ControllerManager.Instance;
        controllerVisualizer = GetComponent<ControllerVisualizer>();
        controller = controllerVisualizer._controllerGameObject;
        HUDCanvas = controller.transform.Find("Canvas").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (controllerManager.GetButtonPressDown(ControllerButton.Trigger))
        {
            Debug.Log("trigger button is pressed!");
            if (UpdateEyeGazedTarget())
            {
                if(UpdateCanvas())
                {
                    Debug.Log("successfully changed panel in Canvas");
                }
            }
        }

        if (controllerManager.GetButtonPressUp(ControllerButton.Trigger))
        {
            //set inactive all panels
            foreach (Transform panel in HUDCanvas.transform)
            {
                panel.gameObject.SetActive(false);
            }
        }

    }

    bool UpdateEyeGazedTarget()
    {
        if (TobiiXR.FocusedObjects.Count > 0)
        {
            var focusedObject = TobiiXR.FocusedObjects[0];
            targetName = focusedObject.GameObject.name;
            GameObject toolTip = focusedObject.GameObject.transform.Find("ToolTip").gameObject;

            if (toolTip != null)
            {
                targetToolTipPosition = toolTip.transform.position;
                targetToolTipOrientation = toolTip.transform.rotation;
                Debug.Log("Tooltip on Target is Found and updated");
                return true;
            }
            else 
                return false;
        }
        else
            return false;
    }

    bool UpdateCanvas()
    {
        //check targetName against each child Object
        foreach (Transform panel in HUDCanvas.transform)
        {
            if (panel.gameObject.name == targetName)
            {
                panel.gameObject.SetActive(true);
                return true;
            }
        }
        return false;     
    }
}
