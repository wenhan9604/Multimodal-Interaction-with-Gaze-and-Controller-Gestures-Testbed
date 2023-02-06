using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    [SerializeField] private float defaultLength = 5.0f;
    [SerializeField] private GameObject dot;

    private LineRenderer lineRenderer = null;

    private void Awake()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        UpdateLine();   
    }

    private void UpdateLine()
    {
        //Use default or Distance 
        float targetLength = defaultLength;//may not be necessary

        //Raycast
        RaycastHit hit = CreateRayCast(targetLength);

        //End position of line based on Default length
        Vector3 lineEndPos = transform.position + (transform.forward * targetLength);

        //End position of line based on hit position
        if (hit.collider != null)
            lineEndPos = hit.point;

        //Set Position of dot 
        dot.transform.position = lineEndPos;

        //Set Line Renderer
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, lineEndPos);

    }

    private RaycastHit CreateRayCast(float length)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, defaultLength);

        return hit;
    }
}
