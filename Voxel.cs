using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Voxel
{
    public Color color;
    public float weight;
    public bool surface; //determines type

    public Voxel()
    {
        color = Color.grey;
        weight = 0.1f;
    }

    public Voxel(bool surface, Color color)
    {
        this.color = color;
        this.surface = surface;
        weight = 0.1f;
    }

    public Voxel(bool surface)
    {
        this.surface = surface;
        color = Color.grey;
        weight = 0.1f;
    }
    
    private float incStep = 0.1f;
    public void incrementWeight()
    {
       
        if (weight + incStep <= 1.0f)
            weight += incStep;
        else
            weight = 1f;
    }

    private float decStep = 0.1f;
    public void decrementWeight()
    {
        if (weight - decStep > 0)
            weight -= decStep;
        else
            weight = 0f;
    }

}
