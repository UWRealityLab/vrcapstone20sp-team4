using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class KeyBoardButton : MonoBehaviour
{
    InterfaceManager interfaceManager;
    private GameObject target;
    private GameObject targetPlaceholder;

    private void Awake()
    {
        GameObject.Find("Button_Click").GetComponent<AudioSource>();
        target = transform.parent.parent.Find("Input/Text").gameObject;
        targetPlaceholder = transform.parent.parent.Find("Input/Placeholder").gameObject;
        Debug.Log(target == null);
        Debug.Log(targetPlaceholder == null);
        interfaceManager = GameObject.Find("InterfaceManager").GetComponent<InterfaceManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hand" && interfaceManager.clickOk())
        {
            interfaceManager.clickButton();
            GetComponent<Button>().onClick.Invoke();
            StartCoroutine(ShowFeedback());
        }
    }

    private IEnumerator ShowFeedback()
    {
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(1.0f);
        GetComponent<Collider>().enabled = true;
    }

    public void clicked()
    {
        //AudioSource.PlayClipAtPoint(buttonClip.clip, this.transform.position);
        
        if (name == "backspace")
        {
            try {
                TextMeshPro targetText = target.GetComponent<TextMeshPro>();
                if (targetText.text.Length > 0)
                {
                    targetText.text = targetText.text.Remove(targetText.text.Length - 1);
                }
                if (targetText.text.Length == 0)
                {
                    targetPlaceholder.SetActive(true);
                    target.SetActive(false);
                }
            } catch (NullReferenceException e)
            {
                targetPlaceholder.SetActive(true);
                target.SetActive(false);
            }
        } else
        {
            targetPlaceholder.SetActive(false);
            target.SetActive(true);
            string letter = name;
            TextMeshPro targetText = target.GetComponent<TextMeshPro>();
            targetText.text = targetText.text + letter;
        }

        
    }
}
