using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateInGameInterface : MonoBehaviour
{
    private MainScheduler2 mainScheduler;
    private VisualCueManager visualCueManager;
    //private TextMeshProUGUI instructionTextFloatingInterf;
    private TextMeshPro instructionTextNearMenu;
    private TextMeshPro detailTextNearMenu;
    //private TextMeshProUGUI clockFloatingInterf;
    private TextMeshPro clockNearMenu;
    //private GameObject gameInterface;
    private GameObject nearInterface;
    private GameObject exitIcon;

    private Material exitMat;
    private Material completeMat;
    private int step;
    private List<GameObject> lockIcons;
    private Material lockMat;
    private Material unlockMat;
    // private GameObject ImagePlane;

    // Start is called before the first frsame update
    void Awake()
    {
        mainScheduler = GameObject.Find("Scheduler").GetComponent<MainScheduler2>();
        visualCueManager = GameObject.Find("VisualCueManager").GetComponent<VisualCueManager>();
        nearInterface = GameObject.Find("NearInterface");
        instructionTextNearMenu = GameObject.Find("NearInterface/InstructionCanvas/Instruction").GetComponent<TextMeshPro>();
        detailTextNearMenu = GameObject.Find("NearInterface/InstructionCanvas/Detail").GetComponent<TextMeshPro>();
        clockNearMenu = GameObject.Find("NearInterface/InterfaceTimer/ClockText").GetComponent<TextMeshPro>();
        exitIcon = GameObject.Find("NearInterface/ExitOrComplete/IconAndText/Icon");
        exitMat = Resources.Load("Mat/ExitButton", typeof(Material)) as Material;
        completeMat = Resources.Load("Mat/CompleteButton", typeof(Material)) as Material;
        // ImagePlane = GameObject.Find("NearInterface/GameInterfaceScreen/InterfaceScreen");
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
            // Debug.Log("no info");
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

            // display video
            string action = info["action"][0];
            if (equipment.Count > 0) {
                visualCueManager.DisplayVideo(equipment[0], action);
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
