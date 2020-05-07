using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameInterfaceButtons : MonoBehaviour
{
    // Start is called before the first frame update
    MainScheduler scheduler;

    void Start()
    {
        scheduler = GameObject.Find("Scheduler").GetComponent<MainScheduler>();
    }

    public void clicked()
    {
        if (name == "Next") {
            //Debug.Log("Next Button, " + name);
            scheduler.toNextStep();
        } else if (name == "Back") {
            //Debug.Log("Back Button, " + name);
            scheduler.toPreviousStep();
        } else if (name == "Start") {
            //Debug.Log("Start Button, " + name);
            scheduler.changeTimerStatus(1);
        } else if (name == "Reset") {
            //Debug.Log("Reset Button, " + name);
            scheduler.changeTimerStatus(2);
        } else if (name == "Pause") {
            scheduler.changeTimerStatus(0);
        } else if (name == "Plus") {
            scheduler.addToTimer();
        } else if (name == "Minus") {
            scheduler.subtractFromTimer();
        } else {
            Debug.Log("Unknown button");
        } 
    }
}
