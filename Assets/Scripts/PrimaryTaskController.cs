using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the timeline of Primary Task: Start, Interactions and End of task.
/// </summary>
public class PrimaryTaskController : MonoSingleton<PrimaryTaskController>
{
    #region Fields

    //Testbed Setup Support
    private GameObject mainCamera;
    private float initialTargetDepth = 5f;
    private float initialCameraHeight;
    private Vector3 ReferencePoint;

    //Task Status Support
    private bool isTaskInProgress;

    //TestCondition fields 
    [Header("True: Use Random values. False: Use Inspector values")]
    [SerializeField] bool isTestConditionRandomized = true;
    [SerializeField] private TargetSize targetObjectSize;
    [SerializeField] private TargetDestinationAngDist targetDestinationAngularDistance;
    [SerializeField] private PathCurvature pathDirection;

    //Support for Target setup support
    [SerializeField] List<GameObject> targetPrefab;
    private GameObject targetObject;

    //Support for TargetSelection
    private bool isDestinationAndPathActive;

    //Support for Destination
    [SerializeField] private GameObject destinationPrefab;
    private GameObject destinationObject;
    RandomValueGenerator randomValueGenerator = new RandomValueGenerator(2, 18);
    float leftOrRightDirection;

    /*
    //Support for Straight Path Rendering
    GameObject straightPathPrefab;
    private GameObject straightPathObject;
    LineRenderer straightPathLineRenderer;

    //Support for Circular Path Rendering
    [SerializeField] private List<GameObject> circularPathPrefabs = new List<GameObject>();
    private GameObject circularPathObject;
    */

    //Support for Curved Path Rendering 
    [SerializeField] private List<GameObject> curvedPathPrefabs = new List<GameObject>();
    private GameObject curvedPathObject;

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

    public TargetSize TargetObjectSize
    {
        get { return targetObjectSize; }
    }

    public TargetDestinationAngDist TargetDestinationAngularDistance
    {
        get { return targetDestinationAngularDistance; }
    }

    #endregion

    public override void Initialize()
    {
        base.Initialize();
        SphericalTarget.OnTargetSelection += OnTargetSelected;
        Destination.OnDestinationSelected += EndPrimaryTask;
    }

    private void OnDisable()
    {
        SphericalTarget.OnTargetSelection -= OnTargetSelected;
        Destination.OnDestinationSelected -= EndPrimaryTask;
    }

    void Start()
    {
        mainCamera = Camera.main.gameObject;
        if (mainCamera != null)
        {
            initialCameraHeight = mainCamera.transform.position.y;
            initialCameraHeight = (initialCameraHeight > 1.6 || initialCameraHeight < 1) ? 1.15f : initialCameraHeight;
            ReferencePoint = new Vector3(0, initialCameraHeight, 0);
        }
    }

    #region Interactions

    /// <summary>
    /// Starts primary task of selection and dragging. Returns the target of interest.
    /// </summary>
    /// <returns></returns>
    public GameObject StartPrimaryTask()
    {
        if(isTestConditionRandomized)
        {
            RandomizeTestConditions();
        }

        IsTaskInProgress = true;
        GameObject targetGO = SetTargetPosAndSize();

        //Invoke OnTargetSelected() for testing purposes; to inspect the curved path and destination
        //OnTargetSelected();
        return targetGO;
    }

    private void OnTargetSelected()
    {
        if (!isDestinationAndPathActive && isTaskInProgress)
        {
            SetDestinationPos();
            SetCurvedPathLine();
            isDestinationAndPathActive = true;

            //Start timer for PrimaryTask 
            DataCollectionManager.Instance.StartTaskTimer("PriTask");
        }
    }

    private void EndPrimaryTask()
    {
        //Destroy Previous Trial Objects
        if (curvedPathObject != null)
            Destroy(curvedPathObject);
        if (destinationObject != null)
            Destroy(destinationObject, 0.5f);
        isDestinationAndPathActive = false;

        IsTaskInProgress = false;

        //Set timer for Data Collection here;
        DataCollectionManager.Instance.EndTaskTimer("PriTask");
    }

