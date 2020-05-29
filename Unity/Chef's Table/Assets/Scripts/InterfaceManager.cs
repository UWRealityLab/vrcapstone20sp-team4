using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InterfaceManager : MonoBehaviour
{
    GameObject nearInterface;
    GameObject simulationInterface;
    GameObject onboardingInterface;
    GameObject cuttingSimulation;
    GameObject floatingInterface;
    GameObject summary; // for completion page
    MainScheduler ms;
    private bool startCountDown = false;
    private float completeRedirectTimer = 10;

    // Start is called before the first frame update
    void Start()
    {
        nearInterface = GameObject.Find("NearInterface");
        simulationInterface = GameObject.Find("SimulationInterface");
        onboardingInterface = GameObject.Find("OnBoardingInterface");
        cuttingSimulation = GameObject.Find("CuttingSimulation");
        floatingInterface = GameObject.Find("Interf");
        ms = GameObject.Find("Scheduler").GetComponent<MainScheduler>();
        summary = GameObject.Find("summary");
        summary.SetActive(false);
        setActiveFloatingInterface(false);
        setActiveNearInterface(false);
        setActiveSimulationInterface(false);
        setActiveCuttingSimulation(false);
        setActiveOnboardingInterface(true);
    }

    public void setActiveCuttingSimulation(bool b)
    {
        cuttingSimulation.SetActive(b);
    }

    public void setActiveNearInterface(bool b)
    {
        nearInterface.SetActive(b);
        completeRedirectTimer = 10;
    }

    public void setActiveSimulationInterface(bool b)
    {
        simulationInterface.SetActive(b);
    }

    public void setActiveOnboardingInterface(bool b)
    {
        onboardingInterface.SetActive(b);
    }

    public void setActiveFloatingInterface(bool b)
    {
        floatingInterface.SetActive(b);
    }

    public bool isActiveSimulationInterface()
    {
        return simulationInterface.activeSelf;
    }

    public bool isActiveCuttingSimulation()
    {
        return cuttingSimulation.activeSelf;
    }

    public bool isActiveOnboardingInterface()
    {
        return onboardingInterface.activeSelf;
    }

    public bool isActiveNearInterface()
    {
        return nearInterface.activeSelf;
    }

    public void exitSimulation()
    {
        // to avoid bug, set everything
        setActiveCuttingSimulation(false);
        setActiveOnboardingInterface(true);
        setActiveSimulationInterface(false);
        setActiveNearInterface(false);
    }

    public void endTutorialGeneral()
    {
        TextMeshPro ExitOrComplete = GameObject.Find("NearInterface/ExitOrComplete/IconAndText/Text").GetComponent<TextMeshPro>();
        if (ExitOrComplete.text == "Exit")
        {
            exitTutorial();
        } else
        {
            completeTutorial();
        }
    }
    void completeTutorial() // this should bring up the a summary page, 
    {
        setActiveNearInterface(false);
        summary.SetActive(true);
        startCountDown = true;
        // populate with backend in update
    }

    void exitTutorial() // bring up a confirm message
    {
        setActiveCuttingSimulation(false);
        setActiveOnboardingInterface(true);
        setActiveSimulationInterface(false);
        setActiveNearInterface(false);
        summary.SetActive(false);
        startCountDown = false;
        ms.reset();
    }
    // Update is called once per frame
    void Update()
    {
        if (startCountDown)
        {
            Debug.Log("reach here");
            completeRedirectTimer -= Time.deltaTime;

            if (completeRedirectTimer < 0)
            {
                completeRedirectTimer = 0;
               
                exitTutorial();
            }
            TextMeshProUGUI statistic = summary.transform.Find("Congratulation").Find("statistic").GetComponent<TextMeshProUGUI>();
            List<string> summaryList = ms.getSummary();
            statistic.text = "Congratulation!\n\n you have successfully made a ";
            statistic.text += summaryList[0] + "\n\n";
            statistic.text += "Total time you spend: " + summaryList[1];
            TextMeshProUGUI cd = summary.transform.Find("Congratulation").Find("countDown").GetComponent<TextMeshProUGUI>();
            cd.text = "Redirecting in " + Mathf.Floor(completeRedirectTimer).ToString() + "s";

        }
    }
}
