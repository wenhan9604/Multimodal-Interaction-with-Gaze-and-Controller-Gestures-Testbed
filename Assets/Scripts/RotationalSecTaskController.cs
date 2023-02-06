using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/// <summary>
/// Task Controller for Rotational Secondary Task. Controls the lifecycle of rotation secondary task: start,end and interactions in task.
/// Contains number of rotation cues to create
/// Contains the logic for matching rotational input from gesture and controller script with current rotation cue.
/// </summary>
public class RotationalSecTaskController : MonoSingleton<RotationalSecTaskController>
{
    #region Fields 
    [SerializeField] private GameObject gestureInputObject;
    [SerializeField] private ControllerSecTaskInput controllerSecTaskInput;
    private GameObject targetObject;

    //Support for Rotation Cues 
    [SerializeField] private GameObject rotationCuePrefab;
    private List<GameObject> rotationCuesGameObjects = new List<GameObject>();
    private RandomValueGenerator randomValueGenerator = new RandomValueGenerator(2, 18);
    private int inputCounter;

    //Support for Task Status
    private bool isTaskInProgress;

    #endregion

    #region Properties 
    
    public bool IsTaskInProgress
    {
        get { return isTaskInProgress; }
        private set
        {
            isTaskInProgress = value;
        }
    }

    #endregion

    public override void Initialize()
    {
        base.Initialize();
        GestureSecTaskInput.OnGestureSecTaskInput += UpdateRotationCue;
        ControllerSecTaskInput.OnControllerSecTaskInput += UpdateRotationCue;
    }

    private void OnDisable()
    {
        GestureSecTaskInput.OnGestureSecTaskInput -= UpdateRotationCue;
        ControllerSecTaskInput.OnControllerSecTaskInput -= UpdateRotationCue;
    }

    // Start is called before the first frame update
    void Start()
    {
        gestureInputObject.SetActive(false);
        controllerSecTaskInput.enabled = false;
    }

    #region Interactions

    /// <summary>
    /// Enables true for gesture rotation behaviour, controller rotation input behaviour and target rotation behaviour. 
    /// Initializes rotation cue game objects in random. Will only initialize maximum 2 cues.
    /// </summary>
    public void StartRotationSecTask(GameObject target)
    {
        //Initialize settings
        this.targetObject = target;
        rotationCuesGameObjects = InitializeRotationCues(RandomRotationCueGenerator(ExperimentController.Instance.SecTaskInputDirectionFlag));

        //Allow secondary task input 
        gestureInputObject.SetActive(true);
        controllerSecTaskInput.enabled = true;
        inputCounter = 2;

        //Allow Target Rotation  
        if(targetObject.transform.Find("Cube").TryGetComponent<TargetRotation>(out TargetRotation targetRotationScript))
        {
            targetRotationScript.enabled = true;
        }

        IsTaskInProgress = true;

        //Send event to start collection for secondary task timing. 
        DataCollectionManager.Instance.StartTaskTimer("RotTask");
    }

    /// <summary>
    /// Try to match rotation input with the current rotation cues. 
    /// If match, will reduce the magnitude of rotation cue. If no match, will send error message and track error. 
    /// Once all rotation cue is successfully matched, will execute the end of this rotation secondary task
    /// </summary>
    /// <param name="inputDirection"></param> Rotation input direction from either gesture or controller input
    private void UpdateRotationCue(RotationDirection inputDirection)
    {
        if (IsTaskInProgress)
        {
            bool isInputDirectionMatchWithCue = false;
            GameObject cueGOToBeDestroyed = null;

            foreach (GameObject cueGO in rotationCuesGameObjects)
            {
                RotationCue cueGOScript = cueGO.GetComponent<RotationCue>();
                if (cueGOScript != null)
                {
                    if (cueGOScript.RotationDirection == inputDirection)
                    {
                        cueGOScript.RotationMagnitude--;
                        isInputDirectionMatchWithCue = true;

                        if (cueGOScript.RotationMagnitude == 0)
                        {
                            cueGOToBeDestroyed = cueGO;
                        }
                    }
                    if(isInputDirectionMatchWithCue == false) // No Rotation cue matched
                    {
                        Debug.Log("Input didnt match with any rotation cue! ");
                    }
                }
            }

            if (cueGOToBeDestroyed != null) // cannot change the list when using foreach loop. Thus put edit list logic outside of foreach loop
            {
                rotationCuesGameObjects.Remove(cueGOToBeDestroyed);
                Destroy(cueGOToBeDestroyed);
            }

            //Register each input's timing 
            if(inputCounter > 0)
            {
                if(inputCounter == 2) //first rot task input 
                {
                    DataCollectionManager.Instance.UpdateRotTaskInputTiming(1);
                }
                else if (inputCounter == 1) //second rot task input
                {
                    DataCollectionManager.Instance.UpdateRotTaskInputTiming(2);
                }
                inputCounter --;
            }

            if (!isInputDirectionMatchWithCue)
            {
                DataCollectionManager.Instance.UpdateWrongRotTaskInput(inputDirection);
            }

            if (rotationCuesGameObjects.Count == 0)
            {
                EndRotationSecTask();
            }
        }
    }

