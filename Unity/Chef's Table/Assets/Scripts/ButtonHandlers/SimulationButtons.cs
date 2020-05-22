using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimulationButtons : MonoBehaviour
{
    GameObject onboardingInterface;
    AudioSource buttonClip;
    changeSimulation changeSimulationScript;
    GameObject cuttingSimulation;
    GameObject simulationInterface;
    GameObject text;
    GameObject icon;

    // Start is called before the first frame update
    void Start()
    {
        buttonClip = GameObject.Find("Button_Click").GetComponent<AudioSource>();
        changeSimulationScript = GameObject.Find("CuttingSimulation").GetComponent<changeSimulation>();
        onboardingInterface = GameObject.Find("OnBoardingInterface");
        cuttingSimulation = GameObject.Find("CuttingSimulation");
        simulationInterface = GameObject.Find("SimulationInterface");
        buttonClip = GameObject.Find("Button_Click").GetComponent<AudioSource>();
        GameObject iconText = transform.parent.transform.Find("IconAndText").gameObject;
        text = iconText.transform.Find("Text").gameObject;
        icon = iconText.transform.Find("Icon").gameObject;
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
        //transform.parent.gameObject.GetComponent<Renderer>().material = highlightMat;
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(1.0f);
        GetComponent<Collider>().enabled = true;
        text.GetComponent<TextMeshPro>().color = Color.white;
        icon.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        //transform.parent.gameObject.GetComponent<Renderer>().material = normalMat;
        if (name == "Exit")
        {
            onboardingInterface.SetActive(true);
            cuttingSimulation.SetActive(false);
            simulationInterface.SetActive(false);
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