    #endregion

    #region Operations

    /// <summary>
    /// Initialize Target and Curve Path Settings with TestConditionManager
    /// </summary>
    private void RandomizeTestConditions()
    {
        TestConditionBlock testConditionBlock = TestConditionManager.Instance.GetCurrentTestConditions();
        targetDestinationAngularDistance = testConditionBlock.TargetDestinationAngularDistance;
        targetObjectSize = testConditionBlock.TargetSize;
        pathDirection = testConditionBlock.PathDirection;
    }

    /// <summary>
    /// Initial Target position.x wrt origin, position.y wrt to camera height when game is run, position.z wrt to parameter: initialTargetDepth
    /// </summary>
    /// <param name="numb"></param>
    private GameObject SetTargetPosAndSize()
    {
        //Instantiate target instance at desired position 
        Vector3 targetPosition = new Vector3(0, ReferencePoint.y, initialTargetDepth);

        //Instantiate target prefab based on targetsize chosen 
        switch(targetObjectSize)
        {
            case TargetSize.big:
                targetObject = GameObject.Instantiate(targetPrefab[0], targetPosition, Quaternion.identity);
                break;
            case TargetSize.medium:
                targetObject = GameObject.Instantiate(targetPrefab[1], targetPosition, Quaternion.identity);
                break;
            case TargetSize.small:
                targetObject = GameObject.Instantiate(targetPrefab[2], targetPosition, Quaternion.identity);
                break;
        }
        return targetObject;
    }

    /// <summary>
    /// Create a destination instance on either the left or right. 
    /// Angular Distance from target is pre-defined at Unity IDE. 
    /// Rotation pivot's x and z is set at origin and height is set by initial camera height
    /// </summary>
    private void SetDestinationPos()
    {
        int randomValue = randomValueGenerator.GetRandomValue();
        leftOrRightDirection = randomValue == 0 ? -1f : 1f; //-1 = left, 1 = right
        //leftOrRightDirection = -1f;

        //Instantiate destination instance at desired position; rotated by angular distance
        Vector3 destinationAngleFromCenter = Quaternion.Euler(0, leftOrRightDirection * (float)TargetDestinationAngularDistance, 0) * Vector3.forward;
        Vector3 destinationPosition = ReferencePoint + Vector3.Normalize(destinationAngleFromCenter) * initialTargetDepth;
        destinationObject = GameObject.Instantiate(destinationPrefab, destinationPosition, Quaternion.identity);
    }

    /*
/// <summary>
/// Create a path line instance from target to destination.
/// </summary>
private void SetStraightPathLine()
{
    if (straightPathPrefab != null)
    {
        straightPathObject = GameObject.Instantiate(straightPathPrefab);
        straightPathLineRenderer = straightPathObject.GetComponent<LineRenderer>();
    }
    Vector3 targetPos = targetObject.transform.position;
    Vector3 destinationPos = destinationObject.transform.position;  
    float targetToDestinationDistance = Vector3.Distance(destinationPos,targetPos);
    Vector3 dirVectorTargetToDestination = Vector3.Normalize(destinationPos - targetPos);

    if (straightPathLineRenderer != null)
    {
        //Set pathline to be in between target and destination
        straightPathObject.transform.position = targetPos + dirVectorTargetToDestination * targetToDestinationDistance/2;
        straightPathObject.transform.LookAt(ReferencePoint);
        straightPathLineRenderer.SetPosition(0, targetObject.transform.position);
        straightPathLineRenderer.SetPosition(1, destinationObject.transform.position);
        straightPathObject.GetComponent<BoxCollider>().size = new Vector3(targetToDestinationDistance, 0.05f, 0.05f);
    }
}

/// <summary>
/// Creates a circular path which connects target to destination. The circular path to be created will be either 10/20/30deg in angular distance 
/// and will be based on initial target destination distance set by user.
/// </summary>
private void SetCircularPathLine()
{
    //Need to decide which circular path to instantiate.
    int circularPathIndex = 0;
    switch (targetDestinationAngularDistance)
    {
        case targetDestinationAngDist.close:
            circularPathIndex = 0;
            break;
        case targetDestinationAngDist.mid:
            circularPathIndex = 1;
            break;
        case targetDestinationAngDist.far:
            circularPathIndex = 2;
            break;             
    }
    if (circularPathPrefabs.Count != 0)
        circularPathObject = GameObject.Instantiate(circularPathPrefabs[circularPathIndex]);

    if(circularPathObject != null)
    {
        if (leftOrRightDir == -1f)// destination instantiated on the left
        {
            circularPathObject.transform.position = destinationObject.transform.position;
        }
        else // destination instantiated on the right
        {
            circularPathObject.transform.position = targetObject.transform.position;
        }
    }
    circularPathObject.transform.LookAt(ReferencePoint);
    circularPathObject.transform.Rotate(0, 90f, 0, Space.Self);
}
*/

