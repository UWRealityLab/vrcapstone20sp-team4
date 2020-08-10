using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameInterfaceButtons : MonoBehaviour
{
    // Start is called before the first frame update
    MainScheduler2 scheduler;
    AudioSource buttonClip;
    AudioSource timerClip;

    void Start()
    {
        scheduler = GameObject.Find("Scheduler").GetComponent<MainScheduler2>();
        buttonClip = GameObject.Find("Button_Click").GetComponent<AudioSource>();
        timerClip = GameObject.Find("Timer").GetComponent<AudioSource>();
    }

    public void clicked()
    {
        if (name == "Next") {
            //Debug.Log("Next Button, " + name);
            scheduler.toNextStep();
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Next").transform.position);
        } else if (name == "Back") {
            //Debug.Log("Back Button, " + name);
            scheduler.toPreviousStep();
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Back").transform.position);
        } else if (name == "Start") {
            //Debug.Log("Start Button, " + name);
            scheduler.changeTimerStatus(1);
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Start").transform.position);
        } else if (name == "Reset") {
            //Debug.Log("Reset Button, " + name);
            scheduler.changeTimerStatus(2);
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Reset").transform.position);
            timerClip.Stop();
        } else if (name == "Pause") {
            scheduler.changeTimerStatus(0);
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Pause").transform.position);
            timerClip.Stop();
        } /*else if (name == "Plus") {
            scheduler.addToTimer();
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Plus").transform.position);
        } else if (name == "Minus") {
            scheduler.subtractFromTimer();
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Minus").transform.position);
        }*/ else {
            Debug.Log("Unknown button");
        } 
    }
}
