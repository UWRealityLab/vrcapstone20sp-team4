using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using System;

public class UpdateInGameInterface : MonoBehaviour
{
    private MainScheduler2 mainScheduler;
    private InterfaceManager interfaceManager;
    // private VisualCueManager visualCueManager;
    //private TextMeshProUGUI instructionTextFloatingInterf;
    private TextMeshPro instructionTextNearMenu;
    private TextMeshPro detailTextNearMenu;
    private TextMeshPro stepNumberNearMenu;
    private GameObject nearInterface;
    //private TextMeshProUGUI clockFloatingInterf;
    private TextMeshPro clockNearMenu;
    //private GameObject gameInterface;
    private GameObject exitIcon;
    private GameObject interfaceTimer;

    private Material exitMat;
    private Material completeMat;
    private int prevStepNum;
    private List<GameObject> lockIcons;
    private Material lockMat;
    private Material unlockMat;
    GameObject icon;
    // private GameObject ImagePlane;

    private ApplicationState appState;
    private VideoPlayer videoPlayer;
    private Dictionary<string, VideoClip> actionsCues;
    // private float criticalEquipmentUpdateTimer = 0;

    public GameObject ParticleSystem;
    //private GameObject closeVideoButton;
    private GameObject interfaceScreen;
    //private bool closeVideo = false;
    private bool locationSetForCurrentStep = false;


    // Start is called before the first frsame update
    void Awake()
    {
        mainScheduler = GameObject.Find("Scheduler").GetComponent<MainScheduler2>();
        interfaceManager = GameObject.Find("InterfaceManager").GetComponent<InterfaceManager>();
        // visualCueManager = GameObject.Find("VisualCueManager").GetComponent<VisualCueManager>();
        nearInterface = GameObject.Find("NearInterface");
        instructionTextNearMenu = GameObject.Find("NearInterface/InstructionCanvas/Instruction").GetComponent<TextMeshPro>();
        detailTextNearMenu = GameObject.Find("NearInterface/InstructionCanvas/Detail").GetComponent<TextMeshPro>();
        stepNumberNearMenu = GameObject.Find("NearInterface/InstructionCanvas/StepNumber").GetComponent<TextMeshPro>();
        clockNearMenu = GameObject.Find("NearInterface/InterfaceTimer/ClockText").GetComponent<TextMeshPro>();
        interfaceTimer = GameObject.Find("NearInterface/InterfaceTimer").gameObject;

        exitIcon = GameObject.Find("NearInterface/ExitOrComplete/IconAndText/Icon");
        // GameObject iconText = transform.parent.transform.Find("IconAndText").gameObject;
        // icon = iconText.transform.Find("Icon").gameObject;

        exitMat = Resources.Load("Mat/ExitButton", typeof(Material)) as Material;
        completeMat = Resources.Load("Mat/CompleteButton", typeof(Material)) as Material;
        prevStepNum = 0;
        lockIcons = new List<GameObject>();
        GameObject lockIcon;
        lockIcon = GameObject.Find("HeadLockCanvas/SimulationInterface/Lock/IconAndText/Icon");
        lockIcons.Add(lockIcon);
        lockIcon = GameObject.Find("HeadLockCanvas/NearInterface/Lock/IconAndText/Icon");
        lockIcons.Add(lockIcon);
        lockIcon = GameObject.Find("HeadLockCanvas/Onboarding/OnboardingInterface/Lock/IconAndText/Icon");
        lockIcons.Add(lockIcon);
        lockMat = Resources.Load("Mat/ButtonLockMat", typeof(Material)) as Material;
        unlockMat = Resources.Load("Mat/ButtonUnlockMat", typeof(Material)) as Material;
        //closeVideoButton = GameObject.Find("CloseVideoButton").transform.Find("CloseVideo").gameObject;

        appState = GameObject.Find("ApplicationState").GetComponent<ApplicationState>();
        videoPlayer = GameObject.Find("NearInterface/GameInterfaceScreen/InterfaceScreen").GetComponent<VideoPlayer>();
        interfaceScreen = GameObject.Find("NearInterface/GameInterfaceScreen/InterfaceScreen").gameObject;
        actionsCues = new Dictionary<string, VideoClip>();
        uploadVideos();
    }

    public void updateLock(bool islocked)
    {
        Material mat = islocked ? lockMat : unlockMat;
        for (int i = 0; i < lockIcons.Count; i++)
        {
            lockIcons[i].GetComponent<Renderer>().material = mat;
        }
    }

