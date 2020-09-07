using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.G2OM;

public class ToolTipScript : MonoBehaviour , IGazeFocusable
{
    [SerializeField] private ToolTipOnGaze toolTipGazeScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GazeFocusChanged( bool hasFocus)
    {
        toolTipGazeScript.GazeFocusChanged(hasFocus);
    }
}
