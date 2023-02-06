using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public struct TestConditionBlock
{
    #region Fields
    private int blockID;
    private int groupingID;
    private TargetSize targetSize;
    private TargetDestinationAngDist targetDestAngularDistance;
    private PathCurvature pathDirection;
    #endregion

    #region Properties
    public int BlockID { get { return blockID; } }
    public int GroupingID { get { return groupingID; } }
    public TargetSize TargetSize { get { return targetSize; } }
    public TargetDestinationAngDist TargetDestinationAngularDistance { get { return targetDestAngularDistance; } }
    public PathCurvature PathDirection { get { return pathDirection; } }
    #endregion

    public TestConditionBlock
            (
            int ID,
            int groupingID,
            TargetSize targetSize,
            TargetDestinationAngDist targetDestAngularDist,
            PathCurvature pathDirection
            )
    {
        blockID = ID;
        this.groupingID = groupingID;
        this.targetSize = targetSize;
        this.targetDestAngularDistance = targetDestAngularDist;
        this.pathDirection = pathDirection;
    }
}

public class TestConditionInfoData 
{
    private List<TestConditionBlock> testConditionBlocks = new List<TestConditionBlock>();

    public List<TestConditionBlock> TestConditionBlocks { get { return testConditionBlocks; } }

    /// <summary>
    /// Instantiate a new TestConditionInfoData object. Read test condition blocks info from csv file.
    /// </summary>
    public TestConditionInfoData()
    {
        StreamReader input = null;
        try
        {
            input = File.OpenText(GetFilePath());

            string headers = input.ReadLine();

            string line = input.ReadLine();
            while (line != null)
            {
                InitializeTestConditionBlocks(line);
                line = input.ReadLine();
            }
        }
        catch(Exception exception)
        {
            Debug.Log(exception.Message);
        }
        finally
        {
            if(input != null)
            {
                input.Close();
            }
        }
    }

    #region Operations

    private string GetFilePath()
    {
        return Application.streamingAssetsPath + "/" + "TestCondition" + "/" + "Test_Conditions_Info.csv";
    }

    private void InitializeTestConditionBlocks(string csvValues)
    {
        string[] values = csvValues.Split(',');
        TestConditionBlock newTestCondBlock = new TestConditionBlock(
            int.Parse(values[0]),
            int.Parse(values[1]),
            TargetUtils.ConvertToTargetSize(values[2]),
            TargetUtils.ConvertToTargetDestAngularDistance(values[3]),
            TargetUtils.ConvertToPathCurvatureDirection(values[4])
            );
        testConditionBlocks.Add(newTestCondBlock);
    }

    #endregion
}
