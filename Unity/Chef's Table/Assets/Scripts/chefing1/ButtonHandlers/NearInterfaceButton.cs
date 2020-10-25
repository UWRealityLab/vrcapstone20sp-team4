using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;
using UnityEngine.Video;

public class NearInterfaceButton : MonoBehaviour
{
    // Start is called before the first frame update
    MainScheduler2 scheduler;
    AudioSource buttonClip;
    AudioSource timerClip;
    GameObject text;
    GameObject icon;
    GameObject timerIcon;
    NIThresholdControl NIControl;
    InterfaceManager interfaceManager;
    
    public GameObject visualCueDisplayContainer;

    private bool timerPaused = true;
    private GameObject playButton;
    private Material startButton;
    private Material pauseButton;
    //private GameObject particles;
    private ApplicationState appState;
   // private VideoPlayer videoPlayer;

    void Awake()
    {
        scheduler = GameObject.Find("Scheduler").GetComponent<MainScheduler2>();
        interfaceManager = GameObject.Find("InterfaceManager").GetComponent<InterfaceManager>();
        buttonClip = GameObject.Find("Button_Click").GetComponent<AudioSource>();
        timerClip = GameObject.Find("Timer").GetComponent<AudioSource>();
        GameObject iconText = transform.parent.transform.Find("IconAndText").gameObject;
        icon = iconText.transform.Find("Icon").gameObject;
        timerIcon = GameObject.Find("InterfaceTimer/PlayButton/IconAndText/Icon");
        NIControl = GameObject.Find("HeadLockCanvas").GetComponent<NIThresholdControl>();
        playButton = GameObject.Find("PlayButton").transform.Find("Start").gameObject;
        startButton = Resources.Load("Mat/ButtonStartMat", typeof(Material)) as Material;
        pauseButton = Resources.Load("Mat/ButtonPauseMat", typeof(Material)) as Material;
        //particles = GameObject.Find("particles");
        appState = GameObject.Find("ApplicationState").GetComponent<ApplicationState>();
        //videoPlayer = GameObject.Find("NearInterface/GameInterfaceScreen/InterfaceScreen").GetComponent<VideoPlayer>();
        // visualCueDisplayContainer = GameObject.Find("visualCueDisplayContainer");
        if (visualCueDisplayContainer != null) {
            visualCueDisplayContainer.SetActive(false);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hand" && interfaceManager.clickOk())
        {
            interfaceManager.clickButton();
            GetComponent<Button>().onClick.Invoke();
            if (name == "Start") return;
            if (name == "Exit")
            {
                interfaceManager.endTutorialGeneral();
                return;
            }
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
        if (name == "Next")
        {
            // Debug.Log("Next Button, " + name);
            scheduler.toNextStep();
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Next").transform.position);
            timerIcon.GetComponent<Renderer>().material = startButton;
            // particles.SetActive(false);
            //appState.clearMaps();
            //videoPlayer.transform.position = new Vector3(0f, -0.15f, 0f);
        }
        else if (name == "Back")
        {
            // Debug.Log("Back Button, " + name);
            scheduler.toPreviousStep();
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Back").transform.position);
            timerIcon.GetComponent<Renderer>().material = startButton;
            // particles.SetActive(false);
            //appState.clearMaps();
            //videoPlayer.transform.position = new Vector3(0f, -0.15f, 0f);
        }
        else if (name == "Start")
        {
            if (timerPaused)
            {
                scheduler.changeTimerStatus(1);
            } else
            {
                scheduler.changeTimerStatus(0);
            }
            timerPaused = !timerPaused;
            icon.GetComponent<Renderer>().material = timerPaused ? startButton : pauseButton;
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
        else if (name == "Lock")
        {
            NIControl.changeLock();
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Lock").transform.position);
        }
        else if (name == "Exit")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Exit").transform.position);
        }
        else if (name == "CloseVideo") {
            visualCueDisplayContainer.SetActive(false);
            AudioSource.PlayClipAtPoint(buttonClip.clip, transform.position);
        }
        else
        {
            Debug.Log("Unknown button");
        }
    }

}
