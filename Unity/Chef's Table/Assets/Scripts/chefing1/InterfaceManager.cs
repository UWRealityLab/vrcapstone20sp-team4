using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class InterfaceManager : MonoBehaviour
{
    GameObject nearInterface;
    GameObject onboardingInterface;
    GameObject onboarding;
    GameObject summary; // for completion page
    MainScheduler2 ms;
    UIFadingAnimation animator;
    GameObject onboardingPreview;
    GameObject headLockCanvas;
    GameObject wrappingSimulation;
    GameObject scanningInterface;
    GameObject scanningActive;
    GameObject beginScan;
    GameObject VirtualHandKeyboard;
    GameObject scanningIngredientNamesDisplay;
    GameObject welcomeInterface;
    private bool startCountDown = false;
    private float completeRedirectTimer = 10;
    private float clickCountDown = 0.05f;

    

    private void Awake()
    {
        //scanningIngredientNamesDisplay = GameObject.Find("Ingredients");
        scanningActive = GameObject.Find("KeepInFront");
        beginScan = GameObject.Find("ScanScreen");
        VirtualHandKeyboard = GameObject.Find("VirtualHandKeyboard");
        welcomeInterface = GameObject.Find("WelcomeInterface");
        scanningInterface = GameObject.Find("ScanningContainer");
        nearInterface = GameObject.Find("NearInterface");
        onboarding = GameObject.Find("Onboarding");
        onboardingPreview = GameObject.Find("Onboarding").transform.Find("OnboardingPreview").gameObject;
        onboardingInterface = GameObject.Find("Onboarding").transform.Find("OnboardingInterface").gameObject;
        animator = GameObject.Find("FadingAnimation").GetComponent<UIFadingAnimation>();
        ms = GameObject.Find("Scheduler").GetComponent<MainScheduler2>();
        summary = GameObject.Find("summary");
        headLockCanvas = GameObject.Find("HeadLockCanvas");
        
    }


    public bool clickOk()
    {
        return clickCountDown == 0;
    }

    public void clickButton()
    {
        clickCountDown = 1.5f;
    }

    // Start is called before the first frame update
    void Start()
    {
        summary.SetActive(false);
        setActiveNearInterface(false);
        setActiveScanningInterface(false);
        setActiveOnboardingInterface(false);
        setActiveWelcomeInterface(true);
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


    public void setActiveWelcomeInterface(bool b)
    {
        if (b)
        {
            StartCoroutine(animator.FadeIn(welcomeInterface));
            // set collidar component to true
        }
        else
        {
            StartCoroutine(animator.FadeOut(welcomeInterface));
            //welcomeInterface.SetActive(b);
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
            scanningInterface.SetActive(true);
            beginScan.SetActive(true);
            beginScan.transform.Find("PauseScanning").gameObject.SetActive(false);
            scanningActive.SetActive(true);
            VirtualHandKeyboard.SetActive(false);
            StartCoroutine(animator.FadeIn(scanningInterface));
        }
        else
        {
            beginScan.SetActive(false);
            scanningActive.SetActive(false);
            VirtualHandKeyboard.SetActive(false);
            scanningInterface.SetActive(false);
            StartCoroutine(animator.FadeOut(scanningInterface));
        }
    }

    public bool isActiveHeadLockCanvas()
    {
        return headLockCanvas.activeSelf;
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
        setActiveOnboardingInterface(true);
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
        clickCountDown = Mathf.Max(0, clickCountDown - Time.deltaTime);
        if (startCountDown)
        {
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
