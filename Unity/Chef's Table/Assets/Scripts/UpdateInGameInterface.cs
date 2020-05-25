using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateInGameInterface : MonoBehaviour
{
    private MainScheduler mainScheduler;
    private TextMeshProUGUI instructionTextFloatingInterf;
    private TextMeshPro instructionTextNearMenu;
    private TextMeshProUGUI clockFloatingInterf;
    private TextMeshPro clockNearMenu;
    private GameObject gameInterface;
    private GameObject nearInterface;

    // Start is called before the first frsame update
    void Awake()
    {
        mainScheduler = GameObject.Find("Scheduler").GetComponent<MainScheduler>();
        
        gameInterface = GameObject.Find("Interf");
        instructionTextFloatingInterf = gameInterface.transform.Find("InstructionCanvas/Instructions").GetComponent<TextMeshProUGUI>();
        clockFloatingInterf = GameObject.Find("Interf/TimerInterface/ClockText").GetComponent<TextMeshProUGUI>();
        
        nearInterface = GameObject.Find("NearInterface");
        instructionTextNearMenu = GameObject.Find("NearInterface/InstructionPanel/Instruction").GetComponent<TextMeshPro>();
        clockNearMenu = GameObject.Find("NearInterface/InstructionPanel/Clock").GetComponent<TextMeshPro>();
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
        }
        
        if (gameInterface.activeSelf) {
            instructionTextFloatingInterf.text = info["description"][0];
            clockFloatingInterf.text = info["timer"][0];
        }
        
    }
}
