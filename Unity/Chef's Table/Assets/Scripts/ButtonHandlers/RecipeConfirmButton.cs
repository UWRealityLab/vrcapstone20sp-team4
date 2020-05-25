using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecipeConfirmButton : MonoBehaviour
{

    InterfaceManager interfaceManager;

    private void Start()
    {
        interfaceManager = GameObject.Find("InterfaceManager").GetComponent<InterfaceManager>();
    }

    public void clicked()
    {
        if (name == "Confirm")
        {
            string recipe_name = transform.parent.GetComponentInChildren<TextMeshProUGUI>().text;
            GameObject sche = GameObject.Find("Scheduler");
            MainScheduler ms = sche.GetComponent<MainScheduler>();
            ms.startTutorial(recipe_name);
            interfaceManager.setActiveOnboardingInterface(false);
            interfaceManager.setActiveNearInterface(true);
        } else if (name == "CuttingButton")
        {
            interfaceManager.setActiveNearInterface(false);
            interfaceManager.setActiveOnboardingInterface(false);
            interfaceManager.setActiveCuttingSimulation(true);
            interfaceManager.setActiveSimulationInterface(true);
        } 
    }
}
