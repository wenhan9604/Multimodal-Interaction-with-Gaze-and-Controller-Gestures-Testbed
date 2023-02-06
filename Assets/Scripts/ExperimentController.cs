using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public enum SecondaryTasks
{
    Primary,
    PrimaryAndRotation
}

/// <summary>
/// Start, create and coordinate primary and secondary tasks.
/// </summary>
public class ExperimentController : MonoSingleton<ExperimentController>
{
    #region Fields

    //instances of all controllers 
    private PrimaryTaskController primaryTaskController;
    private RotationalSecTaskController rotSecondaryTaskController;
    private ColorChangeSecTaskController colorChangeSecTaskController;

    private GameObject targetObject;
    private SphericalTarget targetScript;

    //Support for controlling lifecycle of trials
    private bool isCoroutineRunning = false;
    private bool isSecondaryTaskActivatedInThisTrial = false;

    //Support for controlling lifecycle of session
    private int numbTrialInSession;

    //Experiment Settings
    [SerializeField] private SecondaryTasks taskType = SecondaryTasks.Primary;
    [SerializeField] private PrimaryTaskInput primaryTaskInput = PrimaryTaskInput.ControllerClick;
    [SerializeField] private int study_ID = 1;
    [SerializeField] private bool enableDataCollection = false;
    private float secondaryTaskTimeDelay = 0.2f;
    private float newTrialTimeDelay = 1f;

    //Support for Sec Task Input 
    //[Header("Number of pair of rotation directions: 1 to 3.")]
    private List<RotationDirectionPair> secTaskDirectionInputPairs = new List<RotationDirectionPair>()
    {
        RotationDirectionPair.leftAndRight
    };
    private Dictionary<RotationDirection, bool> secTaskDirInputFlag = new Dictionary<RotationDirection, bool>();


    #endregion

    #region Properties 
    public GameObject TargetObject
    {
        get { return targetObject; }
        private set
        {
            targetObject = value;
        }
    }

    public PrimaryTaskInput PrimaryTaskInput
    {
        get { return primaryTaskInput; }
    }

    public SecondaryTasks SecondaryTaskChosen
    {
        get { return taskType; }
    }

    public Dictionary<RotationDirection,bool> SecTaskInputDirectionFlag
    {
        get { return secTaskDirInputFlag; }
    }

    #endregion 

    public override void Initialize()
    {
        base.Initialize();
    }

    // Start Trial on start()
    void Start()
    {
        rotSecondaryTaskController = RotationalSecTaskController.Instance;
        primaryTaskController = PrimaryTaskController.Instance;
        colorChangeSecTaskController = ColorChangeSecTaskController.Instance;

        //Limit input directions for all secondary tasks.
        SetNumberOfTrialsInSession();
        SetAvailableDirectionInput(secTaskDirectionInputPairs);
        StartPrimaryTask();
    }

    // Update is called once per frame
    void Update()
    {
        switch (taskType)
        {
            case SecondaryTasks.Primary:
                if(!primaryTaskController.IsTaskInProgress && !isCoroutineRunning)
                {
                    StartCoroutine(EndCurrentTrialAndStartNewTrial(newTrialTimeDelay));
                    isCoroutineRunning = true;
                }
                break;

            case SecondaryTasks.PrimaryAndRotation:
                //Start Secondary task only when target is selected and  sec task is not in progress
                if (targetScript.IsTargetSelected && !rotSecondaryTaskController.IsTaskInProgress && !isCoroutineRunning && !isSecondaryTaskActivatedInThisTrial)
                {
                    StartCoroutine(StartRotationSecondaryTask(secondaryTaskTimeDelay));
                    isCoroutineRunning = true;
                }

                //End current trial and start new trial when both tasks are completed
                if (!primaryTaskController.IsTaskInProgress && !rotSecondaryTaskController.IsTaskInProgress && !isCoroutineRunning)
                {
                    StartCoroutine(EndCurrentTrialAndStartNewTrial(newTrialTimeDelay));
                    isCoroutineRunning = true;
                }
                break;
            /*
            case SecondaryTasks.ColorChange:
                if(targetScript.IsTargetSelected && !colorChangeSecTaskController.IsTaskInProgress && !isCoroutineRunning && !isSecondaryTaskActivatedInThisTrial)
                {
                    StartCoroutine(StartColorChangeSecTask(secondaryTaskTimeDelay));
                    isCoroutineRunning = true;
                }

                //End current trial and start new trial when both tasks are completed
                if (!primaryTaskController.IsTaskInProgress && !colorChangeSecTaskController.IsTaskInProgress && !isCoroutineRunning)
                {
                    StartCoroutine(EndCurrentTrialAndStartNewTrial(newTrialTimeDelay));
                    isCoroutineRunning = true;
                }
                break;
                */
        }
    }

    #region Operations

    private void StartPrimaryTask()
    {
        Debug.Log("<color=cyan>Trial Number:</color> " + numbTrialInSession);
        numbTrialInSession--;
        TargetObject = primaryTaskController.StartPrimaryTask();
        targetScript = TargetObject.GetComponent<SphericalTarget>();

        DataCollectionManager.Instance.StartDataCollection(study_ID);
    }

    private IEnumerator StartRotationSecondaryTask(float timeDelay)
    {
        yield return new WaitForSecondsRealtime(timeDelay);
        rotSecondaryTaskController.StartRotationSecTask(TargetObject);
        isSecondaryTaskActivatedInThisTrial = true;
        isCoroutineRunning = false;
    }

    private IEnumerator StartColorChangeSecTask(float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);
        colorChangeSecTaskController.StartColorChangeSecTask(TargetObject);
        isSecondaryTaskActivatedInThisTrial = true;
        isCoroutineRunning = false;
    }

    private IEnumerator EndCurrentTrialAndStartNewTrial(float timeDelay)
    {
        if (TargetObject != null)
        {
            Destroy(TargetObject,0.5f);
        }

        isSecondaryTaskActivatedInThisTrial = false;

        if (enableDataCollection)
        {
            DataCollectionManager.Instance.AppendToParticipantReport();
        }
        else
        {
            DataCollectionManager.Instance.AppendToUnofficialReport();
        }

        //Cant start new trial until target is destroyed
        while (TargetObject)
        {
            yield return null;
        }

        yield return new WaitForSeconds(timeDelay);

        if (numbTrialInSession == 0)
        {
            EndExperimentSession();
        }
        else
        { 
        StartPrimaryTask();
        isCoroutineRunning = false;
        }
    }

    private void SetAvailableDirectionInput(List<RotationDirectionPair> rotDirPairs)
    {
        foreach (var rotationDir in RotationDirectionUtility.GetAllEnumValues<RotationDirection>())
        {
            secTaskDirInputFlag[rotationDir] = false;
        }

        //Set the number of rotation directions available for input
        List<RotationDirection> directionsAvail = RotationDirectionUtility.GetRotationDirectionsFromRotDirPairs(rotDirPairs);

        foreach (var dirInput in directionsAvail)
        {
            if (secTaskDirInputFlag.ContainsKey(dirInput))
            {
                secTaskDirInputFlag[dirInput] = true;
            }
        }
    }

    private void SetNumberOfTrialsInSession()
    {
        numbTrialInSession = 90;
    }

    private void EndExperimentSession()
    {
        TestConditionManager.Instance.UpdateUsedPermutations();
        if(EditorApplication.isPlaying)
        {
            EditorApplication.ExitPlaymode();
        }
    }

    #endregion
}
