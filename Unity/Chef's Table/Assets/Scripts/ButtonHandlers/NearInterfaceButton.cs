﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NearInterfaceButton : MonoBehaviour
{
    // Start is called before the first frame update
    MainScheduler scheduler;
    AudioSource buttonClip;
    AudioSource timerClip;
    GameObject text;
    GameObject icon;
    NIThresholdControl NIControl;
    InterfaceManager interfaceManager;
    changeSimulation changeSimulationScript;

    void Awake()
    {
        scheduler = GameObject.Find("Scheduler").GetComponent<MainScheduler>();
        interfaceManager = GameObject.Find("InterfaceManager").GetComponent<InterfaceManager>();
        changeSimulationScript = GameObject.Find("CuttingSimulation").GetComponent<changeSimulation>();
        buttonClip = GameObject.Find("Button_Click").GetComponent<AudioSource>();
        timerClip = GameObject.Find("Timer").GetComponent<AudioSource>();
        GameObject iconText = transform.parent.transform.Find("IconAndText").gameObject;
        text = iconText.transform.Find("Text").gameObject;
        icon = iconText.transform.Find("Icon").gameObject;
        NIControl = GameObject.Find("HeadLockCanvas").GetComponent<NIThresholdControl>();

    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hand")
        {
            GetComponent<Button>().onClick.Invoke();
            StartCoroutine(ShowFeedback());
        }
    }

    private IEnumerator ShowFeedback()
    {
        if (name != "Lock")
        {
            text.GetComponent<TextMeshPro>().color = Color.red;
            icon.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        }
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(1.0f);
        GetComponent<Collider>().enabled = true;
        if (name != "Lock")
        {
            text.GetComponent<TextMeshPro>().color = Color.white;
            icon.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        }
        if (name == "Exit")
        {
            interfaceManager.setActiveCuttingSimulation(false);
            interfaceManager.setActiveSimulationInterface(false);
            interfaceManager.setActiveNearInterface(false);
            interfaceManager.setActiveOnboardingInterface(true);
        }
    }

    public void clicked()
    {
        if (name == "Next")
        {
            Debug.Log("Next Button, " + name);
            scheduler.toNextStep();
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Next").transform.position);
        }
        else if (name == "Back")
        {
            //Debug.Log("Back Button, " + name);
            scheduler.toPreviousStep();
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Back").transform.position);
        }
        else if (name == "Start")
        {
            //Debug.Log("Start Button, " + name);
            scheduler.changeTimerStatus(1);
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Start").transform.position);
        }
        else if (name == "Reset")
        {
            //Debug.Log("Reset Button, " + name);
            scheduler.changeTimerStatus(2);
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Reset").transform.position);
            timerClip.Stop();
        }
        else if (name == "Pause")
        {
            scheduler.changeTimerStatus(0);
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Pause").transform.position);
            timerClip.Stop();
        }
        else if (name == "Plus")
        {
            scheduler.addToTimer();
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Plus").transform.position);
        }
        else if (name == "Minus")
        {
            scheduler.subtractFromTimer();
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Minus").transform.position);
        } 
        else if (name == "Lock")
        {
            NIControl.changeLock();
            //GameObject icon = transform.parent.Find("IconAndText").Find("Icon").gameObject;
            Renderer rend = icon.GetComponent<Renderer>();
            if (NIControl.getLock())
            {
                // assign different material
                rend.material.color = Color.red;
                text.GetComponent<TextMeshPro>().color = Color.red;
            } else
            {
                rend.material.color = Color.blue;
                text.GetComponent<TextMeshPro>().color = Color.blue;
            }
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Lock").transform.position);
        }
        else if (name == "SimuNext")
        {
            Debug.Log("Next Button, " + name);
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("SimuNext").transform.position);
            changeSimulationScript.nextObject();
        }
        else if (name == "SimuBack")
        {
            //Debug.Log("Back Button, " + name);
            changeSimulationScript.previousObject();
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("SimuBack").transform.position);
        }
        else if (name == "SimuReset")
        {
            //Debug.Log("Reset Button, " + name);
            changeSimulationScript.resetObject();
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("SimuReset").transform.position);
        }
        else if (name == "Exit") 
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Exit").transform.position);
        }
        else if (name == "SwitchSimulationMode")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("SwitchSimulationMode").transform.position);
            changeSimulationScript.resetMode();
        } 
        else if (name == "AddIngredients")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("AddIngredients").transform.position);
            changeSimulationScript.addIngredients();
        }
        else 
        {
            Debug.Log("Unknown button");
        }
    }
}