    private void uploadVideos()
    {
        actionsCues["cutting"] = Resources.Load<VideoClip>("actions/cutting");
        actionsCues["cracking"] = Resources.Load<VideoClip>("actions/egg_cracking");
        actionsCues["mixing"] = Resources.Load<VideoClip>("actions/mixing");
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
        if (!GameObject.Find("Scheduler").activeSelf || !mainScheduler.tutorialStarts) {
            return;
        }
        Dictionary<string, List<string>> info = mainScheduler.getCurrentStepInfo();
        if (info == null) {
            return;
        }
        if (nearInterface.activeSelf)
        {
            int currentStepNum = Int32.Parse(info["StepNum"][0]);
            if (currentStepNum != prevStepNum) {
                // Debug.Log("next step " + currentStepNum + " " + prevStepNum);
                // ParticleSystem.SetActive(false);
                // locationSetForCurrentStep = false;
                interfaceScreen.SetActive(true);
                prevStepNum = currentStepNum;
            }

            // display step number
            string stepNumberText = "Step #" + currentStepNum;
            stepNumberNearMenu.text = stepNumberText;

            // display instruction text
            string instructionText = info["description"][0];

            string detailText = "Ingredients:\n";
            List<string> ingredients = info["ingredients"];
            List<string> measurement = info["measurement"];
            if (ingredients.Count == 0) {
                detailText += "-\n";
            } else {
                for (int i = 0; i < ingredients.Count; i++) {
                    detailText += ingredients[i];
                    if (measurement[i].Length != 0) {
                        detailText += " - " + measurement[i];
                    }
                    detailText += "\n";
                }
            }
            detailText += "\n";

            detailText += "Equipment:\n";
            List<string> equipment = info["equipment"];
            if (equipment.Count == 0) {
                detailText += "-\n";
            } else {
                for (int i = 0; i < equipment.Count; i++) {
                    detailText += equipment[i] + "\n";
                }
            }

            instructionTextNearMenu.text = instructionText;
            detailTextNearMenu.text = detailText;

            // find location of the main equipment
            Vector3 timerLocation;
            if (equipment.Count > 0) {
                string utensil = equipment[0];
                timerLocation = appState.GetLocation(utensil);
                if (timerLocation != Vector3.zero) {
                    ParticleSystem.transform.position = timerLocation;
                    ParticleSystem.SetActive(true);
                    timerLocation = new Vector3(timerLocation.x, timerLocation.y + 0.2f, timerLocation.z);
                    videoPlayer.transform.position = timerLocation;
                } else {
                    ParticleSystem.SetActive(false);
                }
            }

            // display video
            if (interfaceScreen.activeSelf) {
                string action = info["action"][0];
                if (actionsCues.ContainsKey(action)) {
                    VideoClip video = actionsCues[action];
                    videoPlayer.clip = video;
                    videoPlayer.Play();
                } else {
                    interfaceScreen.SetActive(false);
                }
            }
            
            /*
            Renderer temp = ImagePlane.GetComponent<Renderer>();
            temp.material.mainTexture = mainScheduler.getCurrentStepImage();
            */
            
            // TODO: add done and exit switch
            
            if (mainScheduler.isTutorialDone())
            {
                exitIcon.GetComponent<Renderer>().material = completeMat;
            }
            else
            {
                exitIcon.GetComponent<Renderer>().material = exitMat;
            }

            // set time
            if (info["timer"][0] == "") {
                interfaceTimer.SetActive(false);
            } else {
                interfaceTimer.SetActive(true);
                clockNearMenu.text = info["timer"][0];
            }

            // update the spatial timer:

            //if (timerLocation != Vector3.zero)
            //{
            //    Debug.Log("spaitail timer activated");
            //    timer.SetActive(true);
            //    timer.transform.position = timerLocation;
            //    timer.transform.FindChild("Canvas/Text").GetComponent<Text>().text = info["timer"][0];
            //} else
            //{
            //    timer.SetActive(false);
            //}
        }
        /*
        if (gameInterface.activeSelf) {
            instructionTextFloatingInterf.text = info["description"][0];
            clockFloatingInterf.text = info["timer"][0];
        }*/
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hand" && interfaceManager.clickOk()) {
            interfaceManager.clickButton();
            GetComponent<Button>().onClick.Invoke();
            if (name == "CloseVideo") {
                interfaceScreen.SetActive(false);
            }
        }
    }
    public void clicked()
    {
        if (name == "CloseVideo") {
            interfaceScreen.SetActive(false);
        } else {
            Debug.Log("unknown button");
        }
    }
}
