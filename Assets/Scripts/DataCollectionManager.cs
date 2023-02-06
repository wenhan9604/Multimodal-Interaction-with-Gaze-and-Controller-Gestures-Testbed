using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Global Reference to updaate data fields to CSV file. 
/// Parse information to correct format for the data field. 
/// </summary>
public class DataCollectionManager : MonoSingleton<DataCollectionManager>
{
    #region Fields
    private PerformanceData perfomanceData;
    private float priTaskTimeStart;
    private float rotTaskTimeStart;
    private float colTaskTimeStart;

    #endregion

    public override void Initialize()
    {
        perfomanceData = new PerformanceData();
    }

    #region Interactions

    /// <summary>
    /// Update relevant data fields when trial starts
    /// </summary>
    /// <param name="study_ID">Study ID of participant. Necessary to update! </param>
    public void StartDataCollection(int study_ID)
    {
        perfomanceData.Study_ID = study_ID;
        perfomanceData.TaskType = ConvertTaskTypeEnumToInt(ExperimentController.Instance.SecondaryTaskChosen);

        //if rotation task not in use, the rot task parameters will be null
        if(perfomanceData.TaskType == 1)
        {
            perfomanceData.RotTaskStart = 9999f;
            perfomanceData.RotTaskElapsed = 9999f;
            perfomanceData.RotTaskEnd = 9999f;
            perfomanceData.InputOneTiming = 9999f;
            perfomanceData.InputTwoTiming = 9999f;
            perfomanceData.NumbMismatchRotTask = 9999;
        }

        perfomanceData.PriTaskModality = ExperimentController.Instance.PrimaryTaskInput;
        perfomanceData.TargetSize = PrimaryTaskController.Instance.TargetObjectSize;
        perfomanceData.TargetDestDistance = (float)PrimaryTaskController.Instance.TargetDestinationAngularDistance;
    }

    public void UpdatePathDirection(PathCurvature direction)
    {
        perfomanceData.PathDir = direction.ToString();
    }

    /// <summary>
    /// Start timer in data manager when task starts.
    /// </summary>
    /// <param name="taskName">Enter either "PriTask", "RotTask" or "ColTask"</param>
    public void StartTaskTimer(string taskName)
    {
        switch(taskName)
        {
            case "PriTask":
                priTaskTimeStart = Time.time;
                //Debug.Log(taskName + " started at " + priTaskTimeStart.ToString("0.000"));
                break;
            case "RotTask":
                rotTaskTimeStart = Time.time - priTaskTimeStart;
                perfomanceData.RotTaskStart = rotTaskTimeStart;
                //Debug.Log(taskName + " started at " + rotTaskTimeStart.ToString("0.000"));
                break;
            case "ColTask":
                colTaskTimeStart = Time.time;
                //Debug.Log(taskName + " started at " + colTaskTimeStart.ToString("0.000"));
                break;
            default:
                Debug.Log("Error in calling method, please change method input");
                break;
        }
    }

    /// <summary>
    /// End timer for task in data manager when task ends. Store the time elapsed for the completion time of this task.
    /// </summary>
    /// <param name="taskName">Enter either "PriTask", "RotTask" or "ColTask"</param>
    public void EndTaskTimer(string taskName)
    {
        switch (taskName)
        {
            case "PriTask":
                perfomanceData.PriTaskElapsed = Time.time - priTaskTimeStart;
                //Debug.Log("Primary Task Time elapsed: " + perfomanceData.PriTaskElapsed.ToString("0.000"));
                break;
            case "RotTask":
                perfomanceData.RotTaskEnd = Time.time - priTaskTimeStart;
                perfomanceData.RotTaskElapsed = perfomanceData.RotTaskEnd - rotTaskTimeStart;
                //Debug.Log("Rot Task Time elapsed: " + perfomanceData.RotTaskElapsed.ToString("0.000"));
                break;
            case "ColTask":
                perfomanceData.ColTaskElapsed = Time.time - colTaskTimeStart;
                //Debug.Log("Col Task Time elapsed: " + perfomanceData.ColTaskElapsed.ToString("0.000"));
                break;
            default:
                Debug.Log("Error in calling method, please change method input");
                break;
        }
    }

    public void UpdateWrongTargetDeselection()
    {
        perfomanceData.NumbWrongDeselection++;
        //Debug.Log("Number of times wrong deselection : " + perfomanceData.NumbWrongDeselection);
    }

    public void UpdateOnTargetExitPath(string sectionName)
    {
        switch(sectionName)
        {
            case "First Segment":
                perfomanceData.PathFirstSegExit++;
                break;
            case "Middle Segment":
                perfomanceData.PathMidSegExit++;
                break;
            case "Last Segment":
                perfomanceData.PathLastSegExit++;
                break;
            default:
                Debug.Log("Error: Path Exit not registered");
                break;
        }
        perfomanceData.NumbPathExit++;
        //Debug.Log("Number of times target exited path: " + perfomanceData.NumbPathExit);
    }

    /// <summary>
    /// Register rotation task input timing
    /// </summary>
    /// <param name="inputNumb"> Input number. Either 1 or 2</param>
    public void UpdateRotTaskInputTiming(int inputNumb)
    {
        switch(inputNumb)
        {
            case 1:
                perfomanceData.InputOneTiming = Time.time - priTaskTimeStart;
                break;
            case 2:
                perfomanceData.InputTwoTiming = Time.time - priTaskTimeStart;
                break;
            default:
                Debug.Log("System did not register rot task input timing");
                break;
        }
    }

    public void UpdateWrongRotTaskInput(RotationDirection inputDirection)
    {
        perfomanceData.NumbMismatchRotTask++;
        //Debug.Log("Rotation Input Mismatch: " + inputDirection.ToString());
    }

    public void UpdateWrongColTaskInput(RotationDirection inputDirection)
    {
        perfomanceData.NumbMismatchColTask++;
    }

    /// <summary>
    /// End data collection for this trial
    /// </summary>
    public void AppendToParticipantReport()
    {
        perfomanceData.AppendDataFieldsToReport();
        Debug.Log("<color=cyan>Data appended to Participant report, ID:</color> " + perfomanceData.Study_ID);
        ResetFields();
    }

    public void AppendToUnofficialReport()
    {
        perfomanceData.AppendToUnofficialReport();
        ResetFields();
    }
    #endregion

    #region Operations

    private int ConvertTaskTypeEnumToInt(SecondaryTasks taskType)
    {
        int returnable = 1;
        switch(taskType)
        {
            case SecondaryTasks.Primary:
                {
                    returnable = 1;
                    break;
                }
            case SecondaryTasks.PrimaryAndRotation:
                {
                    returnable = 2;
                    break;
                }
            /*case SecondaryTasks.ColorChange:
                {
                    returnable = 3;
                    break;
                }
            */
        }
        return returnable;
    }

    private void ResetFields()
    {
        priTaskTimeStart = 0;
        rotTaskTimeStart = 0;
        colTaskTimeStart = 0;
    }

    #endregion
}
