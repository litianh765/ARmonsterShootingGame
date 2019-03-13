using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGrid {
    public bool IsOccupy { get; set; }

    public float Z { get;private set; }

    public float Y { get;private set; }

    public float X { get;private set; }


    //define the position of grid
    public SpawnGrid(float x, float y,float z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }
    
    public Vector3 GetPos()
    {
        return new Vector3(X, Y, Z);
    }
}
