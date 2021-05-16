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
        // loadSprites();
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

    void FixedUpdate()
    {
        playFrame += 1;
        if (nearInterface.activeSelf)
        {
            Dictionary<string, List<string>> info = mainScheduler.getCurrentStepInfo();
            string action = info["action"][0];
            List<Sprite> video = mainScheduler.getVideo(action);

            if (video != null)
            {
                if (visualCueDisplayContainer.activeSelf)
                {
                    spriteRenderer = GameObject.Find("VisualCueDisplayContainer/Animation").GetComponent<SpriteRenderer>();
                    int temp2 = playFrame % video.Count;
                    spriteRenderer.sprite = video[temp2];
                }
            }
            else
            {
                visualCueDisplayContainer.SetActive(false);
            }
        }


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
            string action = info["action"][0];

            if (currentStepNum != prevStepNum)
            {

                playFrame = 0;
                prevStepNum = currentStepNum;

                
                updateSpaitialVisualCue(info);
                // find location of the main equipment

            }
            updateNearInterface(info, currentStepNum);
            updateTimers(info);
        }
    }

    private void updateTimers(Dictionary<string, List<string>> info)
    {
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
        }
    }

    private void updateSpaitialVisualCue(Dictionary<string, List<string>> info)
    {
        List<string> equipment = info["equipment"];
        Vector3 timerLocation;
        if (equipment.Count > 0)
        {
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
            else
            {
                visualCueDisplayContainer.SetActive(false);
            }
        }

    }
    private void updateNearInterface(Dictionary<string, List<string>> info, int currentStepNum)
    {
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
        if (currentStepNum == 1) {
            instructionTextNearMenu.text += "\n" + mainScheduler.getEpicKitchenStatus();
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
    }

}
