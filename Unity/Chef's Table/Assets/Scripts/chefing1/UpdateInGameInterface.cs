using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

public class UpdateInGameInterface : MonoBehaviour
{
    private MainScheduler2 mainScheduler;
    //private TextMeshProUGUI instructionTextFloatingInterf;
    private TextMeshPro instructionTextNearMenu;
    //private TextMeshProUGUI clockFloatingInterf;
    private TextMeshPro clockNearMenu;
    //private GameObject gameInterface;
    private GameObject nearInterface;
    private GameObject exitIcon;
    public GameObject spaitialTimerPrefab;
    private GameObject timer;
    private Material exitMat;
    private Material completeMat;
    private int step;
    private List<GameObject> lockIcons;
    private Material lockMat;
    private Material unlockMat;
    private GameObject ImagePlane;

    // Start is called before the first frsame update
    void Awake()
    {
        mainScheduler = GameObject.Find("Scheduler").GetComponent<MainScheduler2>();
        nearInterface = GameObject.Find("NearInterface");
        instructionTextNearMenu = GameObject.Find("NearInterface/InstructionCanvas/Instruction").GetComponent<TextMeshPro>();
        clockNearMenu = GameObject.Find("NearInterface/InterfaceTimer/ClockText").GetComponent<TextMeshPro>();
        exitIcon = GameObject.Find("NearInterface/ExitOrComplete/IconAndText/Icon");
        exitMat = Resources.Load("Mat/ExitButton", typeof(Material)) as Material;
        completeMat = Resources.Load("Mat/CompleteButton", typeof(Material)) as Material;
        ImagePlane = GameObject.Find("NearInterface/GameInterfaceScreen/InterfaceScreen");
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
        timer = Instantiate(spaitialTimerPrefab, this.transform);
        timer.SetActive(false);
    }

    public void updateLock(bool islocked)
    {
        Material mat = islocked ? lockMat : unlockMat;
        for (int i = 0; i < lockIcons.Count; i++)
        {
            lockIcons[i].GetComponent<Renderer>().material = mat;
        }
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
            Renderer temp = ImagePlane.GetComponent<Renderer>();
            temp.material.mainTexture = mainScheduler.getCurrentStepImage();
            //int currentStep = int.Parse(info["stepIndex"][0]);
            //if (step != currentStep)
            //{
            //    string name = info["recipe"][0];
            //    string pathToVideo = "Videos/" + name + "/step_" + (currentStep + 1) + "_video";
            //    VideoClip video = Resources.Load<VideoClip>(pathToVideo) as VideoClip;
            //    Debug.Log("video = " + video + ", step = " + currentStep + ", name = " + name);
            //    videoPlayer.clip = video;
            //    videoPlayer.Play();
            //    step = currentStep;
            //}
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
