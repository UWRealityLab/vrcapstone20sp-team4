using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class InterfaceManager : MonoBehaviour
{
    GameObject nearInterface;
    GameObject simulationInterface;
    GameObject onboardingInterface;
    GameObject onboarding;
    GameObject cuttingSimulation;
    GameObject summary; // for completion page
    MainScheduler2 ms;
    UIFadingAnimation animator;
    GameObject onboardingPreview;
    GameObject headLockCanvas;
    GameObject wrappingSimulation;
    GameObject scanningInterface;
    GameObject scanningStart;
    GameObject scanningState;
    GameObject scanningConfirm;
    GameObject scanningIngredientNamesDisplay;
    GameObject brackets;
    GameObject welcomeInterface;
    private bool startCountDown = false;
    private float completeRedirectTimer = 10;

    private void Awake()
    {
        
        scanningStart = GameObject.Find("StartScreen");
        scanningState = GameObject.Find("ScanningText");
        scanningConfirm = GameObject.Find("IngredientScanGet");
        scanningIngredientNamesDisplay = GameObject.Find("Ingredients");
        brackets = GameObject.Find("Brackets");

        welcomeInterface = GameObject.Find("WelcomeInterface");
        scanningInterface = GameObject.Find("ScanningInterface");
        
        nearInterface = GameObject.Find("NearInterface");
        simulationInterface = GameObject.Find("SimulationInterface");
        onboarding = GameObject.Find("Onboarding");
        wrappingSimulation = GameObject.Find("wrappingSimulation");
        cuttingSimulation = GameObject.Find("CuttingSimulation");
        onboardingPreview = GameObject.Find("Onboarding").transform.Find("OnboardingPreview").gameObject;
        onboardingInterface = GameObject.Find("Onboarding").transform.Find("OnboardingInterface").gameObject;
        animator = GameObject.Find("FadingAnimation").GetComponent<UIFadingAnimation>();
        ms = GameObject.Find("Scheduler").GetComponent<MainScheduler2>();
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
        setActiveScanningInterface(false);
        setActiveOnboardingInterface(false);
        setActiveWelcomeInterface(true);
    }

    public void setActiveCuttingSimulation(bool b)
    {
        cuttingSimulation.SetActive(b);
    }

    public async void setActiveNearInterface(bool b)
    {
        GameObject ObjectDetection = GameObject.Find("objectdetection");
        if (b)
        {
            ObjectDetection.SetActive(true);
            await Task.Delay(3000);
            StartCoroutine(animator.FadeIn(nearInterface));
        }
        else
        {
            //if (ObjectDetection.activeSelf) ObjectDetection.SetActive(false);
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

    public void setActiveWelcomeInterface(bool b)
    {
        if (b)
        {
            StartCoroutine(animator.FadeIn(welcomeInterface));
        }
        else
        {
            StartCoroutine(animator.FadeOut(welcomeInterface));
        }
    }

    public void setActiveOnboardingInterface(bool b)
    {
        
        if (b)
        {
            onboarding.SetActive(true);
            onboardingInterface.SetActive(true);
            
            onboardingPreview.SetActive(false);
            onboarding.GetComponent<InterfaceController>().loadOnboarding();
            StartCoroutine(animator.FadeIn(onboarding));
        }
        else
        {
            onboardingPreview.SetActive(false);
            onboardingInterface.SetActive(false);
            onboarding.SetActive(false);
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
    public void setActiveScanningInterface(bool b)
    {
        if (b)
        {
            scanningStart.SetActive(true);
            scanningState.SetActive(false);
            scanningConfirm.SetActive(false);
            brackets.SetActive(true);
            scanningIngredientNamesDisplay.SetActive(false);
            StartCoroutine(animator.FadeIn(scanningInterface));
        }
        else
        {
            scanningStart.SetActive(false);
            scanningState.SetActive(false);
            scanningConfirm.SetActive(false);
            brackets.SetActive(false);
            scanningIngredientNamesDisplay.SetActive(false);
            StartCoroutine(animator.FadeOut(scanningInterface));
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

    public bool isActiveScanningInterface()
    {
        return scanningInterface.activeSelf;
    }
    public bool isActiveWelcomeInterface()
    {
        return welcomeInterface.activeSelf;
    }

    public void exitSimulation()
    {
        // to avoid bug, set everything
        setActiveCuttingSimulation(false);
        setActiveOnboardingInterface(true);
        setActiveSimulationInterface(false);
        setActiveNearInterface(false);
        setActiveScanningInterface(false);
        setActiveWelcomeInterface(false);
    }

    public void endTutorialGeneral()
    {
        MainScheduler2 mainScheduler = GameObject.Find("Scheduler").GetComponent<MainScheduler2>();
        
        if (!mainScheduler.isTutorialDone())
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
        setActiveScanningInterface(false);
        setActiveWelcomeInterface(false);
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
