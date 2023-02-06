using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a random output. Output selected from choices which have equal probabilities in a given number of occurrences.
/// </summary>
public class RandomValueGenerator
{
    #region Fields
    private int totalOccurrences;
    private int numberOfChoices;
    List<int> choicesList;
    #endregion

    public int NumberOfChoicesRemaining { get { return choicesList.Count; } }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="numberOfChoices">input 2 to get truefalse list</param>
    /// <param name="totalOccurrences">Needs to be an even number</param>
    public RandomValueGenerator(int numberOfChoices , int totalOccurrences)
    {
        this.totalOccurrences = totalOccurrences;
        this.numberOfChoices = numberOfChoices;
        if(numberOfChoices ==2)
            GenerateChoicesList(numberOfChoices, totalOccurrences);
    }

    public int GetRandomValue()
    {
        //Generate list if list == 0
        if (choicesList.Count == 0)
        {
            GenerateChoicesList(numberOfChoices, totalOccurrences);
        }
        int randomElementIndex = Random.Range(0, choicesList.Count);
        int randomValue = choicesList[randomElementIndex];
        choicesList.RemoveAt(randomElementIndex);

        return randomValue;
    }

    /*
    /// <summary>
    /// Get a list of equal number of true and false
    /// </summary>
    /// <param name="listCount">Must be an even number</param>
    /// <returns></returns>
    private void GenerateTrueFalseList(int listCount)
    {
        List<bool> returnable = new List<bool>(listCount);

        if (listCount % 2 == 1)
        {
            Debug.Log("Error: ListCount is not an even number.");
            // choicesList = null
        }
        else
        {
            for (int i = 0; i < listCount / 2; i++)
            {
                returnable[i] = false;
            }
            for (int ii = listCount / 2; ii < listCount; ii++)
            {
                returnable[ii] = true;
            }
            //choicesList = truefalselist
        }
        choicesList = returnable;
    }
    */
    private void GenerateChoicesList(int numberOfChoices , int listCount)
    {
        List<int> returnable = new List<int>();
        if (listCount % numberOfChoices == 1)
        {
            Debug.Log("Error: listCount is not divisible by number of choices.");
            // choicesList = null
        }
        else
        {
            
            for (int i = 0; i < (listCount/numberOfChoices); i++)
            {
                returnable.Add(0);
            }
            for (int ii = (listCount / numberOfChoices); ii < listCount; ii++)
            {
                returnable.Add(1);
            }
            //choicesList = truefalselist
        }
        choicesList = returnable;
    }
}
