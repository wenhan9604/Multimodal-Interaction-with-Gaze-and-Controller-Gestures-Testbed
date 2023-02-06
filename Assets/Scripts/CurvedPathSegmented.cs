using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedPathSegmented : MonoBehaviour
{
    #region Fields
    
    private Color defaultPathColor = new Color(0, 0.5939f, 1f);
    private List<GameObject> curvedPathSections = new List<GameObject>();
    private bool isDestinationHoveredOn;
    private bool isOtherSectionColliding = false;
    private bool isAnySectionColliding = false;
    private string lastCollidingSection;
    
    #endregion

    private void OnEnable()
    {
        Destination.OnDestinationHovered += OnDestinationHovered;

        foreach (Transform child in transform)
        {
            curvedPathSections.Add(child.gameObject);
        }
    }

    private void OnDisable()
    {
        Destination.OnDestinationHovered -= OnDestinationHovered;
    }

    #region Interactions

    public void OnCollisionWithCurvedPathSections(string sectionName , bool isSectionColliding)
    {
        if(isAnySectionColliding != isSectionColliding)
        {
            if (isSectionColliding) //When change from no collision to have collision
            {
                isAnySectionColliding = true;
                ChangeCurvedPathColor(defaultPathColor);
            }
            //When changing from have collision to no collision 
            //Check if any child is currently colliding before setting to false
            else
            {
                lastCollidingSection = sectionName;
                CheckForCollisionInOtherSections();
            }
        }
    }

    void OnDestinationHovered(bool isHoveredOn)
    {
        isDestinationHoveredOn = isHoveredOn;

        if(isDestinationHoveredOn)
        {
            ChangeCurvedPathColor(defaultPathColor);
        }    

        if (!isAnySectionColliding && !isDestinationHoveredOn)
        {
            NoCollisionInCurvedPathAndDestination("Last Segment");
        }
    }

    #endregion

    #region Operations

    void CheckForCollisionInOtherSections()
    {
        //Check if other sections is colliding with target
        foreach (var child in curvedPathSections)
        {
            if (child.GetComponent<CurvedPathSection>().IsCollidingWithTarget)
                isOtherSectionColliding = true;
        }

        // If any section is colliding, nothing happens.

        // When no section is colliding 
        if (!isOtherSectionColliding)
        {
            isAnySectionColliding = false;

            if (!isAnySectionColliding && !isDestinationHoveredOn)
            {
                NoCollisionInCurvedPathAndDestination(lastCollidingSection);
            }
        }
        //Reset Flag 
        isOtherSectionColliding = false;
    }

    void NoCollisionInCurvedPathAndDestination(string lastCollidingSection)
    {
        ChangeCurvedPathColor(Color.red);
        DataCollectionManager.Instance.UpdateOnTargetExitPath(lastCollidingSection);
    }

    void ChangeCurvedPathColor(Color inputColor)
    {
        foreach (GameObject child in curvedPathSections)
        {
            child.GetComponent<MeshRenderer>().material.color = inputColor;
        }
    }

    #endregion 
}
