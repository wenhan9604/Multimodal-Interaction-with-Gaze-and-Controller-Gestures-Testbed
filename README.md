# Final Year Project - Multimodal Interaction with Gaze and Controller Gestures
This is my Final Year Project in collaboration between NTU and DSO, from Jan 2021 - Dec 2021. 

# Project Description
This project helps to facilitate the user testing conducted for the data collection in this publication (//Insert publication). 

Features included: 
  - Interaction component that consisted of eye gaze, controller, hand gesture input  
  - Interactable targets that interacted with the above-mentioned interactions. 
  - The dragging task test scene that contains visual elements that reflected the task's progress
  - Event callers and listeners that track the info of the task (time start and end of task, the errors produced, etc.) and stores these data in an excel sheet.
  - 

Plugins / SDKs / Hardware used:

  - HTC's Vive Pro Eye - A VR headset that contains IR eye trackers
  - Tobii Unity Plugin - Tobii plugin provided low-level info about the eye gaze (direction of vector, etc.). This helps to build the required interaction and allowed us to collect data about the participant.

Point for improvement: 
  - Eye jitteriness: During the research, we realized that eye jitteriness is an issue faced by the participant, and resulted in high target-dragging error rate. One of the proposed solution is to build a deadzone around the target to reduce the movement of the target induced by eye jitteriness.  
  
  
# How to run and install the project
  - Unity 2020
    - Follow this procedure 
  - Tobii XR SDK 
    - For this project, the HTC Vive Pro Eye headset was used. Refer here on how to setup the headset with Tobii XR SDK https://developer.tobii.com/xr/develop/unity/getting-started/vive-pro-eye/
    - However, Tobii provides SDK for other headsets too. This project uses Tobii eye-gaze API for most of the eye-gaze-based interaciton. Therefore, this project should be compatible with other headsets, provided Tobii XR SDK is used too.

# How to use the project 
  - Navigate to the main test scene: 
  - Change the test condition to suit your liking 
  - Indicate the info needed for data collection (Participant ID) 
  - Run the project 
