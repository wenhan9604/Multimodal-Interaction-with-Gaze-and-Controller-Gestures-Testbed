using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightPath : MonoBehaviour
{
    private Material pathMat;
    private Color defaultColor;

    private void Awake()
    {
        pathMat = GetComponent<LineRenderer>().material;
        defaultColor = pathMat.GetColor("_TintColor");
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Target"))
        {
            pathMat.SetColor("_TintColor",defaultColor);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Target"))
        {
            pathMat.SetColor("_TintColor", Color.red);
        }
    }
}
