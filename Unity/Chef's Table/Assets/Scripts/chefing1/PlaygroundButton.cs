using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaygroundButton : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject onboardingInterface;
    public GameObject sliceManager;
    public GameObject controller;
    public GameObject choppingBoard;
    public GameObject SlicedObjects;
    public GameObject HandTracking;
    public GameObject HandPointer;
    private int mode = 0; // 1 for simulation, 0 for normal
    public void clicked()
    {
        Debug.Log("playground button clicked");
        if (mode == 0) // switching to 1
        {
            onboardingInterface.SetActive(false);
            HandPointer.SetActive(false);
            HandTracking.SetActive(false);
            SlicedObjects.SetActive(true);
            choppingBoard.SetActive(true);
            sliceManager.SetActive(true);
            controller.SetActive(true);
            mode = 1;
        } else
        {
            onboardingInterface.SetActive(true);
            HandPointer.SetActive(true);
            HandTracking.SetActive(true);
            SlicedObjects.SetActive(false);
            choppingBoard.SetActive(false);
            sliceManager.SetActive(false);
            controller.SetActive(false);
            mode = 0;
        }
    }
}
