using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

public class UpdateInGameInterface : MonoBehaviour
{
    private MainScheduler mainScheduler;
    //private TextMeshProUGUI instructionTextFloatingInterf;
    private TextMeshPro instructionTextNearMenu;
    //private TextMeshProUGUI clockFloatingInterf;
    private TextMeshPro clockNearMenu;
    //private GameObject gameInterface;
    private GameObject nearInterface;
    private TextMeshPro ExitOrComplete;
    private GameObject exitIcon;
    private Material exitMat;
    private Material completeMat;
    private VideoPlayer videoPlayer;
    private int step;

    // Start is called before the first frsame update
    void Awake()
    {
        mainScheduler = GameObject.Find("Scheduler").GetComponent<MainScheduler>();
        /*
        gameInterface = GameObject.Find("Interf");
        instructionTextFloatingInterf = gameInterface.transform.Find("InstructionCanvas/Instructions").GetComponent<TextMeshProUGUI>();
        clockFloatingInterf = GameObject.Find("Interf/TimerInterface/ClockText").GetComponent<TextMeshProUGUI>();
        */
        nearInterface = GameObject.Find("NearInterface");
        instructionTextNearMenu = GameObject.Find("NearInterface/InstructionCanvas/Instruction").GetComponent<TextMeshPro>();
        clockNearMenu = GameObject.Find("NearInterface/InterfaceTimer/ClockText").GetComponent<TextMeshPro>();
        exitIcon = GameObject.Find("NearInterface/ExitOrComplete/IconAndText/Icon");
        exitMat = Resources.Load("Mat/ExitButton", typeof(Material)) as Material;
        completeMat = Resources.Load("Mat/CompleteButton", typeof(Material)) as Material;
        ExitOrComplete = GameObject.Find("NearInterface/ExitOrComplete/IconAndText/Text").GetComponent<TextMeshPro>();
        videoPlayer = GameObject.Find("NearInterface/GameInterfaceScreen/InterfaceScreen").GetComponent<VideoPlayer>();
        step = -1;
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
            instructionTextNearMenu.text = info["description"][0];
            clockNearMenu.text = info["timer"][0];

            int currentStep = int.Parse(info["stepIndex"][0]);
            if (step != currentStep)
            {
                string name = info["recipe"][0];
                string pathToVideo = "Videos/" + name + "/step_" + (currentStep + 1) + "_video";
                VideoClip video = Resources.Load<VideoClip>(pathToVideo) as VideoClip;
                Debug.Log("video = " + video + ", step = " + currentStep + ", name = " + name);
                videoPlayer.clip = video;
                videoPlayer.Play();
                step = currentStep;
            }
            // TODO: add done and exit switch
            
            if (mainScheduler.isTutorialDone())
            {
                ExitOrComplete.text = "Complete";
                exitIcon.GetComponent<Renderer>().material = completeMat;
            }
            else
            {
                ExitOrComplete.text = "Exit";
                exitIcon.GetComponent<Renderer>().material = exitMat;
            }
            
        }
        /*
        if (gameInterface.activeSelf) {
            instructionTextFloatingInterf.text = info["description"][0];
            clockFloatingInterf.text = info["timer"][0];
        }*/
        
    }
}
