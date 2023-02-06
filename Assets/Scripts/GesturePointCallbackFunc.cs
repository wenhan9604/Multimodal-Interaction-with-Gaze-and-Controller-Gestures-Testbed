using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GesturePointCallbackFunc : MonoBehaviour
{
    public RotationDirection rotationDir;
    public System.Action<RotationDirection> Callback = null;

    private void OnTriggerEnter(Collider other)
    {
        if (Callback != null)
            Callback(rotationDir);
    }
}

