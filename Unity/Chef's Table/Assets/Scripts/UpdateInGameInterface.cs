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

    // Start is called before the first frsame update
    void Start()
    {
        mainScheduler = GameObject.Find("Scheduler").GetComponent<MainScheduler>();
        instructionTextFloatingInterf = GameObject.Find("Interf/InstructionCanvas/Instructions").GetComponent<TextMeshProUGUI>();
        instructionTextNearMenu = GameObject.Find("NearInterface/InstructionPanel/Instruction").GetComponent<TextMeshPro>();
        clockFloatingInterf = GameObject.Find("Interf/TimerInterface/ClockText").GetComponent<TextMeshProUGUI>();
        clockNearMenu = GameObject.Find("NearInterface/InstructionPanel/Clock").GetComponent<TextMeshPro>();
        gameInterface = GameObject.Find("Interf");
        gameInterface.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Dictionary<string, List<string>> info = mainScheduler.getCurrentStepInfo();
        if (info == null) {
            return;
        }
        if (!gameInterface.activeSelf) {
            gameInterface.SetActive(true);
        }
        instructionTextFloatingInterf.text = info["description"][0];
        clockFloatingInterf.text = info["timer"][0];
        instructionTextNearMenu.text = info["description"][0];
        clockNearMenu.text = info["timer"][0];
    }
}
