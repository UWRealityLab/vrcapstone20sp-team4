using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient
{
    public string name;
    private Vector3 postion;

    public void setPosition(Vector3 pos)
    {
        postion = pos;
    }

    public Vector3 getPosition()
    {
        return postion;
    }
}
