using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NearInterfaceButton : MonoBehaviour
{
    // Start is called before the first frame update
    MainScheduler scheduler;
    AudioSource buttonClip;
    AudioSource timerClip;
    Material normalMat;
    Material highlightMat;
    GameObject text;
    GameObject icon;

    void Start()
    {
        scheduler = GameObject.Find("Scheduler").GetComponent<MainScheduler>();
        buttonClip = GameObject.Find("Button_Click").GetComponent<AudioSource>();
        timerClip = GameObject.Find("Timer").GetComponent<AudioSource>();
        GameObject iconText = transform.parent.transform.Find("IconAndText").gameObject;
        text = iconText.transform.Find("Text").gameObject;
        icon = iconText.transform.Find("Icon").gameObject;
        normalMat = Resources.Load("Mat/BoxMat", typeof(Material)) as Material;
        highlightMat = Resources.Load("Mat/HighlightMat", typeof(Material)) as Material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hand")
        {
            GetComponent<Button>().onClick.Invoke();
            StartCoroutine(ShowFeedback());
        }
    }

    private IEnumerator ShowFeedback()
    {
        text.GetComponent<TextMeshPro>().color = Color.red;
        icon.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        transform.parent.gameObject.GetComponent<Renderer>().material = highlightMat;
        yield return new WaitForSeconds(0.5f);
        text.GetComponent<TextMeshPro>().color = Color.white;
        icon.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        transform.parent.gameObject.GetComponent<Renderer>().material = normalMat;
    }

    public void clicked()
    {
        if (name == "Next")
        {
            Debug.Log("Next Button, " + name);
            scheduler.toNextStep();
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Next").transform.position);
        }
        else if (name == "Back")
        {
            //Debug.Log("Back Button, " + name);
            scheduler.toPreviousStep();
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Back").transform.position);
        }
        else if (name == "Start")
        {
            //Debug.Log("Start Button, " + name);
            scheduler.changeTimerStatus(1);
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
        else if (name == "Plus")
        {
            scheduler.addToTimer();
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Plus").transform.position);
        }
        else if (name == "Minus")
        {
            scheduler.subtractFromTimer();
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Minus").transform.position);
        }
        else
        {
            Debug.Log("Unknown button");
        }
    }
}
