using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangeSecTaskController : MonoSingleton<ColorChangeSecTaskController>
{
    #region Fields 
    //Initialize Settings
    [SerializeField] private GameObject gestureInputObject;
    [SerializeField] private ControllerSecTaskInput controllerSecTaskInput;
    private GameObject targetObject;
    private TargetColorChange targetColorChangeScript;

    //Color Cue Support 
    private Color correctColor;
    private RandomValueGenerator randomValueGenerator = new RandomValueGenerator(2, 18);

    //Task Status
    private bool isTaskInProgress;

    #endregion

    #region Properties 

    public bool IsTaskInProgress
    {
        get
        {
            return isTaskInProgress;
        }
        private set
        {
            isTaskInProgress = value;
        }
    }

    #endregion

    public override void Initialize()
    {
        base.Initialize();
        GestureSecTaskInput.OnGestureSecTaskInput += UpdateColorCue;
        ControllerSecTaskInput.OnControllerSecTaskInput += UpdateColorCue;
    }

    private void OnDisable()
    {
        GestureSecTaskInput.OnGestureSecTaskInput -= UpdateColorCue;
        ControllerSecTaskInput.OnControllerSecTaskInput -= UpdateColorCue;
    }

    private void Start()
    {
        gestureInputObject.SetActive(false);
        controllerSecTaskInput.enabled = false;
    }

    #region Interactions

    public void StartColorChangeSecTask(GameObject target)
    {
        //Initialize settings
        this.targetObject = target;
        if (targetObject.transform.Find("Cube").TryGetComponent<TargetColorChange>(out TargetColorChange component))
        {
            targetColorChangeScript = component;
            targetColorChangeScript.enabled = true;
        }

        //Initialize color cues.
        correctColor = GenerateRandomColorCues();
        targetColorChangeScript.InitializeColorCues(correctColor);

        //Allow secondary task input 
        gestureInputObject.SetActive(true);
        controllerSecTaskInput.enabled = true;

        IsTaskInProgress = true;

        DataCollectionManager.Instance.StartTaskTimer("ColTask");
    }

    private void UpdateColorCue(RotationDirection inputDirection)
    {
        if(IsTaskInProgress)
        {
            if(ColorInputUtility.TryGetColorDirectionInput(correctColor,out RotationDirection correctDirInput))
            {
                if(inputDirection == correctDirInput)//Input match with Cue
                {
                    //Update Cue at TargetScript
                    correctColor = Color.clear;
                    targetColorChangeScript.DestroyColorCues();
                    EndColorChangeSecTask();
                }
                else
                {
                    //Input doesnt match with Cue
                    DataCollectionManager.Instance.UpdateWrongColTaskInput(inputDirection);
                }
            }
        }
    }

    private void EndColorChangeSecTask()
    {
        gestureInputObject.SetActive(false);
        controllerSecTaskInput.enabled = false;
        targetColorChangeScript.enabled = false;

        IsTaskInProgress = false;

        DataCollectionManager.Instance.EndTaskTimer("ColTask");
    }

    #endregion

    #region Operations

    /// <summary>
    /// Generate color cues. Choose a single color to between a List of colors: green/yellow.
    /// </summary>
    private Color GenerateRandomColorCues()
    {
        List <Color> colorChoices= new List<Color>()
        {
            Color.green,
            Color.yellow
        };

        Color chosenColor = colorChoices[randomValueGenerator.GetRandomValue()];
        Debug.Log("ColorCueCountLeft: " + randomValueGenerator.NumberOfChoicesRemaining);
        return chosenColor;
    }

    #endregion
}
