using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Tobii.G2OM;

public class PlayVideoOnGaze : MonoBehaviour , IGazeFocusable
{
    [SerializeField] private VideoPlayer videoPlayer;
    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GazeFocusChanged(bool hasFocus)
    {
        //if true, set playback speed to 1 
        if (hasFocus)
        {
            videoPlayer.Play();
            //highlights the video
        }

        //if false, set playback speed to 0
        else
        {
            videoPlayer.Pause();
            //unhighlight the video 
        }
    }
}
