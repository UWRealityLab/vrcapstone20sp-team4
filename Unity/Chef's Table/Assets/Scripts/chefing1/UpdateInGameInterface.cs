using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using System;
using Random = System.Random;

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
    private List<GameObject> lockIcons;
    private Material lockMat;
    private Material unlockMat;

    private ApplicationState appState;
    public GameObject visualCueDisplayContainer;
    // private Dictionary<string, VideoClip> actionsCues;
    private Dictionary<string, Sprite[]> actionsCues;
    private AIManager aiManager;
    private SpriteRenderer spriteRenderer;
    private Sprite[] cuttingSpriteArray;
    private Sprite[] crackingSpriteArray;
    private Sprite[] mixingSpriteArray;
    private Sprite[] microwavingSpriteArray;
    private Random r;

    private int framesPerFrame = 10;
    private int playFrame = 0;

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
        r = new Random();

        actionsCues = new Dictionary<string, Sprite[]>();
        loadSprites();
    }

    // public void updateLock(bool islocked)
    // {
    //     Material mat = islocked ? lockMat : unlockMat;
    //     for (int i = 0; i < lockIcons.Count; i++)
    //     {
    //         lockIcons[i].GetComponent<Renderer>().material = mat;
    //     }
    // }

    private void loadSprites()
    {
        cuttingSpriteArray = Resources.LoadAll<Sprite>("cutting");
        crackingSpriteArray = Resources.LoadAll<Sprite>("cracking");
        mixingSpriteArray = Resources.LoadAll<Sprite>("mixing");
        microwavingSpriteArray = Resources.LoadAll<Sprite>("microwaving");

        actionsCues["cutting"] = cuttingSpriteArray;
        actionsCues["cracking"] = crackingSpriteArray;
        actionsCues["mixing"] = mixingSpriteArray;
        actionsCues["microwaving"] = microwavingSpriteArray;
    }

    // Update is called once per frame
    void Update()
    {
        playFrame += 1;
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
            string action = info["action"][0];

            if (currentStepNum != prevStepNum)
            {
                visualCueDisplayContainer.SetActive(false);
                
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
                    Debug.Log("counts not zero");
                    string utensil = equipment[0];
                    timerLocation = appState.GetLocation(utensil);
                    if (timerLocation != Vector3.zero)
                    {
                        visualCueDisplayContainer.SetActive(true);
                        aiManager.addNewAI(timerLocation);
                        timerLocation = new Vector3(timerLocation.x, timerLocation.y + 0.2f, timerLocation.z);
                        visualCueDisplayContainer.transform.position = timerLocation;

                        Debug.Log("get location for " + utensil);
                    }
                }

                // display gif
                /*
                Texture2D[] frames;
                float framesPerSecond = 10.0f;

                int index;
                float time = Time.time * framesPerSecond;
                index = index % frames.Length;
                renderer.material.mainTexture = frames[index];
                */

                // display gif


                // display video
                /*
                if (visualCueDisplayContainer.activeSelf)
                {
                    string action = info["action"][0];
                    if (actionsCues.ContainsKey(action))
                    {
                        visualCueDisplayContainer.SetActive(true);
                        Debug.Log("display animation");
                        
                        VideoClip video = actionsCues[action];
                        visualCueDisplayContainer.transform.Find("VisualCueDisplay").gameObject.GetComponent<VideoPlayer>().clip = video;
                        visualCueDisplayContainer.transform.Find("VisualCueDisplay").gameObject.GetComponent<VideoPlayer>().Play();
                        Debug.Log("play video"); 
                        Debug.Log("play video"); 
                    }
                    else
                    {
                        visualCueDisplayContainer.transform.position = new Vector3(0f, -0.15f, 0f);
                        visualCueDisplayContainer.SetActive(false);
                    }
                }
                */
            }

            // gif animation
            // int framePerSecond = 6;
            if (actionsCues.ContainsKey(action))
            {
                if (visualCueDisplayContainer.activeSelf)
                {
                    if (playFrame % framesPerFrame == 0)
                    {
                        spriteRenderer = GameObject.Find("VisualCueDisplayContainer/Animation").GetComponent<SpriteRenderer>();
                        int temp1 = playFrame / framesPerFrame;
                        int temp2 = temp1 % actionsCues[action].Length;
                        spriteRenderer.sprite = actionsCues[action][temp2];
                    }

                    // int index = (int)(framePerSecond * Time.time);
                    // int index = (int)((Time.deltaTime * 100) % cuttingSpriteArray.Length);
                    // Debug.Log(index);
                    // index = index % cuttingSpriteArray.Length;

                    // Sprite[] sprites = actionsCues[action];
                    // int index = r.Next(0, sprites.Length);

                }
            } else {
                visualCueDisplayContainer.SetActive(false);
            }


            // exit/complete button
            if (mainScheduler.isTutorialDone())
            {
                exitIcon.GetComponent<Renderer>().material = completeMat;
            }
            else
            {
                exitIcon.GetComponent<Renderer>().material = exitMat;
            }

            // set time
            if (info["timer"][0] == "")
            {
                interfaceTimer.SetActive(false);
                if (spatialTimer.activeSelf)
                {
                    spatialTimer.SetActive(false);
                }
            }
            else
            {
                interfaceTimer.SetActive(true);
                clockNearMenu.text = info["timer"][0];
                if (!spatialTimer.activeSelf)
                {
                    spatialTimer.SetActive(true);
                }
                spatialTimer.transform.Find("Time").gameObject.GetComponent<TextMeshPro>().text = info["timer"][0];
                /*
                if (visualCueDisplayContainer.activeSelf) {
                    if (!spatialTimer.activeSelf) {
                        spatialTimer.SetActive(true);
                    }
                    spatialTimer.transform.Find("Time").gameObject.GetComponent<TextMeshPro>().text = info["timer"][0];
                }
                */
            }

        }
    }

}
