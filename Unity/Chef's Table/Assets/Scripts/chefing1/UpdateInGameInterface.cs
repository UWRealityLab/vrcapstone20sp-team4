using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

public class UpdateInGameInterface : MonoBehaviour
{
    private MainScheduler2 mainScheduler;
    // private VisualCueManager visualCueManager;
    //private TextMeshProUGUI instructionTextFloatingInterf;
    private TextMeshPro instructionTextNearMenu;
    private TextMeshPro detailTextNearMenu;
    private GameObject nearInterface;
    //private TextMeshProUGUI clockFloatingInterf;
    private TextMeshPro clockNearMenu;
    //private GameObject gameInterface;
    private GameObject exitIcon;

    private Material exitMat;
    private Material completeMat;
    private int step;
    private List<GameObject> lockIcons;
    private Material lockMat;
    private Material unlockMat;
    // private GameObject ImagePlane;

    private ApplicationState appState;
    private VideoPlayer videoPlayer;
    private Dictionary<string, VideoClip> actionsCues;
    // private float criticalEquipmentUpdateTimer = 0;

    // Start is called before the first frsame update
    void Awake()
    {
        mainScheduler = GameObject.Find("Scheduler").GetComponent<MainScheduler2>();
        // visualCueManager = GameObject.Find("VisualCueManager").GetComponent<VisualCueManager>();
        nearInterface = GameObject.Find("NearInterface");
        instructionTextNearMenu = GameObject.Find("NearInterface/InstructionCanvas/Instruction").GetComponent<TextMeshPro>();
        detailTextNearMenu = GameObject.Find("NearInterface/InstructionCanvas/Detail").GetComponent<TextMeshPro>();
        clockNearMenu = GameObject.Find("NearInterface/InterfaceTimer/ClockText").GetComponent<TextMeshPro>();
        
        exitIcon = GameObject.Find("NearInterface/ExitOrComplete/IconAndText/Icon");
        exitMat = Resources.Load("Mat/ExitButton", typeof(Material)) as Material;
        completeMat = Resources.Load("Mat/CompleteButton", typeof(Material)) as Material;
        step = -1;
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
        
        appState = GameObject.Find("ApplicationState").GetComponent<ApplicationState>();
        videoPlayer = GameObject.Find("NearInterface/GameInterfaceScreen/InterfaceScreen").GetComponent<VideoPlayer>();
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
        actionsCues["cut"] = Resources.Load<VideoClip>("actions/cutting");
        actionsCues["crack"] = Resources.Load<VideoClip>("actions/egg_cracking");
        actionsCues["heat"] = Resources.Load<VideoClip>("actions/heating");
        // actionsCues["melt"] = Resources.Load<VideoClip>("actions/melting");
        actionsCues["mix"] = Resources.Load<VideoClip>("actions/mixing");
        actionsCues["slice"] = Resources.Load<VideoClip>("actions/slicing");
        actionsCues["spread"] = Resources.Load<VideoClip>("actions/spread");
        actionsCues["sprinkle"] = Resources.Load<VideoClip>("actions/sprinkle");
    }

    // Update is called once per frame
    void Update()
    {
        Dictionary<string, List<string>> info = mainScheduler.getCurrentStepInfo();
        if (info == null) {
            return;
        }
        if (nearInterface.activeSelf)
        {
            // display instruction text
            string instructionText = info["description"][0];

            string detailText = "Ingredients:\n";
            List<string> ingredients = info["ingredients"];
            if (ingredients.Count == 0) {
                detailText += "-\n";
            } else {
                for (int i = 0; i < ingredients.Count; i++) {
                    detailText += ingredients[i] + "\n";
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

            // set time
            clockNearMenu.text = info["timer"][0];

            // find location of the main equipment
            Vector3 timerLocation;
            if (equipment.Count > 0) {
                // string utensil = equipment[0];
                string utensil = "bowl";
                timerLocation = appState.GetLocation(utensil);
                if (timerLocation != Vector3.zero) {
                    timerLocation += new Vector3(timerLocation.x, timerLocation.y + 0.7f, timerLocation.z);
                    videoPlayer.transform.position = timerLocation;
                }
            }

            // display video
            string action = info["action"][0];
            // set the video clip and play
            VideoClip video = actionsCues[action];
            videoPlayer.clip = video;
            videoPlayer.Play();
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
}
