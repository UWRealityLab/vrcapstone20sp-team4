using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// saves ingredient list regardless of state the game scene is at
public class IngredientListScript : MonoBehaviour
{
    private List<string> ingredientList = new List<string>();
    public string get(int index)
    {
        return ingredientList[index];
    }

    public void addToList(string item)
    {
        ingredientList.Add(item);
    }

    public void removeFromList(string item)
    {
        ingredientList.Remove(item);
    }

    public int count()
    {
        return ingredientList.Count;
    }

    public string[] array()
    {
        return ingredientList.ToArray();
    }

    public void distinct()
    {
        ingredientList = ingredientList.Distinct().ToList();
    }
}
