using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnboardingRecipeButton : MonoBehaviour
{
    InterfaceController controller;
    UIFadingAnimation fader;
    NIThresholdControl NIControl;
    AudioSource buttonClip;
    InterfaceManager interfaceManager;
    GameObject icon;

    private void Awake()
    {
        controller = GameObject.Find("Onboarding").GetComponent<InterfaceController>();
        fader = GameObject.Find("FadingAnimation").GetComponent<UIFadingAnimation>();
        NIControl = GameObject.Find("HeadLockCanvas").GetComponent<NIThresholdControl>();
        buttonClip = GameObject.Find("Button_Click").GetComponent<AudioSource>();
        interfaceManager = GameObject.Find("InterfaceManager").GetComponent<InterfaceManager>();
        if (name == "RecipeButton") return;
        GameObject iconText = transform.parent.transform.Find("IconAndText").gameObject;
        icon = iconText.transform.Find("Icon").gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hand" && interfaceManager.clickOk())
        {
            interfaceManager.clickButton();
            GetComponent<Button>().onClick.Invoke();
            if (name == "Simulation" || name == "RecipeButton") return;
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
        // Debug.Log("k");
        if (name == "RecipeButton")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("RecipeButton").transform.position);
            string recipe_name = transform.parent.GetComponentInChildren<TextMeshPro>().text;
            controller.loadPreview(recipe_name);
            fader.turnOnPreview();
        } else if (name == "Lock")
        {
            NIControl.changeLock();
            UpdateInGameInterface uii = GameObject.Find("InGameInterface").GetComponent<UpdateInGameInterface>();
            uii.updateLock(NIControl.getLock());
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Lock").transform.position);
        } else if (name == "Simulation")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Simulation").transform.position);
            interfaceManager.setActiveNearInterface(false);
            interfaceManager.setActiveOnboardingInterface(false);
        } else if (name == "OnBoardingBackButtonScript")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("OnBoardingBackButtonScript").transform.position);
            interfaceManager.setActiveNearInterface(false);
            interfaceManager.setActiveOnboardingInterface(false);
            interfaceManager.setActiveWelcomeInterface(true);
        }
    }
}
