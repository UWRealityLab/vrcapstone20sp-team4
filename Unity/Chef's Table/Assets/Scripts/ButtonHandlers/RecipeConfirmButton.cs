using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecipeConfirmButton : MonoBehaviour
{
    public void clicked()
    {
        string recipe_name = transform.parent.GetComponentInChildren<TextMeshProUGUI>().text;
        GameObject sche = GameObject.Find("Scheduler");
        MainScheduler ms = sche.GetComponent<MainScheduler>();
        ms.startTutorial(recipe_name);
        GameObject obinterface = GameObject.Find("OnBoardingInterface");
        obinterface.SetActive(false);
        Debug.Log(recipe_name);
    }
}
