using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularPath : MonoBehaviour
{
    private Material pathMat;
    private Color defaultColor;

    private void Awake()
    {
        pathMat = GetComponent<MeshRenderer>().material;
        defaultColor = pathMat.GetColor("_Color");
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Target"))
        {
            pathMat.SetColor("_Color", defaultColor);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Target"))
        {
            pathMat.SetColor("_Color", Color.red);
        }
    }
}
