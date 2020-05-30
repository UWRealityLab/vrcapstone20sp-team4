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
    MainScheduler ms;

    private void Awake()
    {
        interfaceManager = GameObject.Find("InterfaceManager").GetComponent<InterfaceManager>();
        fader = GameObject.Find("FadingAnimation").GetComponent<UIFadingAnimation>();
        GameObject sche = GameObject.Find("Scheduler");
        ms = sche.GetComponent<MainScheduler>();
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
            string recipe_name = transform.parent.parent.Find("Canvas").Find("RecipeName").GetComponent<TextMeshPro>().text;
            ms.startTutorial(recipe_name);
            interfaceManager.setActiveOnboardingInterface(false);
            interfaceManager.setActiveNearInterface(true);
        } else if (name == "CuttingButton")
        {
            interfaceManager.setActiveNearInterface(false);
            interfaceManager.setActiveOnboardingInterface(false);
            interfaceManager.setActiveCuttingSimulation(true);
            interfaceManager.setActiveSimulationInterface(true);

        } else if (name == "Back")
        {
            fader.turnOffPreview();
        }
    }
}
