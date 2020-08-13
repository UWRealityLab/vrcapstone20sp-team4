using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is for keeping and updating important states info 
// that other program will want to acquire
public class ApplicationState : MonoBehaviour
{

    private Dictionary<string, Vector3> item2Location = new Dictionary<string, Vector3>();
    private List<string> impEquipments = new List<string>() { "oven", "microwave oven", "cutting board", "sink", "gas stove" };

    // in the future, we might need 
    // list of all current animation
    // more...
    public bool contains(string name)
    {
        return item2Location.ContainsKey(name.ToLower());
    }

    // contains should be called before this
    public Vector3 getItemLocation(string name)
    {
        return item2Location[name.ToLower()];
    }

    public void setLocation(string name, Vector3 location)
    {
        if (name.ToLower().Equals("oven"))
        {
            Debug.Log("oven location updated " + location);
        }
        item2Location[name.ToLower()] = location;
       
    }

    public void Clear()
    {
        item2Location.Clear();
    }

    public Vector3 criticalEquipmentLocation(List<Equipment> equipments)
    {
        //foreach(var equip in equipments)
        //{
        //    if (impEquipments.Contains(equip.name.ToLower()))
        //    {
        //        if (item2Location.ContainsKey(equip.name.ToLower()))
        //        {
        //            return item2Location[equip.name.ToLower()];
        //        }
        //    }
        //}
        //string temp = "";
        //foreach(var s in item2Location.Keys)
        //{
        //    temp += " " + s;
           
        //}
        //Debug.Log(temp);
        if (item2Location.ContainsKey("oven"))
        {
            return item2Location["oven"];
        }
        //foreach (var equip in equipments)
        //{
                
            
        //}
        return Vector3.zero;
    }

}
