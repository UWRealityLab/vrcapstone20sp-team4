using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient
{
    public string name;
    public int quantity;
    public string unit;
    private Vector3 postion;

    public Ingredient(string name, int quantity, string unit)
    {
        this.name = name;
        this.quantity = quantity;
        this.unit = unit;
    }

    public void setPosition(Vector3 pos)
    {
        postion = pos;
    }

    public Vector3 getPosition()
    {
        return postion;
    }
}
