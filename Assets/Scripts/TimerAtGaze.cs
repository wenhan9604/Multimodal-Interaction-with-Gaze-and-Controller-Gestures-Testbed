using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.G2OM;

public class TimerAtGaze : MonoBehaviour , IGazeFocusable
{
    //event when timer stops, represents one timing
    public delegate void TimerStop(string name, float timing);
    public static event TimerStop timerstop;

    private float timer;
    private bool timerStatus;
    private bool TimerStatus
    {
        get
        {
            return timerStatus;
        }
        set
        {
            // when user stops focusing on this target
            // Timer stops and event sends out time of this gaze
            // resets timer
            if (value == false) 
            {
                //Debug.Log("Timer value was " + timer + "Object name " + this.gameObject.name);
                if (timerstop != null)
                    timerstop(gameObject.name, timer);
                timer = 0;
            }
            timerStatus = value;
        }
    }
    public void GazeFocusChanged(bool hasFocus)
    {
        TimerStatus = hasFocus;
    }

    // Update is called once per frame
    void Update()
    {
        if(TimerStatus)
        {
            timer += Time.deltaTime;
        }
    }
}
