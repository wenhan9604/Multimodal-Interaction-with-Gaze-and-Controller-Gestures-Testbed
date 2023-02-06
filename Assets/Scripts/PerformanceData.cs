using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class PerformanceData
{
    #region Fields
    //private string fileName = "DataCollection.csv";
    private string reportDirectoryName = "DataCollectionReports";
    private string reportSeparator = ",";
    private string[] reportHeaders = new string[]
        {
            "TaskType",
            "PriTaskModality",
            "TargetSize",
            "TargetDestDistance",
            "PathDir",
            "PriTaskElapsed",
            "RotTaskStart",
            "RotTaskEnd",
            "RotTaskElapsed",
            //"ColTaskElapsed",
            "NumbWrongDeselection",
            "NumbPathExit",
            "PathFirstSegExit",
            "PathMidSegExit",
            "PathLastSegExit",
            "InputOneTiming",
            "InputTwoTiming",
            "NumbMismatchRotTask",
            //"NumbMismatchColTask"
            "Time Stamp"
        };

    #region DataFields
    public int Study_ID = 1;
    public int TaskType = 1;
    public PrimaryTaskInput PriTaskModality = PrimaryTaskInput.ControllerClick;
    public TargetSize TargetSize;
    public float TargetDestDistance = 0;
    public string PathDir = null;
    public float PriTaskElapsed = 0;
    public float RotTaskStart = 0;
    public float RotTaskEnd = 0;
    public float RotTaskElapsed = 0;
    public float ColTaskElapsed = 0;
    public int NumbWrongDeselection = 0;
    public int NumbPathExit = 0;
    public int PathFirstSegExit = 0;
    public int PathMidSegExit = 0;
    public int PathLastSegExit = 0;
    public float InputOneTiming = 0;
    public float InputTwoTiming = 0;
    public int NumbMismatchRotTask = 0;
    public int NumbMismatchColTask = 0;

    #endregion

    #endregion

    #region Interactions 

    public void AppendDataFieldsToReport()
    {
        VerifyDirectory();
        VerifyFile();

        using (StreamWriter sw = File.AppendText(GetFilePath()))
        {
            List<string> dataFields = ConvertDataFieldsToString();
            string finalString = "";
            for (int i = 0; i < dataFields.Count; i++)
            {
                if (finalString != "")
                {
                    finalString += reportSeparator;
                }
                finalString += dataFields[i];
            }
            finalString += reportSeparator + GetTimeStamp();
            sw.WriteLine(finalString);
        }
        ResetAllDataFields();
    }

    public void AppendToUnofficialReport()
    {
        VerifyDirectory();
        VerifyUnofficialReportFilePath();

        using (StreamWriter sw = File.AppendText(GetUnofficialReportFilePath()))
        {
            List<string> dataFields = ConvertDataFieldsToString();
            string finalString = "";
            for (int i = 0; i < dataFields.Count; i++)
            {
                if (finalString != "")
                {
                    finalString += reportSeparator;
                }
                finalString += dataFields[i];
            }
            finalString += reportSeparator + GetCurrentDate() + " / " + GetTimeStamp();
            sw.WriteLine(finalString);
        }
        ResetAllDataFields();
    }

    #endregion

    #region Operations 

    private void VerifyDirectory()
    {
        string dir = GetDirectoryPath();
        if(!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    private void VerifyFile()
    {
        string file = GetFilePath();
        if(!File.Exists(file))
        {
            CreateReport();
        }
    }

    private void CreateReport()
    {
        StreamWriter output = null;
        try
        {
            output = File.CreateText(GetFilePath());

            //Write headers for the new file. 
            string finalString = "";
            for (int i = 0; i < reportHeaders.Length; i++)
            {
                if (finalString != "")
                {
                    finalString += reportSeparator;
                }
                finalString += reportHeaders[i];
            }
            output.WriteLine(finalString);
        }

        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        finally
        {
            if (output != null)
            {
                output.Close();
            }
        }
    }

    private string GetDirectoryPath()
    {
        return Application.streamingAssetsPath + "/" + reportDirectoryName;
    }

    private string GetFilePath()
    {
        return GetDirectoryPath() + "/" + "_ID" + Study_ID + "_" + GetCurrentDate() + ".csv";
    }

    #region UnofficialReport

    private void VerifyUnofficialReportFilePath()
    {
        string file = GetUnofficialReportFilePath();
        if (!File.Exists(file))
        {
            CreateUnofficialReport();
        }
    }

    private void CreateUnofficialReport()
    {
        StreamWriter output = null;
        try
        {
            output = File.CreateText(GetUnofficialReportFilePath());

            //Write headers for the new file. 
            string finalString = "";
            for (int i = 0; i < reportHeaders.Length; i++)
            {
                if (finalString != "")
                {
                    finalString += reportSeparator;
                }
                finalString += reportHeaders[i];
            }
            output.WriteLine(finalString);
        }

        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        finally
        {
            if (output != null)
            {
                output.Close();
            }
        }
    }

    private string GetUnofficialReportFilePath()
    {
        return GetDirectoryPath() + "/" + "UnofficialReport" + ".csv";
    }

    #endregion

    private List<String> ConvertDataFieldsToString()
    {
        List<String> listOfDataFields = new List<string>();
        listOfDataFields.Add(TaskType.ToString());
        listOfDataFields.Add(PriTaskModality.ToString());
        listOfDataFields.Add(TargetSize.ToString());
        listOfDataFields.Add(TargetDestDistance.ToString("0.000"));
        listOfDataFields.Add(PathDir);
        listOfDataFields.Add(PriTaskElapsed.ToString("0.000"));
        listOfDataFields.Add(RotTaskStart.ToString("0.000"));
        listOfDataFields.Add(RotTaskEnd.ToString("0.000"));
        listOfDataFields.Add(RotTaskElapsed.ToString("0.000"));
        //listOfDataFields.Add(ColTaskElapsed.ToString("0.000"));
        listOfDataFields.Add(NumbWrongDeselection.ToString());
        listOfDataFields.Add(NumbPathExit.ToString());
        listOfDataFields.Add(PathFirstSegExit.ToString());
        listOfDataFields.Add(PathMidSegExit.ToString());
        listOfDataFields.Add(PathLastSegExit.ToString());
        listOfDataFields.Add(InputOneTiming.ToString("0.000"));
        listOfDataFields.Add(InputTwoTiming.ToString("0.000"));
        listOfDataFields.Add(NumbMismatchRotTask.ToString());
        //listOfDataFields.Add(NumbMismatchColTask.ToString());
        return listOfDataFields;
    }

    private string GetCurrentDate()
    {
        string currentDate = DateTime.Now.Date.ToString("dd");
        string currentMonth = DateTime.Now.Date.ToString("MMM");
        return currentDate + currentMonth;
    }

    private string GetTimeStamp()
    {
        return DateTime.Now.ToString("T");
    }

    private void ResetAllDataFields()
    {
        TaskType = 1;
        PriTaskModality = PrimaryTaskInput.ControllerClick;
        TargetSize = 0;
        TargetDestDistance = 0;
        PathDir = null;
        PriTaskElapsed = 0;
        RotTaskStart = 0;
        RotTaskEnd = 0;
        RotTaskElapsed = 0;
        ColTaskElapsed = 0;
        NumbWrongDeselection = 0;
        NumbPathExit = 0;
        PathFirstSegExit = 0;
        PathMidSegExit = 0;
        PathLastSegExit = 0;
        InputOneTiming = 0;
        InputTwoTiming = 0;
        NumbMismatchRotTask = 0;
        NumbMismatchColTask = 0;
    }

    #endregion
}
