# Multimodal Interaction with Gaze and Controller Gestures (Unity Project Assets)
This repository contains the Unity project and its relevant assets to help facilitate the data collection phase of this research project that was published in ISMAR 2022 [Multimodal lnteraction with Gaze and Controller Gesture](https://www.researchgate.net/publication/366345429_Multimodal_lnteraction_with_Gaze_and_Controller_Gesture). Happy to know if this project helped anybody on any future development of this / other research projects :)

# Project Description
**Features included:**
  - A Unity scene that comprises of the target, the dragging path and event listeners for the different metrics indicated in the publication's test scenarios.
  - A combination of input interactions (eye gaze, controller, hand gesture input)
  - Targets and dragging path that are interactable with the above-mentioned interactions. 

![DraggingTaskElementsShowcase (3)](https://user-images.githubusercontent.com/60340457/218298590-ec280d68-e39c-4a1a-8b30-b1a66070d86d.gif)

  - Easily control user testing and data collection settings

![TestSceneSS1](https://user-images.githubusercontent.com/60340457/218297924-bc0deb31-b9b2-4b03-bccf-d9cbe45cf13a.jpg)

  - Collect these data in a consolidated excel sheet available in "Streaming Asset" folder

![image](https://user-images.githubusercontent.com/60340457/218296428-4818340e-a22c-43d9-9cf7-e40ee468819a.png)

**To be improved:**
  - Eye jitteriness: During the research, we realized that eye jitteriness is an issue faced by the participant and resulted in high target-dragging error rate. One of the proposed solution is to build a deadzone around the target to reduce the movement of the target induced by eye jitteriness. This proposal could be explored further in the future.
  
# Dependencies
  - HTC's Vive Pro Eye (https://business.vive.com/sea/product/vive-pro-eye/)
  - Unity 2019.4.14f
    - Refer here for more info: https://unity.com/releases/editor/whats-new/2019.4.14
  - Tobii XR SDK 
    - HTC Vive Pro Eye headset was used in this project. Refer here for more info on the setup: https://developer.tobii.com/xr/develop/unity/getting-started/vive-pro-eye/
    - This project uses Tobii eye-gaze API for most of the eye-gaze-based interaction. Therefore, the scripts should be compatible with other headsets that are compatible with Tobii SDK (Refer here for other compatible headsets: https://developer.tobii.com/xr/develop/unity/)

# How to use the project 

**To conduct user testing**
  - Navigate to the main test scene: Assets/Scenes/Main Testbed Final.unity
  - Change the experiment's conditions using the following fields:
  - Under TaskControllers game object
    - Experiment Controller Script (To change the metadata)
      - Task type to either Primary task only or Primary and Rotation task
      - Input mode: ControllerClick / GazeDwell / GazeClick
      - Participant ID
      - Toggle between enable (for actual data collection) or disable (for dry runs) data collection
    - Primary Task Controller Script (To change the test condition)
      - Toggle between randomized test condition or non-randomized (which will utilize the settings below)
      - Change object size 
      - Change target distance
      - Change path direction

![image](https://user-images.githubusercontent.com/60340457/218299329-56b7d2bc-8572-4193-a534-2dce2aa78a68.png)

  - Run the project
