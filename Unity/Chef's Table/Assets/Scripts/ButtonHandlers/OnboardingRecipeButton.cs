using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnboardingRecipeButton : MonoBehaviour
{
    public void clicked()
    {
        GameObject gb = GetComponent<GameObject>();
        Debug.Log("Button Clicked");

    }
}
