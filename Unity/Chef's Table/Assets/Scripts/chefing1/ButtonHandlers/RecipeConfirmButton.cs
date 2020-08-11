using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeConfirmButton : MonoBehaviour
{

    InterfaceManager interfaceManager;
    UIFadingAnimation fader;
    GameObject sche;
    MainScheduler2 ms2;
    AudioSource buttonClip;

    private void Awake()
    {
        interfaceManager = GameObject.Find("InterfaceManager").GetComponent<InterfaceManager>();
        fader = GameObject.Find("FadingAnimation").GetComponent<UIFadingAnimation>();
        GameObject sche = GameObject.Find("Scheduler");
        ms2 = sche.GetComponent<MainScheduler2>();
        buttonClip = GameObject.Find("Button_Click").GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hand")
        {
            GetComponent<Button>().onClick.Invoke();
        }
    }

    public void clicked()
    {
        if (name == "Confirm")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Confirm").transform.position);
            string recipe_name = transform.parent.parent.Find("Canvas").Find("RecipeName").GetComponent<TextMeshPro>().text;
            ms2.startTutorial(recipe_name);
            interfaceManager.setActiveOnboardingInterface(false);
            interfaceManager.setActiveNearInterface(true);

        } else if (name == "CuttingButton")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("CuttingButton").transform.position);
            interfaceManager.setActiveNearInterface(false);
            interfaceManager.setActiveOnboardingInterface(false);
            interfaceManager.setActiveCuttingSimulation(true);
            interfaceManager.setActiveSimulationInterface(true);

        } else if (name == "Back")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Back").transform.position);
            fader.turnOffPreview();
        }
    }
}
