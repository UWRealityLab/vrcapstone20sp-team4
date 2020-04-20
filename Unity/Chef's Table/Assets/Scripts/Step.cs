using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step 
{
    private string identifier;
    private Time timer;
    private bool completed;
    private bool actionRequiredToProceed; // need users' input to be marked completed
    private HashSet<string> utensilSet;
    private HashSet<string> ingredientSet;



    public Step(string identifier, Time timer, bool actionRequired, HashSet<string> utensils, HashSet<string> ingredients)
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

    public Time getTime()
    {
        return timer;
    }

    public bool isCompleted()
    {
        return completed;
    }

    public bool actionRequired()
    {
        return actionRequiredToProceed;
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
