using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utensil
{
    public string name;
    private bool isOccupied;
    private Vector3 postion;

    public Utensil(string name)
    {
        this.name = name;
        this.isOccupied = false;
        this.postion = new Vector3(0, 0, 0);
    }

    public void setState(bool state)
    {
        isOccupied = state;
    }

    public bool getState()
    {
        return isOccupied;
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
