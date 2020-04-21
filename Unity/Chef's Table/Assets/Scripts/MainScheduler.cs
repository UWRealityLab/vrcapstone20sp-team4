using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class MainScheduler : MonoBehaviour
{
    // Start is called before the first frame update

    private List<HashSet<Step>> tutorial = new List<HashSet<Step>>();

    // initialize the scheduler with hardcoded tutorial
    void hardcodeInit()
    {
        //HashSet<string> utensils = new HashSet<string>();
        //utensils.Add("Pot1");
        //utensils.Add("Microwave");
        //HashSet<string> ingredients = new HashSet<string>();
        //ingredients.Add("water");
        //ingredients.Add("bread");
        //Step s = new Step("step1", new Timer(5000), true, utensils, ingredients);
        //HashSet<Step> set = new HashSet<Step>();
        //set.Add(s);
        //tutorial.Add(set);
    }

    void Start()
    {
        hardcodeInit();
        InvokeRepeating("UpdateScheduler", 0f, 0.5f);
        
    }

    // Update is called once per frame
    void UpdateScheduler()
    {

    }
}
