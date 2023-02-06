using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedPath : MonoBehaviour
{
    private Material pathMat;
    private Color defaultColor;

    private void Awake()
    {
        pathMat = GetComponent<MeshRenderer>().material;
        defaultColor = pathMat.color;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Target"))
        {
            pathMat.color = defaultColor;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Target"))
        {
            pathMat.color = Color.red;
            //DataCollectionManager.Instance.UpdateOnTargetExitPath();
        }
    }
}
