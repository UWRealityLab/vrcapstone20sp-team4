using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimulationButtons : MonoBehaviour
{
    AudioSource buttonClip;
    changeSimulation changeSimulationScript;
    GameObject text;
    GameObject icon;
    InterfaceManager interfaceManager;

    // Start is called before the first frame update
    void Start()
    {
        buttonClip = GameObject.Find("Button_Click").GetComponent<AudioSource>();
        changeSimulationScript = GameObject.Find("CuttingSimulation").GetComponent<changeSimulation>();
        buttonClip = GameObject.Find("Button_Click").GetComponent<AudioSource>();
        GameObject iconText = transform.parent.transform.Find("IconAndText").gameObject;
        text = iconText.transform.Find("Text").gameObject;
        icon = iconText.transform.Find("Icon").gameObject;
        interfaceManager = GameObject.Find("InterfaceManager").GetComponent<InterfaceManager>();
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
        if (name != "Lock")
        {
            text.GetComponent<TextMeshPro>().color = Color.red;
            icon.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        }
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(1.0f);
        GetComponent<Collider>().enabled = true;
        if (name != "Lock")
        {
            text.GetComponent<TextMeshPro>().color = Color.white;
            icon.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        }
        if (name == "Exit")
        {
            interfaceManager.setActiveOnboardingInterface(true);
        }
    }


    // Update is called once per frame
    public void clicked()
    {
        if (name == "Next")
        {
            Debug.Log("Next Button, " + name);
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Next").transform.position);
            changeSimulationScript.nextObject();
        }
        else if (name == "Back")
        {
            //Debug.Log("Back Button, " + name);
            changeSimulationScript.previousObject();
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Back").transform.position);
        }
        else if (name == "Reset")
        {
            //Debug.Log("Reset Button, " + name);
            changeSimulationScript.resetObject();
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Reset").transform.position);
        }
        else if (name == "Exit")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Exit").transform.position);
        }
        else
        {
            Debug.Log("Unknown button");
        }
    }
}
