using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedPathSection : MonoBehaviour
{
    private bool isCollidingWithTarget = false;
    public bool IsCollidingWithTarget
    {
        get { return isCollidingWithTarget; }
        private set
        {
            isCollidingWithTarget = value;
        }
    }

    private CurvedPathSegmented parentScript;

    private void OnEnable()
    {
        parentScript = transform.parent.gameObject.GetComponent<CurvedPathSegmented>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Target"))
        {
            IsCollidingWithTarget = true;
            parentScript.OnCollisionWithCurvedPathSections(name, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            IsCollidingWithTarget = false;
            parentScript.OnCollisionWithCurvedPathSections(name, false);
        }
    }
}
