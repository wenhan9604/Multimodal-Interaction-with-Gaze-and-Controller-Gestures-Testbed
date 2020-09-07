using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.XR;
using Tobii.G2OM;

public class FixationSceneController : MonoBehaviour
{
    //private Dictionary<string, float> targetTimingPairs = new Dictionary<string, float>();
    public List<string> targetNameList = new List<string>();
    public List<float> timingList = new List<float>();

    // Start is called before the first frame update
    void Start()
    {
        TimerAtGaze.timerstop += RegisterTimer;
    }

    private void OnDisable()
    {
        TimerAtGaze.timerstop -= RegisterTimer;
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            foreach(var item in targetNameList)
                {
                    Debug.Log("target name: " + item);
                }

            foreach (var item in timingList)
            {
                Debug.Log("timing: " + item);
            }
        }
    }

    void RegisterTimer(string targetName, float timing)
    {
        //targetTimingPairs.Add(targetName, timing);
        targetNameList.Add(targetName);
        timingList.Add(timing);
    }
}
