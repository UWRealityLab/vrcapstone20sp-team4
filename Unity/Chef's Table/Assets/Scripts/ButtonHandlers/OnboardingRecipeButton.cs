using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnboardingRecipeButton : MonoBehaviour
{
    public void clicked()
    {
        InterfaceController controller = GameObject.Find("OnBoardingInterface").GetComponent<InterfaceController>();
        string recipe_name = GetComponentInChildren<TextMeshProUGUI>().text;
        controller.loadPreview(recipe_name);
    }
}
