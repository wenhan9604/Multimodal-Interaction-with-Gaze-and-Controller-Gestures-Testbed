using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetColorChange : MonoBehaviour
{
    #region Field
    private Material targetMat;
    [SerializeField] private float colorChangeDuration = 0.5f;

    //ColorCue support
    [SerializeField] private GameObject cueText;
    [SerializeField] private GameObject colorCueGreen;
    [SerializeField] private GameObject colorCueYellow;

    #endregion

    private void OnEnable()
    {
        GestureSecTaskInput.OnGestureSecTaskInput += StartColorChangeAnimation;
        ControllerSecTaskInput.OnControllerSecTaskInput += StartColorChangeAnimation;
    }

    private void OnDisable()
    {
        GestureSecTaskInput.OnGestureSecTaskInput -= StartColorChangeAnimation;
        ControllerSecTaskInput.OnControllerSecTaskInput -= StartColorChangeAnimation;

    }

    private void Start()
    {
        targetMat = GetComponent<MeshRenderer>().material;
    }

    #region Interactions

    public void InitializeColorCues(Color correctColor)
    {
        if(ColorInputUtility.TryGetColorName(correctColor, out string colorName))
        {
            cueText.SetActive(true);
            colorCueGreen.SetActive(true);
            colorCueYellow.SetActive(true);
            cueText.GetComponent<TextMesh>().text = colorName;
            GameObject textBG = cueText.transform.Find("TextBackGround").gameObject;
            if (textBG)
            {
                textBG.GetComponent<MeshRenderer>().material.color = correctColor;
                textBG.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", correctColor);
            }
        }
    }

    public void DestroyColorCues()
    {
        cueText.SetActive(false);
        colorCueGreen.SetActive(false);
        colorCueYellow.SetActive(false);
    }

    #endregion

    #region Operations

    private void StartColorChangeAnimation(RotationDirection inputDirection)
    {
        var currentColor = targetMat.color;
        Color finalColor = Color.clear;
        switch (inputDirection)
        {
            case RotationDirection.left:
                finalColor = Color.green;
                break;
            case RotationDirection.right:
                finalColor = Color.yellow;
                break;
        }
        StartCoroutine(ChangeColor(currentColor, finalColor));
    }

    private IEnumerator ChangeColor(Color initialColor, Color finalColor)
    {
        float progress = 0;
        Color newColor;

        while (progress < 1.0f)
        {
            progress += Time.deltaTime / colorChangeDuration;
            newColor = Color.Lerp(initialColor, finalColor, progress);
            targetMat.color = newColor;
            yield return null;
        }
    }

    #endregion
}
