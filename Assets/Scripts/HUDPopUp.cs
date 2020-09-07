using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.XR;

public class HUDPopUp : MonoBehaviour
{
    [SerializeField] private List<string> HUDNames;
    private string currentTargetName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(TobiiXR.FocusedObjects.Count > 0)
        {
            var focusedObject = TobiiXR.FocusedObjects[0];
            currentTargetName = focusedObject.GameObject.name;

            //if joystick pressing on something, check name against list 
            //if name is found in list, pops up HUD 

            //if joy stick is pressing
            foreach(var name in HUDNames)
            {
                if(currentTargetName == name)
                {
                    //set boolean to true
                }
            }

        }

        //if joy stick is released 
        //stopallCoroutine
        //startCoroutine(deactivate HUD)

    }
}
