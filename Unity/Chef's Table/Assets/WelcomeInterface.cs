using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WelcomeInterface : MonoBehaviour
{
    NIThresholdControl NIControl;
    AudioSource buttonClip;
    InterfaceManager interfaceManager;
    GameObject icon;
    GameObject scan;

    private void Awake()
    {
        NIControl = GameObject.Find("HeadLockCanvas").GetComponent<NIThresholdControl>();
        buttonClip = GameObject.Find("Button_Click").GetComponent<AudioSource>();
        interfaceManager = GameObject.Find("InterfaceManager").GetComponent<InterfaceManager>();
        if (name == "BrowseRecipeLibrary" || name == "ScanMyIngredients") return;
        GameObject iconText = transform.parent.transform.Find("IconAndText").gameObject;
        icon = iconText.transform.Find("Icon").gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hand")
        {
            GetComponent<Button>().onClick.Invoke();
            if (name == "Simulation" || name == "BrowseRecipeLibrary" || name == "ScanMyIngredients") return;
            StartCoroutine(ShowFeedback());
        }
    }

    private IEnumerator ShowFeedback()
    {
        if (name != "Lock")
        {
            icon.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        }
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(1.0f);
        GetComponent<Collider>().enabled = true;
        if (name != "Lock")
        {
            icon.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        }
    }

    public void clicked()
    {
        if (name == "ScanMyIngredientsButton")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("ScanMyIngredientsButton").transform.position);
            interfaceManager.setActiveNearInterface(false);
            interfaceManager.setActiveOnboardingInterface(false);
            interfaceManager.setActiveCuttingSimulation(false);
            interfaceManager.setActiveSimulationInterface(false);
            interfaceManager.setActiveWelcomeInterface(false);
            interfaceManager.setActiveScanningInterface(true);

        }
        else if (name == "BrowseRecipeLibrary")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("BrowseRecipeButton").transform.position);
            interfaceManager.setActiveNearInterface(false);
            interfaceManager.setActiveCuttingSimulation(false);
            interfaceManager.setActiveSimulationInterface(false);
            interfaceManager.setActiveScanningInterface(false);
            interfaceManager.setActiveWelcomeInterface(false);
            interfaceManager.setActiveOnboardingInterface(true);
        }
        else if (name == "Lock")
        {
            NIControl.changeLock();
            UpdateInGameInterface uii = GameObject.Find("InGameInterface").GetComponent<UpdateInGameInterface>();
            uii.updateLock(NIControl.getLock());
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Lock").transform.position);
        }
    }
}
