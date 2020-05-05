using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateInGameInterface : MonoBehaviour
{
    private MainScheduler mainScheduler;
    private TextMeshProUGUI instructionText;
    private TextMeshProUGUI clock;
    private GameObject gameInterface;

    // Start is called before the first frsame update
    void Start()
    {
        mainScheduler = GameObject.Find("Scheduler").GetComponent<MainScheduler>();
        instructionText = GameObject.Find("Interf/InstructionCanvas/Instructions").GetComponent<TextMeshProUGUI>();
        clock = GameObject.Find("Interf/TimerInterface/ClockText").GetComponent<TextMeshProUGUI>();
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
        instructionText.text = info["description"][0];
        clock.text = info["timer"][0];
    }
}
