using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ViveHandTracking.Sample {

    //Question: How do the points know that it can interact with the finger tip?
class Draw : MonoBehaviour {
  public Color NormalColor = Color.green;
  public Color HighlightColor = Color.red;
  public ObjectFactory Factory = null;
  public GameObject PointPrefab = null;

  private LineRenderer Line = null;
  private List<Collider> Points;
  private int CurrentIndex = 0;

/// <summary>
/// Creates points dynamically and set GO to be false.
/// </summary>
  void Awake() {
    Points = new List<Collider>();
    for (int i = 0; i < 4; i++)
      AddPoint(i);
    Line = GetComponent<LineRenderer>();
    Line.enabled = false;
    gameObject.SetActive(false);
  }

  void Update() {
    if (CurrentIndex == 0)
      return;
    Vector3 indexTip = Vector3.zero;
    if (GestureProvider.RightHand != null)      
      indexTip = GestureProvider.RightHand.points[GestureProvider.Mode == GestureMode.Skeleton ? 8 : 0]; //gets pos of tip of index finger
    if (indexTip.IsValidGesturePoint()) {               //Sticks line to tip of finger
      Line.positionCount = CurrentIndex + 1;
      Line.SetPosition(CurrentIndex, indexTip);
    } else
      Line.positionCount = CurrentIndex;
  }

/// <summary>
/// Sets board to be infront of camera on enable.
/// </summary>
  void OnEnable() { 
    var Camera = GestureProvider.Current.transform;
    transform.position = Camera.position;
    transform.rotation = Quaternion.Euler(0, Camera.rotation.eulerAngles.y, 0);
    SetIndex(0);
    for (int i = 1; i < 4; i++)
      Points[i].GetComponent<Light>().color = NormalColor;
  }

  void OnDisable() {
    Line.enabled = false;
  }

/// <summary>
/// This is the callback when Point collides with hands. Interesting, they set the callback function in this script. Shall follow them
/// </summary>
/// <param name="index"></param>
  void OnTrigger(int index) {
    if (index != CurrentIndex % 4) // If current index != 0 or not the first index
      return;
    if (CurrentIndex == 4) { // When last Point is triggered, creates cube and set this gameobject as false
      Factory.AddObject(); 
      gameObject.SetActive(false);
      return;
    }
    SetIndex(CurrentIndex + 1); //For other points that are not point 1/4, do this function
  }

  public void OnStateChanged(int state) {
    gameObject.SetActive(state == 1);
  }

/// <summary>
/// Called at Awake()
/// Creates Points and add itself as child to this gameObject. 
/// Creates the points' position, rotation, layer, callback function dynamically. Also adjust light and sphere collider dynamically.
/// Stores points as collider in List<collider> Points
/// </summary>
/// <param name="index"></param>
  void AddPoint(int index) {
    const float size = 0.075f;
    float x = (index < 2 ? -size : size) + 0.02f;
    float y = (index % 3 == 0 ? size : -size) + 0.05f;

    var go = GameObject.Instantiate(PointPrefab, transform, false);
    go.name = "Point " + index;
    go.transform.localPosition = new Vector3(x, y, 0.5f);
    go.transform.localRotation = Quaternion.identity;
    go.layer = gameObject.layer;

    var light = go.GetComponent<Light>();
    light.color = NormalColor;
    light.range = 0.03f;
    var collider = go.GetComponent<SphereCollider>();
    collider.radius = light.range;

    var trigger = go.GetComponent<TriggerCallback>();
    trigger.Value = index;
    trigger.Callback = OnTrigger;

    Points.Add(collider);
  }

/// <summary>
/// Called when each point is triggered by finger collision
/// Updates line's position, highlight current point and de-highlight previous point 
/// </summary>
/// <param name="index"></param>
  void SetIndex(int index) {
    CurrentIndex = index;
    Line.enabled = index > 0;
    Points[CurrentIndex % 4].GetComponent<Light>().color = HighlightColor; //Update current point's color
    if (index > 0) {
      Line.positionCount = index;
      Line.SetPosition(index - 1, Points[index - 1].transform.position); //Update Line
      Points[CurrentIndex - 1].GetComponent<Light>().color = NormalColor; //Update previous point's color
    }
  }
}

}
