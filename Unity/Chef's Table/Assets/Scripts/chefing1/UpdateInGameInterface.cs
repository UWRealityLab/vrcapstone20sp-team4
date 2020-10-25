using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using System;

public class UpdateInGameInterface : MonoBehaviour
{

    public GameObject mainCam;

    public GameObject spatialTimer;
    private MainScheduler2 mainScheduler;
    private InterfaceManager interfaceManager;

    private TextMeshPro instructionTextNearMenu;
    private TextMeshPro detailTextNearMenu;
    private TextMeshPro stepNumberNearMenu;
    private GameObject nearInterface;
    private TextMeshPro clockNearMenu;

    private GameObject exitIcon;
    private GameObject interfaceTimer;

    private Material exitMat;
    private Material completeMat;
    private int prevStepNum;
    // private List<GameObject> lockIcons;
    // private Material lockMat;
    // private Material unlockMat;
    GameObject icon;

    private ApplicationState appState;
    public GameObject visualCueDisplayContainer;
    private Dictionary<string, VideoClip> actionsCues;
    private AIManager aiManager;

    private bool locationSetForCurrentStep = false;

    public GameObject NEXT;
    public GameObject BACK;


    // Start is called before the first frsame update
    void Awake()
    {
        mainScheduler = GameObject.Find("Scheduler").GetComponent<MainScheduler2>();
        interfaceManager = GameObject.Find("InterfaceManager").GetComponent<InterfaceManager>();
        nearInterface = GameObject.Find("NearInterface");
        instructionTextNearMenu = GameObject.Find("NearInterface/InstructionCanvas/Instruction").GetComponent<TextMeshPro>();
        detailTextNearMenu = GameObject.Find("NearInterface/InstructionCanvas/Detail").GetComponent<TextMeshPro>();
        stepNumberNearMenu = GameObject.Find("NearInterface/InstructionCanvas/StepNumber").GetComponent<TextMeshPro>();
        clockNearMenu = GameObject.Find("NearInterface/InterfaceTimer/ClockText").GetComponent<TextMeshPro>();
        interfaceTimer = GameObject.Find("NearInterface/InterfaceTimer").gameObject;
        aiManager = GameObject.Find("AIManager").GetComponent<AIManager>();

        exitIcon = GameObject.Find("NearInterface/ExitOrComplete/IconAndText/Icon");

        exitMat = Resources.Load("Mat/ExitButton", typeof(Material)) as Material;
        completeMat = Resources.Load("Mat/CompleteButton", typeof(Material)) as Material;
        prevStepNum = 0;
        // lockIcons = new List<GameObject>();
        // GameObject lockIcon;
        // lockIcon = GameObject.Find("HeadLockCanvas/SimulationInterface/Lock/IconAndText/Icon");
        // lockIcons.Add(lockIcon);
        // lockIcon = GameObject.Find("HeadLockCanvas/NearInterface/Lock/IconAndText/Icon");
        // lockIcons.Add(lockIcon);
        // lockIcon = GameObject.Find("HeadLockCanvas/Onboarding/OnboardingInterface/Lock/IconAndText/Icon");
        // lockIcons.Add(lockIcon);
        

        appState = GameObject.Find("ApplicationState").GetComponent<ApplicationState>();

        actionsCues = new Dictionary<string, VideoClip>();
        uploadVideos();
    }

    // public void updateLock(bool islocked)
    // {
    //     Material mat = islocked ? lockMat : unlockMat;
    //     for (int i = 0; i < lockIcons.Count; i++)
    //     {
    //         lockIcons[i].GetComponent<Renderer>().material = mat;
    //     }
    // }

    private void uploadVideos()
    {
        actionsCues["cutting"] = Resources.Load<VideoClip>("actions/cutting");
        actionsCues["cracking"] = Resources.Load<VideoClip>("actions/egg_cracking");
        actionsCues["mixing"] = Resources.Load<VideoClip>("actions/mixing");
        actionsCues["microwaving"] = Resources.Load<VideoClip>("actions/microwaving");
        /*
        actionsCues["heat"] = Resources.Load<VideoClip>("actions/heating");
        actionsCues["slice"] = Resources.Load<VideoClip>("actions/slicing");
        actionsCues["spread"] = Resources.Load<VideoClip>("actions/spread");
        actionsCues["sprinkle"] = Resources.Load<VideoClip>("actions/sprinkle");
        */
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameObject.Find("Scheduler").activeSelf || !mainScheduler.tutorialStarts)
        {
            return;
        }
       
