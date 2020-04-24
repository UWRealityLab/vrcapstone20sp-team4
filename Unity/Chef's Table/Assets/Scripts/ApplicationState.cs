using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is for keeping and updating important states info 
// that other program will want to acquire
public class ApplicationState : MonoBehaviour
{
    
    public Dictionary<string, Utensil> name2Utensil = new Dictionary<string, Utensil>(); 
    public Dictionary<string, Ingredient> name2Ingredient = new Dictionary<string, Ingredient>();
    // in the future, we might need 
    // list of all current animation
    // more...

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
