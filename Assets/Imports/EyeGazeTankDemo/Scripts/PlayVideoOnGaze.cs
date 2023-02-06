using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Tobii.G2OM;

//Play video upon Gaze
// Upon leaving Gaze, starts timer. Once timer fills up, stop video and off volume
public class PlayVideoOnGaze : MonoBehaviour , IGazeFocusable
{
    [SerializeField] private VideoPlayer videoPlayer;

    //Timer setting
    [SerializeField] private float timeToFill = 3.0f;
    private float timer;
    private float volume;

    private bool isEasingOut;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        volume = videoPlayer.GetDirectAudioVolume(0);
        Debug.Log("Volume initiated: " + volume);
    }


    public void GazeFocusChanged(bool hasFocus)
    {
        if(hasFocus)
        {
            isEasingOut = false;
            timer = 0;
            volume = 1;
            videoPlayer.SetDirectAudioVolume(0, volume);
            videoPlayer.Play();
        }

        else
        {
            isEasingOut = true;
        }
    }

    void Update()
    {
        if(isEasingOut)
        {
            timer += Time.deltaTime;
            volume = Mathf.Lerp(1, 0, timer / timeToFill);
            videoPlayer.SetDirectAudioVolume(0, volume);

            if(timer > timeToFill)
            {
                videoPlayer.Pause();
            }
        }
    }

    /*
    public void GazeFocusChanged(bool hasFocus)
    {
        if (hasFocus)
        {
            Debug.Log("Beginning Video Play!");
            StopCoroutine(VideoEaseOut());
            timer = 0;
            volume = 1;
            videoPlayer.SetDirectAudioVolume(0, volume);
            videoPlayer.Play();
        }

        //if lose gaze, starts timer.
        //volume ease in with timer.
        //once timer fills up, pause video 
        else
        {
            Debug.Log("Stopping Video Play!");
            StartCoroutine(VideoEaseOut());
        }
    }

    private IEnumerator VideoEaseOut()
    {
        while (timer < timeToFill)
        {
            timer += Time.deltaTime;
            volume = Mathf.Lerp(1, 0, timer / timeToFill);
            videoPlayer.SetDirectAudioVolume(0, volume);
            Debug.Log("current timer: " + timer);

            yield return null;
        }
        Debug.Log("Coroutine finished!");

        videoPlayer.Pause();
    }*/
}
