using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

/// <summary>
/// Stream test condition permutations from csv file. 
/// Able to read unused permutations.
/// Able to write used permutations.
/// </summary>
public class PermutationData 
{
    private Dictionary<List<int>, bool> testCondGroupingPermutations = new Dictionary<List<int>, bool>();

    public Dictionary<List<int>, bool> TestConditionGroupingPermutations
    {
        get { return testCondGroupingPermutations; }
    }

    #region Interactions

    /// <summary>
    /// Instantiate object. Read data from Permutation.csv
    /// </summary>
    public PermutationData ()
    {
        StreamReader input = null;
        try
        {
            input = File.OpenText(GetFilePath());

            string line = input.ReadLine();
            //Read to the end of file
            while (line != null)
            {
                InitializePermutationDataFields(line);
                line = input.ReadLine();
            }
        }
        catch (Exception exception)
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

    public void UpdateUsedPermutations(List<int> dictKey)
    {
        if(testCondGroupingPermutations.ContainsKey(dictKey))
        {
            testCondGroupingPermutations[dictKey] = true;
        }
    }

    public void AppendToCSVFile()
    {
        StreamWriter input = null;
        try
        {
            input = new StreamWriter(GetFilePath());
            List<string> csvFields = new List<string>();

            //Iterate through all permutations in dictionary
            for(int ii = 0; ii < testCondGroupingPermutations.Count; ii++)
            {
                csvFields = GetPermutationInString(ii);

                //construct output string from permutation
                string finalString = "";
                for(int i = 0; i < csvFields.Count; i++)
                {
                    if(finalString != "" )
                    {
                        finalString += ",";
                    }
                    finalString += csvFields[i];
                }
                input.WriteLine(finalString);
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

    #endregion 

    #region Operations

    private string GetFilePath()
    {
        return Application.streamingAssetsPath + "/" + "TestCondition" + "/" + "Test_Conditions_Permutations.csv";
    }

    private void InitializePermutationDataFields(string csvValue)
    {
        string[] values = csvValue.Split(',');
        List<int> tempIntList = new List<int>();

        bool flag = false;
        int flagIndex = values.Length - 1;
        string flagValue = values[flagIndex];
        if(flagValue == "y")
        {
            flag = true;
        }
 
        for (int i = 0; i< flagIndex; i++)
        {
            tempIntList.Add(int.Parse(values[i]));
        }

        testCondGroupingPermutations.Add(tempIntList, flag);
    }

    private List<string> GetPermutationInString(int dictionaryIndex)
    {
        List<string> line = new List<string>();
        List<int> permutation = testCondGroupingPermutations.ElementAtOrDefault(dictionaryIndex).Key;
        bool flag = testCondGroupingPermutations.ElementAtOrDefault(dictionaryIndex).Value;
        foreach(var value in permutation)
        {
            line.Add(value.ToString());
        }
        line.Add(flag ? "y" : "n");

        return line;
    }

    #endregion
}