    /// <summary>
    /// Method is called when all cues are successfully matched or when secondary task timeout. 
    /// Set the gesture and controller rotation input behaviour to false. Target cannot rotate.
    /// </summary>
    private void EndRotationSecTask()
    {
        gestureInputObject.SetActive(false);
        controllerSecTaskInput.enabled = false;
        if (targetObject.transform.Find("Cube").TryGetComponent<TargetRotation>(out TargetRotation targetRotationScript))
        {
            targetRotationScript.enabled = false;
        }

        IsTaskInProgress = false;

        DataCollectionManager.Instance.EndTaskTimer("RotTask");
    }

    #endregion

    #region Operations 

    /// <summary>
    /// Randomize the selection of rotation direction from the input. 
    /// When one direction is chosen, its rotation direction pair will be removed from the possible direction to be chosen.
    /// Returns a dictionary - Key: rotation direction and Value: rotation magnitude 
    /// </summary>
    /// <param name="rotDirPairs">List Of rotationDirectionPairs</param>
    /// <returns></returns>
    private Dictionary<RotationDirection, int> RandomRotationCueGenerator(Dictionary<RotationDirection,bool> inputDirectionFlags)
    {
        List<RotationDirection> rotationDirectionChoices = new List<RotationDirection>();
        Dictionary<RotationDirection, int> rotationDirectionsChosen = new Dictionary<RotationDirection, int>();

        //Initialize rotationDirectionOptions from rotation direction pairs
        //rotationDirectionChoices.AddRange(RotationDirectionUtility.GetRotationDirectionsFromRotDirPairs(rotDirPairs));

        foreach(KeyValuePair<RotationDirection,bool> pair in inputDirectionFlags)
        {
            if (pair.Value)
                rotationDirectionChoices.Add(pair.Key);
        }

        //Randomize the number of cues. Currently, total cue count is set to one
         int cueCount = UnityEngine.Random.Range(1, rotationDirectionChoices.Count / 2 + 1);

        //Randomize the selection of direction from list of rotationDirectionOptions. Add to collection.
        for (int ii = 0; ii < cueCount; ii++)
        {
            RotationDirection chosenDirection = RotationDirection.up;
            if (rotationDirectionChoices.Count ==2 )
            {
                chosenDirection = rotationDirectionChoices[randomValueGenerator.GetRandomValue()];
            }
            else
            {
                chosenDirection = rotationDirectionChoices[UnityEngine.Random.Range(0, rotationDirectionChoices.Count)];
            }
            AddRotationDirectionToDictionary(rotationDirectionsChosen, chosenDirection);

            //Update List by removing the opposite direction from choice.
            rotationDirectionChoices = rotationDirectionChoices.Except(RotationDirectionUtility.GetListOfRotationDirectionPair(chosenDirection)).ToList();
        }

        return rotationDirectionsChosen;
        
        void AddRotationDirectionToDictionary(Dictionary<RotationDirection, int> dictionary, RotationDirection direction)
        {
            //Add chosen direction to the dictionary.
            if (!dictionary.ContainsKey(direction)) //initialize the value if it isnt chosen
            {
                dictionary.Add(direction, 1);
            }
            else
            {
                dictionary[direction]++;
            }
        }
    }

    /// <summary>
    /// Will instantiate the rotation cues based on info given from the dictionary of rotation cues. 
    /// Returns a list of rotation cues gameobject.
    /// </summary>
    /// <param name="rotationCueInfoDictionary"></param> Dictionary containing the information of rotation cues; rotation directions and rotation magnitude.
    /// <returns></returns>
    private List<GameObject> InitializeRotationCues(Dictionary<RotationDirection, int> rotationCueInfoDictionary)
    {
        List<GameObject> rotationCuesGO = new List<GameObject>();

        foreach (KeyValuePair<RotationDirection, int> cue in rotationCueInfoDictionary)
        {
            rotationCuesGO.Add(InstantiateRotationCue(cue));
        }
        return rotationCuesGO;

        /// <summary>
        /// Instantiate Rotation Cue based on information given by dictionary. 
        /// </summary>
        /// <param name="cue"></param> A KeyValuePair which contains information about the cue's rotation direction and magnitude.
        /// <returns></returns>
        GameObject InstantiateRotationCue(KeyValuePair<RotationDirection, int> cue)
        {
            GameObject rotationCue = Instantiate(rotationCuePrefab, targetObject.transform);
            RotationCue rotationCueScript = rotationCue.GetComponent<RotationCue>();
            if(rotationCueScript != null)
            {
                rotationCueScript.RotationDirection = cue.Key;
                rotationCueScript.RotationMagnitude = cue.Value;
            }
            return rotationCue;
        }
    }

    #endregion
}