    /// <summary>
    /// Creates a curved path that connects from the target to the destination
    /// </summary>
    private void SetCurvedPathLine()
    {
        //Need to decide which circular path to instantiate.
        int curvedPathIndex = 0;
        float rotationXFineAdjustment = 0;
        float pathRotationX = 0;
        float pathRotationY = 0;

        switch (TargetDestinationAngularDistance)
        {
            case TargetDestinationAngDist.close:
                curvedPathIndex = 0;
                rotationXFineAdjustment = -1.44f;
                break;
            case TargetDestinationAngDist.mid:
                curvedPathIndex = 1;
                rotationXFineAdjustment = 3.98f;
                break;
            case TargetDestinationAngDist.far:
                curvedPathIndex = 2;
                rotationXFineAdjustment = 11.23f;
                break;
        }
        if (curvedPathPrefabs.Count != 0)
            curvedPathObject = GameObject.Instantiate(curvedPathPrefabs[curvedPathIndex]);

        //Find position to place center of curved path
        Vector3 targetPos = targetObject.transform.position;
        Vector3 destinationPos = destinationObject.transform.position;
        float targetToDestinationDistance = Vector3.Distance(destinationPos, targetPos);
        Vector3 dirVectorTargetToDestination = Vector3.Normalize(destinationPos - targetPos);

        //Change path facing direction based on left or right direction 
        if(leftOrRightDirection == -1f) // path direction = left
        {
            pathRotationY = 0f;
            //Change path tilt angle based on path direction (up or down)
            switch (pathDirection)
            {
                case PathCurvature.up: //If upwards = rotate negatively. 
                    pathRotationX = -rotationXFineAdjustment - 90f;
                    break;
                case PathCurvature.down:// If downwards = rotate positively
                    pathRotationX = 90f + rotationXFineAdjustment;
                    break;
            }
        }
        else //path direction = right
        {
            pathRotationY = 180f;
            //Change path tilt angle based on path direction (up or down)
            switch (pathDirection)
            {
                case PathCurvature.up: //If upwards = rotate negatively. 
                    pathRotationX = rotationXFineAdjustment - 90f;
                    break;
                case PathCurvature.down:// If downwards = rotate positively
                    pathRotationX = 90f - rotationXFineAdjustment;
                    break;
            }
        }

        //Set curved path object position and rotation
        if (curvedPathObject != null)
        {
            curvedPathObject.transform.position = targetPos + dirVectorTargetToDestination * targetToDestinationDistance / 2;
            curvedPathObject.transform.LookAt(ReferencePoint);
            curvedPathObject.transform.Rotate(pathRotationX, pathRotationY, 0, Space.Self);
            DataCollectionManager.Instance.UpdatePathDirection(pathDirection);
        }
    }

    #endregion
}
