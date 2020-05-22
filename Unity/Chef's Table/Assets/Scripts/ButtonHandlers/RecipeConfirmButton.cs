using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecipeConfirmButton : MonoBehaviour
{
    GameObject simulationInterface;
    GameObject cuttingSimulation;
    GameObject onboardingInterface;
    private void Start()
    {
        simulationInterface = GameObject.Find("SimulationInterface");
        cuttingSimulation = GameObject.Find("CuttingSimulation");
        onboardingInterface = GameObject.Find("OnBoardingInterface");
    }
    public void clicked()
    {
        if (name == "Confirm")
        {
            string recipe_name = transform.parent.GetComponentInChildren<TextMeshProUGUI>().text;
            GameObject sche = GameObject.Find("Scheduler");
            MainScheduler ms = sche.GetComponent<MainScheduler>();
            ms.startTutorial(recipe_name);
            onboardingInterface.SetActive(false);
            Debug.Log(recipe_name);
        } else if (name == "CuttingButton")
        {
            onboardingInterface.SetActive(false);
            simulationInterface.SetActive(true);
            cuttingSimulation.SetActive(true);
        } 
    }
}
