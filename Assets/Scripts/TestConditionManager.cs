using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestConditionManager : MonoSingleton<TestConditionManager>
{
    private PermutationData permutationData;
    private TestConditionInfoData testConditionInfoData;
    private List<TestConditionBlock> currentBlockSequences;
    private List<int> currentGroupingPermutation;

    #region Interactions

    public override void Initialize()
    {
        base.Initialize();
        permutationData = new PermutationData();
        testConditionInfoData = new TestConditionInfoData();

        //Initialize test conditions and their permutations
        if(currentBlockSequences == null)
        {
            currentBlockSequences = GetNewTestConditionPermutation();
        }   
    }

    public TestConditionBlock GetCurrentTestConditions ()
    {
        if(currentBlockSequences.Count == 0)
        {
            currentBlockSequences = GetNewTestConditionPermutation();
            Debug.Log("<color=orange>New Test Condition Initialized!</color>");
        }

        TestConditionBlock currentBlock = currentBlockSequences[0];
        currentBlockSequences.RemoveAt(0);
        return currentBlock;
    }

    public void UpdateUsedPermutations()
    {
        permutationData.AppendToCSVFile();
    }

    #endregion

    #region Operations

    private List<TestConditionBlock> GetNewTestConditionPermutation()
    {
        //Only update used Permutations when previous permutation is used
        if(currentGroupingPermutation != null)
        {
            permutationData.UpdateUsedPermutations(currentGroupingPermutation);
        }

        currentGroupingPermutation = GetNewGroupingPermutation();

        return GetBlockSequences(currentGroupingPermutation);
    }
        
    /// <summary>
    /// Return an unused test condition permutation from PermutationData
    /// </summary>
    private List<int> GetNewGroupingPermutation()
    {
        int dictLength;
        KeyValuePair<List<int>, bool> currentKeyValuePair = new KeyValuePair<List<int>, bool>();
        List<int> newGroupingsPermutation = new List<int>();

        dictLength = permutationData.TestConditionGroupingPermutations.Count;

        bool isPermutationFound = false;
        while (isPermutationFound == false)
        {
            currentKeyValuePair = permutationData.TestConditionGroupingPermutations.ElementAtOrDefault(Random.Range(0, dictLength));

            //an unused permutation is found! 
            if (currentKeyValuePair.Value == false)
            {
                newGroupingsPermutation = currentKeyValuePair.Key;
                isPermutationFound = true;
            }
        }
        return newGroupingsPermutation;
    }

    /// <summary>
    /// Arrange test condition blocks wrt to the grouping permutation.
    /// </summary>
    /// <param name="groupingsPermutation"></param>
    /// <returns> Rearranged list of test condition blocks</returns>
    private List<TestConditionBlock> GetBlockSequences(List<int> groupingsPermutation)
    {
        List<TestConditionBlock> newBlockSequence = new List<TestConditionBlock>();

        foreach(var groupingIndex in groupingsPermutation)
        {
            foreach(var testConditionBlock in  testConditionInfoData.TestConditionBlocks)
            {
                if(testConditionBlock.GroupingID == groupingIndex)
                {
                    newBlockSequence.Add(testConditionBlock);
                }
            }
        }

        return newBlockSequence;
    }

    #endregion
}
