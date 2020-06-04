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

    private void Awake()
    {
        controller = GameObject.Find("Onboarding").GetComponent<InterfaceController>();
        fader = GameObject.Find("FadingAnimation").GetComponent<UIFadingAnimation>();
        NIControl = GameObject.Find("HeadLockCanvas").GetComponent<NIThresholdControl>();
        buttonClip = GameObject.Find("Button_Click").GetComponent<AudioSource>();
        interfaceManager = GameObject.Find("InterfaceManager").GetComponent<InterfaceManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hand")
        {
            GetComponent<Button>().onClick.Invoke();
        }
    }

    public void clicked()
    {
        if (name == "RecipeButton")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("RecipeButton").transform.position);
            string recipe_name = transform.parent.GetComponentInChildren<TextMeshPro>().text;
            controller.loadPreview(recipe_name);
            fader.turnOnPreview();
        } else if (name == "Lock")
        {
            NIControl.changeLock();
            GameObject icon = transform.parent.Find("IconAndText").Find("Icon").gameObject;
            Renderer rend = icon.GetComponent<Renderer>();
            if (NIControl.getLock())
            {
                // assign different material
                rend.material.color = Color.red;
                //text.GetComponent<TextMeshPro>().color = Color.red;
            }
            else
            {
                rend.material.color = Color.white;
                //text.GetComponent<TextMeshPro>().color = Color.blue;
            }
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Lock").transform.position);
        } else if (name == "Simulation")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Simulation").transform.position);
            interfaceManager.setActiveNearInterface(false);
            interfaceManager.setActiveOnboardingInterface(false);
            interfaceManager.setActiveCuttingSimulation(true);
            interfaceManager.setActiveSimulationInterface(true);
        } else if (name == "RecipeButton")
        {

        }
    }
}
