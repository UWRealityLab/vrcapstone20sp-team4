using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InterfaceManager : MonoBehaviour
{
    GameObject nearInterface;
    GameObject simulationInterface;
    GameObject onboardingInterface;
    GameObject onboarding;
    GameObject cuttingSimulation;
    GameObject summary; // for completion page
    MainScheduler ms;
    UIFadingAnimation animator;
    GameObject onboardingPreview;
    GameObject headLockCanvas;
    GameObject wrappingSimulation;
    private bool startCountDown = false;
    private float completeRedirectTimer = 10;

    private void Awake()
    {
        nearInterface = GameObject.Find("NearInterface");
        simulationInterface = GameObject.Find("SimulationInterface");
        onboarding = GameObject.Find("Onboarding");
        wrappingSimulation = GameObject.Find("wrappingSimulation");
        cuttingSimulation = GameObject.Find("CuttingSimulation");
        onboardingPreview = GameObject.Find("Onboarding").transform.Find("OnboardingPreview").gameObject;
        onboardingInterface = GameObject.Find("Onboarding").transform.Find("OnboardingInterface").gameObject;
        animator = GameObject.Find("FadingAnimation").GetComponent<UIFadingAnimation>();
        ms = GameObject.Find("Scheduler").GetComponent<MainScheduler>();
        summary = GameObject.Find("summary");
        headLockCanvas = GameObject.Find("HeadLockCanvas");
    }

    // Start is called before the first frame update
    void Start()
    {
        summary.SetActive(false);
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
        
        if (b)
        {
            StartCoroutine(animator.FadeIn(nearInterface));
        }
        else
        {
            StartCoroutine(animator.FadeOut(nearInterface));
        }
        //nearInterface.SetActive(b);
        completeRedirectTimer = 10;
    }

    public void setActiveSimulationInterface(bool b)
    {
        
        if (b)
        {
            StartCoroutine(animator.FadeIn(simulationInterface));
        }
        else
        {
            StartCoroutine(animator.FadeOut(simulationInterface));
        }
        //simulationInterface.SetActive(b);
    }

    public void setActiveOnboardingInterface(bool b)
    {
        if (b)
        {
            onboardingPreview.SetActive(false);
            onboardingInterface.SetActive(true);
            StartCoroutine(animator.FadeIn(onboarding));
        }
        else
        {
            StartCoroutine(animator.FadeOut(onboarding));
        }
    }

    public void setActiveHeadLockCanvas(bool b)
    {
        if (b)
        {
            StartCoroutine(animator.FadeIn(headLockCanvas));
        }
        else
        {
            StartCoroutine(animator.FadeOut(headLockCanvas));
        }
    }

    public bool isActiveHeadLockCanvas()
    {
        return headLockCanvas.activeSelf;
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

    public bool isActiveWrappingSimulation()
    {
        return wrappingSimulation.activeSelf;
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
        }
        else
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
