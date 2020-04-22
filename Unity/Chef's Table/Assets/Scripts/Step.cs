using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Step 
{
    private string identifier;
    private float timer;
    private bool completed;
    private bool actionRequiredToProceed; // need users' input to be marked completed
    private HashSet<string> utensilSet;
    private HashSet<string> ingredientSet;



    public Step(string identifier, float timer, bool actionRequired, HashSet<string> utensils, HashSet<string> ingredients)
    {
        this.identifier = identifier;
        this.completed = false;
        this.timer = timer;
        this.actionRequiredToProceed = actionRequired;
        this.utensilSet = utensils;
        this.ingredientSet = ingredients;
    }

    public string getIdentifier()
    {
        return identifier;
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

    public HashSet<string> getUtensilsSet()
    {
        return utensilSet;
    }

    public HashSet<string> getIngredientsSet()
    {
        return ingredientSet;
    }

    // need to add timer callback function

}
