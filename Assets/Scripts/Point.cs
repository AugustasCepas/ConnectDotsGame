using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    private int x { get; set; }
    private int y { get; set; }

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public int GetX()
    {
        return x;
    }

    public void SetX(int value)
    {
        x = value;
    }
    public int GetY()
    {
        return y;
    }

    public void SetY(int value)
    {
        y = value;
    }
}