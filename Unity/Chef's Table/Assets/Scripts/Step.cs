using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Step 
{
    private string name;
    private float timer;
    private bool completed;
    private bool actionRequiredToProceed; // need users' input to be marked completed
    private List<string> utensilSet;
    private List<string> ingredientSet;



    public Step(string name, float timer, bool actionRequired, List<string> utensils, List<string> ingredients)
    {
        this.name = name;
        this.completed = false;
        this.timer = timer;
        this.actionRequiredToProceed = actionRequired;
        this.utensilSet = utensils;
        this.ingredientSet = ingredients;
    }

    public string getName()
    {
        return name;
    }

    public float getTime()
    {
        return timer;
    }

    public void setTimer(float t)
    {
        timer = t;
    }

    public bool isCompleted()
    {
        return completed;
    }

    public bool actionRequired()
    {
        return actionRequiredToProceed;
    }
    
    public void setActionRequired(bool actionRequired)
    {
        actionRequiredToProceed = actionRequired;
    }

    public List<string> getUtensilsSet()
    {
        return utensilSet;
    }

    public List<string> getIngredientsSet()
    {
        return ingredientSet;
    }

    // need to add timer callback function

}
