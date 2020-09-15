using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is for keeping and updating important states info 
// that other program will want to acquire
public class ApplicationState : MonoBehaviour
{

    private int timeStamp = 0;
    private List<string> singletonEquipments = new List<string>() { "microwave oven", "gas stove", "oven", "frying pan", "cutting board", "kitchen knife" };
    private List<string> multiEquipments = new List<string>() { "bottle", "bowl", "plate" };
    private Dictionary<string, Vector3> singleEquipMap = new Dictionary<string, Vector3>();
    private Dictionary<string, Vector3> multiEquipMap = new Dictionary<string, Vector3>();
    private Dictionary<string, Vector3> ingredientsMap = new Dictionary<string, Vector3>();

    public bool isIngredients(string name)
    {
        return !singletonEquipments.Contains(name) && !multiEquipments.Contains(name);
    }
    
    public bool contains(string name)
    {
        return singleEquipMap.ContainsKey(name) || multiEquipMap.ContainsKey(name) || ingredientsMap.ContainsKey(name);
    }


    public void setLocation(string name, Vector3 position)
    {
        if (singletonEquipments.Contains(name))
        {
            singleEquipMap[name] = position;
        } else if (multiEquipments.Contains(name))
        {

            multiEquipMap[name] = position;
        } else
        {
            ingredientsMap[name] = position;
        }
    }



    public void Clear()
    {
        singleEquipMap.Clear();
        multiEquipMap.Clear();
        ingredientsMap.Clear();
    }

}