        Dictionary<string, List<string>> info = mainScheduler.getCurrentStepInfo();
        if (info == null)
        {
            return;
        }
        int currentStepNum = Int32.Parse(info["StepNum"][0]);
        if (nearInterface.activeSelf)
        {
            
            if (currentStepNum != prevStepNum)
            {
   
                visualCueDisplayContainer.SetActive(true);
                prevStepNum = currentStepNum;
                // display step number
                string stepNumberText = "Step #" + currentStepNum;
                stepNumberNearMenu.text = stepNumberText;

                // display instruction text
                string instructionText = info["description"][0];

                string detailText = "Ingredients:\n";
                List<string> ingredients = info["ingredients"];
                List<string> measurement = info["measurement"];
           
                if (ingredients.Count == 0)
                {
                    detailText += "-\n";
                }
                else
                {
                    for (int i = 0; i < ingredients.Count; i++)
                    {
                        detailText += ingredients[i];
                        if (measurement[i].Length != 0)
                        {
                            detailText += " - " + measurement[i];
                        }
                        detailText += "\n";
                    }
                }
                detailText += "\n";
      
                detailText += "Equipment:\n";
                List<string> equipment = info["equipment"];
                if (equipment.Count == 0)
                {
                    detailText += "-\n";
                }
                else
                {
                    for (int i = 0; i < equipment.Count; i++)
                    {
                        detailText += equipment[i] + "\n";
                    }
                }
          
                instructionTextNearMenu.text = instructionText;
                detailTextNearMenu.text = detailText;

                // find location of the main equipment
                Vector3 timerLocation;
                if (equipment.Count > 0)
                {
                    string utensil = equipment[0];
                    timerLocation = appState.GetLocation(utensil);
                    if (timerLocation != Vector3.zero)
                    {
                        //ParticleSystem.transform.position = timerLocation;
                        //ParticleSystem.SetActive(true);
                        aiManager.addNewAI(timerLocation);
                        timerLocation = new Vector3(timerLocation.x, timerLocation.y + 0.2f, timerLocation.z);
                        visualCueDisplayContainer.transform.position = timerLocation;
                        //visualCueDisplayContainer.transform.LookAt(mainCam.transform.position);
                    } else {
                        
                    }
                }
        
                
                // display video
                if (visualCueDisplayContainer.activeSelf)
                {
                    string action = info["action"][0];
                    if (actionsCues.ContainsKey(action))
                    {
                        VideoClip video = actionsCues[action];
                        visualCueDisplayContainer.transform.Find("VisualCueDisplay").gameObject.GetComponent<VideoPlayer>().clip = video;
                        visualCueDisplayContainer.transform.Find("VisualCueDisplay").gameObject.GetComponent<VideoPlayer>().Play();
                    }
                    else
                    {
                        visualCueDisplayContainer.transform.position = new Vector3(0f, -0.15f, 0f);
                        visualCueDisplayContainer.SetActive(false);
                        
                    }
                }
            }
   
            // visualize buttons
            if (mainScheduler.isTutorialDone())
            {
                exitIcon.GetComponent<Renderer>().material = completeMat;
            }
            else
            {
                exitIcon.GetComponent<Renderer>().material = exitMat;
            }
            if (currentStepNum == 1) {
                BACK.SetActive(false);
            } else if (mainScheduler.isTutorialDone()) {
                NEXT.SetActive(false);
            } else {
                BACK.SetActive(true);
                NEXT.SetActive(true);
                BACK.transform.Find("Back").gameObject.GetComponent<BoxCollider>().enabled = true;
                NEXT.transform.Find("Next").gameObject.GetComponent<BoxCollider>().enabled = true;
            }

            // set time
            if (info["timer"][0] == "")
            {
                interfaceTimer.SetActive(false);
                if (spatialTimer.activeSelf) {
                    spatialTimer.SetActive(false);
                }
            }
            else
            {
                interfaceTimer.SetActive(true);
                clockNearMenu.text = info["timer"][0];
                if (visualCueDisplayContainer.activeSelf) {
                    if (!spatialTimer.activeSelf) {
                        spatialTimer.SetActive(true);
                    }
                    spatialTimer.transform.Find("Time").gameObject.GetComponent<TextMeshPro>().text = info["timer"][0];
                }
                
            } 
       
        }

    }
}
